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
using System.Windows.Forms;
using System.Xml.Serialization;

using ICSharpCode.Reports.Core.Dialogs;
using ICSharpCode.Reports.Core.Globals;

namespace ICSharpCode.Reports.Core{
	/// <summary>
	/// This class stores all the basic settings of an Report
	/// </summary>
	/// 
	
	/// <summary>
	/// Description of ReportSettingsDesigner.
	/// </summary>
	public class ReportSettingsDesigner:ComponentDesigner
	{
		static string settingsName = "ReportSettings";
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
	public class ReportSettings :Component,IDisposable
	{
		private const string defaultReportName = "Report1";

		
		private string reportName;
		private string fileName;
		
		private SqlParameterCollection sqlParameters;
		private ParameterCollection parameterCollection;
		private AvailableFieldsCollection availableFields;
		private GroupColumnCollection groupingsCollection;
		private SortColumnCollection sortingCollection;

		
		#region Constructor
		
		public ReportSettings():this(GlobalValues.DefaultPageSize,"","")
		{
		}
		
		
		public ReportSettings(Size pageSize,
		                      string reportName,
		                      string fileName):base()
		{
			if (pageSize == null) {
				throw new ArgumentNullException("pageSize");
			}
			
			if (String.IsNullOrEmpty(reportName)) {
				this.reportName = GlobalValues.DefaultReportName;
			} else {
				this.reportName = reportName;
			}
			
			if (String.IsNullOrEmpty(fileName)) {
				this.fileName = GlobalValues.PlainFileName;
			} else {
				this.fileName = BuildFilename(fileName);
			}
			BaseValues();
			this.PageSize = pageSize;
		}
		
		
		void BaseValues()
		{
			this.UseStandardPrinter = true;
			this.GraphicsUnit = GraphicsUnit.Pixel;
			this.Padding = new Padding(5);
			this.DefaultFont = GlobalValues.DefaultFont;
			this.ReportType = GlobalEnums.ReportType.FormSheet;
			
			this.DataModel = GlobalEnums.PushPullModel.FormSheet;
			
			this.CommandType =  System.Data.CommandType.Text;
			this.ConnectionString = String.Empty;
			this.CommandText = String.Empty;
			
			this.TopMargin = GlobalValues.DefaultPageMargin.Left;
			this.BottomMargin = GlobalValues.DefaultPageMargin.Bottom;
			this.LeftMargin = GlobalValues.DefaultPageMargin.Left;
			this.RightMargin = GlobalValues.DefaultPageMargin.Right;
			
			this.availableFields = new AvailableFieldsCollection();
			this.groupingsCollection = new GroupColumnCollection();
			this.sortingCollection = new SortColumnCollection();
			this.sqlParameters = new SqlParameterCollection();
			this.parameterCollection = new ParameterCollection();
			this.NoDataMessage = "No Data for this Report";
		}
		
		#endregion
		
		private static string BuildFilename (string file)
		{
			if (file.EndsWith (GlobalValues.ReportExtension,StringComparison.OrdinalIgnoreCase)) {
				return file;
			} else {
				if (file.IndexOf('.') > 0) {
					string [] s1 = file.Split('.');
					return s1[0] + GlobalValues.ReportExtension;
				} else {
					return file + GlobalValues.ReportExtension;
				}
			}
		}
		
		
		#region BaseSettings
		
		[Category("Base Settings")]
		[DefaultValueAttribute ("")]
		public string ReportName
		{
			get {
				if (reportName.Length == 0) {
					reportName = defaultReportName;
				}
				return reportName;
			}
			set {
				if (reportName != value) {
					reportName = value;
				}
			}
		}
		
		
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
			
		
		#endregion
		
		#region Page Settings
		
		[Category("Page Settings")]
		public int BottomMargin {get;set;}
			
		
		[Category("Page Settings")]
		public int TopMargin  {get;set;}
		
		
		
		[Category("Page Settings")]
		public int LeftMargin {get;set;}
		
		
		
		[Category("Page Settings")]
		public int RightMargin  {get;set;}
			
		
		
		[Category("Page Settings")]
		public Size PageSize {get;set;}
			
		
		[Category("Page Settings")]
		public bool Landscape {get;set;}
		
		
		#endregion
		
		#region DesignerSettings
		
		[Category("Designer Settings")]
		[DefaultValueAttribute (System.Drawing.GraphicsUnit.Millimeter)]
		public System.Drawing.GraphicsUnit GraphicsUnit{get;set;}
		
		
		[Category("Designer Settings")]
		public Padding Padding{get;set;}
		
		#endregion
		
		#region Sorting,grouping
		
		[Browsable(false)]
		public  AvailableFieldsCollection AvailableFieldsCollection{
			get { return this.availableFields;}}
		
		/// <summary>
		/// Get/Set a Collection of <see cref="SortColumn">SortColumn</see>
		/// </summary>
		
		[Category("Sorting/Grouping")]
		[EditorAttribute ( typeof(SortingCollectionEditor),
		                  typeof(System.Drawing.Design.UITypeEditor) )]
		public SortColumnCollection SortColumnsCollection {
			get {return sortingCollection;}
		}
		
		
		[Category("Sorting/Grouping")]
		[EditorAttribute ( typeof(GroupingCollectionEditor),
		                  typeof(System.Drawing.Design.UITypeEditor) )]
		public GroupColumnCollection GroupColumnsCollection {
			get {
				return groupingsCollection;
			}
		}
		
		#endregion
		
		#region ReportParameters
		
		[Category("Parameters")]
		[EditorAttribute ( typeof(ParameterCollectionEditor),
		                  typeof(System.Drawing.Design.UITypeEditor) )]
		public ParameterCollection ParameterCollection
		{
			get{return parameterCollection;}
		}
		
		[Category("Parameters")]
		[EditorAttribute ( typeof(ParameterCollectionEditor),
		                  typeof(System.Drawing.Design.UITypeEditor) )]
		
		public SqlParameterCollection SqlParameters
		{
			get { return sqlParameters; }
		}
		
		#endregion
		
		#region DataRelated
		
		[Category("Data")]
		[DefaultValueAttribute ("")]
		[EditorAttribute (typeof(DefaultTextEditor),
		                  typeof(System.Drawing.Design.UITypeEditor) )]
		public string ConnectionString {get;set;}
			
		
		
		[Category("Data")]
		[DefaultValueAttribute ("")]
		[EditorAttribute ( typeof(DefaultTextEditor),
		                  typeof(System.Drawing.Design.UITypeEditor) )]
		public string CommandText {get;set;}
		
		
		
		[Category("Data")]
		public System.Data.CommandType CommandType {get;set;}
		
		
		[Category("Data")]
		public GlobalEnums.PushPullModel DataModel {get;set;}
		
		
		[Category("Data")]
		[EditorAttribute ( typeof(DefaultTextEditor),
		                  typeof(System.Drawing.Design.UITypeEditor) )]
		public string NoDataMessage {get;set;}
		
		
		
		#endregion
		
		#region OutPut Settings
		
		[Category("Output Settings")]
		public Font DefaultFont {get;set;}
			
		
		
		[Category("Output Settings")]
		[DefaultValueAttribute (true)]
		public bool UseStandardPrinter{get;set;}
		
		
		#endregion
		
		[XmlIgnoreAttribute]
		public override ISite Site {
			get { return base.Site; }
			set { base.Site = value; }
		}
		
		#region IDisposable

		protected override void Dispose(bool disposing)

		{
			if (disposing)
			{
				// free managed resources
				if (this.DefaultFont != null)
				{
					this.DefaultFont.Dispose();
					this.DefaultFont = null;
				}
			}
		}
		#endregion
		
	}
}
