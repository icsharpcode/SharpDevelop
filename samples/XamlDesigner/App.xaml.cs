using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.XamlDesigner.Configuration;

namespace ICSharpCode.XamlDesigner
{
	public partial class App
	{
		public static string[] Args;

		protected override void OnStartup(StartupEventArgs e)
		{
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AppDomain_CurrentDomain_AssemblyResolve);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomain_CurrentDomain_UnhandledException);
			DragDropExceptionHandler.UnhandledException += new ThreadExceptionEventHandler(DragDropExceptionHandler_UnhandledException);
			DispatcherUnhandledException += App_DispatcherUnhandledException;
			Args = e.Args;
			base.OnStartup(e);
		}

		private static bool internalLoad = false;
		private static string lastRequesting = null;
		
		Assembly AppDomain_CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			if (internalLoad)
				return null;
			
			if (args.Name.Split(new [] { ',' })[0].Trim().EndsWith(".resources"))
				return null;
			
			internalLoad = true;
			
			Assembly ass = null;
			try {
				
				ass = Assembly.Load(args.Name);
			}
			catch (Exception) { }
			
			if (ass == null && args.RequestingAssembly != null) {
				lastRequesting = args.RequestingAssembly.Location;
				var dir = Path.GetDirectoryName(args.RequestingAssembly.Location);
				var file = args.Name.Split(new [] { ',' })[0].Trim() + ".dll";
				try {
					ass = Assembly.LoadFrom(Path.Combine(dir, file));
				}
				catch (Exception) { }
			}
			else if (lastRequesting != null) {
				var dir = Path.GetDirectoryName(lastRequesting);
				var file = args.Name.Split(new [] { ',' })[0].Trim() + ".dll";
				try {
					ass = Assembly.LoadFrom(Path.Combine(dir, file));
				}
				catch (Exception) { }
			}
			
			internalLoad = false;
			
			return ass;
		}
		
		void DragDropExceptionHandler_UnhandledException(object sender, ThreadExceptionEventArgs e)
		{
			Shell.ReportException(e.Exception);
			
		}

		void AppDomain_CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Shell.ReportException(e.ExceptionObject as Exception);
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
