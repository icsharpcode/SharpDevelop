// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Widgets.AutoHide
{
	/// <summary>
	/// AutoHideStatusStripContainer can be used instead of StatusStrip to get a status strip
	/// which is automaticaly hiden and shown. It is especially useful in fullscreen.
	/// </summary>
	public class AutoHideStatusStripContainer: AutoHideContainer
	{
		public AutoHideStatusStripContainer(StatusStrip statusStrip): base(statusStrip)
		{
			statusStrip.AutoSize = false;
			statusStrip.MouseMove += StatusStripMouseMove;
			statusStrip.ItemAdded += delegate(object sender, ToolStripItemEventArgs e) {
				e.Item.MouseMove += StatusStripMouseMove;
			};
			foreach(ToolStripItem i in statusStrip.Items) {
				i.MouseMove += StatusStripMouseMove;
			}
		}
		
		void StatusStripMouseMove(object sender, MouseEventArgs e)
		{
			if (e.Y < control.Height / 2) {
				ShowOverlay = false;
			}
		}
	}
}
