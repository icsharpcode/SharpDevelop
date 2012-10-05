// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

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
