using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;
using System.IO;

namespace DockSample
{
	/// <summary>
	/// Summary description for DummyDoc.
	/// </summary>
	public class DummyDoc : DockContent
	{
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.ContextMenu contextMenuTabPage;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItemCheckTest;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DummyDoc()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItemCheckTest = new System.Windows.Forms.MenuItem();
			this.contextMenuTabPage = new System.Windows.Forms.ContextMenu();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// richTextBox1
			// 
			this.richTextBox1.AcceptsTab = true;
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox1.Location = new System.Drawing.Point(0, 0);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(264, 337);
			this.richTextBox1.TabIndex = 0;
			this.richTextBox1.Text = "";
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem2,
																					  this.menuItemCheckTest});
			this.menuItem1.MergeOrder = 1;
			this.menuItem1.Text = "Rtf File";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Text = "Test";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// menuItemCheckTest
			// 
			this.menuItemCheckTest.Index = 1;
			this.menuItemCheckTest.Text = "Check Test";
			this.menuItemCheckTest.Click += new System.EventHandler(this.menuItemCheckTest_Click);
			// 
			// contextMenuTabPage
			// 
			this.contextMenuTabPage.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							   this.menuItem3,
																							   this.menuItem4,
																							   this.menuItem5});
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 0;
			this.menuItem3.Text = "Option &1";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 1;
			this.menuItem4.Text = "Option &2";
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 2;
			this.menuItem5.Text = "Option &3";
			// 
			// DummyDoc
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(264, 337);
			this.Controls.Add(this.richTextBox1);
			this.Menu = this.mainMenu;
			this.Name = "DummyDoc";
			this.TabPageContextMenu = this.contextMenuTabPage;
			this.ResumeLayout(false);

		}
		#endregion

		private string m_fileName = string.Empty;
		public string FileName
		{
			get	{	return m_fileName;	}
			set
			{
				if (value != string.Empty)
				{
					Stream s = new FileStream(value, FileMode.Open);

					FileInfo efInfo = new FileInfo(value);

					string fext = efInfo.Extension.ToUpper();

					if (fext.Equals(".RTF"))
						richTextBox1.LoadFile(s, RichTextBoxStreamType.RichText);
					else
						richTextBox1.LoadFile(s, RichTextBoxStreamType.PlainText);
					s.Close();
				}

				m_fileName = value;
				this.ToolTipText = value;
			}
		}

		// workaround of RichTextbox control's bug:
		// If load file before the control showed, all the text format will be lost
		// re-load the file after it get showed.
		private bool m_resetText = true;
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (m_resetText)
			{
				m_resetText = false;
				FileName = FileName;
			}
		}

		protected override string GetPersistString()
		{
			return GetType().ToString() + "," + FileName + "," + Text;
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			MessageBox.Show("This is to demostrate menu item has been successfully merged into the main form.");
		}

		private void menuItemCheckTest_Click(object sender, System.EventArgs e)
		{
			menuItemCheckTest.Checked = !menuItemCheckTest.Checked;
		}
	}
}
