// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core.WinForms;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.Addin.Designer;
using CommandID = System.ComponentModel.Design.CommandID;

namespace ICSharpCode.Reporting.Addin.Services
{
	class MenuCommandService : System.ComponentModel.Design.MenuCommandService
	{
		
		Control panel;
		
		public MenuCommandService(Control panel, IServiceProvider serviceProvider) : base(serviceProvider)
		{
			this.panel = panel;
//			this.InitializeGlobalCommands( );
		}
		
		/*
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

//			AbstractFormsDesignerCommand viewCodeCommand = new ViewCode();
//			AbstractFormsDesignerCommand propertiesCodeCommand = new ShowProperties();
//			this.AddCommand( new MenuCommand(viewCodeCommand.CommandCallBack, viewCodeCommand.CommandID));
//			this.AddCommand( new MenuCommand(propertiesCodeCommand.CommandCallBack, propertiesCodeCommand.CommandID));
		}
	*/
		public override void ShowContextMenu(CommandID menuID, int x, int y)
		{
			string contextMenuPath = "/SharpDevelop/ReportDesigner/ContextMenus/";
			var selectionService = (ISelectionService)base.GetService(typeof(ISelectionService));
			
			if (selectionService != null) {
				if (menuID == MenuCommands.TraySelectionMenu) {
					contextMenuPath += "TraySelectionMenu";
				}
				else if (selectionService.PrimarySelection is RootReportModel) {
					System.Console.WriteLine("found Root");
					contextMenuPath += "ContainerMenu";
				}
				else if (selectionService.PrimarySelection is BaseSection) {
					System.Console.WriteLine("found baseSection");
					contextMenuPath += "ContainerMenu";
				}
				else {
					contextMenuPath += "SelectionMenu";
				}
				
				Point p = panel.PointToClient(new Point(x, y));
				MenuService.ShowContextMenu(this, contextMenuPath, panel, p.X, p.Y);
			}
		}
	}
}
