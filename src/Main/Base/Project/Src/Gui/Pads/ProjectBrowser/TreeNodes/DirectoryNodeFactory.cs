// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Gui.Pads.ProjectBrowser.TreeNodes;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Static factory methods for creating Directory nodes.
	/// </summary>
	public static class DirectoryNodeFactory
	{
		public static DirectoryNode CreateDirectoryNode(TreeNode parent, IProject project, string directory)
		{
			DirectoryNode node = new DirectoryNode(directory);
			if (!string.IsNullOrEmpty(project.AppDesignerFolder)
			    && directory == Path.Combine(project.Directory, project.AppDesignerFolder))
			{
				node.SpecialFolder = SpecialFolder.AppDesigner;
			} else if (DirectoryNode.IsWebReferencesFolder(project, directory)) {
				node = new WebReferencesFolderNode(directory);
			} else if (parent != null && parent is WebReferencesFolderNode) {
				node = new WebReferenceNode(directory);
			} else if (ServiceReferencesProjectItem.IsServiceReferencesFolder(project, directory)) {
				node = new ServiceReferencesFolderNode(directory);
			} else if (parent is ServiceReferencesFolderNode) {
				node = new ServiceReferenceNode(directory);
			}
			return node;
		}
		
		public static DirectoryNode CreateDirectoryNode(ProjectItem item, FileNodeStatus status)
		{
			DirectoryNode node;
			if (item is WebReferencesProjectItem) {
				node = new WebReferencesFolderNode((WebReferencesProjectItem)item);
				node.FileNodeStatus = status;
			} else if (item is ServiceReferencesProjectItem) {
				node = new ServiceReferencesFolderNode(item, status);
			} else {
				node = new DirectoryNode(item.FileName.Trim('\\', '/'), status);
				node.ProjectItem = item;
			}
			return node;
		}
	}
}
