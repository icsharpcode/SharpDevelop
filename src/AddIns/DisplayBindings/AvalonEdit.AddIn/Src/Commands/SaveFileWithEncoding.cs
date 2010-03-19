// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
