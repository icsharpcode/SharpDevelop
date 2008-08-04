// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core.WinForms;
using ICSharpCode.FormsDesigner.Commands;
using CommandID = System.ComponentModel.Design.CommandID;
using MenuCommand = System.ComponentModel.Design.MenuCommand;

namespace ICSharpCode.FormsDesigner.Services
{
	class MenuCommandService : System.ComponentModel.Design.MenuCommandService
	{
		
		Control panel;
		
		public MenuCommandService(Control panel, IServiceProvider serviceProvider) : base(serviceProvider)
		{
			this.panel = panel;
			this.InitializeGlobalCommands( );
		}

		private void InitializeGlobalCommands()
		{
			//Most commands like Delete, Cut, Copy and paste are all added to the MenuCommandService
			// by the other services like the DesignerHost.  Commands like ViewCode and ShowProperties
			// need to be added by the IDE because only the IDE would know how to perform those actions.
			// This allows people to call MenuCommandSerice.GlobalInvoke( StandardCommands.ViewCode );
			// from designers and what not.  .Net Control Designers like the TableLayoutPanelDesigner
			// build up their own context menus instead of letting the MenuCommandService build it.
			// The context menus they build up are in the format that Visual studio expects and invokes
			// the ViewCode and Properties commands by using GlobalInvoke.

			AbstractFormsDesignerCommand viewCodeCommand = new ViewCode();
			AbstractFormsDesignerCommand propertiesCodeCommand = new ShowProperties();
			this.AddCommand( new MenuCommand(viewCodeCommand.CommandCallBack, viewCodeCommand.CommandID));
			this.AddCommand( new MenuCommand(propertiesCodeCommand.CommandCallBack, propertiesCodeCommand.CommandID));
		}

		public override void ShowContextMenu(CommandID menuID, int x, int y)
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
			Point p = panel.PointToClient(new Point(x, y));
			
			
			MenuService.ShowContextMenu(this, contextMenuPath, panel, p.X, p.Y);
		}
	}
}
