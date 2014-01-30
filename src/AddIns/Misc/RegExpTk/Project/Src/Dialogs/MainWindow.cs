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

// thanks to Chris Wille who contributed
// the compile stuff

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace Plugins.RegExpTk {
	// TODO: rewrite without XMLForms
	#pragma warning disable 618

	public class RegExpTkDialog : BaseSharpDevelopForm
	{
		
		class QuickInsert
		{
			string name;
			string text;
			
			public QuickInsert(string name, string text)
			{
				Name = StringParser.Parse(name);
				Text = text;
			}
			
			public string Name
			{
				get {
					return name;
				}
				set {
					name = value;
				}
			}
			
			public string Text
			{
				get {
					return text;
				}
				set {
					text = value;
				}
			}
		}
		
		
		private ContextMenuStrip quickInsertMenu          = new ContextMenuStrip();
		private ContextMenuStrip matchListViewContextMenu = new ContextMenuStrip();
		
		public RegExpTkDialog()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.RegExpTkMainForm.xfrm"));
			
			List<QuickInsert> quickies = new List<QuickInsert>();
			quickies.Add(new QuickInsert("${res:RegExpTk.RegExpMenu.UngreedyStar}", "*?"));
			quickies.Add(new QuickInsert("${res:RegExpTk.RegExpMenu.WordCharacter}", "\\w"));
			quickies.Add(new QuickInsert("${res:RegExpTk.RegExpMenu.NonWordCharacter}", "\\W"));
			quickies.Add(new QuickInsert("${res:RegExpTk.RegExpMenu.WhitespaceCharacter}", "\\s"));
			quickies.Add(new QuickInsert("${res:RegExpTk.RegExpMenu.NonWhitespaceCharacter}", "\\S"));
			quickies.Add(new QuickInsert("${res:RegExpTk.RegExpMenu.DigitCharacter}", "\\d"));
			quickies.Add(new QuickInsert("${res:RegExpTk.RegExpMenu.NonDigitCharacter}", "\\D"));
			quickies.Add(new QuickInsert("${res:RegExpTk.RegExpMenu.WordBorder}", "\\b"));
			
			foreach (QuickInsert insert in quickies) {
				MenuCommand cmd = new MenuCommand(insert.Name, new EventHandler(quickInsert));
				cmd.Tag           = insert.Text;
				quickInsertMenu.Items.Add(cmd);
			}
			
			matchListViewContextMenu.Items.Add(new MenuCommand("${res:RegExpTk.MainDialog.ShowGroups}", new EventHandler(MatchListViewContextMenu_Clicked)));
			
			((Button)ControlDictionary["OkButton"]).Click += new EventHandler(OkButton_Click);
			((CheckBox)ControlDictionary["ReplaceCheckBox"]).CheckedChanged += new EventHandler(ReplaceCheckBox_CheckedChanged);
			((ListView)ControlDictionary["GroupListView"]).SelectedIndexChanged += new EventHandler(GroupListView_SelectedIndexChanged);
			((ListView)ControlDictionary["GroupListView"]).DoubleClick += new EventHandler(GroupListView_DoubleClick);
			((ListView)ControlDictionary["GroupListView"]).MouseUp += new MouseEventHandler(GroupListView_MouseUp);
			((Button)ControlDictionary["quickInsertButton"]).MouseDown += new MouseEventHandler(showQuickInsertMenu);
			((Button)ControlDictionary["quickInsertButton"]).Image = WinFormsResourceService.GetBitmap("Icons.16x16.PasteIcon");
			ControlDictionary["RegularExpressionTextBox"].KeyPress += delegate(object sender, KeyPressEventArgs e) {
				if (e.KeyChar == '\r') { OkButton_Click(null, null); e.Handled = true; }
			};
			((RichTextBox)ControlDictionary["InputTextBox"]).DetectUrls = false;
			
			ReplaceCheckBox_CheckedChanged((CheckBox)ControlDictionary["ReplaceCheckBox"], null);

			this.Width=Screen.PrimaryScreen.WorkingArea.Width / 2;
			
			FormLocationHelper.Apply(this, "RegExpTk.WindowBounds", true);
		}
		
		
		void GroupListView_DoubleClick(object sender, EventArgs e)
		{
			if(((ListView)ControlDictionary["GroupListView"]).SelectedItems.Count > 0) {
				Match match = (Match)((ListView)ControlDictionary["GroupListView"]).SelectedItems[0].Tag;
				showGroupForm(match);
			}
		}
		
		void MatchListViewContextMenu_Clicked(object sender, EventArgs e)
		{
			Match match = (Match)((ListView)ControlDictionary["GroupListView"]).SelectedItems[0].Tag;
			showGroupForm(match);

		}
		
		void showGroupForm(Match match)
		{
			GroupForm groupform = new GroupForm(match);
			groupform.ShowDialog(SD.WinForms.MainWin32Window);
		}
		
		void GroupListView_MouseUp(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Right && ((ListView)ControlDictionary["GroupListView"]).SelectedItems.Count > 0) {
				Point cords = new Point(((ListView)ControlDictionary["GroupListView"]).Left + e.X, ((ListView)ControlDictionary["GroupListView"]).Top + e.Y + 30);
				matchListViewContextMenu.Show(this, cords);
			}
		}
		
		private void quickInsert(object sender, EventArgs e)
		{
			//// Alex: changed to text box
			((TextBox)ControlDictionary["RegularExpressionTextBox"]).SelectedText += (string)((MenuCommand)sender).Tag;
		}
		
		private void showQuickInsertMenu(object sender, MouseEventArgs e)
		{
			((TextBox)ControlDictionary["RegularExpressionTextBox"]).Select();
			Point cords = new Point(((Button)ControlDictionary["quickInsertButton"]).Left + e.X, ((Button)ControlDictionary["quickInsertButton"]).Top + e.Y + 30);
			quickInsertMenu.Show(this, cords);
		}
		
		private void OkButton_Click(object sender, System.EventArgs e)
		{
			MatchCollection matches = null;
			RegexOptions options = new RegexOptions();
			
			((TextBox)ControlDictionary["RegularExpressionTextBox"]).ForeColor = System.Drawing.Color.Black;
			
			if(((CheckBox)(ControlDictionary["IgnoreCaseCheckBox"])).Checked) {
				options = options | RegexOptions.IgnoreCase;
			}
			
			if(((CheckBox)(ControlDictionary["MultilineCheckBox"])).Checked) {
				options = options | RegexOptions.Multiline;
			}
			
			((ListView)ControlDictionary["GroupListView"]).Items.Clear();
			
			try {
				matches = Regex.Matches(((RichTextBox)ControlDictionary["InputTextBox"]).Text, ((TextBox)ControlDictionary["RegularExpressionTextBox"]).Text, options);
				if(((CheckBox)ControlDictionary["ReplaceCheckBox"]).Checked) {
					((TextBox)ControlDictionary["ReplaceResultTextBox"]).Text = Regex.Replace(((RichTextBox)ControlDictionary["InputTextBox"]).Text, ((TextBox)ControlDictionary["RegularExpressionTextBox"]).Text, ((TextBox)ControlDictionary["ReplacementStringTextBox"]).Text, options);
				}
			}
			catch(Exception exception) {
				((TextBox)ControlDictionary["RegularExpressionTextBox"]).ForeColor = System.Drawing.Color.Red;
				((StatusBar)ControlDictionary["StatusBar"]).Text = exception.Message;
				return;
			}
			
			if(matches.Count != 1) {
				((StatusBar)ControlDictionary["StatusBar"]).Text = matches.Count.ToString() + " " + ResourceService.GetString("RegExpTk.Messages.Match");
			} else {
				((StatusBar)ControlDictionary["StatusBar"]).Text = matches.Count.ToString() + " " + ResourceService.GetString("RegExpTk.Messages.Matches");
			}
			
			RichTextBox inputBox = (RichTextBox)ControlDictionary["InputTextBox"];
			
			TextBox dummy = new TextBox();
			dummy.Text = inputBox.Text;
			inputBox.Text =  dummy.Text;
			
			inputBox.Select(0, inputBox.Text.Length);
			inputBox.SelectionColor = Color.Black;
			inputBox.SelectionFont = dummy.Font;
			
			int colorIndex = 0;
			Color[] colors = new Color[] {Color.Blue, Color.Red, Color.DarkGreen,
				Color.DarkRed, Color.Navy, Color.DarkGray};
			
			foreach (Match match in matches) {
				ListViewItem lvwitem = ((ListView)ControlDictionary["GroupListView"]).Items.Add(match.ToString());
				lvwitem.Tag = match;
				lvwitem.SubItems.Add(match.Index.ToString());
				lvwitem.SubItems.Add((match.Index + match.Length).ToString());
				lvwitem.SubItems.Add(match.Length.ToString());
				lvwitem.SubItems.Add(match.Groups.Count.ToString());
				
				// the whole match is group #0
				foreach (Group g in match.Groups) {
					inputBox.Select(g.Index, g.Length);
					inputBox.SelectionColor = colors[colorIndex++ % colors.Length];
				}
			}
			inputBox.Select(0, 0);
		}
		
		private void GroupListView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				((RichTextBox)ControlDictionary["InputTextBox"]).Select(System.Convert.ToInt32(((ListView)ControlDictionary["GroupListView"]).SelectedItems[0].SubItems[1].Text),
				                                                        System.Convert.ToInt32(((ListView)ControlDictionary["GroupListView"]).SelectedItems[0].SubItems[3].Text));
			} catch {
			}
		}
		
		private void ReplaceCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			((TextBox)ControlDictionary["ReplaceResultTextBox"]).Enabled = ((CheckBox)ControlDictionary["ReplaceCheckBox"]).Checked;
			((TextBox)ControlDictionary["ReplacementStringTextBox"]).Enabled = ((CheckBox)ControlDictionary["ReplaceCheckBox"]).Checked;
		}
		
	}
}
