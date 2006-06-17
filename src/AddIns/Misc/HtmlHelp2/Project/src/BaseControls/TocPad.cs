// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mathias Simmack" email="mathias@simmack.de"/>
//     <version>$Revision$</version>
// </file>

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
	using HtmlHelp2.Environment;
	using HtmlHelp2.ControlsValidation;
	using HtmlHelp2.ResourcesHelperClass;
	using PrintOptions = MSHelpServices.HxHierarchy_PrintNode_Options;


	public class ShowTocMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PadDescriptor toc = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2TocPad));
			if (toc != null) toc.BringPadToFront();
		}
	}

	public class HtmlHelp2TocPad : AbstractPadContent
	{
		protected MsHelp2TocControl help2TocControl;

		public override Control Control
		{
			get { return help2TocControl; }
		}

		public override void Dispose()
		{
			help2TocControl.Dispose();
		}

		public override void RedrawContent()
		{
			help2TocControl.RedrawContent();
		}

		public HtmlHelp2TocPad()
		{
			help2TocControl = new MsHelp2TocControl();
			if (help2TocControl.IsEnabled) help2TocControl.LoadToc();
		}

		public void SyncToc(string topicUrl)
		{
			if (help2TocControl.IsEnabled) help2TocControl.SynchronizeToc(topicUrl);
		}

		public void GetPrevFromNode()
		{
			if (help2TocControl.IsEnabled) help2TocControl.GetPrevFromNode();
		}

		public void GetPrevFromUrl(string topicUrl)
		{
			if (help2TocControl.IsEnabled) help2TocControl.GetPrevFromUrl(topicUrl);
		}

		public void GetNextFromNode()
		{
			if (help2TocControl.IsEnabled) help2TocControl.GetNextFromNode();
		}

		public void GetNextFromUrl(string topicUrl)
		{
			if (help2TocControl.IsEnabled) help2TocControl.GetNextFromUrl(topicUrl);
		}

		public bool IsNotFirstNode
		{
			get { return help2TocControl.IsNotFirstNode; }
		}

		public bool IsNotLastNode
		{
			get { return help2TocControl.IsNotLastNode; }
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
		bool controlIsEnabled              = false;

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && tocControl != null) { tocControl.Dispose(); }
		}

		public bool IsEnabled
		{
			get { return this.controlIsEnabled; }
		}

		public void RedrawContent()
		{
			label1.Text = StringParser.Parse("${res:AddIns.HtmlHelp2.FilteredBy}");
		}

		public MsHelp2TocControl()
		{
			this.controlIsEnabled = (HtmlHelp2Environment.IsReady &&
			                         Help2ControlsValidation.IsTocControlRegistered);

			if (this.controlIsEnabled)
			{
				try
				{
					tocControl                        = new AxHxTocCtrl();
					tocControl.BeginInit();
					tocControl.Dock                   = DockStyle.Fill;
					tocControl.NodeClick             +=
						new AxMSHelpControls.IHxTreeViewEvents_NodeClickEventHandler(this.TocNodeClicked);
					tocControl.NodeRightClick        +=
						new AxMSHelpControls.IHxTreeViewEvents_NodeRightClickEventHandler(this.TocNodeRightClicked);
					tocControl.EndInit();
					Controls.Add(tocControl);
					tocControl.CreateControl();
					tocControl.BorderStyle            = HxBorderStyle.HxBorderStyle_FixedSingle;
					tocControl.FontSource             = HxFontSourceConstant.HxFontExternal;
					#if DExplore8Style_NoTOCPictures
					tocControl.TreeStyle              = HxTreeStyleConstant.HxTreeStyle_TreelinesPlusMinusText;
					#endif

					printTopic.Image                  = ResourcesHelper.GetBitmap("HtmlHelp2.16x16.Print.bmp");
					printTopic.DisplayStyle           = ToolStripItemDisplayStyle.ImageAndText;
					printTopic.Text                   = StringParser.Parse("${res:AddIns.HtmlHelp2.PrintTopic}");
					printChildTopics.Text             = StringParser.Parse("${res:AddIns.HtmlHelp2.PrintSubtopics}");
					printPopup.Items.Add(printTopic);
					printTopic.Click                 += new EventHandler(this.PrintTopic);
					printPopup.Items.Add(printChildTopics);
					printChildTopics.Click           += new EventHandler(this.PrintTopicAndSubtopics);
				}
				catch (Exception ex)
				{
					LoggingService.Error("Help 2.0: TOC control failed; " + ex.ToString());
					this.FakeHelpControl();
				}
			}
			else
			{
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
			filterCombobox.Enabled                = this.controlIsEnabled;
			filterCombobox.SelectedIndexChanged  += new EventHandler(this.FilterChanged);

			// Filter label
			Controls.Add(label1);
			label1.Text                           = StringParser.Parse("${res:AddIns.HtmlHelp2.FilteredBy}");
			label1.Dock                           = DockStyle.Top;
			label1.TextAlign                      = ContentAlignment.MiddleLeft;
			label1.Enabled                        = this.controlIsEnabled;

			if (this.controlIsEnabled)
			{
				HtmlHelp2Environment.FilterQueryChanged  += new EventHandler(this.FilterQueryChanged);
				HtmlHelp2Environment.NamespaceReloaded   += new EventHandler(this.NamespaceReloaded);
			}
		}

		private void FakeHelpControl()
		{
			tocControl            = null;
			Label nohelpLabel     = new Label();
			nohelpLabel.Dock      = DockStyle.Fill;
			nohelpLabel.Text      = StringParser.Parse("${res:AddIns.HtmlHelp2.HelpSystemNotAvailable}");
			nohelpLabel.TextAlign = ContentAlignment.MiddleCenter;
			Controls.Add(nohelpLabel);
		}

		public void LoadToc()
		{
			if (!this.controlIsEnabled) return;

			try
			{
				tocControl.Hierarchy                 = HtmlHelp2Environment.GetTocHierarchy(HtmlHelp2Environment.CurrentFilterQuery);
				filterCombobox.SelectedIndexChanged -= new EventHandler(this.FilterChanged);
				HtmlHelp2Environment.BuildFilterList(filterCombobox);
				filterCombobox.SelectedIndexChanged += new EventHandler(this.FilterChanged);
			}
			catch
			{
				LoggingService.Error("Help 2.0: cannot connect to IHxHierarchy interface (Contents)");
				tocControl.Enabled     = false;
				tocControl.BackColor   = SystemColors.ButtonFace;
				filterCombobox.Enabled = false;
			}
		}

		private void FilterChanged(object sender, EventArgs e)
		{
			string selectedString = filterCombobox.SelectedItem.ToString();

			if (selectedString != null && selectedString != "")
			{
				Cursor.Current       = Cursors.WaitCursor;
				tocControl.Hierarchy = HtmlHelp2Environment.GetTocHierarchy(HtmlHelp2Environment.FindFilterQuery(selectedString));
				Cursor.Current       = Cursors.Default;
			}
		}

		#region Help 2.0 Environment Events
		private void FilterQueryChanged(object sender, EventArgs e)
		{
			Application.DoEvents();

			string currentFilterName = filterCombobox.SelectedItem.ToString();
			if (String.Compare(currentFilterName, HtmlHelp2Environment.CurrentFilterName) != 0)
			{
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
			if (topicUrl != null && topicUrl != "")
			{
				if (syncToc) this.SynchronizeToc(topicUrl);
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
			if (e.hNode != 0)
			{
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
			if (tocControl.Selection != 0)
			{
				tocControl.Hierarchy.PrintNode(0,
				                               tocControl.Selection,
				                               PrintOptions.HxHierarchy_PrintNode_Option_Node);
			}
		}

		private void PrintTopicAndSubtopics(object sender, EventArgs e)
		{
			if (tocControl.Selection != 0)
			{
				tocControl.Hierarchy.PrintNode(0,
				                               tocControl.Selection,
				                               PrintOptions.HxHierarchy_PrintNode_Option_Children);
			}
		}
		#endregion

		#region published Help2 TOC Commands
		public void SynchronizeToc(string topicUrl)
		{
			try {
				tocControl.Synchronize(topicUrl);
			} catch (System.Runtime.InteropServices.COMException) {
				// SD2-812: ignore exception when trying to synchronize non-existing URL
			}
		}

		public void GetNextFromNode()
		{
			int currentNode = tocControl.Hierarchy.GetNextFromNode(tocControl.Selection);
			string topicUrl = tocControl.Hierarchy.GetURL(currentNode);
			this.CallHelp(topicUrl, true);
		}

		public void GetNextFromUrl(string url)
		{
			try {
				int currentNode = tocControl.Hierarchy.GetNextFromUrl(url);
				string topicUrl = tocControl.Hierarchy.GetURL(currentNode);
				this.CallHelp(topicUrl, true);
			} catch (System.Runtime.InteropServices.COMException) {
			} catch (ArgumentException) {
				// SD2-812: ignore exception when trying to synchronize non-existing URL
			}
		}

		public void GetPrevFromNode()
		{
			int currentNode = tocControl.Hierarchy.GetPrevFromNode(tocControl.Selection);
			string topicUrl = tocControl.Hierarchy.GetURL(currentNode);
			this.CallHelp(topicUrl, true);
		}

		public void GetPrevFromUrl(string url)
		{
			try {
				int currentNode = tocControl.Hierarchy.GetPrevFromUrl(url);
				string topicUrl = tocControl.Hierarchy.GetURL(currentNode);
				this.CallHelp(topicUrl, true);
			} catch (ArgumentException) {
			} catch (System.Runtime.InteropServices.COMException) {
				// SD2-812: ignore exception when trying to synchronize non-existing URL
			}
		}

		public bool IsNotFirstNode
		{
			get
			{
				try
				{
					int currentNode = tocControl.Hierarchy.GetPrevFromNode(tocControl.Selection);
					return currentNode != 0;
				}
				catch
				{
					return true;
				}
			}
		}

		public bool IsNotLastNode
		{
			get
			{
				try
				{
					int currentNode = tocControl.Hierarchy.GetNextFromNode(tocControl.Selection);
					return currentNode != 0;
				}
				catch
				{
					return true;
				}
			}
		}
		#endregion
	}
}
