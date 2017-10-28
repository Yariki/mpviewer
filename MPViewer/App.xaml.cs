using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MPViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private MPBootstraper _bootstraper;

        protected override void OnStartup(StartupEventArgs e)
        {
            _bootstraper = new MPBootstraper();
            _bootstraper.Run();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _bootstraper?.Exit();
            base.OnExit(e);
        }
    }
}
