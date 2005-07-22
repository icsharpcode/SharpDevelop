// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
