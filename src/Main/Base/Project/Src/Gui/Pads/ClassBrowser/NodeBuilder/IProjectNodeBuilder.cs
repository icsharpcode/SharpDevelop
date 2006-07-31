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
	public interface IProjectNodeBuilder
	{
		bool CanBuildProjectTree(IProject project);
		TreeNode AddProjectNode(ExtTreeView classBrowser, IProject project);
	}
}
