// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	public class AddWixExtensionToProject : AbstractMenuCommand
	{
		public override void Run()
		{
			// Get WixExtensionFolderNode.
			WixExtensionFolderNode folderNode = GetWixExtensionFolderNode();
			if (folderNode == null) {
				return;
			}
			
			// Display file dialog.
			using (OpenFileDialog fileDialog = CreateOpenFileDialog()) {
				if (DialogResult.OK == fileDialog.ShowDialog(WorkbenchSingleton.MainWin32Window)) {
					// Add files to project.
					WixProject project = ((WixProject)folderNode.Project);
					project.AddWixExtensions(fileDialog.FileNames);
					project.Save();
				}
			}
			
			// Refresh project browser.
			folderNode.Refresh();
			folderNode.Expanding();
			folderNode.Expand();
		}
		
		static WixExtensionFolderNode GetWixExtensionFolderNode()
		{
			ProjectNode projectNode = ProjectBrowserPad.Instance.CurrentProject;
			if (projectNode != null) {
				return GetWixExtensionFolderNodeFromProjectNode(projectNode);
			}
			return null;
		}
		
		static WixExtensionFolderNode GetWixExtensionFolderNodeFromProjectNode(ProjectNode projectNode)
		{
			foreach (TreeNode node in projectNode.Nodes) {
				WixExtensionFolderNode folderNode = node as WixExtensionFolderNode;
				if (folderNode != null) {
					return folderNode;
				}
			}
			return null;
		}
		
		static OpenFileDialog CreateOpenFileDialog()
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.AddExtension    = true;
			dialog.FilterIndex     = 0;
			dialog.Filter          = StringParser.Parse("${res:ICSharpCode.WixBinding.AddWixExtensionToProject.WixExtensionFileFilterName} (*.dll)|*.dll|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			dialog.Multiselect     = true;
			dialog.CheckFileExists = true;
			dialog.Title = StringParser.Parse("${res:ProjectComponent.ContextMenu.AddExistingFiles}");
			return dialog;
		}
	}
}
