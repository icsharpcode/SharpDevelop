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

namespace ICSharpCode.SharpDevelop.Project
{
	public class ProjectBrowserTreeNodeVisitor
	{
		public object Visit(AbstractProjectBrowserTreeNode abstractProjectBrowserTreeNode, object data)
		{
			LoggingService.Warn("Warning visited default Visit() for : " + abstractProjectBrowserTreeNode);
			abstractProjectBrowserTreeNode.AcceptChildren(this, data);
			return data;
		}
		
		public virtual object Visit(SolutionNode solutionNode, object data)
		{
			solutionNode.AcceptChildren(this, data);
			return data;
		}
		
		public virtual object Visit(SolutionFolderNode solutionFolderNode, object data)
		{
			solutionFolderNode.AcceptChildren(this, data);
			return data;
		}
		
		public virtual object Visit(SolutionItemNode solutionItemNode, object data)
		{
			solutionItemNode.AcceptChildren(this, data);
			return data;
		}
		
		public virtual object Visit(ProjectNode projectNode, object data)
		{
			projectNode.AcceptChildren(this, data);
			return data;
		}
		
		public virtual object Visit(DirectoryNode directoryNode, object data)
		{
			directoryNode.AcceptChildren(this, data);
			return data;
		}
		
		public virtual object Visit(FileNode fileNode, object data)
		{
			fileNode.AcceptChildren(this, data);
			return data;
		}
		
		public virtual object Visit(CustomFolderNode customFolderNode, object data)
		{
			customFolderNode.AcceptChildren(this, data);
			return data;
		}
		
		public virtual object Visit(CustomNode customNode, object data)
		{
			customNode.AcceptChildren(this, data);
			return data;
		}
		public virtual object Visit(ReferenceFolder referenceFolder, object data)
		{
			referenceFolder.AcceptChildren(this, data);
			return data;
		}
		public virtual object Visit(ReferenceNode referenceNode, object data)
		{
			referenceNode.AcceptChildren(this, data);
			return data;
		}
		
		
	}
}
