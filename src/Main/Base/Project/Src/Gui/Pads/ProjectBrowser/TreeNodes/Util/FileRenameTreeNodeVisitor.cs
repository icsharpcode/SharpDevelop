// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public class FileRenameTreeNodeVisitor : ProjectBrowserTreeNodeVisitor
	{
		string oldName;
		string newName;
		
		public FileRenameTreeNodeVisitor(string oldName, string newName)
		{
			this.oldName = oldName;
			this.newName = newName;
		}
		
		public override object Visit(SolutionItemNode solutionItemNode, object data)
		{
			if (FileUtility.IsEqualFileName(oldName, solutionItemNode.FileName)) {
				solutionItemNode.Text = Path.GetFileName(newName);
			}
			solutionItemNode.AcceptChildren(this, data);
			return data;
		}
		
		
		public override object Visit(ProjectNode projectNode, object data)
		{
			if (FileUtility.IsBaseDirectory(oldName, projectNode.Directory) ||
			    FileUtility.IsBaseDirectory(projectNode.Directory, oldName)) {
				projectNode.AcceptChildren(this, data);
			}
			return data;
		}
		
		public override object Visit(DirectoryNode directoryNode, object data)
		{
			if (FileUtility.IsBaseDirectory(oldName, directoryNode.Directory)) {
				directoryNode.Directory = FileUtility.RenameBaseDirectory(directoryNode.Directory, oldName, newName);
				directoryNode.AcceptChildren(this, data);
			} else if (FileUtility.IsBaseDirectory(directoryNode.Directory, oldName)) {
				directoryNode.AcceptChildren(this, data);
			}
			return data;
		}
		
		public override object Visit(FileNode fileNode, object data)
		{
			if (FileUtility.IsEqualFileName(oldName, fileNode.FileName)) {
				fileNode.FileName = newName;
			} else if (FileUtility.IsBaseDirectory(oldName, fileNode.FileName)) {
				fileNode.FileName = FileUtility.RenameBaseDirectory(fileNode.FileName, oldName, newName);
			}
			fileNode.AcceptChildren(this, data);
			return data;
		}
	}
}
