// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Threading;

namespace ICSharpCode.Core.Presentation
{
	public static class DispatcherErrorHandler
	{
		[ThreadStatic] static bool isRegistered;
		
		/// <summary>
		/// Registers the WPF error handler for this thread.
		/// If the error handler is already registered, this method does nothing.
		/// 
		/// WPF applications should call this method when they initialize to prevent
		/// SharpDevelop from crashing completely when there is an error in WPF event handlers.
		/// </summary>
		public static void Register()
		{
			if (!isRegistered) {
				isRegistered = true;
				Dispatcher.CurrentDispatcher.UnhandledException += DispatcherUnhandledException;
			}
		}
		
		static void DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			if (!e.Handled) {
				e.Handled = true;
				MessageService.ShowError(e.Exception, "Unhandled WPF exception");
			}
		}
	}
}
