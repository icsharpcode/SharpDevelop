// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.TextEditor;

namespace SearchAndReplace
{
	/// <summary>
	/// Description of SearchAndReplacePanel.
	/// </summary>
	public class SearchAndReplacePanel : BaseSharpDevelopUserControl
	{
		SearchAndReplaceMode  searchAndReplaceMode;
		
		public SearchAndReplaceMode SearchAndReplaceMode {
			get {
				return searchAndReplaceMode;
			}
			set {
				searchAndReplaceMode = value;
				SuspendLayout();
				Controls.Clear();
				switch (searchAndReplaceMode) {
					case SearchAndReplaceMode.Search:
						SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.FindPanel.xfrm"));
						Get<Button>("findAll").Click += new EventHandler(FindAllButtonClicked);
						break;
					case SearchAndReplaceMode.Replace:
						SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.ReplacePanel.xfrm"));
						break;
				}
				
				Get<Button>("findNext").Click += new EventHandler(FindNextButtonClicked);
				SetOptions();
				ResumeLayout(false);
			}
		}
		
		public SearchAndReplacePanel()
		{
		}
		
		void FindNextButtonClicked(object sender, EventArgs e)
		{
			WritebackOptions();
			SearchReplaceManager.FindNext();
		}
		
		void FindAllButtonClicked(object sender, EventArgs e)
		{
			WritebackOptions();
			SearchReplaceInFilesManager.FindAll();
		}
		
		void WritebackOptions()
		{
			SearchOptions.FindPattern = Get<ComboBox>("find").Text;
			
			if (searchAndReplaceMode == SearchAndReplaceMode.Replace) {
				SearchOptions.ReplacePattern = Get<ComboBox>("replace").Text;
			}
			
			SearchOptions.LookIn = Get<ComboBox>("lookIn").Text;
			SearchOptions.LookInFiletypes = Get<ComboBox>("fileTypes").Text;
			SearchOptions.MatchCase = Get<CheckBox>("matchCase").Checked;
			SearchOptions.MatchWholeWord = Get<CheckBox>("matchWholeWord").Checked;
			SearchOptions.IncludeSubdirectories = Get<CheckBox>("includeSubFolder").Checked;
			
			SearchOptions.SearchStrategyType = (SearchStrategyType)Get<ComboBox>("use").SelectedIndex;
			SearchOptions.DocumentIteratorType = (DocumentIteratorType)Get<ComboBox>("lookIn").SelectedIndex;
		}
		
		void SetOptions()
		{
			Get<ComboBox>("find").Text = SearchOptions.FindPattern;
			Get<ComboBox>("find").Items.Clear();
			
			Get<ComboBox>("find").Text = SearchOptions.FindPattern;
			Get<ComboBox>("find").Items.Clear();
			foreach (string findPattern in SearchOptions.FindPatterns) {
				Get<ComboBox>("find").Items.Add(findPattern);
			}
				
			if (searchAndReplaceMode == SearchAndReplaceMode.Replace) {
				Get<ComboBox>("replace").Text = SearchOptions.ReplacePattern;
				Get<ComboBox>("replace").Items.Clear();
				foreach (string replacePattern in SearchOptions.ReplacePatterns) {
					Get<ComboBox>("replace").Items.Add(replacePattern);
				}
			}
			
			Get<ComboBox>("lookIn").Text = SearchOptions.LookIn;
			Get<ComboBox>("lookIn").Items.Add("current document");
			Get<ComboBox>("lookIn").Items.Add("current selection");
			Get<ComboBox>("lookIn").Items.Add("all open documents");
			Get<ComboBox>("lookIn").Items.Add("whole project");
			Get<ComboBox>("lookIn").Items.Add("whole solution");
			Get<ComboBox>("lookIn").Items.Add(SearchOptions.LookIn);
			Get<ComboBox>("lookIn").DropDownStyle = ComboBoxStyle.DropDownList;
			Get<ComboBox>("lookIn").SelectedIndexChanged += new EventHandler(LookInSelectedIndexChanged);
			Get<ComboBox>("lookIn").SelectedIndex = (int)SearchOptions.DocumentIteratorType;
			
			Get<ComboBox>("fileTypes").Text         = SearchOptions.LookInFiletypes;
			Get<CheckBox>("matchCase").Checked      = SearchOptions.MatchCase;
			Get<CheckBox>("matchWholeWord").Checked = SearchOptions.MatchWholeWord;
			Get<CheckBox>("includeSubFolder").Checked = SearchOptions.IncludeSubdirectories;
			
			Get<ComboBox>("use").Items.Clear();
			Get<ComboBox>("use").Items.Add("Standard search");
			Get<ComboBox>("use").Items.Add("Regular expressions");
			Get<ComboBox>("use").Items.Add("Wildcards");
			switch (SearchOptions.SearchStrategyType) {
				case SearchStrategyType.RegEx:
					Get<ComboBox>("use").SelectedIndex = 1;
					break;
				case SearchStrategyType.Wildcard:
					Get<ComboBox>("use").SelectedIndex = 2;
					break;
				default:
					Get<ComboBox>("use").SelectedIndex = 0;
					break;
			}
		}
		
		void LookInSelectedIndexChanged(object sender, EventArgs e)
		{
			if (Get<ComboBox>("lookIn").SelectedIndex == 5) {
				Get<ComboBox>("lookIn").DropDownStyle = ComboBoxStyle.DropDown;
				Get<CheckBox>("includeSubFolder").Enabled = true;
				Get<ComboBox>("fileTypes").Enabled = true;
				Get<Label>("lookAtTypes").Enabled = true;
			} else {
				Get<ComboBox>("lookIn").DropDownStyle = ComboBoxStyle.DropDownList;
				Get<CheckBox>("includeSubFolder").Enabled = false;
				Get<ComboBox>("fileTypes").Enabled = false;
				Get<Label>("lookAtTypes").Enabled = false;
			}
		}
	}
}
