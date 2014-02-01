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
using ICSharpCode.SharpDevelop.Gui;
using System.IO;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	public class ShowColorDialog : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditor textEditor = SD.GetActiveViewContentService<ITextEditor>();
			if (textEditor == null)
				return;
			
			using (SharpDevelopColorDialog cd = new SharpDevelopColorDialog()) {
				if (cd.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK) {
					string ext = Path.GetExtension(textEditor.FileName).ToLowerInvariant();
					string colorstr;
					if (ext == ".cs" || ext == ".vb" || ext == ".boo") {
						if (cd.Color.IsKnownColor) {
							colorstr = "Color." + cd.Color.ToKnownColor().ToString();
						} else if (cd.Color.A < 255) {
							colorstr = "Color.FromArgb(0x" + cd.Color.ToArgb().ToString("x") + ")";
						} else {
							colorstr = string.Format("Color.FromArgb({0}, {1}, {2})", cd.Color.R, cd.Color.G, cd.Color.B);
						}
					} else {
						if (cd.Color.IsKnownColor) {
							colorstr = cd.Color.ToKnownColor().ToString();
						} else if (cd.Color.A < 255) {
							colorstr = "#" + cd.Color.ToArgb().ToString("X");
						} else {
							colorstr = string.Format("#{0:X2}{1:X2}{2:X2}", cd.Color.R, cd.Color.G, cd.Color.B);
						}
					}
					
					textEditor.SelectedText = colorstr;
					textEditor.Select(textEditor.SelectionStart + textEditor.SelectionLength, 0);
				}
			}
		}
	}
}
