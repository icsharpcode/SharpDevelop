// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public static class NodeBuilders
	{
		/// <summary>
		/// This method builds a ProjectBrowserNode Tree out of a given combine.
		/// </summary>
		public static TreeNode AddProjectNode(TreeNode motherNode, IProject project)
		{
			IProjectNodeBuilder   projectNodeBuilder = null;
			foreach (IProjectNodeBuilder nodeBuilder in AddInTree.BuildItems<IProjectNodeBuilder>("/SharpDevelop/Views/ProjectBrowser/NodeBuilders", null, true)) {
				if (nodeBuilder.CanBuildProjectTree(project)) {
					projectNodeBuilder = nodeBuilder;
					break;
				}
			}
			if (projectNodeBuilder != null) {
				return projectNodeBuilder.AddProjectNode(motherNode, project);
			}

			throw new NotImplementedException("can't create node builder for project type " + project.Language);
		}
	}
}
