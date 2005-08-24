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
				return textArea.SelectionManager.HasSomethingSelected;
			}
		}
		
		public bool EnableCopy {
			get {
				return textArea.SelectionManager.HasSomethingSelected;
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
		
		bool CopyTextToClipboard()
		{
			string str = textArea.SelectionManager.SelectedText;
			
			if (str.Length > 0) {
				DataObject dataObject = new DataObject();
				dataObject.SetData(DataFormats.UnicodeText, true, str);
				// Default has no highlighting, therefore we don't need RTF output
				if (textArea.Document.HighlightingStrategy.Name != "Default") {
					dataObject.SetData(DataFormats.Rtf, RtfWriter.GenerateRtf(textArea));
				}
				OnCopyText(new CopyTextEventArgs(str));
				Clipboard.SetDataObject(dataObject, true, 50, 50);
				return true;
			}
			return false;
		}
		
		public void Cut(object sender, EventArgs e)
		{
			if (CopyTextToClipboard()) {
				// remove text
				textArea.BeginUpdate();
				textArea.Caret.Position = textArea.SelectionManager.SelectionCollection[0].StartPosition;
				textArea.SelectionManager.RemoveSelectedText();
				textArea.EndUpdate();
			}
		}
		
		public void Copy(object sender, EventArgs e)
		{
			CopyTextToClipboard();
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
