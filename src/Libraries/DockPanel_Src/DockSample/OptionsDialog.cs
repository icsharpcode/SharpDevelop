using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace DockSample
{
	/// <summary>
	/// Summary description for Options.
	/// </summary>
	public class OptionsDialog: System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox ActiveDocumentChanged;
		private System.Windows.Forms.CheckBox ContentAdded;
		private System.Windows.Forms.CheckBox ContentRemoved;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;

		private Options m_options = null;

		public OptionsDialog(Options options)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			m_options = options;

			ActiveDocumentChanged.Checked = m_options.ActiveDocumentChanged;
			ContentAdded.Checked = m_options.ContentAdded;
			ContentRemoved.Checked = m_options.ContentRemoved;
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.ContentRemoved = new System.Windows.Forms.CheckBox();
			this.ContentAdded = new System.Windows.Forms.CheckBox();
			this.ActiveDocumentChanged = new System.Windows.Forms.CheckBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.ContentRemoved);
			this.groupBox1.Controls.Add(this.ContentAdded);
			this.groupBox1.Controls.Add(this.ActiveDocumentChanged);
			this.groupBox1.Location = new System.Drawing.Point(24, 24);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(264, 112);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Monitor &Events";
			// 
			// ContentRemoved
			// 
			this.ContentRemoved.Location = new System.Drawing.Point(24, 72);
			this.ContentRemoved.Name = "ContentRemoved";
			this.ContentRemoved.Size = new System.Drawing.Size(224, 24);
			this.ContentRemoved.TabIndex = 2;
			this.ContentRemoved.Text = "&3. ContentRemoved";
			// 
			// ContentAdded
			// 
			this.ContentAdded.Location = new System.Drawing.Point(24, 48);
			this.ContentAdded.Name = "ContentAdded";
			this.ContentAdded.Size = new System.Drawing.Size(224, 24);
			this.ContentAdded.TabIndex = 1;
			this.ContentAdded.Text = "&2. ContentAdded";
			// 
			// ActiveDocumentChanged
			// 
			this.ActiveDocumentChanged.Location = new System.Drawing.Point(24, 24);
			this.ActiveDocumentChanged.Name = "ActiveDocumentChanged";
			this.ActiveDocumentChanged.Size = new System.Drawing.Size(224, 24);
			this.ActiveDocumentChanged.TabIndex = 0;
			this.ActiveDocumentChanged.Text = "&1. ActiveDocumentChanged";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(112, 160);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "&OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(208, 160);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "&Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// OptionsDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(322, 215);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OptionsDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Options";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			m_options.ActiveDocumentChanged = ActiveDocumentChanged.Checked;
			m_options.ContentAdded = ContentAdded.Checked;
			m_options.ContentRemoved = ContentRemoved.Checked;

			Close();
		}
	}
}
