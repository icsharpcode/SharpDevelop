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
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;

using System.Xml.Serialization;
using ICSharpCode.Reporting.Globals;



namespace ICSharpCode.Reporting.Items
{
	/// <summary>
	/// Description of ReportSettings.
	/// </summary>
	public class ReportSettingsDesigner:ComponentDesigner
	{
		const string settingsName = "ReportSettings";
		public ReportSettingsDesigner()
		{
		}
		
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			component.Site.Name = ReportSettingsDesigner.settingsName;
		}
	}
	
	
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
			ReportType = GlobalEnums.ReportType.FormSheet;
//			
			this.DataModel = GlobalEnums.PushPullModel.FormSheet;
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
//			this.sqlParameters = new SqlParameterCollection();
			ParameterCollection = new ParameterCollection();
//			this.NoDataMessage = "No Data for this Report";
		}
		
		#region BaseSettings
		
		private string reportName;
		
		[Category("Base Settings")]
		[DefaultValueAttribute ("")]
		public string ReportName
		{
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
		
		[Category("Base Settings")]
		[XmlIgnoreAttribute]
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
		
		
		[Browsable(true), Category("Base Settings")]
		public GlobalEnums.ReportType ReportType {get;set;}
		
		
		[Browsable(true), Category("Base Settings")]
		public GlobalEnums.PushPullModel DataModel {get;set;}
		
		#endregion
		
		
		#region Pagesettings
		
		[Category("Page Settings")]
		public int BottomMargin {get;set;}
			
		
		[Category("Page Settings")]
		public int TopMargin  {get;set;}
		
		
		
		[Category("Page Settings")]
		public int LeftMargin {get;set;}
		
		
		
		[Category("Page Settings")]
		public int RightMargin  {get;set;}
			
		
		private Size pageSize;
		
		[Category("Page Settings")]
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
		
		
		[Category("Page Settings")]
		public bool Landscape {get;set;}
		
		
		#endregion
		
		#region
		
		
//		[Category("Data")]
		
		
		
		
		
		
//		[Category("Parameters")]
//		[EditorAttribute ( typeof(ParameterCollectionEditor),
//		                  typeof(System.Drawing.Design.UITypeEditor) )]
		
		public ParameterCollection ParameterCollection {get; private set;}
	
		public SortColumnCollection SortColumnsCollection {get;private set;}
		
		public GroupColumnCollection GroupColumnsCollection {get;private set;}
		
		#endregion
	}
}
