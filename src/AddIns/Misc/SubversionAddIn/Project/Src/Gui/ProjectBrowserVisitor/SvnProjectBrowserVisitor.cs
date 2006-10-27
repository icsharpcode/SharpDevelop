// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Svn
{
	/// <summary>
	/// Description of SvnProjectBrowserVisitor.
	/// </summary>
	public class SvnProjectBrowserVisitor : ProjectBrowserTreeNodeVisitor
	{
		public override object Visit(SolutionNode node, object data)
		{
			if (Commands.RegisterEventsCommand.CanBeVersionControlledDirectory(node.Solution.Directory)) {
				OverlayIconManager.Enqueue(node);
			}
			return node.AcceptChildren(this, data);
		}
		
		public override object Visit(ProjectNode node, object data)
		{
			return Visit((DirectoryNode)node, data);
		}
		
		public override object Visit(DirectoryNode node, object data)
		{
			if (Commands.RegisterEventsCommand.CanBeVersionControlledDirectory(node.Directory)) {
				OverlayIconManager.Enqueue(node);
				return node.AcceptChildren(this, data);
			}
			return data;
		}
		
		public override object Visit(FileNode node, object data)
		{
			OverlayIconManager.Enqueue(node);
			return node.AcceptChildren(this, data);
		}
	}
}
