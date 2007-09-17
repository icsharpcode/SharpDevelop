// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mathias Simmack" email="mathias@simmack.de"/>
//     <version>$Revision$</version>
// </file>

namespace HtmlHelp2
{
	using System;
	using System.Drawing;
	using System.Security.Permissions;
	using System.Windows.Forms;
	
	using AxMSHelpControls;
	using HtmlHelp2.Environment;
	using ICSharpCode.Core;
	using ICSharpCode.SharpDevelop;
	using ICSharpCode.SharpDevelop.Gui;
	using MSHelpControls;
	using PrintOptions = MSHelpServices.HxHierarchy_PrintNode_Options;
	using TSC = MSHelpControls.HxTreeStyleConstant;

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
		MSHelp2TocControl help2TocControl;

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
			help2TocControl = new MSHelp2TocControl();
		}

		public void SyncToc(string topic)
		{
			help2TocControl.SynchronizeToc(topic);
		}

		public void GetPrevFromNode()
		{
			help2TocControl.GetPrevFromNode();
		}

		public void GetPrevFromUrl(string topic)
		{
			help2TocControl.GetPrevFromUrl(topic);
		}

		public void GetNextFromNode()
		{
			help2TocControl.GetNextFromNode();
		}

		public void GetNextFromUrl(string topic)
		{
			help2TocControl.GetNextFromUrl(topic);
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

	public class MSHelp2TocControl : UserControl
	{
		AxHxTocCtrl tocControl;
		ComboBox filterCombobox = new ComboBox();
		Label label1 = new Label();
		Label infoLabel = new Label();
		ContextMenuStrip printContextMenu = new ContextMenuStrip();
		ToolStripMenuItem printTopic = new ToolStripMenuItem();
		ToolStripMenuItem printTopicAndSubTopics = new ToolStripMenuItem();
		bool tocControlFailed;

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && tocControl != null)
			{
				tocControl.Dispose();
			}
		}

		[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
		public MSHelp2TocControl()
		{
			this.InitializeComponents();
			this.UpdateControl();

			HtmlHelp2Environment.FilterQueryChanged += new EventHandler(this.FilterQueryChanged);
			HtmlHelp2Environment.NamespaceReloaded += new EventHandler(this.NamespaceReloaded);
		}

		private void UpdateControl()
		{
			filterCombobox.Enabled =
				(HtmlHelp2Environment.SessionIsInitialized && !this.tocControlFailed);
			infoLabel.Visible = false;

			if (this.tocControlFailed)
			{
				this.ShowInfoMessage
					(StringParser.Parse("${res:AddIns.HtmlHelp2.HelpSystemNotAvailable}"));
			}
			else if (!HtmlHelp2Environment.SessionIsInitialized)
			{
				if (tocControl != null) tocControl.Visible = false;
				this.ShowInfoMessage
					("${res:AddIns.HtmlHelp2.HelpCollectionMayBeEmpty}");
			}
			else
			{
				tocControl.Visible = true;
				this.LoadToc();
			}
		}
		
		private void InitializeComponents()
		{
			infoLabel.Dock = DockStyle.Fill;
			infoLabel.Visible = false;
			infoLabel.TextAlign = ContentAlignment.MiddleCenter;
			Controls.Add(infoLabel);
			
			if (Help2ControlsValidation.IsTocControlRegistered)
			{
				try
				{
					tocControl = new AxHxTocCtrl();
					tocControl.BeginInit();
					tocControl.Dock = DockStyle.Fill;
					tocControl.NodeClick +=
						new AxMSHelpControls.IHxTreeViewEvents_NodeClickEventHandler(this.TocNodeClick);
					tocControl.NodeRightClick +=
						new AxMSHelpControls.IHxTreeViewEvents_NodeRightClickEventHandler(TocNodeRightClick);
					tocControl.EndInit();
					Controls.Add(tocControl);
					tocControl.CreateControl();
					
					tocControl.Visible = false;
					tocControl.BorderStyle = HxBorderStyle.HxBorderStyle_FixedSingle;
					tocControl.FontSource = HxFontSourceConstant.HxFontExternal;
					tocControl.TreeStyle =
						(HtmlHelp2Environment.Config.TocPictures)?TSC.HxTreeStyle_TreelinesPlusMinusPictureText:TSC.HxTreeStyle_TreelinesPlusMinusText;
					
					printTopic.Image = ResourcesHelper.GetBitmap("HtmlHelp2.16x16.Print.bmp");
					printTopic.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
					printTopic.Text = StringParser.Parse("${res:AddIns.HtmlHelp2.PrintTopic}");
					printTopic.Click += new EventHandler(this.PrintTopic);
					printContextMenu.Items.Add(printTopic);
					
					printTopicAndSubTopics.Text = StringParser.Parse("${res:AddIns.HtmlHelp2.PrintSubtopics}");
					printTopicAndSubTopics.Click += new EventHandler(this.PrintTopicAndSubTopics);
					printContextMenu.Items.Add(printTopicAndSubTopics);
				}
				catch (System.Runtime.InteropServices.COMException cEx)
				{
					LoggingService.Error("Help 2.0: TOC control failed: " + cEx.ToString());
					this.tocControlFailed = true;
				}
			}

			Panel panel1 = new Panel();
			Controls.Add(panel1);
			panel1.Dock = DockStyle.Top;
			panel1.Height = filterCombobox.Height + 7;

			panel1.Controls.Add(filterCombobox);
			filterCombobox.Dock = DockStyle.Top;
			filterCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
			filterCombobox.Sorted = true;
			filterCombobox.Enabled = false;
			filterCombobox.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			filterCombobox.SelectedIndexChanged += new EventHandler(this.FilterChanged);

			Controls.Add(label1);
			label1.Dock = DockStyle.Top;
			label1.TextAlign = ContentAlignment.MiddleLeft;
			label1.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RedrawContent();
			
			this.tocControlFailed = (this.tocControlFailed || tocControl == null);
		}

		private void ShowInfoMessage(string infoText)
		{
			filterCombobox.Items.Clear();
			infoLabel.Text = infoText;
			infoLabel.Visible = true;
		}

		public void RedrawContent()
		{
			label1.Text = StringParser.Parse("${res:AddIns.HtmlHelp2.FilteredBy}");
		}
		
		private void TocNodeClick(object sender, IHxTreeViewEvents_NodeClickEvent e)
		{
			string topicUrl = tocControl.Hierarchy.GetURL(e.hNode);
			this.CallHelp(topicUrl);
		}

		private void TocNodeRightClick(object sender, IHxTreeViewEvents_NodeRightClickEvent e)
		{
			if (e.hNode != 0)
			{
				printTopic.Enabled = !string.IsNullOrEmpty(tocControl.Hierarchy.GetURL(e.hNode));
				printTopicAndSubTopics.Enabled = tocControl.Hierarchy.GetFirstChild(e.hNode) != 0;
				bool selectTextFlag = (tocControl.Hierarchy.GetFirstChild(e.hNode) == 0 ||
				                       string.IsNullOrEmpty(tocControl.Hierarchy.GetURL(e.hNode)));
				printTopicAndSubTopics.Text =
					StringParser.Parse((selectTextFlag)?
					                   "${res:AddIns.HtmlHelp2.PrintSubtopics}":
					                   "${res:AddIns.HtmlHelp2.PrintTopicAndSubtopics}");

				Point p = new Point(e.x, e.y);
				p = this.PointToClient(p);
				printContextMenu.Show(this, p);
			}
		}

		#region Printing
		private void PrintTopic(object sender, EventArgs e)
		{
			if (tocControl.Selection != 0)
			{
				tocControl.Hierarchy.PrintNode(0, tocControl.Selection, PrintOptions.HxHierarchy_PrintNode_Option_Node);
			}
		}

		private void PrintTopicAndSubTopics(object sender, EventArgs e)
		{
			if (tocControl.Selection != 0)
			{
				tocControl.Hierarchy.PrintNode(0, tocControl.Selection, PrintOptions.HxHierarchy_PrintNode_Option_Children);
			}
		}
		#endregion

		private void FilterChanged(object sender, EventArgs e)
		{
			string selectedFilterName = filterCombobox.SelectedItem.ToString();
			if (selectedFilterName != null && selectedFilterName.Length > 0)
			{
				Cursor.Current = Cursors.WaitCursor;
				this.SetToc(selectedFilterName);
				Cursor.Current = Cursors.Default;
			}
		}
		
		private void LoadToc()
		{
			if (this.SetToc(HtmlHelp2Environment.CurrentFilterName))
			{
				filterCombobox.SelectedIndexChanged -= new EventHandler(this.FilterChanged);
				HtmlHelp2Environment.BuildFilterList(filterCombobox);
				filterCombobox.SelectedIndexChanged += new EventHandler(this.FilterChanged);
			}
		}
		
		private bool SetToc(string filterName)
		{
			try
			{
				tocControl.Hierarchy =
					HtmlHelp2Environment.GetTocHierarchy(HtmlHelp2Environment.FindFilterQuery(filterName));
				return true;
			}
			catch (System.Runtime.InteropServices.COMException)
			{
				LoggingService.Error("Help 2.0: Cannot connect to the IHxHierarchy interface.");
				return false;
			}
		}

		private void CallHelp(string topic)
		{
			this.CallHelp(topic, true);
		}

		private void CallHelp(string topic, bool syncToc)
		{
			if (!string.IsNullOrEmpty(topic))
			{
				if (syncToc) this.SynchronizeToc(topic);
				ShowHelpBrowser.OpenHelpView(topic);
			}
		}

		#region Help 2.0 Environment Events
		private void FilterQueryChanged(object sender, EventArgs e)
		{
			Application.DoEvents();

			string currentFilterName = filterCombobox.SelectedItem.ToString();
			if (string.Compare(currentFilterName, HtmlHelp2Environment.CurrentFilterName) != 0)
			{
				filterCombobox.SelectedIndexChanged -= new EventHandler(this.FilterChanged);
				filterCombobox.SelectedIndex =
					filterCombobox.Items.IndexOf(HtmlHelp2Environment.CurrentFilterName);
				this.SetToc(HtmlHelp2Environment.CurrentFilterName);
				filterCombobox.SelectedIndexChanged += new EventHandler(this.FilterChanged);
			}
		}

		private void NamespaceReloaded(object sender, EventArgs e)
		{
			this.UpdateControl();

			if (tocControl != null) {
				tocControl.TreeStyle =
					(HtmlHelp2Environment.Config.TocPictures)?TSC.HxTreeStyle_TreelinesPlusMinusPictureText:TSC.HxTreeStyle_TreelinesPlusMinusText;
			}
		}
		#endregion

		#region Published Help 2.0 Commands
		public void SynchronizeToc(string topic)
		{
			try
			{
				tocControl.Synchronize(topic);
			}
			catch (System.Runtime.InteropServices.COMException)
			{
				// SD2-812: ignore exception when trying to synchronize non-existing URL
			}
		}

		public void GetNextFromNode()
		{
			try
			{
				int currentNode = tocControl.Hierarchy.GetNextFromNode(tocControl.Selection);
				string topicUrl = tocControl.Hierarchy.GetURL(currentNode);
				this.CallHelp(topicUrl, true);
			}
			catch (System.Runtime.InteropServices.COMException)
			{
			}
		}

		public void GetNextFromUrl(string topic)
		{
			if (topic == null || topic.Length == 0) return;
			try
			{
				int currentNode = tocControl.Hierarchy.GetNextFromUrl(topic);
				string topicUrl = tocControl.Hierarchy.GetURL(currentNode);
				this.CallHelp(topicUrl, true);
			}
			catch (System.Runtime.InteropServices.COMException)
			{
				// SD2-812: ignore exception when trying to synchronize non-existing URL
			}
			catch (ArgumentException)
			{
			}
		}

		public void GetPrevFromNode()
		{
			try
			{
				int currentNode = tocControl.Hierarchy.GetPrevFromNode(tocControl.Selection);
				string topicUrl = tocControl.Hierarchy.GetURL(currentNode);
				this.CallHelp(topicUrl, true);
			}
			catch (System.Runtime.InteropServices.COMException)
			{
			}
		}

		public void GetPrevFromUrl(string topic)
		{
			if (topic == null || topic.Length == 0) return;
			try
			{
				int currentNode = tocControl.Hierarchy.GetPrevFromUrl(topic);
				string topicUrl = tocControl.Hierarchy.GetURL(currentNode);
				this.CallHelp(topicUrl, true);
			}
			catch (System.Runtime.InteropServices.COMException)
			{
				// SD2-812: ignore exception when trying to synchronize non-existing URL
			}
			catch (ArgumentException)
			{
			}
		}

		public bool IsNotFirstNode
		{
			get
			{
				try
				{
					int node = tocControl.Hierarchy.GetPrevFromNode(tocControl.Selection);
					return node != 0;
				}
				catch (System.Runtime.InteropServices.COMException)
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
					int node = tocControl.Hierarchy.GetNextFromNode(tocControl.Selection);
					return (node != 0);
				}
				catch (System.Runtime.InteropServices.COMException)
				{
					return true;
				}
			}
		}
		#endregion
	}
}
