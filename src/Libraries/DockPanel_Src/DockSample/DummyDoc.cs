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
        #if FRAMEWORK_VER_2x
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.MenuStrip mainMenu;
		private System.Windows.Forms.ToolStripMenuItem menuItem1;
		private System.Windows.Forms.ToolStripMenuItem menuItem2;
		private System.Windows.Forms.ContextMenuStrip contextMenuTabPage;
		private System.Windows.Forms.ToolStripMenuItem menuItem3;
		private System.Windows.Forms.ToolStripMenuItem menuItem4;
		private System.Windows.Forms.ToolStripMenuItem menuItem5;
		private System.Windows.Forms.ToolStripMenuItem menuItemCheckTest;
		private System.Windows.Forms.ToolTip toolTip;
		private System.ComponentModel.IContainer components;
        #else
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.ContextMenu contextMenuTabPage;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItemCheckTest;
		private System.Windows.Forms.ToolTip toolTip;
		private System.ComponentModel.IContainer components;
		#endif

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

		#if FRAMEWORK_VER_2x
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DummyDoc));
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.mainMenu = new System.Windows.Forms.MenuStrip();
			this.menuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemCheckTest = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuTabPage = new System.Windows.Forms.ContextMenuStrip();
			this.menuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem4 = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// richTextBox1
			// 
			this.richTextBox1.AcceptsTab = true;
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox1.Location = new System.Drawing.Point(0, 4);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(448, 389);
			this.richTextBox1.TabIndex = 0;
			this.richTextBox1.Text = "";
			this.toolTip.SetToolTip(this.richTextBox1, "Test Tooltip");
			// 
			// mainMenu
			//
            this.mainMenu.Visible = false;
			this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
																					 this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
																					  this.menuItem2,
																					  this.menuItemCheckTest});
			this.menuItem1.MergeIndex = 1;
            this.menuItem1.MergeAction = MergeAction.Insert;
			this.menuItem1.Text = "&MDI Document";
			// 
			// menuItem2
			// 
			this.menuItem2.Text = "Test";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// menuItemCheckTest
			// 
			this.menuItemCheckTest.Text = "Check Test";
			this.menuItemCheckTest.Click += new System.EventHandler(this.menuItemCheckTest_Click);
			// 
			// contextMenuTabPage
			// 
			this.contextMenuTabPage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
																							   this.menuItem3,
																							   this.menuItem4,
																							   this.menuItem5});
			// 
			// menuItem3
			// 
			this.menuItem3.Text = "Option &1";
			// 
			// menuItem4
			// 
			this.menuItem4.Text = "Option &2";
			// 
			// menuItem5
			// 
			this.menuItem5.Text = "Option &3";
			// 
			// DummyDoc
			// 
			this.ClientSize = new System.Drawing.Size(448, 393);
			this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.mainMenu);
            this.DockableAreas = ((WeifenLuo.WinFormsUI.DockAreas)(((((WeifenLuo.WinFormsUI.DockAreas.DockLeft | WeifenLuo.WinFormsUI.DockAreas.DockRight) 
				| WeifenLuo.WinFormsUI.DockAreas.DockTop) 
				| WeifenLuo.WinFormsUI.DockAreas.DockBottom) 
				| WeifenLuo.WinFormsUI.DockAreas.Document)));
			this.DockPadding.Top = 4;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.mainMenu;
			this.Name = "DummyDoc";
			this.TabPageContextMenuStrip = this.contextMenuTabPage;
			this.ResumeLayout(false);

		}
		#endregion
		#else
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DummyDoc));
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItemCheckTest = new System.Windows.Forms.MenuItem();
			this.contextMenuTabPage = new System.Windows.Forms.ContextMenu();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// richTextBox1
			// 
			this.richTextBox1.AcceptsTab = true;
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox1.Location = new System.Drawing.Point(0, 4);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(448, 389);
			this.richTextBox1.TabIndex = 0;
			this.richTextBox1.Text = "";
			this.toolTip.SetToolTip(this.richTextBox1, "Test Tooltip");
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
			this.ClientSize = new System.Drawing.Size(448, 393);
			this.Controls.Add(this.richTextBox1);
			this.DockableAreas = ((WeifenLuo.WinFormsUI.DockAreas)(((((WeifenLuo.WinFormsUI.DockAreas.DockLeft | WeifenLuo.WinFormsUI.DockAreas.DockRight | WeifenLuo.WinFormsUI.DockAreas.Float) 
				| WeifenLuo.WinFormsUI.DockAreas.DockTop) 
				| WeifenLuo.WinFormsUI.DockAreas.DockBottom) 
				| WeifenLuo.WinFormsUI.DockAreas.Document)));
			this.DockPadding.Top = 4;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu;
			this.Name = "DummyDoc";
			this.TabPageContextMenu = this.contextMenuTabPage;
			this.ResumeLayout(false);

		}
		#endregion
		#endif

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
			MessageBox.Show("This is to demostrate menu item has been successfully merged into the main form. Form Text=" + Text);
		}

		private void menuItemCheckTest_Click(object sender, System.EventArgs e)
		{
			menuItemCheckTest.Checked = !menuItemCheckTest.Checked;
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged (e);
			if (FileName == string.Empty)
				this.richTextBox1.Text = Text;
		}
	}
}
