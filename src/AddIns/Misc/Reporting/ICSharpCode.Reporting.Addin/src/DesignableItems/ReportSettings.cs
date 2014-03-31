/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 15.03.2014
 * Time: 18:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Addin.Designer;

namespace ICSharpCode.Reporting.Addin.DesignableItems
{
	
	[Designer(typeof(ReportSettingsDesigner))]
	public class ReportSettings:Component
	{
		
		public ReportSettings()
		{
			this.pageSize = GlobalValues.DefaultPageSize;
			BaseValues();
		}
		
		
		void BaseValues()
		{
			
//			this.UseStandardPrinter = true;
//			this.GraphicsUnit = GraphicsUnit.Pixel;
//			this.Padding = new Padding(5);
//			this.DefaultFont = GlobalValues.DefaultFont;
			this.ReportType = GlobalEnums.ReportType.FormSheet;
//			
			this.DataModel = GlobalEnums.PushPullModel.FormSheet;
//			
//			this.CommandType =  System.Data.CommandType.Text;
//			this.ConnectionString = String.Empty;
//			this.CommandText = String.Empty;
//			
//			this.TopMargin = GlobalValues.DefaultPageMargin.Left;
//			this.BottomMargin = GlobalValues.DefaultPageMargin.Bottom;
//			this.LeftMargin = GlobalValues.DefaultPageMargin.Left;
//			this.RightMargin = GlobalValues.DefaultPageMargin.Right;
//			
//			this.availableFields = new AvailableFieldsCollection();
//			this.groupingsCollection = new GroupColumnCollection();
			this.SortColumnsCollection = new SortColumnCollection();
			this.GroupColumnsCollection = new GroupColumnCollection();
//			this.sqlParameters = new SqlParameterCollection();
			ParameterCollection = new ParameterCollection();
//			this.NoDataMessage = "No Data for this Report";
		}
		
		
		private string reportName;
		
//		[Category("Base Settings")]
//		[DefaultValueAttribute ("")]
		public string ReportName
		{
			get {
				if (string.IsNullOrEmpty(reportName)) {
					reportName = GlobalValues.DefaultReportName;
				}
				return reportName;
			}
			set {
				if (reportName != value) {
					reportName = value;
				}
			}
		}
		
		private string fileName;
//		[Category("Base Settings")]
//		[XmlIgnoreAttribute]
		public string FileName
		{
			get {
				if (String.IsNullOrEmpty(fileName)) {
					fileName = GlobalValues.PlainFileName;
				}
				return Path.GetFullPath(fileName);
			}
			set {
				fileName = value;
			}
		}
		
		
//		[Category("Page Settings")]
		public int BottomMargin {get;set;}
			
		
//		[Category("Page Settings")]
		public int TopMargin  {get;set;}
		
		
		
//		[Category("Page Settings")]
		public int LeftMargin {get;set;}
		
		
		
//		[Category("Page Settings")]
		public int RightMargin  {get;set;}
			
		private Size pageSize;
		
		public Size PageSize {
			get {
				if (!Landscape) {
					return pageSize;
				} else {
					return new Size(pageSize.Height,pageSize.Width);
				}
				 }
			set { pageSize = value; }
		}
		
//		[Category("Page Settings")]
//		public Size PageSize {get;set;}
		
//		[Category("Page Settings")]
		public bool Landscape {get;set;}
		
//		[Category("Data")]
		public GlobalEnums.PushPullModel DataModel {get;set;}
		
		
//		[Browsable(true), Category("Base Settings")]
		public GlobalEnums.ReportType ReportType {get;set;}
		
		
//		[Category("Parameters")]
//		[EditorAttribute ( typeof(ParameterCollectionEditor),
//		                  typeof(System.Drawing.Design.UITypeEditor) )]
		
		public ParameterCollection ParameterCollection {get; private set;}
	
		public SortColumnCollection SortColumnsCollection {get;private set;}
		
		public GroupColumnCollection GroupColumnsCollection {get;private set;}
	}
}
