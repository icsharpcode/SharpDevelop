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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class AddExistingItemsToProject : AbstractMenuCommand
	{
		int GetFileFilterIndex(IProject project, string[] fileFilters)
		{
			if (project != null) {
				LanguageBindingDescriptor languageCodon = LanguageBindingService.GetCodonPerLanguageName(project.Language);
				for (int i = 0; i < fileFilters.Length; ++i) {
					for (int j = 0; j < languageCodon.Supportedextensions.Length; ++j) {
						if (fileFilters[i].ToUpper().IndexOf(languageCodon.Supportedextensions[j].ToUpper()) >= 0) {
							return i + 1;
						}
					}
				}
			}
			return 0;
		}
			
		public static void CopyDirectory(string directoryName, DirectoryNode node)
		{
			string copiedFileName = Path.Combine(node.Directory, Path.GetFileName(directoryName));
			if (!FileUtility.IsEqualFileName(directoryName, copiedFileName)) {
				FileUtility.DeepCopy(directoryName, copiedFileName, true);
				DirectoryNode newNode = new DirectoryNode(copiedFileName);
				newNode.AddTo(node);
				newNode.Expanding();
				IncludeFileInProject.IncludeDirectoryNode(newNode, true);
			} else {
				foreach (TreeNode childNode in node.Nodes) {
					if (childNode is DirectoryNode) {
						DirectoryNode directoryNode = (DirectoryNode)childNode;
						if (FileUtility.IsEqualFileName(directoryNode.Directory, copiedFileName)) {
							IncludeFileInProject.IncludeDirectoryNode(directoryNode, true);
						}
					}
				}
			}
		}
		
		public static void CopyFile(string fileName, DirectoryNode node, bool includeInProject)
		{
			string copiedFileName = Path.Combine(node.Directory, Path.GetFileName(fileName));
			if (!FileUtility.IsEqualFileName(fileName, copiedFileName)) {
				File.Copy(fileName, copiedFileName, true);
				FileNode newNode = new FileNode(copiedFileName);
				newNode.AddTo(node);
				if (includeInProject) {
					IncludeFileInProject.IncludeFileNode(newNode);
				}
			} else if (includeInProject) {
				foreach (TreeNode childNode in node.Nodes) {
					if (childNode is FileNode) {
						FileNode fileNode = (FileNode)childNode;
						if (FileUtility.IsEqualFileName(fileNode.FileName, copiedFileName)) {
							IncludeFileInProject.IncludeFileNode(fileNode);
						}
					}
				}
			}
		}
		
		public override void Run()
		{
			TreeNode selectedNode = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedNode;
			DirectoryNode node = selectedNode as DirectoryNode;
			if (node == null) {
				node = selectedNode.Parent as DirectoryNode;
			}
			if (node == null) {
				return;
			}
			node.Expanding();
			node.Expand();
			
			using (OpenFileDialog fdiag  = new OpenFileDialog()) {
				fdiag.AddExtension    = true;
				string[] fileFilters  = (string[])(AddInTree.GetTreeNode("/SharpDevelop/Workbench/FileFilter").BuildChildItems(this)).ToArray(typeof(string));
				
				fdiag.FilterIndex     = GetFileFilterIndex(node.Project, fileFilters);
				fdiag.Filter          = String.Join("|", fileFilters);
				fdiag.Multiselect     = true;
				fdiag.CheckFileExists = true;
				
				if (fdiag.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					foreach (string fileName in fdiag.FileNames) {
						 CopyFile(fileName, node, true);
					}
				}
			}
			ProjectService.SaveSolution();
		}
	}
	
	public class AddNewItemsToProject : AbstractMenuCommand
	{
		
		FileNode CreateNewFile(DirectoryNode upper, string fileName)
		{
			upper.Expanding();
			
			FileNode fileNode = new FileNode(fileName, FileNodeStatus.InProject);
			fileNode.AddTo(upper);
			fileNode.EnsureVisible();
			IncludeFileInProject.IncludeFileNode(fileNode);
			return fileNode;
		}
		
		public override void Run()
		{
			TreeNode selectedNode = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedNode;
			DirectoryNode node = selectedNode as DirectoryNode;
			if (node == null) {
				node = selectedNode.Parent as DirectoryNode;
			}
			if (node == null) {
				return;
			}
			node.Expand();
			node.Expanding();
			
			using (NewFileDialog nfd = new NewFileDialog(node.Directory)) {
				if (nfd.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					foreach (string createdFile in nfd.CreatedFiles) {
						CreateNewFile(node, createdFile);
					}
				}
			}
			
			ProjectService.SaveSolution();
		}
	}
	
	public class AddNewFolderToProject : AbstractMenuCommand
	{
		string GenerateValidDirectoryName(string inDirectory)
		{
			string directoryName = Path.Combine(inDirectory, ResourceService.GetString("ProjectComponent.NewFolderString"));
			
			if (Directory.Exists(directoryName)) {
				int index = 1;
				while (Directory.Exists(directoryName + index)) {
					index++;
				}
				return directoryName + index;
			}
			return directoryName;
		}
		
		DirectoryNode CreateNewDirectory(DirectoryNode upper, string directoryName)
		{
			upper.Expanding();
			Directory.CreateDirectory(directoryName);
			
			DirectoryNode directoryNode = new DirectoryNode(directoryName, FileNodeStatus.InProject);
			directoryNode.AddTo(upper);
			
			IncludeFileInProject.IncludeDirectoryNode(directoryNode, false);
			return directoryNode;
		}
		
		public override void Run()
		{
			TreeNode selectedNode = ProjectBrowserPad.Instance.ProjectBrowserControl.SelectedNode;
			DirectoryNode node = selectedNode as DirectoryNode;
			if (node == null) {
				node = selectedNode.Parent as DirectoryNode;
			}
			if (node == null) {
				return;
			}
			node.Expanding();
			string newDirectoryName = GenerateValidDirectoryName(node.Directory);
			DirectoryNode newDirectoryNode = CreateNewDirectory(node, newDirectoryName);
			ProjectBrowserPad.Instance.StartLabelEdit(newDirectoryNode);
			ProjectService.SaveSolution();
		}
	}
}
