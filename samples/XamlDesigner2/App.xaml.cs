using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using SharpDevelop.Samples.XamlDesigner.Properties;
using System.Windows.Threading;
using System.Diagnostics;

namespace SharpDevelop.Samples.XamlDesigner
{
	public partial class App : Application
	{
		public static string[] Args;

        protected override void OnStartup(StartupEventArgs e)
        {
			Args = e.Args;
			DispatcherUnhandledException += App_DispatcherUnhandledException;			
			System.Windows.Forms.Application.EnableVisualStyles();			
            base.OnStartup(e);
        }

		void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			if (!Debugger.IsAttached && Shell.Instance.CurrentDocument != null) {
				Shell.Instance.CurrentDocument.Context.DesignView.Exception = e.Exception;
				e.Handled = true;
			}
		}

        protected override void OnExit(ExitEventArgs e)
        {            
            Settings.Default.Save();
            base.OnExit(e);
        }
	}
}
