/*
 * Created by SharpDevelop.
 * User: Forstmeier Peter
 * Date: 23.02.2005
 * Time: 10:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Data;
using System.Globalization;
using System.ComponentModel;
using System.Collections;

using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

using SharpReport;
using SharpReportCore;
using SharpReport.ReportItems;

namespace ReportGenerator
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
		
		private ReportGenerator generator;
		private Properties customizer;
		
		private ReportItemCollection colDetail;
		
		public PushModelPanel(){
			InitializeComponent();
			base.EnableFinish = false;
			base.EnableCancel = true;
			base.EnableNext = false;
			Localise ();
		}
		
		void Localise () {
			this.label1.Text = ResourceService.GetString("SharpReport.Wizard.PushModel.Path");
			this.label2.Text = ResourceService.GetString("SharpReport.Wizard.PushModel.AvailableFields");
		}
		
		void BtnPathClick(object sender, System.EventArgs e){
			using (OpenFileDialog fdiag = new OpenFileDialog()) {
				fdiag.AddExtension    = true;
				fdiag.DefaultExt = GlobalValues.XsdExtension;
				fdiag.Filter = GlobalValues.XsdFileFilter;
				fdiag.Multiselect = false;
				if (fdiag.ShowDialog() == DialogResult.OK) {
					string	fileName = fdiag.FileName;
					this.txtPath.Text = fileName;
					customizer.Set ("XSD_File",fdiag.FileName);
					FillListBox (fileName);
				}
			}
		}
		
		#region  ListBox
		void FillListBox (string fileName) {
			DataSet ds = new DataSet();
			ds.Locale = CultureInfo.CurrentCulture;
			ds.ReadXml (fileName);
			using  (AutoReport auto = new AutoReport()){
				ReportModel model = generator.FillReportModel (new ReportModel());
				colDetail = auto.ReportItemsFromSchema(model,ds);
				
				if (colDetail != null) {
					foreach (ReportDataItem item in colDetail) {
						this.checkedListBox.Items.Add (item.MappingName,CheckState.Checked);
					}
				}
			}
			base.EnableNext = true;
			base.EnableFinish = true;
			base.IsLastPanel = true;
		}
		
		#endregion
		
		#region overrides
		public override object CustomizationObject {
			get {
				return customizer;
			}
			set {
				this.customizer = (Properties)value;
				generator = (ReportGenerator)customizer.Get("Generator");
			}
		}
		
		public override bool ReceiveDialogMessage(DialogMessage message){

			if (message == DialogMessage.Finish) {

				ReportItemCollection itemCol = new ReportItemCollection ();
				if (this.colDetail != null) {
					foreach (int ind in this.checkedListBox.CheckedIndices) {
						IItemRenderer item = this.colDetail[ind];
						itemCol.Add(item);
					}
				}
		
				customizer.Set ("ReportItemCollection",itemCol);
			}
			return true;
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
			this.SuspendLayout();
			// 
			// txtPath
			// 
			this.txtPath.Location = new System.Drawing.Point(64, 64);
			this.txtPath.Name = "txtPath";
			this.txtPath.Size = new System.Drawing.Size(248, 20);
			this.txtPath.TabIndex = 0;
			this.txtPath.Text = "";
			// 
			// checkedListBox
			// 
			this.checkedListBox.Location = new System.Drawing.Point(64, 120);
			this.checkedListBox.Name = "checkedListBox";
			this.checkedListBox.Size = new System.Drawing.Size(248, 109);
			this.checkedListBox.TabIndex = 3;
			// 
			// btnPath
			// 
			this.btnPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnPath.Location = new System.Drawing.Point(336, 64);
			this.btnPath.Name = "btnPath";
			this.btnPath.Size = new System.Drawing.Size(32, 21);
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
			this.label2.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(64, 96);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(112, 16);
			this.label2.TabIndex = 4;
			// 
			// PushModelPanel
			// 
			this.Controls.Add(this.label2);
			this.Controls.Add(this.checkedListBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnPath);
			this.Controls.Add(this.txtPath);
			this.Name = "PushModelPanel";
			this.Size = new System.Drawing.Size(392, 266);
			this.ResumeLayout(false);
		}
		#endregion
		
		
	}
}
