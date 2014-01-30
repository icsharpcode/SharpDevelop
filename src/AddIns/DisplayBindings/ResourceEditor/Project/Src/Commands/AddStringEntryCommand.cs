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

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ResourceEditor
{
	class AddStringCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ResourceEditorControl editor = ((ResourceEditWrapper)SD.Workbench.ActiveViewContent).ResourceEditor;
			
			if(editor.ResourceList.WriteProtected) {
				return;
			}
			
			int count = 1;
			string newNameBase = " new string entry ";
			string newName = newNameBase + count.ToString();
			string type = "System.String";
			
			while(editor.ResourceList.Resources.ContainsKey(newName)) {
				count++;
				newName = newNameBase + count.ToString();
			}
			
			ResourceItem item = new ResourceItem(newName, "");
			editor.ResourceList.Resources.Add(newName, item);
			ListViewItem lv = new ListViewItem(new string[] { newName, type, "" }, item.ImageIndex);
			editor.ResourceList.Items.Add(lv);
			editor.ResourceList.OnChanged();
			lv.BeginEdit();
		}
	}
}
