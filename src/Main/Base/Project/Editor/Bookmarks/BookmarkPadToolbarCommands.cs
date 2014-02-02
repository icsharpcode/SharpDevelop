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
				SD.FileService.JumpToFilePosition(pad.SelectedItem.FileName, pad.SelectedItem.LineNumber, pad.SelectedItem.ColumnNumber);
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
				SD.FileService.JumpToFilePosition(pad.SelectedItem.FileName, pad.SelectedItem.LineNumber, pad.SelectedItem.ColumnNumber);
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
			BookmarkPadBase pad = (BookmarkPadBase)Owner;
			bool anyEnabled = pad.Items.OfType<IHaveStateEnabled>().Any(bp => bp.IsEnabled);
			foreach (var bp in pad.Items.OfType<IHaveStateEnabled>()) {
				bp.IsEnabled = !anyEnabled;
			}
		}
	}
}
