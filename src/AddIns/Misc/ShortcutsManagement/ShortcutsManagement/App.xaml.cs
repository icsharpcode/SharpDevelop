using System.Windows;
using ICSharpCode.Core;

namespace ICSharpCode.ShortcutsManagement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() {
            Startup += App_Startup;
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            PropertyService.InitializeService(".", ".", ".");
            PropertyService.Load();

            ResourceService.InitializeService(FileUtility.Combine(PropertyService.DataDirectory, "resources"));
        }
    }
}
