/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 14.10.2007
 * Zeit: 15:39
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using ICSharpCode.Core.WinForms;
using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.Core;
using CommandID = System.ComponentModel.Design.CommandID;

namespace ICSharpCode.Reports.Addin
{
	public class MenuCommandService : System.ComponentModel.Design.MenuCommandService
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
			ISelectionService sp = (ISelectionService)base.GetService(typeof(ISelectionService));
			
			if (sp != null) {
				if (menuID == MenuCommands.TraySelectionMenu) {
					contextMenuPath += "TraySelectionMenu";
				}
				else if (sp.PrimarySelection is RootReportModel) {
					System.Console.WriteLine("found Root");
					contextMenuPath += "ContainerMenu";
				}
				else if (sp.PrimarySelection is BaseSection) {
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

