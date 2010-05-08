// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>


using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

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

		
		private bool useStandardPrinter;
		private bool landSacpe;
		private GraphicsUnit graphicsUnit;
		
		private Padding padding;
		private string connectionString;
		private string commandText;
		private string noDataMessage = "No Data for this Report";
		private System.Data.CommandType commandType =  System.Data.CommandType.Text;
		

		private Font defaultFont;
		
		private GlobalEnums.ReportType reportType;
		private GlobalEnums.PushPullModel dataModel;
		
		private ParameterCollection parameterCollection;
		private AvailableFieldsCollection availableFields;
		private ColumnCollection groupingsCollection;
		private SortColumnCollection sortingCollection;
		private int bottomMargin;
		private int topMargin;
		private int leftMargin;
		private int rightMargin;
		private Size pageSize;
		
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
			this.pageSize = pageSize;
		}
		
		
		void BaseValues()
		{
			this.useStandardPrinter = true;
			this.graphicsUnit = GraphicsUnit.Pixel;
			this.padding = new Padding(5);
			this.defaultFont = GlobalValues.DefaultFont;
			this.reportType = GlobalEnums.ReportType.FormSheet;
			this.dataModel = GlobalEnums.PushPullModel.FormSheet;
			this.pageSize = GlobalValues.DefaultPageSize;
			this.topMargin = GlobalValues.DefaultPageMargin.Left;
			this.bottomMargin = GlobalValues.DefaultPageMargin.Bottom;
			this.leftMargin = GlobalValues.DefaultPageMargin.Left;
			this.rightMargin = GlobalValues.DefaultPageMargin.Right;
			this.availableFields = new AvailableFieldsCollection();
			this.groupingsCollection = new ColumnCollection();
			this.sortingCollection = new SortColumnCollection();
			this.parameterCollection = new ParameterCollection();
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
		public GlobalEnums.ReportType ReportType {
			get {
				return reportType;
			}
			set {
				if (reportType != value) {
					reportType = value;
				}
			}
		}
		
		#endregion
		
		#region Page Settings
		
		[Category("Page Settings")]
		public int BottomMargin {
			get { return bottomMargin; }
			set { bottomMargin = value; }
		}
		
		
		[Category("Page Settings")]
		public int TopMargin {
			get { return topMargin; }
			set { topMargin = value; }
		}
		
		
		
		[Category("Page Settings")]
		public int LeftMargin {
			get { return leftMargin; }
			set { leftMargin = value; }
		}
		
		
		
		[Category("Page Settings")]
		public int RightMargin {
			get { return rightMargin; }
			set { rightMargin = value; }
		}
		
		
		
		[Category("Page Settings")]
		public Size PageSize {
			get { return pageSize; }
			set { pageSize = value; }
		}
		
		
		[Category("Page Settings")]
		public bool Landscape {
			get { return this.landSacpe; }
			set { this.landSacpe = value;}
		}
		
		
		#endregion
		
		#region DesignerSettings
		
		[Category("Designer Settings")]
		[DefaultValueAttribute (System.Drawing.GraphicsUnit.Millimeter)]
		public System.Drawing.GraphicsUnit GraphicsUnit
		{
			get {
				return graphicsUnit;
			}
			set {
				if (graphicsUnit != value) {
					graphicsUnit = value;
				}
			}
		}
		
		
		[Category("Designer Settings")]
		public Padding Padding
		{
			get {
				return padding;
			}
			set {
				if (this.padding != value) {
					this.padding = value;
				}
				
			}
		}
		
		#endregion
		
		#region Sorting,grouping
		
		[Browsable(false)]
		public  AvailableFieldsCollection AvailableFieldsCollection{
			get { return this.availableFields;}}
		
		/// <summary>
		/// Get/Set a Collection of <see cref="SortColumn">SortColumn</see>
		/// </summary>
		
		public SortColumnCollection SortColumnCollection {
			get {return sortingCollection;}
		}
		
		
		[Browsable(false)]
		public ColumnCollection GroupColumnsCollection {
			get {
				return groupingsCollection;
			}
		}
		
		#endregion
		
		#region ReportParameters
		
		[Category("Parameters")]
		[EditorAttribute ( typeof(ExtendedCollectionEditor),
		                  typeof(System.Drawing.Design.UITypeEditor) )]
		public ParameterCollection ParameterCollection
		{
			get{return parameterCollection;}
		}
		
		#endregion
		
		#region DataRelated
		
		[Category("Data")]
		[DefaultValueAttribute ("")]
		[EditorAttribute (typeof(DefaultTextEditor),
		                  typeof(System.Drawing.Design.UITypeEditor) )]
		public string ConnectionString {
			get {
				if (this.connectionString == null) {
					this.connectionString = String.Empty;
				}
				return connectionString;
			}
			set {
				if (connectionString != value) {
					connectionString = value;
				}
			}
		}
		
		
		[Category("Data")]
		[DefaultValueAttribute ("")]
		[EditorAttribute ( typeof(DefaultTextEditor),
		                  typeof(System.Drawing.Design.UITypeEditor) )]
		public string CommandText {
			get {
				if (this.commandText == null) {
					this.commandText = String.Empty;
				}
				return commandText;
			}
			set {
				if (commandText != value) {
					commandText = value;
				}
			}
		}
		
		
		[Category("Data")]
		public System.Data.CommandType CommandType {
			get {
				return commandType;
			}
			set {
				if (commandType != value) {
					commandType = value;
				}
			}
		}
		
		
		[Category("Data")]
		public GlobalEnums.PushPullModel DataModel {
			get {
				return dataModel;
			}
			set {
				if (dataModel != value) {
					dataModel = value;
				}
			}
		}
		
		[Category("Data")]
		[EditorAttribute ( typeof(DefaultTextEditor),
		                  typeof(System.Drawing.Design.UITypeEditor) )]
		public string NoDataMessage {
			get {
				return noDataMessage;
			}
			set {
				noDataMessage = value;
			}
		}
		
		
		#endregion
		
		#region OutPut Settings
		
		[Category("Output Settings")]
		public Font DefaultFont {
			get {
				return defaultFont;
			}
			set {
				if (defaultFont != value) {
					defaultFont = value;
				}
			}
		}
		
		
		[Category("Output Settings")]
		[DefaultValueAttribute (true)]
		public bool UseStandardPrinter
		{
			get {
				return useStandardPrinter;
			}
			set {
				if (useStandardPrinter != value) {
					useStandardPrinter = value;
				}
			}
		}
		
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
				if (this.defaultFont != null)
				{
					this.defaultFont.Dispose();
					this.defaultFont = null;
				}
			}
		}
		#endregion
		
	}
}

