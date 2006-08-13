/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 09.08.2006
 * Time: 13:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using SharpReportCore;

namespace SharpReport{
	/// <summary>
	/// Description of ParameterDialog.
	/// </summary>
	public  class ParameterDialog : System.Windows.Forms.Form
	{
		private SqlParametersCollection collection;
		public ParameterDialog(SqlParametersCollection collection):this(){
			
			this.collection = collection;
			this.dataGrid1.DataSource = this.collection;
		}
		
		public ParameterDialog()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
		}

		#region Designer generated
		
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override  void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.dataGrid1 = new System.Windows.Forms.DataGrid();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.groupBox = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// dataGrid1
			// 
			this.dataGrid1.DataMember = "";
			this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid1.Location = new System.Drawing.Point(46, 87);
			this.dataGrid1.Name = "dataGrid1";
			this.dataGrid1.Size = new System.Drawing.Size(363, 120);
			this.dataGrid1.TabIndex = 1;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77.28119F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.71881F));
			this.tableLayoutPanel1.Controls.Add(this.cancelButton, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.okButton, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(10, 233);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(457, 30);
			this.tableLayoutPanel1.TabIndex = 3;
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.cancelButton.Location = new System.Drawing.Point(374, 3);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(80, 24);
			this.cancelButton.TabIndex = 0;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.okButton.Location = new System.Drawing.Point(275, 3);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 24);
			this.okButton.TabIndex = 1;
			this.okButton.Text = "Ok";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// groupBox
			// 
			this.groupBox.Controls.Add(this.label2);
			this.groupBox.Controls.Add(this.label1);
			this.groupBox.Location = new System.Drawing.Point(46, 13);
			this.groupBox.Name = "groupBox";
			this.groupBox.Size = new System.Drawing.Size(363, 56);
			this.groupBox.TabIndex = 5;
			this.groupBox.TabStop = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(109, 17);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(248, 17);
			this.label2.TabIndex = 5;
			this.label2.Text = "label2";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(97, 17);
			this.label1.TabIndex = 3;
			this.label1.Text = "Stored Procedure:";
			// 
			// ParameterDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(477, 273);
			this.ControlBox = false;
			this.Controls.Add(this.groupBox);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.dataGrid1);
			this.Name = "ParameterDialog";
			this.Padding = new System.Windows.Forms.Padding(10);
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ParameterDialog";
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.groupBox.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.GroupBox groupBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DataGrid dataGrid1;
		#endregion
	}
}
