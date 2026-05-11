using System;                           // Necesario para manejar "Exception"
using System.IO;                        // Necesario para Directory, File y System.IO.Path
using System.Runtime.InteropServices;   // Necesario para DllImport
using System.Threading.Tasks;           // Necesario para usar "await Task.Run"
using System.Windows;
using System.Windows.Interop;           // <-- NUEVO: Necesario para obtener el HWND de la ventana

namespace AntivirusUI
{
    public partial class MainWindow : Window
    {
        // =====================================================================
        // IMPORTACIONES DEL MOTOR EN ASSEMBLY
        // =====================================================================

        // Al poner solo el nombre, Windows buscará automáticamente MotorAntivirus.dll
        // en la misma carpeta donde se esté ejecutando AntivirusUI.exe
        [DllImport("MotorAntivirus.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int EscanearArchivo(string ruta);

        // IMPORTAMOS LA FUNCIÓN DE DESTRUCCIÓN DESDE LA DLL DE C++
        [DllImport("MotorAntivirus.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int DestruirArchivo(string ruta);

        // =====================================================================
        // IMPORTACIONES PARA EL DISEÑO MICA / ACRÍLICO (WINDOWS 11)
        // =====================================================================

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;

        public MainWindow()
        {
            InitializeComponent();

            // Suscribimos un evento para cuando la ventana se conecte al sistema operativo
            this.SourceInitialized += MainWindow_SourceInitialized;
        }

        // Este evento se dispara justo antes de que la ventana se dibuje
        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            // Obtenemos el identificador nativo (Handle) de nuestra ventana de WPF
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            // 1. Forzar el Modo Oscuro en la barra de título de Windows
            int trueValue = 1;
            DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref trueValue, Marshal.SizeOf(typeof(int)));

            // 2. Activar el efecto translúcido (Backdrop)
            // Valores: 2 = Mica (Por defecto Win 11), 3 = Acrílico (Más difuminado), 4 = Mica Alt
            int backdropType = 2; // Puedes cambiarlo a 3 si prefieres Acrílico
            DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref backdropType, Marshal.SizeOf(typeof(int)));
        }

        // =====================================================================
        // LÓGICA DE LA INTERFAZ Y ESCANEO
        // =====================================================================

        // 2. BOTÓN PARA SELECCIONAR CARPETA
        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e)
        {
            // Usamos un truco nativo de Windows para no instalar librerías extra para el diálogo de carpetas
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtRuta.Text = dialog.SelectedPath;
                lstResultados.Items.Clear();
            }
        }

        // 3. BOTÓN PARA INICIAR EL ESCANEO
        private async void BtnEscanear_Click(object sender, RoutedEventArgs e)
        {
            string rutaBase = txtRuta.Text;

            if (string.IsNullOrEmpty(rutaBase) || !Directory.Exists(rutaBase))
            {
                System.Windows.MessageBox.Show("Por favor, selecciona una carpeta válida primero.");
                return;
            }

            btnEscanear.IsEnabled = false;
            lstResultados.Items.Add($"[*] Iniciando escaneo en: {rutaBase}");

            // Usamos Task.Run para que la interfaz gráfica no se congele durante el escaneo
            await Task.Run(() => ProcesarDirectorio(rutaBase));

            lstResultados.Dispatcher.Invoke(() => lstResultados.Items.Add($"[*] Escaneo finalizado."));
            btnEscanear.IsEnabled = true;
        }

        // 4. LÓGICA DE ESCANEO Y RECURSIVIDAD
        private void ProcesarDirectorio(string rutaDirectorio)
        {
            try
            {
                // Obtenemos todos los archivos de esta carpeta
                string[] archivos = Directory.GetFiles(rutaDirectorio);

                foreach (string archivo in archivos)
                {
                    // Llamada P/Invoke a tu código Assembly
                    int resultado = EscanearArchivo(archivo);

                    // Actualizamos la UI desde un hilo secundario
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (resultado == 1) // VIRUS DETECTADO
                        {
                            lstResultados.Items.Add($"[!!!] AMENAZA ENCONTRADA: {archivo}");
                            NeutralizarAmenaza(archivo);
                        }
                        else if (resultado == 0) // LIMPIO
                        {
                            lstResultados.Items.Add($"[OK] Archivo limpio: {Path.GetFileName(archivo)}");
                        }
                        else // ERROR (Ej. Archivo bloqueado por Windows)
                        {
                            lstResultados.Items.Add($"[?] No se pudo leer: {Path.GetFileName(archivo)}");
                        }

                        // Auto-scroll al último elemento
                        lstResultados.ScrollIntoView(lstResultados.Items[lstResultados.Items.Count - 1]);
                    });
                }

                // Escanear subcarpetas (Recursividad)
                string[] subdirectorios = Directory.GetDirectories(rutaDirectorio);
                foreach (string subDir in subdirectorios)
                {
                    ProcesarDirectorio(subDir);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Es normal que Windows nos deniegue acceso a carpetas del sistema
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    lstResultados.Items.Add($"[Bloqueado] Acceso denegado a la carpeta: {rutaDirectorio}"));
            }
        }

        // 5. FUNCIÓN ANTIVIRUS: ELIMINAR/CUARENTENA
        private void NeutralizarAmenaza(string rutaArchivo)
        {
            try
            {
                // En un AV real esto movería el archivo a una carpeta encriptada (Cuarentena).
                // Para tu proyecto, la eliminación directa es suficiente demostración.
                File.Delete(rutaArchivo);
                lstResultados.Items.Add($"[-] Acción: Archivo malicioso eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                lstResultados.Items.Add($"[X] Error al eliminar amenaza: {ex.Message}");
            }
        }

        // 6. FUNCIÓN ANTIVIRUS: DESTRUIR ARCHIVOS
        private void BtnDestruir_Click(object sender, RoutedEventArgs e)
        {
            // Usamos el diálogo nativo de WPF para seleccionar un archivo
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "Seleccione un archivo para DESTRUIR desde Assembly";
            dialog.Filter = "Todos los archivos (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                string archivo = dialog.FileName;

                // Confirmación de seguridad
                MessageBoxResult result = System.Windows.MessageBox.Show(
                    $"El motor de Assembly sobrescribirá con ceros y eliminará este archivo permanentemente:\n\n{archivo}",
                    "Operación Irreversible",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    DestruirArchivoSeguro(archivo);
                }
            }
        }

        // Lógica de trituración (Shredding) delegada al motor Assembly
        private void DestruirArchivoSeguro(string ruta)
        {
            try
            {
                // Llamamos directamente a la función exportada desde destructor.asm
                int resultado = DestruirArchivo(ruta);

                if (resultado == 1)
                {
                    // Éxito: El assembly devolvió 1
                    lstResultados.Items.Add($"[DESTRUIDO] Archivo aniquilado por Assembly: {Path.GetFileName(ruta)}");
                }
                else
                {
                    // Fallo: El assembly devolvió 0 (Probablemente por falta de permisos o archivo en uso)
                    lstResultados.Items.Add($"[ERROR] Assembly no pudo destruir el archivo. Verifica permisos.");
                }

                // Auto-scroll al último elemento agregado
                lstResultados.ScrollIntoView(lstResultados.Items[lstResultados.Items.Count - 1]);
            }
            catch (Exception ex)
            {
                lstResultados.Items.Add($"[EXCEPCIÓN] Error al invocar el motor Assembly: {ex.Message}");
            }
        }
    }
}