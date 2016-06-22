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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.AvalonEdit.AddIn.Commands
{
	/// <summary>
	/// Choose encoding, then save file.
	/// </summary>
	public class SaveFileWithEncoding : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent vc = SD.Workbench.ActiveViewContent;
			if (vc == null)
				return;
			var codeEditor = vc.GetService<CodeEditor>();
			if (codeEditor != null) {
				ChooseEncodingDialog dlg = new ChooseEncodingDialog();
				dlg.Owner = SD.Workbench.MainWindow;
				dlg.Encoding = codeEditor.PrimaryTextEditor.Encoding;
				if (dlg.ShowDialog() == true) {
					codeEditor.PrimaryTextEditor.Encoding = dlg.Encoding;
					SharpDevelop.Commands.SaveFile.Save(vc.PrimaryFile);
				}
			}
		}
	}
}
