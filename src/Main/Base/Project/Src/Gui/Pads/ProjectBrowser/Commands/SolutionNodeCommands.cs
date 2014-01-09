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
			if (solutionFolderNode != null) {
				SD.UIService.ShowNewProjectDialog(solutionFolderNode.Folder);
			}
		}
		
		internal static string GetInitialDirectorySuggestion(ISolutionFolder solutionFolder)
		{
			// Detect the correct folder to place the new project in:
			int projectCount = 0;
			string initialDirectory = null;
			foreach (ISolutionItem folderEntry in solutionFolder.Items) {
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
				return solutionFolder.ParentSolution.Directory;
			}
		}
	}
	
	public class AddExistingProjectToSolution : AbstractMenuCommand
	{
		public override void Run()
		{
			AbstractProjectBrowserTreeNode node = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedNode;
			ISolutionFolderNode solutionFolderNode = node as ISolutionFolderNode;
			if (solutionFolderNode != null) {
				using (OpenFileDialog fdiag = new OpenFileDialog()) {
					fdiag.AddExtension    = true;
					fdiag.Filter = ProjectService.GetAllProjectsFilter(this, false);
					fdiag.Multiselect     = true;
					fdiag.CheckFileExists = true;
					fdiag.InitialDirectory = AddNewProjectToSolution.GetInitialDirectorySuggestion(solutionFolderNode.Folder);
					if (fdiag.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK) {
						try {
							foreach (string fileName in fdiag.FileNames) {
								solutionFolderNode.Folder.AddExistingProject(FileName.Create(fileName));
							}
						} catch (ProjectLoadException ex) {
							MessageService.ShowError(ex.Message);
						} catch (IOException ex) {
							MessageService.ShowError(ex.Message);
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
					if (fdiag.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK) {
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
				ISolutionFolder newSolutionFolder = solutionFolderNode.Folder.CreateFolder(ResourceService.GetString("ProjectComponent.NewFolderString"));
				solutionFolderNode.Solution.Save();
				
				SolutionFolderNode newSolutionFolderNode = new SolutionFolderNode(newSolutionFolder);
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
