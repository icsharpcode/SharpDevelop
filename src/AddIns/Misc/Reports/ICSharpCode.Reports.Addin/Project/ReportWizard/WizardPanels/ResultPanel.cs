// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.Reports.Core;
using ICSharpCode.SharpDevelop;


namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// This Class show the Result of the Query build in the previos window,
	/// also we can create *.xsd File's (Schema onla and/or Data + Schema)
	/// </summary>
	public class ResultPanel : AbstractWizardPanel
	{

		const string defaultContextMenu = "/ReportGenerator/ResultPanel/ContextMenuDefault";
		
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtSqlString;
		
		private ReportStructure reportStructure;
		private Properties customizer;
		private ReportModel model;
		private ConnectionObject connectionObject;
		private DataSet resultDataSet;
		
		
		#region Constructor
		
		public ResultPanel()
		{
			InitializeComponent();
			base.EnableFinish = true;
			base.EnableCancel = true;
			base.EnableNext = true;
			base.Refresh();
			this.label1.Text = ResourceService.GetString("SharpReport.Wizard.PullModel.CommandText");
		}
		
		#endregion
		
		
		#region Fill data
		
		private DataSet FillGrid() 
		{
			this.connectionObject = ConnectionObject.CreateInstance(this.model.ReportSettings.ConnectionString,
			                                                      System.Data.Common.DbProviderFactories.GetFactory("System.Data.OleDb"));
			
			this.txtSqlString.Text = String.Empty;
			SqlQueryChecker.Check(model.ReportSettings.CommandType,
			                      model.ReportSettings.CommandText);
			DataSet dataSet = ResultPanel.CreateDataSet ();
			
			
			switch (model.ReportSettings.CommandType) {
				case CommandType.Text:
						this.txtSqlString.Text = model.ReportSettings.CommandText;
						dataSet = BuildFromSqlString();
					break;
				case CommandType.StoredProcedure:
					MessageService.ShowError("Stored Procedures are not suppurted at the moment");
					break;
				case CommandType.TableDirect:
					MessageService.ShowError("TableDirect is not suppurted at the moment");
					break;
				default:
					throw new Exception("Invalid value for CommandType");
			}
			
			
			if (model.ReportSettings.CommandType == CommandType.StoredProcedure){
				/*
				if (reportStructure.SharpQueryProcedure == null) {
					throw new IllegalQueryException();
				}

				SharpQueryProcedure procedure = reportStructure.SharpQueryProcedure;
				SharpQuerySchemaClassCollection sc = procedure.GetSchemaParameters();
				
				if ((sc != null) && sc.Count > 0) {
					dataSet = ExecuteStoredProc (procedure);
				}else {
					dataSet = ExecuteStoredProc ();
				}
				*/
			}
			
			// from here we create from an SqlString like "Select...."
//			if (model.ReportSettings.CommandType == CommandType.Text){
//				this.txtSqlString.Text = model.ReportSettings.CommandText;
//				dataSet = BuildFromSqlString();
//			}
			return dataSet;
		}
		
		
		private static DataSet CreateDataSet() 
		{
			DataSet dataSet = new DataSet();
			dataSet.Locale = CultureInfo.CurrentCulture;
			return dataSet;
		}
		
		
		private DbDataAdapter BuildAdapter () {
				DbDataAdapter adapter = this.connectionObject.ProviderFactory.CreateDataAdapter();
				adapter.SelectCommand = (DbCommand)this.BuildCommand();
				return adapter;
		}
		
		private  IDbCommand BuildCommand () 
		{
			if (this.connectionObject != null) {
				IDbCommand command = this.connectionObject.Connection.CreateCommand();
				command.CommandText = this.model.ReportSettings.CommandText;
				command.CommandType = this.model.ReportSettings.CommandType;
				return command;
			}
			throw new MissingDataSourceException();
		}
	
		/*
		private DataSet ExecuteStoredProc ()
		{
			
			DbDataAdapter adapter = null;
			try {
				adapter = this.BuildAdapter();
				DataSet dataSet = ResultPanel.CreateDataSet();
				adapter.Fill(dataSet);
				return dataSet;
				
			}finally {
				if (adapter.SelectCommand.Connection.State == ConnectionState.Open) {
					adapter.SelectCommand.Connection.Close();
				}
			}
		}
		*/
		/*
		private DataSet ExecuteStoredProc (SharpQueryProcedure procedure)
		{
			
			SharpQuerySchemaClassCollection tmp = procedure.GetSchemaParameters();
			this.sqlParamsCollection = new ParameterCollection();
			SqlParameterConverter converter = new SqlParameterConverter();
			
			if (converter.CanConvertFrom(typeof(SharpQuerySchemaClassCollection))) {
				if (converter.CanConvertTo(null,typeof(ParameterCollection))){
					sqlParamsCollection = (ParameterCollection)converter.ConvertTo(null,
					                                                                   CultureInfo.InstalledUICulture,
					                                                                   tmp,
					                                                                   typeof(ParameterCollection));
				}
			}
			
			if (sqlParamsCollection.Count > 0){
				using (ParameterDialog inputform = new ParameterDialog(sqlParamsCollection)) {
					if ( inputform.ShowDialog() != DialogResult.OK ){
						return null;
					}
					else
					{
						IDbCommand command = this.BuildCommand();
						DbDataAdapter adapter = this.BuildAdapter();
						DataSet dataSet = ResultPanel.CreateDataSet();
						try {
							SqlDataAccessStrategy.BuildQueryParameters(command,sqlParamsCollection);
							adapter.SelectCommand = (DbCommand)command;
							
							adapter.Fill (dataSet);
							return dataSet;
						} catch (Exception e) {	
							MessageService.ShowError(e.Message);
						} finally {
							if (adapter.SelectCommand.Connection.State == ConnectionState.Open) {
								adapter.SelectCommand.Connection.Close();
							}
						}
					}
				}
			}
			return null;
		}
		
*/
		private DataSet BuildFromSqlString () 
		{
			DbDataAdapter adapter = null;
			try {
				adapter = this.BuildAdapter();
				DataSet dataSet = ResultPanel.CreateDataSet();
				adapter.Fill(dataSet);
				return dataSet;
			} finally {
				if (adapter.SelectCommand.Connection.State == ConnectionState.Open) {
					adapter.SelectCommand.Connection.Close();
				} 
			}
		}
		
		#endregion
		
		
		private void GrdQueryMouseUp(object sender, System.Windows.Forms.MouseEventArgs e){
			
			if (e.Button == MouseButtons.Right) {
				DataGridView.HitTestInfo hti = this.grdQuery.HitTest(e.X,e.Y);
				switch (hti.Type) {
					case DataGridViewHitTestType.ColumnHeader:
						break;
					default:
						ContextMenuStrip defMen = MenuService.CreateContextMenu (this,
						                                                        ResultPanel.defaultContextMenu);
						defMen.Show (this.grdQuery,new Point (e.X,e.Y));
						break;
				}
			}
		}
		
		#region called from commands
		
		/// <summary>
		/// Create a *.xsd File
		/// </summary>
		/// <param name="schemaOnly">true = schema only</param>
		
		public void SaveXsdFile(bool schemaOnly){
			
			using (SaveFileDialog saveFileDialog = new SaveFileDialog()){
				saveFileDialog.Filter = GlobalValues.XsdFileFilter;
				saveFileDialog.DefaultExt = GlobalValues.XsdExtension;
				saveFileDialog.AddExtension    = true;
				
				if(saveFileDialog.ShowDialog() == DialogResult.OK){
					using (System.IO.FileStream fileStream =
					       new System.IO.FileStream(saveFileDialog.FileName,
					                                System.IO.FileMode.Create)){
						XmlTextWriter xmlWriter = new XmlTextWriter(fileStream, System.Text.Encoding.UTF8);
						xmlWriter.WriteStartDocument(true);
						
						if (schemaOnly) {
							this.resultDataSet.WriteXmlSchema(xmlWriter);
						} else {
							this.resultDataSet.WriteXml(xmlWriter,XmlWriteMode.WriteSchema);
						}
						xmlWriter.Close();
					}
				}
			}
		}
		
		#endregion
		
		
		#region overrides
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (customizer == null) {
				customizer = (Properties)base.CustomizationObject;
				reportStructure = (ReportStructure)customizer.Get("Generator");
			}
			if (message == DialogMessage.Activated) 
			{
				ShowData();
				base.EnableNext = true;
				base.EnableFinish = true;
				
			} 
			else if (message == DialogMessage.Next)
			{
				base.EnableNext = true;
				base.EnableFinish = true;
				WriteResult();
			}
			else if (message == DialogMessage.Finish)
			{
				WriteResult();
				
				base.EnableNext = true;
				base.EnableFinish = true;
			}
			return true;
		}
		
		
		void ShowData()
		{
			this.model = reportStructure.CreateAndFillReportModel();
			this.resultDataSet =  FillGrid();
			if (resultDataSet.Tables.Count > 0) {
				SetupGrid ();
			}
			
		}
		
		
		private void SetupGrid()
		{
			if (this.resultDataSet != null) {
				this.grdQuery.DataSource = this.resultDataSet.Tables[0];
				foreach (DataGridViewColumn dd in this.grdQuery.Columns) {
					DataGridViewColumnHeaderCheckBoxCell cb = new DataGridViewColumnHeaderCheckBoxCell();
					cb.CheckBoxAlignment = HorizontalAlignment.Right;
					cb.Checked = true;
					dd.HeaderCell = cb;
					dd.SortMode = DataGridViewColumnSortMode.NotSortable;
				}
				this.grdQuery.AllowUserToOrderColumns = true;
			}
		}
		
		
		private void WriteResult ()
		{
				if (this.resultDataSet != null) {
					// check reordering of columns
					DataGridViewColumn[] displayCols;
					DataGridViewColumnCollection dc = this.grdQuery.Columns;
					
					displayCols = new DataGridViewColumn[dc.Count];
					for (int i = 0; i < dc.Count; i++){
						if (dc[i].Visible) {
							displayCols[dc[i].DisplayIndex] = dc[i];
						}
					}
					
					
					ReportItemCollection sourceItems = WizardHelper.ReportItemCollection(this.resultDataSet);
					
					AvailableFieldsCollection abstractColumns = WizardHelper.AvailableFieldsCollection(this.resultDataSet);
					
					ReportItemCollection destItems = new ReportItemCollection();
					
					// only checked columns are used in the report
					foreach (DataGridViewColumn cc in displayCols) {
						DataGridViewColumnHeaderCheckBoxCell hc= (DataGridViewColumnHeaderCheckBoxCell)cc.HeaderCell;
						if (hc.Checked) {
							BaseReportItem br = (BaseReportItem)sourceItems.Find(cc.HeaderText);
							destItems.Add(br);
						}
					}
					
					reportStructure.ReportItemCollection.Clear();
					reportStructure.ReportItemCollection.AddRange(destItems);
					/*
					if ((this.sqlParamsCollection != null) && (this.sqlParamsCollection.Count > 0)) {
						reportStructure.SqlQueryParameters.AddRange(sqlParamsCollection);
					}
					*/
					if (abstractColumns != null) {
						reportStructure.AvailableFieldsCollection.Clear();
						reportStructure.AvailableFieldsCollection.AddRange(abstractColumns);
					}
					/*
					if ((this.sqlParamsCollection != null) && (this.sqlParamsCollection.Count > 0)) {
						reportStructure.SqlQueryParameters.Clear();
						reportStructure.SqlQueryParameters.AddRange(sqlParamsCollection);
					}
					*/
				}
				base.EnableNext = true;
				base.EnableFinish = true;
		}
		#endregion
		
		protected override void Dispose(bool disposing)
		{
			
			if (this.grdQuery != null) {
				this.grdQuery.Dispose();
			}
			if (this.txtSqlString != null) {
				this.txtSqlString.Dispose();
			}
			if (this.resultDataSet != null) {
				this.resultDataSet.Dispose();
			}
			base.Dispose(disposing);
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.txtSqlString = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.grdQuery = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)(this.grdQuery)).BeginInit();
			this.SuspendLayout();
			// 
			// txtSqlString
			// 
			this.txtSqlString.Location = new System.Drawing.Point(16, 48);
			this.txtSqlString.Name = "txtSqlString";
			this.txtSqlString.Size = new System.Drawing.Size(408, 20);
			this.txtSqlString.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(120, 16);
			this.label1.TabIndex = 1;
			// 
			// grdQuery
			// 
			this.grdQuery.AllowUserToAddRows = false;
			this.grdQuery.AllowUserToDeleteRows = false;
			this.grdQuery.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.grdQuery.Location = new System.Drawing.Point(16, 89);
			this.grdQuery.Name = "grdQuery";
			this.grdQuery.Size = new System.Drawing.Size(408, 214);
			this.grdQuery.TabIndex = 2;
			this.grdQuery.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GrdQueryMouseUp);
			// 
			// ResultPanel
			// 
			this.ClientSize = new System.Drawing.Size(529, 332);
			this.Controls.Add(this.grdQuery);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtSqlString);
			this.Name = "ResultPanel";
			((System.ComponentModel.ISupportInitialize)(this.grdQuery)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.DataGridView grdQuery;
		#endregion
	}
}
