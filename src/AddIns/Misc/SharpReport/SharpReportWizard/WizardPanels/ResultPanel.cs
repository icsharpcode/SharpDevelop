/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 25.02.2005
 * Time: 10:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Globalization;
using System.Data;
using System.Data.OleDb;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

using SharpReport;
using SharpReportCore;
using ICSharpCode.SharpDevelop.Gui;

using SharpQuery.SchemaClass;
using SharpQuery.Connection;
using SharpQuery.Gui.DataView;
using SharpQuery.Collections;

namespace ReportGenerator{
	/// <summary>
	/// This Class show the Result of the Query build in the previos window,
	/// also we can create *.xsd File's (Schema onla and/or Data + Schema)
	/// </summary>
	public class ResultPanel : AbstractWizardPanel{
		
		const string contextMenuPath = "/ReportGenerator/ResultPanel/ContextMenu";
		
		
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtSqlString;
		private System.Windows.Forms.DataGrid grdQuery;
		
		private ReportGenerator generator;
		private Properties customizer;
		private ReportModel model;
		private SharpQuerySchemaClassCollection parametersClass;
		
		private DataSet resultDataSet;
		private ReportItemCollection colDetail;
		private ColumnCollection abstractColumns;
		
		public ResultPanel(){
			InitializeComponent();
			base.EnableFinish = true;
			base.EnableCancel = true;
			base.EnableNext = true;
			base.Refresh();
			this.label1.Text = ResourceService.GetString("SharpReport.Wizard.PullModel.CommandText");
			
		}
		
		#region Fill data
		private DataSet FillGrid() {
			this.grdQuery.DataSource = null;
			this.txtSqlString.Text = null;
	
			DataSet dataSet = new DataSet();
			dataSet.Locale = CultureInfo.CurrentCulture;
			this.txtSqlString.Text = model.ReportSettings.CommandText;
			
			if (model.ReportSettings.CommandType == CommandType.StoredProcedure){
				
				if (generator.SharpQueryProcedure == null) {
					throw new NullReferenceException("ResultPanel:FillGrid");
				}
				System.Console.WriteLine("\tStoredProcedure {0}",generator.SharpQueryProcedure.Name);
				SharpQueryProcedure proc = generator.SharpQueryProcedure;
				
				// we have some Parameters, to get them we use the Dialogfrom SharpQuery
				if (proc.GetSchemaParameters() != null && proc.GetSchemaParameters().Count > 0) {
					SharpQuerySchemaClassCollection tmp = proc.GetSchemaParameters();
					SharpQueryParameterCollection parameters = null;
					if (tmp.Count == 1 && tmp[0] is SharpQueryNotSupported) {
						parameters = new SharpQueryParameterCollection();
					}
					else{
						parameters = new SharpQueryParameterCollection();
						foreach (SharpQueryParameter par in tmp){
							parameters.Add(par);
						}
					}
//					SharpQueryParameterCollection parameters = new SharpQueryParameterCollection(tmp);
					System.Console.WriteLine("\t\t # of params {0}",parameters.Count);
					if (parameters != null && parameters.Count > 0){
						
						using (SQLParameterInput inputform = new SQLParameterInput(parameters)){
							if ( inputform.ShowDialog() != DialogResult.OK ){
								parametersClass = null;
							}else{
								parametersClass	= parameters.ToBaseSchemaCollection();
								if (parametersClass != null) {
									dataSet = (DataSet) proc.Execute(0,parametersClass);
									generator.Parameters = parametersClass;
								}
							}
						}
					}
					
				} else {
					System.Console.WriteLine("\t\t No params");
					try {
						// Stored Proc without Parameters
						dataSet = (DataSet) proc.Execute(0,proc.GetSchemaParameters());
					} catch (Exception) {
						throw;
					}
					
				}
			}
			
			// from here we create from an SqlString like "Select...."
			if (model.ReportSettings.CommandType == CommandType.Text){
				System.Console.WriteLine("\tCommandText");
				try {
					generator.Parameters = null;
					this.txtSqlString.Text = model.ReportSettings.CommandText;
					dataSet = BuildFromSqlString(model.ReportSettings);
					
				} catch (OleDbException) {
					throw;
				}
				catch (Exception) {
					throw;
				}
			}
			CreateCollections (dataSet);
			
			return dataSet;
		}
		
		private void CreateCollections (DataSet dataSet) {
			using  (AutoReport auto = new AutoReport()){
				abstractColumns  = auto.AbstractColumnsFromDataSet (dataSet);
				colDetail = auto.ReportItemsFromSchema(this.model,dataSet);
			}
		}
		
		private static DataSet BuildFromSqlString(ReportSettings settings) {
			OLEDBConnectionWrapper con = null;
			DataSet ds = null;
			if (settings.CommandText.IndexOf("?") > 0) {
				throw new NotSupportedException ("Parameters in SqlText are not supportet");
			}
			
			try {
				con = new OLEDBConnectionWrapper(settings.ConnectionString);
				con.Open();
				if (con.IsOpen) {
					ds = (DataSet)con.ExecuteSQL(settings.CommandText,0);
					return ds;
				}
			} catch (Exception) {
				throw;
			} finally {
				con.Close();
			}
			return null;
		}
		#endregion
		
		#region Create a *.Xsd File
		private void GrdQueryMouseUp(object sender, System.Windows.Forms.MouseEventArgs e){
			if (e.Button == MouseButtons.Right) {
				ContextMenuStrip ctMen = MenuService.CreateContextMenu (this,contextMenuPath);
				ctMen.Show (this.grdQuery,new Point (e.X,e.Y));
				
			}
		}
		
		/// <summary>
		/// Create a *.xsd File
		/// </summary>
		/// <param name="schemaOnly">true = schema only</param>
		
		public void SaveXsdFile(bool schemaOnly){
			try {
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
			} catch (Exception ) {
				throw ;
			}
		}
		
		#endregion
		
		#region overrides
		public override bool ReceiveDialogMessage(DialogMessage message){
			if (message == DialogMessage.Activated) {
				try {
					this.resultDataSet = new DataSet();
					this.resultDataSet.Locale = CultureInfo.InvariantCulture;
					this.model = generator.FillReportModel(new ReportModel());
					this.resultDataSet =  FillGrid();
					if (this.resultDataSet != null) {
						this.grdQuery.DataSource = this.resultDataSet.Tables[0];
						
					}
					base.EnableNext = true;
					base.EnableFinish = true;
				} catch (Exception e) {
					MessageService.ShowError(e.Message);
					base.EnableNext = false;
					base.EnableFinish = false;
				}
			} else if (message == DialogMessage.Finish) {
				customizer.Set ("ColumnCollection",abstractColumns);
				customizer.Set ("ReportItemCollection",colDetail);
				base.EnableNext = true;
				base.EnableFinish = true;
			}
			return true;
		}
		
		
		public override object CustomizationObject {
			get {
				return customizer;
			}
			set {
				this.customizer = (Properties)value;
				generator = (ReportGenerator)customizer.Get("Generator");
			}
		}

		#endregion
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.grdQuery = new System.Windows.Forms.DataGrid();
			this.txtSqlString = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.grdQuery)).BeginInit();
			this.SuspendLayout();
			// 
			// grdQuery
			// 
			this.grdQuery.CaptionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.grdQuery.DataMember = "";
			this.grdQuery.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.grdQuery.Location = new System.Drawing.Point(16, 88);
			this.grdQuery.Name = "grdQuery";
			this.grdQuery.Size = new System.Drawing.Size(408, 264);
			this.grdQuery.TabIndex = 2;
			this.grdQuery.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GrdQueryMouseUp);
			// 
			// txtSqlString
			// 
			this.txtSqlString.Location = new System.Drawing.Point(16, 48);
			this.txtSqlString.Name = "txtSqlString";
			this.txtSqlString.Size = new System.Drawing.Size(408, 20);
			this.txtSqlString.TabIndex = 0;
			this.txtSqlString.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(120, 16);
			this.label1.TabIndex = 1;
		
			// 
			// ResultPanel
			// 
			this.Controls.Add(this.grdQuery);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtSqlString);
			this.Name = "ResultPanel";
			this.Size = new System.Drawing.Size(440, 368);
			((System.ComponentModel.ISupportInitialize)(this.grdQuery)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
	}
}
