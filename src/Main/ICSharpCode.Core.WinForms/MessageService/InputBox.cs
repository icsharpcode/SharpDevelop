// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	/// <summary>
	/// Class used to display an input box.
	/// </summary>
	sealed class InputBox : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.Button acceptButton;
		
		public InputBox(string text, string caption, string defaultValue)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			text = StringParser.Parse(text);
			this.Text = StringParser.Parse(caption);
			acceptButton.Text = StringParser.Parse("${res:Global.OKButtonText}");
			cancelButton.Text = StringParser.Parse("${res:Global.CancelButtonText}");
			
			Size size;
			using (Graphics g = this.CreateGraphics()) {
				Rectangle screen = Screen.PrimaryScreen.WorkingArea;
				SizeF sizeF = g.MeasureString(text, label.Font, screen.Width - 20);
				size = sizeF.ToSize();
				size.Width += 4;
			}
			if (size.Width < 200)
				size.Width = 200;
			Size clientSize = this.ClientSize;
			clientSize.Width += size.Width - label.Width;
			clientSize.Height += size.Height - label.Height;
			this.ClientSize = clientSize;
			label.Text = text;
			textBox.Text = defaultValue;
			this.DialogResult = DialogResult.Cancel;
			RightToLeftConverter.ConvertRecursive(this);
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.acceptButton = new System.Windows.Forms.Button();
			this.textBox = new System.Windows.Forms.TextBox();
			this.cancelButton = new System.Windows.Forms.Button();
			this.label = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// acceptButton
			// 
			this.acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.acceptButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.acceptButton.Location = new System.Drawing.Point(176, 114);
			this.acceptButton.Name = "acceptButton";
			this.acceptButton.TabIndex = 2;
			this.acceptButton.Text = "OK";
			this.acceptButton.Click += new System.EventHandler(this.AcceptButtonClick);
			// 
			// textBox
			// 
			this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox.Location = new System.Drawing.Point(8, 86);
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(318, 20);
			this.textBox.TabIndex = 1;
			this.textBox.Text = "";
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.cancelButton.Location = new System.Drawing.Point(256, 114);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 3;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
			// 
			// label
			// 
			this.label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
						| System.Windows.Forms.AnchorStyles.Left) 
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label.Location = new System.Drawing.Point(8, 8);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(328, 74);
			this.label.TabIndex = 0;
			this.label.UseMnemonic = false;
			// 
			// InputBox
			// 
			this.AcceptButton = this.acceptButton;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(338, 144);
			this.Controls.Add(this.textBox);
			this.Controls.Add(this.label);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.acceptButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InputBox";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "InputBox";
			this.ResumeLayout(false);
		}
		#endregion
		
		void CancelButtonClick(object sender, System.EventArgs e)
		{
			result = null;
			this.Close();
		}
		
		void AcceptButtonClick(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			result = textBox.Text;
			this.Close();
		}
		
		string result;
		
		public string Result {
			get {
				return result;
			}
		}
	}
}
