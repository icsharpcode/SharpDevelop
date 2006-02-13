using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace DockSample
{
	/// <summary>
	/// Summary description for About.
	/// </summary>
	public class AboutDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AboutDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			Font = SystemInformation.MenuFont;
			
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonOK.Location = new System.Drawing.Point(240, 184);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 88);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(272, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "DockSample, Version 1.4";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 120);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(272, 32);
			this.label2.TabIndex = 2;
			this.label2.Text = "Copyright 2003 - 2006, Weifen Luo";
			// 
			// AboutDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.CancelButton = this.buttonOK;
			this.ClientSize = new System.Drawing.Size(322, 215);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
