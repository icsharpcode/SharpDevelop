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
