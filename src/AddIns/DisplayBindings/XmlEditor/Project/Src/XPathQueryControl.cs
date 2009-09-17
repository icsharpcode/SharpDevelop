// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

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
		string fileName = String.Empty;
		
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
			xPathComboBox.KeyDown += XPathComboBoxKeyDown;
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
		public ReadOnlyCollection<XmlNamespace> GetNamespaces()
		{
			List<XmlNamespace> namespaces = new List<XmlNamespace>();
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
			return new ReadOnlyCollection<XmlNamespace>(namespaces);
		}
		
		public DataGridView NamespacesDataGridView {
			get {
				return namespacesDataGridView;
			}
		}
		
		public ListView XPathResultsListView {
			get {
				return xPathResultsListView;
			}
		}
		
		public ComboBox XPathComboBox {
			get {
				return xPathComboBox;
			}
		}
		
		/// <summary>
		/// Creates a properties object that contains the current state of the
		/// control.
		/// </summary>
		public Properties CreateMemento()
		{
			Properties properties = new Properties();
			
			// Save namespaces.
			properties.Set(NamespacesProperty, GetNamespaceStringArray());
			
			// Save namespace data grid column widths.
			properties.Set<int>(PrefixColumnWidthProperty, prefixColumn.Width);

			// Save xpath results list view column widths.
			properties.Set<int>(MatchColumnWidthProperty, matchColumnHeader.Width);
			properties.Set<int>(LineColumnWidthProperty, lineColumnHeader.Width);

			// Save xpath query history.
			properties.Set(XPathComboBoxTextProperty, XPathComboBox.Text);
			properties.Set(XPathComboBoxItemsProperty, GetXPathHistory());
			
			return properties;
		}
		
		/// <summary>
		/// Reloads the state of the control.
		/// </summary>
		public void SetMemento(Properties memento)
		{
			ignoreXPathTextChanges = true;
			
			try {
				// Set namespaces.
				string[] namespaces = memento.Get(NamespacesProperty, new string[0]);
				foreach (string ns in namespaces) {
					XmlNamespace xmlNamespace = XmlNamespace.FromString(ns);
					AddNamespace(xmlNamespace.Prefix, xmlNamespace.Uri);
				}
				
				// Set namespace data grid column widths.
				prefixColumn.Width = memento.Get<int>(PrefixColumnWidthProperty, 50);
				
				// Set xpath results list view column widths.
				matchColumnHeader.Width = memento.Get<int>(MatchColumnWidthProperty, 432);
				lineColumnHeader.Width = memento.Get<int>(LineColumnWidthProperty, 60);
				
				// Set xpath query history.
				XPathComboBox.Text = memento.Get(XPathComboBoxTextProperty, String.Empty);
				string[] xpaths = memento.Get(XPathComboBoxItemsProperty, new string[0]);
				foreach (string xpath in xpaths) {
					xPathComboBox.Items.Add(xpath);
				}
			} finally {
				ignoreXPathTextChanges = false;
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
		/// Removes all the XPath Node markers from all the open documents.
		/// </summary>
		public void RemoveXPathNodeTextMarkers()
		{
			foreach (IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection) {
				ITextEditorControlProvider textEditorProvider = view as ITextEditorControlProvider;
				if (textEditorProvider != null) {
					XPathNodeTextMarker.RemoveMarkers(textEditorProvider.TextEditorControl.Document.MarkerStrategy);
					textEditorProvider.TextEditorControl.Refresh();
				}
			}
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
			this.xPathComboBox = new System.Windows.Forms.ComboBox();
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
			this.xPathComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.xPathComboBox.FormattingEnabled = true;
			this.xPathComboBox.Location = new System.Drawing.Point(55, 3);
			this.xPathComboBox.Name = "xPathComboBox";
			this.xPathComboBox.Size = new System.Drawing.Size(438, 21);
			this.xPathComboBox.TabIndex = 1;
			this.xPathComboBox.TextChanged += new System.EventHandler(this.XPathComboBoxTextChanged);
			this.xPathComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.XPathComboBoxKeyDown);
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
			this.Controls.Add(this.xPathComboBox);
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
		private System.Windows.Forms.ComboBox xPathComboBox;
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
			queryButton.Enabled = IsXPathQueryEntered && XmlView.IsXmlViewActive;
		}
		
		bool IsXPathQueryEntered {
			get {
				return xPathComboBox.Text.Length > 0;
			}
		}
		
		void QueryButtonClick(object sender, EventArgs e)
		{
			RunXPathQuery();
		}
		
		void RunXPathQuery()
		{
			XmlView view = XmlView.ActiveXmlView;
			if (view == null) {
				return;
			}
			
			try {
				MarkerStrategy markerStrategy = view.TextEditorControl.Document.MarkerStrategy;
				fileName = view.PrimaryFileName;
			
				// Clear previous XPath results.
				ClearResults();
				XPathNodeTextMarker.RemoveMarkers(markerStrategy);

				// Run XPath query.
				XPathNodeMatch[] nodes = view.SelectNodes(xPathComboBox.Text, GetNamespaces());
				if (nodes.Length > 0) {
					AddXPathResults(nodes);
					XPathNodeTextMarker.AddMarkers(markerStrategy, nodes);
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
				view.TextEditorControl.Refresh();
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
					item.SubItems.Add(line.ToString());
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
			item.SubItems.Add(ex.LineNumber.ToString());
			item.Tag = ex;
			xPathResultsListView.Items.Add(item);
		}
		
		void AddErrorResult(XPathException ex)
		{
			ListViewItem item = new ListViewItem(String.Concat(StringParser.Parse("${res:ICSharpCode.XmlEditor.XPathQueryPad.XPathLabel}"), " ", ex.Message), ErrorImageIndex);
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
			lineColumnHeader.Text = StringParser.Parse("${res:CompilerResultView.LineText}");
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
					xPathComboBox.Focus();
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
		
		void JumpTo(string fileName, int line, int column)
		{
			FileService.JumpToFilePosition(fileName, line, column);
		}
		
		/// <summary>
		/// Scrolls to the specified line and column and also selects the given
		/// length of text at this location.
		/// </summary>
		void ScrollTo(string fileName, int line, int column, int length)
		{
			XmlView view = XmlView.ActiveXmlView;
			if (view != null && IsFileNameMatch(view)) {
				TextAreaControl textAreaControl = view.TextEditorControl.ActiveTextAreaControl;
				if (length > 0 && line < textAreaControl.Document.TotalNumberOfLines) {
					SelectionManager selectionManager = textAreaControl.SelectionManager;
					selectionManager.ClearSelection();
					TextLocation startPos = new TextLocation(column, line);
					TextLocation endPos = new TextLocation(column + length, line);
					selectionManager.SetSelection(startPos, endPos);
				}
				line = Math.Min(line, textAreaControl.Document.TotalNumberOfLines - 1);
				textAreaControl.ScrollTo(line, column);
			}
		}
		
		void ScrollTo(string fileName, int line, int column)
		{
			ScrollTo(fileName, line, column, 0);
		}
		
		/// <summary>
		/// Tests whether the specified view matches the filename the XPath
		/// results were found in.
		/// </summary>
		bool IsFileNameMatch(XmlView view)
		{
			return FileUtility.IsEqualFileName(fileName, view.PrimaryFileName);
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
		/// <returns></returns>
		string [] GetXPathHistory()
		{
			List<string> xpaths = new List<string>();
			foreach (string xpath in xPathComboBox.Items) {
				xpaths.Add(xpath);
			}
			return xpaths.ToArray();
		}
		
		/// <summary>
		/// Gets the namespace prefix in the specified row.
		/// </summary>
		string GetPrefix(DataGridViewRow row)
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
		string GetNamespace(DataGridViewRow row)
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
			string newXPath = xPathComboBox.Text;
			if (!xPathComboBox.Items.Contains(newXPath)) {
				xPathComboBox.Items.Insert(0, newXPath);
				if (xPathComboBox.Items.Count > xpathQueryHistoryLimit) {
					xPathComboBox.Items.RemoveAt(xpathQueryHistoryLimit);
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
