// *****************************************************************************
// 
//  Copyright 2003, Weifen Luo
//  All rights reserved. The software and associated documentation 
//  supplied hereunder are the proprietary information of Weifen Luo
//  and are supplied subject to licence terms.
// 
//  DockSample Application 1.0
// *****************************************************************************

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Data;
using System.IO;
using WeifenLuo.WinFormsUI;

namespace DockSample
{
	/// <summary>
	/// Summary description for MainForm.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private DeserializeDockContent m_deserializeDockContent;
		private DummySolutionExplorer m_solutionExplorer = new DummySolutionExplorer();
		private DummyPropertyWindow m_propertyWindow = new DummyPropertyWindow();
		private DummyToolbox m_toolbox = new DummyToolbox();
		private DummyOutputWindow m_outputWindow = new DummyOutputWindow();
		private DummyTaskList m_taskList = new DummyTaskList();

		#if FRAMEWORK_VER_2x
		private System.Windows.Forms.MenuStrip mainMenu;
		private System.Windows.Forms.ToolStripSeparator menuItem4;
		private System.Windows.Forms.ToolStripMenuItem menuItemFile;
		private System.Windows.Forms.ToolStripMenuItem menuItemExit;
		private System.Windows.Forms.ToolStripMenuItem menuItemView;
		private System.Windows.Forms.ToolStripMenuItem menuItemSolutionExplorer;
		private System.Windows.Forms.ToolStripMenuItem menuItemPropertyWindow;
		private System.Windows.Forms.ToolStripMenuItem menuItemOutputWindow;
		private System.Windows.Forms.ToolStripMenuItem menuItemTaskList;
		private System.Windows.Forms.ToolStripMenuItem menuItemToolbox;
		private System.Windows.Forms.ToolStripMenuItem menuItemWindow;
		private System.Windows.Forms.ToolStripMenuItem menuItemHelp;
		private System.Windows.Forms.ToolStripMenuItem menuItemAbout;
		private System.Windows.Forms.ToolStripMenuItem menuItemNew;
		private System.Windows.Forms.ToolStripMenuItem menuItemOpen;
		private System.Windows.Forms.ToolStripMenuItem menuItemClose;
		private System.Windows.Forms.ToolStripMenuItem menuItemCloseAll;
		private System.Windows.Forms.ToolStripMenuItem menuItemTools;
		private System.Windows.Forms.StatusStrip statusBar;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.ToolStripButton toolBarButtonNew;
		private System.Windows.Forms.ToolStripButton toolBarButtonOpen;
		private System.Windows.Forms.ToolStripButton toolBarButtonSolutionExplorer;
		private System.Windows.Forms.ToolStripButton toolBarButtonPropertyWindow;
		private System.Windows.Forms.ToolStripButton toolBarButtonToolbox;
		private System.Windows.Forms.ToolStripButton toolBarButtonOutputWindow;
		private System.Windows.Forms.ToolStripButton toolBarButtonTaskList;
		private System.Windows.Forms.ToolStripSeparator menuItem1;
		private System.Windows.Forms.ToolStripMenuItem menuItemToolBar;
		private System.Windows.Forms.ToolStripMenuItem menuItemStatusBar;
		private System.Windows.Forms.ToolStrip toolBar;
		private System.Windows.Forms.ToolStripMenuItem menuItemNewWindow;
		private WeifenLuo.WinFormsUI.DockPanel dockPanel;
		private System.Windows.Forms.ToolStripMenuItem menuItemLockLayout;
		private System.Windows.Forms.ToolStripSeparator menuItem2;
		private System.Windows.Forms.ToolStripSeparator toolBarButtonSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolBarButtonSeparator2;
		private System.Windows.Forms.ToolStripMenuItem menuItemLayoutByCode;
		private System.Windows.Forms.ToolStripButton toolBarButtonLayoutByCode;
		private System.Windows.Forms.ToolStripMenuItem menuItemLayoutByXml;
		private System.Windows.Forms.ToolStripButton toolBarButtonLayoutByXml;
		private System.Windows.Forms.ToolStripMenuItem menuItemSchemaDefault;
		private System.Windows.Forms.ToolStripMenuItem menuItemSchemaOverride;
		private System.Windows.Forms.ToolStripMenuItem menuItemSchemaFromBase;
		private System.Windows.Forms.ToolStripSeparator menuItem3;
		private System.Windows.Forms.ToolStripSeparator menuItem5;
		private System.Windows.Forms.ToolStripMenuItem menuItemCloseAllButThisOne;
		private System.Windows.Forms.ToolStripSeparator menuItem6;
		private System.Windows.Forms.ToolStripMenuItem menuItemDockingMdi;
		private System.Windows.Forms.ToolStripMenuItem menuItemDockingWindow;
		private System.Windows.Forms.ToolStripMenuItem menuItemSystemMdi;
		private System.Windows.Forms.ToolStripMenuItem menuItemDockingSdi;
		private System.Windows.Forms.ToolStripMenuItem menuItemShowDocumentIcon;
		private System.ComponentModel.IContainer components;
		#else
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItemFile;
		private System.Windows.Forms.MenuItem menuItemExit;
		private System.Windows.Forms.MenuItem menuItemView;
		private System.Windows.Forms.MenuItem menuItemSolutionExplorer;
		private System.Windows.Forms.MenuItem menuItemPropertyWindow;
		private System.Windows.Forms.MenuItem menuItemOutputWindow;
		private System.Windows.Forms.MenuItem menuItemTaskList;
		private System.Windows.Forms.MenuItem menuItemToolbox;
		private System.Windows.Forms.MenuItem menuItemWindow;
		private System.Windows.Forms.MenuItem menuItemHelp;
		private System.Windows.Forms.MenuItem menuItemAbout;
		private System.Windows.Forms.MenuItem menuItemNew;
		private System.Windows.Forms.MenuItem menuItemOpen;
		private System.Windows.Forms.MenuItem menuItemClose;
		private System.Windows.Forms.MenuItem menuItemCloseAll;
		private System.Windows.Forms.MenuItem menuItemTools;
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.ToolBarButton toolBarButtonNew;
		private System.Windows.Forms.ToolBarButton toolBarButtonOpen;
		private System.Windows.Forms.ToolBarButton toolBarButtonSolutionExplorer;
		private System.Windows.Forms.ToolBarButton toolBarButtonPropertyWindow;
		private System.Windows.Forms.ToolBarButton toolBarButtonToolbox;
		private System.Windows.Forms.ToolBarButton toolBarButtonOutputWindow;
		private System.Windows.Forms.ToolBarButton toolBarButtonTaskList;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItemToolBar;
		private System.Windows.Forms.MenuItem menuItemStatusBar;
		private System.Windows.Forms.ToolBar toolBar;
		private System.Windows.Forms.MenuItem menuItemNewWindow;
		private WeifenLuo.WinFormsUI.DockPanel dockPanel;
		private System.Windows.Forms.MenuItem menuItemLockLayout;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.ToolBarButton toolBarButtonSeparator1;
		private System.Windows.Forms.ToolBarButton toolBarButtonSeparator2;
		private System.Windows.Forms.MenuItem menuItemLayoutByCode;
		private System.Windows.Forms.ToolBarButton toolBarButtonLayoutByCode;
		private System.Windows.Forms.MenuItem menuItemLayoutByXml;
		private System.Windows.Forms.ToolBarButton toolBarButtonLayoutByXml;
		private System.Windows.Forms.MenuItem menuItemSchemaDefault;
		private System.Windows.Forms.MenuItem menuItemSchemaOverride;
		private System.Windows.Forms.MenuItem menuItemSchemaFromBase;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItemCloseAllButThisOne;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItemDockingMdi;
		private System.Windows.Forms.MenuItem menuItemDockingWindow;
		private System.Windows.Forms.MenuItem menuItemSystemMdi;
		private System.Windows.Forms.MenuItem menuItemDockingSdi;
		private System.Windows.Forms.MenuItem menuItemShowDocumentIcon;
		private System.ComponentModel.IContainer components;
		#endif

		public MainForm()
		{
			m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.mainMenu = new System.Windows.Forms.MenuStrip();
			this.menuItemFile = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemNew = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemClose = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemCloseAll = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemCloseAllButThisOne = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemExit = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemView = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemSolutionExplorer = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemPropertyWindow = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemToolbox = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemOutputWindow = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemTaskList = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemToolBar = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemStatusBar = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemLayoutByCode = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemLayoutByXml = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemTools = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemLockLayout = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemSchemaDefault = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemSchemaOverride = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemSchemaFromBase = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem6 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemDockingMdi = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemDockingSdi = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemDockingWindow = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemSystemMdi = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItem5 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemShowDocumentIcon = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemWindow = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemNewWindow = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.dockPanel = new WeifenLuo.WinFormsUI.DockPanel();
			this.statusBar = new System.Windows.Forms.StatusStrip();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.toolBar = new System.Windows.Forms.ToolStrip();
			this.toolBarButtonNew = new System.Windows.Forms.ToolStripButton();
			this.toolBarButtonOpen = new System.Windows.Forms.ToolStripButton();
			this.toolBarButtonSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolBarButtonSolutionExplorer = new System.Windows.Forms.ToolStripButton();
			this.toolBarButtonPropertyWindow = new System.Windows.Forms.ToolStripButton();
			this.toolBarButtonToolbox = new System.Windows.Forms.ToolStripButton();
			this.toolBarButtonOutputWindow = new System.Windows.Forms.ToolStripButton();
			this.toolBarButtonTaskList = new System.Windows.Forms.ToolStripButton();
			this.toolBarButtonSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolBarButtonLayoutByCode = new System.Windows.Forms.ToolStripButton();
			this.toolBarButtonLayoutByXml = new System.Windows.Forms.ToolStripButton();
			this.SuspendLayout();
			// 
			// mainMenu
			// 
			this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
																					 this.menuItemFile,
																					 this.menuItemView,
																					 this.menuItemTools,
																					 this.menuItemWindow,
																					 this.menuItemHelp});
            this.mainMenu.MdiWindowListItem = this.menuItemWindow;

			// 
			// menuItemFile
			// 
            this.menuItemFile.Text = "&File";
			this.menuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
																						 this.menuItemNew,
																						 this.menuItemOpen,
																						 this.menuItemClose,
																						 this.menuItemCloseAll,
																						 this.menuItemCloseAllButThisOne,
																						 this.menuItem4,
																						 this.menuItemExit});
			this.menuItemFile.DropDownOpening += new System.EventHandler(this.menuItemFile_Popup);
			// 
			// menuItemNew
			// 
			this.menuItemNew.Text = "&New";
			this.menuItemNew.Click += new System.EventHandler(this.menuItemNew_Click);
			// 
			// menuItemOpen
			// 
			this.menuItemOpen.Text = "&Open...";
			this.menuItemOpen.Click += new System.EventHandler(this.menuItemOpen_Click);
			// 
			// menuItemClose
			// 
			this.menuItemClose.Text = "&Close";
			this.menuItemClose.Click += new System.EventHandler(this.menuItemClose_Click);
			// 
			// menuItemCloseAll
			// 
			this.menuItemCloseAll.Text = "Close &All";
			this.menuItemCloseAll.Click += new System.EventHandler(this.menuItemCloseAll_Click);
			// 
			// menuItemCloseAllButThisOne
			// 
			this.menuItemCloseAllButThisOne.Text = "Close All &But This One";
			this.menuItemCloseAllButThisOne.Click += new System.EventHandler(this.menuItemCloseAllButThisOne_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Text = "-";
			// 
			// menuItemExit
			// 
			this.menuItemExit.Text = "&Exit";
			this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
			// 
			// menuItemView
			// 
			this.menuItemView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
																						 this.menuItemSolutionExplorer,
																						 this.menuItemPropertyWindow,
																						 this.menuItemToolbox,
																						 this.menuItemOutputWindow,
																						 this.menuItemTaskList,
																						 this.menuItem1,
																						 this.menuItemToolBar,
																						 this.menuItemStatusBar,
																						 this.menuItem2,
																						 this.menuItemLayoutByCode,
																						 this.menuItemLayoutByXml});
			this.menuItemView.MergeIndex = 1;
			this.menuItemView.Text = "&View";
			// 
			// menuItemSolutionExplorer
			// 
			this.menuItemSolutionExplorer.Text = "&Solution Explorer";
			this.menuItemSolutionExplorer.Click += new System.EventHandler(this.menuItemSolutionExplorer_Click);
			// 
			// menuItemPropertyWindow
			// 
			this.menuItemPropertyWindow.ShortcutKeys = System.Windows.Forms.Keys.F4;
			this.menuItemPropertyWindow.Text = "&Property Window";
			this.menuItemPropertyWindow.Click += new System.EventHandler(this.menuItemPropertyWindow_Click);
			// 
			// menuItemToolbox
			// 
			this.menuItemToolbox.Text = "&Toolbox";
			this.menuItemToolbox.Click += new System.EventHandler(this.menuItemToolbox_Click);
			// 
			// menuItemOutputWindow
			// 
			this.menuItemOutputWindow.Text = "&Output Window";
			this.menuItemOutputWindow.Click += new System.EventHandler(this.menuItemOutputWindow_Click);
			// 
			// menuItemTaskList
			// 
			this.menuItemTaskList.Text = "Task &List";
			this.menuItemTaskList.Click += new System.EventHandler(this.menuItemTaskList_Click);
			// 
			// menuItemToolBar
			// 
			this.menuItemToolBar.Checked = true;
			this.menuItemToolBar.Text = "Tool &Bar";
			this.menuItemToolBar.Click += new System.EventHandler(this.menuItemToolBar_Click);
			// 
			// menuItemStatusBar
			// 
			this.menuItemStatusBar.Checked = true;
			this.menuItemStatusBar.Text = "Status B&ar";
			this.menuItemStatusBar.Click += new System.EventHandler(this.menuItemStatusBar_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Text = "-";
			// 
			// menuItemLayoutByCode
			// 
			this.menuItemLayoutByCode.Text = "Layout By &Code";
			this.menuItemLayoutByCode.Click += new System.EventHandler(this.menuItemLayoutByCode_Click);
			// 
			// menuItemLayoutByXml
			// 
			this.menuItemLayoutByXml.Text = "Layout By &XML";
			this.menuItemLayoutByXml.Click += new System.EventHandler(this.menuItemLayoutByXml_Click);
			// 
			// menuItemTools
			// 
			this.menuItemTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
																						  this.menuItemLockLayout,
																						  this.menuItem3,
																						  this.menuItemSchemaDefault,
																						  this.menuItemSchemaOverride,
																						  this.menuItemSchemaFromBase,
																						  this.menuItem6,
																						  this.menuItemDockingMdi,
																						  this.menuItemDockingSdi,
																						  this.menuItemDockingWindow,
																						  this.menuItemSystemMdi,
																						  this.menuItem5,
																						  this.menuItemShowDocumentIcon});
			this.menuItemTools.MergeIndex = 2;
			this.menuItemTools.Text = "&Tools";
			this.menuItemTools.DropDownOpening += new System.EventHandler(this.menuItemTools_Popup);
			// 
			// menuItemLockLayout
			// 
			this.menuItemLockLayout.Text = "&Lock Layout";
			this.menuItemLockLayout.Click += new System.EventHandler(this.menuItemLockLayout_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Text = "-";
			// 
			// menuItemSchemaDefault
			// 
			this.menuItemSchemaDefault.Checked = true;
			this.menuItemSchemaDefault.Text = "Schema: &Default";
			this.menuItemSchemaDefault.Click += new System.EventHandler(this.SetSchema);
			// 
			// menuItemSchemaOverride
			// 
			this.menuItemSchemaOverride.Text = "Schema: &Override From Default";
			this.menuItemSchemaOverride.Click += new System.EventHandler(this.SetSchema);
			// 
			// menuItemSchemaFromBase
			// 
			this.menuItemSchemaFromBase.Text = "Schema: &Built From Base";
			this.menuItemSchemaFromBase.Click += new System.EventHandler(this.SetSchema);
			// 
			// menuItem6
			// 
			this.menuItem6.Text = "-";
			// 
			// menuItemDockingMdi
			// 
			this.menuItemDockingMdi.Checked = true;
			this.menuItemDockingMdi.Text = "Document Style: Docking &MDI";
			this.menuItemDockingMdi.Click += new System.EventHandler(this.SetDocumentStyle);
			// 
			// menuItemDockingSdi
			// 
			this.menuItemDockingSdi.Text = "Document Style: Docking &SDI";
			this.menuItemDockingSdi.Click += new System.EventHandler(this.SetDocumentStyle);
			// 
			// menuItemDockingWindow
			// 
			this.menuItemDockingWindow.Text = "Document Style: Docking &Window";
			this.menuItemDockingWindow.Click += new System.EventHandler(this.SetDocumentStyle);
			// 
			// menuItemSystemMdi
			// 
			this.menuItemSystemMdi.Text = "Document Style: S&ystem MDI";
			this.menuItemSystemMdi.Click += new System.EventHandler(this.SetDocumentStyle);
			// 
			// menuItem5
			// 
			this.menuItem5.Text = "-";
			// 
			// menuItemShowDocumentIcon
			// 
			this.menuItemShowDocumentIcon.Text = "&Show Document Icon";
			this.menuItemShowDocumentIcon.Click += new System.EventHandler(this.menuItemShowDocumentIcon_Click);
			// 
			// menuItemWindow
			// 
			this.menuItemWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
																						   this.menuItemNewWindow});
			this.menuItemWindow.MergeIndex = 2;
			this.menuItemWindow.Text = "&Window";
			// 
			// menuItemNewWindow
			// 
			this.menuItemNewWindow.Text = "&New Window";
			this.menuItemNewWindow.Click += new System.EventHandler(this.menuItemNewWindow_Click);
			// 
			// menuItemHelp
			// 
			this.menuItemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
																						 this.menuItemAbout});
			this.menuItemHelp.MergeIndex = 3;
			this.menuItemHelp.Text = "&Help";
			// 
			// menuItemAbout
			// 
			this.menuItemAbout.Text = "&About DockSample...";
			this.menuItemAbout.Click += new System.EventHandler(this.menuItemAbout_Click);
			// 
			// dockPanel
			// 
			this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dockPanel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.dockPanel.Location = new System.Drawing.Point(0, 28);
			this.dockPanel.Name = "dockPanel";
			this.dockPanel.Size = new System.Drawing.Size(579, 359);
			this.dockPanel.TabIndex = 0;
			// 
			// statusBar
			// 
			this.statusBar.Location = new System.Drawing.Point(0, 387);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(579, 22);
			this.statusBar.TabIndex = 4;
			// 
			// imageList
			// 
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// toolBar
			// 
			this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
																					   this.toolBarButtonNew,
																					   this.toolBarButtonOpen,
																					   this.toolBarButtonSeparator1,
																					   this.toolBarButtonSolutionExplorer,
																					   this.toolBarButtonPropertyWindow,
																					   this.toolBarButtonToolbox,
																					   this.toolBarButtonOutputWindow,
																					   this.toolBarButtonTaskList,
																					   this.toolBarButtonSeparator2,
																					   this.toolBarButtonLayoutByCode,
																					   this.toolBarButtonLayoutByXml});
			this.toolBar.ImageList = this.imageList;
			this.toolBar.Location = new System.Drawing.Point(0, 0);
			this.toolBar.Name = "toolBar";
			this.toolBar.Size = new System.Drawing.Size(579, 28);
			this.toolBar.TabIndex = 6;
			this.toolBar.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolBar_ButtonClick);
			// 
			// toolBarButtonNew
			// 
			this.toolBarButtonNew.ImageIndex = 0;
			this.toolBarButtonNew.ToolTipText = "Show Layout From XML";
			// 
			// toolBarButtonOpen
			// 
			this.toolBarButtonOpen.ImageIndex = 1;
			this.toolBarButtonOpen.ToolTipText = "Open";
			// 
			// toolBarButtonSolutionExplorer
			// 
			this.toolBarButtonSolutionExplorer.ImageIndex = 2;
			this.toolBarButtonSolutionExplorer.ToolTipText = "Solution Explorer";
			// 
			// toolBarButtonPropertyWindow
			// 
			this.toolBarButtonPropertyWindow.ImageIndex = 3;
			this.toolBarButtonPropertyWindow.ToolTipText = "Property Window";
			// 
			// toolBarButtonToolbox
			// 
			this.toolBarButtonToolbox.ImageIndex = 4;
			this.toolBarButtonToolbox.ToolTipText = "Tool Box";
			// 
			// toolBarButtonOutputWindow
			// 
			this.toolBarButtonOutputWindow.ImageIndex = 5;
			this.toolBarButtonOutputWindow.ToolTipText = "Output Window";
			// 
			// toolBarButtonTaskList
			// 
			this.toolBarButtonTaskList.ImageIndex = 6;
			this.toolBarButtonTaskList.ToolTipText = "Task List";
			// 
			// toolBarButtonLayoutByCode
			// 
			this.toolBarButtonLayoutByCode.ImageIndex = 7;
			this.toolBarButtonLayoutByCode.ToolTipText = "Show Layout By Code";
			// 
			// toolBarButtonLayoutByXml
			// 
			this.toolBarButtonLayoutByXml.ImageIndex = 8;
			this.toolBarButtonLayoutByXml.ToolTipText = "Show layout by predefined XML file";
			// 
			// MainForm
			// 
			this.ClientSize = new System.Drawing.Size(579, 409);
			this.Controls.Add(this.dockPanel);
			this.Controls.Add(this.toolBar);
            this.Controls.Add(mainMenu);
            this.Controls.Add(this.statusBar);
			this.IsMdiContainer = true;
			this.MainMenuStrip = this.mainMenu;
			this.Name = "MainForm";
			this.Text = "DockSample";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
			this.Load += new System.EventHandler(this.MainForm_Load);
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuItemFile = new System.Windows.Forms.MenuItem();
			this.menuItemNew = new System.Windows.Forms.MenuItem();
			this.menuItemOpen = new System.Windows.Forms.MenuItem();
			this.menuItemClose = new System.Windows.Forms.MenuItem();
			this.menuItemCloseAll = new System.Windows.Forms.MenuItem();
			this.menuItemCloseAllButThisOne = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItemExit = new System.Windows.Forms.MenuItem();
			this.menuItemView = new System.Windows.Forms.MenuItem();
			this.menuItemSolutionExplorer = new System.Windows.Forms.MenuItem();
			this.menuItemPropertyWindow = new System.Windows.Forms.MenuItem();
			this.menuItemToolbox = new System.Windows.Forms.MenuItem();
			this.menuItemOutputWindow = new System.Windows.Forms.MenuItem();
			this.menuItemTaskList = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItemToolBar = new System.Windows.Forms.MenuItem();
			this.menuItemStatusBar = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItemLayoutByCode = new System.Windows.Forms.MenuItem();
			this.menuItemLayoutByXml = new System.Windows.Forms.MenuItem();
			this.menuItemTools = new System.Windows.Forms.MenuItem();
			this.menuItemLockLayout = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItemSchemaDefault = new System.Windows.Forms.MenuItem();
			this.menuItemSchemaOverride = new System.Windows.Forms.MenuItem();
			this.menuItemSchemaFromBase = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItemDockingMdi = new System.Windows.Forms.MenuItem();
			this.menuItemDockingSdi = new System.Windows.Forms.MenuItem();
			this.menuItemDockingWindow = new System.Windows.Forms.MenuItem();
			this.menuItemSystemMdi = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItemShowDocumentIcon = new System.Windows.Forms.MenuItem();
			this.menuItemWindow = new System.Windows.Forms.MenuItem();
			this.menuItemNewWindow = new System.Windows.Forms.MenuItem();
			this.menuItemHelp = new System.Windows.Forms.MenuItem();
			this.menuItemAbout = new System.Windows.Forms.MenuItem();
			this.dockPanel = new WeifenLuo.WinFormsUI.DockPanel();
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.toolBar = new System.Windows.Forms.ToolBar();
			this.toolBarButtonNew = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonOpen = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonSeparator1 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonSolutionExplorer = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonPropertyWindow = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonToolbox = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonOutputWindow = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonTaskList = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonSeparator2 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonLayoutByCode = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonLayoutByXml = new System.Windows.Forms.ToolBarButton();
			this.SuspendLayout();
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuItemFile,
																					 this.menuItemView,
																					 this.menuItemTools,
																					 this.menuItemWindow,
																					 this.menuItemHelp});
			// 
			// menuItemFile
			// 
			this.menuItemFile.Index = 0;
			this.menuItemFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItemNew,
																						 this.menuItemOpen,
																						 this.menuItemClose,
																						 this.menuItemCloseAll,
																						 this.menuItemCloseAllButThisOne,
																						 this.menuItem4,
																						 this.menuItemExit});
			this.menuItemFile.Text = "&File";
			this.menuItemFile.Popup += new System.EventHandler(this.menuItemFile_Popup);
			// 
			// menuItemNew
			// 
			this.menuItemNew.Index = 0;
			this.menuItemNew.Text = "&New";
			this.menuItemNew.Click += new System.EventHandler(this.menuItemNew_Click);
			// 
			// menuItemOpen
			// 
			this.menuItemOpen.Index = 1;
			this.menuItemOpen.Text = "&Open...";
			this.menuItemOpen.Click += new System.EventHandler(this.menuItemOpen_Click);
			// 
			// menuItemClose
			// 
			this.menuItemClose.Index = 2;
			this.menuItemClose.Text = "&Close";
			this.menuItemClose.Click += new System.EventHandler(this.menuItemClose_Click);
			// 
			// menuItemCloseAll
			// 
			this.menuItemCloseAll.Index = 3;
			this.menuItemCloseAll.Text = "Close &All";
			this.menuItemCloseAll.Click += new System.EventHandler(this.menuItemCloseAll_Click);
			// 
			// menuItemCloseAllButThisOne
			// 
			this.menuItemCloseAllButThisOne.Index = 4;
			this.menuItemCloseAllButThisOne.Text = "Close All &But This One";
			this.menuItemCloseAllButThisOne.Click += new System.EventHandler(this.menuItemCloseAllButThisOne_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 5;
			this.menuItem4.Text = "-";
			// 
			// menuItemExit
			// 
			this.menuItemExit.Index = 6;
			this.menuItemExit.Text = "&Exit";
			this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
			// 
			// menuItemView
			// 
			this.menuItemView.Index = 1;
			this.menuItemView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItemSolutionExplorer,
																						 this.menuItemPropertyWindow,
																						 this.menuItemToolbox,
																						 this.menuItemOutputWindow,
																						 this.menuItemTaskList,
																						 this.menuItem1,
																						 this.menuItemToolBar,
																						 this.menuItemStatusBar,
																						 this.menuItem2,
																						 this.menuItemLayoutByCode,
																						 this.menuItemLayoutByXml});
			this.menuItemView.MergeOrder = 1;
			this.menuItemView.Text = "&View";
			// 
			// menuItemSolutionExplorer
			// 
			this.menuItemSolutionExplorer.Index = 0;
			this.menuItemSolutionExplorer.Text = "&Solution Explorer";
			this.menuItemSolutionExplorer.Click += new System.EventHandler(this.menuItemSolutionExplorer_Click);
			// 
			// menuItemPropertyWindow
			// 
			this.menuItemPropertyWindow.Index = 1;
			this.menuItemPropertyWindow.Shortcut = System.Windows.Forms.Shortcut.F4;
			this.menuItemPropertyWindow.Text = "&Property Window";
			this.menuItemPropertyWindow.Click += new System.EventHandler(this.menuItemPropertyWindow_Click);
			// 
			// menuItemToolbox
			// 
			this.menuItemToolbox.Index = 2;
			this.menuItemToolbox.Text = "&Toolbox";
			this.menuItemToolbox.Click += new System.EventHandler(this.menuItemToolbox_Click);
			// 
			// menuItemOutputWindow
			// 
			this.menuItemOutputWindow.Index = 3;
			this.menuItemOutputWindow.Text = "&Output Window";
			this.menuItemOutputWindow.Click += new System.EventHandler(this.menuItemOutputWindow_Click);
			// 
			// menuItemTaskList
			// 
			this.menuItemTaskList.Index = 4;
			this.menuItemTaskList.Text = "Task &List";
			this.menuItemTaskList.Click += new System.EventHandler(this.menuItemTaskList_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 5;
			this.menuItem1.Text = "-";
			// 
			// menuItemToolBar
			// 
			this.menuItemToolBar.Checked = true;
			this.menuItemToolBar.Index = 6;
			this.menuItemToolBar.Text = "Tool &Bar";
			this.menuItemToolBar.Click += new System.EventHandler(this.menuItemToolBar_Click);
			// 
			// menuItemStatusBar
			// 
			this.menuItemStatusBar.Checked = true;
			this.menuItemStatusBar.Index = 7;
			this.menuItemStatusBar.Text = "Status B&ar";
			this.menuItemStatusBar.Click += new System.EventHandler(this.menuItemStatusBar_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 8;
			this.menuItem2.Text = "-";
			// 
			// menuItemLayoutByCode
			// 
			this.menuItemLayoutByCode.Index = 9;
			this.menuItemLayoutByCode.Text = "Layout By &Code";
			this.menuItemLayoutByCode.Click += new System.EventHandler(this.menuItemLayoutByCode_Click);
			// 
			// menuItemLayoutByXml
			// 
			this.menuItemLayoutByXml.Index = 10;
			this.menuItemLayoutByXml.Text = "Layout By &XML";
			this.menuItemLayoutByXml.Click += new System.EventHandler(this.menuItemLayoutByXml_Click);
			// 
			// menuItemTools
			// 
			this.menuItemTools.Index = 2;
			this.menuItemTools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						  this.menuItemLockLayout,
																						  this.menuItem3,
																						  this.menuItemSchemaDefault,
																						  this.menuItemSchemaOverride,
																						  this.menuItemSchemaFromBase,
																						  this.menuItem6,
																						  this.menuItemDockingMdi,
																						  this.menuItemDockingSdi,
																						  this.menuItemDockingWindow,
																						  this.menuItemSystemMdi,
																						  this.menuItem5,
																						  this.menuItemShowDocumentIcon});
			this.menuItemTools.MergeOrder = 2;
			this.menuItemTools.Text = "&Tools";
			this.menuItemTools.Popup += new System.EventHandler(this.menuItemTools_Popup);
			// 
			// menuItemLockLayout
			// 
			this.menuItemLockLayout.Index = 0;
			this.menuItemLockLayout.Text = "&Lock Layout";
			this.menuItemLockLayout.Click += new System.EventHandler(this.menuItemLockLayout_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.Text = "-";
			// 
			// menuItemSchemaDefault
			// 
			this.menuItemSchemaDefault.Checked = true;
			this.menuItemSchemaDefault.Index = 2;
			this.menuItemSchemaDefault.Text = "Schema: &Default";
			this.menuItemSchemaDefault.Click += new System.EventHandler(this.SetSchema);
			// 
			// menuItemSchemaOverride
			// 
			this.menuItemSchemaOverride.Index = 3;
			this.menuItemSchemaOverride.Text = "Schema: &Override From Default";
			this.menuItemSchemaOverride.Click += new System.EventHandler(this.SetSchema);
			// 
			// menuItemSchemaFromBase
			// 
			this.menuItemSchemaFromBase.Index = 4;
			this.menuItemSchemaFromBase.Text = "Schema: &Built From Base";
			this.menuItemSchemaFromBase.Click += new System.EventHandler(this.SetSchema);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 5;
			this.menuItem6.Text = "-";
			// 
			// menuItemDockingMdi
			// 
			this.menuItemDockingMdi.Checked = true;
			this.menuItemDockingMdi.Index = 6;
			this.menuItemDockingMdi.Text = "Document Style: Docking &MDI";
			this.menuItemDockingMdi.Click += new System.EventHandler(this.SetDocumentStyle);
			// 
			// menuItemDockingSdi
			// 
			this.menuItemDockingSdi.Index = 7;
			this.menuItemDockingSdi.Text = "Document Style: Docking &SDI";
			this.menuItemDockingSdi.Click += new System.EventHandler(this.SetDocumentStyle);
			// 
			// menuItemDockingWindow
			// 
			this.menuItemDockingWindow.Index = 8;
			this.menuItemDockingWindow.Text = "Document Style: Docking &Window";
			this.menuItemDockingWindow.Click += new System.EventHandler(this.SetDocumentStyle);
			// 
			// menuItemSystemMdi
			// 
			this.menuItemSystemMdi.Index = 9;
			this.menuItemSystemMdi.Text = "Document Style: S&ystem MDI";
			this.menuItemSystemMdi.Click += new System.EventHandler(this.SetDocumentStyle);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 10;
			this.menuItem5.Text = "-";
			// 
			// menuItemShowDocumentIcon
			// 
			this.menuItemShowDocumentIcon.Index = 11;
			this.menuItemShowDocumentIcon.Text = "&Show Document Icon";
			this.menuItemShowDocumentIcon.Click += new System.EventHandler(this.menuItemShowDocumentIcon_Click);
			// 
			// menuItemWindow
			// 
			this.menuItemWindow.Index = 3;
			this.menuItemWindow.MdiList = true;
			this.menuItemWindow.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						   this.menuItemNewWindow});
			this.menuItemWindow.MergeOrder = 2;
			this.menuItemWindow.Text = "&Window";
			// 
			// menuItemNewWindow
			// 
			this.menuItemNewWindow.Index = 0;
			this.menuItemNewWindow.Text = "&New Window";
			this.menuItemNewWindow.Click += new System.EventHandler(this.menuItemNewWindow_Click);
			// 
			// menuItemHelp
			// 
			this.menuItemHelp.Index = 4;
			this.menuItemHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItemAbout});
			this.menuItemHelp.MergeOrder = 3;
			this.menuItemHelp.Text = "&Help";
			// 
			// menuItemAbout
			// 
			this.menuItemAbout.Index = 0;
			this.menuItemAbout.Text = "&About DockSample...";
			this.menuItemAbout.Click += new System.EventHandler(this.menuItemAbout_Click);
			// 
			// dockPanel
			// 
			this.dockPanel.ActiveAutoHideContent = null;
			this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dockPanel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.dockPanel.Location = new System.Drawing.Point(0, 28);
			this.dockPanel.Name = "dockPanel";
			this.dockPanel.Size = new System.Drawing.Size(579, 359);
			this.dockPanel.TabIndex = 0;
			// 
			// statusBar
			// 
			this.statusBar.Location = new System.Drawing.Point(0, 387);
			this.statusBar.Name = "statusBar";
			this.statusBar.ShowPanels = true;
			this.statusBar.Size = new System.Drawing.Size(579, 22);
			this.statusBar.TabIndex = 4;
			// 
			// imageList
			// 
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// toolBar
			// 
			this.toolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																					   this.toolBarButtonNew,
																					   this.toolBarButtonOpen,
																					   this.toolBarButtonSeparator1,
																					   this.toolBarButtonSolutionExplorer,
																					   this.toolBarButtonPropertyWindow,
																					   this.toolBarButtonToolbox,
																					   this.toolBarButtonOutputWindow,
																					   this.toolBarButtonTaskList,
																					   this.toolBarButtonSeparator2,
																					   this.toolBarButtonLayoutByCode,
																					   this.toolBarButtonLayoutByXml});
			this.toolBar.DropDownArrows = true;
			this.toolBar.ImageList = this.imageList;
			this.toolBar.Location = new System.Drawing.Point(0, 0);
			this.toolBar.Name = "toolBar";
			this.toolBar.ShowToolTips = true;
			this.toolBar.Size = new System.Drawing.Size(579, 28);
			this.toolBar.TabIndex = 6;
			this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
			// 
			// toolBarButtonNew
			// 
			this.toolBarButtonNew.ImageIndex = 0;
			this.toolBarButtonNew.ToolTipText = "Show Layout From XML";
			// 
			// toolBarButtonOpen
			// 
			this.toolBarButtonOpen.ImageIndex = 1;
			this.toolBarButtonOpen.ToolTipText = "Open";
			// 
			// toolBarButtonSeparator1
			// 
			this.toolBarButtonSeparator1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarButtonSolutionExplorer
			// 
			this.toolBarButtonSolutionExplorer.ImageIndex = 2;
			this.toolBarButtonSolutionExplorer.ToolTipText = "Solution Explorer";
			// 
			// toolBarButtonPropertyWindow
			// 
			this.toolBarButtonPropertyWindow.ImageIndex = 3;
			this.toolBarButtonPropertyWindow.ToolTipText = "Property Window";
			// 
			// toolBarButtonToolbox
			// 
			this.toolBarButtonToolbox.ImageIndex = 4;
			this.toolBarButtonToolbox.ToolTipText = "Tool Box";
			// 
			// toolBarButtonOutputWindow
			// 
			this.toolBarButtonOutputWindow.ImageIndex = 5;
			this.toolBarButtonOutputWindow.ToolTipText = "Output Window";
			// 
			// toolBarButtonTaskList
			// 
			this.toolBarButtonTaskList.ImageIndex = 6;
			this.toolBarButtonTaskList.ToolTipText = "Task List";
			// 
			// toolBarButtonSeparator2
			// 
			this.toolBarButtonSeparator2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarButtonLayoutByCode
			// 
			this.toolBarButtonLayoutByCode.ImageIndex = 7;
			this.toolBarButtonLayoutByCode.ToolTipText = "Show Layout By Code";
			// 
			// toolBarButtonLayoutByXml
			// 
			this.toolBarButtonLayoutByXml.ImageIndex = 8;
			this.toolBarButtonLayoutByXml.ToolTipText = "Show layout by predefined XML file";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(579, 409);
			this.Controls.Add(this.dockPanel);
			this.Controls.Add(this.toolBar);
			this.Controls.Add(this.statusBar);
			this.IsMdiContainer = true;
			this.Menu = this.mainMenu;
			this.Name = "MainForm";
			this.Text = "DockSample";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.ResumeLayout(false);

		}
		#endregion
		#endif

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}

		private void menuItemExit_Click(object sender, System.EventArgs e)
		{
			Close();
			Application.Exit();
		}

		private void menuItemSolutionExplorer_Click(object sender, System.EventArgs e)
		{
			m_solutionExplorer.Show(dockPanel);
		}

		private void menuItemPropertyWindow_Click(object sender, System.EventArgs e)
		{
			m_propertyWindow.Show(dockPanel);
		}

		private void menuItemToolbox_Click(object sender, System.EventArgs e)
		{
			m_toolbox.Show(dockPanel);
		}

		private void menuItemOutputWindow_Click(object sender, System.EventArgs e)
		{
			m_outputWindow.Show(dockPanel);
		}

		private void menuItemTaskList_Click(object sender, System.EventArgs e)
		{
			m_taskList.Show(dockPanel);
		}

		private void menuItemAbout_Click(object sender, System.EventArgs e)
		{
			AboutDialog aboutDialog = new AboutDialog();
			aboutDialog.ShowDialog(this);
		}

		private IDockContent FindDocument(string text)
		{
			if (dockPanel.DocumentStyle == DocumentStyles.SystemMdi)
			{
				foreach (Form form in MdiChildren)
					if (form.Text == text)
						return form as IDockContent;
				
				return null;
			}
			else
			{
				IDockContent[] documents = dockPanel.Documents;

				foreach (IDockContent content in documents)
					if (content.DockHandler.TabText == text)
						return content;

				return null;
			}
		}

		private void menuItemNew_Click(object sender, System.EventArgs e)
		{
			DummyDoc dummyDoc = CreateNewDocument();
			if (dockPanel.DocumentStyle == DocumentStyles.SystemMdi)
			{
				dummyDoc.MdiParent = this;
				dummyDoc.Show();
			}
			else
				dummyDoc.Show(dockPanel);
		}

		private DummyDoc CreateNewDocument()
		{
			DummyDoc dummyDoc = new DummyDoc();

			int count = 1;
			string text = "Document" + count.ToString();
			while (FindDocument(text) != null)
			{
				count ++;
				text = "Document" + count.ToString();
			}
			dummyDoc.Text = text;
			return dummyDoc;
		}

		private DummyDoc CreateNewDocument(string text)
		{
			DummyDoc dummyDoc = new DummyDoc();
			dummyDoc.Text = text;
			return dummyDoc;
		}

		private void menuItemOpen_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog openFile = new OpenFileDialog();

			openFile.InitialDirectory = Application.ExecutablePath;
			openFile.Filter = "rtf files (*.rtf)|*.rtf|txt files (*.txt)|*.txt|All files (*.*)|*.*" ;
			openFile.FilterIndex = 1;
			openFile.RestoreDirectory = true ;

			if(openFile.ShowDialog() == DialogResult.OK)
			{
				string fullName = openFile.FileName;
				string fileName = Path.GetFileName(fullName);

				if (FindDocument(fileName) != null)
				{
					MessageBox.Show("The document: " + fileName + " has already opened!");
					return;
				}

				DummyDoc dummyDoc = new DummyDoc();
				dummyDoc.Text = fileName;
				dummyDoc.Show(dockPanel);
				try
				{
					dummyDoc.FileName = fullName;
				}
				catch (Exception exception)
				{
					dummyDoc.Close();
					MessageBox.Show(exception.Message);
				}

			}
		}

		private void menuItemFile_Popup(object sender, System.EventArgs e)
		{
			if (dockPanel.DocumentStyle == DocumentStyles.SystemMdi)
			{
				menuItemClose.Enabled = menuItemCloseAll.Enabled = (ActiveMdiChild != null);
			}
			else
			{
				menuItemClose.Enabled = (dockPanel.ActiveDocument != null);
				menuItemCloseAll.Enabled = (dockPanel.Documents.Length > 0);
			}
		}

		private void menuItemClose_Click(object sender, System.EventArgs e)
		{
			if (dockPanel.DocumentStyle == DocumentStyles.SystemMdi)
				ActiveMdiChild.Close();
			else if (dockPanel.ActiveDocument != null)
				dockPanel.ActiveDocument.DockHandler.Close();
		}

		private void menuItemCloseAll_Click(object sender, System.EventArgs e)
		{
			CloseAllDocuments();
		}

		private void CloseAllDocuments()
		{
			if (dockPanel.DocumentStyle == DocumentStyles.SystemMdi)
			{
				foreach (Form form in MdiChildren)
					form.Close();
			}
			else
			{
				foreach (IDockContent content in dockPanel.Documents)
					content.DockHandler.Close();
			}
		}

		private IDockContent GetContentFromPersistString(string persistString)
		{
			if (persistString == typeof(DummySolutionExplorer).ToString())
				return m_solutionExplorer;
			else if (persistString == typeof(DummyPropertyWindow).ToString())
				return m_propertyWindow;
			else if (persistString == typeof(DummyToolbox).ToString())
				return m_toolbox;
			else if (persistString == typeof(DummyOutputWindow).ToString())
				return m_outputWindow;
			else if (persistString == typeof(DummyTaskList).ToString())
				return m_taskList;
			else
			{
				string[] parsedStrings = persistString.Split(new char[] { ',' });
				if (parsedStrings.Length != 3)
					return null;

				if (parsedStrings[0] != typeof(DummyDoc).ToString())
					return null;

				DummyDoc dummyDoc = new DummyDoc();
				if (parsedStrings[1] != string.Empty)
					dummyDoc.FileName = parsedStrings[1];
				if (parsedStrings[2] != string.Empty)
					dummyDoc.Text = parsedStrings[2];

				return dummyDoc;
			}
		}

		private void MainForm_Load(object sender, System.EventArgs e)
		{
			string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");

			if (File.Exists(configFile))
				dockPanel.LoadFromXml(configFile, m_deserializeDockContent);
		}

		private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
			dockPanel.SaveAsXml(configFile);
		}

		private void menuItemToolBar_Click(object sender, System.EventArgs e)
		{
			toolBar.Visible = menuItemToolBar.Checked = !menuItemToolBar.Checked;
		}

		private void menuItemStatusBar_Click(object sender, System.EventArgs e)
		{
			statusBar.Visible = menuItemStatusBar.Checked = !menuItemStatusBar.Checked;
		}

        #if FRAMEWORK_VER_2x
        private void toolBar_ButtonClick(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == toolBarButtonNew)
                menuItemNew_Click(null, null);
            else if (e.ClickedItem == toolBarButtonOpen)
                menuItemOpen_Click(null, null);
            else if (e.ClickedItem == toolBarButtonSolutionExplorer)
                menuItemSolutionExplorer_Click(null, null);
            else if (e.ClickedItem == toolBarButtonPropertyWindow)
                menuItemPropertyWindow_Click(null, null);
            else if (e.ClickedItem == toolBarButtonToolbox)
                menuItemToolbox_Click(null, null);
            else if (e.ClickedItem == toolBarButtonOutputWindow)
                menuItemOutputWindow_Click(null, null);
            else if (e.ClickedItem == toolBarButtonTaskList)
                menuItemTaskList_Click(null, null);
            else if (e.ClickedItem == toolBarButtonLayoutByCode)
                menuItemLayoutByCode_Click(null, null);
            else if (e.ClickedItem == toolBarButtonLayoutByXml)
                menuItemLayoutByXml_Click(null, null);
        }
        #else
		private void toolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == toolBarButtonNew)
				menuItemNew_Click(null, null);
			else if (e.Button == toolBarButtonOpen)
				menuItemOpen_Click(null, null);
			else if (e.Button == toolBarButtonSolutionExplorer)
				menuItemSolutionExplorer_Click(null, null);
			else if (e.Button == toolBarButtonPropertyWindow)
				menuItemPropertyWindow_Click(null, null);
			else if (e.Button == toolBarButtonToolbox)
				menuItemToolbox_Click(null, null);
			else if (e.Button == toolBarButtonOutputWindow)
				menuItemOutputWindow_Click(null, null);
			else if (e.Button == toolBarButtonTaskList)
				menuItemTaskList_Click(null, null);
			else if (e.Button == toolBarButtonLayoutByCode)
				menuItemLayoutByCode_Click(null, null);
			else if (e.Button == toolBarButtonLayoutByXml)
				menuItemLayoutByXml_Click(null, null);
		}
        #endif

		private void menuItemNewWindow_Click(object sender, System.EventArgs e)
		{
			MainForm newWindow = new MainForm();
			newWindow.Text = newWindow.Text + " - New";
			newWindow.Show();
		}

		private void menuItemTools_Popup(object sender, System.EventArgs e)
		{
			menuItemLockLayout.Checked = !this.dockPanel.AllowRedocking;
		}

		private void menuItemLockLayout_Click(object sender, System.EventArgs e)
		{
			dockPanel.AllowRedocking = !dockPanel.AllowRedocking;
		}

		private void menuItemLayoutByCode_Click(object sender, System.EventArgs e)
		{
			dockPanel.SuspendLayout(true);

			m_solutionExplorer.Show(dockPanel, DockState.DockRight);
			m_propertyWindow.Show(m_solutionExplorer.Pane, m_solutionExplorer);
			m_toolbox.Show(dockPanel, new Rectangle(98, 133, 200, 383));
			m_outputWindow.Show(m_solutionExplorer.Pane, DockAlignment.Bottom, 0.35);
			m_taskList.Show(m_toolbox.Pane, DockAlignment.Left, 0.4);

			CloseAllDocuments();
			DummyDoc doc1 = CreateNewDocument("Document1");
			DummyDoc doc2 = CreateNewDocument("Document2");
			DummyDoc doc3 = CreateNewDocument("Document3");
			DummyDoc doc4 = CreateNewDocument("Document4");
			doc1.Show(dockPanel, DockState.Document);
			doc2.Show(doc1.Pane, null);
			doc3.Show(doc1.Pane, DockAlignment.Bottom, 0.5);
			doc4.Show(doc3.Pane, DockAlignment.Right, 0.5);

			dockPanel.ResumeLayout(true, true);
		}

		private void menuItemLayoutByXml_Click(object sender, System.EventArgs e)
		{
			dockPanel.SuspendLayout(true);

			// In order to load layout from XML, we need to close all the DockContents
			CloseAllContents();

			Assembly assembly = Assembly.GetAssembly(typeof(MainForm));
			Stream xmlStream = assembly.GetManifestResourceStream("DockSample.Resources.DockPanel.xml");
			dockPanel.LoadFromXml(xmlStream, m_deserializeDockContent);
			xmlStream.Close();

			dockPanel.ResumeLayout(true, true);
		}

		private void CloseAllContents()
		{
			// we don't want to create another instance of tool window, set DockPanel to null
			m_solutionExplorer.DockPanel = null;
			m_propertyWindow.DockPanel = null;
			m_toolbox.DockPanel = null;
			m_outputWindow.DockPanel = null;
			m_taskList.DockPanel = null;

			// Close all other document windows
			CloseAllDocuments();
		}

		private void SetSchema(object sender, System.EventArgs e)
		{
			CloseAllContents();

			if (sender == menuItemSchemaDefault)
				Extender.SetSchema(dockPanel, Extender.Schema.Default);
			else if (sender == menuItemSchemaOverride)
				Extender.SetSchema(dockPanel, Extender.Schema.Override);
			else if (sender == menuItemSchemaFromBase)
				Extender.SetSchema(dockPanel, Extender.Schema.FromBase);

            menuItemSchemaDefault.Checked = (sender == menuItemSchemaDefault);
            menuItemSchemaOverride.Checked = (sender == menuItemSchemaOverride);
            menuItemSchemaFromBase.Checked = (sender == menuItemSchemaFromBase);
		}

		private void SetDocumentStyle(object sender, System.EventArgs e)
		{
			DocumentStyles oldStyle = dockPanel.DocumentStyle;
			DocumentStyles newStyle;
			if (sender == menuItemDockingMdi)
				newStyle = DocumentStyles.DockingMdi;
			else if (sender == menuItemDockingWindow)
				newStyle = DocumentStyles.DockingWindow;
			else if (sender == menuItemDockingSdi)
				newStyle = DocumentStyles.DockingSdi;
			else
				newStyle = DocumentStyles.SystemMdi;
			
			if (oldStyle == newStyle)
				return;

			if (oldStyle == DocumentStyles.SystemMdi || newStyle == DocumentStyles.SystemMdi)
				CloseAllDocuments();

			dockPanel.DocumentStyle = newStyle;
			menuItemDockingMdi.Checked = (newStyle == DocumentStyles.DockingMdi);
			menuItemDockingWindow.Checked = (newStyle == DocumentStyles.DockingWindow);
			menuItemDockingSdi.Checked = (newStyle == DocumentStyles.DockingSdi);
			menuItemSystemMdi.Checked = (newStyle == DocumentStyles.SystemMdi);
			menuItemLayoutByCode.Enabled = (newStyle != DocumentStyles.SystemMdi);
			menuItemLayoutByXml.Enabled = (newStyle != DocumentStyles.SystemMdi);
			toolBarButtonLayoutByCode.Enabled = (newStyle != DocumentStyles.SystemMdi);
			toolBarButtonLayoutByXml.Enabled = (newStyle != DocumentStyles.SystemMdi);
		}

		private void menuItemCloseAllButThisOne_Click(object sender, System.EventArgs e)
		{
			if (dockPanel.DocumentStyle == DocumentStyles.SystemMdi)
			{
				Form activeMdi = ActiveMdiChild;
				foreach (Form form in MdiChildren)
				{
					if (form != activeMdi)
						form.Close();
				}
			}
			else
			{
				IDockContent[] documents = dockPanel.Documents;

				foreach (IDockContent document in documents)
				{
					if (!document.DockHandler.IsActivated)
						document.DockHandler.Close();
				}
			}
		}

		private void menuItemShowDocumentIcon_Click(object sender, System.EventArgs e)
		{
			dockPanel.ShowDocumentIcon = menuItemShowDocumentIcon.Checked= !menuItemShowDocumentIcon.Checked;
		}
	}
}
