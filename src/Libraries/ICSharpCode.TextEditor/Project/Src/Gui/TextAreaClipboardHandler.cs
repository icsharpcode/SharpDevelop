// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.Xml;
using System.Text;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Undo;
using ICSharpCode.TextEditor.Util;

namespace ICSharpCode.TextEditor
{
	public class TextAreaClipboardHandler
	{
		TextArea textArea;
		
		public bool EnableCut {
			get {
				return true; //textArea.SelectionManager.HasSomethingSelected;
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
				return textArea.SelectionManager.HasSomethingSelected;
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
		
		bool CopyTextToClipboard(string stringToCopy)
		{
			if (stringToCopy.Length > 0) {
				DataObject dataObject = new DataObject();
				dataObject.SetData(DataFormats.UnicodeText, true, stringToCopy);
				// Default has no highlighting, therefore we don't need RTF output
				if (textArea.Document.HighlightingStrategy.Name != "Default") {
					dataObject.SetData(DataFormats.Rtf, RtfWriter.GenerateRtf(textArea));
				}
				OnCopyText(new CopyTextEventArgs(stringToCopy));
				
				// Work around ExternalException bug. (SD2-426)
				// Best reproducable inside Virtual PC.
				// SetDataObject has "retry" parameters, but apparently a call to "DoEvents"
				// is necessary for the workaround to work.
				int i = 0;
				while (true) {
					try {
						Clipboard.SetDataObject(dataObject, true, 5, 50);
						return true;
					} catch (ExternalException) {
						if (i++ > 5)
							throw;
					}
					System.Threading.Thread.Sleep(50);
					Application.DoEvents();
					System.Threading.Thread.Sleep(50);
				}
			} else {
				return false;
			}
		}
		
		public void Cut(object sender, EventArgs e)
		{
			if (CopyTextToClipboard(textArea.SelectionManager.SelectedText)) {
				// Remove text
				textArea.BeginUpdate();
				textArea.Caret.Position = textArea.SelectionManager.SelectionCollection[0].StartPosition;
				textArea.SelectionManager.RemoveSelectedText();
				textArea.EndUpdate();
			} else {
				// No text was selected, select and cut the entire line
				int curLineNr = textArea.Document.GetLineNumberForOffset(textArea.Caret.Offset);
				LineSegment lineWhereCaretIs = textArea.Document.GetLineSegment(curLineNr);
				string caretLineText = textArea.Document.GetText(lineWhereCaretIs.Offset, lineWhereCaretIs.TotalLength);
				textArea.SelectionManager.SetSelection(textArea.Document.OffsetToPosition(lineWhereCaretIs.Offset), textArea.Document.OffsetToPosition(lineWhereCaretIs.Offset + lineWhereCaretIs.TotalLength));
				if (CopyTextToClipboard(caretLineText)) {
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
			if (!CopyTextToClipboard(textArea.SelectionManager.SelectedText)) {
				// No text was selected, select the entire line, copy it, and then deselect
				int curLineNr = textArea.Document.GetLineNumberForOffset(textArea.Caret.Offset);
				LineSegment lineWhereCaretIs = textArea.Document.GetLineSegment(curLineNr);
				string caretLineText = textArea.Document.GetText(lineWhereCaretIs.Offset, lineWhereCaretIs.TotalLength);
				textArea.SelectionManager.SetSelection(textArea.Document.OffsetToPosition(lineWhereCaretIs.Offset), textArea.Document.OffsetToPosition(lineWhereCaretIs.Offset + lineWhereCaretIs.TotalLength));
				CopyTextToClipboard(caretLineText);
				textArea.SelectionManager.ClearSelection();
			}
		}
		
		public void Paste(object sender, EventArgs e)
		{
			// Clipboard.GetDataObject may throw an exception...
			for (int i = 0;; i++) {
				try {
					IDataObject data = Clipboard.GetDataObject();
					if (data.GetDataPresent(DataFormats.UnicodeText)) {
						string text = (string)data.GetData(DataFormats.UnicodeText);
						if (text.Length > 0) {
							int redocounter = 0;
							if (textArea.SelectionManager.HasSomethingSelected) {
								Delete(sender, e);
								redocounter++;
							}
							textArea.InsertString(text);
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
