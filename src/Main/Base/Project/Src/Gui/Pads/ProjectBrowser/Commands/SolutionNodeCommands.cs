// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
			IProject newProject;
			ILanguageBinding binding = LanguageBindingService.GetBindingPerProjectFile(fileName);
			if (binding != null) {
				newProject = binding.LoadProject(fileName, Path.GetFileNameWithoutExtension(fileName));
			} else {
				newProject = new UnknownProject(fileName);
				newProject.IdGuid = "{" + Guid.NewGuid().ToString() + "}";
			}
			newProject.Location = FileUtility.GetRelativePath(solutionFolderNode.Solution.Directory, fileName);
			solutionFolderNode.Container.AddFolder(newProject);
			new DefaultDotNetNodeBuilder().AddProjectNode((TreeNode)solutionFolderNode, newProject).EnsureVisible();
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
		
//	public class AddNewSolutionToSolution : AbstractMenuCommand
//	{
//		public override void Run()
//		{
////			ProjectBrowserView browser = (ProjectBrowserView)Owner;
////			CombineBrowserNode node    = browser.SelectedNode as CombineBrowserNode;
////			
////			if (node != null) {
////				using (NewProjectDialog npdlg = new NewProjectDialog(false)) {
////					if (npdlg.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
////						node.Nodes.Add(ProjectBrowserView.BuildCombineTreeNode((Combine)node.Combine.AddEntry(npdlg.NewCombineLocation)));
////						ProjectService.SaveCombine();
////					}
////				}
////			}
//		}
//	}
//	
//		
//	public class AddSolutionToSolution : AbstractMenuCommand
//	{
//		public override void Run()
//		{
////			ProjectBrowserView browser = (ProjectBrowserView)Owner;
////			CombineBrowserNode node    = browser.SelectedNode as CombineBrowserNode;
////			
////			if (node != null) {
////				using (OpenFileDialog fdiag = new OpenFileDialog()) {
////					fdiag.AddExtension    = true;
////					
////					fdiag.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.CombineFiles}|*.cmbx|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
////					fdiag.Multiselect     = false;
////					fdiag.CheckFileExists = true;
////					if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
////						object obj = node.Combine.AddEntry(fdiag.FileName);
////						if(obj != null) {
////							if (obj is IProject) {
////								node.Nodes.Add(ProjectBrowserView.BuildProjectTreeNode((IProject)obj));
////							} else {
////								node.Nodes.Add(ProjectBrowserView.BuildCombineTreeNode((Combine)obj));
////							}
////							ProjectService.SaveCombine();
////						}
////					}
////				}
////			}
//		}
//	}
	
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
				SolutionFolder newSolutionFolder = solutionFolderNode.Solution.CreateFolder("New Folder");
				solutionFolderNode.Container.AddFolder(newSolutionFolder);
				solutionFolderNode.Solution.Save();
				
				SolutionFolderNode newSolutionFolderNode = new SolutionFolderNode(solutionFolderNode.Solution, newSolutionFolder);
				newSolutionFolderNode.AddTo(node);
				ProjectBrowserPad.Instance.StartLabelEdit(newSolutionFolderNode);
			}
		}
	}
	
	
//	This will be part of the SolutionNode.ShowProperties() method:
//	public class CombineOptions : AbstractMenuCommand
//	{
//		public override void Run()
//		{
//			
//			ProjectBrowserView browser = (ProjectBrowserView)Owner;
//			CombineBrowserNode node    = browser.SelectedNode as CombineBrowserNode;
//			
//			if (node != null) {
//				Properties defaultProperties = new Properties();
//				defaultProperties.Set("Combine", node.Combine);
//				using (TreeViewOptions optionsDialog = new TreeViewOptions(defaultProperties,
//				                                                           AddInTree.GetTreeNode("/SharpDevelop/Workbench/CombineOptions"))) {
////					optionsDialog.Size = new Size(700, 450);
//					optionsDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
//							
//					optionsDialog.Owner = (Form)WorkbenchSingleton.Workbench;
//					optionsDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
//					ProjectService.SaveCombine();
//				}
//			}
//		}
//	}
}
