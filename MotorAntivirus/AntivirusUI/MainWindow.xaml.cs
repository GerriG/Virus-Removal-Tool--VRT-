using System;                           // Necesario para manejar "Exception"
using System.IO;                        // Necesario para Directory, File y System.IO.Path
using System.Runtime.InteropServices;   // Necesario para DllImport
using System.Threading.Tasks;           // Necesario para usar "await Task.Run"
using System.Windows;



namespace AntivirusUI
{
    public partial class MainWindow : Window
    {
        // 1. IMPORTAMOS EL MOTOR EN ASSEMBLY
        // Asegúrate de que MotorAntivirus.dll esté en la misma carpeta que AntivirusUI.exe al compilar
        // PON LA RUTA EXACTA DE TU CARPETA AQUÍ Y NO OLVIDES EL \MotorAntivirus.dll AL FINAL
        [DllImport(@"C:\Users\adona\source\repos\MotorAntivirus\x64\Debug\MotorAntivirus.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int EscanearArchivo(string ruta);

        public MainWindow()
        {
            InitializeComponent();
        }

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
    }
}