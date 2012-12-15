// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project.Dialogs;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class AddNewProjectToSolution : AbstractMenuCommand
	{
		public override void Run()
		{
			TreeNode node = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedNode;
			while (node != null && !(node is ISolutionFolderNode))
				node = node.Parent;
			ISolutionFolderNode solutionFolderNode = node as ISolutionFolderNode;
			if (node != null) {
				using (NewProjectDialog npdlg = new NewProjectDialog(false)) {
					npdlg.SolutionFolderNode = solutionFolderNode;
					npdlg.InitialProjectLocationDirectory = GetInitialDirectorySuggestion(solutionFolderNode);
					
					// show the dialog to request project type and name
					if (npdlg.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
						if (npdlg.NewProjectLocation.Length == 0) {
							MessageService.ShowError("No project has been created, there is nothing to add.");
							return;
						}
					}
				}
			}
		}
		
		internal static string GetInitialDirectorySuggestion(ISolutionFolderNode solutionFolderNode)
		{
			// Detect the correct folder to place the new project in:
			int projectCount = 0;
			string initialDirectory = null;
			foreach (ISolutionFolder folderEntry in solutionFolderNode.Container.Folders) {
				IProject project = folderEntry as IProject;
				if (project != null) {
					if (projectCount == 0)
						initialDirectory = project.Directory;
					else
						initialDirectory = FileUtility.GetCommonBaseDirectory(initialDirectory, project.Directory);
					projectCount++;
				}
			}
			if (initialDirectory != null) {
				if (projectCount == 1) {
					return FileUtility.GetAbsolutePath(initialDirectory, "..");
				} else {
					return initialDirectory;
				}
			} else {
				return solutionFolderNode.Solution.Directory;
			}
		}
	}
	
	public class AddExistingProjectToSolution : AbstractMenuCommand
	{
		public static void AddProject(ISolutionFolderNode solutionFolderNode, string fileName)
		{
			if (solutionFolderNode == null)
				throw new ArgumentNullException("solutionFolderNode");
			ProjectLoadInformation loadInfo = new ProjectLoadInformation(solutionFolderNode.Solution, fileName, Path.GetFileNameWithoutExtension(fileName));
			AddProject(solutionFolderNode, ProjectBindingService.LoadProject(loadInfo));
		}
		
		public static void AddProject(ISolutionFolderNode solutionFolderNode, IProject newProject)
		{
			if (solutionFolderNode == null)
				throw new ArgumentNullException("solutionFolderNode");
			if (newProject != null) {
				newProject.Location = FileUtility.GetRelativePath(solutionFolderNode.Solution.Directory, newProject.FileName);
				ProjectService.AddProject(solutionFolderNode, newProject);
				NodeBuilders.AddProjectNode((TreeNode)solutionFolderNode, newProject).EnsureVisible();
				solutionFolderNode.Solution.ApplySolutionConfigurationAndPlatformToProjects();
			}
		}
		
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedNode;
			ISolutionFolderNode solutionFolderNode = node as ISolutionFolderNode;
			if (node != null) {
				using (OpenFileDialog fdiag = new OpenFileDialog()) {
					fdiag.AddExtension    = true;
					fdiag.Filter = ProjectService.GetAllProjectsFilter(this, false);
					fdiag.Multiselect     = true;
					fdiag.CheckFileExists = true;
					fdiag.InitialDirectory = AddNewProjectToSolution.GetInitialDirectorySuggestion(solutionFolderNode);
					if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
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
					if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
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
				newSolutionFolderNode.InsertSorted(node);
				ProjectBrowserPad.Instance.StartLabelEdit(newSolutionFolderNode);
			}
		}
	}
	
	public class CollapseAll : AbstractMenuCommand
	{
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedNode;
			if (node is SolutionNode) {
				// Solution node does not collapse fully, but its subitems stay visible
				foreach (var subNode in node.Nodes) {
					var subBrowserNode = subNode as AbstractProjectBrowserTreeNode;
					if (subBrowserNode != null) {
						subBrowserNode.Collapse();
					}
				}
			} else {
				node.Collapse();
			}
		}
	}
}
