// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design;
using ICSharpCode.Core;
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
		/// Gets a service. Returns null if service is not found.
		/// </summary>
		public static T GetService<T>() where T : class
		{
			return ServiceManager.Instance.GetService<T>();
		}
		
		/// <summary>
		/// Gets a service. Returns null if service is not found.
		/// </summary>
		public static T GetRequiredService<T>() where T : class
		{
			return ServiceManager.Instance.GetRequiredService<T>();
		}
		
		/// <summary>
		/// Gets the workbench.
		/// </summary>
		public static IWorkbench Workbench {
			get {
				var workbench = WorkbenchSingleton.Workbench;
				if (workbench == null)
					throw new ServiceNotFoundException("Workbench");
				return workbench;
			}
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
