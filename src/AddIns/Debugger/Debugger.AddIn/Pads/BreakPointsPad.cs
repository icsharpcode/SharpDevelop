// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;

using Debugger;
using Debugger.AddIn.Pads.Controls;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;

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
		}
		
		protected override bool ShowBookmarkInThisPad(SDBookmark mark)
		{
			return mark.IsVisibleInBookmarkPad && mark is BreakpointBookmark;
		}
		
		protected override void OnItemActivated(SDBookmark bookmark)
		{
			if (bookmark is DecompiledBreakpointBookmark) {
//				// get information from breakpoint and navigate to the decompiled type
//				string assemblyFile, typeName;
//				if (DecompiledBreakpointBookmark.GetAssemblyAndType(bookmark.FileName, out assemblyFile, out typeName)) {
//					NavigationService.NavigateTo(assemblyFile, typeName, string.Empty, bookmark.LineNumber, false);
//				}
				throw new NotImplementedException();
			} else {
				base.OnItemActivated(bookmark);
			}
		}
	}
}
