// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Dialogs;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class CreateNewSolution : AbstractMenuCommand
	{
		public override void Run()
		{
			
			
			using (NewProjectDialog npdlg = new NewProjectDialog(true)) {
				npdlg.Owner = (Form)WorkbenchSingleton.Workbench;
				npdlg.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
			}
		}
	}
	
	public class LoadSolution : AbstractMenuCommand
	{
		public override void Run()
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.AddExtension    = true;
				fdiag.Filter          = String.Join("|", (string[])(AddInTree.GetTreeNode("/SharpDevelop/Workbench/Combine/FileFilter").BuildChildItems(this)).ToArray(typeof(string)));
				fdiag.Multiselect     = false;
				fdiag.CheckFileExists = true;
				if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					switch (Path.GetExtension(fdiag.FileName).ToUpper()) {
						case ".CMBX":
						case ".SLN":
							ProjectService.LoadSolution(fdiag.FileName);
							break;
//						case ".PRJX":
//							
//							
//							FileUtility.ObservedLoad(new NamedFileOperationDelegate(ProjectService.OpenSolution), fdiag.FileName);
//							break;
						default:
							
							
							MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.InvalidProjectOrCombine}", new string[,] {{"FileName", fdiag.FileName}}));
							break;
					}
				}
			}
		}
	}
	
	public class CloseSolution : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectService.CloseSolution();
		}
	}
}
