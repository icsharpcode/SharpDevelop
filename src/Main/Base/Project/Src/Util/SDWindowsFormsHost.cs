// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Util;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// WindowsFormsHost used in SharpDevelop.
	/// </summary>
	public class SDWindowsFormsHost : CustomWindowsFormsHost
	{
		/// <summary>
		/// Creates a new SDWindowsFormsHost instance.
		/// </summary>
		/// <param name="processShortcutsInWPF">
		/// Determines whether the shortcuts for the default actions (Cut,Copy,Paste,Undo, etc.)
		/// are processed by the WPF command system.
		/// The default value is false. Pass true only if WinForms does not handle those shortcuts by itself.
		/// See SD-1671 and SD-1737.
		/// </param>
		public SDWindowsFormsHost(bool processShortcutsInWPF = false)
		{
			this.DisposeChild = true;
			CreateBindings(processShortcutsInWPF);
		}
		
		#region Binding
		void CreateBindings(bool processShortcutsInWPF)
		{
			AddBinding(processShortcutsInWPF, ApplicationCommands.Copy, (IClipboardHandler c) => c.Copy(), c => c.EnableCopy);
			AddBinding(processShortcutsInWPF, ApplicationCommands.Cut, (IClipboardHandler c) => c.Cut(), c => c.EnableCut);
			AddBinding(processShortcutsInWPF, ApplicationCommands.Paste, (IClipboardHandler c) => c.Paste(), c => c.EnablePaste);
			AddBinding(processShortcutsInWPF, ApplicationCommands.Delete, (IClipboardHandler c) => c.Delete(), c => c.EnableDelete);
			AddBinding(processShortcutsInWPF, ApplicationCommands.SelectAll, (IClipboardHandler c) => c.SelectAll(), c => c.EnableSelectAll);
			AddBinding(processShortcutsInWPF, ApplicationCommands.Help, (IContextHelpProvider h) => h.ShowHelp(), h => true);
			AddBinding(processShortcutsInWPF, ApplicationCommands.Undo, (IUndoHandler u) => u.Undo(), u => u.EnableUndo);
			AddBinding(processShortcutsInWPF, ApplicationCommands.Redo, (IUndoHandler u) => u.Redo(), u => u.EnableRedo);
			AddBinding(processShortcutsInWPF, ApplicationCommands.Print, (IPrintable p) => WindowsFormsPrinting.Print(p), p => true);
			AddBinding(processShortcutsInWPF, ApplicationCommands.PrintPreview, (IPrintable p) => WindowsFormsPrinting.PrintPreview(p), p => true);
		}
		
		void AddBinding<T>(bool processShortcutsInWPF, ICommand command, Action<T> execute, Predicate<T> canExecute) where T : class
		{
			ExecutedRoutedEventHandler onExecuted = (sender, e) => {
				if (e.Command == command) {
					var cbh = GetInterface<T>();
					if (cbh != null) {
						e.Handled = true;
						if (canExecute(cbh))
							execute(cbh);
					}
				}
			};
			CanExecuteRoutedEventHandler onCanExecute = (sender, e) => {
				if (e.Command == command) {
					var cbh = GetInterface<T>();
					if (cbh != null) {
						e.Handled = true;
						e.CanExecute = canExecute(cbh);
					}
				}
			};
			if (processShortcutsInWPF) {
				this.CommandBindings.Add(new CommandBinding(command, onExecuted, onCanExecute));
			} else {
				// Don't use this.CommandBindings because CommandBindings with built-in shortcuts would handle the key press
				// before WinForms gets to see it. Using the events ensures that the command gets executed only when the user
				// clicks on the menu/toolbar item. (this fixes SD2-1671)
				CommandManager.AddCanExecuteHandler(this, onCanExecute);
				CommandManager.AddExecutedHandler(this, onExecuted);
			}
		}
		#endregion
		
		public override string ToString()
		{
			if (ServiceObject != null)
				return "[SDWindowsFormsHost " + Child + " for " + ServiceObject + "]";
			else
				return "[SDWindowsFormsHost " + Child + "]";
		}
		
		#region Service Object
		/// <summary>
		/// Gets/Sets the object that implements the IClipboardHandler, IUndoHandler etc. interfaces...
		/// </summary>
		public object ServiceObject { get; set; }
		
		T GetInterface<T>() where T : class
		{
			T instance = this.ServiceObject as T;
			if (instance == null) {
				instance = GetServiceWrapper(GetActiveControl()) as T;
			}
			return instance;
		}
		
		Control GetActiveControl()
		{
			ContainerControl container = null;
			Control ctl = this.Child;
			while (ctl != null) {
				container = ctl as ContainerControl;
				if (container == null)
					return ctl;
				ctl = container.ActiveControl;
			}
			return container;
		}
		
		static object GetServiceWrapper(Control ctl)
		{
			TextBoxBase tb = ctl as TextBoxBase;
			if (tb != null)
				return new TextBoxWrapper(tb);
			ComboBox cb = ctl as ComboBox;
			if (cb != null && cb.DropDownStyle != ComboBoxStyle.DropDownList)
				return new ComboBoxWrapper(cb);
			return ctl;
		}
		
		sealed class TextBoxWrapper : IClipboardHandler, IUndoHandler
		{
			TextBoxBase textBox;
			public TextBoxWrapper(TextBoxBase textBox) {
				this.textBox = textBox;
			}
			public bool EnableCut {
				get { return !textBox.ReadOnly && textBox.SelectionLength > 0; }
			}
			public bool EnableCopy {
				get { return textBox.SelectionLength > 0; }
			}
			public bool EnablePaste {
				get { return !textBox.ReadOnly && Clipboard.ContainsText(); }
			}
			public bool EnableDelete {
				get { return !textBox.ReadOnly && textBox.SelectionLength > 0; }
			}
			public bool EnableSelectAll {
				get { return textBox.TextLength > 0; }
			}
			public void Cut()       { textBox.Cut(); }
			public void Copy()      { textBox.Copy(); }
			public void Paste()     { textBox.Paste(); }
			public void Delete()    { textBox.SelectedText = ""; }
			public void SelectAll() { textBox.SelectAll(); }
			
			public bool EnableUndo { get { return textBox.CanUndo; } }
			public bool EnableRedo { get { return false; } }
			
			public void Undo()
			{
				textBox.Undo();
			}
			
			public void Redo()
			{
			}
		}
		
		sealed class ComboBoxWrapper : IClipboardHandler
		{
			ComboBox comboBox;
			public ComboBoxWrapper(ComboBox comboBox) {
				this.comboBox = comboBox;
			}
			public bool EnableCut {
				get { return comboBox.SelectionLength > 0; }
			}
			public bool EnableCopy {
				get { return comboBox.SelectionLength > 0; }
			}
			public bool EnablePaste {
				get { return Clipboard.ContainsText(); }
			}
			public bool EnableDelete {
				get { return true; }
			}
			public bool EnableSelectAll {
				get { return comboBox.Text.Length > 0; }
			}
			public void Cut()       { ClipboardWrapper.SetText(comboBox.SelectedText); comboBox.SelectedText = ""; }
			public void Copy()      { ClipboardWrapper.SetText(comboBox.SelectedText); }
			public void Paste()     { comboBox.SelectedText = ClipboardWrapper.GetText(); }
			public void Delete()    { comboBox.SelectedText = ""; }
			public void SelectAll() { comboBox.SelectAll(); }
		}
		#endregion
		
		/// <summary>
		/// Gets/Sets whether the windows forms control will be disposed
		/// when the WindowsFormsHost is disposed.
		/// The default value is true.
		/// </summary>
		/// <remarks>
		/// The default WindowsFormsHost disposes its child when the WPF application shuts down,
		/// but some events in SharpDevelop occur after the WPF shutdown (e.g. SolutionClosed), so we must
		/// not dispose pads that could still be handling them.
		/// </remarks>
		public bool DisposeChild { get; set; }
		
		public bool IsDisposed { get; private set; }
		
		protected override void Dispose(bool disposing)
		{
			if (disposing && !this.DisposeChild && Child != null) {
				// prevent child from being disposed
				Child = null;
			}
			IsDisposed = disposing;
			base.Dispose(disposing);
		}
	}
}
