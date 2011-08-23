// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project.Dialogs;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class CreateNewSolution : AbstractMenuCommand
	{
		public override void Run()
		{
			using (NewProjectDialog npdlg = new NewProjectDialog(true)) {
				npdlg.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWin32Window);
			}
		}
	}
	
	public class LoadSolution : AbstractMenuCommand
	{
		public override void Run()
		{
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.AddExtension    = true;
				fdiag.Filter          = ProjectService.GetAllProjectsFilter(this, true);
				fdiag.Multiselect     = false;
				fdiag.CheckFileExists = true;
				if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
					ProjectService.LoadSolutionOrProject(fdiag.FileName);
				}
			}
		}
	}
	
	public class CloseSolution : AbstractMenuCommand
	{
		public override void Run()
		{
			if (!ProjectService.IsClosingCanceled()) {
				ProjectService.SaveSolutionPreferences();
				if (WorkbenchSingleton.Workbench.CloseAllSolutionViews()) {
					ProjectService.CloseSolution();
				}
			}
		}
	}
}
