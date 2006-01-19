/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 19.01.2006
 * Time: 16:34
 */

using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;

namespace StringResourceToolAddIn
{
	public class ToolCommand1 : AbstractMenuCommand
	{
		public override void Run()
		{
			// Here an example that shows how to access the current text document:
			
			ITextEditorControlProvider tecp = WorkbenchSingleton.Workbench.ActiveContent as ITextEditorControlProvider;
			if (tecp == null) {
				// active content is not a text editor control
				return;
			}
			// Get the active text area from the control:
			TextArea textArea = tecp.TextEditorControl.ActiveTextAreaControl.TextArea;
			if (!textArea.SelectionManager.HasSomethingSelected)
				return;
			// get the selected text:
			string text = textArea.SelectionManager.SelectedText;
			
			string resourceName = MessageService.ShowInputBox("Add Resource", "Enter the resource name", PropertyService.Get("ResourceToolLastResourceName"));
			if (resourceName == null || resourceName.Length == 0) return;
			PropertyService.Set("ResourceToolLastResourceName", resourceName);
			
			string purpose = MessageService.ShowInputBox("Add Resource", "Enter resource purpose (may be empty)", "");
			if (purpose == null) return;
			
			string newText = "${res:" + resourceName + "}";
			
			// ensure caret is at start of selection
			textArea.Caret.Position = textArea.SelectionManager.SelectionCollection[0].StartPosition;
			// deselect text
			textArea.SelectionManager.ClearSelection();
			// replace the selected text with the new text:
			// Replace() takes the arguments: start offset to replace, length of the text to remove, new text
			textArea.Document.Replace(textArea.Caret.Offset,
			                          text.Length,
			                          newText);
			
			// Redraw:
			textArea.Refresh();
			
			string path = Path.Combine(FileUtility.ApplicationRootPath, "src/Tools/StringResourceTool/bin/Debug");
			ProcessStartInfo info = new ProcessStartInfo(path + "/StringResourceTool.exe",
			                                             "\"" + resourceName + "\" "
			                                             + "\"" + text + "\" "
			                                             + "\"" + purpose + "\"");
			info.WorkingDirectory = path;
			Process.Start(info);
		}
	}
}
