// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Project;
using System.Windows.Forms;

namespace FSharpBinding
{
	public class FSharpProjectNodeBuilder : IProjectNodeBuilder
	{
		public bool CanBuildProjectTree(IProject project)
		{
			return project is FSharpProject;
		}
		
		public TreeNode AddProjectNode(TreeNode motherNode, IProject project)
		{
			FSharpProjectNode prjNode = new FSharpProjectNode(project);
			prjNode.AddTo(motherNode);
			new ReferenceFolder(project).AddTo(prjNode);
			return prjNode;
		}
	}
}
