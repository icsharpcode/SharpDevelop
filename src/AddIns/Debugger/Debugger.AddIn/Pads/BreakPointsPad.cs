// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Windows.Controls;

using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class BreakPointsPad : BookmarkPadBase
	{
		public BreakPointsPad()
		{
			var res = new CommonResources();
			res.InitializeComponent();
			
			Grid grid = (Grid)this.Control;
			ToolBar toolbar = ToolBarService.CreateToolBar(grid, this, "/SharpDevelop/Pads/BreakpointPad/Toolbar");
			grid.Children.Add(toolbar);
			
			this.control.listView.View = (GridView)res["breakpointsGridView"];
			this.control.listView.SetValue(GridViewColumnAutoSize.AutoWidthProperty, "35;50%;50%");
		}
		
		protected override bool ShowBookmarkInThisPad(SDBookmark mark)
		{
			return mark.IsVisibleInBookmarkPad && mark is BreakpointBookmark;
		}
	}
}
