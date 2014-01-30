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
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
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
				if (DialogResult.OK == fileDialog.ShowDialog(SD.WinForms.MainWin32Window)) {
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
