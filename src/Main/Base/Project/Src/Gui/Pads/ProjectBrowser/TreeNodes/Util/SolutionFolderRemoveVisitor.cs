// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public class SolutionFolderRemoveVisitor : ProjectBrowserTreeNodeVisitor
	{
		ISolutionFolder folder;
		
		public SolutionFolderRemoveVisitor(ISolutionFolder folder)
		{
			this.folder = folder;
		}
		
		public override object Visit(SolutionFolderNode solutionFolderNode, object data)
		{
			if (folder == solutionFolderNode.Folder) {
				ExtTreeNode parent = solutionFolderNode.Parent as ExtTreeNode;
				solutionFolderNode.Remove();
				if (parent != null) {
					parent.Refresh();
				}
			} else {
				solutionFolderNode.AcceptChildren(this, data);
			}
			return data;
		}
		
		public override object Visit(ProjectNode projectNode, object data)
		{
			if (folder == projectNode.Project) {
				ExtTreeNode parent = projectNode.Parent as ExtTreeNode;
				projectNode.Remove();
				if (parent != null) {
					parent.Refresh();
				}
			}
			return data;
		}
		
		
	}
}
