using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls; // Agregado para usar TextBlock
using Wpf.Ui.Controls; // Controles modernos WinUI 3

namespace AntivirusUI
{
    public partial class MainWindow : FluentWindow
    {
        // =====================================================================
        // IMPORTACIONES DEL MOTOR EN ASSEMBLY (P/Invoke)
        // =====================================================================

        [DllImport("MotorAntivirus.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int EscanearArchivo(string ruta);

        [DllImport("MotorAntivirus.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int DestruirArchivo(string ruta);

        public MainWindow()
        {
            InitializeComponent();
        }

        // =====================================================================
        // LÓGICA DE LA INTERFAZ Y ESCANEO
        // =====================================================================

        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog
            {
                Title = "Selecciona la carpeta a escanear"
            };

            if (dialog.ShowDialog() == true)
            {
                txtRuta.Text = dialog.FolderName;
                lstResultados.Items.Clear();
            }
        }

        private async void BtnEscanear_Click(object sender, RoutedEventArgs e)
        {
            string rutaBase = txtRuta.Text;

            if (string.IsNullOrEmpty(rutaBase) || !Directory.Exists(rutaBase))
            {
                var msgBox = new Wpf.Ui.Controls.MessageBox
                {
                    Owner = this,
                    Title = "Virus Removal Tool",
                    Content = new Wpf.Ui.Controls.TextBlock
                    {
                        Text = "Por favor, selecciona una carpeta válida antes de iniciar el escaneo.",
                        TextAlignment = TextAlignment.Left,
                        TextWrapping = TextWrapping.Wrap
                    },
                    PrimaryButtonText = "Seleccionar carpeta", // Botón útil (Acción principal)
                    CloseButtonText = "¡Entendido!",           // Botón para simplemente cerrar

                    // Bloqueo estricto de redimensión:
                    Width = 450,
                    MinWidth = 450,
                    MaxWidth = 450,
                    Height = 160,
                    MinHeight = 160,
                    MaxHeight = 160
                };

                var result = await msgBox.ShowDialogAsync();

                if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
                {
                    // 1. Abrimos la ventana de elegir carpeta reutilizando tu método
                    BtnSeleccionar_Click(sender, e);

                    // 2. Refrescamos nuestra variable con lo que el usuario eligió
                    rutaBase = txtRuta.Text;

                    // 3. Verificamos si el usuario de verdad eligió algo o si canceló la ventana
                    if (string.IsNullOrEmpty(rutaBase) || !Directory.Exists(rutaBase))
                    {
                        return; // Abortar: Canceló el explorador de archivos
                    }
                }
                else
                {
                    return; // Abortar: Presionó "¡Entendido!" o cerró el MessageBox
                }
            }

            // =========================================================
            // Si el código llega hasta aquí, ES SEGURO que tenemos una ruta válida
            // (ya sea porque la puso al inicio o porque la acaba de seleccionar).
            // Comenzamos el escaneo inmediatamente.
            // =========================================================

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

        private async void BtnDestruir_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Seleccione un archivo para DESTRUIR desde Assembly",
                Filter = "Todos los archivos (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                string archivo = dialog.FileName;

                var msgBox = new Wpf.Ui.Controls.MessageBox
                {
                    Owner = this,
                    Title = "Advertencia de Seguridad",
                    Content = new Wpf.Ui.Controls.TextBlock
                    {
                        Text = $"El motor de Assembly sobrescribirá con ceros y eliminará permanentemente este archivo:\n\n{archivo}",
                        TextAlignment = TextAlignment.Left,
                        TextWrapping = TextWrapping.Wrap
                    },
                    PrimaryButtonText = "Sí, destruir archivo",
                    CloseButtonText = "Cancelar",
                    PrimaryButtonAppearance = ControlAppearance.Danger,
                    // Bloqueo estricto de redimensión:
                    Width = 500,
                    MinWidth = 500,
                    MaxWidth = 500,
                    Height = 215,
                    MinHeight = 215,
                    MaxHeight = 215
                };

                var result = await msgBox.ShowDialogAsync();

                if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
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