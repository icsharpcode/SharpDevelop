// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Base class for the AddWixLibraryToProject and 
	/// AddWixExtensionToProject commands.
	/// </summary>
	public abstract class AddWixItemToProjectBaseCommand : AbstractMenuCommand
	{		
		/// <summary>
		/// Gets the file filter to be used with the open file dialog.
		/// </summary>
		public abstract string FileFilter { get; }
		
		/// <summary>
		/// Gets the folder tree node type to find from the
		/// project node.</summary>
		public abstract Type FolderNodeType { get; }
		
		public override void Run()
		{
			// Get FolderNode.
			CustomFolderNode folderNode = GetFolderNode(FolderNodeType);
			if (folderNode == null) {
				return;
			}
			
			// Display file dialog.
			using (OpenFileDialog fileDialog = CreateOpenFileDialog(FileFilter)) {
				if (DialogResult.OK == fileDialog.ShowDialog(WorkbenchSingleton.MainWin32Window)) {
					// Add files to project.
					WixProject project = ((WixProject)folderNode.Project);
					AddFiles(project, fileDialog.FileNames);
					project.Save();
				}
			}
			
			// Refresh project browser.
			folderNode.Refresh();
			folderNode.Expanding();
			folderNode.Expand();
		}

		/// <summary>
		/// Method used to add the files selected by the user to the project.
		/// </summary>
		protected abstract void AddFiles(WixProject project, string[] fileNames);		
		
		static CustomFolderNode GetFolderNode(Type folderNodeType)
		{
			ProjectNode projectNode = ProjectBrowserPad.Instance.CurrentProject;
			if (projectNode != null) {
				return GetFolderNodeFromProjectNode(projectNode, folderNodeType);
			}
			return null;
		}
		
		static CustomFolderNode GetFolderNodeFromProjectNode(ProjectNode projectNode, Type folderNodeType)
		{
			foreach (TreeNode node in projectNode.Nodes) {
				CustomFolderNode folderNode = node as CustomFolderNode;
				if (folderNode != null && folderNode.GetType() == folderNodeType) {
					return folderNode;
				}
			}
			return null;
		}
		
		static OpenFileDialog CreateOpenFileDialog(string fileFilter)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.AddExtension    = true;
			dialog.FilterIndex     = 0;
			dialog.Filter          = StringParser.Parse(fileFilter);
			dialog.Multiselect     = true;
			dialog.CheckFileExists = true;
			dialog.Title = StringParser.Parse("${res:ProjectComponent.ContextMenu.AddExistingFiles}");
			return dialog;
		}
		
	}
}
