namespace CSharpEditor
{
	partial class MainForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.textEditorControl1 = new ICSharpCode.TextEditor.TextEditorControl();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.parserThreadLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// textEditorControl1
			// 
			this.textEditorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textEditorControl1.IsIconBarVisible = false;
			this.textEditorControl1.Location = new System.Drawing.Point(0, 0);
			this.textEditorControl1.Name = "textEditorControl1";
			this.textEditorControl1.ShowEOLMarkers = true;
			this.textEditorControl1.ShowInvalidLines = false;
			this.textEditorControl1.ShowSpaces = true;
			this.textEditorControl1.ShowTabs = true;
			this.textEditorControl1.ShowVRuler = true;
			this.textEditorControl1.Size = new System.Drawing.Size(556, 248);
			this.textEditorControl1.TabIndex = 0;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.parserThreadLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 248);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(556, 22);
			this.statusStrip1.TabIndex = 1;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// parserThreadLabel
			// 
			this.parserThreadLabel.Name = "parserThreadLabel";
			this.parserThreadLabel.Size = new System.Drawing.Size(109, 17);
			this.parserThreadLabel.Text = "toolStripStatusLabel1";
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "Icons.16x16.Class.png");
			this.imageList1.Images.SetKeyName(1, "Icons.16x16.Method.png");
			this.imageList1.Images.SetKeyName(2, "Icons.16x16.Property.png");
			this.imageList1.Images.SetKeyName(3, "Icons.16x16.Field.png");
			this.imageList1.Images.SetKeyName(4, "Icons.16x16.Enum.png");
			this.imageList1.Images.SetKeyName(5, "Icons.16x16.NameSpace.png");
			this.imageList1.Images.SetKeyName(6, "Icons.16x16.Event.png");
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(556, 270);
			this.Controls.Add(this.textEditorControl1);
			this.Controls.Add(this.statusStrip1);
			this.Name = "MainForm";
			this.Text = "CSharpEditor";
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		internal System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ToolStripStatusLabel parserThreadLabel;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private ICSharpCode.TextEditor.TextEditorControl textEditorControl1;
	}
}
