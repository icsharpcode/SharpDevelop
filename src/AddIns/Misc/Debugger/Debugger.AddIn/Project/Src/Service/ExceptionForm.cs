// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using Debugger;

namespace ICSharpCode.SharpDevelop.Services
{
	public class ExceptionForm : System.Windows.Forms.Form
	{
		public enum Result {Break, Continue, Ignore};

		private Result result = Result.Continue;

		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.Button buttonContinue;
		private System.Windows.Forms.Button buttonIgnore;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.Button buttonBreak;


		private ExceptionForm()
		{
			InitializeComponent();
			this.Text = StringParser.Parse(this.Text);
			buttonContinue.Text = StringParser.Parse(buttonContinue.Text);
			buttonIgnore.Text = StringParser.Parse(buttonIgnore.Text);
			buttonBreak.Text = StringParser.Parse(buttonBreak.Text);
		}
		
		public static Result Show(Debugger.Exception exception)
		{
			using (ExceptionForm form = new ExceptionForm()) {
				form.textBox.Text =
					ResourceService.GetString("MainWindow.Windows.Debug.ExceptionForm.Message").Replace("{0}", exception.Type) + "\r\n" +
					exception.Message + "\r\n\r\n" +
					exception.Callstack.Replace("\n","\r\n");
				form.pictureBox.Image = ResourceService.GetBitmap((exception.ExceptionType != ExceptionType.DEBUG_EXCEPTION_UNHANDLED)?"Icons.32x32.Warning":"Icons.32x32.Error");
				form.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
				return form.result;
			}
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			this.buttonBreak = new System.Windows.Forms.Button();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.buttonIgnore = new System.Windows.Forms.Button();
			this.buttonContinue = new System.Windows.Forms.Button();
			this.textBox = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonBreak
			// 
			this.buttonBreak.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonBreak.Location = new System.Drawing.Point(176, 160);
			this.buttonBreak.Name = "buttonBreak";
			this.buttonBreak.Size = new System.Drawing.Size(91, 32);
			this.buttonBreak.TabIndex = 0;
			this.buttonBreak.Text = "${res:XML.MainMenu.DebugMenu.Break}";
			this.buttonBreak.Click += new System.EventHandler(this.buttonBreak_Click);
			// 
			// pictureBox
			// 
			this.pictureBox.Location = new System.Drawing.Point(14, 16);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(56, 64);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox.TabIndex = 3;
			this.pictureBox.TabStop = false;
			// 
			// buttonIgnore
			// 
			this.buttonIgnore.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonIgnore.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonIgnore.Location = new System.Drawing.Point(372, 160);
			this.buttonIgnore.Name = "buttonIgnore";
			this.buttonIgnore.Size = new System.Drawing.Size(91, 32);
			this.buttonIgnore.TabIndex = 2;
			this.buttonIgnore.Text = "${res:Global.IgnoreButtonText}";
			this.buttonIgnore.Click += new System.EventHandler(this.buttonIgnore_Click);
			// 
			// buttonContinue
			// 
			this.buttonContinue.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonContinue.Location = new System.Drawing.Point(274, 160);
			this.buttonContinue.Name = "buttonContinue";
			this.buttonContinue.Size = new System.Drawing.Size(91, 32);
			this.buttonContinue.TabIndex = 1;
			this.buttonContinue.Text = "${res:ICSharpCode.SharpDevelop.ExceptionBox.Continue}";
			this.buttonContinue.Click += new System.EventHandler(this.buttonContinue_Click);
			// 
			// textBox
			// 
			this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			                                                             | System.Windows.Forms.AnchorStyles.Left)
			                                                            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox.Location = new System.Drawing.Point(91, 16);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.ReadOnly = true;
			this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox.Size = new System.Drawing.Size(528, 138);
			this.textBox.TabIndex = 4;
			this.textBox.WordWrap = false;
			// 
			// ExceptionForm
			// 
			this.CancelButton = this.buttonIgnore;
			this.ClientSize = new System.Drawing.Size(638, 203);
			this.Controls.Add(this.textBox);
			this.Controls.Add(this.pictureBox);
			this.Controls.Add(this.buttonIgnore);
			this.Controls.Add(this.buttonContinue);
			this.Controls.Add(this.buttonBreak);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ExceptionForm";
			this.ShowInTaskbar = false;
			this.Text = "${res:MainWindow.Windows.Debug.ExceptionHistory.Exception}";
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion

		private void buttonBreak_Click(object sender, System.EventArgs e)
		{
			result = Result.Break;
			Close();
		}

		private void buttonContinue_Click(object sender, System.EventArgs e)
		{
			result = Result.Continue;
			Close();
		}

		private void buttonIgnore_Click(object sender, System.EventArgs e)
		{
			result = Result.Ignore;
			Close();
		}
	}
}
