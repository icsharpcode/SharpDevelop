namespace CodeFormatServiceClient
{
    partial class CodeFormatForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabctrlMain = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPageOutput = new System.Windows.Forms.TabPage();
            this.tabPagePreview = new System.Windows.Forms.TabPage();
            this.availableHighlighters = new System.Windows.Forms.ComboBox();
            this.sourceTextDocument = new System.Windows.Forms.TextBox();
            this.buttonFormatCode = new System.Windows.Forms.Button();
            this.htmlOutput = new System.Windows.Forms.TextBox();
            this.formatPreview = new System.Windows.Forms.WebBrowser();
            this.tabctrlMain.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPageOutput.SuspendLayout();
            this.tabPagePreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabctrlMain
            // 
            this.tabctrlMain.Controls.Add(this.tabPage1);
            this.tabctrlMain.Controls.Add(this.tabPageOutput);
            this.tabctrlMain.Controls.Add(this.tabPagePreview);
            this.tabctrlMain.Location = new System.Drawing.Point(12, 12);
            this.tabctrlMain.Name = "tabctrlMain";
            this.tabctrlMain.SelectedIndex = 0;
            this.tabctrlMain.Size = new System.Drawing.Size(721, 436);
            this.tabctrlMain.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.buttonFormatCode);
            this.tabPage1.Controls.Add(this.sourceTextDocument);
            this.tabPage1.Controls.Add(this.availableHighlighters);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(713, 410);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Input";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPageOutput
            // 
            this.tabPageOutput.Controls.Add(this.htmlOutput);
            this.tabPageOutput.Location = new System.Drawing.Point(4, 22);
            this.tabPageOutput.Name = "tabPageOutput";
            this.tabPageOutput.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOutput.Size = new System.Drawing.Size(713, 410);
            this.tabPageOutput.TabIndex = 1;
            this.tabPageOutput.Text = "Output";
            this.tabPageOutput.UseVisualStyleBackColor = true;
            // 
            // tabPagePreview
            // 
            this.tabPagePreview.Controls.Add(this.formatPreview);
            this.tabPagePreview.Location = new System.Drawing.Point(4, 22);
            this.tabPagePreview.Name = "tabPagePreview";
            this.tabPagePreview.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePreview.Size = new System.Drawing.Size(713, 410);
            this.tabPagePreview.TabIndex = 2;
            this.tabPagePreview.Text = "Preview";
            this.tabPagePreview.UseVisualStyleBackColor = true;
            // 
            // availableHighlighters
            // 
            this.availableHighlighters.FormattingEnabled = true;
            this.availableHighlighters.Location = new System.Drawing.Point(6, 6);
            this.availableHighlighters.Name = "availableHighlighters";
            this.availableHighlighters.Size = new System.Drawing.Size(121, 21);
            this.availableHighlighters.TabIndex = 1;
            // 
            // sourceTextDocument
            // 
            this.sourceTextDocument.AcceptsReturn = true;
            this.sourceTextDocument.AcceptsTab = true;
            this.sourceTextDocument.Location = new System.Drawing.Point(6, 33);
            this.sourceTextDocument.Multiline = true;
            this.sourceTextDocument.Name = "sourceTextDocument";
            this.sourceTextDocument.Size = new System.Drawing.Size(701, 371);
            this.sourceTextDocument.TabIndex = 2;
            // 
            // buttonFormatCode
            // 
            this.buttonFormatCode.Location = new System.Drawing.Point(142, 4);
            this.buttonFormatCode.Name = "buttonFormatCode";
            this.buttonFormatCode.Size = new System.Drawing.Size(120, 23);
            this.buttonFormatCode.TabIndex = 3;
            this.buttonFormatCode.Text = "Format Code";
            this.buttonFormatCode.UseVisualStyleBackColor = true;
            this.buttonFormatCode.Click += new System.EventHandler(this.buttonFormatCode_Click);
            // 
            // htmlOutput
            // 
            this.htmlOutput.AcceptsReturn = true;
            this.htmlOutput.AcceptsTab = true;
            this.htmlOutput.Location = new System.Drawing.Point(6, 6);
            this.htmlOutput.Multiline = true;
            this.htmlOutput.Name = "htmlOutput";
            this.htmlOutput.Size = new System.Drawing.Size(701, 398);
            this.htmlOutput.TabIndex = 3;
            // 
            // formatPreview
            // 
            this.formatPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formatPreview.Location = new System.Drawing.Point(3, 3);
            this.formatPreview.MinimumSize = new System.Drawing.Size(20, 20);
            this.formatPreview.Name = "formatPreview";
            this.formatPreview.Size = new System.Drawing.Size(707, 404);
            this.formatPreview.TabIndex = 0;
            // 
            // CodeFormatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(743, 459);
            this.Controls.Add(this.tabctrlMain);
            this.Name = "CodeFormatForm";
            this.Text = "Format Code";
            this.Load += new System.EventHandler(this.CodeFormatForm_Load);
            this.tabctrlMain.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPageOutput.ResumeLayout(false);
            this.tabPageOutput.PerformLayout();
            this.tabPagePreview.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabctrlMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPageOutput;
        private System.Windows.Forms.Button buttonFormatCode;
        private System.Windows.Forms.TextBox sourceTextDocument;
        private System.Windows.Forms.ComboBox availableHighlighters;
        private System.Windows.Forms.TextBox htmlOutput;
        private System.Windows.Forms.TabPage tabPagePreview;
        private System.Windows.Forms.WebBrowser formatPreview;
    }
}

