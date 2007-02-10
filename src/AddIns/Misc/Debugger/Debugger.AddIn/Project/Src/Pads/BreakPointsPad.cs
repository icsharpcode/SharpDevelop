// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using Debugger;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Debugging;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class BreakPointsPad : BookmarkPadBase
	{
		WindowsDebugger debugger;
		NDebugger debuggerCore;
		
		public BreakPointsPad()
		{
			InitializeComponents();
		}
		
		void InitializeComponents()
		{
			debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;

			if (debugger.ServiceInitialized) {
				InitializeDebugger();
			} else {
				debugger.Initialize += delegate {
					InitializeDebugger();
				};
			}
		}

		public void InitializeDebugger()
		{
			debuggerCore = debugger.DebuggerCore;
		}
		
		protected override bool ShowBookmarkInThisPad(SDBookmark mark)
		{
			return mark.IsVisibleInBookmarkPad && mark is Debugging.BreakpointBookmark;
		}
	}
}
