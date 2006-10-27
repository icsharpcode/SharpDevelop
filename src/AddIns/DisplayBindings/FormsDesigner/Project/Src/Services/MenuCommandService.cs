// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core;

namespace ICSharpCode.FormsDesigner.Services
{
	class MenuCommandService : System.ComponentModel.Design.MenuCommandService
	{
		
		Control panel;
		
		public MenuCommandService(Control panel, IServiceProvider serviceProvider) : base(serviceProvider)
		{
			this.panel = panel;
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
