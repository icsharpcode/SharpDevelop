// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

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
	public class SharpReportManager :SharpReportEngine,IDisposable {
		
		private BaseDesignerControl baseDesignerControl;
		private ReportModel reportModel;
		
		//TODO Move this to reportSettings
		private ColumnCollection availableFields = null;
		
		public SharpReportManager():base(){
		}
		
		#region privates
		
		private ConnectionObject BuildConnectionObject (ReportSettings settings) {
			if (settings.ReportType == GlobalEnums.ReportTypeEnum.DataReport) {
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
			ColumnCollection columnCollecion = new ColumnCollection();
			switch (baseDesignerControl.ReportModel.DataModel) {
				case GlobalEnums.PushPullModelEnum.FormSheet:
					//Plain FormSheet we do nothing for the moment
					break;
				case GlobalEnums.PushPullModelEnum.PushData:
					//PushData
					columnCollecion = SharpReportEngine.CollectFieldsFromModel(this.baseDesignerControl.ReportModel);
					break;
				case GlobalEnums.PushPullModelEnum.PullData:
					// PullData, query the Datasource and ask for the available Fields
					if (base.ConnectionObject == null) {
						base.ConnectionObject = this.BuildConnectionObject(baseDesignerControl.ReportModel.ReportSettings);
					}

					if (this.baseDesignerControl.ReportModel.DataModel.Equals(GlobalEnums.PushPullModelEnum.PullData)){
						try {
							using (DataManager dataManager = new DataManager(base.ConnectionObject,
							                                                 baseDesignerControl.ReportModel.ReportSettings)) {
								
								dataManager.DataBind();
								columnCollecion = dataManager.AvailableFields;
							}
							
						} catch (Exception ) {
							throw;
						} finally {
//							System.Console.WriteLine("ReportManager:ReadColumnCollection in finally");
						}
					}
					break;
				default:
					break;
			}
			return columnCollecion;
		}
		
	
		
		#endregion
		
		
		
		#region Standarts for all reports (Headlines etc)
		
		/// <summary>
		/// Insert a <see cref="ReportTextItem"></see> in the PageHeader with
		/// the <see cref="ReportModel.ReportSettings.ReportName"></see> as
		/// text
		/// </summary>
		/// <param name="model">ReportModel</param>
		public void CreatePageHeader (ReportModel model) {
			if (model == null) {
				throw new ArgumentNullException("model");
			}
			
			BaseSection section = model.PageHeader;
			section.SuspendLayout();
			SharpReport.Designer.IDesignableFactory gf = new SharpReport.Designer.IDesignableFactory();
			BaseTextItem item = (BaseTextItem)gf.Create ("ReportTextItem");
			item.SuspendLayout();
			item.Text = model.ReportSettings.ReportName;
			item.Font = CopyFont(model.ReportSettings.DefaultFont);
			item.Location = new Point (0,0);
			item.Size = new Size (item.Size.Width,item.Font.Height + SharpReportCore.GlobalValues.EnlargeControl);
			item.Parent = section;
			section.Items.Add (item);
			item.ResumeLayout();
			section.ResumeLayout();
		}
		
		///<summary>
		/// Insert Function 'PageNumber' in Section PageFooter
		/// </summary>
		/// <param name="model">ReportModel</param>
		
		public void CreatePageNumber (ReportModel model) {
			if (model == null) {
				throw new ArgumentNullException("model");
			}
			
			BaseSection section = model.PageFooter;
			section.SuspendLayout();
			FunctionFactory gf = new FunctionFactory();
			PageNumber pageNumber = (PageNumber)gf.Create ("PageNumber");
			pageNumber.SuspendLayout();
			
			pageNumber.Text = ResourceService.GetString("SharpReport.Toolbar.Functions.PageNumber");
			pageNumber.Location = new Point (0,0);
			pageNumber.Parent = section;
			section.Items.Add(pageNumber);
			pageNumber.ResumeLayout();
			section.ResumeLayout();
		}	
		#endregion
		
		private Font CopyFont (Font orgF) {
			Font f = new Font(orgF.Name,orgF.Size,orgF.Style,orgF.Unit);
			return f;
		}
		
		#region Preview
		
		public  AbstractRenderer GetRendererForStandartReports (ReportModel model) {
			if (model == null) {
				throw new ArgumentNullException("model");
			}
			return this.BuildStandartRenderer (model);
		}
		
		/// <summary>
		/// Run Preview with Designer
		/// </summary>
		/// <param name="model"><see cref="">ReportModel</see></param>
		/// <param name="showInUserControl"></param>
		public void ReportPreview (ReportModel model,bool standAlone) {
			if (model == null) {
				throw new ArgumentNullException("model");
			}
			
			try {
				AbstractRenderer abstr = this.BuildStandartRenderer (model);
				if (abstr != null) {
					PreviewControl.ShowPreview (abstr,1.5,standAlone);
				}
				
			} catch (Exception e) {
				MessageService.ShowError (e,"SharpReportManager:ReportPreview");
			}
		}
		
		
		
		private AbstractRenderer BuildStandartRenderer (ReportModel model) {
			if (model == null) {
				throw new ArgumentNullException("model");
			}
			if (base.ConnectionObject == null) {
				base.ConnectionObject = this.BuildConnectionObject(model.ReportSettings);
			}
			
			return  base.AbstractRenderer(model);
		}
		
		
		public AbstractRenderer GetRendererForPushDataReports (ReportModel model,
		                                                       DataSet dataSet) {
			if (model == null) {
				throw new ArgumentNullException("model");
			}
			if (dataSet == null) {
				throw new ArgumentNullException("dataSet");
			}
			return base.SetupPushDataRenderer(model,dataSet.Tables[0]);
		}
		
		
		public void ReportPreviewPushData (ReportModel model,
		                                   DataSet dataSet,
		                                   bool standAlone) {
			if (model == null) {
				throw new ArgumentNullException("model");
			}
			if (dataSet == null) {
				throw new ArgumentNullException("dataSet");
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
				this.baseDesignerControl.Accept ((SharpReportCore.IModelVisitor)saveVisitor);
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
		
		public SqlParametersCollection SqlParametersCollection{
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
		
		#region IDisposable
		
		public new void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~SharpReportManager(){
			Dispose(false);
		}
		
		protected new void Dispose(bool disposing){
			try {
				if (disposing) {
					// Free other state (managed objects).
					if (this.baseDesignerControl != null) {
						this.baseDesignerControl.Dispose();
					}
					if (this.reportModel != null) {
						this.reportModel.Dispose();
					}
				}
			} finally {
				// Release unmanaged resources.
				// Set large fields to null.
				// Call Dispose on your base class.
				base.Dispose();
			}
		}
		#endregion
	}
	
}
