/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 03.10.2008
 * Zeit: 17:52
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
namespace ICSharpCode.Reports.Addin.ReportWizard
{
	partial class LayoutPanelControl
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioTableLayout = new System.Windows.Forms.RadioButton();
			this.radioListLayout = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioTableLayout);
			this.groupBox1.Controls.Add(this.radioListLayout);
			this.groupBox1.Location = new System.Drawing.Point(17, 18);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(232, 103);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Report Layout";
			// 
			// radioTableLayout
			// 
			this.radioTableLayout.Location = new System.Drawing.Point(64, 65);
			this.radioTableLayout.Name = "radioTableLayout";
			this.radioTableLayout.Size = new System.Drawing.Size(104, 24);
			this.radioTableLayout.TabIndex = 3;
			this.radioTableLayout.TabStop = true;
			this.radioTableLayout.Text = "Table Layout";
			this.radioTableLayout.UseVisualStyleBackColor = true;
			// 
			// radioListLayout
			// 
			this.radioListLayout.Location = new System.Drawing.Point(64, 19);
			this.radioListLayout.Name = "radioListLayout";
			this.radioListLayout.Size = new System.Drawing.Size(104, 24);
			this.radioListLayout.TabIndex = 2;
			this.radioListLayout.TabStop = true;
			this.radioListLayout.Text = "List Layout";
			this.radioListLayout.UseVisualStyleBackColor = true;
			// 
			// LayoutPanelControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Name = "LayoutPanelControl";
			this.Size = new System.Drawing.Size(270, 144);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.RadioButton radioTableLayout;
		private System.Windows.Forms.RadioButton radioListLayout;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}
