// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Robert Zaunere" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Gui.ClassBrowser
{
	public interface IClassNodeBuilder
	{
		bool CanBuildClassTree(IClass c);
		TreeNode AddClassNode(ExtTreeView classBrowser, IProject project, IClass c);
	}
}
