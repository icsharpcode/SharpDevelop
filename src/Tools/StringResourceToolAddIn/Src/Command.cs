/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 19.01.2006
 * Time: 16:34
 */

using ICSharpCode.SharpDevelop.Editor;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace StringResourceToolAddIn
{
	public class ToolCommand1 : AbstractMenuCommand
	{
		public override void Run()
		{
			// Here an example that shows how to access the current text document:
			
			ITextEditorProvider tecp = WorkbenchSingleton.Workbench.ActiveContent as ITextEditorProvider;
			if (tecp == null) {
				// active content is not a text editor control
				return;
			}
			
			// Get the active text area from the control:
			ITextEditor textEditor = tecp.TextEditor;
			if (textEditor.SelectionLength == 0)
				return;
			// get the selected text:
			string text = textEditor.SelectedText;
			
			string sdSrcPath = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location),
			                                "../../../..");
			string resxFile = Path.Combine(sdSrcPath, "../data/Resources/StringResources.resx");
			
			using (ResXResourceReader r = new ResXResourceReader(resxFile)) {
				IDictionaryEnumerator en = r.GetEnumerator();
				// Goes through the enumerator, printing out the key and value pairs.
				while (en.MoveNext()) {
					if (object.Equals(en.Value, text)) {
						SetText(textEditor, en.Key.ToString(), text);
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
			
			SetText(textEditor, resourceName, text);
			
			string path = Path.GetFullPath(Path.Combine(sdSrcPath, "Tools/StringResourceTool/bin/Debug"));
			ProcessStartInfo info = new ProcessStartInfo(path + "\\StringResourceTool.exe",
			                                             "\"" + resourceName + "\" "
			                                             + "\"" + text + "\" "
			                                             + "\"" + purpose + "\"");
			info.WorkingDirectory = path;
			try {
				Process.Start(info);
			} catch (Exception ex) {
				MessageService.ShowException(ex, "Error starting " + info.FileName);
			}
		}
		
		void SetText(ITextEditor textEditor, string resourceName, string oldText)
		{
			// ensure caret is at start of selection / deselect text
			textEditor.Select(textEditor.SelectionStart, 0);
			// replace the selected text with the new text:
			string newText;
			if (Path.GetExtension(textEditor.FileName) == ".xaml")
				newText = "{core:Localize " + resourceName + "}";
			else
				newText = "$" + "{res:" + resourceName + "}";
			// Replace() takes the arguments: start offset to replace, length of the text to remove, new text
			textEditor.Document.Replace(textEditor.Caret.Offset, oldText.Length, newText);
		}
	}
}
