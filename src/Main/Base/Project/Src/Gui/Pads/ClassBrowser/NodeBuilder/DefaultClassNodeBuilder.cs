// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.ClassBrowser
{
	public class DefaultClassNodeBuilder : IClassNodeBuilder
	{
		public bool CanBuildClassTree(IClass c)
		{
			return true;
		}

		public TreeNode AddClassNode(ExtTreeView classBrowser, IProject project, IClass c)
		{
			ClassNode cNode = new ClassNode(project, c);
			cNode.AddTo(classBrowser);
			return cNode;
		}
	}
}
