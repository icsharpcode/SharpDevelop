// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using NoGoop.Controls;
using NoGoop.ObjBrowser.Dialogs;
using NoGoop.ObjBrowser.Panels;
using NoGoop.Util;

namespace NoGoop.ObjBrowser
{
	public class ObjectBrowserForm : Form
	{
		ObjectBrowser                  _objectBrowser;
		static CustomizeDialog         _customizeForm;
		static AttachDialog            _attachForm;
		static TracingDialog           _tracingForm;
		static ObjectBrowserForm       _instance;
		
		public ObjectBrowserForm()
		{
			_instance = this;
			
			using (SplashDialog splash = new SplashDialog()) {
				splash.SplashProductName = ".NET Component Inspector";
				splash.Show();
				Application.DoEvents();
				InitializeComponent();
				
				new System.Resources.ResourceManager(typeof(ObjectBrowser));
			}

			Show();
			AssemblySupport.AddCurrentAssemblies();
		}
		
		public static ObjectBrowserForm Instance {
			get {
				return _instance;
			}
		}
				
		void CreateMenus()
		{
			ToolStripMenuItem open = new ToolStripMenuItem();
			open.ShortcutKeys = Keys.Control | Keys.O;
			//open.Text = "&Open...";
			open.Text = StringParser.Parse("${res:XML.MainMenu.FileMenu.Open}");
			open.Click += new System.EventHandler(OpenClick);
			ToolStripMenuItem close = new ToolStripMenuItem();
			close.Text = StringParser.Parse("${res:Global.CloseButtonText}");
			close.Enabled = false;
			close.Click += new EventHandler(CloseClick);

			ToolStripMenuItem exit = new ToolStripMenuItem();
			exit.Text = StringParser.Parse("${res:XML.MainMenu.FileMenu.Exit}");
			exit.Click += new System.EventHandler(ExitClick);

			ToolStripMenuItem fileMenuItem = new ToolStripMenuItem();
			fileMenuItem.DropDownItems.AddRange(new ToolStripMenuItem[] {open, close, exit});
			fileMenuItem.Text = StringParser.Parse("${res:XML.MainMenu.FileMenu}");

			ToolStripMenuItem viewMenuItem = new ToolStripMenuItem();
			ToolStripMenuItem mi = new ToolStripMenuItem();
			mi.Text = StringParser.Parse("${res:ComponentInspector.ObjectBrowserForm.ObjectTreeMenuItem}");
			mi.Click += new System.EventHandler(CustObjectClick);
			viewMenuItem.DropDownItems.Add(mi);

			mi = new ToolStripMenuItem();
			mi.Text = StringParser.Parse("${res:ComponentInspector.ObjectBrowserForm.TypeHandlersMenuItem}");
			mi.Click += new System.EventHandler(CustTypeHandlerClick);
			viewMenuItem.DropDownItems.Add(mi);

			mi = new ToolStripMenuItem();
			mi.Text = StringParser.Parse("${res:ComponentInspector.ObjectBrowserForm.PanelsMenuItem}");
			mi.Click += new System.EventHandler(CustPanelClick);
			viewMenuItem.DropDownItems.Add(mi);

			mi = new ToolStripMenuItem();
			mi.Text = StringParser.Parse("${res:ComponentInspector.ObjectBrowserForm.DirectoriesMenuItem}");
			mi.Click += new System.EventHandler(CustDirClick);
			viewMenuItem.DropDownItems.Add(mi);

			mi = new ToolStripMenuItem();
			mi.Text = "&ActiveX/COM...";
			mi.Click += new System.EventHandler(CustActiveXClick);
			viewMenuItem.DropDownItems.Add(mi);

			// Create the customization form
			_customizeForm = new CustomizeDialog();

			viewMenuItem.Text = StringParser.Parse("${res:ComponentInspector.ObjectBrowserForm.OptionsMenuItem}");

			ToolStripMenuItem actionMenu = new ToolStripMenuItem();
			actionMenu.Text = StringParser.Parse("${res:ComponentInspector.ObjectBrowserForm.ActionMenuItem}");
			ActionMenuHelper actionMenuHelper = new ActionMenuHelper(close);
			actionMenuHelper.BuildActionMenu(actionMenu);
			ActionMenuHelper.Helper = actionMenuHelper;

			ToolStripMenuItem editMenu = new ToolStripMenuItem();
			editMenu.Text = StringParser.Parse("${res:XML.MainMenu.EditMenu}");
			actionMenuHelper.BuildEditMenu(editMenu);
			editMenu.DropDownItems.Add("-");

			// Create the find form
			mi = new ToolStripMenuItem();
			mi.Text = StringParser.Parse("${res:ComponentInspector.InspectorMenu.Find}");
			mi.ShortcutKeys = Keys.Control | Keys.F;
			mi.Click += new System.EventHandler(FindClick);
			editMenu.DropDownItems.Add(mi);

			// Create the attach form
			_attachForm = new AttachDialog();

			ToolStripMenuItem remoteMenuItem = new ToolStripMenuItem();
			remoteMenuItem.Text = "&Development";
			mi = new ToolStripMenuItem();
			mi.Text = StringParser.Parse("${res:ComponentInspector.ObjectBrowserForm.AttachMenuItem}");
			mi.Click += new System.EventHandler(AttachClick);
			remoteMenuItem.DropDownItems.Add(mi);

			// Create the attach form
			_tracingForm = new TracingDialog();

			mi = new ToolStripMenuItem();
			mi.Text = StringParser.Parse("${res:ComponentInspector.ObjectBrowserForm.TracingMenuItem}");
			mi.Click += new System.EventHandler(TracingClick);
			remoteMenuItem.DropDownItems.Add(mi);

			ToolStripMenuItem about = new ToolStripMenuItem();
			about.Text = StringParser.Parse("${res:XML.MainMenu.HelpMenu.About}");
			about.Click += new System.EventHandler(AboutClick);

			ToolStripMenuItem help = new ToolStripMenuItem();
			help.Text = StringParser.Parse("${res:ComponentInspector.ObjectBrowserForm.ShowHelpMenuItem}");
			help.Click += new System.EventHandler(HelpClick);

			ToolStripMenuItem helpMenuItem = new ToolStripMenuItem();
			helpMenuItem.DropDownItems.AddRange(new ToolStripMenuItem[] {help, about});
			helpMenuItem.Text = StringParser.Parse("${res:XML.MainMenu.HelpMenu}");

			MainMenuStrip = new MenuStrip();
			Controls.Add(MainMenuStrip);
			MainMenuStrip.Items.Add(fileMenuItem);
			MainMenuStrip.Items.Add(editMenu);
			MainMenuStrip.Items.Add(actionMenu);
			MainMenuStrip.Items.Add(viewMenuItem);
			if (LocalPrefs.Get(LocalPrefs.DEV_MENU) != null) {
				MainMenuStrip.Items.Add(remoteMenuItem);
			}
			MainMenuStrip.Items.Add(helpMenuItem);
		}
		
		void OpenClick(object sender, EventArgs e)
		{
			while (true) {
				OpenFileDialog ofd = new OpenFileDialog();

				String filterString =
					StringParser.Parse("${res:ComponentInspector.ObjectBrowserForm.ActiveXFilesFilterName} (.tlb, *.olb, *.ocx, *.dll)" 
					    + "|*.tlb;*.olb;*.ocx;*.dll|");
				filterString += 
					StringParser.Parse("${res:ComponentInspector.ObjectBrowserForm.AssemblyFilesFilterName} (*.exe, *.dll)|*.exe;*.dll");
				filterString += StringParser.Parse("|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				ofd.Filter = filterString;
				ofd.FilterIndex = 1 ;
				ofd.RestoreDirectory = true ;
				ofd.Title = StringParser.Parse("${res:ComponentInspector.ObjectBrowserForm.OpenFileDialogTitle}");

				if (ofd.ShowDialog() == DialogResult.OK) {
					// Returns true if worked
					if (OpenFile(ofd.FileName.ToLower())) {
						break;
					}
					continue;
				}
				break;
			}
		}
		
		void CloseClick(object sender, EventArgs e)
		{
			_objectBrowser.CloseSelectedFile();
		}

		/// <summary>
		/// Returns true if the file was opened successfully.
		/// </summary>
		public bool OpenFile(string fileName)
		{
			return _objectBrowser.OpenFile(fileName);
		}
		
		void FindClick(object sender, EventArgs e)
		{
			FindDialog.DoShowDialog();
		}
		
		void AboutClick(object sender, EventArgs e)
		{
			using (Form about = new AboutDialog()) {
				about.ShowDialog();
			}
		}

		void HelpClick(object sender, EventArgs e)
		{
			try {
				Help.ShowHelp(this, ObjectBrowser.HelpFile);
			} catch (Exception ex) {
				TraceUtil.WriteLineIf(null, TraceLevel.Error, "Showing help: " + ex);
			}
		}

		void CustObjectClick(object sender, EventArgs e)
		{
			_customizeForm.DoShowDialog(new CustObjectPanel());
		}

		void CustTypeHandlerClick(object sender, EventArgs e)
		{
			_customizeForm.DoShowDialog(new CustTypeHandlerPanel());
		}

		void CustPanelClick(object sender, EventArgs e)
		{
			_customizeForm.DoShowDialog(new CustShowPanel());
		}

		void CustDirClick(object sender, EventArgs e)
		{
			_customizeForm.DoShowDialog(new CustDirPanel());
		}

		void CustActiveXClick(object sender, EventArgs e)
		{
			_customizeForm.DoShowDialog(new CustActiveXPanel());
		}

		void AttachClick(object sender, EventArgs e)
		{
			// This checks the results and sets everything
			_attachForm.DoShowDialog();
		}

		void TracingClick(object sender, EventArgs e)
		{
			// This checks the results and sets everything
			_tracingForm.DoShowDialog();
		}
		
		void InitializeComponent()
		{
			SuspendLayout();

			Text = Constants.NOGOOP + " " + ".NET Component Inspector";
			Icon = PresentationMap.GetApplicationIcon();
			CausesValidation = false;

			Size = new Size(800, 700);

			_objectBrowser = new ObjectBrowser();
			_objectBrowser.Dock = DockStyle.Fill;
			Controls.Add(_objectBrowser);
		
			CreateMenus();
		
			ResumeLayout();
		}
		
		void ExitClick(object sender, EventArgs e)
		{
			Close();
		}
	}
}
