// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class UserCredentialsDialog : System.Windows.Forms.Form
	{
		string authenticationType = String.Empty;
		
		public UserCredentialsDialog(string url, string authenticationType)
		{
			InitializeComponent();
			this.url.Text = url;
			this.authenticationType = authenticationType;
			AddStringResources();
		}
		
		public DiscoveryNetworkCredential Credential {
			get {
				return new DiscoveryNetworkCredential(userTextBox.Text, passwordTextBox.Text, domainTextBox.Text, authenticationType);
			}
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.urlLabel = new System.Windows.Forms.Label();
			this.userNameLabel = new System.Windows.Forms.Label();
			this.passwordLabel = new System.Windows.Forms.Label();
			this.domainLabel = new System.Windows.Forms.Label();
			this.userTextBox = new System.Windows.Forms.TextBox();
			this.passwordTextBox = new System.Windows.Forms.TextBox();
			this.domainTextBox = new System.Windows.Forms.TextBox();
			this.url = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.infoLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// urlLabel
			// 
			this.urlLabel.Location = new System.Drawing.Point(10, 59);
			this.urlLabel.Name = "urlLabel";
			this.urlLabel.Size = new System.Drawing.Size(91, 23);
			this.urlLabel.TabIndex = 0;
			this.urlLabel.Text = "Url:";
			this.urlLabel.UseCompatibleTextRendering = true;
			// 
			// userNameLabel
			// 
			this.userNameLabel.Location = new System.Drawing.Point(10, 88);
			this.userNameLabel.Name = "userNameLabel";
			this.userNameLabel.Size = new System.Drawing.Size(91, 23);
			this.userNameLabel.TabIndex = 1;
			this.userNameLabel.Text = "&User name:";
			this.userNameLabel.UseCompatibleTextRendering = true;
			// 
			// passwordLabel
			// 
			this.passwordLabel.Location = new System.Drawing.Point(10, 115);
			this.passwordLabel.Name = "passwordLabel";
			this.passwordLabel.Size = new System.Drawing.Size(91, 23);
			this.passwordLabel.TabIndex = 3;
			this.passwordLabel.Text = "&Password:";
			this.passwordLabel.UseCompatibleTextRendering = true;
			// 
			// domainLabel
			// 
			this.domainLabel.Location = new System.Drawing.Point(10, 142);
			this.domainLabel.Name = "domainLabel";
			this.domainLabel.Size = new System.Drawing.Size(91, 23);
			this.domainLabel.TabIndex = 5;
			this.domainLabel.Text = "&Domain:";
			this.domainLabel.UseCompatibleTextRendering = true;
			// 
			// userTextBox
			// 
			this.userTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.userTextBox.Location = new System.Drawing.Point(93, 85);
			this.userTextBox.Name = "userTextBox";
			this.userTextBox.Size = new System.Drawing.Size(187, 21);
			this.userTextBox.TabIndex = 2;
			// 
			// passwordTextBox
			// 
			this.passwordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.passwordTextBox.Location = new System.Drawing.Point(93, 112);
			this.passwordTextBox.Name = "passwordTextBox";
			this.passwordTextBox.PasswordChar = '*';
			this.passwordTextBox.Size = new System.Drawing.Size(187, 21);
			this.passwordTextBox.TabIndex = 4;
			// 
			// domainTextBox
			// 
			this.domainTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.domainTextBox.Location = new System.Drawing.Point(93, 139);
			this.domainTextBox.Name = "domainTextBox";
			this.domainTextBox.Size = new System.Drawing.Size(187, 21);
			this.domainTextBox.TabIndex = 6;
			// 
			// url
			// 
			this.url.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.url.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.url.Location = new System.Drawing.Point(93, 57);
			this.url.Name = "url";
			this.url.Size = new System.Drawing.Size(187, 21);
			this.url.TabIndex = 9;
			this.url.UseCompatibleTextRendering = true;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(146, 166);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(64, 26);
			this.okButton.TabIndex = 7;
			this.okButton.Text = "OK";
			this.okButton.UseCompatibleTextRendering = true;
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(216, 166);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(64, 26);
			this.cancelButton.TabIndex = 8;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseCompatibleTextRendering = true;
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// infoLabel
			// 
			this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.infoLabel.Location = new System.Drawing.Point(12, 9);
			this.infoLabel.Name = "infoLabel";
			this.infoLabel.Size = new System.Drawing.Size(267, 48);
			this.infoLabel.TabIndex = 10;
			this.infoLabel.Text = "Please supply the credentials to access the specified url.";
			this.infoLabel.UseCompatibleTextRendering = true;
			// 
			// UserCredentialsDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(292, 202);
			this.Controls.Add(this.infoLabel);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.url);
			this.Controls.Add(this.domainTextBox);
			this.Controls.Add(this.passwordTextBox);
			this.Controls.Add(this.userTextBox);
			this.Controls.Add(this.domainLabel);
			this.Controls.Add(this.passwordLabel);
			this.Controls.Add(this.userNameLabel);
			this.Controls.Add(this.urlLabel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(300, 236);
			this.Name = "UserCredentialsDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Discovery Credential";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Label infoLabel;
		private System.Windows.Forms.TextBox passwordTextBox;
		private System.Windows.Forms.Label userNameLabel;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Label url;
		private System.Windows.Forms.TextBox domainTextBox;
		private System.Windows.Forms.TextBox userTextBox;
		private System.Windows.Forms.Label domainLabel;
		private System.Windows.Forms.Label passwordLabel;
		private System.Windows.Forms.Label urlLabel;
		#endregion
	
		void AddStringResources()
		{
			Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.UserCredentialsDialog.DialogTitle}");
			infoLabel.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.UserCredentialsDialog.InformationLabel}");
			urlLabel.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.UserCredentialsDialog.UrlLabel}");
			userNameLabel.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.UserCredentialsDialog.UserNameLabel}");
			passwordLabel.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.UserCredentialsDialog.PasswordLabel}");
			domainLabel.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.UserCredentialsDialog.DomainLabel}");
			cancelButton.Text = StringParser.Parse("${res:Global.CancelButtonText}");
			okButton.Text = StringParser.Parse("${res:Global.OKButtonText}");
		}
	}
}
