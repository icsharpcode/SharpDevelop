// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project.Dialogs;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class AddNewProjectToSolution : AbstractMenuCommand
	{
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedNode;
			ISolutionFolderNode solutionFolderNode = node as ISolutionFolderNode;
			if (node != null) {
				using (NewProjectDialog npdlg = new NewProjectDialog(false)) {
					if (npdlg.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
						if (npdlg.NewProjectLocation.Length == 0) {
							MessageService.ShowError("No project has been created, there is nothing to add.");
							return;
						}
						AddExitingProjectToSolution.AddProject(solutionFolderNode, npdlg.NewProjectLocation);
						ProjectService.SaveSolution();
					}
				}
			}
		}
	}
	
	public class AddExitingProjectToSolution : AbstractMenuCommand
	{
		public static void AddProject(ISolutionFolderNode solutionFolderNode, string fileName)
		{
			IProject newProject = LanguageBindingService.LoadProject(fileName, Path.GetFileNameWithoutExtension(fileName));
			newProject.Location = FileUtility.GetRelativePath(solutionFolderNode.Solution.Directory, fileName);
			ParserService.CreateProjectContentForAddedProject(newProject);
			solutionFolderNode.Container.AddFolder(newProject);
			solutionFolderNode.Solution.FixSolutionConfiguration(new IProject[] { newProject });
			NodeBuilders.AddProjectNode((TreeNode)solutionFolderNode, newProject).EnsureVisible();
			solutionFolderNode.Solution.ApplySolutionConfigurationToProjects();
			solutionFolderNode.Solution.ApplySolutionPlatformToProjects();
		}
		
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedNode;
			ISolutionFolderNode solutionFolderNode = node as ISolutionFolderNode;
			if (node != null) {
				using (OpenFileDialog fdiag = new OpenFileDialog()) {
					fdiag.AddExtension    = true;
					fdiag.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
					fdiag.Multiselect     = true;
					fdiag.CheckFileExists = true;
					if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
						foreach (string fileName in fdiag.FileNames) {
							AddProject(solutionFolderNode, fileName);
						}
						ProjectService.SaveSolution();
					}
				}
			}
		}
	}
	
	public class AddExistingItemToSolution : AbstractMenuCommand
	{
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedNode;
			ISolutionFolderNode solutionFolderNode = node as ISolutionFolderNode;
			if (node != null) {
				using (OpenFileDialog fdiag = new OpenFileDialog()) {
					fdiag.AddExtension    = true;
					fdiag.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.AllFiles}|*.*");
					fdiag.Multiselect     = true;
					fdiag.CheckFileExists = true;
					if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
						foreach (string fileName in fdiag.FileNames) {
							solutionFolderNode.AddItem(fileName);
						}
						ProjectService.SaveSolution();
					}
				}
			}
		}
	}
	
	public class AddNewSolutionFolderToSolution : AbstractMenuCommand
	{
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedNode;
			ISolutionFolderNode solutionFolderNode = node as ISolutionFolderNode;
			if (node != null) {
				SolutionFolder newSolutionFolder = solutionFolderNode.Solution.CreateFolder(ResourceService.GetString("ProjectComponent.NewFolderString"));
				solutionFolderNode.Container.AddFolder(newSolutionFolder);
				solutionFolderNode.Solution.Save();
				
				SolutionFolderNode newSolutionFolderNode = new SolutionFolderNode(solutionFolderNode.Solution, newSolutionFolder);
				newSolutionFolderNode.AddTo(node);
				ProjectBrowserPad.Instance.StartLabelEdit(newSolutionFolderNode);
			}
		}
	}
}
