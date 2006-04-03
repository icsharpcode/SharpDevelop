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
