// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;

namespace ICSharpCode.SharpDevelop.Editor.Bookmarks
{
	public sealed class NextBookmarkPadCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			BookmarkPadBase pad = (BookmarkPadBase)this.Owner;
			if (pad.ListView.Items.Count > 0) {
				pad.ListView.SelectedIndex = (pad.ListView.SelectedIndex + 1) % pad.ListView.Items.Count;
				FileService.JumpToFilePosition(pad.SelectedItem.FileName, pad.SelectedItem.LineNumber, pad.SelectedItem.ColumnNumber);
			}
		}
	}
	
	public sealed class PrevBookmarkPadCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			BookmarkPadBase pad = (BookmarkPadBase)this.Owner;
			if (pad.ListView.Items.Count > 0) {
				pad.ListView.SelectedIndex = (pad.ListView.SelectedIndex - 1 + pad.ListView.Items.Count) % pad.ListView.Items.Count;
				FileService.JumpToFilePosition(pad.SelectedItem.FileName, pad.SelectedItem.LineNumber, pad.SelectedItem.ColumnNumber);
			}
		}
	}
	
	public class DeleteAllMarks : AbstractMenuCommand
	{
		public override void Run()
		{
			BookmarkPadBase pad = (BookmarkPadBase)this.Owner;
			foreach(SDBookmark bm in pad.Items.OfType<SDBookmark>().ToList()) {
				SD.BookmarkManager.RemoveMark(bm);
			}
		}
	}
	
	public class DeleteMark : AbstractMenuCommand
	{
		public override void Run()
		{
			BookmarkPadBase pad = (BookmarkPadBase)this.Owner;
			if (pad.SelectedItem != null) {
				SD.BookmarkManager.RemoveMark(pad.SelectedItem);
			}
		}
	}
	
	public class EnableDisableAll : AbstractMenuCommand
	{
		public override void Run()
		{
			BookmarkPadBase pad = (BookmarkPadBase)this.Owner;
			bool anyEnabled = pad.Items.OfType<BreakpointBookmark>().Any(bp => bp.IsEnabled);
			foreach (var bp in pad.Items.OfType<BreakpointBookmark>()) {
				bp.IsEnabled = !anyEnabled;
			}
		}
	}
}
