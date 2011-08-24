// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	public class XPathQueryControl : System.Windows.Forms.UserControl, IMementoCapable
	{
		const int ErrorImageIndex = 0;
		const string NamespacesProperty = "Namespaces";
		const string PrefixColumnWidthProperty = "NamespacesDataGridView.PrefixColumn.Width";
		const string MatchColumnWidthProperty = "XPathResultsListView.MatchColumn.Width";
		const string LineColumnWidthProperty = "XPathResultsListView.LineColumn.Width";
		const string XPathComboBoxTextProperty = "XPathQuery.LastQuery";
		const string XPathComboBoxItemsProperty = "XPathQuery.History";
		
		/// <summary>
		/// The filename that the last query was executed on.
		/// </summary>
		string fileName = string.Empty;
		
		/// <summary>
		/// The total number of xpath queries to remember.
		/// </summary>
		const int xpathQueryHistoryLimit = 20;
		
		bool ignoreXPathTextChanges;
		
		enum MoveCaret {
			ByJumping = 1,
			ByScrolling = 2
		}
		
		public XPathQueryControl()
		{
			InitializeComponent();
			InitStrings();
			InitImageList();
			xpathComboBox.KeyDown += XPathComboBoxKeyDown;
			InitAutoCompleteMode();
		}
		
		/// <summary>
		/// Adds a namespace to the namespace list.
		/// </summary>
		public void AddNamespace(string prefix, string uri)
		{
			namespacesDataGridView.Rows.Add(new object[] {prefix, uri});
		}
		
		/// <summary>
		/// Gets the list of namespaces in the namespace list.
		/// </summary>
		public XmlNamespaceCollection GetNamespaces()
		{
			XmlNamespaceCollection namespaces = new XmlNamespaceCollection();
			for (int i = 0; i < namespacesDataGridView.Rows.Count - 1; ++i) {
				DataGridViewRow row = namespacesDataGridView.Rows[i];
				string prefix = GetPrefix(row);
				string uri = GetNamespace(row);
				if (prefix.Length == 0 && uri.Length == 0) {
					// Ignore.
				} else {
					namespaces.Add(new XmlNamespace(prefix, uri));
				}
			}
			return namespaces;
		}
		
		public DataGridView NamespacesDataGridView {
			get { return namespacesDataGridView; }
		}
		
		public ListView XPathResultsListView {
			get { return xPathResultsListView; }
		}
		
		public ComboBox XPathComboBox {
			get { return xpathComboBox; }
		}
		
		/// <summary>
		/// Creates a properties object that contains the current state of the
		/// control.
		/// </summary>
		public Properties CreateMemento()
		{
			Properties properties = new Properties();
			
			SaveNamespaces(properties);
			SaveNamespaceDataGridColumnWidths(properties);
			SaveXPathResultsListViewColumnWidths(properties);
			SaveXPathQueryHistory(properties);
			
			return properties;
		}
		
		void SaveNamespaces(Properties properties)
		{
			properties.Set(NamespacesProperty, GetNamespaceStringArray());
		}
		
		void SaveNamespaceDataGridColumnWidths(Properties properties)
		{
			properties.Set<int>(PrefixColumnWidthProperty, prefixColumn.Width);
		}
		
		void SaveXPathResultsListViewColumnWidths(Properties properties)
		{
			properties.Set<int>(MatchColumnWidthProperty, matchColumnHeader.Width);
			properties.Set<int>(LineColumnWidthProperty, lineColumnHeader.Width);
		}
		
		void SaveXPathQueryHistory(Properties properties)
		{
			properties.Set(XPathComboBoxTextProperty, XPathComboBox.Text);
			properties.Set(XPathComboBoxItemsProperty, GetXPathHistory());
		}
		
		/// <summary>
		/// Reloads the state of the control.
		/// </summary>
		public void SetMemento(Properties properties)
		{
			ignoreXPathTextChanges = true;
			
			try {
				LoadNamespaces(properties);
				LoadNamespaceDataGridColumnWidths(properties);
				LoadXPathResultsListViewColumnWidths(properties);
				LoadXPathQueryHistory(properties);
			} finally {
				ignoreXPathTextChanges = false;
			}
		}
		
		void LoadNamespaces(Properties properties)
		{
			string[] namespaces = properties.Get(NamespacesProperty, new string[0]);
			foreach (string ns in namespaces) {
				XmlNamespace xmlNamespace = XmlNamespace.FromString(ns);
				AddNamespace(xmlNamespace.Prefix, xmlNamespace.Name);
			}
		}
		
		void LoadNamespaceDataGridColumnWidths(Properties properties)
		{
			prefixColumn.Width = properties.Get<int>(PrefixColumnWidthProperty, 50);
		}
		
		void LoadXPathResultsListViewColumnWidths(Properties properties)
		{
			matchColumnHeader.Width = properties.Get<int>(MatchColumnWidthProperty, 432);
			lineColumnHeader.Width = properties.Get<int>(LineColumnWidthProperty, 60);
		}
		
		void LoadXPathQueryHistory(Properties properties)
		{
			XPathComboBox.Text = properties.Get(XPathComboBoxTextProperty, string.Empty);
			string[] xpaths = properties.Get(XPathComboBoxItemsProperty, new string[0]);
			foreach (string xpath in xpaths) {
				xpathComboBox.Items.Add(xpath);
			}
		}
		
		/// <summary>
		/// Called when the active workbench window has changed.
		/// </summary>
		public void ActiveWindowChanged()
		{
			UpdateQueryButtonState();
		}
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		#region Forms Designer generated code
		
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		System.ComponentModel.IContainer components = null;

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.xPathLabel = new System.Windows.Forms.Label();
			this.xpathComboBox = new System.Windows.Forms.ComboBox();
			this.queryButton = new System.Windows.Forms.Button();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.xPathResultsTabPage = new System.Windows.Forms.TabPage();
			this.xPathResultsListView = new System.Windows.Forms.ListView();
			this.matchColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.lineColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.namespacesTabPage = new System.Windows.Forms.TabPage();
			this.namespacesDataGridView = new System.Windows.Forms.DataGridView();
			this.prefixColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.namespaceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.tabControl.SuspendLayout();
			this.xPathResultsTabPage.SuspendLayout();
			this.namespacesTabPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.namespacesDataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// xPathLabel
			// 
			this.xPathLabel.Location = new System.Drawing.Point(3, 3);
			this.xPathLabel.Name = "xPathLabel";
			this.xPathLabel.Size = new System.Drawing.Size(46, 19);
			this.xPathLabel.TabIndex = 0;
			this.xPathLabel.Text = "XPath:";
			this.xPathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// xPathComboBox
			// 
			this.xpathComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			                                                                  | System.Windows.Forms.AnchorStyles.Right)));
			this.xpathComboBox.FormattingEnabled = true;
			this.xpathComboBox.Location = new System.Drawing.Point(55, 3);
			this.xpathComboBox.Name = "xPathComboBox";
			this.xpathComboBox.Size = new System.Drawing.Size(438, 21);
			this.xpathComboBox.TabIndex = 1;
			this.xpathComboBox.TextChanged += new System.EventHandler(this.XPathComboBoxTextChanged);
			this.xpathComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.XPathComboBoxKeyDown);
			// 
			// queryButton
			// 
			this.queryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.queryButton.Enabled = false;
			this.queryButton.Location = new System.Drawing.Point(499, 3);
			this.queryButton.Name = "queryButton";
			this.queryButton.Size = new System.Drawing.Size(70, 23);
			this.queryButton.TabIndex = 2;
			this.queryButton.Text = "Query";
			this.queryButton.UseVisualStyleBackColor = true;
			this.queryButton.Click += new System.EventHandler(this.QueryButtonClick);
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			                                                                | System.Windows.Forms.AnchorStyles.Left)
			                                                               | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.xPathResultsTabPage);
			this.tabControl.Controls.Add(this.namespacesTabPage);
			this.tabControl.Location = new System.Drawing.Point(0, 30);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(572, 208);
			this.tabControl.TabIndex = 3;
			// 
			// xPathResultsTabPage
			// 
			this.xPathResultsTabPage.Controls.Add(this.xPathResultsListView);
			this.xPathResultsTabPage.Location = new System.Drawing.Point(4, 22);
			this.xPathResultsTabPage.Name = "xPathResultsTabPage";
			this.xPathResultsTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.xPathResultsTabPage.Size = new System.Drawing.Size(564, 182);
			this.xPathResultsTabPage.TabIndex = 0;
			this.xPathResultsTabPage.Text = "Results";
			this.xPathResultsTabPage.UseVisualStyleBackColor = true;
			// 
			// xPathResultsListView
			// 
			this.xPathResultsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			                                           	this.matchColumnHeader,
			                                           	this.lineColumnHeader});
			this.xPathResultsListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.xPathResultsListView.FullRowSelect = true;
			this.xPathResultsListView.HideSelection = false;
			this.xPathResultsListView.Location = new System.Drawing.Point(3, 3);
			this.xPathResultsListView.MultiSelect = false;
			this.xPathResultsListView.Name = "xPathResultsListView";
			this.xPathResultsListView.Size = new System.Drawing.Size(558, 176);
			this.xPathResultsListView.SmallImageList = this.imageList;
			this.xPathResultsListView.TabIndex = 0;
			this.xPathResultsListView.UseCompatibleStateImageBehavior = false;
			this.xPathResultsListView.View = System.Windows.Forms.View.Details;
			this.xPathResultsListView.ItemActivate += new System.EventHandler(this.XPathResultsListViewItemActivate);
			this.xPathResultsListView.SelectedIndexChanged += new System.EventHandler(this.XPathResultsListViewSelectedIndexChanged);
			this.xPathResultsListView.Click += new System.EventHandler(this.XPathResultsListViewClick);
			// 
			// matchColumnHeader
			// 
			this.matchColumnHeader.Text = "Match";
			this.matchColumnHeader.Width = 432;
			// 
			// lineColumnHeader
			// 
			this.lineColumnHeader.Text = "Line";
			// 
			// imageList
			// 
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// namespacesTabPage
			// 
			this.namespacesTabPage.Controls.Add(this.namespacesDataGridView);
			this.namespacesTabPage.Location = new System.Drawing.Point(4, 22);
			this.namespacesTabPage.Name = "namespacesTabPage";
			this.namespacesTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.namespacesTabPage.Size = new System.Drawing.Size(564, 182);
			this.namespacesTabPage.TabIndex = 1;
			this.namespacesTabPage.Text = "Namespaces";
			this.namespacesTabPage.UseVisualStyleBackColor = true;
			// 
			// namespacesDataGridView
			// 
			this.namespacesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.namespacesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
			                                             	this.prefixColumn,
			                                             	this.namespaceColumn});
			this.namespacesDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.namespacesDataGridView.Location = new System.Drawing.Point(3, 3);
			this.namespacesDataGridView.MultiSelect = false;
			this.namespacesDataGridView.Name = "namespacesDataGridView";
			this.namespacesDataGridView.RowHeadersWidth = 25;
			this.namespacesDataGridView.ShowEditingIcon = false;
			this.namespacesDataGridView.Size = new System.Drawing.Size(558, 176);
			this.namespacesDataGridView.TabIndex = 0;
			// 
			// prefixColumn
			// 
			this.prefixColumn.HeaderText = "Prefix";
			this.prefixColumn.Name = "prefixColumn";
			this.prefixColumn.Width = 50;
			// 
			// namespaceColumn
			// 
			this.namespaceColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.namespaceColumn.HeaderText = "Namespace";
			this.namespaceColumn.Name = "namespaceColumn";
			// 
			// XPathQueryControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.queryButton);
			this.Controls.Add(this.xpathComboBox);
			this.Controls.Add(this.xPathLabel);
			this.Name = "XPathQueryControl";
			this.Size = new System.Drawing.Size(572, 238);
			this.tabControl.ResumeLayout(false);
			this.xPathResultsTabPage.ResumeLayout(false);
			this.namespacesTabPage.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.namespacesDataGridView)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.DataGridViewTextBoxColumn namespaceColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn prefixColumn;
		private System.Windows.Forms.DataGridView namespacesDataGridView;
		private System.Windows.Forms.ColumnHeader lineColumnHeader;
		private System.Windows.Forms.ColumnHeader matchColumnHeader;
		private System.Windows.Forms.ListView xPathResultsListView;
		private System.Windows.Forms.TabPage namespacesTabPage;
		private System.Windows.Forms.TabPage xPathResultsTabPage;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.Button queryButton;
		private System.Windows.Forms.ComboBox xpathComboBox;
		private System.Windows.Forms.Label xPathLabel;
		
		#endregion
		
		void XPathComboBoxTextChanged(object sender, EventArgs e)
		{
			if (!ignoreXPathTextChanges) {
				UpdateQueryButtonState();
			}
		}
		
		void UpdateQueryButtonState()
		{
			queryButton.Enabled = IsXPathQueryEntered && XmlDisplayBinding.XmlViewContentActive;
		}
		
		bool IsXPathQueryEntered {
			get { return xpathComboBox.Text.Length > 0; }
		}
		
		void QueryButtonClick(object sender, EventArgs e)
		{
			RunXPathQuery();
		}
		
		void RunXPathQuery()
		{
			XmlView xmlView = XmlView.ActiveXmlView;
			if (xmlView == null) {
				return;
			}
			
			try {
				fileName = xmlView.File.FileName;
				
				// Clear previous XPath results.
				ClearResults();
				XPathNodeTextMarker marker = new XPathNodeTextMarker(xmlView.TextEditor.Document);
				marker.RemoveMarkers();
				
				// Run XPath query.
				XPathQuery query = new XPathQuery(xmlView.TextEditor, GetNamespaces());
				XPathNodeMatch[] nodes = query.FindNodes(xpathComboBox.Text);
				if (nodes.Length > 0) {
					AddXPathResults(nodes);
					marker.AddMarkers(nodes);
				} else {
					AddNoXPathResult();
				}
				AddXPathToHistory();
			} catch (XPathException xpathEx) {
				AddErrorResult(xpathEx);
			} catch (XmlException xmlEx) {
				AddErrorResult(xmlEx);
			} finally {
				BringResultsTabToFront();
			}
		}
		
		void ClearResults()
		{
			xPathResultsListView.Items.Clear();
		}
		
		void BringResultsTabToFront()
		{
			tabControl.SelectedTab = tabControl.TabPages[0];
		}
		
		void AddXPathResults(XPathNodeMatch[] nodes)
		{
			foreach (XPathNodeMatch node in nodes) {
				ListViewItem item = new ListViewItem(node.DisplayValue);
				if (node.HasLineInfo()) {
					int line = node.LineNumber + 1;
					item.SubItems.Add(line.ToString(CultureInfo.InvariantCulture));
				}
				item.Tag = node;
				xPathResultsListView.Items.Add(item);
			}
		}
		
		void AddNoXPathResult()
		{
			xPathResultsListView.Items.Add(StringParser.Parse("${res:ICSharpCode.XmlEditor.XPathQueryPad.NoXPathResultsMessage}"));
		}
		
		void AddErrorResult(XmlException ex)
		{
			ListViewItem item = new ListViewItem(ex.Message, ErrorImageIndex);
			item.SubItems.Add(ex.LineNumber.ToString(CultureInfo.InvariantCulture));
			item.Tag = ex;
			xPathResultsListView.Items.Add(item);
		}
		
		void AddErrorResult(XPathException ex)
		{
			ListViewItem item = new ListViewItem(string.Concat(StringParser.Parse("${res:ICSharpCode.XmlEditor.XPathQueryPad.XPathLabel}"), " ", ex.Message), ErrorImageIndex);
			item.Tag = ex;
			xPathResultsListView.Items.Add(item);
		}
		
		void InitImageList()
		{
			try {
				imageList.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Error"));
			} catch (ResourceNotFoundException) { }
		}
		
		void InitStrings()
		{
			lineColumnHeader.Text = StringParser.Parse("${res:Global.TextLine}");
			matchColumnHeader.Text = StringParser.Parse("${res:ICSharpCode.XmlEditor.XPathQueryPad.XPathMatchColumnHeaderTitle}");
			prefixColumn.HeaderText = StringParser.Parse("${res:ICSharpCode.XmlEditor.XPathQueryPad.PrefixColumnHeaderTitle}");
			namespaceColumn.HeaderText = StringParser.Parse("${res:ICSharpCode.XmlEditor.XPathQueryPad.NamespaceColumnHeaderTitle}");
			queryButton.Text = StringParser.Parse("${res:ICSharpCode.XmlEditor.XPathQueryPad.QueryButton}");
			xPathLabel.Text = StringParser.Parse("${res:ICSharpCode.XmlEditor.XPathQueryPad.XPathLabel}");
			xPathResultsTabPage.Text = StringParser.Parse("${res:ICSharpCode.XmlEditor.XPathQueryPad.ResultsTab}");
			namespacesTabPage.Text = StringParser.Parse("${res:ICSharpCode.XmlEditor.XPathQueryPad.NamespacesTab}");
		}
		
		void InitAutoCompleteMode()
		{
			// Auto-completion disabled due to bug - see SD2-1049 - XPath query combo box is case insensitive
			/*try {
				xPathComboBox.AutoCompleteMode = AutoCompleteMode.Suggest;
				xPathComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
			} catch (ThreadStateException) { }*/
		}
		
		void XPathResultsListViewItemActivate(object sender, EventArgs e)
		{
			JumpToResultLocation();
		}
		
		/// <summary>
		/// Switches focus to the location of the XPath query result.
		/// </summary>
		void JumpToResultLocation()
		{
			MoveCaretToResultLocation(MoveCaret.ByJumping);
		}
		
		/// <summary>
		/// Scrolls the text editor so the location of the XPath query results is visible.
		/// </summary>
		void ScrollToResultLocation()
		{
			MoveCaretToResultLocation(MoveCaret.ByScrolling);
		}
		
		void MoveCaretToResultLocation(MoveCaret moveCaret)
		{
			if (xPathResultsListView.SelectedItems.Count > 0) {
				ListViewItem item = xPathResultsListView.SelectedItems[0];
				XPathNodeMatch xPathNodeMatch = item.Tag as XPathNodeMatch;
				XPathException xpathException = item.Tag as XPathException;
				XmlException xmlException = item.Tag as XmlException;
				if (xPathNodeMatch != null) {
					MoveCaretToXPathNodeMatch(moveCaret, xPathNodeMatch);
				} else if (xmlException != null) {
					MoveCaretToXmlException(moveCaret, xmlException);
				} else if (xpathException != null && moveCaret == MoveCaret.ByJumping) {
					xpathComboBox.Focus();
				}
			}
		}
		
		void MoveCaretToXPathNodeMatch(MoveCaret moveCaret, XPathNodeMatch node)
		{
			if (moveCaret == MoveCaret.ByJumping) {
				JumpTo(fileName, node.LineNumber, node.LinePosition);
			} else {
				ScrollTo(fileName, node.LineNumber, node.LinePosition, node.Value.Length);
			}
		}
		
		void MoveCaretToXmlException(MoveCaret moveCaret, XmlException ex)
		{
			int line =  ex.LineNumber - 1;
			int column = ex.LinePosition - 1;
			if (moveCaret == MoveCaret.ByJumping) {
				JumpTo(fileName, line, column);
			} else {
				ScrollTo(fileName, line, column);
			}
		}
		
		static void JumpTo(string fileName, int line, int column)
		{
			FileService.JumpToFilePosition(fileName, line + 1, column + 1);
		}
		
		/// <summary>
		/// Scrolls to the specified line and column and also selects the given
		/// length of text at this location.
		/// </summary>
		static void ScrollTo(string filename, int line, int column, int length)
		{
			XmlView view = XmlView.ForFileName(filename);
			if (view != null) {
				ITextEditor editor = view.TextEditor;
				if (editor == null) return;
				int corLine = Math.Min(line + 1, editor.Document.TotalNumberOfLines - 1);
				editor.JumpTo(corLine, column + 1);
				if (length > 0 && line < editor.Document.TotalNumberOfLines) {
					int offset = editor.Document.PositionToOffset(line + 1, column + 1);
					editor.Select(offset, length);
				}
			}
		}
		
		static void ScrollTo(string fileName, int line, int column)
		{
			ScrollTo(fileName, line, column, 0);
		}
		
		/// <summary>
		/// Gets the namespaces and prefixes as a string array.
		/// </summary>
		string[] GetNamespaceStringArray()
		{
			List<string> namespaces = new List<string>();
			foreach (XmlNamespace ns in GetNamespaces()) {
				namespaces.Add(ns.ToString());
			}
			return namespaces.ToArray();
		}
		
		/// <summary>
		/// Gets the previously used XPath queries from the combo box drop down list.
		/// </summary>
		string [] GetXPathHistory()
		{
			List<string> xpaths = new List<string>();
			foreach (string xpath in xpathComboBox.Items) {
				xpaths.Add(xpath);
			}
			return xpaths.ToArray();
		}
		
		/// <summary>
		/// Gets the namespace prefix in the specified row.
		/// </summary>
		static string GetPrefix(DataGridViewRow row)
		{
			string prefix = (string)row.Cells[0].Value;
			if (prefix != null) {
				return prefix;
			}
			return String.Empty;
		}
		
		/// <summary>
		/// Gets the namespace stored in the row.
		/// </summary>
		static string GetNamespace(DataGridViewRow row)
		{
			string ns = (string)row.Cells[1].Value;
			if (ns != null) {
				return ns;
			}
			return String.Empty;
		}
		
		/// <summary>
		/// Adds the text in the combo box to the combo box drop down list.
		/// </summary>
		void AddXPathToHistory()
		{
			string newXPath = xpathComboBox.Text;
			if (!xpathComboBox.Items.Contains(newXPath)) {
				xpathComboBox.Items.Insert(0, newXPath);
				if (xpathComboBox.Items.Count > xpathQueryHistoryLimit) {
					xpathComboBox.Items.RemoveAt(xpathQueryHistoryLimit);
				}
			}
		}
		
		void XPathComboBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return) {
				RunXPathQuery();
			}
		}
		
		void XPathResultsListViewSelectedIndexChanged(object sender, EventArgs e)
		{
			ScrollToResultLocation();
		}
		
		void XPathResultsListViewClick(object sender, EventArgs e)
		{
			ScrollToResultLocation();
		}
	}
}
