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
