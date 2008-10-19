// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.DefaultEditor;
using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class Undo : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				IUndoHandler editable = (WorkbenchSingleton.Workbench.ActiveContent as IUndoHandler) ?? (WorkbenchSingleton.ActiveControl as IUndoHandler);
				if (editable != null) {
					return editable.EnableUndo;
				} else {
					TextBoxBase textBox = WorkbenchSingleton.ActiveControl as TextBoxBase;
					if (textBox != null) {
						return textBox.CanUndo;
					}
				}
				return false;
			}
		}
		
		public override void Run()
		{
			IUndoHandler editable = (WorkbenchSingleton.Workbench.ActiveContent as IUndoHandler) ?? (WorkbenchSingleton.ActiveControl as IUndoHandler);
			if (editable != null) {
				editable.Undo();
			} else {
				TextBoxBase textBox = WorkbenchSingleton.ActiveControl as TextBoxBase;
				if (textBox != null) {
					textBox.Undo();
				}
			}
		}
	}
	
	public class Redo : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				IUndoHandler editable = (WorkbenchSingleton.Workbench.ActiveContent as IUndoHandler) ?? (WorkbenchSingleton.ActiveControl as IUndoHandler);
				if (editable != null) {
					return editable.EnableRedo;
				}
				return false;
			}
		}
		
		public override void Run()
		{
			IUndoHandler editable = (WorkbenchSingleton.Workbench.ActiveContent as IUndoHandler) ?? (WorkbenchSingleton.ActiveControl as IUndoHandler);
			if (editable != null) {
				editable.Redo();
			}
		}
	}
	
	public abstract class AbstractClipboardCommand : AbstractMenuCommand
	{
		protected abstract bool GetEnabled(IClipboardHandler editable);
		protected abstract void Run(IClipboardHandler editable);
		
		public static IClipboardHandler GetClipboardHandlerWrapper(Control ctl)
		{
			TextBoxBase tb = ctl as TextBoxBase;
			if (tb != null)
				return new TextBoxWrapper(tb);
			ComboBox cb = ctl as ComboBox;
			if (cb != null && cb.DropDownStyle != ComboBoxStyle.DropDownList)
				return new ComboBoxWrapper(cb);
			return ctl as IClipboardHandler;
		}
		
		private class TextBoxWrapper : IClipboardHandler
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
				get { return !textBox.ReadOnly; }
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
		}
		
		private class ComboBoxWrapper : IClipboardHandler
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
				get { return ClipboardHandling.GetClipboardContainsText(); }
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
		
		public override bool IsEnabled {
			get {
				IClipboardHandler editable = WorkbenchSingleton.Workbench.ActiveContent as IClipboardHandler;
				if (editable == null)
					editable = GetClipboardHandlerWrapper(WorkbenchSingleton.ActiveControl);
				if (editable != null) {
					return GetEnabled(editable);
				}
				return false;
			}
		}
		
		public override void Run()
		{
			IClipboardHandler editable = WorkbenchSingleton.Workbench.ActiveContent as IClipboardHandler;
			if (editable == null)
				editable = GetClipboardHandlerWrapper(WorkbenchSingleton.ActiveControl);
			if (editable != null) {
				Run(editable);
			}
		}
	}
	
	public class Cut : AbstractClipboardCommand
	{
		protected override bool GetEnabled(IClipboardHandler editable) {
			return editable.EnableCut;
		}
		protected override void Run(IClipboardHandler editable) {
			editable.Cut();
		}
	}
	
	public class Copy : AbstractClipboardCommand
	{
		protected override bool GetEnabled(IClipboardHandler editable) {
			return editable.EnableCopy;
		}
		protected override void Run(IClipboardHandler editable) {
			editable.Copy();
		}
	}
	
	public class Paste : AbstractClipboardCommand
	{
		protected override bool GetEnabled(IClipboardHandler editable) {
			return editable.EnablePaste;
		}
		protected override void Run(IClipboardHandler editable) {
			editable.Paste();
		}
	}
	
	public class Delete : AbstractClipboardCommand
	{
		protected override bool GetEnabled(IClipboardHandler editable) {
			return editable.EnableDelete;
		}
		protected override void Run(IClipboardHandler editable) {
			editable.Delete();
		}
	}
	
	public class SelectAll : AbstractClipboardCommand
	{
		protected override bool GetEnabled(IClipboardHandler editable) {
			return editable.EnableSelectAll;
		}
		protected override void Run(IClipboardHandler editable) {
			editable.SelectAll();
		}
	}
	
	public class WordCount : AbstractMenuCommand
	{
		public override void Run()
		{
			using (WordCountDialog wcd = new WordCountDialog()) {
				wcd.Owner = WorkbenchSingleton.MainForm;
				wcd.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
			}
		}
	}
}
