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
using System.Drawing;
using System.IO;

using ICSharpCode.Reporting.Globals;

namespace ICSharpCode.Reporting.Items
{
	/// <summary>
	/// Description of ReportSettings.
	/// </summary>
	public class ReportSettings
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
					reportName = Globals.GlobalValues.DefaultReportName;
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
