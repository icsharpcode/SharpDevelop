// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
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
		}
		
		void StatusStripMouseMove(object sender, MouseEventArgs e)
		{
			if (e.Y < control.Height - 3) {
				HideOverlay();
			}
		}
	}
}
