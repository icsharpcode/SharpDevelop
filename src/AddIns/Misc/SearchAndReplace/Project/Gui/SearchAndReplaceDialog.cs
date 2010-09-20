// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace SearchAndReplace
{
	public enum SearchAndReplaceMode {
		Search,
		Replace
	}
	
	public class SearchAndReplaceDialog : Form
	{
		public static string SearchPattern  = String.Empty;
		public static string ReplacePattern = String.Empty;
		
		Keys searchKeyboardShortcut = Keys.None;
		Keys replaceKeyboardShortcut = Keys.None;
		const string SearchMenuAddInPath = "/SharpDevelop/Workbench/MainMenu/Search";

		static SearchAndReplaceDialog Instance;
		
		public static void ShowSingleInstance(SearchAndReplaceMode searchAndReplaceMode)
		{
			if (Instance == null) {
				Instance = new SearchAndReplaceDialog(searchAndReplaceMode);
				Instance.Show(WorkbenchSingleton.MainWin32Window);
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
			
			searchKeyboardShortcut = GetKeyboardShortcut(SearchMenuAddInPath, "Find");
			replaceKeyboardShortcut = GetKeyboardShortcut(SearchMenuAddInPath, "Replace");
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
			} else if (searchKeyboardShortcut == e.KeyData && !searchButton.Checked) {
				EnableSearchMode(true);
			} else if (replaceKeyboardShortcut == e.KeyData && !replaceButton.Checked) {
				EnableSearchMode(false);
			}
		}
		
		void SearchButtonClick(object sender, EventArgs e)
		{
			if (!searchButton.Checked) {
				EnableSearchMode(true);
			}
		}
		
		void ReplaceButtonClick(object sender, EventArgs e)
		{
			if (!replaceButton.Checked) {
				EnableSearchMode(false);
			}
		}
		
		void EnableSearchMode(bool enable)
		{
			searchButton.Checked = enable;
			replaceButton.Checked = !enable;
			SetSearchAndReplaceMode();
			Focus();
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
		
		/// <summary>
		/// Gets the keyboard shortcut for the menu item with the given addin tree
		/// path and given codon id.
		/// </summary>
		Keys GetKeyboardShortcut(string path, string id)
		{
			AddInTreeNode node = AddInTree.GetTreeNode(path);
			if (node != null) {
				foreach (Codon codon in node.Codons) {
					if (codon.Id == id) {
						return MenuCommand.ParseShortcut(codon.Properties["shortcut"]);
					}
				}
			}
			return Keys.None;
		}
	}
}
