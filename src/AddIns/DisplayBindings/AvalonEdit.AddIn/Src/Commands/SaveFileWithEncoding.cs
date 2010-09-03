// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
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
			IViewContent vc = WorkbenchSingleton.Workbench.ActiveViewContent;
			ICodeEditorProvider cep = vc as ICodeEditorProvider;
			if (cep != null) {
				ChooseEncodingDialog dlg = new ChooseEncodingDialog();
				dlg.Owner = WorkbenchSingleton.MainWindow;
				dlg.Encoding = cep.CodeEditor.PrimaryTextEditor.Encoding;
				if (dlg.ShowDialog() == true) {
					cep.CodeEditor.PrimaryTextEditor.Encoding = dlg.Encoding;
					SharpDevelop.Commands.SaveFile.Save(vc.PrimaryFile);
				}
			}
		}
	}
}
