// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

namespace HexEditor.View
{
    partial class HexEditContainer
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
            if (disposing)
            {
                if (components != null)
                {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HexEditContainer));
            this.tbSizeToFit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tCBViewMode = new System.Windows.Forms.DomainUpDown();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tSTBCharsPerLine = new System.Windows.Forms.NumericUpDown();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.hexEditControl = new HexEditor.Editor();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbSizeToFit
            // 
            this.tbSizeToFit.CheckOnClick = true;
            this.tbSizeToFit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbSizeToFit.Image = ((System.Drawing.Image)(resources.GetObject("tbSizeToFit.Image")));
            this.tbSizeToFit.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbSizeToFit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbSizeToFit.Name = "tbSizeToFit";
            this.tbSizeToFit.Size = new System.Drawing.Size(23, 22);
            this.tbSizeToFit.Text = "${res:AddIns.HexEditor.SizeToFit}";
            this.tbSizeToFit.Click += new System.EventHandler(this.TbSizeToFitClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tCBViewMode
            // 
            this.tCBViewMode.Items.AddRange(new object[] {
            "Hexadecimal",
            "Octal",
            "Decimal"});
            this.tCBViewMode.Name = "tCBViewMode";
            this.tCBViewMode.Size = new System.Drawing.Size(121, 25);
            this.tCBViewMode.SelectedItemChanged += new System.EventHandler(this.TCBViewModeSelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbSizeToFit,
            this.toolStripSeparator1,
            this.toolStripProgressBar1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(689, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tSTBCharsPerLine
            // 
            this.tSTBCharsPerLine.Name = "tSTBCharsPerLine";
            this.tSTBCharsPerLine.Size = new System.Drawing.Size(40, 25);
            this.tSTBCharsPerLine.TextChanged += new System.EventHandler(tSTBCharsPerLine_TextChanged);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.toolStripProgressBar1.Size = new System.Drawing.Size(150, 22);
            this.toolStripProgressBar1.Visible = false;
            // 
            // hexEditControl
            // 
            this.hexEditControl.BackColor = System.Drawing.Color.White;
            this.hexEditControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.hexEditControl.BytesPerLine = 16;
            this.hexEditControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexEditControl.Encoding = ((System.Text.Encoding)(resources.GetObject("hexEditControl.Encoding")));
            this.hexEditControl.FileName = null;
            this.hexEditControl.FitToWindowWidth = false;
            this.hexEditControl.Location = new System.Drawing.Point(0, 25);
            this.hexEditControl.Name = "hexEditControl";
            this.hexEditControl.ProgressBar = this.toolStripProgressBar1;
            this.hexEditControl.Size = new System.Drawing.Size(689, 308);
            this.hexEditControl.TabIndex = 3;
            this.hexEditControl.ViewMode = HexEditor.Util.ViewMode.Hexadecimal;
            // 
            // HexEditContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.hexEditControl);
            this.Controls.Add(this.toolStrip1);
            this.Name = "HexEditContainer";
            this.Size = new System.Drawing.Size(689, 333);
            this.Resize += new System.EventHandler(this.HexEditContainer_Resize);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        internal HexEditor.Editor hexEditControl;
        private System.Windows.Forms.DomainUpDown tCBViewMode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tbSizeToFit;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.NumericUpDown tSTBCharsPerLine;
    }
}
