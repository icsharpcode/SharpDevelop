// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core.WinForms;
using ICSharpCode.FormsDesigner.Commands;

namespace ICSharpCode.FormsDesigner.Services
{
	public class SharpDevelopCommandProvider : ICommandProvider
	{
		FormsDesignerViewContent vc;
		
		public SharpDevelopCommandProvider(FormsDesignerViewContent vc)
		{
			this.vc = vc;
		}
		
		public void InitializeGlobalCommands(IMenuCommandService service)
		{
			// Most commands like Delete, Cut, Copy and paste are all added to the MenuCommandService
			// by the other services like the DesignerHost.  Commands like ViewCode and ShowProperties
			// need to be added by the IDE because only the IDE would know how to perform those actions.
			// This allows people to call MenuCommandSerice.GlobalInvoke( StandardCommands.ViewCode );
			// from designers and what not.  .Net Control Designers like the TableLayoutPanelDesigner
			// build up their own context menus instead of letting the MenuCommandService build it.
			// The context menus they build up are in the format that Visual studio expects and invokes
			// the ViewCode and Properties commands by using GlobalInvoke.

			AbstractFormsDesignerCommand viewCodeCommand = new ViewCode();
			AbstractFormsDesignerCommand propertiesCodeCommand = new ShowProperties();
			service.AddCommand(new System.ComponentModel.Design.MenuCommand(viewCodeCommand.CommandCallBack, viewCodeCommand.CommandID));
			service.AddCommand(new System.ComponentModel.Design.MenuCommand(propertiesCodeCommand.CommandCallBack, propertiesCodeCommand.CommandID));
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
			
			Control panel = vc.UserContent;
			if (panel != null) {
				Point p = panel.PointToClient(new Point(x, y));
				
				MenuService.ShowContextMenu(this, contextMenuPath, panel, p.X, p.Y);
			}
		}
	}
}
