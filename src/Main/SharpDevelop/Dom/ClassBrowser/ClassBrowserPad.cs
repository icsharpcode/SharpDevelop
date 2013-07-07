// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows.Controls;
using ICSharpCode.Core.Presentation;
using ICSharpCode.TreeView;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	class ClassBrowserPad : AbstractPadContent
	{
		IProjectService projectService;
		ClassBrowserTreeView treeView;
		ClassBrowserSettings settings;
		DockPanel panel;
		ToolBar toolBar;

		public ClassBrowserPad()
			: this(SD.GetRequiredService<IProjectService>())
		{
		}
		
		public ClassBrowserPad(IProjectService projectService)
		{
			this.projectService = projectService;
			this.settings = new ClassBrowserSettings();
			
			panel = new DockPanel();
			treeView = new ClassBrowserTreeView(); // treeView must be created first because it's used by CreateToolBar

			toolBar = CreateToolBar("/SharpDevelop/Pads/ClassBrowser/Toolbar");
			panel.Children.Add(toolBar);
			DockPanel.SetDock(toolBar, Dock.Top);
			
			panel.Children.Add(treeView);
			
			//treeView.ContextMenu = CreateContextMenu("/SharpDevelop/Pads/UnitTestsPad/ContextMenu");
			
			projectService.CurrentSolutionChanged += ProjectServiceCurrentSolutionChanged;
			ProjectServiceCurrentSolutionChanged(null, null);
		}
		
		public override void Dispose()
		{
			projectService.CurrentSolutionChanged -= ProjectServiceCurrentSolutionChanged;
			base.Dispose();
		}
		
		public override object Control {
			get { return panel; }
		}
		
		public IClassBrowserTreeView TreeView {
			get { return treeView; }
		}
		
		void ProjectServiceCurrentSolutionChanged(object sender, EventArgs e)
		{
			if (projectService.CurrentSolution == null) {
				treeView.Workspace = settings.LoadDefaultWorkspace();
			} else {
				treeView.Workspace = settings.LoadForSolution(projectService.CurrentSolution);
			}
		}
		
		/// <summary>
		/// Virtual method so we can override this method and return
		/// a dummy ToolBar when testing.
		/// </summary>
		protected virtual ToolBar CreateToolBar(string name)
		{
			Debug.Assert(treeView != null);
			return ToolBarService.CreateToolBar(treeView, treeView, name);
		}
		
		/// <summary>
		/// Virtual method so we can override this method and return
		/// a dummy ContextMenu when testing.
		/// </summary>
		protected virtual ContextMenu CreateContextMenu(string name)
		{
			Debug.Assert(treeView != null);
			return MenuService.CreateContextMenu(treeView, name);
		}
	}
}
