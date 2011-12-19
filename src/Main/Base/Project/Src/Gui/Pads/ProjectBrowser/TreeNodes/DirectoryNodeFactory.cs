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
			if (IsAppDesignerFolder(project, directory)) {
				return new DirectoryNode(directory) { SpecialFolder = SpecialFolder.AppDesigner };
			} else if (WebReferencesProjectItem.IsWebReferencesFolder(project, directory)) {
				return new WebReferencesFolderNode(directory);
			} else if (parent is WebReferencesFolderNode) {
				return new WebReferenceNode(directory);
			} else if (ServiceReferencesProjectItem.IsServiceReferencesFolder(project, directory)) {
				return new ServiceReferencesFolderNode(directory);
			} else if (parent is ServiceReferencesFolderNode) {
				return new ServiceReferenceNode(directory);
			}
			return new DirectoryNode(directory);
		}
		
		static bool IsAppDesignerFolder(IProject project, string directory)
		{
			return !String.IsNullOrEmpty(project.AppDesignerFolder) && 
				directory == Path.Combine(project.Directory, project.AppDesignerFolder);
		}
		
		public static DirectoryNode CreateDirectoryNode(ProjectItem item, FileNodeStatus status)
		{
			if (item is WebReferencesProjectItem) {
				return new WebReferencesFolderNode((WebReferencesProjectItem)item, status);
			} else if (item is ServiceReferencesProjectItem) {
				return new ServiceReferencesFolderNode(item, status);
			}
			return new DirectoryNode(item.FileName, status, item);
		}
	}
}
