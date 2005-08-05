/* ***********************************************************
 *
 * Help 2.0 Environment for SharpDevelop
 * Table of Contents Pad
 * Copyright (c) 2005, Mathias Simmack. All rights reserved.
 *
 * ********************************************************* */

//#define DExplore8Style_NoTOCPictures

namespace HtmlHelp2
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;
	using ICSharpCode.Core;
	using ICSharpCode.SharpDevelop;
	using ICSharpCode.SharpDevelop.Gui;
	using AxMSHelpControls;
	using MSHelpControls;
	using MSHelpServices;
	using HtmlHelp2Service;
	using HtmlHelp2Browser;


	public class ShowTocMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PadDescriptor toc = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2TocPad));
			if(toc != null) toc.BringPadToFront();
		}
	}


	public class HtmlHelp2TocPad : AbstractPadContent
	{
		protected MsHelp2TocControl help2TocControl;

		public override Control Control
		{
			get {
				return help2TocControl;
			}
		}

		public override void Dispose()
		{
			try {
				help2TocControl.Dispose();
			}
			catch {
			}
		}

		public override void RedrawContent()
		{
			help2TocControl.RedrawContent();
		}

		public HtmlHelp2TocPad()
		{
			help2TocControl = new MsHelp2TocControl();
			help2TocControl.LoadToc();
		}

		public void SyncToc(string topicUrl)
		{
			try {
				help2TocControl.SynToc(topicUrl);
			}
			catch {
			}
		}

		public void GetPrevFromNode()
		{
			try {
				help2TocControl.GetPrevFromNode();
			}
			catch {
			}
		}

		public void GetNextFromNode()
		{
			try {
				help2TocControl.GetNextFromNode();
			}
			catch {
			}
		}

		public bool IsNotFirstNode
		{
			get {
				return help2TocControl.IsNotFirstNode;
			}
		}

		public bool IsNotLastNode
		{
			get {
				return help2TocControl.IsNotLastNode;
			}
		}
	}


	public class MsHelp2TocControl : UserControl
	{
		AxHxTocCtrl tocControl             = null;
		ComboBox filterCombobox            = new ComboBox();
		Label label1                       = new Label();
		ContextMenuStrip printPopup        = new ContextMenuStrip();
		ToolStripMenuItem printTopic       = new ToolStripMenuItem();
		ToolStripMenuItem printChildTopics = new ToolStripMenuItem();

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if(disposing && tocControl != null)	{
				tocControl.Dispose();
			}
		}

		public void RedrawContent()
		{
			label1.Text = StringParser.Parse("${res:AddIns.HtmlHelp2.FilteredBy}");
		}

		public MsHelp2TocControl()
		{
			bool Help2EnvIsReady = (HtmlHelp2Environment.IsReady && Help2ControlsValidation.IsTocControlRegistered);

			if(Help2EnvIsReady) {
				try {
					tocControl                        = new AxHxTocCtrl();
					tocControl.BeginInit();
					tocControl.Dock                   = DockStyle.Fill;
					tocControl.NodeClick             += new AxMSHelpControls.IHxTreeViewEvents_NodeClickEventHandler(this.TocNodeClicked);
					tocControl.NodeRightClick        += new AxMSHelpControls.IHxTreeViewEvents_NodeRightClickEventHandler(this.TocNodeRightClicked);
					tocControl.EndInit();
					Controls.Add(tocControl);
					tocControl.CreateControl();
					tocControl.BorderStyle            = HxBorderStyle.HxBorderStyle_FixedSingle;
					tocControl.FontSource             = HxFontSourceConstant.HxFontExternal;
					#if DExplore8Style_NoTOCPictures
					tocControl.TreeStyle              = HxTreeStyleConstant.HxTreeStyle_TreelinesPlusMinusText;
					#endif

					printTopic.Text                   = StringParser.Parse("${res:AddIns.HtmlHelp2.PrintTopic}");
					printChildTopics.Text             = StringParser.Parse("${res:AddIns.HtmlHelp2.PrintSubtopics}");
					printPopup.Items.Add(printTopic);
					printTopic.Click                 += new EventHandler(this.PrintTopic);
					printPopup.Items.Add(printChildTopics);
					printChildTopics.Click           += new EventHandler(this.PrintTopicAndSubtopics);
				}
				catch {
					this.FakeHelpControl();
				}
			}
			else {
				this.FakeHelpControl();
			}

			// Combobox panel
			Panel panel1                          = new Panel();
			Controls.Add(panel1);
			panel1.Dock                           = DockStyle.Top;
			panel1.Height                         = filterCombobox.Height + 7;
			panel1.Controls.Add(filterCombobox);
			filterCombobox.Dock                   = DockStyle.Top;
			filterCombobox.DropDownStyle          = ComboBoxStyle.DropDownList;
			filterCombobox.Sorted                 = true;
			filterCombobox.Enabled                = Help2EnvIsReady;
			filterCombobox.SelectedIndexChanged  += new EventHandler(this.FilterChanged);

			// Filter label
			Controls.Add(label1);
			label1.Text                           = StringParser.Parse("${res:AddIns.HtmlHelp2.FilteredBy}");
			label1.Dock                           = DockStyle.Top;
			label1.TextAlign                      = ContentAlignment.MiddleLeft;
			label1.Enabled                        = Help2EnvIsReady;

			if(Help2EnvIsReady) {
				HtmlHelp2Environment.FilterQueryChanged  += new EventHandler(this.FilterQueryChanged);
				HtmlHelp2Environment.NamespaceReloaded   += new EventHandler(this.NamespaceReloaded);
			}
		}

		private void FakeHelpControl()
		{
			tocControl              = null;
			Panel nohelpPanel       = new Panel();
			Controls.Add(nohelpPanel);
			nohelpPanel.Dock        = DockStyle.Fill;
			nohelpPanel.BorderStyle = BorderStyle.Fixed3D;
		}

		public void LoadToc()
		{
			try {
				tocControl.Hierarchy                 = HtmlHelp2Environment.GetTocHierarchy(HtmlHelp2Environment.CurrentFilterQuery);
				filterCombobox.SelectedIndexChanged -= new EventHandler(this.FilterChanged);
				HtmlHelp2Environment.BuildFilterList(filterCombobox);
				filterCombobox.SelectedIndexChanged += new EventHandler(this.FilterChanged);
			}
			catch {
			}
		}

		private void FilterChanged(object sender, EventArgs e)
		{
			string selectedString = filterCombobox.SelectedItem.ToString();

			if(selectedString != null && selectedString != "") {
				try {
					Cursor.Current       = Cursors.WaitCursor;
					tocControl.Hierarchy = HtmlHelp2Environment.GetTocHierarchy(HtmlHelp2Environment.FindFilterQuery(selectedString));
					Cursor.Current       = Cursors.Default;
				}
				catch {
				}
			}
		}

		#region Help 2.0 Environment Events
		private void FilterQueryChanged(object sender, EventArgs e)
		{
			tocControl.Refresh();

			string currentFilterName = filterCombobox.SelectedItem.ToString();
			if(String.Compare(currentFilterName, HtmlHelp2Environment.CurrentFilterName) != 0) {
				filterCombobox.SelectedIndexChanged -= new EventHandler(this.FilterChanged);
				filterCombobox.SelectedIndex         = filterCombobox.Items.IndexOf(HtmlHelp2Environment.CurrentFilterName);
				tocControl.Hierarchy                 = HtmlHelp2Environment.GetTocHierarchy(HtmlHelp2Environment.CurrentFilterQuery);
				filterCombobox.SelectedIndexChanged += new EventHandler(this.FilterChanged);
			}
		}

		private void NamespaceReloaded(object sender, EventArgs e)
		{
			this.LoadToc();
		}
		#endregion

		private void CallHelp(string topicUrl, bool syncToc)
		{
			if(topicUrl != null && topicUrl != "") {
				if(syncToc) this.SynToc(topicUrl);
				ShowHelpBrowser.OpenHelpView(topicUrl);
			}
		}

		private void TocNodeClicked(object sender, IHxTreeViewEvents_NodeClickEvent e)
		{
			string TopicUrl = tocControl.Hierarchy.GetURL(e.hNode);
			this.CallHelp(TopicUrl,false);
		}

		#region Printing
		private void TocNodeRightClicked(object sender, IHxTreeViewEvents_NodeRightClickEvent e)
		{
			if(e.hNode != 0) {
				printTopic.Enabled       = tocControl.Hierarchy.GetURL(e.hNode) != "";
				printChildTopics.Enabled = tocControl.Hierarchy.GetFirstChild(e.hNode) != 0;
				printChildTopics.Text    = StringParser.Parse((tocControl.Hierarchy.GetFirstChild(e.hNode) == 0 || tocControl.Hierarchy.GetURL(e.hNode) == "")?
				                                              "${res:AddIns.HtmlHelp2.PrintSubtopics}":
				                                              "${res:AddIns.HtmlHelp2.PrintTopicAndSubtopics}");

				Point p = new Point(e.x, e.y);
				p       = this.PointToClient(p);
				printPopup.Show(this, p);
			}
		}

		private void PrintTopic(object sender, EventArgs e)
		{
			if(tocControl.Selection != 0) {
				tocControl.Hierarchy.PrintNode(0,
				                               tocControl.Selection,
				                               HxHierarchy_PrintNode_Options.HxHierarchy_PrintNode_Option_Node);
			}
		}

		private void PrintTopicAndSubtopics(object sender, EventArgs e)
		{
			if(tocControl.Selection != 0) {
				tocControl.Hierarchy.PrintNode(0,
				                               tocControl.Selection,
				                               HxHierarchy_PrintNode_Options.HxHierarchy_PrintNode_Option_Children);
			}
		}
		#endregion

		#region published Help2 TOC Commands
		public void SynToc(string topicUrl)
		{
			try {
				tocControl.Synchronize(topicUrl);
			}
			catch {
			}
		}

		public void GetNextFromNode()
		{
			try {
				int currentNode = tocControl.Hierarchy.GetNextFromNode(tocControl.Selection);
				string TopicUrl = tocControl.Hierarchy.GetURL(currentNode);
				this.CallHelp(TopicUrl,true);
			}
			catch {
			}
		}

		public void GetPrevFromNode()
		{
			try {
				int currentNode = tocControl.Hierarchy.GetPrevFromNode(tocControl.Selection);
				string TopicUrl = tocControl.Hierarchy.GetURL(currentNode);
				this.CallHelp(TopicUrl,true);
			}
			catch {
			}
		}

		public bool IsNotFirstNode
		{
			get {
				try {
					int currentNode = tocControl.Hierarchy.GetPrevFromNode(tocControl.Selection);
					return currentNode != 0;
				}
				catch {
					return true;
				}
			}
		}

		public bool IsNotLastNode
		{
			get {
				try {
					int currentNode = tocControl.Hierarchy.GetNextFromNode(tocControl.Selection);
					return currentNode != 0;
				}
				catch {
					return true;
				}
			}
		}
		#endregion
	}
}
