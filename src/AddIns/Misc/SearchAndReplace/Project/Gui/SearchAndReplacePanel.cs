// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.ComponentModel;
using System.Linq;
using System.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.SharpDevelop.Editor;
using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace SearchAndReplace
{
	public class SearchAndReplacePanel : BaseSharpDevelopUserControl
	{
		SearchAndReplaceMode searchAndReplaceMode;
		ITextEditor textEditor;
		ISegment selection;
		
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
						SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("SearchAndReplace.Resources.FindPanel.xfrm"));
						Get<Button>("bookmarkAll").Click += BookmarkAllButtonClicked;
						Get<Button>("findAll").Click += FindAllButtonClicked;
						this.ParentForm.AcceptButton = Get<Button>("findNext");
						break;
					case SearchAndReplaceMode.Replace:
						SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("SearchAndReplace.Resources.ReplacePanel.xfrm"));
						Get<Button>("replace").Click += ReplaceButtonClicked;
						Get<Button>("replaceAll").Click += ReplaceAllButtonClicked;
						this.ParentForm.AcceptButton = Get<Button>("replace");
						break;
				}
				
				Get<ComboBox>("find").TextChanged += FindPatternChanged;
				ControlDictionary["findNextButton"].Click     += FindNextButtonClicked;
				ControlDictionary["lookInBrowseButton"].Click += LookInBrowseButtonClicked;
				((Form)Parent).AcceptButton = (Button)ControlDictionary["findNextButton"];
				SetOptions();
				EnableButtons(HasFindPattern);
				RightToLeftConverter.ReConvertRecursive(this);
				ResumeLayout(false);
			}
		}
		
		public SearchAndReplacePanel()
		{
		}
		
		protected override void Dispose(bool disposing)
		{
			RemoveSelectionChangedHandler();
			RemoveActiveWindowChangedHandler();
			base.Dispose(disposing);
		}
		
		public SearchTarget SearchTarget {
			get {
				return (SearchTarget)(Get<ComboBox>("lookIn").SelectedIndex);
			}
			set {
				Get<ComboBox>("lookIn").SelectedIndex = (int)value;
			}
		}
		
		void LookInBrowseButtonClicked(object sender, EventArgs e)
		{
			ComboBox lookinComboBox = Get<ComboBox>("lookIn");
			using (FolderBrowserDialog dlg = FileService.CreateFolderBrowserDialog("${res:Dialog.NewProject.SearchReplace.LookIn.SelectDirectory}", lookinComboBox.Text)) {
				if (dlg.ShowDialog() == DialogResult.OK) {
					lookinComboBox.SelectedIndex = customDirectoryIndex;
					lookinComboBox.Text = dlg.SelectedPath;
				}
			}
		}
		
		SearchResultMatch lastMatch;
		
		void FindNextButtonClicked(object sender, EventArgs e)
		{
			WritebackOptions();
			lastMatch = SearchManager.FindNext(SearchOptions.FindPattern, !SearchOptions.MatchCase, SearchOptions.MatchWholeWord, SearchOptions.SearchStrategyType, SearchOptions.SearchTarget, SearchOptions.LookIn, SearchOptions.LookInFiletypes, SearchOptions.IncludeSubdirectories);
			SearchManager.SelectResult(lastMatch);
			Focus();
		}
		
		void FindAllButtonClicked(object sender, EventArgs e)
		{
			WritebackOptions();
			var monitor = WorkbenchSingleton.StatusBar.CreateProgressMonitor();
			monitor.TaskName = StringParser.Parse("${res:AddIns.SearchReplace.SearchProgressTitle}");
			try {
				var results = SearchManager.FindAllParallel(monitor, SearchOptions.FindPattern, !SearchOptions.MatchCase, SearchOptions.MatchWholeWord, SearchOptions.SearchStrategyType, SearchOptions.SearchTarget, SearchOptions.LookIn, SearchOptions.LookInFiletypes, SearchOptions.IncludeSubdirectories, selection);
				SearchManager.ShowSearchResults(SearchOptions.FindPattern, results);
			} catch (OperationCanceledException) {}
		}
		
		void BookmarkAllButtonClicked(object sender, EventArgs e)
		{
			WritebackOptions();
			var monitor = WorkbenchSingleton.StatusBar.CreateProgressMonitor();
			monitor.TaskName = StringParser.Parse("${res:AddIns.SearchReplace.SearchProgressTitle}");
			try {
				var results = SearchManager.FindAllParallel(monitor, SearchOptions.FindPattern, !SearchOptions.MatchCase, SearchOptions.MatchWholeWord, SearchOptions.SearchStrategyType, SearchOptions.SearchTarget, SearchOptions.LookIn, SearchOptions.LookInFiletypes, SearchOptions.IncludeSubdirectories);
				SearchManager.MarkAll(results);
			} catch (OperationCanceledException) {}
		}
		
		void ReplaceAllButtonClicked(object sender, EventArgs e)
		{
			WritebackOptions();
			int count = -1;
			AsynchronousWaitDialog.RunInCancellableWaitDialog(
				StringParser.Parse("${res:AddIns.SearchReplace.SearchProgressTitle}"), null,
				monitor => {
					var results = SearchManager.FindAll(monitor, SearchOptions.FindPattern, !SearchOptions.MatchCase, SearchOptions.MatchWholeWord, SearchOptions.SearchStrategyType, SearchOptions.SearchTarget, SearchOptions.LookIn, SearchOptions.LookInFiletypes, SearchOptions.IncludeSubdirectories, selection);
					count = SearchManager.ReplaceAll(results, SearchOptions.ReplacePattern, monitor.CancellationToken);
				});
			if (count != -1)
				SearchManager.ShowReplaceDoneMessage(count);
		}
		
		void ReplaceButtonClicked(object sender, EventArgs e)
		{
			WritebackOptions();
			if (SearchManager.IsResultSelected(lastMatch))
				SearchManager.Replace(lastMatch, SearchOptions.ReplacePattern);
			lastMatch = SearchManager.FindNext(SearchOptions.FindPattern, !SearchOptions.MatchCase, SearchOptions.MatchWholeWord, SearchOptions.SearchStrategyType, SearchOptions.SearchTarget, SearchOptions.LookIn, SearchOptions.LookInFiletypes, SearchOptions.IncludeSubdirectories);
			SearchManager.SelectResult(lastMatch);
			Focus();
		}
		
		void WritebackOptions()
		{
			SearchOptions.FindPattern = Get<ComboBox>("find").Text;
			
			if (searchAndReplaceMode == SearchAndReplaceMode.Replace) {
				SearchOptions.ReplacePattern = Get<ComboBox>("replace").Text;
			}
			
			if (Get<ComboBox>("lookIn").DropDownStyle == ComboBoxStyle.DropDown) {
				SearchOptions.LookIn = Get<ComboBox>("lookIn").Text;
			}
			SearchOptions.LookInFiletypes = Get<ComboBox>("fileTypes").Text;
			SearchOptions.MatchCase = Get<CheckBox>("matchCase").Checked;
			SearchOptions.MatchWholeWord = Get<CheckBox>("matchWholeWord").Checked;
			SearchOptions.IncludeSubdirectories = Get<CheckBox>("includeSubFolder").Checked;
			
			SearchOptions.SearchStrategyType = (SearchMode)Get<ComboBox>("use").SelectedIndex;
			if (Get<ComboBox>("lookIn").DropDownStyle == ComboBoxStyle.DropDown) {
				SearchOptions.SearchTarget = SearchTarget.Directory;
			} else {
				SearchOptions.SearchTarget = (SearchTarget)Get<ComboBox>("lookIn").SelectedIndex;
			}
		}
		
		const int customDirectoryIndex = 5;
		
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
			foreach (string lookInText in typeof(SearchTarget).GetFields().SelectMany(f => f.GetCustomAttributes(false).OfType<DescriptionAttribute>()).Select(da => da.Description)) {
				Get<ComboBox>("lookIn").Items.Add(StringParser.Parse(lookInText));
			}
			Get<ComboBox>("lookIn").Items.Add(SearchOptions.LookIn);
			Get<ComboBox>("lookIn").SelectedIndexChanged += new EventHandler(LookInSelectedIndexChanged);
			
			if (IsMultipleLineSelection(SearchManager.GetActiveTextEditor())) {
				SearchTarget = SearchTarget.CurrentSelection;
			} else {
				if (SearchOptions.SearchTarget == SearchTarget.CurrentSelection) {
					SearchOptions.SearchTarget = SearchTarget.CurrentDocument;
				}
				SearchTarget = SearchOptions.SearchTarget;
			}
			
			Get<ComboBox>("fileTypes").Text         = SearchOptions.LookInFiletypes;
			Get<CheckBox>("matchCase").Checked      = SearchOptions.MatchCase;
			Get<CheckBox>("matchWholeWord").Checked = SearchOptions.MatchWholeWord;
			Get<CheckBox>("includeSubFolder").Checked = SearchOptions.IncludeSubdirectories;
			
			Get<ComboBox>("use").Items.Clear();
			Get<ComboBox>("use").Items.Add(StringParser.Parse("${res:Dialog.NewProject.SearchReplace.SearchStrategy.Standard}"));
			Get<ComboBox>("use").Items.Add(StringParser.Parse("${res:Dialog.NewProject.SearchReplace.SearchStrategy.RegexSearch}"));
			Get<ComboBox>("use").Items.Add(StringParser.Parse("${res:Dialog.NewProject.SearchReplace.SearchStrategy.WildcardSearch}"));
			switch (SearchOptions.SearchStrategyType) {
				case SearchMode.RegEx:
					Get<ComboBox>("use").SelectedIndex = 1;
					break;
				case SearchMode.Wildcard:
					Get<ComboBox>("use").SelectedIndex = 2;
					break;
				default:
					Get<ComboBox>("use").SelectedIndex = 0;
					break;
			}
		}
		
		void LookInSelectedIndexChanged(object sender, EventArgs e)
		{
			if (Get<ComboBox>("lookIn").SelectedIndex == customDirectoryIndex) {
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
			if (IsSelectionSearch) {
				InitSelectionSearch();
			} else {
				RemoveSelectionSearchHandlers();
			}
		}
		
		bool IsSelectionSearch {
			get {
				return SearchTarget == SearchTarget.CurrentSelection;
			}
		}
		
		/// <summary>
		/// Checks whether the selection spans two or more lines.
		/// </summary>
		static bool IsMultipleLineSelection(ITextEditor editor)
		{
			if (editor == null)
				return false;
			else
				return editor.SelectedText.IndexOf('\n') != -1;
		}
		
		/// <summary>
		/// Returns the first ISelection object from the currently active text editor
		/// </summary>
		static ISegment GetCurrentTextSelection()
		{
			ITextEditor textArea = SearchManager.GetActiveTextEditor();
			if (textArea != null) {
				return new TextSegment { StartOffset = textArea.SelectionStart, Length = textArea.SelectionLength };
			}
			return null;
		}
		
		void WorkbenchActiveViewContentChanged(object source, EventArgs e)
		{
			ITextEditor activeTextEditorControl = SearchManager.GetActiveTextEditor();
			if (activeTextEditorControl != this.textEditor) {
				AddSelectionChangedHandler(activeTextEditorControl);
				TextSelectionChanged(source, e);
			}
		}
		
		void AddSelectionChangedHandler(ITextEditor textEditor)
		{
			RemoveSelectionChangedHandler();
			
			this.textEditor = textEditor;
			if (textEditor != null) {
				this.textEditor.SelectionChanged += TextSelectionChanged;
			}
		}
		
		void RemoveSelectionChangedHandler()
		{
			if (textEditor != null) {
				textEditor.SelectionChanged -= TextSelectionChanged;
			}
		}
		
		void RemoveActiveWindowChangedHandler()
		{
			WorkbenchSingleton.Workbench.ActiveViewContentChanged -= WorkbenchActiveViewContentChanged;
		}
		
		/// <summary>
		/// When the selected text is changed make sure the 'Current Selection'
		/// option is not selected if no text is selected.
		/// </summary>
		/// <remarks>The text selection can change either when the user
		/// selects different text in the editor or the active window is
		/// changed.</remarks>
		void TextSelectionChanged(object source, EventArgs e)
		{
			LoggingService.Debug("TextSelectionChanged.");
			selection = GetCurrentTextSelection();
		}
		
		void InitSelectionSearch()
		{
			selection = GetCurrentTextSelection();
			AddSelectionChangedHandler(SearchManager.GetActiveTextEditor());
			WorkbenchSingleton.Workbench.ActiveViewContentChanged += WorkbenchActiveViewContentChanged;
		}
		
		void RemoveSelectionSearchHandlers()
		{
			RemoveSelectionChangedHandler();
			RemoveActiveWindowChangedHandler();
		}
		
		/// <summary>
		/// Enables the various find, bookmark and replace buttons
		/// depending on whether any find string has been entered. The buttons
		/// are disabled otherwise.
		/// </summary>
		void EnableButtons(bool enabled)
		{
			if (searchAndReplaceMode == SearchAndReplaceMode.Replace) {
				Get<Button>("replace").Enabled = enabled;
				Get<Button>("replaceAll").Enabled = enabled;
			} else {
				Get<Button>("bookmarkAll").Enabled = enabled;
				Get<Button>("findAll").Enabled = enabled;
			}
			ControlDictionary["findNextButton"].Enabled = enabled;
		}
		
		/// <summary>
		/// Returns true if the string entered in the find or replace text box
		/// is not an empty string.
		/// </summary>
		bool HasFindPattern {
			get {
				return Get<ComboBox>("find").Text.Length != 0;
			}
		}
		
		/// <summary>
		/// Updates the enabled/disabled state of the search and replace buttons
		/// after the search or replace text has changed.
		/// </summary>
		void FindPatternChanged(object source, EventArgs e)
		{
			EnableButtons(HasFindPattern);
		}
	}
}
