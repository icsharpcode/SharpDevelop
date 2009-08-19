// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core.Services;
using System;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Sda
{
	sealed class SDServiceManager : ServiceManager
	{
		readonly ILoggingService loggingService = new log4netLoggingService();
		readonly IMessageService messageService = new SDMessageService();
		readonly ThreadSafeServiceContainer container = new ThreadSafeServiceContainer();
		
		public override ILoggingService LoggingService {
			get { return loggingService; }
		}
		
		public override IMessageService MessageService {
			get { return messageService; }
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
