namespace CodeConvertServiceClient
{
    partial class CodeConversionForm
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
            this.typeOfConversion = new System.Windows.Forms.ComboBox();
            this.performConversion = new System.Windows.Forms.Button();
            this.inputSource = new System.Windows.Forms.TextBox();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // typeOfConversion
            // 
            this.typeOfConversion.FormattingEnabled = true;
            this.typeOfConversion.Items.AddRange(new object[] {
            "C# to VB.NET",
            "VB.NET to C#",
            "C# to Boo",
            "VB.NET to Boo"});
            this.typeOfConversion.Location = new System.Drawing.Point(376, 19);
            this.typeOfConversion.Name = "typeOfConversion";
            this.typeOfConversion.Size = new System.Drawing.Size(121, 21);
            this.typeOfConversion.TabIndex = 1;
            // 
            // performConversion
            // 
            this.performConversion.Location = new System.Drawing.Point(204, 303);
            this.performConversion.Name = "performConversion";
            this.performConversion.Size = new System.Drawing.Size(166, 23);
            this.performConversion.TabIndex = 4;
            this.performConversion.Text = "Perform Conversion";
            this.performConversion.UseVisualStyleBackColor = true;
            this.performConversion.Click += new System.EventHandler(this.performConversion_Click);
            // 
            // inputSource
            // 
            this.inputSource.Location = new System.Drawing.Point(16, 82);
            this.inputSource.Multiline = true;
            this.inputSource.Name = "inputSource";
            this.inputSource.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.inputSource.Size = new System.Drawing.Size(709, 211);
            this.inputSource.TabIndex = 3;
            // 
            // outputTextBox
            // 
            this.outputTextBox.Location = new System.Drawing.Point(16, 366);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.outputTextBox.Size = new System.Drawing.Size(709, 211);
            this.outputTextBox.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(354, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Step 1: Choose source language and destination language for conversion";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(327, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Step 2: Paste the source code (entire class[es]) you want to convert";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 340);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Step 4: Output";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 303);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(185, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Step 3: Perform the actual conversion";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(373, 57);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(132, 13);
            this.linkLabel1.TabIndex = 8;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Online Snippet Conversion";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // CodeConversionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 592);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.inputSource);
            this.Controls.Add(this.performConversion);
            this.Controls.Add(this.typeOfConversion);
            this.Name = "CodeConversionForm";
            this.Text = "Code Converter";
            this.Load += new System.EventHandler(this.CodeConversionForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox typeOfConversion;
        private System.Windows.Forms.Button performConversion;
        private System.Windows.Forms.TextBox inputSource;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}

