using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using ICSharpCode.XamlDesigner.Configuration;
using System.Windows.Threading;
using System.Diagnostics;

namespace ICSharpCode.XamlDesigner
{
	public partial class App
	{
		public static string[] Args;

        protected override void OnStartup(StartupEventArgs e)
        {
			DispatcherUnhandledException += App_DispatcherUnhandledException;
			Args = e.Args;
            base.OnStartup(e);
        }

		void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Shell.ReportException(e.Exception);
			e.Handled = true;
		}

        protected override void OnExit(ExitEventArgs e)
        {            
            Settings.Default.Save();
            base.OnExit(e);
        }
	}
}
