// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace Plugins.RegExpTk {

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
		
		
		private ErrorProvider compileErrorProvider;
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
			((Button)ControlDictionary["ChooseAssemblyFileCompileButton"]).Click += new EventHandler(ChooseAssemblyFileCompileButton_Click);
			((Button)ControlDictionary["CreateAssemblyFileCompileButton"]).Click += new EventHandler(CreateAssemblyFile);
			((Button)ControlDictionary["quickInsertButton"]).MouseDown += new MouseEventHandler(showQuickInsertMenu);
			((Button)ControlDictionary["quickInsertButton"]).Image = WinFormsResourceService.GetBitmap("Icons.16x16.PasteIcon");
			ControlDictionary["RegularExpressionTextBox"].KeyPress += delegate(object sender, KeyPressEventArgs e) {
				if (e.KeyChar == '\r') { OkButton_Click(null, null); e.Handled = true; }
			};
			((RichTextBox)ControlDictionary["InputTextBox"]).DetectUrls = false;
			
			ReplaceCheckBox_CheckedChanged((CheckBox)ControlDictionary["ReplaceCheckBox"], null);

			this.Width=Screen.PrimaryScreen.WorkingArea.Width / 2;
			
			((TextBox)ControlDictionary["RegularExpressionTextBox"]).TextChanged+=new EventHandler(SetRegEx);
			FormLocationHelper.Apply(this, "RegExpTk.WindowBounds", true);
		}
		
		
		private void SetRegEx(object sender, EventArgs ea) {
			((TextBox)ControlDictionary["RegularExpressionCompileTextBox"]).Text=((TextBox)ControlDictionary["RegularExpressionTextBox"]).Text;
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
			groupform.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWin32Window);
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
		
		private void CreateAssemblyFile(object sender, EventArgs e)
		{
			RegexOptions options = RegexOptions.Compiled;
			
			if(compileErrorProvider != null) {
				compileErrorProvider.Dispose();
				compileErrorProvider = null;
			}
			compileErrorProvider = new ErrorProvider();
			
			// validate input
			
			bool error = false;
			
			if(((TextBox)ControlDictionary["ClassNameCompileTextBox"]).Text == "") {
				compileErrorProvider.SetError((TextBox)ControlDictionary["ClassNameCompileTextBox"], ResourceService.GetString("RegExpTk.Messages.ClassNameMissing"));
				error = true;
			}
			
			if(ControlDictionary["RegularExpressionCompileTextBox"].Text == "") {
				compileErrorProvider.SetError((TextBox)ControlDictionary["RegularExpressionCompileTextBox"], ResourceService.GetString("RegExpTk.Messages.RegexMissing"));
				error = true;
			}
			
			if(((TextBox)ControlDictionary["AssemblyFileCompileFileTextBox"]).Text == "") {
				compileErrorProvider.SetError((TextBox)ControlDictionary["AssemblyFileCompileFileTextBox"], ResourceService.GetString("RegExpTk.Messages.FilenameMissing"));
				error = true;
			}
			
			string file_ = ((TextBox)ControlDictionary["AssemblyFileCompileFileTextBox"]).Text;
			
			if(! FileUtility.IsValidPath(((TextBox)ControlDictionary["AssemblyFileCompileFileTextBox"]).Text)) {
				compileErrorProvider.SetError((TextBox)ControlDictionary["AssemblyFileCompileFileTextBox"], ResourceService.GetString("RegExpTk.Messages.FilenameInvalid"));
				error = true;
			}
			
			if(error) return;
			
			// set options
			if(((CheckBox)ControlDictionary["IgnoreCaseCompileCheckBox"]).Checked)
				options = options | RegexOptions.IgnoreCase;
			
			if(((CheckBox)ControlDictionary["SingleLineCompileCheckBox"]).Checked)
				options = options | RegexOptions.Singleline;
			
			if(((CheckBox)ControlDictionary["IgnoreWhitespaceCompileCheckBox"]).Checked)
				options = options | RegexOptions.IgnorePatternWhitespace;
			
			if(((CheckBox)ControlDictionary["ExplicitCaptureCompileCheckBox"]).Checked)
				options = options | RegexOptions.ExplicitCapture;
			
			if(((CheckBox)ControlDictionary["EcmaScriptCompileCheckBox"]).Checked)
				options = options | RegexOptions.ECMAScript;
			
			if(((CheckBox)ControlDictionary["MultilineCompileCheckBox"]).Checked)
				options = options | RegexOptions.Multiline;
			
			if(((CheckBox)ControlDictionary["RightToLeftCompileCheckBox"]).Checked)
				options = options | RegexOptions.RightToLeft;
			
			try {
				Regex re = new Regex(((TextBox)ControlDictionary["RegularExpressionCompileTextBox"]).Text, options);
			} catch (ArgumentException ae) {
				MessageService.ShowError(ResourceService.GetString("RegExpTk.Messages.CreationError") + " " + ae.Message);
				return;
			}
			
			RegexCompilationInfo rci = new RegexCompilationInfo(((TextBox)ControlDictionary["RegularExpressionCompileTextBox"]).Text,
			                                                     options,
			                                                     ((TextBox)ControlDictionary["ClassNameCompileTextBox"]).Text,
			                                                     ((TextBox)ControlDictionary["NamespaceCompileTextBox"]).Text,
			                                                     ((CheckBox)ControlDictionary["PublibVisibleCompileCheckBox"]).Checked);
			
			AssemblyName asmName = new AssemblyName();
			asmName.Name = Path.GetFileNameWithoutExtension(((TextBox)ControlDictionary["AssemblyFileCompileFileTextBox"]).Text);
			
			RegexCompilationInfo[] rciArray = new RegexCompilationInfo[] { rci };
			
			try {
				Regex.CompileToAssembly(rciArray, asmName);
			} catch (ArgumentException ae) {
				MessageService.ShowError(ResourceService.GetString("RegExpTk.Messages.CompilationError") + " " + ae.Message);
				return;
			}
			
			string aboluteFileName = FileUtility.NormalizePath(((TextBox)ControlDictionary["AssemblyFileCompileFileTextBox"]).Text);
			((StatusBar)ControlDictionary["StatusBar"]).Text = ResourceService.GetString("RegExpTk.Messages.FileCreated") + " " + aboluteFileName;
		}
		
		private void ChooseAssemblyFileCompileButton_Click(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			
			sfd.InitialDirectory = "c:\\";
			sfd.Filter = ResourceService.GetString("RegExpTk.MainDialog.Assemblies");
			sfd.DefaultExt = "dll";
			sfd.CheckPathExists = true;
			
			if (sfd.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
				((TextBox)ControlDictionary["AssemblyFileCompileFileTextBox"]).Text = sfd.FileName;
			}
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
