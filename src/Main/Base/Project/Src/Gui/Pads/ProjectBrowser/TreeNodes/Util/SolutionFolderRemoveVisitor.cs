// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
