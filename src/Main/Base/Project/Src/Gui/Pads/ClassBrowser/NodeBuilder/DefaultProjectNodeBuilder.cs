// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Robert Zaunere" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.ClassBrowser
{
	public class DefaultProjectNodeBuilder : IProjectNodeBuilder
	{
		public bool CanBuildProjectTree(IProject project)
		{
			return true;
		}

		public TreeNode AddProjectNode(ExtTreeView classBrowser, IProject project)
		{
			ProjectNode prjNode = new ProjectNode(project);
			prjNode.AddTo(classBrowser);
			return prjNode;
		}
	}
}
