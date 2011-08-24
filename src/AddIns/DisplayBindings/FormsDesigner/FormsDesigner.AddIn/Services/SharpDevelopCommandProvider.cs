// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Integration;

using ICSharpCode.Core.WinForms;
using ICSharpCode.FormsDesigner.Commands;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FormsDesigner.Services
{
	public class SharpDevelopCommandProvider : MarshalByRefObject, ICommandProvider
	{
		FormsDesignerViewContent vc;
		
		public SharpDevelopCommandProvider(FormsDesignerViewContent vc)
		{
			this.vc = vc;
		}
		
		public void ShowContextMenu(CommandID menuID, int x, int y)
		{
			string contextMenuPath = "/SharpDevelop/FormsDesigner/ContextMenus/";
			
			if (menuID == MenuCommands.ComponentTrayMenu) {
				contextMenuPath += "ComponentTrayMenu";
			} else if (menuID == MenuCommands.ContainerMenu) {
				contextMenuPath += "ContainerMenu";
			} else if (menuID == MenuCommands.SelectionMenu) {
				contextMenuPath += "SelectionMenu";
			} else if (menuID == MenuCommands.TraySelectionMenu) {
				contextMenuPath += "TraySelectionMenu";
			} else {
				throw new Exception();
			}
			
//			Control panel = ((WindowsFormsHost)vc.UserContent).Child;
//			if (panel != null) {
//				Point p = panel.PointToClient(new Point(x, y));
//				
//				MenuService.ShowContextMenu(this, contextMenuPath, panel, p.X, p.Y);
//			}
		}
	}
}
