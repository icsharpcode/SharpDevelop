// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Robert Zaunere" email=""/>
//     <version>$Revision$</version>
// </file>

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
