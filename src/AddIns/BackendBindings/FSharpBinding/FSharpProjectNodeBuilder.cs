// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
