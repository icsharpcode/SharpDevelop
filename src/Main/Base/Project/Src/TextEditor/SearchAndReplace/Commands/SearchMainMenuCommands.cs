// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window != null && (window.ViewContent is ITextEditorControlProvider)) {
				TextEditorControl textarea = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;				
				string selectedText = textarea.ActiveTextAreaControl.TextArea.SelectionManager.SelectedText;
				if (selectedText != null && selectedText.Length > 0) {
					SearchOptions.CurrentFindPattern = selectedText;
				}
			}
		}
		
		public override void Run()
		{
			SetSearchPattern();
			
			SearchAndReplaceDialog searchAndReplaceDialog = new SearchAndReplaceDialog(SearchAndReplaceMode.Search);
			searchAndReplaceDialog.Show();
			
//			if (SearchReplaceManager.ReplaceDialog != null) {
//				SearchReplaceManager.ReplaceDialog.SetSearchPattern(SearchReplaceManager.SearchOptions.SearchPattern);
//			} else {
//				ReplaceDialog rd = new ReplaceDialog(false);
//				rd.Owner = (Form)WorkbenchSingleton.Workbench;
//				rd.Show();
//			}
		}
	}
	
	public class FindNext : AbstractMenuCommand
	{
		public override void Run()
		{
			try {
				SearchReplaceManager.FindNext();
			} catch (Exception e) {
				
				MessageService.ShowError(e);
			}
		}
	}
	
	public class Replace : AbstractMenuCommand
	{
		public override void Run()
		{ 
			Find.SetSearchPattern();
			SearchAndReplaceDialog searchAndReplaceDialog = new SearchAndReplaceDialog(SearchAndReplaceMode.Replace);
			searchAndReplaceDialog.Show();
//			
//			
//			if (SearchReplaceManager.ReplaceDialog != null) {
//				SearchReplaceManager.ReplaceDialog.SetSearchPattern(SearchReplaceManager.SearchOptions.SearchPattern);
//			} else {
//				ReplaceDialog rd = new ReplaceDialog(true);
//				rd.Owner = (Form)WorkbenchSingleton.Workbench;
//				rd.Show();
//			}
		}
	}
}
