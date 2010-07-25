// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using NoGoop.Controls;
using NoGoop.Obj;
using NoGoop.ObjBrowser.GuiDesigner;
using NoGoop.ObjBrowser.Panels;
using NoGoop.ObjBrowser.TreeNodes;
using NoGoop.Util;

namespace NoGoop.ObjBrowser
{
	public class ObjectBrowser : UserControl, IDropTarget
	{
		const int    WIDTH_UNITS  = 5;
		const int    HEIGHT_UNITS = 6;
		string TOP_OBJ_NAME;
				
		internal const bool   INTERNAL = true;
		internal const string CODEBASE_VERSION = Version.VERSION;

		static ObjectBrowser _objBrowser;
		static NoGoop.Debug.Debugger _debugger;
		static string _helpFile;
				
		static BrowserTree _objTree;
		static Panel _objTreePanel;
		static ParamPanel _params;
		static EventLogList _eventLog;
		static TabControl _tabControl;
		static TreeView _mostRecentFocusedTree;
		static OutputList _outputList;
		static DesignerHost _designerHost;
		static ImagePanel _imagePanel;
		TabPage _gacTabPage;
		Panel _topPanel;
		Panel _statusPanel;
		Splitter _topBottomSplitter;
		Splitter _topTabSplitter;
		DetailPanel _detailPanel;
		PanelLabel _statusPanelLabel;
		TabPage _objTreeTabPage;

		internal static ParamPanel ParamPanel {
			get	{
				return _params;
			}
		}

		internal static BrowserTree ObjTree	{
			get	{
				return _objTree;
			}
		}

		internal static Panel ObjTreePanel {
			get	{
				return _objTreePanel;
			}
		}
		
		internal static TabControl TabControl {
			get	{
				return _tabControl;
			}
		}

		internal static DesignerHost DesignerHost {
			get	{
				return _designerHost;
			}
		}

		internal static ObjectBrowser ObjBrowser {
			get	{
				return _objBrowser;
			}
		}
		
		internal static NoGoop.Debug.Debugger Debugger {
			get	{
				return _debugger;
			}
			set {
				_debugger = value;
			}
		}
		
		internal static ImagePanel ImagePanel {
			get {
				return _imagePanel;
			}
		}

		internal static EventLogList EventLog {
			get {
				return _eventLog;
			}
		}
		
		internal static string HelpFile {
			get {
				return _helpFile;
			}
		}

		internal static TreeView MostRecentFocusedTree {
			get {
				return _mostRecentFocusedTree;
			}
		}

		public ObjectBrowser() : this(true, false)
		{
		}
		
		public ObjectBrowser(bool showStatusPanel, bool tabbedLayout)
		{
			TOP_OBJ_NAME = StringParser.Parse("${res:ComponentInspector.ObjectBrowser.TopLevelObjects}");
			int start = Environment.TickCount;
			_objBrowser = this;
			InitTypeHandlers();
			InitializeComponent(showStatusPanel, tabbedLayout);
			CreateControl();
			
			// Create the designer host after everything is setup
			_designerHost = new DesignerHost(_objTree, _imagePanel.DesignPanel);
			
			AssemblySupport.AddCurrentAssemblies();
			_helpFile = Directory.GetCurrentDirectory() + "\\CompInsp_" + "1" + ".chm";
			int timeTaken = Environment.TickCount - start;
		}

		static internal ObjectTreeNode GetTopLevelObjectNode()
		{
			return (ObjectTreeNode)_objTree.Nodes[0];
		}
		
		public bool CanDrop(DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				return true;

			return false;
		}

		public void DragEnterEvent(object sender, DragEventArgs e)
		{
			try	{
				if (CanDrop(e))	{
					e.Effect = DragDropEffects.Copy;
				}
			} catch (Exception ex){
				TraceUtil.WriteLineWarning(this, "Exception in event handler: " + ex);
			}
		}

		public void DragDropEvent(object sender, DragEventArgs e)
		{
			try	{
				if (CanDrop(e)) {
					string[] fileElements = (string[])e.Data.GetData(DataFormats.FileDrop);
					OpenFile(fileElements[0]);
				}
			} catch (Exception ex) {
				TraceUtil.WriteLineWarning(this, "Exception in event handler: " + ex);
			}
		}

		public void DragOverEvent(object sender, DragEventArgs e)
		{
			DragEnterEvent(sender, e);
		}
		
		/// <summary>
		/// Tries to open the specified file into the object browser.
		/// </summary>
		public bool OpenFile(string fileName)
		{
			Exception savedException = null;

			try {
				AssemblySupport.OpenFile(fileName);
				return true;
			} catch (Exception ex) {
				savedException = ex;
			}

			try {
				ComSupport.OpenFile(fileName);
				return true;
			} catch (Exception ex) {
				if (savedException == null)
					savedException = ex;
			}

			TraceUtil.WriteLineWarning(null, String.Concat("Error opening file ", fileName, ": " + savedException));
			ErrorDialog.Show(savedException,
				 "Error opening file " + fileName + "\n\n" + 
				 "The Inspector can only open .NET assemblies, ActiveX controls or ActiveX type libraries.",
				 "Error opening file " + fileName,
				 MessageBoxIcon.Error);
			return false;
		}
		
		public void CloseSelectedFile()
		{
			AssemblySupport.TypeTree.TreeNodePopupDelete(this, new EventArgs());
		}	
		
		void InitializeComponent(bool showStatusPanel, bool tabbedLayout)
		{
			SuspendLayout();
						
			CausesValidation = false;
			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			Name = "ObjectBrowserControl";
			Size = new Size(800, 700);
			
			// All of the dimensions are here
			int objTreeWidth = (int)(ClientSize.Width * 2/WIDTH_UNITS);
			int assyTreeWidth = (int)(ClientSize.Width * 3/WIDTH_UNITS);
			int paramsWidth = (int)(ClientSize.Width * 1/WIDTH_UNITS);
			int imageWidth = (int)(ClientSize.Width * 2/WIDTH_UNITS);
			int topHeight = (int)(ClientSize.Height * 2.5/HEIGHT_UNITS);
			int bottomHeight = (int)(ClientSize.Height * 2.5/HEIGHT_UNITS);
			int detailHeight = (int)(ClientSize.Height * 1/HEIGHT_UNITS);
			int objTreeHeight = ClientSize.Height - detailHeight;

			// Contents of treePanel, on the left
			_objTree = new BrowserTree(TOP_OBJ_NAME);

			// Hook up the routines that get called when preferences change
			ComponentInspectorProperties.ShowPropertyAccessorMethodsChanged += ObjectTreeInvalidated;
			ComponentInspectorProperties.ShowFieldsChanged += ObjectTreeInvalidated;
			ComponentInspectorProperties.ShowPropertiesChanged += ObjectTreeInvalidated;
			ComponentInspectorProperties.ShowMethodsChanged += ObjectTreeInvalidated;
			ComponentInspectorProperties.ShowEventsChanged += ObjectTreeInvalidated;
			ComponentInspectorProperties.ShowBaseClassesChanged += ObjectTreeInvalidated;
			ComponentInspectorProperties.ShowPublicMembersOnlyChanged += ObjectTreeInvalidated;
			ComponentInspectorProperties.ShowMemberCategoriesChanged += ObjectTreePreferencesChanged;
			ComponentInspectorProperties.ShowBaseCategoriesChanged += ObjectTreePreferencesChanged;
			ComponentInspectorProperties.CategoryThresholdChanged += ObjectTreePreferencesChanged;
			ComponentInspectorProperties.ShowBaseClassNamesChanged += ObjectTreeInvalidated;
			ComponentInspectorProperties.DisplayHexChanged += ObjectTreeInvalidated;
			ComponentInspectorProperties.ShowAssemblyPanelChanged += TabPanelChanged;
			ComponentInspectorProperties.ShowControlPanelChanged += TabPanelChanged;
			ComponentInspectorProperties.ShowGacPanelChanged += TabPanelChanged;
			ComponentInspectorProperties.TypeHandlerChanged += ObjectTreeInvalidated;
				
			ColumnHeader ch = new ColumnHeader();
			ch.Text = StringParser.Parse("${res:ComponentInspector.ObjectBrowser.ValueColumnHeader}");
			ch.TextAlign = HorizontalAlignment.Left;
			_objTree.Columns.Add(ch);
			_objTree.BorderStyle = BorderStyle.None;
			_objTree.AllowDrop = true;
			_objTree.IsObjectContainer = true;
			_objTree.IsDropTarget = true;
			_objTree.UseCompareTo = true;
			_objTree.GotFocus += new EventHandler(TreeFocused);
			UpdateObjectTreePreferences();

			_objTree.SetupPanel();
			_objTree.Panel.Dock = DockStyle.Fill;
			_objTree.Panel.Width = objTreeWidth;
			_objTree.Panel.Height = objTreeHeight;
			_objTree.Panel.BorderStyle = BorderStyle.None;

			_objTreePanel = new Panel();
			_objTreePanel.Dock = DockStyle.Left;
			_objTreePanel.Width = _objTree.Panel.Width;
			
			// Note we add the parent, because the tree comes with a
			// panel that's the parent of the tree
			_objTreePanel.Controls.Add(_objTree.Panel);
			_objTreePanel.BorderStyle = BorderStyle.Fixed3D;
			new PanelLabel(_objTreePanel, StringParser.Parse("${res:ComponentInspector.ObjectBrowser.ObjectsTreePanel}"));

			// Image panel
			_imagePanel =  NoGoop.ObjBrowser.ImagePanel.CreateImagePanel(objTreeWidth, !tabbedLayout);
			_imagePanel.WrapperPanel.Dock = DockStyle.Fill;

			// For text associated with each tree node
			_detailPanel = new DetailPanel();
			_detailPanel.Dock = DockStyle.Bottom;

			_params = new ParamPanel();
			_params.Dock = DockStyle.Fill;
			_params.Width = paramsWidth;
			
			_eventLog = new EventLogList(this);

			AssemblySupport.Init();
			AssemblySupport.AssyTree.GotFocus += new EventHandler(TreeFocused);
			
			// Splitter between main tree and the rest, vertical
			Splitter mainSplitter = new Splitter();
			mainSplitter.Dock = DockStyle.Left;
			mainSplitter.Width = Utils.SPLITTER_SIZE;

			Panel paramPanel = new Panel();
			paramPanel.Dock = DockStyle.Left;
			paramPanel.Width = _params.Width;
			paramPanel.Controls.Add(_params);
			paramPanel.BorderStyle = BorderStyle.Fixed3D;
			new PanelLabel(paramPanel, StringParser.Parse("${res:ComponentInspector.ObjectBrowser.ParametersPanel}"));

			Splitter propImageSplitter = new Splitter();
			propImageSplitter.Dock = DockStyle.Left;
			propImageSplitter.Width = Utils.SPLITTER_SIZE;

			// Contains the property panel and image panel
			Panel propImagePanel = new Panel();
			propImagePanel.Dock = DockStyle.Top;
			propImagePanel.Height = topHeight;
			propImagePanel.Controls.Add(_imagePanel.WrapperPanel);
			propImagePanel.Controls.Add(propImageSplitter);
			propImagePanel.Controls.Add(paramPanel);

			// Splitter between node details and the rest
			_topTabSplitter = new Splitter();
			_topTabSplitter.Dock = DockStyle.Top;
			_topTabSplitter.Height = Utils.SPLITTER_SIZE;

			GacList gacList = new GacList();
			gacList.Width = assyTreeWidth;
			gacList.Dock = DockStyle.Fill;
			gacList.BorderStyle = BorderStyle.None;
			_gacTabPage = new TabPage();
			_gacTabPage.Controls.Add(gacList);
			_gacTabPage.Text = "GAC";
			_gacTabPage.BorderStyle = BorderStyle.None;
			
			// Object tab page.
			if (tabbedLayout) {
				_objTreeTabPage = new TabPage();
				_objTreeTabPage.Controls.Add(_objTreePanel);
				_objTreeTabPage.Text = StringParser.Parse("${res:ComponentInspector.ObjectBrowser.ObjectsTreePanel}");
				_objTreeTabPage.BorderStyle = BorderStyle.None;
			}

			// Not presently used
			_outputList = new OutputList();
			_outputList.Width = assyTreeWidth;
			_outputList.Dock = DockStyle.Fill;
			_outputList.BorderStyle = BorderStyle.None;
			TabPage outputTabPage = new TabPage();
			outputTabPage.Controls.Add(_outputList);
			outputTabPage.Text = StringParser.Parse("${res:ComponentInspector.ObjectBrowser.OutputTab}");
			outputTabPage.BorderStyle = BorderStyle.None;
			
			_tabControl = new TabControl();
			_tabControl.Dock = DockStyle.Fill;
			_tabControl.Width = assyTreeWidth;
			_tabControl.SelectedIndexChanged += new EventHandler(TabIndexChangedHandler);

			// Contains the property panel and image panel
			Panel tabPanel = new Panel();
			tabPanel.Dock = DockStyle.Fill;
			if (tabbedLayout) {
				propImagePanel.Dock = DockStyle.Fill;
			} else {
				tabPanel.Controls.Add(_tabControl);
				tabPanel.Controls.Add(_topTabSplitter);
			}
			tabPanel.Controls.Add(propImagePanel);

			// All of the panels on the top
			_topPanel = new Panel();
			_topPanel.Dock = DockStyle.Fill;
			_topPanel.Height = topHeight + bottomHeight;
			_topPanel.Controls.Add(tabPanel);
			_topPanel.Controls.Add(mainSplitter);
			if (tabbedLayout) {
				_tabControl.Dock = DockStyle.Left;
				_objTreePanel.Dock = DockStyle.Fill;
				_topPanel.Controls.Add(_tabControl);
			} else {
				_topPanel.Controls.Add(_objTreePanel);
			}

			if (!tabbedLayout) {
				_topBottomSplitter = new Splitter();
				_topBottomSplitter.Dock = DockStyle.Bottom;
				_topBottomSplitter.Height = Utils.SPLITTER_SIZE;
				_topBottomSplitter.MinSize = detailHeight;
			}

			if (showStatusPanel) {
				_statusPanel = new StatusPanel();
				_statusPanelLabel = new PanelLabel(_statusPanel);
				_statusPanelLabel.Dock = DockStyle.Top;
			}

			Controls.Add(_topPanel);
			if (showStatusPanel) {
				Controls.Add(_statusPanelLabel);
			}
			if (!tabbedLayout) {
				Controls.Add(_topBottomSplitter);
				Controls.Add(_detailPanel);
			}
			
			// To allow file dropping
			DragEnter += new DragEventHandler(DragEnterEvent);
			DragDrop += new DragEventHandler(DragDropEvent);
			DragOver += new DragEventHandler(DragOverEvent);
			AllowDrop = true;
			
			_objTree.BeginUpdate();

			// Add top level nodes
			ArrayList tlList = new ArrayList();
			ObjectInfo objInfo = ObjectInfoFactory.GetObjectInfo(false, tlList);
			objInfo.ObjectName = TOP_OBJ_NAME;
			BrowserTreeNode node = new ObjectTreeNode(false, objInfo);

			// Make sure this is the first node
			node.NodeOrder = 0;
			node.AllowDelete = false;
			_objTree.Nodes.Add(node);

			// Just for testing
			if (LocalPrefs.Get(LocalPrefs.DEV) != null)
				tlList.Add(this);

			_objTree.EndUpdate();
			ComSupport.Init();
			ComSupport.ComTree.GotFocus += new EventHandler(TreeFocused);
			
			SetTabPanels();

			ResumeLayout();
		}
		
		void ObjectTreePreferencesChanged(object sender, EventArgs e)
		{
			UpdateObjectTreePreferences();
		}
		
		// Used when preferences change
		void UpdateObjectTreePreferences()
		{
			_objTree.IntermediateNodeThreshold = ComponentInspectorProperties.CategoryThreshold;
			_objTree.UseIntermediateNodes = ComponentInspectorProperties.ShowMemberCategories | 
				ComponentInspectorProperties.ShowBaseCategories;

			_objTree.InvalidateAll();
		}
		
		void ObjectTreeInvalidated(object source, EventArgs e)
		{
			_objTree.InvalidateAll();
		}
		
		/// <summary>
		/// Configured tab panels being displayed has been changed.
		/// </summary>
		void TabPanelChanged(object source, EventArgs e)
		{
			SetTabPanels();
		}
		
		void SetTabPanels()
		{
			_tabControl.Controls.Clear();
			
			// Set up the tabs in the right order
			if (_objTreeTabPage != null)
				_tabControl.Controls.Add(_objTreeTabPage);
			if (ComSupport.ComTabPage != null)
				_tabControl.Controls.Add(ComSupport.ComTabPage);
			if (ComponentInspectorProperties.ShowAssemblyPanel)
				_tabControl.Controls.Add(AssemblySupport.AssyTabPage);
			if (ComponentInspectorProperties.ShowControlPanel) {
				// Make sure the tree is up to date
				ControlTree.SetupControlTree();
				_tabControl.Controls.Add(AssemblySupport.ControlTabPage);
			}

			if (ComponentInspectorProperties.ShowGacPanel)
				_tabControl.Controls.Add(_gacTabPage);
			_tabControl.Controls.Add(_eventLog.EventLogTabPage);
			_tabControl.Controls.Add(_eventLog.EventsBeingLoggedTabPage);
		}
		
		void TreeFocused(object sender, EventArgs e)
		{
			_mostRecentFocusedTree = (TreeView)sender;
		}
		
		void TabIndexChangedHandler(object sender, EventArgs e)
		{
			DetailPanel.Clear();
			if (_tabControl.SelectedTab == null) {
				return;
			}

			Control tabPageContents = _tabControl.SelectedTab.Controls[0];
			if (tabPageContents is BrowserTree) {
				((BrowserTree)tabPageContents).DoTabSelected();
			}
		}
		
		/// <summary>
		/// Ensure that the tab control has at leasts its minimum height
		/// </summary>
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			Form parentForm = Parent as Form;
			if (parentForm != null && parentForm.WindowState != FormWindowState.Minimized) {
				if (_tabControl != null) {
					int diff = _tabControl.Height - _topTabSplitter.MinSize;
					if (diff < 0) {
						_topTabSplitter.SplitPosition -= diff;
						_tabControl.Height = _topTabSplitter.MinSize;
					}
				}
			}
		}
		
		void InitTypeHandlers()
		{
			ICollection typeHandlers = TypeHandlerManager.Instance.GetTypeHandlers();
			foreach (TypeHandlerManager.TypeHandlerInfo th in typeHandlers) {
				th._enabled = ComponentInspectorProperties.IsTypeHandlerEnabled(th._name);
			}
		}
	}
}

