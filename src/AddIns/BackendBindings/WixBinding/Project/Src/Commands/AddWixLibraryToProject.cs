// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	public class AddWixLibraryToProject : AbstractMenuCommand
	{
		public override void Run()
		{
			// Get WixLibraryFolderNode.
			WixLibraryFolderNode folderNode = GetWixLibraryFolderNode();
			if (folderNode == null) {
				return;
			}
			
			// Display file dialog.
			using (OpenFileDialog fileDialog = CreateOpenFileDialog()) {
				if (DialogResult.OK == fileDialog.ShowDialog(WorkbenchSingleton.MainForm)) {
					// Add files to project.
					WixProject project = ((WixProject)folderNode.Project);
					project.AddWixLibraries(fileDialog.FileNames);
					project.Save();
				}
			}
			
			// Refresh project browser.
			folderNode.Refresh();
			folderNode.Expanding();
			folderNode.Expand();
		}
		
		static WixLibraryFolderNode GetWixLibraryFolderNode()
		{
			ProjectNode projectNode = ProjectBrowserPad.Instance.CurrentProject;
			if (projectNode != null) {
				return GetWixLibraryFolderNodeFromProjectNode(projectNode);
			}
			return null;
		}
		
		static WixLibraryFolderNode GetWixLibraryFolderNodeFromProjectNode(ProjectNode projectNode)
		{
			foreach (TreeNode node in projectNode.Nodes) {
				WixLibraryFolderNode folderNode = node as WixLibraryFolderNode;
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
			dialog.Filter          = StringParser.Parse("${res:ICSharpCode.WixBinding.AddWixLibraryToProject.WixLibraryFileFilterName} (*.wixlib)|*.wixlib|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			dialog.Multiselect     = true;
			dialog.CheckFileExists = true;
			dialog.Title = StringParser.Parse("${res:ProjectComponent.ContextMenu.AddExistingFiles}");
			return dialog;
		}
	}
}
