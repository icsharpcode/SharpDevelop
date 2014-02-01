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
				directoryNode.Directory = DirectoryName.Create(FileUtility.RenameBaseDirectory(directoryNode.Directory, oldName, newName));
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
