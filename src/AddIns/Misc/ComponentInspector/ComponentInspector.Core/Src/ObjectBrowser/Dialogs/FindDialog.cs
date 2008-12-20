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
using NoGoop.ObjBrowser.TreeNodes;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.Dialogs
{
	public class FindDialog : Dialog
	{
		protected BrowserFinder     _finder;
		protected Label             _startingNode;
		protected ComboBox          _findWhat;
		protected RadioButton       _levelAll;
		protected RadioButton       _levelSelect;
		protected NumericTextBox    _levelSelectNum;

		protected RadioButton       _treeObj;
		protected RadioButton       _treeAssy;
		protected RadioButton       _treeAx;

		protected Panel             _objTreePanel;
		protected CheckBox          _objTreeName;
		protected CheckBox          _objTreeValue;
		protected Label             _objTreeLabel;

		protected RadioButton       _fullName;
		protected RadioButton       _startsWith;
		protected RadioButton       _contains;

		protected ListView          _foundList;
		protected ColumnHeader      _foundListColumn;

		protected Label             _lookingLabel;
		protected Label             _lookingNode;
		protected int               _lookingNodeCount;

		protected Button            _findButton;
		protected Button            _closeButton;
		protected Button            _cancelButton;
		protected Button            _helpButton;

		// Delegates
		protected SearchNodeDelegate _nodeFound;
		protected SearchNodeDelegate _nodeLooking;
		protected SearchStatusDelegate _searchStatus;
		protected SearchInvalidateDelegate _searchInvalidate;
		
		protected bool              _cancelled;
		static FindDialog           _instance;

		public FindDialog() : base(!INCLUDE_BUTTONS)
		{
			Text = StringParser.Parse("${res:ComponentInspector.FindDialog.Title}");
			FormBorderStyle = FormBorderStyle.SizableToolWindow;
			ControlBox = false;
			StartPosition = FormStartPosition.CenterParent;
			ShowInTaskbar = false;

			Label label;
			Panel panel;

			AutoScaleBaseSize = new Size(5, 13);
			ClientSize = new Size(540, 333);
			MinimumSize = new Size(536, 296);

			_nodeFound = new SearchNodeDelegate(NodeFound);
			_nodeLooking = new SearchNodeDelegate(NodeLooking);
			_searchStatus = new SearchStatusDelegate(SearchStatus);
			_searchInvalidate = new SearchInvalidateDelegate(SearchInvalidate);

			SuspendLayout();

			// So we can make the list column change when the form
			// is resized
			Layout += new LayoutEventHandler(LayoutHandler);

			Activated += new EventHandler(ActivateHandler);

			//
			// Current tree node
			//
			label = new Label();
			label.Location = new Point(8, 16);
			label.Size = new Size(72, 30);
			label.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.StartingWithLabel}");
			label.TextAlign = ContentAlignment.TopRight;
			Controls.Add(label);

			_startingNode = new Label();
			_startingNode.Location = new Point(80, 16);
			_startingNode.Size = new Size(360, 30);
			_startingNode.TextAlign = ContentAlignment.TopLeft;
			_startingNode.Font = new Font(_startingNode.Font, FontStyle.Bold);
			_startingNode.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			Controls.Add(_startingNode);

			//
			// Find text combo
			//
			label = new Label();
			label.Location = new Point(8, 48);
			label.Size = new Size(72, 13);
			label.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.FindWhatLabel}");
			label.TextAlign = ContentAlignment.MiddleRight;
			Controls.Add(label);

			_findWhat = new ComboBox();
			_findWhat.Location = new Point(80, 48);
			_findWhat.Size = new Size(304, 21);
			_findWhat.TabIndex = 0;
			_findWhat.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			_findWhat.KeyPress += new KeyPressEventHandler(FindWhatKeyHandler);
			Controls.Add(_findWhat);

			Panel selectionPanel = new Panel();
			selectionPanel.Location = new Point(8, 80);
			selectionPanel.Size = new Size(430, 75);
			Controls.Add(selectionPanel);

			//
			// Tree selection panel
			//
			label = new Label();
			label.AutoSize = true;
			label.Location = new Point(10, 0);
			label.Size = new Size(40, 13);
			label.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.FindIn}");
			label.TextAlign = ContentAlignment.MiddleRight;
			selectionPanel.Controls.Add(label);

			panel = new Panel();
			panel.Location = new Point(0, 8);
			panel.Size = new Size(120, 64);
			panel.BorderStyle = BorderStyle.Fixed3D;
			panel.TabIndex = 30;
			selectionPanel.Controls.Add(panel);

			_treeObj = new RadioButton();
			_treeObj.Location = new Point(5, 8);
			_treeObj.Size = new Size(120, 15);
			_treeObj.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.ObjectTreeRadioButtonText}");
			_treeObj.TabIndex = 0;
			_treeObj.Click += new EventHandler(TreeSelectClick);
			panel.Controls.Add(_treeObj);

			_treeAssy = new RadioButton();
			_treeAssy.Location = new Point(5, 23);
			_treeAssy.Size = new Size(120, 15);
			_treeAssy.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.AssembliesRadioButton}");
			_treeAssy.TabIndex = 0;
			_treeAssy.Click += new EventHandler(TreeSelectClick);
			panel.Controls.Add(_treeAssy);

			_treeAx = new RadioButton();
			_treeAx.Location = new Point(5, 38);
			_treeAx.Size = new Size(120, 15);
			_treeAx.Text = "ActiveX/COM";
			_treeAx.TabIndex = 0;
			_treeAx.Click += new EventHandler(TreeSelectClick);
			panel.Controls.Add(_treeAx);

			//
			// Level selection panel
			//
			label = new Label();
			label.AutoSize = true;
			label.Location = new Point(135, 0);
			label.Size = new Size(40, 13);
			label.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.LevelsLabel}");
			label.TextAlign = ContentAlignment.MiddleRight;
			selectionPanel.Controls.Add(label);

			panel = new Panel();
			panel.Location = new Point(125, 8);
			panel.Size = new Size(110, 64);
			panel.BorderStyle = BorderStyle.Fixed3D;
			panel.TabIndex = 35;
			selectionPanel.Controls.Add(panel);

			_levelAll = new RadioButton();
			_levelAll.Location = new Point(5, 8);
			_levelAll.Size = new Size(110, 15);
			_levelAll.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.AllLevelsLabel}");
			_levelAll.Checked = true;
			_levelAll.TabIndex = 0;
			panel.Controls.Add(_levelAll);

			_levelSelect = new RadioButton();
			_levelSelect.Location = new Point(5, 23);
			_levelSelect.Size = new Size(80, 15);
			_levelSelect.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.LevelSelectOnlyDownLabel}");
			_levelSelect.TabIndex = 1;
			panel.Controls.Add(_levelSelect);

			_levelSelectNum = new NumericTextBox();
			_levelSelectNum.Location = new Point(20, 38);
			_levelSelectNum.Width = 25;
			_levelSelectNum.Height = 25;
			_levelSelectNum.Text = "2";
			panel.Controls.Add(_levelSelectNum);

			label = new Label();
			label.Location = new Point(48, 38);
			label.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.LevelsLabel}");
			panel.Controls.Add(label);

			//
			// Object tree options
			//
			_objTreeLabel = new Label();
			_objTreeLabel.Location = new Point(250, 0);
			_objTreeLabel.Size = new Size(64, 16);
			_objTreeLabel.TabIndex = 17;
			_objTreeLabel.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.ObjectTreeRadioButtonText}");
			selectionPanel.Controls.Add(_objTreeLabel);

			_objTreePanel = new Panel();
			_objTreePanel.Location = new Point(240, 8);
			_objTreePanel.Size = new Size(88, 64);
			_objTreePanel.TabIndex = 16;
			_objTreePanel.BorderStyle = BorderStyle.Fixed3D;
			_objTreePanel.TabIndex = 40;
			selectionPanel.Controls.Add(_objTreePanel);

			_objTreeName = new CheckBox();
			_objTreeName.Location = new Point(5, 8);
			_objTreeName.Size = new Size(110, 15);
			_objTreeName.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.ObjectTreeNameCheckBox}");
			_objTreeName.Checked = true;
			_objTreeName.TabIndex = 0;
			_objTreePanel.Controls.Add(_objTreeName);

			_objTreeValue = new CheckBox();
			_objTreeValue.Location = new Point(5, 23);
			_objTreeValue.Size = new Size(110, 15);
			_objTreeValue.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.ObjectTreeValueCheckBox}");
			_objTreeValue.TabIndex = 1;
			_objTreePanel.Controls.Add(_objTreeValue);

			//
			// Matching options panel
			//
			label = new Label();
			label.AutoSize = true;
			label.Location = new Point(343, 0);
			label.Size = new Size(35, 13);
			label.TabIndex = 7;
			label.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.MatchLabel}");
			selectionPanel.Controls.Add(label);

			panel = new Panel();
			panel.BorderStyle = BorderStyle.Fixed3D;
			panel.Location = new Point(333, 8);
			panel.Size = new Size(96, 64);
			panel.TabIndex = 45;
			selectionPanel.Controls.Add(panel);

			_fullName = new RadioButton();
			_fullName.Location = new Point(5, 8);
			_fullName.Size = new Size(110, 15);
			_fullName.Checked = true;
			_fullName.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.FullNameLabel}");
			_fullName.TabIndex = 1;
			panel.Controls.Add(_fullName);
			
			_startsWith = new RadioButton();
			_startsWith.Location = new Point(5, 23);
			_startsWith.Size = new Size(110, 15);
			_startsWith.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.StartsWithRadioButton}");
			_startsWith.TabIndex = 1;
			panel.Controls.Add(_startsWith);

			_contains = new RadioButton();
			_contains.Location = new Point(5, 38);
			_contains.Size = new Size(110, 15);
			_contains.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.ContainsRadioButton}");
			_contains.TabIndex = 1;
			panel.Controls.Add(_contains);

			// Border
			label = new Label();
			label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			label.Location = new Point(8, 158);
			label.Size = new Size(ClientSize.Width - 16, 2);
			label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			label.TabIndex = 10;
			Controls.Add(label);

			// Found list
			_foundList = new ListView();
			_foundList.Location = new Point(8, 166);
			_foundList.Size = new Size(ClientSize.Width - 16, ClientSize.Height - 166 - 8 - 21);
			_foundList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			_foundList.TabIndex = 50;
			_foundList.FullRowSelect = true;
			_foundList.MultiSelect = false;
			_foundList.SmallImageList = PresentationMap.ImageList;
			_foundList.View = View.Details;
			_foundList.ItemActivate += new EventHandler(ShowClick);

			MenuItem mi = new MenuItem();
			mi.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.ShowItemMenuItem}");
			mi.Click += new EventHandler(ShowClick);
			_foundList.ContextMenu = new ContextMenu();
			_foundList.ContextMenu.MenuItems.Add(mi);

			_foundListColumn = new ColumnHeader();
			_foundListColumn.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.FoundListColumnHeader}");
			_foundListColumn.TextAlign = HorizontalAlignment.Left;
			_foundListColumn.Width = _foundList.ClientSize.Width;
			_foundList.Columns.Add(_foundListColumn);
			Controls.Add(_foundList);

			//
			// Current tree node
			//
			_lookingLabel = new Label();
			_lookingLabel.Location = new Point(8, ClientSize.Height - 21);
			_lookingLabel.Size = new Size(72, 13);
			_lookingLabel.TextAlign = ContentAlignment.MiddleLeft;
			_lookingLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			Controls.Add(_lookingLabel);

			_lookingNode = new Label();
			_lookingNode.Location = new Point(80, ClientSize.Height - 21);
			_lookingNode.Size = new Size(360, 13);
			_lookingNode.TextAlign = ContentAlignment.MiddleLeft;
			_lookingNode.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			Controls.Add(_lookingNode);

			//
			// Buttons
			//
			_findButton = new Button();
			_findButton.Location = new Point(ClientSize.Width - _findButton.Width - 8, 16);
			_findButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			_findButton.TabIndex = 5;
			_findButton.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.FindButton}");
			_findButton.Click += new EventHandler(FindClick);
			Controls.Add(_findButton);

			_closeButton = new Button();
			_closeButton.Location = new Point(ClientSize.Width - _closeButton.Width - 8, 48);
			_closeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			_closeButton.TabIndex = 10;
			_closeButton.Text = StringParser.Parse("${res:Global.CloseButtonText}");
			_closeButton.Click += new EventHandler(CloseClick);
			Controls.Add(_closeButton);

			_cancelButton = new Button();
			_cancelButton.Location = new Point(ClientSize.Width - _cancelButton.Width - 8, 80);
			_cancelButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			_cancelButton.TabIndex = 15;
			_cancelButton.Text = StringParser.Parse("${res:Global.CloseButtonText}");
			_cancelButton.Click += new EventHandler(CancelClick);
			Controls.Add(_cancelButton);

			_helpButton = new Button();
			_helpButton.Location = new Point(ClientSize.Width - _helpButton.Width - 8, 112);
			_helpButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			_helpButton.TabIndex = 20;
			_helpButton.Text = StringParser.Parse("${res:Global.HelpButtonText}");
			_helpButton.Click += new EventHandler(HelpClick);
			Controls.Add(_helpButton);

			ResumeLayout();
		}
		
		public static void DoShowDialog()
		{
			if (_instance != null) {
				_instance.BringToFront();
				return;
			} else {
				_instance = new FindDialog();
				_instance.ShowDialogInternal();
			}
		}
		
		void ShowDialogInternal()
		{
			TreeNode node = GetFirstSelectedNode();
			if (node == null) {
				Hide();
				_instance = null;
				ErrorDialog.Show(StringParser.Parse("${res:ComponentInspector.FindDialog.NoTreeNodeSelectedMessage}"),
				                 StringParser.Parse("${res:ComponentInspector.FindDialog.NoTreeNodeSelectedDialogTitle}"),
				                 MessageBoxIcon.Error);
				return;
			}

			_startingNode.Text = node.FullPath;
			_startingNode.Tag = node;

			SetButtons(false);
			Show();
		}
		
		protected void SetButtons(bool searching)
		{
			_cancelButton.Enabled = searching;
			_findButton.Enabled = !searching;
			_closeButton.Enabled = !searching;
			_helpButton.Enabled = !searching;
		}
		
		protected void FindWhatKeyHandler(Object sender, KeyPressEventArgs e)
		{
			// Return initiates the find
			if (e.KeyChar == (char)13) {
				e.Handled = true;
				FindClick(null, null);
			}
		}

		protected void FindClick(Object sender, EventArgs e)
		{
			SetButtons(true);
			_cancelled = false;

			SearchInvalidate(null);

			TraceUtil.WriteLineInfo(this, "Looking for: " + _findWhat.Text);
			
			int compareType = 0;
			if (_fullName.Checked)
				compareType = BrowserFinder.COMPARE_FULL;
			else if (_startsWith.Checked)
				compareType = BrowserFinder.COMPARE_STARTS;
			else if (_contains.Checked)
				compareType = BrowserFinder.COMPARE_CONTAINS;

			bool useName = true;
			bool useValue = false;

			TreeView tree = null;
			if (_treeObj.Checked) {
				tree = ObjectBrowser.ObjTree;
				useName = _objTreeName.Checked;
				useValue = _objTreeValue.Checked;
			} else if (_treeAssy.Checked) {
				tree = AssemblySupport.AssyTree;
			} else if (_treeAx.Checked) {
				tree = ComSupport.ComTree;
			}

			// Got to have one of them
			if (!(useName || useValue)) {
				ErrorDialog.Show
					(StringParser.Parse("${res:ComponentInspector.FindDialog.SelectNameOrValueMessage}"),
					 StringParser.Parse("${res:ComponentInspector.FindDialog.SelectNameOrValueDialogTitle}"),
					 MessageBoxIcon.Error);
				SetButtons(false);
				return;
			}

			int maxLevel;
			if (_levelAll.Checked)
				maxLevel = BrowserFinder.ALL_LEVELS;
			else if (!Int32.TryParse(_levelSelectNum.Text, out maxLevel)) {
				ErrorDialog.Show
					("Please input a valid number for the number of levels to search.",
					 String.Empty, 
					 MessageBoxIcon.Error);
			}
			
			_finder = new BrowserFinder
				((String)_findWhat.Text,
				 compareType,
				 maxLevel,
				 useName,
				 useValue,
				 (BrowserTreeNode)_startingNode.Tag,
				 _nodeFound,
				 _nodeLooking,
				 _searchStatus,
				 _searchInvalidate);

			tree.BeginUpdate();

			_finder.Search();
			
			tree.EndUpdate();

			if (_foundList.Items.Count == 0) {
				ListViewItem li = new ListViewItem();
				li.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.NoItemsFoundMessage}");
				_foundList.Items.Add(li);
			}

			// Save the last search, only if its different than
			// what has already been saved
			bool found = false;
			foreach (String s in _findWhat.Items) {
				if (s.Equals(_findWhat.Text)) {
					found = true;
					break;
				}
			}
			if (!found)
				_findWhat.Items.Insert(0, _findWhat.Text);

			_lookingNodeCount = 0;
			_lookingLabel.Text = null;
			_lookingNode.Text = null;
			SetButtons(false);
		}


		// Removes all of the found nodes if the tree has changed
		internal void SearchInvalidate(BrowserFinder finder)
		{
			_foundList.Items.Clear();
			// Make sure the list shows as empty
			Application.DoEvents();
		}

		internal bool NodeFound(BrowserFinder finder, ISearchNode node)
		{
			if (TraceUtil.If(this, TraceLevel.Verbose)) {
				Trace.WriteLine("node found: " + node.GetType().Name
				                + " " + finder.GetFullName());
			}

			ListViewItem li = new ListViewItem();
			li.Text = finder.GetFullName();
			li.Tag = node.GetSearchMaterializer(finder);
			li.ImageIndex = node.GetImageIndex();
			_foundList.Items.Add(li);

			// Show it as its found
			Application.DoEvents();

			return true;
		}

		internal void SearchStatus(String action, String obj)
		{
			_lookingLabel.Text = action;
			_lookingNode.Text = obj;
			Application.DoEvents();
		}

		// Returns true if the search is to continue
		internal bool NodeLooking(BrowserFinder finder, ISearchNode node)
		{
			if (TraceUtil.If(this, TraceLevel.Verbose)) {
				Trace.WriteLine("node looking: " + node.GetType().Name
				                + " " + finder.GetFullName());
			}

			if (_cancelled)
				return false;

			_lookingNodeCount++;

			// Show it as its found
			if ((_lookingNodeCount % 10) == 0) {
				_lookingLabel.Text = StringParser.Parse("${res:ComponentInspector.FindDialog.LookingAtLabel}");
				_lookingNode.Text = finder.GetFullName();
				Application.DoEvents();
			}

			return true;
		}

		protected void CloseClick(Object sender, EventArgs e)
		{
			Close();
		}

		protected void CancelClick(Object sender, EventArgs e)
		{
			_cancelled = true;
		}

		protected void HelpClick(Object sender, EventArgs e)
		{
			Help.ShowHelp(this, ObjectBrowser.HelpFile,
			              HelpNavigator.Topic, "Find_"
			              + "1"
			              + ".html");
		}


		protected void ShowClick(Object sender, EventArgs e)
		{
			if (_foundList.SelectedItems.Count == 0)
				return;

			// Materialize can sometimes take a while
			Cursor save = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			ISearchMaterializer searchMaterializer = _foundList.SelectedItems[0].Tag as ISearchMaterializer;
			if (searchMaterializer != null) {
				searchMaterializer.PointToNode();
				Close();
			}
			Cursor.Current = save;
		}

		protected void LayoutHandler(object sender, LayoutEventArgs e)
		{
			_foundListColumn.Width = _foundList.ClientSize.Width;
		}

		// When this dialog is activated make sure we point to
		// the find what combo box
		protected void ActivateHandler(object sender, EventArgs e)
		{
			_findWhat.Focus();
		}

		// Gets the selected tree node of the possible trees
		// in order to determine the initial setting of the
		// selected tree
		protected TreeNode GetFirstSelectedNode()
		{
			if (ObjectBrowser.MostRecentFocusedTree == ObjectBrowser.ObjTree) {
				_treeObj.Checked = true;
				TreeSelectClick(_treeObj, null);
				return ObjectBrowser.ObjTree.SelectedNode;
			}

			if (ObjectBrowser.MostRecentFocusedTree == ComSupport.ComTree) {
				_treeAx.Checked = true;
				TreeSelectClick(_treeAx, null);
				return ComSupport.ComTree.SelectedNode;
			}

			if (ObjectBrowser.MostRecentFocusedTree == AssemblySupport.AssyTree) {
				_treeAssy.Checked = true;
				TreeSelectClick(_treeAssy, null);
				return AssemblySupport.AssyTree.SelectedNode;
			}

			// no nodes selected in any tree
			return null;
		}

		protected void TreeSelectClick(Object sender, EventArgs e)
		{
			TreeView tree = null;
			String treeName = null;
			
			if (sender == _treeObj) {
				_objTreePanel.Enabled = true;
				_objTreeLabel.Enabled = true;
				tree = ObjectBrowser.ObjTree;
				treeName = "Object";
			} else if (sender == _treeAssy) {
				_objTreePanel.Enabled = false;
				_objTreeLabel.Enabled = false;
				tree = AssemblySupport.AssyTree;
				treeName = StringParser.Parse("${res:ComponentInspector.FindDialog.AssembliesRadioButton}");
			} else if (sender == _treeAx) {
				_objTreePanel.Enabled = false;
				_objTreeLabel.Enabled = false;
				tree = ComSupport.ComTree;
				treeName = "ActiveX/COM";
			}

			if (tree.SelectedNode != null) {
				_startingNode.Text = tree.SelectedNode.FullPath;
				_startingNode.Tag = tree.SelectedNode;
			} else {
				Close();
				ErrorDialog.Show(String.Format(StringParser.Parse("${res:ComponentInspector.FindDialog.SelectNodeInTreeNameMessage}"), treeName),
				                StringParser.Parse("${res:ComponentInspector.FindDialog.NoTreeNodeSelectedDialogTitle}"),
				                 MessageBoxIcon.Error);
			}
		}
		
		protected override void OnClosed(EventArgs e)
		{
			_instance = null;
			base.OnClosed(e);
			Dispose();
		}
	}
}
