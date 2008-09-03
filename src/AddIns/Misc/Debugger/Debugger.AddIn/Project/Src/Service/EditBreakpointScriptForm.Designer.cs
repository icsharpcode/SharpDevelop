/*
 * Created by SharpDevelop.
 * User: HP
 * Date: 25.08.2008
 * Time: 09:37
 */
namespace Debugger.AddIn.Service
{
	partial class EditBreakpointScriptForm
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
			this.txtCode = new ICSharpCode.TextEditor.TextEditorControl();
			this.cmbLanguage = new System.Windows.Forms.ComboBox();
			this.btnCheckSyntax = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtCode
			// 
			this.txtCode.IsReadOnly = false;
			this.txtCode.Location = new System.Drawing.Point(3, 31);
			this.txtCode.Name = "txtCode";
			this.txtCode.Size = new System.Drawing.Size(575, 355);
			this.txtCode.TabIndex = 0;
			// 
			// cmbLanguage
			// 
			this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbLanguage.FormattingEnabled = true;
			this.cmbLanguage.Location = new System.Drawing.Point(153, 4);
			this.cmbLanguage.Name = "cmbLanguage";
			this.cmbLanguage.Size = new System.Drawing.Size(121, 21);
			this.cmbLanguage.TabIndex = 1;
			this.cmbLanguage.SelectedIndexChanged += new System.EventHandler(this.CmbLanguageSelectedIndexChanged);
			// 
			// btnCheckSyntax
			// 
			this.btnCheckSyntax.Location = new System.Drawing.Point(280, 2);
			this.btnCheckSyntax.Name = "btnCheckSyntax";
			this.btnCheckSyntax.Size = new System.Drawing.Size(119, 22);
			this.btnCheckSyntax.TabIndex = 2;
			this.btnCheckSyntax.Text = "Check Syntax";
			this.btnCheckSyntax.UseVisualStyleBackColor = true;
			this.btnCheckSyntax.Click += new System.EventHandler(this.BtnCheckSyntaxClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(144, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = "Scripting Language:";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(415, 390);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.BtnOKClick);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(496, 390);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// EditBreakpointScriptForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(583, 418);
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnCheckSyntax);
			this.Controls.Add(this.cmbLanguage);
			this.Controls.Add(this.txtCode);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EditBreakpointScriptForm";
			this.ShowIcon = false;
			this.Text = "Edit Debugger Script";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnCheckSyntax;
		private System.Windows.Forms.ComboBox cmbLanguage;
		private ICSharpCode.TextEditor.TextEditorControl txtCode;
	}
}
