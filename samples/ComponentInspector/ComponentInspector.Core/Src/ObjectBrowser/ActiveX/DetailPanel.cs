// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using NoGoop.Util;

namespace NoGoop.ObjBrowser.ActiveX
{

	[ProgId(DetailPanel.PROGID)]
	[GuidAttribute(DetailPanel.GUIDSTR)]
	[ComVisible(true)]    
		public class DetailPanel : Panel, IWin32Window
	{

		public const String                 PROGID = 
			Constants.COMPINSP_PROGID + ".DetailPanel";

		public const String                 WINDOW_NAME = 
			"nogoop Detail Panel";

		public const String                 GUIDSTR = 
			"8BC44703-D2A8-48f9-BD91-9D37877ED74C";

		public DetailPanel() : base()
		{
			Control panel = ObjBrowser.Panels.DetailPanel.Panel;
			panel.Dock = DockStyle.Fill;
			Controls.Add(panel);
		}

	}
}
