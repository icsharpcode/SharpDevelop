/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 06.01.2006
 * Time: 23:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

using SharpReportCore;
using SharpReport.Designer;

namespace SharpReport
{
	/// <summary>
	/// Description of BuildSideTab.
	/// </summary>
	public class BuildSideTab{
		SharpDevelopSideBar	sideBar;
		AxSideTab sideTab = null;	
		AxSideTab sideTabFunctions;
		
		public BuildSideTab(SharpDevelopSideBar sideBar){
			this.sideBar = sideBar;
		}
		
		public void CreateSidetabs() {
			
			sideBar.DoAddTab = true;
			
			sideTab = sideBar.SideTabFactory.CreateSideTab (sideBar,
			                                                ResourceService.GetString("SharpReport.Toolbar.Controls"));
			CusomizeSideTab (sideTab);
			sideTab.Items.Add(this.BuildPointer());
			AddReportElements(this.sideTab);
			
			sideBar.ActiveTab = sideTab;
			sideBar.Tabs.Add (sideTab);
			sideTabFunctions = sideBar.SideTabFactory.CreateSideTab (sideBar,
			                                                         ResourceService.GetString("SharpReport.Toolbar.Functions"));
			CusomizeSideTab (this.sideTabFunctions);
			this.sideTabFunctions.Items.Add(this.BuildPointer());
			
			this.AddFunctionElements(this.sideTabFunctions);
			sideBar.Tabs.Add (this.sideTabFunctions);
		}
		
		private AxSideTabItem BuildPointer () {
			return sideTab.SideTabItemFactory.CreateSideTabItem(ResourceService.GetString("SharpReport.Toolbar.Pointer"),
			                                                               "pointer",
			                                                               ResourceService.GetBitmap("Icons.16x16.FormsDesigner.PointerIcon"));
		}
		
		private  void CusomizeSideTab (AxSideTab tab) {
			tab.CanDragDrop = true;
			tab.CanSaved = false;
		}
		
		private void AddReportElements (AxSideTab tab) {

			AxSideTabItem	t = sideTab.SideTabItemFactory.CreateSideTabItem(ResourceService.GetString("SharpReport.Toolbar.TextBox"),
			                                                               GlobalEnums.ReportItemType.ReportTextItem.ToString(),
			                                                               GlobalValues.TextBoxBitmap());
		
			tab.Items.Add (t);
			t = sideTab.SideTabItemFactory.CreateSideTabItem( ResourceService.GetString("SharpReport.Toolbar.DataField"),
			                                                 GlobalEnums.ReportItemType.ReportDataItem.ToString(),
			                                                 ResourceService.GetBitmap("Icons.16x16.SharpQuery.Column"));
			tab.Items.Add (t);
			
			Icon icon = ResourceService.GetIcon("Icons.16x16.ResourceEditor.bmp");
			t = sideTab.SideTabItemFactory.CreateSideTabItem( "Image",
			                                                 GlobalEnums.ReportItemType.ReportImageItem.ToString(),
			                                                 icon.ToBitmap());
			tab.Items.Add (t);
			
			t = sideTab.SideTabItemFactory.CreateSideTabItem( ResourceService.GetString("SharpReport.Toolbar.Rectangle"),
			                                                 GlobalEnums.ReportItemType.ReportRectangleItem.ToString(),
			                                                 GlobalValues.RectangleBitmap());
			tab.Items.Add (t);
			
			t = sideTab.SideTabItemFactory.CreateSideTabItem( ResourceService.GetString("SharpReport.Toolbar.Line"),
			                                                 GlobalEnums.ReportItemType.ReportLineItem.ToString(),
			                                                 GlobalValues.LineBitmap());
			tab.Items.Add (t);
			t = sideTab.SideTabItemFactory.CreateSideTabItem(ResourceService.GetString("SharpReport.Toolbar.Circle"),
			                                                 GlobalEnums.ReportItemType.ReportCircleItem.ToString(),
			                                                 GlobalValues.CircleBitmap());
			tab.Items.Add (t);
		}
		
		private void AddFunctionElements (AxSideTab tab) {
			
			FunctionFactory ff = new FunctionFactory();
			string localise = "SharpReport.Toolbar.Functions.";
			
			AxSideTabItem	t = sideTab.SideTabItemFactory.CreateSideTabItem(ResourceService.GetString(localise + ff.AvailableTypes[0]) ,
			                                                               ff.AvailableTypes[0].ToString(),
			                                                               GlobalValues.FunctionBitmap());
			
			tab.Items.Add (t);
			

			t = sideTab.SideTabItemFactory.CreateSideTabItem (ResourceService.GetString(localise + ff.AvailableTypes[1]),
			                                                  ff.AvailableTypes[1],
			                                                  GlobalValues.FunctionBitmap());
			
			tab.Items.Add (t);
			
		}
	}
}
