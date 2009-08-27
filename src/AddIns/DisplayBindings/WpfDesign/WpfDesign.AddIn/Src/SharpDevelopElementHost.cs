// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WpfDesign.AddIn
{
	/// <summary>
	/// Hosts a WPF element inside a Windows.Forms application.
	/// </summary>
	public class SharpDevelopElementHost : ElementHost, IUndoHandler, IClipboardHandler
	{
		[ThreadStatic]
		static bool registeredErrorHandler;
		
		public SharpDevelopElementHost()
		{
			if (!registeredErrorHandler) {
				registeredErrorHandler = true;
				Dispatcher.CurrentDispatcher.UnhandledException += CurrentDispatcher_UnhandledException;
			}
		}

        //needed for command routing (SharpDevelopElementHost -> DesignSurface)
		internal WpfViewContent ViewContent;

		static void CurrentDispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			ICSharpCode.Core.MessageService.ShowError(e.Exception, "Unhandled WPF exception");
			e.Handled = true;
		}
		
		bool IsEnabled(RoutedCommand command)
		{
			if (command.CanExecute(null, null))
				return true;
			else if (ViewContent != null)
				return command.CanExecute(null, ViewContent.DesignSurface);
			else
				return false;
		}
		
		void Run(RoutedCommand command)
		{
			if (command.CanExecute(null, null)) {
				command.Execute(null, null);
			} else if (ViewContent != null) {
				command.Execute(null, ViewContent.DesignSurface);
			}
		}
		
		public bool EnableUndo {
			get { return IsEnabled(ApplicationCommands.Undo); }
		}
		
		public bool EnableRedo {
			get { return IsEnabled(ApplicationCommands.Redo); }
		}
		
		public void Undo()
		{
			Run(ApplicationCommands.Undo);
		}
		
		public void Redo()
		{
			Run(ApplicationCommands.Redo);
		}
		
		public bool EnableCut {
			get { return IsEnabled(ApplicationCommands.Undo); }
		}
		
		public bool EnableCopy {
			get { return IsEnabled(ApplicationCommands.Copy); }
		}
		
		public bool EnablePaste {
			get { return IsEnabled(ApplicationCommands.Paste); }
		}
		
		public bool EnableDelete {
			get { return IsEnabled(ApplicationCommands.Delete); }
		}
		
		public bool EnableSelectAll {
			get { return IsEnabled(ApplicationCommands.SelectAll); }
		}
		
		public void Cut()
		{
			Run(ApplicationCommands.Cut);
		}
		
		public void Copy()
		{
			Run(ApplicationCommands.Copy);
		}
		
		public void Paste()
		{
			Run(ApplicationCommands.Paste);
		}
		
		public void Delete()
		{
			Run(ApplicationCommands.Delete);
		}
		
		public void SelectAll()
		{
			Run(ApplicationCommands.SelectAll);
		}
	}
}
