// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// Description of PushModelPanel.
	/// </summary>
	public class PushModelPanel : AbstractWizardPanel
	{
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnPath;
		private System.Windows.Forms.CheckedListBox checkedListBox;
		private System.Windows.Forms.TextBox txtPath;
		
		private ReportStructure reportStructure;
		private Properties customizer;
		private DataSet resultDataSet;
		
		public PushModelPanel()
		{
			InitializeComponent();
			base.EnableFinish = false;
			base.EnableCancel = true;
			base.EnableNext = false;
			Localize ();
		}
		
		
		void Localize ()
		{
			this.label1.Text = ResourceService.GetString("SharpReport.Wizard.PushModel.Path");
			this.label2.Text = ResourceService.GetString("SharpReport.Wizard.PushModel.AvailableFields");
		}
		
		
		void BtnPathClick(object sender, System.EventArgs e)
		{
			using (OpenFileDialog fdiag = new OpenFileDialog()) {
				fdiag.AddExtension    = true;
				fdiag.DefaultExt = GlobalValues.XsdExtension;
				fdiag.Filter = GlobalValues.XsdFileFilter;
				fdiag.Multiselect = false;
				if (fdiag.ShowDialog() == DialogResult.OK)
				{
					string	fileName = fdiag.FileName;
					this.txtPath.Text = fileName;
					FillDataGrid(fileName);
					base.EnableNext = true;
					base.EnableFinish= true;
				}
			}
		}
		
		
		#region  DataGridView
		
		void FillDataGrid(string fileName)
		{
			this.resultDataSet = new DataSet();
			this.resultDataSet.Locale = CultureInfo.CurrentCulture;
			resultDataSet.ReadXml (fileName);
			this.grdQuery.DataSource = resultDataSet.Tables[0];
			WizardHelper.SetupGridView(this.grdQuery);
			grdQuery.AllowUserToOrderColumns = true;
			base.EnableFinish = true;
		}

		#endregion
		
		
		#region overrides
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (customizer == null) {
				customizer = (Properties)base.CustomizationObject;
				reportStructure = (ReportStructure)customizer.Get("Generator");
				base.NextWizardPanelID = "Layout";
			}
			
			else if (message == DialogMessage.Activated) {
				WriteResult();
				base.EnableNext = true;
			}
			
			else if (message == DialogMessage.Next)
			{
				WriteResult();
				base.EnableNext = true;
			}
			else if (message == DialogMessage.Finish) {
				WriteResult();
				base.EnableNext = true;
				base.EnableFinish = true;
			}
			return true;
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
				
				reportStructure.ReportItemCollection.Clear();
				var items = WizardHelper.CreateItemsCollection(this.resultDataSet,displayCols);
				reportStructure.ReportItemCollection.AddRange(items);
				
				var abstractColumns = WizardHelper.AvailableFieldsCollection(this.resultDataSet);
				if (abstractColumns != null) {
					reportStructure.AvailableFieldsCollection.Clear();
					reportStructure.AvailableFieldsCollection.AddRange(abstractColumns);
				}
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
			this.txtPath = new System.Windows.Forms.TextBox();
			this.checkedListBox = new System.Windows.Forms.CheckedListBox();
			this.btnPath = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.grdQuery = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)(this.grdQuery)).BeginInit();
			this.SuspendLayout();
			// 
			// txtPath
			// 
			this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			                                                            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtPath.Location = new System.Drawing.Point(16, 63);
			this.txtPath.Name = "txtPath";
			this.txtPath.Size = new System.Drawing.Size(338, 20);
			this.txtPath.TabIndex = 0;
			// 
			// checkedListBox
			// 
			this.checkedListBox.Location = new System.Drawing.Point(64, 120);
			this.checkedListBox.Name = "checkedListBox";
			this.checkedListBox.Size = new System.Drawing.Size(248, 109);
			this.checkedListBox.TabIndex = 3;
			this.checkedListBox.Visible = false;
			// 
			// btnPath
			// 
			this.btnPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			                                                            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnPath.Location = new System.Drawing.Point(383, 58);
			this.btnPath.Name = "btnPath";
			this.btnPath.Size = new System.Drawing.Size(41, 25);
			this.btnPath.TabIndex = 1;
			this.btnPath.Text = "...";
			this.btnPath.Click += new System.EventHandler(this.BtnPathClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(64, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(160, 24);
			this.label1.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(16, 95);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(112, 22);
			this.label2.TabIndex = 4;
			this.label2.Text = "aaaaaa";
			// 
			// dataGridView1
			// 
			this.grdQuery.AllowUserToOrderColumns = true;
			this.grdQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			                                                              | System.Windows.Forms.AnchorStyles.Left)
			                                                             | System.Windows.Forms.AnchorStyles.Right)));
			this.grdQuery.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.grdQuery.Location = new System.Drawing.Point(16, 120);
			this.grdQuery.Name = "dataGridView1";
			this.grdQuery.Size = new System.Drawing.Size(408, 189);
			this.grdQuery.TabIndex = 5;
			// 
			// PushModelPanel
			// 
			this.ClientSize = new System.Drawing.Size(449, 332);
			this.Controls.Add(this.grdQuery);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.checkedListBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnPath);
			this.Controls.Add(this.txtPath);
			this.Name = "PushModelPanel";
			((System.ComponentModel.ISupportInitialize)(this.grdQuery)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.DataGridView grdQuery;
		#endregion
		
		
	}
}
