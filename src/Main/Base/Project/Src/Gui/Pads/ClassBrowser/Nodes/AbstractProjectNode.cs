// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.ClassBrowser
{
	public abstract class AbstractProjectNode : ExtTreeNode
	{
		IProject project;

		public IProject Project {
			get {
				return project;
			}
		}

		protected AbstractProjectNode()
		{
			this.project = null;
		}

		public AbstractProjectNode(IProject project)
		{
			this.project = project;
		}

		public abstract void UpdateParseInformation(ICompilationUnit oldUnit, ICompilationUnit unit);
		public abstract TreeNode GetNodeByPath(string directory, bool create);
	}
}
