namespace Debugger.AddIn.Visualizers.Graph
{
	partial class VisualizerWinFormsControl
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
			this.components = new System.ComponentModel.Container();
			this.txtExpression = new System.Windows.Forms.TextBox();
			this.btnInspect = new System.Windows.Forms.Button();
			this.lblInfo = new System.Windows.Forms.Label();
			this.lblExpression = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// txtExpression
			// 
			this.txtExpression.Location = new System.Drawing.Point(88, 13);
			this.txtExpression.Name = "txtExpression";
			this.txtExpression.Size = new System.Drawing.Size(131, 20);
			this.txtExpression.TabIndex = 0;
			this.toolTip1.SetToolTip(this.txtExpression, "Expression (e.g. variable name) to be inspected");
			// 
			// btnInspect
			// 
			this.btnInspect.Location = new System.Drawing.Point(225, 10);
			this.btnInspect.Name = "btnInspect";
			this.btnInspect.Size = new System.Drawing.Size(75, 23);
			this.btnInspect.TabIndex = 1;
			this.btnInspect.Text = "Inspect";
			this.btnInspect.UseVisualStyleBackColor = true;
			this.btnInspect.Click += new System.EventHandler(this.BtnInspectClick);
			// 
			// lblInfo
			// 
			this.lblInfo.Location = new System.Drawing.Point(11, 58);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(349, 23);
			this.lblInfo.TabIndex = 2;
			this.lblInfo.Text = "< Result >";
			// 
			// lblExpression
			// 
			this.lblExpression.Location = new System.Drawing.Point(11, 14);
			this.lblExpression.Name = "lblExpression";
			this.lblExpression.Size = new System.Drawing.Size(71, 23);
			this.lblExpression.TabIndex = 4;
			this.lblExpression.Text = "Expression:";
			// 
			// VisualizerWinFormsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblExpression);
			this.Controls.Add(this.lblInfo);
			this.Controls.Add(this.btnInspect);
			this.Controls.Add(this.txtExpression);
			this.Name = "VisualizerWinFormsControl";
			this.Size = new System.Drawing.Size(525, 426);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button btnInspect;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label lblExpression;
		private System.Windows.Forms.Label lblInfo;
		private System.Windows.Forms.TextBox txtExpression;
	}
}
