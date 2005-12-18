// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Project
{
	public interface IProjectNodeBuilder
	{
		bool CanBuildProjectTree(IProject project);
		TreeNode AddProjectNode(TreeNode motherNode, IProject project);
	}
}
