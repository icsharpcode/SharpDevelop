// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
			
			var textEditor = SD.GetActiveViewContentService<ITextEditor>();
			if (textEditor == null) {
				// active content is not a text editor control
				return;
			}
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
			                                                  "This should be a namespace-like construct, please see what the names of resources in the same component are.", SD.PropertyService.Get("ResourceToolLastResourceName", ""));
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
