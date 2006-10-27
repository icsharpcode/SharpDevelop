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
	[ProgId(ObjectTreePanel.PROGID)]
	[GuidAttribute(ObjectTreePanel.GUIDSTR)]
	[ComVisible(true)]    
	public class ObjectTreePanel : Panel, IWin32Window
	{
		public const String PROGID = Constants.COMPINSP_PROGID + ".ObjectTreePanel";
		public const String WINDOW_NAME = "nogoop Objects";
		public const String GUIDSTR = "A82FC021-CE9C-47cd-9757-C637FCDA366E";

		public ObjectTreePanel()
		{
			Control objTree = ObjectBrowser.ObjTreePanel;
			objTree.Dock = DockStyle.Fill;
			Controls.Add(objTree);
		}
	}
}
