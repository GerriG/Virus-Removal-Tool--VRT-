using System.Windows;

namespace AntivirusUI
{
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // 1. Habilitar estilos visuales (Soluciona los iconos planos/viejos)
            System.Windows.Forms.Application.EnableVisualStyles();

            // 2. Mejorar renderizado de texto
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            // 3. Forzar procesamiento de mensajes de Windows
            System.Windows.Forms.Application.DoEvents();

            base.OnStartup(e);
        }
    }
}