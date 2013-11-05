// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.TreeView;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestsPad : AbstractPadContent
	{
		ITestService testService;
		TestTreeView treeView;
		DockPanel panel;
		ToolBar toolBar;
		List<Tuple<IUnresolvedFile, IUnresolvedFile>> pending = new List<Tuple<IUnresolvedFile, IUnresolvedFile>>();

		public UnitTestsPad()
			: this(SD.GetRequiredService<ITestService>())
		{
		}
		
		public UnitTestsPad(ITestService testService)
		{
			this.testService = testService;
			
			panel = new DockPanel();
			treeView = new TestTreeView(); // treeView must be created first because it's used by CreateToolBar

			toolBar = CreateToolBar("/SharpDevelop/Pads/UnitTestsPad/Toolbar");
			panel.Children.Add(toolBar);
			DockPanel.SetDock(toolBar, Dock.Top);
			
			panel.Children.Add(treeView);
			
			treeView.ContextMenu = CreateContextMenu("/SharpDevelop/Pads/UnitTestsPad/ContextMenu");
			
			testService.OpenSolutionChanged += testService_OpenSolutionChanged;
			testService_OpenSolutionChanged(null, null);
		}
		
		public override void Dispose()
		{
			testService.OpenSolutionChanged -= testService_OpenSolutionChanged;
			base.Dispose();
		}
		
		public override object Control {
			get { return panel; }
		}
		
		public ITestTreeView TreeView {
			get { return treeView; }
		}
		
		void testService_OpenSolutionChanged(object sender, EventArgs e)
		{
			treeView.TestSolution = testService.OpenSolution;
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
