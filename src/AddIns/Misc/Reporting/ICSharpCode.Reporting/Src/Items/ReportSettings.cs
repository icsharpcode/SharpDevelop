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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Interfaces;

namespace ICSharpCode.Reporting.Items
{
	/// <summary>
	/// Description of ReportSettings.
	/// </summary>
	
	public class ReportSettings:Component,IReportSettings
	{
		
		public ReportSettings(){
		
			this.pageSize = GlobalValues.DefaultPageSize;
			BaseValues();
			var x = PdfSharp.PageSizeConverter.ToSize(PdfSharp.PageSize.A4);
			//http://www.sizepaper.com/a-series/a4
			//http://www.sizepaper.com/american-loose
			
			var paperProp = new System.Drawing.Printing.PageSettings();
			var p = paperProp.PaperSize.PaperName.ToString();
		}
		
		
		void BaseValues(){
//			this.UseStandardPrinter = true;
//			this.GraphicsUnit = GraphicsUnit.Pixel;
//			this.Padding = new Padding(5);
//			this.DefaultFont = GlobalValues.DefaultFont;
//			ReportType = ReportType.FormSheet;
//			
			this.DataModel = PushPullModel.FormSheet;
//			
//			this.CommandType =  System.Data.CommandType.Text;
//			this.ConnectionString = String.Empty;
//			this.CommandText = String.Empty;
//			
			TopMargin = GlobalValues.DefaultPageMargin.Left;
			BottomMargin = GlobalValues.DefaultPageMargin.Bottom;
			LeftMargin = GlobalValues.DefaultPageMargin.Left;
			RightMargin = GlobalValues.DefaultPageMargin.Right;
//			
//			this.availableFields = new AvailableFieldsCollection();

			SortColumnsCollection = new SortColumnCollection();
			GroupColumnsCollection = new GroupColumnCollection();
			ParameterCollection = new ParameterCollection();
//			this.sqlParameters = new SqlParameterCollection();
			
//			this.NoDataMessage = "No Data for this Report";
		}
		
		#region BaseSettings
		
		string reportName;
		
//		[Category("Base Settings")]
//		[DefaultValueAttribute ("")]
		public string ReportName{
			get {
				if (string.IsNullOrEmpty(reportName)) {
					reportName = GlobalValues.DefaultReportName;
				}
				return reportName;
			}
			set {
				reportName = value;
			}
		}
		
		string fileName;
		
//		[Category("Base Settings")]
//		[XmlIgnoreAttribute]
		public string FileName{
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
		

		[Browsable(true), Category("Base Settings")]
		public PushPullModel DataModel {get;set;}
		
		#endregion
		
		#region Pagesettings
		
//		[Category("Page Settings")]
		public int BottomMargin {get;set;}
			
		
//		[Category("Page Settings")]
		public int TopMargin  {get;set;}
		
		
		
//		[Category("Page Settings")]
		public int LeftMargin {get;set;}
		
		
		
//		[Category("Page Settings")]
		public int RightMargin  {get;set;}
			
		
		Size pageSize;
		
//		[Category("Page Settings")]
		public Size PageSize {
			get {
				return !Landscape ? pageSize : new Size(pageSize.Height, pageSize.Width);
			}
//			set { pageSize = value; }
		}
		
		
//		[Category("Page Settings")]
		public bool Landscape {get;set;}
		
		
		#endregion
		
		#region
		
		public ParameterCollection ParameterCollection {get; private set;}
	
		public SortColumnCollection SortColumnsCollection {get;private set;}
		
		public GroupColumnCollection GroupColumnsCollection {get;private set;}
		
		#endregion
	}
}
