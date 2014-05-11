// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
				Instance.Show(SD.WinForms.MainWin32Window);
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
			SuspendLayout();
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
			
			this.AutoScaleMode = AutoScaleMode.Dpi;
			this.AutoScaleDimensions = new SizeF(96, 96);
			ResumeLayout();
			
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
				e.Handled = true;
			} else if (replaceKeyboardShortcut == e.KeyData && !replaceButton.Checked) {
				EnableSearchMode(false);
				e.Handled = true;
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
			SuspendLayout();
			searchAndReplacePanel.SearchAndReplaceMode = searchButton.Checked ? SearchAndReplaceMode.Search : SearchAndReplaceMode.Replace;
			this.AutoScaleMode = AutoScaleMode.Dpi;
			this.AutoScaleDimensions = new SizeF(96, 96);
			if (searchButton.Checked) {
				this.ClientSize      = new Size(430, 335);
			} else {
				this.ClientSize      = new Size(430, 385);
			}
			ResumeLayout();
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
						return (Keys)new KeysConverter().ConvertFromInvariantString(codon.Properties["shortcut"].Replace('|', '+'));
					}
				}
			}
			return Keys.None;
		}
	}
}
