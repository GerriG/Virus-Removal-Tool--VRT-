using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace AntivirusUI
{
    public partial class MainWindow : Window
    {
        // =====================================================================
        // IMPORTACIONES DEL MOTOR EN ASSEMBLY
        // =====================================================================

        [DllImport("MotorAntivirus.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int EscanearArchivo(string ruta);

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
            // FIX DEL CRASH: Encendemos el motor visual para TaskDialog
            System.Windows.Forms.Application.EnableVisualStyles();

            InitializeComponent();
            this.SourceInitialized += MainWindow_SourceInitialized;
        }

        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            int trueValue = 1;
            DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref trueValue, Marshal.SizeOf(typeof(int)));

            int backdropType = 2; // 2 = Mica, 3 = Acrílico
            DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref backdropType, Marshal.SizeOf(typeof(int)));

            HwndSource source = HwndSource.FromHwnd(hwnd);
            if (source != null && source.CompositionTarget != null)
            {
                source.CompositionTarget.BackgroundColor = System.Windows.Media.Colors.Transparent;
            }
        }

        // =====================================================================
        // LÓGICA DE LA INTERFAZ Y ESCANEO
        // =====================================================================

        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtRuta.Text = dialog.SelectedPath;
                lstResultados.Items.Clear();
            }
        }

        private async void BtnEscanear_Click(object sender, RoutedEventArgs e)
        {
            string rutaBase = txtRuta.Text;

            if (string.IsNullOrEmpty(rutaBase) || !Directory.Exists(rutaBase))
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                var page = new System.Windows.Forms.TaskDialogPage()
                {
                    Caption = "Motor Antivirus",
                    Heading = "Acción Requerida",
                    Text = "Por favor, selecciona una carpeta válida antes de iniciar el escaneo.",
                    Icon = System.Windows.Forms.TaskDialogIcon.Information
                };
                System.Windows.Forms.TaskDialog.ShowDialog(hwnd, page);
                return;
            }

            btnEscanear.IsEnabled = false;
            lstResultados.Items.Add($"[*] Iniciando escaneo en: {rutaBase}");

            await Task.Run(() => ProcesarDirectorio(rutaBase));

            lstResultados.Dispatcher.Invoke(() => lstResultados.Items.Add($"[*] Escaneo finalizado."));
            btnEscanear.IsEnabled = true;
        }

        private void ProcesarDirectorio(string rutaDirectorio)
        {
            try
            {
                string[] archivos = Directory.GetFiles(rutaDirectorio);

                foreach (string archivo in archivos)
                {
                    int resultado = EscanearArchivo(archivo);

                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (resultado == 1)
                        {
                            lstResultados.Items.Add($"[!!!] AMENAZA ENCONTRADA: {archivo}");
                            NeutralizarAmenaza(archivo);
                        }
                        else if (resultado == 0)
                        {
                            lstResultados.Items.Add($"[OK] Archivo limpio: {Path.GetFileName(archivo)}");
                        }
                        else
                        {
                            lstResultados.Items.Add($"[?] No se pudo leer: {Path.GetFileName(archivo)}");
                        }

                        lstResultados.ScrollIntoView(lstResultados.Items[lstResultados.Items.Count - 1]);
                    });
                }

                string[] subdirectorios = Directory.GetDirectories(rutaDirectorio);
                foreach (string subDir in subdirectorios)
                {
                    ProcesarDirectorio(subDir);
                }
            }
            catch (UnauthorizedAccessException)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    lstResultados.Items.Add($"[Bloqueado] Acceso denegado a la carpeta: {rutaDirectorio}"));
            }
        }

        private void NeutralizarAmenaza(string rutaArchivo)
        {
            try
            {
                File.Delete(rutaArchivo);
                lstResultados.Items.Add($"[-] Acción: Archivo malicioso eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                lstResultados.Items.Add($"[X] Error al eliminar amenaza: {ex.Message}");
            }
        }

        private void BtnDestruir_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "Seleccione un archivo para DESTRUIR desde Assembly";
            dialog.Filter = "Todos los archivos (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                string archivo = dialog.FileName;

                IntPtr hwnd = new WindowInteropHelper(this).Handle;

                var btnDestruirArchivo = new System.Windows.Forms.TaskDialogButton("Sí, destruir archivo");
                var btnCancelar = System.Windows.Forms.TaskDialogButton.Cancel;

                var page = new System.Windows.Forms.TaskDialogPage()
                {
                    Caption = "Advertencia de Seguridad",
                    Heading = "Operación Irreversible",
                    Text = $"El motor de Assembly sobrescribirá con ceros y eliminará permanentemente este archivo:\n\n{archivo}",

                    // Cambia esto para usar el escudo de advertencia moderno
                    Icon = System.Windows.Forms.TaskDialogIcon.ShieldWarningYellowBar,

                    Buttons = { btnCancelar, btnDestruirArchivo },
                    DefaultButton = btnCancelar
                };

                var result = System.Windows.Forms.TaskDialog.ShowDialog(hwnd, page);

                if (result == btnDestruirArchivo)
                {
                    DestruirArchivoSeguro(archivo);
                }
            }
        }

        private void DestruirArchivoSeguro(string ruta)
        {
            try
            {
                int resultado = DestruirArchivo(ruta);

                if (resultado == 1)
                {
                    lstResultados.Items.Add($"[DESTRUIDO x64] Archivo aniquilado por Assembly: {Path.GetFileName(ruta)}");
                }
                else
                {
                    lstResultados.Items.Add($"[ERROR x64] Assembly no pudo destruir el archivo. Verifica permisos.");
                }

                lstResultados.ScrollIntoView(lstResultados.Items[lstResultados.Items.Count - 1]);
            }
            catch (Exception ex)
            {
                lstResultados.Items.Add($"[EXCEPCIÓN] Error al invocar el motor Assembly: {ex.Message}");
            }
        }
    }
}