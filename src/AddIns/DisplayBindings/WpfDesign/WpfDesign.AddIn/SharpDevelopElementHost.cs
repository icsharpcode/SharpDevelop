// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3519 $</version>
// </file>

using System;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WpfDesign.Designer;

namespace ICSharpCode.WpfDesign.AddIn
{
	/// <summary>
	/// Hosts a WPF element inside a Windows.Forms application.
	/// </summary>
	public class SharpDevelopElementHost : ElementHost, IUndoHandler, IClipboardHandler
	{
		public SharpDevelopElementHost(UIElement child)
		{
			Child = child;

			if (!registeredErrorHandler) {
				registeredErrorHandler = true;
				Dispatcher.CurrentDispatcher.UnhandledException += CurrentDispatcher_UnhandledException;
			}
		}

		[ThreadStatic]
		static bool registeredErrorHandler;

		static void CurrentDispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			ICSharpCode.Core.MessageService.ShowError(e.Exception, "Unhandled WPF exception");
			e.Handled = true;
		}

		ICommandService CurrentCommandService
		{
			get
			{
				if (WpfTools.ActiveContext != null) {
					return WpfTools.ActiveContext.CommandService;
				}
				return null;
			}
		}

		public bool EnableUndo
		{
			get { return CurrentCommandService != null && CurrentCommandService.CanUndo(); }
		}

		public bool EnableRedo
		{
			get { return CurrentCommandService != null && CurrentCommandService.CanRedo(); }
		}

		public bool EnableCopy
		{
			get { return CurrentCommandService != null && CurrentCommandService.CanCopy(); }
		}

		public bool EnablePaste
		{
			get { return CurrentCommandService != null && CurrentCommandService.CanPaste(); }
		}

		public bool EnableCut
		{
			get { return CurrentCommandService != null && CurrentCommandService.CanCut(); }
		}

		public bool EnableSelectAll
		{
			get { return CurrentCommandService != null && CurrentCommandService.CanSelectAll(); }
		}

		public bool EnableDelete
		{
			get { return CurrentCommandService != null && CurrentCommandService.CanDelete(); }
		}

		public void Undo()
		{
			CurrentCommandService.Undo();
		}

		public void Redo()
		{
			CurrentCommandService.Redo();
		}

		public void Copy()
		{
			CurrentCommandService.Copy();
		}

		public void Paste()
		{
			CurrentCommandService.Paste();
		}

		public void Cut()
		{
			CurrentCommandService.Cut();
		}

		public void SelectAll()
		{
			CurrentCommandService.SelectAll();
		}

		public void Delete()
		{
			CurrentCommandService.Delete();
		}
	}
}
