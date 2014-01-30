// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
