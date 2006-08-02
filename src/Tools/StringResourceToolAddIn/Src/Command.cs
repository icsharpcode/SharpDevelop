/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 19.01.2006
 * Time: 16:34
 */

using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Resources;
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
			
			string sdSrcPath = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location),
			                                "../../../..");
			
			using (ResourceReader r = new ResourceReader(Path.Combine(sdSrcPath,
			                                                          "Main/StartUp/Project/Resources/StringResources.resources"))) {
				IDictionaryEnumerator en = r.GetEnumerator();
				// Goes through the enumerator, printing out the key and value pairs.
				while (en.MoveNext()) {
					if (object.Equals(en.Value, text)) {
						SetText(textArea, en.Key.ToString(), text);
						return;
					}
				}
			}
			
			string resourceName = MessageService.ShowInputBox("Add Resource", "Please enter the name for the new resource.\n" +
			                                                  "This should be a namespace-like construct, please see what the names of resources in the same component are.", PropertyService.Get("ResourceToolLastResourceName"));
			if (resourceName == null || resourceName.Length == 0) return;
			PropertyService.Set("ResourceToolLastResourceName", resourceName);
			
			string purpose = MessageService.ShowInputBox("Add Resource", "Enter resource purpose (may be empty)", "");
			if (purpose == null) return;
			
			SetText(textArea, resourceName, text);
			
			string path = Path.GetFullPath(Path.Combine(sdSrcPath, "Tools/StringResourceTool/bin/Debug"));
			ProcessStartInfo info = new ProcessStartInfo(path + "\\StringResourceTool.exe",
			                                             "\"" + resourceName + "\" "
			                                             + "\"" + text + "\" "
			                                             + "\"" + purpose + "\"");
			info.WorkingDirectory = path;
			try {
				Process.Start(info);
			} catch (Exception ex) {
				MessageService.ShowError(ex, "Error starting " + info.FileName);
			}
		}
		
		void SetText(TextArea textArea, string resourceName, string oldText)
		{
			// ensure caret is at start of selection
			textArea.Caret.Position = textArea.SelectionManager.SelectionCollection[0].StartPosition;
			// deselect text
			textArea.SelectionManager.ClearSelection();
			// replace the selected text with the new text:
			// Replace() takes the arguments: start offset to replace, length of the text to remove, new text
			textArea.Document.Replace(textArea.Caret.Offset,
			                          oldText.Length,
			                          "$" + "{res:" + resourceName + "}");
			// Redraw:
			textArea.Refresh();
		}
	}
}
