// Copyright (c) 2005 Daniel Grunwald
// Licensed under the terms of the "BSD License", see doc/license.txt

using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace Base
{
	public class InfoForm : System.Windows.Forms.Form
	{
		public InfoForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			FormLocationHelper.Apply(this, "InfoForm", false);
			
			versionLabel.Text = typeof(AddInTree).Assembly.GetName().Version.ToString();
			demoVersionLabel.Text = Assembly.GetEntryAssembly().GetName().Version.ToString();
			listBox.UseCustomTabOffsets = true;
			listBox.CustomTabOffsets.Add(100);
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
				AssemblyName name = asm.GetName();
				listBox.Items.Add(name.Name + "\t" + name.Version.ToString());
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
			System.Windows.Forms.LinkLabel linkLabel1;
			System.Windows.Forms.LinkLabel linkLabel2;
			System.Windows.Forms.LinkLabel linkLabel3;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Button okButton;
			System.Windows.Forms.Label label4;
			this.listBox = new System.Windows.Forms.ListBox();
			this.versionLabel = new System.Windows.Forms.Label();
			this.demoVersionLabel = new System.Windows.Forms.Label();
			linkLabel1 = new System.Windows.Forms.LinkLabel();
			linkLabel2 = new System.Windows.Forms.LinkLabel();
			linkLabel3 = new System.Windows.Forms.LinkLabel();
			label1 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			okButton = new System.Windows.Forms.Button();
			label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// linkLabel1
			// 
			linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			                                                          | System.Windows.Forms.AnchorStyles.Right)));
			linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(43, 12);
			linkLabel1.Location = new System.Drawing.Point(12, 9);
			linkLabel1.Name = "linkLabel1";
			linkLabel1.Size = new System.Drawing.Size(368, 45);
			linkLabel1.TabIndex = 1;
			linkLabel1.TabStop = true;
			linkLabel1.Tag = "http://sharpdevelop.net/";
			linkLabel1.Text = "Demo application for ICSharpCode.Core, the SharpDevelop add-in architecture.";
			linkLabel1.UseCompatibleTextRendering = true;
			linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelLinkClicked);
			// 
			// linkLabel2
			// 
			linkLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			                                                          | System.Windows.Forms.AnchorStyles.Right)));
			linkLabel2.LinkArea = new System.Windows.Forms.LinkArea(16, 15);
			linkLabel2.Location = new System.Drawing.Point(12, 54);
			linkLabel2.Name = "linkLabel2";
			linkLabel2.Size = new System.Drawing.Size(368, 28);
			linkLabel2.TabIndex = 2;
			linkLabel2.TabStop = true;
			linkLabel2.Tag = "mailto:daniel@danielgrunwald.de";
			linkLabel2.Text = "Demo written by Daniel Grunwald on the 22nd December, 2005.";
			linkLabel2.UseCompatibleTextRendering = true;
			linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelLinkClicked);
			// 
			// linkLabel3
			// 
			linkLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			                                                          | System.Windows.Forms.AnchorStyles.Right)));
			linkLabel3.LinkArea = new System.Windows.Forms.LinkArea(81, 17);
			linkLabel3.Location = new System.Drawing.Point(12, 82);
			linkLabel3.Name = "linkLabel3";
			linkLabel3.Size = new System.Drawing.Size(368, 42);
			linkLabel3.TabIndex = 3;
			linkLabel3.TabStop = true;
			linkLabel3.Tag = "http://wiki.sharpdevelop.net/default.aspx/SharpDevelop.Contributors";
			linkLabel3.Text = "ICSharpCode.Core was written by Mike Krüger, Daniel Grunwald and the rest of the " +
				"SharpDevelop team.";
			linkLabel3.UseCompatibleTextRendering = true;
			linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelLinkClicked);
			// 
			// label1
			// 
			label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
			                                                      | System.Windows.Forms.AnchorStyles.Right)));
			label1.Location = new System.Drawing.Point(12, 172);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(368, 23);
			label1.TabIndex = 6;
			label1.Tag = "";
			label1.Text = "List of loaded assemblies:";
			label1.UseCompatibleTextRendering = true;
			// 
			// label3
			// 
			label3.Location = new System.Drawing.Point(35, 124);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(144, 23);
			label3.TabIndex = 4;
			label3.Text = "ICSharpCode.Core version:";
			label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			label3.UseCompatibleTextRendering = true;
			// 
			// okButton
			// 
			okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			okButton.Location = new System.Drawing.Point(305, 302);
			okButton.Name = "okButton";
			okButton.Size = new System.Drawing.Size(75, 23);
			okButton.TabIndex = 0;
			okButton.Text = "OK";
			okButton.UseCompatibleTextRendering = true;
			okButton.UseVisualStyleBackColor = true;
			okButton.Click += new System.EventHandler(this.OkButtonClick);
			// 
			// listBox
			// 
			this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
			                                                            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBox.FormattingEnabled = true;
			this.listBox.Location = new System.Drawing.Point(12, 188);
			this.listBox.Name = "listBox";
			this.listBox.Size = new System.Drawing.Size(368, 108);
			this.listBox.TabIndex = 7;
			// 
			// versionLabel
			// 
			this.versionLabel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
			this.versionLabel.Location = new System.Drawing.Point(185, 124);
			this.versionLabel.Name = "versionLabel";
			this.versionLabel.Size = new System.Drawing.Size(112, 23);
			this.versionLabel.TabIndex = 5;
			this.versionLabel.Text = "#.#.#.#";
			this.versionLabel.UseCompatibleTextRendering = true;
			// 
			// demoVersionLabel
			// 
			this.demoVersionLabel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
			this.demoVersionLabel.Location = new System.Drawing.Point(185, 147);
			this.demoVersionLabel.Name = "demoVersionLabel";
			this.demoVersionLabel.Size = new System.Drawing.Size(112, 23);
			this.demoVersionLabel.TabIndex = 9;
			this.demoVersionLabel.Text = "#.#.#.#";
			this.demoVersionLabel.UseCompatibleTextRendering = true;
			// 
			// label4
			// 
			label4.Location = new System.Drawing.Point(35, 147);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(144, 23);
			label4.TabIndex = 8;
			label4.Text = "Demo application version:";
			label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			label4.UseCompatibleTextRendering = true;
			// 
			// InfoForm
			// 
			this.AcceptButton = okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(392, 330);
			this.Controls.Add(this.demoVersionLabel);
			this.Controls.Add(label4);
			this.Controls.Add(okButton);
			this.Controls.Add(this.versionLabel);
			this.Controls.Add(label3);
			this.Controls.Add(this.listBox);
			this.Controls.Add(label1);
			this.Controls.Add(linkLabel3);
			this.Controls.Add(linkLabel2);
			this.Controls.Add(linkLabel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InfoForm";
			this.Text = "About ICSharpCode.Core.Demo";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Label demoVersionLabel;
		private System.Windows.Forms.ListBox listBox;
		private System.Windows.Forms.Label versionLabel;
		#endregion
		
		void LinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string url = (sender as Control).Tag.ToString();
			try {
				System.Diagnostics.Process.Start(url);
			} catch (Exception) {
				MessageService.ShowMessage(url);
			}
		}
		
		void OkButtonClick(object sender, EventArgs e)
		{
			Close();
		}
	}
	
	public class InfoCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			using (InfoForm frm = new InfoForm()) {
				frm.ShowDialog(this.Owner as IWin32Window);
			}
		}
	}
}
