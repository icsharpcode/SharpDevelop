// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Util;

namespace ICSharpCode.TextEditor
{
	public class TextAreaClipboardHandler
	{
		TextArea textArea;
		
		public bool EnableCut {
			get {
				return textArea.EnableCutOrPaste; //textArea.SelectionManager.HasSomethingSelected;
			}
		}
		
		public bool EnableCopy {
			get {
				return true; //textArea.SelectionManager.HasSomethingSelected;
			}
		}
		
		public bool EnablePaste {
			get {
				try {
					return Clipboard.ContainsText();
				} catch (ExternalException) {
					return false;
				}
			}
		}
		
		public bool EnableDelete {
			get {
				return textArea.SelectionManager.HasSomethingSelected && textArea.EnableCutOrPaste;
			}
		}
		
		public bool EnableSelectAll {
			get {
				return true;
			}
		}
		
		public TextAreaClipboardHandler(TextArea textArea)
		{
			this.textArea = textArea;
			textArea.SelectionManager.SelectionChanged += new EventHandler(DocumentSelectionChanged);
		}
		
		void DocumentSelectionChanged(object sender, EventArgs e)
		{
//			((DefaultWorkbench)WorkbenchSingleton.Workbench).UpdateToolbars();
		}

		string LineSelectedType
		{
			get {
				return "MSDEVLineSelect";  // This is the type VS 2003 and 2005 use for flagging a whole line copy
			}
		}
		
		bool CopyTextToClipboard(string stringToCopy, bool asLine)
		{
			if (stringToCopy.Length > 0) {
				DataObject dataObject = new DataObject();
				dataObject.SetData(DataFormats.UnicodeText, true, stringToCopy);
				if (asLine) {
					MemoryStream lineSelected = new MemoryStream(1);
					lineSelected.WriteByte(1);
					dataObject.SetData(LineSelectedType, false, lineSelected);
				}
				// Default has no highlighting, therefore we don't need RTF output
				if (textArea.Document.HighlightingStrategy.Name != "Default") {
					dataObject.SetData(DataFormats.Rtf, RtfWriter.GenerateRtf(textArea));
				}
				OnCopyText(new CopyTextEventArgs(stringToCopy));
				
				// Work around ExternalException bug. (SD2-426)
				// Best reproducable inside Virtual PC.
				try {
					Clipboard.SetDataObject(dataObject, true, 10, 50);
				} catch (ExternalException) {
					Application.DoEvents();
					try {
						Clipboard.SetDataObject(dataObject, true, 10, 50);
					} catch (ExternalException) {}
				}
				return true;
			} else {
				return false;
			}
		}

		bool CopyTextToClipboard(string stringToCopy)
		{
			return CopyTextToClipboard(stringToCopy, false);
		}
		
		public void Cut(object sender, EventArgs e)
		{
			if (CopyTextToClipboard(textArea.SelectionManager.SelectedText)) {
				// Remove text
				textArea.BeginUpdate();
				textArea.Caret.Position = textArea.SelectionManager.SelectionCollection[0].StartPosition;
				textArea.SelectionManager.RemoveSelectedText();
				textArea.EndUpdate();
			} else if (textArea.Document.TextEditorProperties.CutCopyWholeLine){
				// No text was selected, select and cut the entire line
				int curLineNr = textArea.Document.GetLineNumberForOffset(textArea.Caret.Offset);
				LineSegment lineWhereCaretIs = textArea.Document.GetLineSegment(curLineNr);
				string caretLineText = textArea.Document.GetText(lineWhereCaretIs.Offset, lineWhereCaretIs.TotalLength);
				textArea.SelectionManager.SetSelection(textArea.Document.OffsetToPosition(lineWhereCaretIs.Offset), textArea.Document.OffsetToPosition(lineWhereCaretIs.Offset + lineWhereCaretIs.TotalLength));
				if (CopyTextToClipboard(caretLineText, true)) {
					// remove line
					textArea.BeginUpdate();
					textArea.Caret.Position = textArea.Document.OffsetToPosition(lineWhereCaretIs.Offset);
					textArea.SelectionManager.RemoveSelectedText();
					textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.PositionToEnd, new Point(0, curLineNr)));
					textArea.EndUpdate();
				}
			}
		}
		
		public void Copy(object sender, EventArgs e)
		{
			if (!CopyTextToClipboard(textArea.SelectionManager.SelectedText) && textArea.Document.TextEditorProperties.CutCopyWholeLine) {
				// No text was selected, select the entire line, copy it, and then deselect
				int curLineNr = textArea.Document.GetLineNumberForOffset(textArea.Caret.Offset);
				LineSegment lineWhereCaretIs = textArea.Document.GetLineSegment(curLineNr);
				string caretLineText = textArea.Document.GetText(lineWhereCaretIs.Offset, lineWhereCaretIs.TotalLength);
				CopyTextToClipboard(caretLineText, true);
			}
		}
		
		public void Paste(object sender, EventArgs e)
		{
			// Clipboard.GetDataObject may throw an exception...
			for (int i = 0;; i++) {
				try {
					IDataObject data = Clipboard.GetDataObject();
					bool fullLine = data.GetDataPresent(LineSelectedType);
					if (data.GetDataPresent(DataFormats.UnicodeText)) {
						string text = (string)data.GetData(DataFormats.UnicodeText);
						if (text.Length > 0) {
							int redocounter = 0;
							if (textArea.SelectionManager.HasSomethingSelected) {
								Delete(sender, e);
								redocounter++;
							}
							if (fullLine) {
								int col = textArea.Caret.Column;
								textArea.Caret.Column = 0;
								textArea.InsertString(text);
								textArea.Caret.Column = col;
							}
							else {
								textArea.InsertString(text);
							}
							if (redocounter > 0) {
								textArea.Document.UndoStack.UndoLast(redocounter + 1); // redo the whole operation
							}
						}
					}
					return;
				} catch (ExternalException) {
					// GetDataObject does not provide RetryTimes parameter
					if (i > 5) throw;
				}
			}
		}
		
		public void Delete(object sender, EventArgs e)
		{
			new ICSharpCode.TextEditor.Actions.Delete().Execute(textArea);
		}
		
		public void SelectAll(object sender, EventArgs e)
		{
			new ICSharpCode.TextEditor.Actions.SelectWholeDocument().Execute(textArea);
		}
		
		protected virtual void OnCopyText(CopyTextEventArgs e)
		{
			if (CopyText != null) {
				CopyText(this, e);
			}
		}
		
		public event CopyTextEventHandler CopyText;
	}
	
	public delegate void CopyTextEventHandler(object sender, CopyTextEventArgs e);
	public class CopyTextEventArgs : EventArgs
	{
		string text;
		
		public string Text {
			get {
				return text;
			}
		}
		
		public CopyTextEventArgs(string text)
		{
			this.text = text;
		}
	}
}
