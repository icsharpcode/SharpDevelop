// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
//     <version>$Revision$</version>
// </file>

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
