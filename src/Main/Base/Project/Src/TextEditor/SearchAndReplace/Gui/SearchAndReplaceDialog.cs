// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace SearchAndReplace
{
	public enum SearchAndReplaceMode {
		Search,
		Replace
	}
	
	public class SearchAndReplaceDialog : Form
	{
		public static string SearchPattern  = "";
		public static string ReplacePattern = "";
		
		static SearchAndReplaceDialog Instance;
		
		public static void ShowSingleInstance(SearchAndReplaceMode searchAndReplaceMode)
		{
			if (Instance == null) {
				Instance = new SearchAndReplaceDialog(searchAndReplaceMode);
				Instance.Show(WorkbenchSingleton.MainForm);
			} else {
				if (searchAndReplaceMode == SearchAndReplaceMode.Search) {
					Instance.searchButton.PerformClick();
				} else {
					Instance.replaceButton.PerformClick();
				}
				Instance.Focus();
			}
		}
		
		ToolStripButton searchButton = new ToolStripButton();
		ToolStripButton replaceButton = new ToolStripButton();
		
		SearchAndReplacePanel searchAndReplacePanel;
		
		public SearchAndReplaceDialog(SearchAndReplaceMode searchAndReplaceMode)
		{
			this.Owner           = WorkbenchSingleton.MainForm;
			this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
			this.ShowInTaskbar   = false;
			this.TopMost         = false;
			this.Text            = StringParser.Parse("${res:Dialog.NewProject.SearchReplace.Title}");
			this.KeyPreview = true;
			
			searchAndReplacePanel = new SearchAndReplacePanel();
			searchAndReplacePanel.Dock = DockStyle.Fill;
			Controls.Add(searchAndReplacePanel);
			
			ToolStrip toolStrip = new ToolStrip();
			toolStrip.Dock = DockStyle.Top;
			toolStrip.Stretch   = true;
			toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			
			searchButton.Text = StringParser.Parse("${res:Dialog.NewProject.SearchReplace.FindDialogName}");
			searchButton.Image = IconService.GetBitmap("Icons.16x16.FindIcon");
			searchButton.Checked = searchAndReplaceMode == SearchAndReplaceMode.Search;
			searchButton.Click += new EventHandler(SearchButtonClick);
			toolStrip.Items.Add(searchButton);
			
			replaceButton.Text = StringParser.Parse("${res:Dialog.NewProject.SearchReplace.ReplaceDialogName}");
			replaceButton.Image = IconService.GetBitmap("Icons.16x16.ReplaceIcon");
			replaceButton.Checked = searchAndReplaceMode == SearchAndReplaceMode.Replace;
			replaceButton.Click += new EventHandler(ReplaceButtonClick);
			toolStrip.Items.Add(replaceButton);
			
			Controls.Add(toolStrip);
			RightToLeftConverter.ConvertRecursive(this);
			
			SetSearchAndReplaceMode();
			FormLocationHelper.Apply(this, "ICSharpCode.SharpDevelop.Gui.SearchAndReplaceDialog.Location", false);
		}
		
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);
			Instance = null;
		}
		
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyData == Keys.Escape) {
				Close();
			}
		}
		
		
		void SearchButtonClick(object sender, EventArgs e)
		{
			if (!searchButton.Checked) {
				searchButton.Checked = true;
				replaceButton.Checked = false;
				SetSearchAndReplaceMode();
				Focus();
			}
		}
		
		void ReplaceButtonClick(object sender, EventArgs e)
		{
			if (!replaceButton.Checked) {
				replaceButton.Checked = true;
				searchButton.Checked = false;
				SetSearchAndReplaceMode();
				Focus();
			}
		}
		
		void SetSearchAndReplaceMode()
		{
			searchAndReplacePanel.SearchAndReplaceMode = searchButton.Checked ? SearchAndReplaceMode.Search : SearchAndReplaceMode.Replace;
			if (searchButton.Checked) {
				this.ClientSize      = new Size(430, 335);
			} else {
				this.ClientSize      = new Size(430, 385);
			}
		}
		
	}
}
