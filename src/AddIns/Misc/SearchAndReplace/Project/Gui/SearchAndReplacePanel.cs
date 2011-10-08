// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	class StoredSelection
	{
		public readonly int Offset, Length;
		
		public int EndOffset {
			get { return Offset + Length; }
		}
		
		public bool IsTextSelected {
			get { return Length > 0; }
		}
		
		public StoredSelection(int offset, int length)
		{
			this.Offset = offset;
			this.Length = length;
		}
	}
	
	public class SearchAndReplacePanel : BaseSharpDevelopUserControl
	{
		SearchAndReplaceMode  searchAndReplaceMode;
		StoredSelection selection;
		ITextEditor textEditor;
		bool ignoreSelectionChanges;
		bool findFirst;
		
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
		
		void FindNextButtonClicked(object sender, EventArgs e)
		{
			WritebackOptions();
			if (IsSelectionSearch) {
				if (selection.IsTextSelected) {
					FindNextInSelection();
				}
			} else {
				using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("Search", true))
				{
					monitor.Progress = double.NaN; // progress not implemented, use indeterminate progress
//					SearchReplaceManager.FindNext(monitor);
				}
			}
			Focus();
		}
		
		void FindAllButtonClicked(object sender, EventArgs e)
		{
			WritebackOptions();
			if (IsSelectionSearch) {
				if (selection.IsTextSelected) {
					RunAllInSelection(0);
				}
			} else {
				var results = SearchManager.FindAll(SearchOptions.FindPattern, !SearchOptions.MatchCase, SearchOptions.MatchWholeWord, SearchOptions.SearchStrategyType, SearchOptions.DocumentIteratorType, SearchOptions.LookIn, SearchOptions.LookInFiletypes, SearchOptions.IncludeSubdirectories);
				SearchResultsPad.Instance.ShowSearchResults("Search", results);
			}
		}
		
		void BookmarkAllButtonClicked(object sender, EventArgs e)
		{
			WritebackOptions();
			if (IsSelectionSearch) {
				if (selection.IsTextSelected) {
					RunAllInSelection(1);
				}
			} else {
				using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("Search", true))
				{
					monitor.Progress = double.NaN; // progress not implemented, use indeterminate progress
//					SearchReplaceManager.MarkAll(monitor);
				}
			}
		}
		
		void ReplaceAllButtonClicked(object sender, EventArgs e)
		{
			WritebackOptions();
			if (IsSelectionSearch) {
				if (selection.IsTextSelected) {
					RunAllInSelection(2);
				}
			} else {
				using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("Search", true))
				{
					monitor.Progress = double.NaN; // progress not implemented, use indeterminate progress
//					SearchReplaceManager.ReplaceAll(monitor);
				}
			}
		}
		
		void ReplaceButtonClicked(object sender, EventArgs e)
		{
			WritebackOptions();
			if (IsSelectionSearch) {
				if (selection.IsTextSelected) {
					ReplaceInSelection();
				}
			} else {
				using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("Search", true))
				{
					monitor.Progress = double.NaN; // progress not implemented, use indeterminate progress
//					SearchReplaceManager.Replace(monitor);
				}
			}
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
				SearchOptions.DocumentIteratorType = SearchTarget.Directory;
			} else {
				SearchOptions.DocumentIteratorType = (SearchTarget)Get<ComboBox>("lookIn").SelectedIndex;
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
			string[] lookInTexts = {
				// must be in the same order as the DocumentIteratorType enum
				"${res:Dialog.NewProject.SearchReplace.LookIn.CurrentDocument}",
				"${res:Dialog.NewProject.SearchReplace.LookIn.CurrentSelection}",
				"${res:Dialog.NewProject.SearchReplace.LookIn.AllOpenDocuments}",
				"${res:Dialog.NewProject.SearchReplace.LookIn.WholeProject}",
				"${res:Dialog.NewProject.SearchReplace.LookIn.WholeSolution}"
			};
			foreach (string lookInText in lookInTexts) {
				Get<ComboBox>("lookIn").Items.Add(StringParser.Parse(lookInText));
			}
			Get<ComboBox>("lookIn").Items.Add(SearchOptions.LookIn);
			Get<ComboBox>("lookIn").SelectedIndexChanged += new EventHandler(LookInSelectedIndexChanged);
			
//			if (IsMultipleLineSelection(SearchReplaceUtilities.GetActiveTextEditor())) {
//				SearchTarget = SearchTarget.CurrentSelection;
//			} else {
//				if (SearchOptions.DocumentIteratorType == SearchTarget.CurrentSelection) {
//					SearchOptions.DocumentIteratorType = SearchTarget.CurrentDocument;
//				}
//				SearchTarget = SearchOptions.DocumentIteratorType;
//			}
			
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
		
		void FindNextInSelection()
		{
			int startOffset = Math.Min(selection.Offset, selection.EndOffset);
			int endOffset = Math.Max(selection.Offset, selection.EndOffset);
			
			if (findFirst) {
				textEditor.Caret.Offset = startOffset;
			}
			
			try {
				ignoreSelectionChanges = true;
				if (findFirst) {
					findFirst = false;
//					SearchReplaceManager.FindFirstInSelection(startOffset, endOffset - startOffset, null);
				} else {
//					findFirst = !SearchReplaceManager.FindNextInSelection(null);
					if (findFirst) {
						textEditor.Select(startOffset, endOffset - startOffset);
					}
				}
			} finally {
				ignoreSelectionChanges = false;
			}
		}
		
		/// <summary>
		/// Returns the first ISelection object from the currently active text editor
		/// </summary>
		static StoredSelection GetCurrentTextSelection()
		{
//			ITextEditor textArea = SearchReplaceUtilities.GetActiveTextEditor();
//			if (textArea != null) {
//				return new StoredSelection(textArea.SelectionStart, textArea.SelectionLength);
//			}
			return null;
		}
		
		void WorkbenchActiveViewContentChanged(object source, EventArgs e)
		{
//			ITextEditor activeTextEditorControl = SearchReplaceUtilities.GetActiveTextEditor();
//			if (activeTextEditorControl != this.textEditor) {
//				AddSelectionChangedHandler(activeTextEditorControl);
//				TextSelectionChanged(source, e);
//			}
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
			if (!ignoreSelectionChanges) {
				LoggingService.Debug("TextSelectionChanged.");
				selection = GetCurrentTextSelection();
				findFirst = true;
			}
		}
		
		void InitSelectionSearch()
		{
			findFirst = true;
			selection = GetCurrentTextSelection();
//			AddSelectionChangedHandler(SearchReplaceUtilities.GetActiveTextEditor());
			WorkbenchSingleton.Workbench.ActiveViewContentChanged += WorkbenchActiveViewContentChanged;
		}
		
		void RemoveSelectionSearchHandlers()
		{
			RemoveSelectionChangedHandler();
			RemoveActiveWindowChangedHandler();
		}
		
		/// <summary>
		/// action: 0 = find, 1 = mark, 2 = replace
		/// </summary>
		void RunAllInSelection(int action)
		{
			const IProgressMonitor monitor = null;
			
			int startOffset = Math.Min(selection.Offset, selection.EndOffset);
			int endOffset = Math.Max(selection.Offset, selection.EndOffset);
			
			textEditor.Select(startOffset, endOffset - startOffset);
			
//			try {
//				ignoreSelectionChanges = true;
//				if (action == 0) {
//					SearchInFilesManager.FindAll(startOffset, endOffset - startOffset, monitor);
//				} else if (action == 1) {
//					SearchReplaceManager.MarkAll(startOffset, endOffset - startOffset, monitor);
//				} else if (action == 2) {
//					// use anchor for endOffset because the replacement might change the text length
//					var anchor = textEditor.Document.CreateAnchor(endOffset);
//					SearchReplaceManager.ReplaceAll(startOffset, endOffset - startOffset, monitor);
//					endOffset = anchor.Offset;
//				}
//				textEditor.Select(startOffset, endOffset - startOffset);
//			} finally {
//				ignoreSelectionChanges = false;
//			}
		}
		
		void ReplaceInSelection()
		{
			int startOffset = Math.Min(selection.Offset, selection.EndOffset);
			int endOffset = Math.Max(selection.Offset, selection.EndOffset);
			
			if (findFirst) {
				textEditor.Caret.Offset = startOffset;
			}
			
//			try {
//				ignoreSelectionChanges = true;
//				if (findFirst) {
//					findFirst = false;
//					SearchReplaceManager.ReplaceFirstInSelection(startOffset, endOffset - startOffset, null);
//				} else {
//					findFirst = !SearchReplaceManager.ReplaceNextInSelection(null);
//					if (findFirst) {
//						textEditor.Select(startOffset, endOffset - startOffset);
//					}
//				}
//			} finally {
//				ignoreSelectionChanges = false;
//			}
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
