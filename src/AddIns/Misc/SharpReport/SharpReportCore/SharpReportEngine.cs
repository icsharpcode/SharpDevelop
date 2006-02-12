//
// SharpDevelop ReportEditor
//
// Copyright (C) 2005 Peter Forstmeier
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Peter Forstmeier (Peter.Forstmeier@t-online.de)

using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing.Printing;
using System.Windows.Forms;

using SharpReportCore;

/// <summary>
/// This Class contains the basic Functions to handle reports
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 31.08.2005 16:21:38
/// </remarks>

namespace SharpReportCore {
	public class SharpReportEngine : object,IDisposable {
		
		private PreviewControl previewControl;
		
		private ConnectionObject connectionObject;
		
		public event EventHandler <SharpReportEventArgs> NoData;
		public event EventHandler <SharpReportParametersEventArgs> ParametersRequest;
		
		public SharpReportEngine() {
			if (SharpReportCore.GlobalValues.IsValidPrinter() == false) {
				InvalidPrinterException ex = new InvalidPrinterException(new PrinterSettings());
				throw ex;
			}
		}
		
		#region ParameterHandling

		private bool CheckReportParameters (ReportModel model,ReportParameters reportParameters) {
			if (model.ReportSettings.ReportType != GlobalEnums.enmReportType.FormSheet) {
				if (this.connectionObject == null) {
					
					if (model.ReportSettings.ConnectionString != "") {
						this.connectionObject = new ConnectionObject (model.ReportSettings.ConnectionString);
					}
					
					if (reportParameters != null) {
						this.connectionObject = reportParameters.ConnectionObject;
					}
					
					if (this.connectionObject.Connection != null) {
						return true;
					} else {
						throw new SharpReportException("SharpReportEngine:CheckReportParameters : No valid Connection");
					}
				}
			} else {
				return true;
			}
			return false;
		}
		
		
		void GrapSqlParameters (ReportSettings settings) {
			if (settings.SqlParametersCollection != null && settings.SqlParametersCollection.Count > 0) {
				if (this.ParametersRequest != null) {
					SharpReportParametersEventArgs e = new SharpReportParametersEventArgs();
					e.SqlParametersCollection = settings.SqlParametersCollection;
					e.ReportName = settings.ReportName;
					ParametersRequest (this,e);
				}
			}
			
		}
		
		
		void SetSqlParameters (ReportModel model,AbstractParametersCollection sqlParams) {
			if ((sqlParams == null)||(sqlParams.Count == 0)) {
				return;
			}
			model.ReportSettings.SqlParametersCollection.Clear();
			model.ReportSettings.SqlParametersCollection.AddRange(sqlParams);
		}
		
		
		
		private static void SetCustomSorting (ReportModel model,ColumnCollection sortParams) {
			if ((sortParams == null)||(sortParams.Count == 0)) {
				return;
			}
			model.ReportSettings.SortColumnCollection.Clear();
			model.ReportSettings.SortColumnCollection.AddRange(sortParams);
		}
		
		
		
		private void ApplyReportParameters (ReportModel model,ReportParameters parameters){
			if (model == null) {
				throw new MissingModelException();
			}
			if (parameters == null ){
				throw new ArgumentNullException("parameters");
			}
			
			SetSqlParameters (model,parameters.SqlParameters);
			SetCustomSorting (model,parameters.SortColumnCollection);
		}
		#endregion
		
		#region Setup for print/preview
		private static  bool CheckForPushModel (ReportModel model) {
			if (model.ReportSettings.DataModel == GlobalEnums.enmPushPullModel.PushData) {
				return true;
			} else {
				return false;
			}
		}

		private DataManager SetupDataContainer (ReportSettings settings) {
			
			if (settings.ReportType == GlobalEnums.enmReportType.DataReport) {
				if (settings.CommandText != null) {
					try {
						GrapSqlParameters (settings);
						
						if (this.connectionObject != null) {
							DataManager container = new DataManager(this.connectionObject,
							                                        settings);
							
							if (container.DataBind() == true) {
								return container;
							} else {
								return null;
							}
						}else {
							throw new NullReferenceException("SetupContainer:connectionObject is missing");
						}
						
					}
					catch (System.Data.OleDb.OleDbException) {
						throw ;
					}
					catch (Exception) {
						throw;
					}
				}
			}
			return null;
		}
		
		
		protected ColumnCollection CollectFieldsFromModel(ReportModel model){
			if (model == null) {
				throw new ArgumentNullException("model");
			}
			ColumnCollection col = new ColumnCollection();
			
			foreach (BaseSection section in model.SectionCollection){
				for (int i = 0;i < section.Items.Count ;i ++ ) {
					IItemRenderer item = section.Items[i];
					BaseDataItem baseItem = item as BaseDataItem;
					if (baseItem != null) {
						col.Add(new AbstractColumn(baseItem.ColumnName));
					}
				}
			}
			return col;
		}
		
		
		protected SharpReportCore.AbstractRenderer SetupStandartRenderer (ReportModel model) {
			AbstractRenderer abstr = null;
			switch (model.ReportSettings.ReportType) {
					//FormSheets reports
				case GlobalEnums.enmReportType.FormSheet:
					abstr = new RendererFactory().Create (model,null);
					break;
					//Databased reports
				case GlobalEnums.enmReportType.DataReport :
					DataManager dataManager = SetupDataContainer (model.ReportSettings);
					if (dataManager != null) {
						if (dataManager.DataSource != null) {
							abstr = new RendererFactory().Create (model,dataManager);
						}
						
					} else {
						if (NoData != null) {
							SharpReportEventArgs e = new SharpReportEventArgs();
							e.PageNumber = 0;
							e.Cancel = false;
							NoData (this,e);
							if (e.Cancel == true) {
								// If we cancel, we have to create an instance of any kind of renderer
								//to set the cancel flag to true
								abstr = new RendererFactory().Create (model,null);
								abstr.Cancel = true;
							} else {
								// Print the report only as Formsheet -> only Text and Graphic Items
								abstr = new RendererFactory().Create (model,null);
							}
						}
					}
					
					break;
				default:
					throw new SharpReportException ("SharpReportmanager:SetupRenderer -> Unknown Reporttype");
			}
			
			return abstr;
		}
		
		
		protected SharpReportCore.AbstractRenderer SetupPushDataRenderer (ReportModel model,
		                                                                  DataTable dataTable) {
			
			if (model.ReportSettings.ReportType != GlobalEnums.enmReportType.DataReport) {
				throw new ArgumentException("SetupPushDataRenderer <No valid ReportModel>");
			}
			if (model.ReportSettings.DataModel != GlobalEnums.enmPushPullModel.PushData) {
				throw new ArgumentException("SetupPushDataRenderer <No valid ReportType>");
			}
			
			AbstractRenderer abstr = null;
			DataManager dataManager = new DataManager (dataTable,model.ReportSettings);
			
			if (dataManager != null) {
				dataManager.DataBind();
				if (dataManager.DataSource != null) {
					abstr = new RendererFactory().Create (model,dataManager);
				}
				
				return abstr;
			}
			return null;
		}
		
		#endregion
		
		/// <summary>
		/// Creates an <see cref="AbstractRenderer"></see>
		/// any Class deriving from this can be
		/// used to get a <see cref="System.Drawing.Printing.PrintDocument"></see>
		/// </summary>
		/// <param name="model"><see cref="ReportModel"></see></param>
		/// <returns></returns>
		
		protected AbstractRenderer AbstractRenderer (ReportModel model) {
			if (model == null) {
				throw new MissingModelException();
			}
			AbstractRenderer abstr = SetupStandartRenderer(model);
			return abstr;
		}
		
		
		#region Parameter Handling
		///<summary>Loads the report, and initialise <see cref="ReportParameters"></see>
		/// Fill the <see cref="AbstractParametersCollection"></see> with <see cref="SqlParameters">
		/// this is an easy way to ask the report for desired paramaters</see></summary>
		
		public ReportParameters LoadParameters (string fileName) {
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			ReportModel model = null;
			try {
				model = ModelFromFile (fileName);
				ReportParameters pars = new ReportParameters();
				pars.ConnectionObject = null;
				pars.SqlParameters.AddRange (model.ReportSettings.SqlParametersCollection);
				return pars;
			} catch (Exception) {
				throw;
			}
		}
		
		#endregion
		
		
		#region Preview
		///<summary>
		/// Opens the PreviewDialog as standalone, so, no need for Designer
		/// </summary>
		///<param name="fileName">Report's Filenema</param>
		/// <param name="/// <summary>
		/// send report to printer
		/// </summary>
		/// <param name="fileName">Path to ReportFile</param>
		
		public void PreviewStandartReport (string fileName) {
			if (fileName.Length == 0) {
				throw new ArgumentNullException("fileName");
			}
			PreviewStandartReport (fileName,null);
		}
		
		
		public void PreviewStandartReport (string fileName,ReportParameters reportParameters) {
			if (fileName.Length == 0) {
				throw new ArgumentNullException("fileName");
			}

			ReportModel model = null;
			AbstractRenderer renderer = null;
			try {
				model = ModelFromFile (fileName);
				if (CheckReportParameters (model,reportParameters)) {
					renderer = SetupStandartRenderer (model);
					if (renderer != null) {
						PreviewControl.ShowPreview(renderer,1.5,false);
					}
				}
			} catch (Exception) {
				throw;
			}
			
		}
		/// <summary>
		/// Preview a "PushModel - Report"
		/// </summary>
		/// <param name="fileName">Filename to the location of the ReportFile</param>
		/// <param name="dataTable">a Datatable, containing the data</param>
		public void PreviewPushDataReport (string fileName,DataTable dataTable) {
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			if (dataTable == null) {
				throw new ArgumentNullException("dataTable");
			}
			ReportModel model = null;
			AbstractRenderer renderer = null;
			try {
				model = ModelFromFile (fileName);
				
				if (!CheckForPushModel(model)) {
					throw new SharpReportException ("PrintPushdataReport: No valid ReportModel");
				}
				
				renderer = this.SetupPushDataRenderer (model,dataTable);
				if (renderer != null) {
					PreviewControl.ShowPreview(renderer,1.5,false);
				}
			} catch (Exception) {
				
			}
		}
		
		#endregion
		
		#region Printing
		
		private void ReportToPrinter (AbstractRenderer renderer,ReportModel model) {
			if (renderer == null) {
				throw new NullReferenceException("SparpReportEngine:ReportToPrinter: No valid Renderer");
			}
			PrintDocument  doc = null;
			if (renderer.Cancel == false) {
				doc = renderer.ReportDocument;
				using (PrintDialog dlg = new PrintDialog()) {
					dlg.Document = doc;
//					MessageBox.Show (model.ReportSettings.UseStandartPrinter.ToString());
					if (model.ReportSettings.UseStandartPrinter == true) {
						dlg.Document.Print();
					} else {
						DialogResult result = dlg.ShowDialog();
						// If the result is OK then print the document.
						if (result==DialogResult.OK){
							dlg.Document.Print();
						}
					}
				}
			}
		}
		
		/// <summary>
		/// send a Standart (PullModel/FormSheet) Report to printer
		/// </summary>
		/// <param name="fileName">Path to ReportFile</param>
		/// <param name="renderTo">Type of renderer currently only "ToText" is implemented</param>

		public void PrintStandartReport (string fileName) {
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			PrintStandartReport (fileName,null);
			
		}
		
		public void PrintStandartReport (string fileName,ReportParameters reportParameters) {
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			
			ReportModel model = null;
			AbstractRenderer renderer = null;
			try {
				model = ModelFromFile (fileName);
				if (CheckReportParameters (model,reportParameters)) {
					renderer = SetupStandartRenderer (model);
					this.ReportToPrinter (renderer,model);
				}
			} catch (Exception) {
				throw;
			}
		}
		/// <summary>
		/// Print a PullModel Report
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="dataTable"></param>
		/// <param name="showPrintDialog">if set to true, show the <see cref="PrintDialog"></see>
		/// </param>
		
		public void PrintPushDataReport (string fileName,
		                                 DataTable dataTable,
		                                 bool showPrintDialog) {
			
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			if (dataTable == null) {
				throw new ArgumentNullException("dataTable");
			}
			ReportModel model = null;
			AbstractRenderer renderer = null;
			try {
				model = ModelFromFile (fileName);
				if (!CheckForPushModel(model)) {
					throw new SharpReportException ("PrintPushdataReport: No valid ReportModel");
				}
				
				renderer = SetupPushDataRenderer (model,dataTable);
				this.ReportToPrinter(renderer,model);
				
			} catch (Exception) {
			}
			
			
		}
		/// <summary>
		/// Print a PullModel Report, if <see cref="UseStandartPrinter"></see> in
		/// <see cref="ReportSettings"></see> is true, the Report is send directly to the printer,
		///  otherwise, we show a <see cref="PrintDialog"></see>
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="dataTable"></param>
		public void PrintPushDataReport (string fileName,DataTable dataTable) {
			if (fileName.Length == 0) {
				throw new ArgumentNullException("fileName");
			}
			if (dataTable == null) {
				throw new ArgumentNullException("dataTable");
			}
			ReportModel model = null;
			AbstractRenderer renderer = null;
			
			try {
				model = ModelFromFile (fileName);
				if (!CheckForPushModel(model)) {
					throw new SharpReportException ("PrintPushdataReport: No valid ReportModel");
				}
				
				renderer = this.SetupPushDataRenderer (model,dataTable);
				this.ReportToPrinter(renderer,model);
				
			} catch (Exception) {
			}
		}
		
		
		#endregion
		
		
		
		
		#region Loading report
		/// <summary>
		/// Load's a <see cref="ReportModel"></see> from File using the
		/// <see cref="LoadModelVisitor"></see>
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns><see cref="ReportModel"></see></returns>
		
		protected ReportModel ModelFromFile (string fileName) {
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			
			try {
				ReportModel model = new ReportModel();
				SharpReportCore.LoadModelVisitor modelVisitor = new SharpReportCore.LoadModelVisitor(model,fileName);
				model.Accept (modelVisitor);
				return model;
			} catch (Exception) {
				throw;
			}
		}

		
		#endregion
		
		#region Properties
		///<summary>
		/// Use this Dialog for previewing a Report
		/// This Control is also used by the Designer
		/// </summary>
		
		public SharpReportCore.PreviewControl  PreviewControl  {
			get {
				if (this.previewControl == null) {
					previewControl = new PreviewControl();
				}
				return this.previewControl;
			}
		}
		
		public ConnectionObject ConnectionObject {
			get {
				return connectionObject;
			}
			set {
				connectionObject = value;
			}
		}
		
		#endregion
		
		#region IDisposable
		
		public void Dispose(){
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		~SharpReportEngine(){
			Dispose(false);
		}
		
		protected virtual void Dispose(bool disposing){
			if (disposing) {
				// Free other state (managed objects).
				if (this.connectionObject != null) {
					this.connectionObject.Dispose();
				}
				if (this.previewControl != null) {
					this.previewControl.Dispose();
				}
			}
			
			// Release unmanaged resources.
			// Set large fields to null.
			// Call Dispose on your base class.
		}
		#endregion
	}
}
