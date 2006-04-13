// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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
	public class SearchAndReplacePanel : BaseSharpDevelopUserControl
	{
		SearchAndReplaceMode  searchAndReplaceMode;
		ISelection selection;
		TextEditorControl textEditor;
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
						SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.FindPanel.xfrm"));
						Get<Button>("findAll").Click += FindAllButtonClicked;
						Get<Button>("bookmarkAll").Click += BookmarkAllButtonClicked;
						break;
					case SearchAndReplaceMode.Replace:
						SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.ReplacePanel.xfrm"));
						Get<Button>("replace").Click += ReplaceButtonClicked;
						Get<Button>("replaceAll").Click += ReplaceAllButtonClicked;
						break;
				}
				
				ControlDictionary["findNextButton"].Click     += FindNextButtonClicked;
				ControlDictionary["lookInBrowseButton"].Click += LookInBrowseButtonClicked;
				((Form)Parent).AcceptButton = (Button)ControlDictionary["findNextButton"];
				SetOptions();
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
		
		public DocumentIteratorType DocumentIteratorType {
			get {
				return (DocumentIteratorType)(Get<ComboBox>("lookIn").SelectedIndex);
			}
			set {
				Get<ComboBox>("lookIn").SelectedIndex = (int)value;
			}
		}
		
		void LookInBrowseButtonClicked(object sender, EventArgs e)
		{
			FolderDialog dlg = new FolderDialog();
			if (dlg.DisplayDialog("${res:Dialog.NewProject.SearchReplace.LookIn.SelectDirectory}") == DialogResult.OK) {
				Get<ComboBox>("lookIn").SelectedIndex = customDirectoryIndex;
				Get<ComboBox>("lookIn").Text = dlg.Path;
			}
		}
		
		void FindNextButtonClicked(object sender, EventArgs e)
		{
			WritebackOptions();
			if (IsSelectionSearch) {
				if (IsTextSelected(selection)) {
					FindNextInSelection();
				}
			} else {
				SearchReplaceManager.FindNext();
			}
			Focus();
		}
		
		void FindAllButtonClicked(object sender, EventArgs e)
		{
			WritebackOptions();
			if (IsSelectionSearch) {
				if (IsTextSelected(selection)) {
					RunAllInSelection(0);
				}
			} else {
				SearchInFilesManager.FindAll();
			}
		}
		
		void BookmarkAllButtonClicked(object sender, EventArgs e)
		{
			WritebackOptions();
			if (IsSelectionSearch) {
				if (IsTextSelected(selection)) {
					RunAllInSelection(1);
				}
			} else {
				SearchReplaceManager.MarkAll();
			}
		}
		
		void ReplaceAllButtonClicked(object sender, EventArgs e)
		{
			WritebackOptions();
			if (IsSelectionSearch) {
				if (IsTextSelected(selection)) {
					RunAllInSelection(2);
				}
			} else {
				SearchReplaceManager.ReplaceAll();
			}
		}
		
		void ReplaceButtonClicked(object sender, EventArgs e)
		{
			WritebackOptions();
			if (IsSelectionSearch) {
				if (IsTextSelected(selection)) {
					ReplaceInSelection();
				}
			} else {
				SearchReplaceManager.Replace();
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
			
			SearchOptions.SearchStrategyType = (SearchStrategyType)Get<ComboBox>("use").SelectedIndex;
			if (Get<ComboBox>("lookIn").DropDownStyle == ComboBoxStyle.DropDown) {
				SearchOptions.DocumentIteratorType = DocumentIteratorType.Directory;
			} else {
				SearchOptions.DocumentIteratorType = (DocumentIteratorType)Get<ComboBox>("lookIn").SelectedIndex;
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
			
			if (IsMultipleLineSelection(GetCurrentTextSelection())) {
				DocumentIteratorType = DocumentIteratorType.CurrentSelection;
			} else {
				DocumentIteratorType = SearchOptions.DocumentIteratorType;
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
				return DocumentIteratorType == DocumentIteratorType.CurrentSelection;
			}
		}
		
		/// <summary>
		/// Checks whether the selection spans two or more lines.
		/// </summary>
		/// <remarks>Maybe the ISelection interface should have an
		/// IsMultipleLine method?</remarks>
		static bool IsMultipleLineSelection(ISelection selection)
		{
			if (IsTextSelected(selection)) {
				return selection.SelectedText.IndexOf('\n') != -1;
			}
			return false;
		}
		
		static bool IsTextSelected(ISelection selection)
		{
			if (selection != null) {
				return !selection.IsEmpty;
			}
			return false;
		}
		
		void FindNextInSelection()
		{
			int startOffset = Math.Min(selection.Offset, selection.EndOffset);
			int endOffset = Math.Max(selection.Offset, selection.EndOffset);
			
			if (findFirst) {
				SetCaretPosition(textEditor.ActiveTextAreaControl.TextArea, startOffset);
			}
			
			try {
				ignoreSelectionChanges = true;
				if (findFirst) {
					findFirst = false;
					SearchReplaceManager.FindFirstInSelection(startOffset, endOffset - startOffset);
				} else {
					findFirst = !SearchReplaceManager.FindNextInSelection();
					if (findFirst) {
						SearchReplaceUtilities.SelectText(textEditor, startOffset, endOffset);
					}
				}
			} finally {
				ignoreSelectionChanges = false;
			}
		}
		
		/// <summary>
		/// Returns the first ISelection object from the currently active text editor
		/// </summary>
		static ISelection GetCurrentTextSelection()
		{
			TextEditorControl textArea = SearchReplaceUtilities.GetActiveTextEditor();
			if (textArea != null) {
				SelectionManager selectionManager = textArea.ActiveTextAreaControl.SelectionManager;
				if (selectionManager.HasSomethingSelected) {
					return selectionManager.SelectionCollection[0];
				}
			}
			return null;
		}
		
		void WorkbenchWindowChanged(object source, EventArgs e)
		{
			TextEditorControl activeTextEditorControl = SearchReplaceUtilities.GetActiveTextEditor();
			if (activeTextEditorControl != this.textEditor) {
				AddSelectionChangedHandler(activeTextEditorControl);
				TextSelectionChanged(source, e);
			}
		}
		
		void AddSelectionChangedHandler(TextEditorControl textEditor)
		{
			RemoveSelectionChangedHandler();
			
			this.textEditor = textEditor;
			if (textEditor != null) {
				this.textEditor.ActiveTextAreaControl.SelectionManager.SelectionChanged += TextSelectionChanged;
			}
		}
		
		void RemoveSelectionChangedHandler()
		{
			if (textEditor != null) {
				textEditor.ActiveTextAreaControl.SelectionManager.SelectionChanged -= TextSelectionChanged;
			}
		}
		
		void RemoveActiveWindowChangedHandler()
		{
			WorkbenchSingleton.Workbench.ActiveWorkbenchWindowChanged -= WorkbenchWindowChanged;
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
		
		void SetCaretPosition(TextArea textArea, int offset)
		{
			textArea.Caret.Position = textArea.Document.OffsetToPosition(offset);
		}
		
		void InitSelectionSearch()
		{
			findFirst = true;
			selection = GetCurrentTextSelection();
			AddSelectionChangedHandler(SearchReplaceUtilities.GetActiveTextEditor());
			WorkbenchSingleton.Workbench.ActiveWorkbenchWindowChanged += WorkbenchWindowChanged;
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
			int startOffset = Math.Min(selection.Offset, selection.EndOffset);
			int endOffset = Math.Max(selection.Offset, selection.EndOffset);
			
			SearchReplaceUtilities.SelectText(textEditor, startOffset, endOffset);
			SetCaretPosition(textEditor.ActiveTextAreaControl.TextArea, startOffset);
			
			try {
				ignoreSelectionChanges = true;
				if (action == 0)
					SearchInFilesManager.FindAll(startOffset, endOffset - startOffset);
				else if (action == 1)
					SearchReplaceManager.MarkAll(startOffset, endOffset - startOffset);
				else if (action == 2)
					SearchReplaceManager.ReplaceAll(startOffset, endOffset - startOffset);
				SearchReplaceUtilities.SelectText(textEditor, startOffset, endOffset);
			} finally {
				ignoreSelectionChanges = false;
			}
		}
		
		void ReplaceInSelection()
		{
			int startOffset = Math.Min(selection.Offset, selection.EndOffset);
			int endOffset = Math.Max(selection.Offset, selection.EndOffset);
			
			if (findFirst) {
				SetCaretPosition(textEditor.ActiveTextAreaControl.TextArea, startOffset);
			}
			
			try {
				ignoreSelectionChanges = true;
				if (findFirst) {
					findFirst = false;
					SearchReplaceManager.ReplaceFirstInSelection(startOffset, endOffset - startOffset);
				} else {
					findFirst = !SearchReplaceManager.ReplaceNextInSelection();
					if (findFirst) {
						SearchReplaceUtilities.SelectText(textEditor, startOffset, endOffset);
					}
				}
			} finally {
				ignoreSelectionChanges = false;
			}
		}
	}
}
