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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public class FileRemoveTreeNodeVisitor : ProjectBrowserTreeNodeVisitor
	{
		string fileName;
		
		public FileRemoveTreeNodeVisitor(string fileName)
		{
			this.fileName = fileName;
		}
		
		public override object Visit(SolutionItemNode solutionItemNode, object data)
		{
			if (FileUtility.IsBaseDirectory(fileName, solutionItemNode.FileName)) {
				solutionItemNode.Remove();
			} else {
				solutionItemNode.AcceptChildren(this, data);
			}
			return data;
		}
		
		
		public override object Visit(ProjectNode projectNode, object data)
		{
			if (FileUtility.IsBaseDirectory(projectNode.Directory, fileName)) {
				projectNode.AcceptChildren(this, data);
			}
			return data;
		}
		
		public override object Visit(DirectoryNode directoryNode, object data)
		{
			if (FileUtility.IsBaseDirectory(fileName, directoryNode.Directory)) {
				ExtTreeNode parent = directoryNode.Parent as ExtTreeNode;
				directoryNode.Remove();
				if (parent != null) {
					parent.Refresh();
				}
			} else {
				if (FileUtility.IsBaseDirectory(directoryNode.Directory, fileName)) {
					directoryNode.AcceptChildren(this, data);
				}
			}
			return data;
		}
		
		public override object Visit(FileNode fileNode, object data)
		{
			if (FileUtility.IsBaseDirectory(fileName, fileNode.FileName)) {
				ExtTreeNode parent = fileNode.Parent as ExtTreeNode;
				fileNode.Remove();
				if (parent != null) {
					parent.Refresh();
				}
			} else {
				fileNode.AcceptChildren(this, data);
			}
			return data;
		}
	}
}
