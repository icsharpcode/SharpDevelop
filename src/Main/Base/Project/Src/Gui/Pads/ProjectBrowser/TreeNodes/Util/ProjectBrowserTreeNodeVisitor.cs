using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ProjectBrowserTreeNodeVisitor
	{
		public object Visit(AbstractProjectBrowserTreeNode abstractProjectBrowserTreeNode, object data)
		{
			Console.WriteLine("Warning visited default Visit() for : " + abstractProjectBrowserTreeNode);
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
