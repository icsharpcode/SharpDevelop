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
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.Data.Core.Enums;
using ICSharpCode.Data.Core.Interfaces;
using ICSharpCode.Reports.Addin.Commands;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.DataAccess;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Project.BaseClasses;
using ICSharpCode.Reports.Core.Project.Interfaces;
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
		private ReportModel model;
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
			SqlQueryChecker.Check(model.ReportSettings.CommandType,
			                      model.ReportSettings.CommandText);
			
			DataSet dataSet = ResultPanel.CreateDataSet ();
			
			this.txtSqlString.Text = model.ReportSettings.CommandText;
			
			var dataAccess = new SqlDataAccessStrategy(model.ReportSettings);
			
			switch (model.ReportSettings.CommandType) {
				case CommandType.Text:
					dataSet = DataSetFromSqlText(dataAccess);	
					break;
				case CommandType.StoredProcedure:
					dataSet = DataSetFromStoredProcedure(dataAccess);
					break;
				case CommandType.TableDirect:
					MessageService.ShowError("TableDirect is not suppurted at the moment");
					break;
				default:
					throw new Exception("Invalid value for CommandType");
			}
			return dataSet;
		}
		
		
		string CreateTableName(ReportStructure reportStructure)
		{
			ITable t = reportStructure.IDatabaseObjectBase as ITable;
			string str = String.Empty;
			
			if (t != null) {
				str = t.Name;
			}
			else
			{
				str = "Table1";
			}
			return str;
		}
		
		
		private DataSet DataSetFromSqlText(IDataAccessStrategy dataAccess)
		{
			var dataSet = dataAccess.ReadData();
			dataSet.Tables[0].TableName = CreateTableName (reportStructure);
			return dataSet;
		}
		
		
		private DataSet DataSetFromStoredProcedure(IDataAccessStrategy dataAccess)
		{
			DataSet dataSet = ResultPanel.CreateDataSet();
			
			IProcedure procedure = reportStructure.IDatabaseObjectBase as IProcedure;
			var sqlParamCollection = CreateSqlParameters(procedure);
			
			if (sqlParamCollection.Count > 0) {
				reportStructure.SqlQueryParameters.AddRange(sqlParamCollection);
				model.ReportSettings.SqlParameters.AddRange(sqlParamCollection);
				CollectParamValues(model.ReportSettings);

			}
			dataSet = dataAccess.ReadData();
			dataSet.Tables[0].TableName = procedure.Name;
			return dataSet;
		}
		
		
		void CollectParamValues(ReportSettings reportSettings){

			CollectParametersCommand p = new CollectParametersCommand(reportSettings);
			p.Run();
		}
		
		
		SqlParameterCollection CreateSqlParameters(IProcedure procedure)
		{
			SqlParameterCollection col = new SqlParameterCollection();
			SqlParameter par = null;
			foreach (var element in procedure.Items) {
				DbType dbType = TypeHelpers.DbTypeFromStringRepresenation(element.DataType);
				par	 = new SqlParameter(element.Name,dbType,"",ParameterDirection.Input);
				
				if (element.ParameterMode == ParameterMode.In) {
					par.ParameterDirection = ParameterDirection.Input;
					
				} else if (element.ParameterMode == ParameterMode. InOut){
					par.ParameterDirection = ParameterDirection.InputOutput;
				}
				col.Add(par);
			}
			return col;
		}
		
		
		private static DataSet CreateDataSet() 
		{
			DataSet dataSet = new DataSet();
			dataSet.Locale = CultureInfo.CurrentCulture;
			return dataSet;
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
			reportStructure = (ReportStructure)base.CustomizationObject;
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
				WizardHelper.SetupGridView(this.grdQuery);
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
				
				ReportItemCollection destItems = WizardHelper.CreateItemsCollection(this.resultDataSet,displayCols);
				reportStructure.ReportItemCollection.Clear();
				reportStructure.ReportItemCollection.AddRange(destItems);
				
				
				var abstractColumns = WizardHelper.AvailableFieldsCollection(this.resultDataSet);
				if (abstractColumns != null) {
					reportStructure.AvailableFieldsCollection.Clear();
					reportStructure.AvailableFieldsCollection.AddRange(abstractColumns);
				}
				
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
