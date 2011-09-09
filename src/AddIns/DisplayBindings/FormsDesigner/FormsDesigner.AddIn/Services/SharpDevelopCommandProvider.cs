// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Security.Principal;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Integration;
using ICSharpCode.Core.WinForms;
using ICSharpCode.FormsDesigner.Commands;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.FormsDesigner.Services
{
	public class SharpDevelopCommandProvider : MarshalByRefObject, ICommandProvider
	{
		FormsDesignerViewContent vc;
		
		public SharpDevelopCommandProvider(FormsDesignerViewContent vc)
		{
			this.vc = vc;
		}
		
		public void ShowContextMenu(CommandIDEnum menuID, int x, int y)
		{
			string contextMenuPath = "/SharpDevelop/FormsDesigner/ContextMenus/";
			var menu = CommandIDEnumConverter.ToCommandID(menuID);
			
			if (menu == MenuCommands.ComponentTrayMenu) {
				contextMenuPath += "ComponentTrayMenu";
			} else if (menu == MenuCommands.ContainerMenu) {
				contextMenuPath += "ContainerMenu";
			} else if (menu == MenuCommands.SelectionMenu) {
				contextMenuPath += "SelectionMenu";
			} else if (menu == MenuCommands.TraySelectionMenu) {
				contextMenuPath += "TraySelectionMenu";
			} else {
				throw new Exception();
			}
			
			Control panel = ((CustomWindowsFormsHost)((System.Windows.Controls.ContentPresenter)vc.Control).Content).Child;
			if (panel != null) {
				Point p = panel.PointToClient(new Point(x, y));
				
				MenuService.ShowContextMenu(vc.AppDomainHost, contextMenuPath, panel, p.X, p.Y);
			}
		}
	}
}
