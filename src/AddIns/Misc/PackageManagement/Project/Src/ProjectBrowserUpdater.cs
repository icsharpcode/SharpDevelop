// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement
{
	public class ProjectBrowserUpdater : IProjectBrowserUpdater
	{
		ProjectBrowserControl projectBrowser;
		
		public ProjectBrowserUpdater()
			: this(ProjectBrowserPad.Instance.ProjectBrowserControl)
		{
		}
		
		public ProjectBrowserUpdater(ProjectBrowserControl projectBrowser)
		{
			this.projectBrowser = projectBrowser;
			ProjectService.ProjectItemAdded += ProjectItemAdded;
		}

		protected virtual void ProjectItemAdded(object sender, ProjectItemEventArgs e)
		{
			if (e.ProjectItem is FileProjectItem) {
				AddFileProjectItemToProjectBrowser(e);
			}
		}
		
		void AddFileProjectItemToProjectBrowser(ProjectItemEventArgs e)
		{
			var visitor = new UpdateProjectBrowserFileNodesVisitor(e);
			foreach (AbstractProjectBrowserTreeNode node in projectBrowser.TreeView.Nodes) {
				node.AcceptVisitor(visitor, null);
			}
		}
		
		public void Dispose()
		{
			ProjectService.ProjectItemAdded -= ProjectItemAdded;
		}
	}
}
