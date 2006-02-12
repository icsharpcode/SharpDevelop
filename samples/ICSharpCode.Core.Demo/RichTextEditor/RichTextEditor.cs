// Copyright (c) 2005 Daniel Grunwald
// Licensed under the terms of the "BSD License", see doc/license.txt

using System;
using System.IO;
using System.Windows.Forms;
using Base;

namespace RichTextEditor
{
	public class DisplayBinding : IDisplayBinding
	{
		public IViewContent OpenFile(string fileName)
		{
			if (Path.GetExtension(fileName).ToLowerInvariant() == ".rtf") {
				return new RichTextViewContent(fileName);
			}
			return null;
		}
	}
	
	public class RichTextViewContent : FileViewContent, IClipboardHandler, IUndoHandler
	{
		RichTextBox textBox = new RichTextBox();
		
		public RichTextViewContent()
		{
			textBox.RichTextShortcutsEnabled = false;
			textBox.AcceptsTab = true;
			textBox.ScrollBars = RichTextBoxScrollBars.Both;
			textBox.TextChanged += delegate {
				this.Dirty = true;
			};
		}
		
		public RichTextViewContent(string fileName) : this()
		{
			textBox.LoadFile(fileName);
			this.FileName = fileName;
			this.Dirty = false;
		}
		
		public override Control Control {
			get {
				return textBox;
			}
		}
		
		protected override bool Save(string fileName)
		{
			textBox.SaveFile(fileName);
			return true;
		}
		
		#region IClipboardHandler implementation
		bool IClipboardHandler.CanPaste {
			get {
				return !textBox.ReadOnly;
			}
		}
		
		bool IClipboardHandler.CanCut {
			get {
				return !textBox.ReadOnly && textBox.SelectionLength > 0;
			}
		}
		
		bool IClipboardHandler.CanCopy {
			get {
				return textBox.SelectionLength > 0;
			}
		}
		
		bool IClipboardHandler.CanDelete {
			get {
				return !textBox.ReadOnly && textBox.SelectionLength > 0;
			}
		}
		
		void IClipboardHandler.Paste()
		{
			textBox.Paste();
		}
		
		void IClipboardHandler.Cut()
		{
			textBox.Cut();
		}
		
		void IClipboardHandler.Copy()
		{
			textBox.Copy();
		}
		
		void IClipboardHandler.Delete()
		{
			textBox.SelectedText = "";
		}
		#endregion
		
		#region IUndoHandler implementation
		bool IUndoHandler.CanUndo {
			get {
				return textBox.CanUndo;
			}
		}
		
		bool IUndoHandler.CanRedo {
			get {
				return textBox.CanRedo;
			}
		}
		
		void IUndoHandler.Undo()
		{
			textBox.Undo();
		}
		
		void IUndoHandler.Redo()
		{
			textBox.Redo();
		}
		#endregion
	}
}
