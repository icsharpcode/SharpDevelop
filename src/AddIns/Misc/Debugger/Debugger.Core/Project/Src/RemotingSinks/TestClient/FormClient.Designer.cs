namespace CustomSinks
{
	partial class FormClient
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
			this.buttonSend = new System.Windows.Forms.Button();
			this.textBoxInput = new System.Windows.Forms.TextBox();
			this.richTextBoxChat = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// buttonSend
			// 
			this.buttonSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSend.Location = new System.Drawing.Point(184, 158);
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Size = new System.Drawing.Size(75, 23);
			this.buttonSend.TabIndex = 0;
			this.buttonSend.Text = "Send";
			this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
			// 
			// textBoxInput
			// 
			this.textBoxInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxInput.Location = new System.Drawing.Point(2, 158);
			this.textBoxInput.Name = "textBoxInput";
			this.textBoxInput.Size = new System.Drawing.Size(176, 26);
			this.textBoxInput.TabIndex = 1;
			// 
			// richTextBoxChat
			// 
			this.richTextBoxChat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.richTextBoxChat.Location = new System.Drawing.Point(2, 2);
			this.richTextBoxChat.Name = "richTextBoxChat";
			this.richTextBoxChat.Size = new System.Drawing.Size(257, 150);
			this.richTextBoxChat.TabIndex = 2;
			this.richTextBoxChat.Text = "";
			// 
			// FormClient
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(262, 188);
			this.Controls.Add(this.richTextBoxChat);
			this.Controls.Add(this.textBoxInput);
			this.Controls.Add(this.buttonSend);
			this.Name = "FormClient";
			this.Text = "Client";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonSend;
		private System.Windows.Forms.TextBox textBoxInput;
		private System.Windows.Forms.RichTextBox richTextBoxChat;
	}
}

