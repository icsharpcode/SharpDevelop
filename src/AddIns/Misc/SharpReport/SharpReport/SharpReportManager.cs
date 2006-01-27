/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 14.11.2004
 * Time: 17:58
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Data;
using System.Data.OleDb;

using System.Collections;

using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

using SharpQuery.Connection;

using SharpReportCore;

using SharpReport.Designer;
using SharpReport.ReportItems;
using SharpReport.ReportItems.Functions;


using System.Diagnostics;

namespace SharpReport{
	/// <summary>
	/// Description of SharpReportManager.
	/// </summary>
	public class SharpReportManager :SharpReportEngine {
		
		private BaseDesignerControl baseDesignerControl;
		private ReportModel reportModel;
		
		//TODO Move this to reportSettings
		private ColumnCollection availableFields = null;
		
		public SharpReportManager():base(){
		}
		
		#region privates
		
		private ConnectionObject BuildConnectionObject (ReportSettings settings) {
			if (settings.ReportType == GlobalEnums.enmReportType.DataReport) {
				try {
					if (settings.ConnectionString.Length > 0) {
						return new ConnectionObject(settings.ConnectionString);
					} else {
						IConnection ole = OLEDBConnectionWrapper.CreateFromDataConnectionLink();
						return new ConnectionObject(ole.ConnectionString);
					}
				} catch (Exception) {
					throw;
				}
			}
			return null;
		}
	
		/// <summary>
		/// Get the <see cref="ColumnCollection"></see> from ReportModel or
		/// use the <see cref="DataManager"></see> to read the Fields from the Query
		/// </summary>
		/// <returns><see cref="ColumnCollection"</returns>
	
		private ColumnCollection ReadColumnCollection() {
		
			DataManager dataManager = null;
			if (baseDesignerControl == null) {
				return new ColumnCollection();
			}
			//FormSheet report = no available Fileds
			if (baseDesignerControl.ReportModel.ReportSettings.ReportType == GlobalEnums.enmReportType.FormSheet) {
				return new ColumnCollection();
			}
	
			if (baseDesignerControl.ReportModel.DataModel.Equals(GlobalEnums.enmPushPullModel.PushData)) {
				return new ColumnCollection();
			}
			// PullData, query the Datasource and ask for the available Fields
			if (base.ConnectionObject == null) {
				base.ConnectionObject = this.BuildConnectionObject(baseDesignerControl.ReportModel.ReportSettings);
			}
			
			if (this.baseDesignerControl.ReportModel.DataModel.Equals(GlobalEnums.enmPushPullModel.PullData)){
				
				using (dataManager = new DataManager(base.ConnectionObject,
				                                     baseDesignerControl.ReportModel.ReportSettings)) {
					dataManager.DataBind();
					return dataManager.AvailableFields;
				}
			}
			
			//Pushdata, we walk thru all sections and collect the ReportDataItems
			if (this.baseDesignerControl.ReportModel.DataModel == GlobalEnums.enmPushPullModel.PushData) {
				return base.CollectFieldsFromModel(this.baseDesignerControl.ReportModel);
			}
			throw new ArgumentOutOfRangeException("SharpReportManager:ReadColumnCollection");
		}
		
		
		
		
		
		private void AddItemsToSection (BaseSection section,ReportItemCollection collection) {
			
			if ((section == null)|| (collection == null) ) {
				throw new ArgumentNullException ("Sharpreportmanager:AddItemsToSection");
			}
			// if there are already items in the section,
			// the we have to append the Items, means whe have to enlarge the section
			if (section.Items.Count > 0) {
				section.Size = new Size (section.Size.Width,
				                         section.Size.Height + GlobalValues.DefaultSectionHeight);
				
				// Adjust the Location
				foreach (IItemRenderer i in collection) {
					i.Location = new Point (i.Location.X,GlobalValues.DefaultSectionHeight);
				}
			}
			
			for (int i = 0;i < collection.Count ;i ++ ) {
				BaseReportItem r = (BaseReportItem)collection[i];
				r.Parent = section.Name;
				section.Items.Add (r);
			}
		}
		
		private Font CopyFont (Font orgF) {
			Font f = new Font(orgF.Name,orgF.Size,orgF.Style,orgF.Unit);
			return f;
		}
		
		#endregion
		
		#region Standarts for all reports (Headlines etc)
		
		/// <summary>
		/// Create TextItem in PageHeader with Reportmodel.ReportSettings.ReportName
		/// </summary>
		/// <param name="model">ReportModel</param>
		public void CreatePageHeader (ReportModel model) {
			BaseSection section = model.PageHeader;
			section.SuspendLayout();
			SharpReport.Designer.IDesignableFactory gf = new SharpReport.Designer.IDesignableFactory();
			BaseTextItem item = (BaseTextItem)gf.Create ("ReportTextItem");
			item.SuspendLayout();
			item.Text = model.ReportSettings.ReportName;
			item.Font = CopyFont(model.ReportSettings.DefaultFont);
			item.Location = new Point (0,0);
			item.Size = new Size (item.Size.Width,item.Font.Height + SharpReportCore.GlobalValues.EnlargeControl);
			section.Items.Add (item);
			item.ResumeLayout();
			section.ResumeLayout();
		}
		
		///<summary>
		/// Insert Function 'PageNumber' in Section PageFooter
		/// </summary>
		/// <param name="model">ReportModel</param>
		
		public void CreatePageNumber (ReportModel model) {
			BaseSection section = model.PageFooter;
			section.SuspendLayout();
			FunctionFactory gf = new FunctionFactory();
			PageNumber pageNumber = (PageNumber)gf.Create ("PageNumber");
			pageNumber.SuspendLayout();
			
			pageNumber.Text = ResourceService.GetString("SharpReport.Toolbar.Functions.PageNumber");
			pageNumber.Location = new Point (0,0);
			section.Items.Add(pageNumber);
			pageNumber.ResumeLayout();
			section.ResumeLayout();
		}
		
		
		public void AdjustNames (ReportModel model) {
			NameService nameService = new NameService();
			foreach (BaseSection section in model.SectionCollection) {
				foreach (IItemRenderer item in section.Items) {
					item.Name = nameService.CreateName(section.Items,item.Name);
				}
			}
		}
			
		#endregion
		
		#region Create report from Query
		/// <summary>
		/// Builds ColumHeaders for report, we take the ColumnNames as Text Property
		/// </summary>
		/// <param name="model">A valid(filled) reportModel</param>
		/// <param name="section">The Section to use for headerLines</param>
		/// <param name="schemaTable">SchemaTable witch contains the Column Informations</param>
		
		public void CreateColumnHeadersFromTable (ReportModel model,BaseSection section,DataTable schemaTable) {
			if (section == null) {
				throw new ArgumentException("SharpReportManager:CreateColumnHeadersFromTable <section>");
			}
			using  (AutoReport auto = new AutoReport()){
				try {
					ReportItemCollection headerCol = auto.AutoHeaderFromTable (model,section,schemaTable,false);
					AddItemsToSection (section,headerCol);
				} catch (Exception) {
					throw;
				}
			}
		}
		
		/// <summary>
		/// Create Columns from SchemaTable
		/// </summary>
		///<param name="model">a valid reportModel</param>
		///<param name="schemaTable">DataTable witch contaisn SchemaDefinitions</param>
		/// 
		
		public void CreateColumnsFromTable (ReportModel model,DataTable schemaTable) {
			if ((model == null)||(schemaTable.Rows.Count == 0) ) {
				throw new ArgumentException ("Invalid Arguments in SharpReportmanager:CreateColumnsFromFile");
			}
			
			using  (AutoReport auto = new AutoReport()){
				try {
					ReportItemCollection colDetail = auto.AutoColumnsFromTable (model,
					                                                            schemaTable);
					BaseSection section = model.DetailSection;
					section.SuspendLayout();
					AddItemsToSection (section,colDetail);
					section.ResumeLayout();
				} catch (Exception) {
					throw;
				}
			}
		}
		#endregion
		
		#region Create Reports from .Xsd Files
		/*
		/// <summary>
		/// create Header from an .Xsd File
		/// </summary>
		/// <param name="model">a valid ReportModel</param>
		/// <param name="section">Section in witch the header should be created</param>
		/// <param name="fileName">File/Path to .Xsd file</param>
		
		public void a_CreateColumnHeadersFromXsd (ReportModel model,BaseSection section,string fileName){
			Debug.Assert (fileName.Length > 0,"CreateColumnsHeadersFromScheman : No valid FileName");
			try {
				DataSet ds = new DataSet();
				ds.ReadXml (fileName);
				using  (AutoReport auto = new AutoReport()){
					try {
//						ReportItemCollection colDetail = auto.AutoHeaderFromSchema (model,section,ds,false);
//						AddItemsToSection (section,colDetail);
					} catch (Exception) {
						throw;
					}
				}
			} catch (Exception) {
				throw;
			}
		}
		*/
		///<summary>
		/// Create the ReportHeader
		/// </summary>
		/// <param name="section">A ReportSection whre to build the Hedarlines</param>
		///<param name="collection">A reportItemcollection containing the basic informations</param>
		public void CreateHeaderColumns (BaseSection section,ReportItemCollection collection) {
			using  (AutoReport auto = new AutoReport()){
				try {
					ReportItemCollection colDetail = auto.AutoHeaderColumns (collection);
					section.SuspendLayout();
					AddItemsToSection (section,colDetail);
					section.ResumeLayout();
				} catch(Exception) {
					throw;
				}
			}
		}
		///<summary>
		/// Create the DataColumns
		/// </summary>
		/// <param name="section">A ReportSection where to build the
		///  <see cref="ReportDataItem"></see>
		/// DataItems</param>
		///<param name="collection">A reportItemcollection containing the basic informations</param>
		public void CreateDataColumns (BaseSection section,ReportItemCollection collection) {
			using  (AutoReport auto = new AutoReport()){
				try {
					ReportItemCollection colDetail = auto.AutoDataColumns (collection);
					section.SuspendLayout();
					AddItemsToSection (section,colDetail);
					section.ResumeLayout();
				}catch (Exception) {
					throw;
				}
			}
		}
		
		#endregion
		
		
		
		#region Preview
		
		
		public  AbstractRenderer GetRendererForStandartReports (ReportModel model) {
			System.Console.WriteLine("Manager.GetRenderer");
			if (model == null) {
				throw new ArgumentException("SharpReportManager:GetRendererForStandartReports 'ReportModel'");
			}
			return this.BuildStandartRenderer (model);
		}
		
		/// <summary>
		/// Run Preview with Designer
		/// </summary>
		/// <param name="model"><see cref="">ReportModel</see></param>
		/// <param name="showInUserControl"></param>
		public void ReportPreview (ReportModel model,bool standAlone) {
			try {
				AbstractRenderer abstr = this.BuildStandartRenderer (model);
				if (abstr != null) {
					PreviewControl.ShowPreview (abstr,1.5,standAlone);
				}
				
			} catch (Exception ) {
				throw;
			}
		}
		
		private AbstractRenderer BuildStandartRenderer (ReportModel model) {
			if (model == null) {
				throw new ArgumentException("SharpReportManager:BuildStandartRenderer 'ReportModel'");
			}
			
			if (base.ConnectionObject == null) {
				base.ConnectionObject = this.BuildConnectionObject(model.ReportSettings);
			}
			return  base.AbstractRenderer(model);
		}
		
		
		public AbstractRenderer GetRendererForPushDataReports (ReportModel model,DataSet dataSet) {
			if (model == null) {
				throw new ArgumentException("SharpReportManager:GetRendererForPushDataReports 'ReportModel'");
			}
			if (dataSet == null) {
				throw new ArgumentException("SharpReportManager:GetRendererForPushDataReports 'DataSet'");
			}
			return base.SetupPushDataRenderer(model,dataSet.Tables[0]);
		}
		
		
		public void ReportPreviewPushData (ReportModel model,
		                                   DataSet dataSet,
		                                   bool standAlone) {
			if (model == null) {
				throw new ArgumentException("SharpReportManager:ReportPreviewPushData 'ReportModel'");
			}
			
			try {
				AbstractRenderer abstr = GetRendererForPushDataReports (model,dataSet);
				if (abstr != null) {
					
					PreviewControl.ShowPreview (abstr,1.5,standAlone);
				}
				
			} catch (Exception) {
				throw;
			}
		}
		
		#endregion
		
		
		#region Load and Save
		/// <summary>
		/// Saves the File to 'FileName"'
		/// </summary>
		/// <param name="fileName">FileName of Report</param>
		
		public void  SaveToFile(string fileName){
			try {
				SharpReport.Visitors.SaveReportVisitor saveVisitor = new SharpReport.Visitors.SaveReportVisitor();
				this.baseDesignerControl.Accept (saveVisitor);
				XmlDocument xmlDoc = saveVisitor.XmlDocument;
				XmlTextWriter xw = new XmlTextWriter (fileName,System.Text.Encoding.UTF8);
				xmlDoc.PreserveWhitespace = true;
				xmlDoc.Save (xw);
				xw.Close();	
				FileService.RecentOpen.AddLastFile(fileName);
			} catch (Exception) {
				throw;
			}
		}
		
		
		
		/// <summary>
		/// Load the Designer with a Report
		/// </summary>
		/// <param name="fileName">Valid Filename with .xml or .sdr Extension</param>
		
		public void LoadFromFile(string fileName){
			SharpReport.Visitors.LoadReportVisitor loadVisitor = new SharpReport.Visitors.LoadReportVisitor(fileName);
			this.baseDesignerControl.Accept(loadVisitor);
			this.baseDesignerControl.ReportModel.ReportSettings.FileName = fileName;
			reportModel = this.baseDesignerControl.ReportModel;
		}
		
		
		
		
		#endregion
		
		#region Collections
		/// <summary>
		/// A Typed Collection of all Available fields return from a query
		/// </summary>
		
		public ColumnCollection AvailableFieldsCollection {
			
			get {
				if (this.baseDesignerControl.ReportModel.ReportSettings.AvailableFieldsCollection.Count == 0) {
					this.availableFields = 	this.ReadColumnCollection();
				} else {
					this.availableFields = this.baseDesignerControl.ReportModel.ReportSettings.AvailableFieldsCollection;
				}
				
				this.baseDesignerControl.ReportModel.ReportSettings.AvailableFieldsCollection = this.availableFields;
				return availableFields;
			}
		}
		
		/// <summary>
		/// A Collection of Columns we like to sort
		/// </summary>
		public ColumnCollection SortColumnCollection  {
			get {
				return this.baseDesignerControl.ReportModel.ReportSettings.SortColumnCollection;
			}
		}
		
		/// <summary>
		/// A Collection of Columns we like to Group
		/// </summary>
		
		public ColumnCollection GroupColumnCollection  {
			get {
				return this.baseDesignerControl.ReportModel.ReportSettings.GroupColumnsCollection;
			}
		}
		
		/// <summary>
		/// Collection of Parameters for the Report
		/// </summary>
		
		public AbstractParametersCollection SqlParametersCollection{
			get {
				return this.baseDesignerControl.ReportModel.ReportSettings.SqlParametersCollection;
			}
		}
		#endregion
		
		#region property's
		
		public SharpReport.Designer.BaseDesignerControl BaseDesignControl {
			get {
				if (this.baseDesignerControl == null) {
					this.baseDesignerControl = new BaseDesignerControl();
				}
				return this.baseDesignerControl;
			}
			set {
				this.baseDesignerControl = value;
			}
		}
		
		#endregion
	}
	
}
