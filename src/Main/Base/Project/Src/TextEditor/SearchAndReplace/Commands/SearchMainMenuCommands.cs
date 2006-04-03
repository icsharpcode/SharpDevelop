// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop;

namespace SearchAndReplace
{
	public class Find : AbstractMenuCommand
	{
		public static void SetSearchPattern()
		{
			// Get Highlighted value and set it to FindDialog.searchPattern
			TextEditorControl textArea = SearchReplaceUtilities.GetActiveTextEditor();
			string selectedText = textArea.ActiveTextAreaControl.TextArea.SelectionManager.SelectedText;
			if (selectedText != null && selectedText.Length > 0 && !IsMultipleLines(selectedText)) {
				SearchOptions.CurrentFindPattern = selectedText;
			}
		}
		
		public override void Run()
		{
			SetSearchPattern();
			SearchAndReplaceDialog.ShowSingleInstance(SearchAndReplaceMode.Search);
		}
		
		static bool IsMultipleLines(string text)
		{
			return text.IndexOf('\n') != -1;
		}
	}
	
	public class FindNext : AbstractMenuCommand
	{
		public override void Run()
		{
			if (SearchOptions.CurrentFindPattern.Length > 0) {
				SearchReplaceManager.FindNext();
			} else {
				Find find = new Find();
				find.Run();
			}
		}
	}
	
	public class Replace : AbstractMenuCommand
	{
		public override void Run()
		{
			Find.SetSearchPattern();
			SearchAndReplaceDialog.ShowSingleInstance(SearchAndReplaceMode.Replace);
		}
	}
}
