// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.DefaultEditor;
using System;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// WindowsFormsHost used in SharpDevelop.
	/// </summary>
	public class SDWindowsFormsHost : WindowsFormsHost
	{
		public SDWindowsFormsHost()
		{
			this.DisposeChild = true;
			CreateBindings();
		}
		
		#region Binding
		void CreateBindings()
		{
			AddBinding(ApplicationCommands.Copy, (IClipboardHandler c) => c.Copy(), c => c.EnableCopy);
			AddBinding(ApplicationCommands.Cut, (IClipboardHandler c) => c.Cut(), c => c.EnableCut);
			AddBinding(ApplicationCommands.Paste, (IClipboardHandler c) => c.Paste(), c => c.EnablePaste);
			AddBinding(ApplicationCommands.Delete, (IClipboardHandler c) => c.Delete(), c => c.EnableDelete);
			AddBinding(ApplicationCommands.SelectAll, (IClipboardHandler c) => c.SelectAll(), c => c.EnableSelectAll);
			AddBinding(ApplicationCommands.Help, (IContextHelpProvider h) => h.ShowHelp(), h => true);
			AddBinding(ApplicationCommands.Undo, (IUndoHandler u) => u.Undo(), u => u.EnableUndo);
			AddBinding(ApplicationCommands.Redo, (IUndoHandler u) => u.Redo(), u => u.EnableRedo);
			AddBinding(ApplicationCommands.Print, (IPrintable p) => WindowsFormsPrinting.Print(p), p => true);
			AddBinding(ApplicationCommands.PrintPreview, (IPrintable p) => WindowsFormsPrinting.PrintPreview(p), p => true);
		}
		
		void AddBinding<T>(ICommand command, Action<T> execute, Predicate<T> canExecute) where T : class
		{
			ExecutedRoutedEventHandler onExected = (sender, e) => {
				var cbh = GetInterface<T>();
				if (cbh != null) {
					e.Handled = true;
					if (canExecute(cbh))
						execute(cbh);
				}
			};
			CanExecuteRoutedEventHandler onCanExecute = (sender, e) => {
				var cbh = GetInterface<T>();
				if (cbh != null) {
					e.Handled = true;
					e.CanExecute = canExecute(cbh);
				}
			};
			this.CommandBindings.Add(new CommandBinding(command, onExected, onCanExecute));
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
		
		protected override void Dispose(bool disposing)
		{
			if (disposing && !this.DisposeChild && Child != null) {
				// prevent child from being disposed
				Child = null;
			}
			base.Dispose(disposing);
		}
	}
}
