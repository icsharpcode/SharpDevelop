// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
