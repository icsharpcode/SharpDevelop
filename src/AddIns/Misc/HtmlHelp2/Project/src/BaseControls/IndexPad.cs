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
	using MSHelpServices;

	public class ShowIndexMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PadDescriptor index = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2IndexPad));
			if (index != null) index.BringPadToFront();
		}
	}

	public class HtmlHelp2IndexPad : AbstractPadContent
	{
		MSHelp2IndexControl help2IndexControl;

		public override Control Control
		{
			get { return help2IndexControl;	}
		}

		public override void Dispose()
		{
			help2IndexControl.Dispose();
		}

		public override void RedrawContent()
		{
			help2IndexControl.RedrawContent();
		}

		public HtmlHelp2IndexPad()
		{
			help2IndexControl = new MSHelp2IndexControl();
		}
	}
	
	public class MSHelp2IndexControl : UserControl
	{
		AxHxIndexCtrl indexControl;
		ComboBox filterCombobox = new ComboBox();
		ComboBox searchTerm = new ComboBox();
		Label label1 = new Label();
		Label label2 = new Label();
		Label infoLabel = new Label();
		bool indexControlFailed;
		bool itemClicked;

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && indexControl != null)
			{
				indexControl.Dispose();
			}
		}

		[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
		public MSHelp2IndexControl()
		{
			this.InitializeComponents();
			this.UpdateControls();

			HtmlHelp2Environment.FilterQueryChanged += new EventHandler(this.FilterQueryChanged);
			HtmlHelp2Environment.NamespaceReloaded += new EventHandler(this.NamespaceReloaded);
		}

		private void UpdateControls()
		{
			filterCombobox.Enabled =
				(HtmlHelp2Environment.SessionIsInitialized && !this.indexControlFailed);
			searchTerm.Enabled =
				(HtmlHelp2Environment.SessionIsInitialized && !this.indexControlFailed);
			infoLabel.Visible = false;

			if (this.indexControlFailed)
			{
				this.ShowInfoMessage
					(StringParser.Parse("${res:AddIns.HtmlHelp2.HelpSystemNotAvailable}"));
			}
			else if (!HtmlHelp2Environment.SessionIsInitialized)
			{
				if (indexControl != null) indexControl.Visible = false;
				this.ShowInfoMessage
					("${res:AddIns.HtmlHelp2.HelpCollectionMayBeEmpty}");
			}
			else
			{
				indexControl.Visible = true;
				this.LoadIndex();
			}
		}
		
		private void InitializeComponents()
		{
			infoLabel.Dock = DockStyle.Fill;
			infoLabel.Visible = false;
			infoLabel.TextAlign = ContentAlignment.MiddleCenter;
			Controls.Add(infoLabel);

			if (Help2ControlsValidation.IsIndexControlRegistered)
			{
				try
				{
					indexControl = new AxHxIndexCtrl();
					indexControl.BeginInit();
					indexControl.Dock = DockStyle.Fill;
					indexControl.ItemClick +=
						new AxMSHelpControls.IHxIndexViewEvents_ItemClickEventHandler(this.IndexItemClick);
					indexControl.EndInit();
					Controls.Add(indexControl);
					indexControl.CreateControl();
					indexControl.BorderStyle = HxBorderStyle.HxBorderStyle_FixedSingle;
					indexControl.FontSource = HxFontSourceConstant.HxFontExternal;
				}
				catch (System.Runtime.InteropServices.COMException ex)
				{
					LoggingService.Error("Help 2.0: Index control failed; " + ex.ToString());
					this.indexControlFailed = true;
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

			Panel panel2 = new Panel();
			Controls.Add(panel2);
			panel2.Dock = DockStyle.Top;
			panel2.Height = searchTerm.Height + 7;

			panel2.Controls.Add(searchTerm);
			searchTerm.Dock = DockStyle.Top;
			searchTerm.Enabled = false;
			searchTerm.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			searchTerm.TextChanged += new EventHandler(this.SearchTextChanged);
			searchTerm.KeyPress += new KeyPressEventHandler(this.SearchKeyPress);

			Controls.Add(label2);
			label2.Dock = DockStyle.Top;
			label2.TextAlign = ContentAlignment.MiddleLeft;
			label2.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

			this.RedrawContent();

			this.indexControlFailed = (this.indexControlFailed || indexControl == null);
		}

		private void ShowInfoMessage(string infoText)
		{
			filterCombobox.Items.Clear();
			searchTerm.Items.Clear();
			searchTerm.Text = string.Empty;
			infoLabel.Text = infoText;
			infoLabel.Visible = true;
		}

		public void RedrawContent()
		{
			label1.Text = StringParser.Parse("${res:AddIns.HtmlHelp2.FilteredBy}");
			label2.Text = StringParser.Parse("${res:AddIns.HtmlHelp2.LookFor}");
		}

		private void IndexItemClick(object sender, IHxIndexViewEvents_ItemClickEvent e)
		{
			string indexTerm = indexControl.IndexData.GetFullStringFromSlot(e.iItem, ",");
			int indexSlot = e.iItem;

			itemClicked = true;
			searchTerm.Items.Insert(0, indexTerm);
			searchTerm.SelectedIndex = 0;
			itemClicked = false;

			this.ShowSelectedItemEntry(indexTerm, indexSlot);
		}

		private void FilterChanged(object sender, EventArgs e)
		{
			string selectedFilterName = filterCombobox.SelectedItem.ToString();
			if (!string.IsNullOrEmpty(selectedFilterName))
			{
				Cursor.Current = Cursors.WaitCursor;
				this.SetIndex(selectedFilterName);
				Cursor.Current = Cursors.Default;
			}
		}

		private void SearchTextChanged(object sender, EventArgs e)
		{
			if (!this.itemClicked && searchTerm.Text.Length > 0)
			{
				indexControl.Selection =
					indexControl.IndexData.GetSlotFromString(searchTerm.Text);
			}
		}

		private void SearchKeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)13)
			{
				int indexSlot = indexControl.IndexData.GetSlotFromString(searchTerm.Text);
				string indexTerm = indexControl.IndexData.GetFullStringFromSlot(indexSlot, ",");

				searchTerm.Items.Insert(0, indexTerm);
				searchTerm.SelectedIndex = 0;
				if (searchTerm.Items.Count > 10)
				{
					searchTerm.Items.RemoveAt(10);
				}

				this.ShowSelectedItemEntry(indexTerm, indexSlot);
			}
		}
	
		private void LoadIndex()
		{
			if (this.SetIndex(HtmlHelp2Environment.CurrentFilterName))
			{
				searchTerm.Text = string.Empty;
				searchTerm.Items.Clear();
				filterCombobox.SelectedIndexChanged -= new EventHandler(this.FilterChanged);
				HtmlHelp2Environment.BuildFilterList(filterCombobox);
				filterCombobox.SelectedIndexChanged += new EventHandler(this.FilterChanged);
			}
		}

		private bool SetIndex(string filterName)
		{
			try
			{
				indexControl.IndexData =
					HtmlHelp2Environment.GetIndex(HtmlHelp2Environment.FindFilterQuery(filterName));
				return true;
			}
			catch (System.Runtime.InteropServices.COMException)
			{
				LoggingService.Error("Help 2.0: cannot connect to IHxIndex interface (Index)");
				return false;
			}
		}

		private void ShowSelectedItemEntry(string indexTerm, int indexSlot)
		{
			PadDescriptor indexResults =
				WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2IndexResultsPad));
			if (indexResults == null) return;

			try
			{
				IHxTopicList matchingTopics = indexControl.IndexData.GetTopicsFromSlot(indexSlot);

				try
				{
					((HtmlHelp2IndexResultsPad)indexResults.PadContent).CleanUp();
					((HtmlHelp2IndexResultsPad)indexResults.PadContent).IndexResultsListView.BeginUpdate();

					foreach (IHxTopic topic in matchingTopics)
					{
						ListViewItem lvi = new ListViewItem();
						lvi.Text =
							topic.get_Title(HxTopicGetTitleType.HxTopicGetRLTitle,
							                HxTopicGetTitleDefVal.HxTopicGetTitleFileName);
						lvi.Tag = topic;
						lvi.SubItems.Add(topic.Location);
						((HtmlHelp2IndexResultsPad)indexResults.PadContent).IndexResultsListView.Items.Add(lvi);
					}
				}
				finally
				{
					((HtmlHelp2IndexResultsPad)indexResults.PadContent).IndexResultsListView.EndUpdate();
					((HtmlHelp2IndexResultsPad)indexResults.PadContent).SortLV(0);
					((HtmlHelp2IndexResultsPad)indexResults.PadContent).SetStatusMessage(indexTerm);
				}

				switch (matchingTopics.Count)
				{
					case 0:
						break;
					case 1:
						IHxTopic topic = (IHxTopic)matchingTopics.ItemAt(1);
						if(topic != null) ShowHelpBrowser.OpenHelpView(topic);
						break;
					default:
						indexResults.BringPadToFront();
						break;
				}
			}
			catch (System.Runtime.InteropServices.COMException cEx)
			{
				LoggingService.Error("Help 2.0: cannot get matching index entries; " + cEx.ToString());
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
				this.SetIndex(HtmlHelp2Environment.CurrentFilterName);
				filterCombobox.SelectedIndexChanged += new EventHandler(this.FilterChanged);
			}
		}

		private void NamespaceReloaded(object sender, EventArgs e)
		{
			this.UpdateControls();
		}
		#endregion
	}
}
