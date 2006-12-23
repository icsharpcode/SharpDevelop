/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 23/09/2006
 * Time: 14:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ClassDiagramApp
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.saveImgBtn = new System.Windows.Forms.Button();
			this.addNoteBtn = new System.Windows.Forms.Button();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.shrinkBtn = new System.Windows.Forms.Button();
			this.matchBtn = new System.Windows.Forms.Button();
			this.modifiedBtn = new System.Windows.Forms.Button();
			this.layoutBtn = new System.Windows.Forms.Button();
			this.initBtn = new System.Windows.Forms.Button();
			this.loadBtn = new System.Windows.Forms.Button();
			this.saveBtn = new System.Windows.Forms.Button();
			this.colExpBtn = new System.Windows.Forms.Button();
			this.zoom = new System.Windows.Forms.TrackBar();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.addInterfaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addEnumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.zoom)).BeginInit();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(631, 480);
			this.splitContainer1.SplitterDistance = 278;
			this.splitContainer1.TabIndex = 0;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.saveImgBtn);
			this.splitContainer2.Panel1.Controls.Add(this.addNoteBtn);
			this.splitContainer2.Panel1.Controls.Add(this.comboBox1);
			this.splitContainer2.Panel1.Controls.Add(this.shrinkBtn);
			this.splitContainer2.Panel1.Controls.Add(this.matchBtn);
			this.splitContainer2.Panel1.Controls.Add(this.modifiedBtn);
			this.splitContainer2.Panel1.Controls.Add(this.layoutBtn);
			this.splitContainer2.Panel1.Controls.Add(this.initBtn);
			this.splitContainer2.Panel1.Controls.Add(this.loadBtn);
			this.splitContainer2.Panel1.Controls.Add(this.saveBtn);
			this.splitContainer2.Panel1.Controls.Add(this.colExpBtn);
			this.splitContainer2.Panel1.Controls.Add(this.zoom);
			this.splitContainer2.Size = new System.Drawing.Size(631, 198);
			this.splitContainer2.SplitterDistance = 222;
			this.splitContainer2.TabIndex = 5;
			// 
			// saveImgBtn
			// 
			this.saveImgBtn.Location = new System.Drawing.Point(113, 170);
			this.saveImgBtn.Name = "saveImgBtn";
			this.saveImgBtn.Size = new System.Drawing.Size(104, 23);
			this.saveImgBtn.TabIndex = 16;
			this.saveImgBtn.Text = "Save as Bitmap";
			this.saveImgBtn.UseVisualStyleBackColor = true;
			this.saveImgBtn.Click += new System.EventHandler(this.SaveImgBtnClick);
			// 
			// addNoteBtn
			// 
			this.addNoteBtn.Location = new System.Drawing.Point(3, 170);
			this.addNoteBtn.Name = "addNoteBtn";
			this.addNoteBtn.Size = new System.Drawing.Size(104, 23);
			this.addNoteBtn.TabIndex = 15;
			this.addNoteBtn.Text = "Add Note";
			this.addNoteBtn.UseVisualStyleBackColor = true;
			this.addNoteBtn.Click += new System.EventHandler(this.AddNoteBtnClick);
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
									"10%",
									"25%",
									"50%",
									"75%",
									"100%",
									"125%",
									"150%",
									"175%",
									"200%",
									"250%",
									"300%",
									"350%",
									"400%"});
			this.comboBox1.Location = new System.Drawing.Point(113, 12);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(104, 21);
			this.comboBox1.TabIndex = 14;
			this.comboBox1.Text = "100%";
			this.comboBox1.TextChanged += new System.EventHandler(this.ComboBox1TextChanged);
			// 
			// shrinkBtn
			// 
			this.shrinkBtn.Location = new System.Drawing.Point(113, 141);
			this.shrinkBtn.Name = "shrinkBtn";
			this.shrinkBtn.Size = new System.Drawing.Size(104, 23);
			this.shrinkBtn.TabIndex = 13;
			this.shrinkBtn.Text = "Shrink";
			this.shrinkBtn.UseVisualStyleBackColor = true;
			this.shrinkBtn.Click += new System.EventHandler(this.ShrinkBtnClick);
			// 
			// matchBtn
			// 
			this.matchBtn.Location = new System.Drawing.Point(113, 112);
			this.matchBtn.Name = "matchBtn";
			this.matchBtn.Size = new System.Drawing.Size(104, 23);
			this.matchBtn.TabIndex = 12;
			this.matchBtn.Text = "Match";
			this.matchBtn.UseVisualStyleBackColor = true;
			this.matchBtn.Click += new System.EventHandler(this.MatchBtnClick);
			// 
			// modifiedBtn
			// 
			this.modifiedBtn.Enabled = false;
			this.modifiedBtn.Location = new System.Drawing.Point(3, 141);
			this.modifiedBtn.Name = "modifiedBtn";
			this.modifiedBtn.Size = new System.Drawing.Size(104, 23);
			this.modifiedBtn.TabIndex = 11;
			this.modifiedBtn.Text = "Modified";
			this.modifiedBtn.UseVisualStyleBackColor = true;
			this.modifiedBtn.Click += new System.EventHandler(this.ModifiedBtnClick);
			// 
			// layoutBtn
			// 
			this.layoutBtn.Location = new System.Drawing.Point(113, 83);
			this.layoutBtn.Name = "layoutBtn";
			this.layoutBtn.Size = new System.Drawing.Size(104, 23);
			this.layoutBtn.TabIndex = 10;
			this.layoutBtn.Text = "Layout";
			this.layoutBtn.UseVisualStyleBackColor = true;
			this.layoutBtn.Click += new System.EventHandler(this.LayoutBtnClick);
			// 
			// initBtn
			// 
			this.initBtn.Location = new System.Drawing.Point(3, 54);
			this.initBtn.Name = "initBtn";
			this.initBtn.Size = new System.Drawing.Size(104, 23);
			this.initBtn.TabIndex = 9;
			this.initBtn.Text = "Init";
			this.initBtn.UseVisualStyleBackColor = true;
			this.initBtn.Click += new System.EventHandler(this.InitBtnClick);
			// 
			// loadBtn
			// 
			this.loadBtn.Location = new System.Drawing.Point(3, 83);
			this.loadBtn.Name = "loadBtn";
			this.loadBtn.Size = new System.Drawing.Size(104, 23);
			this.loadBtn.TabIndex = 8;
			this.loadBtn.Text = "Load";
			this.loadBtn.UseVisualStyleBackColor = true;
			this.loadBtn.Click += new System.EventHandler(this.LoadBtnClick);
			// 
			// saveBtn
			// 
			this.saveBtn.Location = new System.Drawing.Point(3, 112);
			this.saveBtn.Name = "saveBtn";
			this.saveBtn.Size = new System.Drawing.Size(104, 23);
			this.saveBtn.TabIndex = 7;
			this.saveBtn.Text = "Save";
			this.saveBtn.UseVisualStyleBackColor = true;
			this.saveBtn.Click += new System.EventHandler(this.SaveBtnClick);
			// 
			// colExpBtn
			// 
			this.colExpBtn.Location = new System.Drawing.Point(113, 54);
			this.colExpBtn.Name = "colExpBtn";
			this.colExpBtn.Size = new System.Drawing.Size(104, 23);
			this.colExpBtn.TabIndex = 6;
			this.colExpBtn.Text = "Collapse All";
			this.colExpBtn.UseVisualStyleBackColor = true;
			this.colExpBtn.Click += new System.EventHandler(this.ColExpBtnClick);
			// 
			// zoom
			// 
			this.zoom.Location = new System.Drawing.Point(3, 3);
			this.zoom.Maximum = 400;
			this.zoom.Minimum = 25;
			this.zoom.Name = "zoom";
			this.zoom.Size = new System.Drawing.Size(104, 45);
			this.zoom.SmallChange = 25;
			this.zoom.TabIndex = 5;
			this.zoom.TickFrequency = 25;
			this.zoom.Value = 100;
			this.zoom.Scroll += new System.EventHandler(this.ZoomValueChanged);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.toolStripMenuItem1,
									this.addInterfaceToolStripMenuItem,
									this.addEnumToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(142, 70);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(141, 22);
			this.toolStripMenuItem1.Text = "Add Class";
			this.toolStripMenuItem1.Click += new System.EventHandler(this.ToolStripMenuItem1Click);
			// 
			// addInterfaceToolStripMenuItem
			// 
			this.addInterfaceToolStripMenuItem.Name = "addInterfaceToolStripMenuItem";
			this.addInterfaceToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.addInterfaceToolStripMenuItem.Text = "Add Interface";
			// 
			// addEnumToolStripMenuItem
			// 
			this.addEnumToolStripMenuItem.Name = "addEnumToolStripMenuItem";
			this.addEnumToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.addEnumToolStripMenuItem.Text = "Add Enum";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(631, 480);
			this.Controls.Add(this.splitContainer1);
			this.Name = "MainForm";
			this.Text = "ClassDiagramApp";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFormFormClosed);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.zoom)).EndInit();
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.Button saveImgBtn;
		private System.Windows.Forms.Button addNoteBtn;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Button shrinkBtn;
		private System.Windows.Forms.Button matchBtn;
		private System.Windows.Forms.Button layoutBtn;
		private System.Windows.Forms.Button modifiedBtn;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.Button loadBtn;
		private System.Windows.Forms.Button initBtn;
		private System.Windows.Forms.Button saveBtn;
		private System.Windows.Forms.Button colExpBtn;
		private System.Windows.Forms.TrackBar zoom;
		private System.Windows.Forms.ToolStripMenuItem addEnumToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addInterfaceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.SplitContainer splitContainer1;

	}
}
