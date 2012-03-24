// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Core.Services;
using System;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Logging
{
	sealed class SDServiceManager : ServiceManager
	{
		readonly ThreadSafeServiceContainer container = new ThreadSafeServiceContainer();
		
		public SDServiceManager()
		{
			container.AddService(typeof(IMessageService), new SDMessageService());
			container.AddService(typeof(ILoggingService), new log4netLoggingService());
		}
		
		public override object GetService(Type serviceType)
		{
			return container.GetService(serviceType);
		}
	}
	
	sealed class SDMessageService : WinFormsMessageService
	{
		public override void ShowException(Exception ex, string message)
		{
			if (ex != null)
				ExceptionBox.ShowErrorBox(ex, message);
			else
				ShowError(message);
		}
	}
}
