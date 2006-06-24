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
		}

		public void SyncToc(string topicUrl)
		{
			help2TocControl.SynchronizeToc(topicUrl);
		}

		public void GetPrevFromNode()
		{
			help2TocControl.GetPrevFromNode();
		}

		public void GetPrevFromUrl(string topicUrl)
		{
			help2TocControl.GetPrevFromUrl(topicUrl);
		}

		public void GetNextFromNode()
		{
			help2TocControl.GetNextFromNode();
		}

		public void GetNextFromUrl(string topicUrl)
		{
			help2TocControl.GetNextFromUrl(topicUrl);
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
		AxHxTocCtrl tocControl = null;
		ComboBox filterCombobox = new ComboBox();
		Label label1 = new Label();
		ContextMenuStrip printContextMenu = new ContextMenuStrip();
		ToolStripMenuItem printTopic = new ToolStripMenuItem();
		ToolStripMenuItem printTopicAndSubTopics = new ToolStripMenuItem();
		bool isEnabled = false;

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && tocControl != null)
			{
				tocControl.Dispose();
			}
		}

		public MsHelp2TocControl()
		{
			this.isEnabled = (HtmlHelp2Environment.IsReady && Help2ControlsValidation.IsTocControlRegistered);

			if (this.isEnabled)
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
				catch(Exception ex)
				{
					LoggingService.Error("Help 2.0: TOC control failed; " + ex.ToString());
					this.isEnabled = false;
					this.FakeHelpControl();
				}
			}
			else
			{
				this.FakeHelpControl();
			}
			
			Panel panel1 = new Panel();
			Controls.Add(panel1);
			panel1.Dock = DockStyle.Top;
			panel1.Height = filterCombobox.Height + 7;

			panel1.Controls.Add(filterCombobox);
			filterCombobox.Dock = DockStyle.Top;
			filterCombobox.DropDownStyle = ComboBoxStyle.DropDownList;
			filterCombobox.Sorted = true;
			filterCombobox.Enabled = this.isEnabled;
			filterCombobox.SelectedIndexChanged += new EventHandler(this.FilterChanged);

			Controls.Add(label1);
			label1.Dock = DockStyle.Top;
			label1.TextAlign = ContentAlignment.MiddleLeft;
			label1.Enabled = this.isEnabled;
			this.RedrawContent();
			
			if (this.isEnabled)
			{
				HtmlHelp2Environment.FilterQueryChanged += new EventHandler(this.FilterQueryChanged);
				HtmlHelp2Environment.NamespaceReloaded += new EventHandler(this.NamespaceReloaded);
				this.LoadToc();
			}
		}

		private void FakeHelpControl()
		{
			if (tocControl != null) tocControl.Dispose();
			tocControl = null;

			Controls.Clear();
			Label nohelpLabel = new Label();
			nohelpLabel.Dock = DockStyle.Fill;
			nohelpLabel.Text = StringParser.Parse("${res:AddIns.HtmlHelp2.HelpSystemNotAvailable}");
			nohelpLabel.TextAlign = ContentAlignment.MiddleCenter;
			Controls.Add(nohelpLabel);
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
				printTopic.Enabled = tocControl.Hierarchy.GetURL(e.hNode) != "";
				printTopicAndSubTopics.Enabled = tocControl.Hierarchy.GetFirstChild(e.hNode) != 0;
				bool selectTextFlag = (tocControl.Hierarchy.GetFirstChild(e.hNode) == 0 ||
				                       tocControl.Hierarchy.GetURL(e.hNode) == "");
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
			this.SetToc(HtmlHelp2Environment.CurrentFilterName);
			if (this.isEnabled)
			{
				filterCombobox.SelectedIndexChanged -= new EventHandler(this.FilterChanged);
				HtmlHelp2Environment.BuildFilterList(filterCombobox);
				filterCombobox.SelectedIndexChanged += new EventHandler(this.FilterChanged);
			}
		}
	
		private void SetToc(string filterName)
		{
			if (!this.isEnabled) return;
			try
			{
				tocControl.Hierarchy =
					HtmlHelp2Environment.GetTocHierarchy(HtmlHelp2Environment.FindFilterQuery(filterName));
			}
			catch
			{
				this.isEnabled = false;
				tocControl.Enabled = false;
				tocControl.BackColor = SystemColors.ButtonFace;
				filterCombobox.Enabled = false;
				label1.Enabled = false;
				LoggingService.Error("Help 2.0: cannot connect to IHxHierarchy interface (Contents)");
			}
		}

		private void CallHelp(string topicUrl)
		{
			this.CallHelp(topicUrl, true);
		}

		private void CallHelp(string topicUrl, bool syncToc)
		{
			if (topicUrl.Length > 0)
			{
				if (syncToc) this.SynchronizeToc(topicUrl);
				ShowHelpBrowser.OpenHelpView(topicUrl);
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
			this.LoadToc();
			
			tocControl.TreeStyle =
				(HtmlHelp2Environment.Config.TocPictures)?TSC.HxTreeStyle_TreelinesPlusMinusPictureText:TSC.HxTreeStyle_TreelinesPlusMinusText;
		}
		#endregion

		#region Published Help 2.0 Commands
		public void SynchronizeToc(string topicUrl)
		{
			if (!this.isEnabled) return;
			try
			{
				tocControl.Synchronize(topicUrl);
			}
			catch (System.Runtime.InteropServices.COMException)
			{
				// SD2-812: ignore exception when trying to synchronize non-existing URL
			}
		}

		public void GetNextFromNode()
		{
			if (!this.isEnabled) return;
			int currentNode = tocControl.Hierarchy.GetNextFromNode(tocControl.Selection);
			string topicUrl = tocControl.Hierarchy.GetURL(currentNode);
			this.CallHelp(topicUrl, true);
		}

		public void GetNextFromUrl(string url)
		{
			if (!this.isEnabled || url == null || url.Length == 0) return;
			try
			{
				int currentNode = tocControl.Hierarchy.GetNextFromUrl(url);
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
			if (!this.isEnabled) return;
			int currentNode = tocControl.Hierarchy.GetPrevFromNode(tocControl.Selection);
			string topicUrl = tocControl.Hierarchy.GetURL(currentNode);
			this.CallHelp(topicUrl, true);
		}

		public void GetPrevFromUrl(string url)
		{
			if (!this.isEnabled || url == null || url.Length == 0) return;
			try
			{
				int currentNode = tocControl.Hierarchy.GetPrevFromUrl(url);
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
					int currentNode = tocControl.Hierarchy.GetPrevFromNode(tocControl.Selection);
					return (currentNode != 0);
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
					return (currentNode != 0);
				}
				catch
				{
					return true;
				}
			}
		}

		public bool IsEnabled
		{
			get { return this.isEnabled; }
		}
		#endregion
	}
}
