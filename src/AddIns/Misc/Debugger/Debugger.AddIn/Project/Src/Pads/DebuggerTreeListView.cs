// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 1254 $</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class DebuggerTreeListView: TreeListView
	{
		static int updateInterval = 100; // ms
		
		Timer refreshingTimer;
		
		public DebuggerTreeListView()
		{
			refreshingTimer = new Timer();
			refreshingTimer.Interval = updateInterval;
			refreshingTimer.Tick += delegate {
				refreshingTimer.Enabled = false;
				this.EndUpdate();
			};
		}
		
		public void DelayRefresh()
		{
			if (!refreshingTimer.Enabled) {
				this.BeginUpdate();
				refreshingTimer.Enabled = true;
			}
		}
	}
}
