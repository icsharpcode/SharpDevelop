// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Debugging
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
