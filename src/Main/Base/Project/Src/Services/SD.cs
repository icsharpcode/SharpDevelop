// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design;
using ICSharpCode.Core;
using ICSharpCode.Core.Implementation;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Static entry point for retrieving SharpDevelop services.
	/// </summary>
	public static class SD
	{
		/// <summary>
		/// Gets the main service container for SharpDevelop.
		/// </summary>
		public static IServiceContainer Services {
			get { return GetRequiredService<IServiceContainer>(); }
		}
		
		/// <summary>
		/// Initializes the services for unit testing.
		/// This will replace the whole service container with a new container that
		/// contains only the following services:
		/// - ILoggingService (logging to Diagnostics.Trace)
		/// - IMessageService (writing to Console.Out)
		/// - PropertyService gets initialized with empty in-memory property container
		/// </summary>
		public static void InitializeForUnitTests()
		{
			var container = new ThreadSafeServiceContainer();
			container.AddService(typeof(ILoggingService), new TextWriterLoggingService(new TraceTextWriter()));
			container.AddService(typeof(IMessageService), new TextWriterMessageService(Console.Out));
			PropertyService.InitializeServiceForUnitTests();
			ServiceSingleton.ServiceProvider = container;
		}
		
		/// <summary>
		/// Gets a service. Returns null if service is not found.
		/// </summary>
		public static T GetService<T>() where T : class
		{
			return ServiceSingleton.ServiceProvider.GetService<T>();
		}
		
		/// <summary>
		/// Gets a service. Returns null if service is not found.
		/// </summary>
		public static T GetRequiredService<T>() where T : class
		{
			return ServiceSingleton.ServiceProvider.GetRequiredService<T>();
		}
		
		/// <summary>
		/// Gets the workbench.
		/// </summary>
		public static IWorkbench Workbench {
			get { return GetRequiredService<IWorkbench>(); }
		}
		
		/// <summary>
		/// Gets the status bar.
		/// </summary>
		public static IStatusBarService StatusBar {
			get { return GetRequiredService<IStatusBarService>(); }
		}
		
		public static ILoggingService LoggingService {
			get { return GetRequiredService<ILoggingService>(); }
		}
		
		public static IMessageService MessageService {
			get { return GetRequiredService<IMessageService>(); }
		}
	}
}
