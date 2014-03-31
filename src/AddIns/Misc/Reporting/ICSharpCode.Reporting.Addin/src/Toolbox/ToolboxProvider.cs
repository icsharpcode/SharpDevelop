/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 23.03.2014
 * Time: 12:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Drawing.Design;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Widgets.SideBar;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.Reporting.Addin.DesignableItems;

namespace ICSharpCode.Reporting.Addin.Toolbox
{
	/// <summary>
	/// Description of ToolboxProvider.
	/// </summary>
	static class ToolboxProvider
	{
		 static SideTab standardSideTab;
		 static int viewCount;
		 static bool initialised;
		
		public static void AddViewContent(IViewContent viewContent)
		{
			if (viewContent == null)
				throw new ArgumentNullException("viewContent");
			
			if (!initialised)
				Initialise();
			
			// Make sure the standard workflow sidebar exists
			if (standardSideTab == null) {
				LoggingService.Debug("Creating Reporting Sidetab");
				standardSideTab = CreateReportingSidetab();
			}
			ViewCount++;
		}
		
		static void Initialise()
		{
			initialised = true;
		}
		
		
		static SideTab CreateReportingSidetab ()
		{
			var sideTab = new SideTab("ReportDesigner");
			sideTab.CanSaved = false;
			sideTab.Items.Add(CreateToolboxPointer(sideTab));
			
			// TextItem
			var toolboxItem = new ToolboxItem(typeof(BaseTextItem)) {
				DisplayName = ResourceService.GetString("SharpReport.Toolbar.TextBox"),
				Bitmap = IconService.GetBitmap("Icons.16.16.SharpReport.Textbox")
			};
				sideTab.Items.Add(new SideTabItemDesigner(toolboxItem));
			
			//Grahics
			// Line
			toolboxItem = new ToolboxItem(typeof(BaseLineItem)) {
				DisplayName = ResourceService.GetString("SharpReport.Toolbar.Line"),
				Bitmap = IconService.GetBitmap("Icons.16.16.SharpReport.Line")
			};
		
			sideTab.Items.Add(new SideTabItemDesigner(toolboxItem));	
		
	/*
			//GroupHeader
			tb = new ToolboxItem(typeof(ICSharpCode.Reports.Addin.GroupHeader));
			tb.Bitmap = WinFormsResourceService.GetBitmap("Icons.16x16.NameSpace");
			tb.DisplayName = ResourceService.GetString("SharpReport.Toolbar.GroupHeader");
			sideTab.Items.Add(new SideTabItemDesigner(tb));
			
			
			//GroupFooter
			tb = new ToolboxItem(typeof(ICSharpCode.Reports.Addin.GroupFooter));
			tb.Bitmap = WinFormsResourceService.GetBitmap("Icons.16x16.NameSpace");
			tb.DisplayName = ResourceService.GetString("SharpReport.Toolbar.GroupFooter");
			sideTab.Items.Add(new SideTabItemDesigner(tb));
			
			// Row
			tb = new ToolboxItem(typeof(ICSharpCode.Reports.Addin.BaseRowItem));
			tb.Bitmap = WinFormsResourceService.GetBitmap("Icons.16x16.SharpQuery.Table");
			tb.DisplayName = ResourceService.GetString("SharpReport.Toolbar.DataRow");
			sideTab.Items.Add(new SideTabItemDesigner(tb));
			
			//BaseTable
//			tb.Bitmap = WinFormsResourceService.GetBitmap("Icons.16x16.SharpQuery.Table");
			tb.Bitmap = WinFormsResourceService.GetBitmap("Icons.16x16.SharpQuery.Table");
			tb = new ToolboxItem(typeof(ICSharpCode.Reports.Addin.BaseTableItem));
			tb.DisplayName = ResourceService.GetString("SharpReport.Toolbar.Table");
			sideTab.Items.Add(new SideTabItemDesigner(tb));	
			
			
			//BaseDataItem
			tb = new ToolboxItem(typeof(ICSharpCode.Reports.Addin.BaseDataItem));
			tb.DisplayName = ResourceService.GetString("SharpReport.Toolbar.DataField");
//				tb.Bitmap = WinFormsResourceService.GetBitmap("Icons.16x16.SharpQuery.Column");
			tb.Bitmap = WinFormsResourceService.GetBitmap("Icons.16x16.SharpQuery.Column");
			sideTab.Items.Add(new SideTabItemDesigner(tb));	
			
			//Grahics
			// Line
			tb = new ToolboxItem(typeof(ICSharpCode.Reports.Addin.BaseLineItem));
			tb.DisplayName = ResourceService.GetString("SharpReport.Toolbar.Line");
			tb.Bitmap = WinFormsResourceService.GetIcon("Icons.16.16.SharpReport.Line").ToBitmap();
			sideTab.Items.Add(new SideTabItemDesigner(tb));
			
			// Rectangle
			tb = new ToolboxItem(typeof(ICSharpCode.Reports.Addin.BaseRectangleItem));
			tb.DisplayName = ResourceService.GetString("SharpReport.Toolbar.Rectangle");
			tb.Bitmap = GlobalValues.RectangleBitmap();
			sideTab.Items.Add(new SideTabItemDesigner(tb));
			
			// Circle
			tb = new ToolboxItem(typeof(ICSharpCode.Reports.Addin.BaseCircleItem));
			tb.DisplayName = ResourceService.GetString("SharpReport.Toolbar.Circle");
			tb.Bitmap = GlobalValues.CircleBitmap();
			sideTab.Items.Add(new SideTabItemDesigner(tb));
			
		
			// Image
			tb = new ToolboxItem(typeof(ICSharpCode.Reports.Addin.BaseImageItem));
			tb.DisplayName = ResourceService.GetString("SharpReport.Toolbar.Image");
			tb.Bitmap = WinFormsResourceService.GetIcon("Icons.16x16.ResourceEditor.bmp").ToBitmap();
			sideTab.Items.Add(new SideTabItemDesigner(tb));
			*/
			return sideTab;
		}
		
		
		static SideTabItem  CreateToolboxPointer(SideTab sideTab)
		{
			var pointer = new SharpDevelopSideTabItem("Pointer") {
				CanBeRenamed = false,
				CanBeDeleted = false,
				Icon = new Bitmap(IconService.GetBitmap("Icons.16x16.FormsDesigner.PointerIcon"), 16, 16),
				Tag = null
			};
			return pointer;
		}
		
		
		static SharpDevelopSideBar reportingSideBar;
		
		public static SharpDevelopSideBar ReportingSideBar {
			get {
				if (reportingSideBar == null) {
					reportingSideBar = new SharpDevelopSideBar();
					reportingSideBar.Tabs.Add(standardSideTab);
					ReportingSideBar.ActiveTab = standardSideTab;
				}
				return reportingSideBar;
			}
		}
		
		
		static int ViewCount {
			get { return viewCount; }
			set {
				viewCount = value;
				
				if (viewCount == 0)	{
					standardSideTab = null;
				}
			}
		}
	}
}
