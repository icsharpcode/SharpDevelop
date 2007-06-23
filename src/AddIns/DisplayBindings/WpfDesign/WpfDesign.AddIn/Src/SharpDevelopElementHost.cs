// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms.Integration;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WpfDesign.AddIn
{
	/// <summary>
	/// Hosts a WPF element inside a Windows.Forms application.
	/// </summary>
	public class SharpDevelopElementHost : ElementHost, IUndoHandler, IClipboardHandler
	{
		static bool IsEnabled(ICommand command)
		{
			return command.CanExecute(null);
		}
		
		static void Run(ICommand command)
		{
			command.Execute(null);
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
