using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.Core
{
	public class BreakpointBookmarkEventArgs: EventArgs
	{
		BreakpointBookmark breakpointBookmark;

		public BreakpointBookmark BreakpointBookmark {
			get {
				return breakpointBookmark;
			}
		}

		public BreakpointBookmarkEventArgs(BreakpointBookmark breakpointBookmark)
		{
			this.breakpointBookmark = breakpointBookmark;
		}
	}
}
