// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class IncrementalSearch : IDisposable
	{		
		bool disposed;
		TextEditorControl textEditor;
		IFormattingStrategy previousFormattingStrategy;
		string incrementalSearchStartMessage;
		
		StringBuilder searchText = new StringBuilder();
		string text;
		int startIndex;
		int originalStartIndex;
		Cursor cursor;
		bool passedEndOfDocument;
		
		bool codeCompletionEnabled;
		
		// Indicates whether this is a forward search or a reverse search.
		bool forwards = true;
		
		/// <summary>
		/// Dummy formatting strategy that stops the FormatLine method
		/// from automatically adding things like xml comment tags.
		/// </summary>
		class IncrementalSearchFormattingStrategy : DefaultFormattingStrategy
		{
			public override void FormatLine(TextArea textArea, int line, int cursorOffset, char ch)
			{
			}
		}
		
		/// <summary>
		/// Creates a incremental search that goes forwards.
		/// </summary>
		public IncrementalSearch(TextEditorControl textEditor)
			: this(textEditor, true)
		{
		}
		
		/// <summary>
		/// Creates an incremental search for the specified text editor.
		/// </summary>
		/// <param name="textEditor">The text editor to search in.</param>
		/// <param name="forwards">Indicates whether the search goes
		/// forward from the cursor or backwards.</param>
		public IncrementalSearch(TextEditorControl textEditor, bool forwards)
		{
			
			this.forwards = forwards;
			if (forwards) {
				incrementalSearchStartMessage = StringParser.Parse("${res:ICSharpCode.SharpDevelop.DefaultEditor.IncrementalSearch.ForwardsSearchStatusBarMessage} ");
			} else {
				incrementalSearchStartMessage = StringParser.Parse("${res:ICSharpCode.SharpDevelop.DefaultEditor.IncrementalSearch.ReverseSearchStatusBarMessage} ");
			}
			this.textEditor = textEditor;
			
			// Disable code completion.
			codeCompletionEnabled = CodeCompletionOptions.EnableCodeCompletion;
			CodeCompletionOptions.EnableCodeCompletion = false;
			
			AddFormattingStrategy();
			
			TextArea.KeyEventHandler += TextAreaKeyPress;
			TextArea.DoProcessDialogKey += TextAreaProcessDialogKey;
			TextArea.LostFocus += TextAreaLostFocus;
			TextArea.MouseClick += TextAreaMouseClick;
			
			EnableIncrementalSearchCursor();
			
			// Get text to search and initial search position.
			text = textEditor.Document.TextContent;
			startIndex = TextArea.Caret.Offset;
			originalStartIndex = startIndex;
		
			GetInitialSearchText();
			ShowTextFound(searchText.ToString());
		}
		
		public void Dispose()
		{
			if (!disposed) {
				disposed = true;
				TextArea.KeyEventHandler -= TextAreaKeyPress;
				TextArea.DoProcessDialogKey -= TextAreaProcessDialogKey;
				TextArea.LostFocus -= TextAreaLostFocus;
				TextArea.MouseClick -= TextAreaMouseClick;
				DisableIncrementalSearchCursor();
				RemoveFormattingStrategy();
				if (cursor != null) {
					cursor.Dispose();
				}
				ClearStatusBarMessage();
			}
		}
		
		TextArea TextArea {
			get {
				return textEditor.ActiveTextAreaControl.TextArea;
			}
		}
	
		void TextAreaLostFocus(object source, EventArgs e)
		{
			StopIncrementalSearch();
		}
		
		/// <summary>
		/// Stop the incremental search if the user clicks.
		/// </summary>
		void TextAreaMouseClick(object source, MouseEventArgs e)
		{
			StopIncrementalSearch();
		}
		
		void StopIncrementalSearch()
		{
			// Reset code completion state.
			CodeCompletionOptions.EnableCodeCompletion = codeCompletionEnabled;
			Dispose();
		}
		
		/// <summary>
		/// Searches the text incrementally on each key press.
		/// </summary>
		bool TextAreaKeyPress(char ch)
		{			
			// Search for text.
			searchText.Append(ch);
			RunSearch();
			return true;
		}
		
		void HighlightText(int offset, int length)
		{
			int endOffset = offset + length;
			TextArea.Caret.Position = TextArea.Document.OffsetToPosition(endOffset);
			TextArea.SelectionManager.ClearSelection();
			IDocument document = TextArea.Document;
			DefaultSelection selection = new DefaultSelection(document, document.OffsetToPosition(offset), document.OffsetToPosition(endOffset));
			TextArea.SelectionManager.SetSelection(selection);
			textEditor.Refresh();
		}
		
		/// <summary>
		/// Runs the incremental search either forwards or backwards.
		/// </summary>
		void RunSearch()
		{
			string find = searchText.ToString();
			int index = FindText(find, startIndex, forwards);
			if (index == -1) {
				index = FindText(find, GetWrapAroundStartIndex(), forwards);
				passedEndOfDocument = true;
			}
			
			// Highlight found text and show status bar message.
			if (index >= 0) {
				startIndex = index;
				HighlightText(index, find.Length);
				ShowTextFound(find);
			} else {
				ShowTextNotFound(find);				
			}
		}
		
		/// <summary>
		/// Gets the index the search should start from when we wrap around. This
		/// is either the start of the string or the very end depending on which
		/// way we are searching.
		/// </summary>
		int GetWrapAroundStartIndex()
		{
			int wrapAroundIndex = 0;
			if (!forwards) {
				wrapAroundIndex = text.Length - 1;
			}
			return wrapAroundIndex;
		}
		
		/// <summary>
		/// Looks for the string from the last position we searched from. The
		/// search is case insensitive if all the characters of the search
		/// string are lower case. If one of the search characters is upper case
		/// then the search is case sensitive. The search can be either forwards
		/// or backwards.
		/// </summary>
		int FindText(string find, int startIndex, bool forwards)
		{
			StringComparison stringComparison = GetStringComparisonType(find);
			if (forwards) {
				return text.IndexOf(find, startIndex, stringComparison);
			}
			// Reverse search. 
			string searchText = GetReverseSearchText(startIndex + find.Length);
			return searchText.LastIndexOf(find, stringComparison);
		}
		
		/// <summary>
		/// Gets whether the search string comparision should be case
		/// sensitive. If all the characters of the find string are lower case
		/// then the search is case insensitive. If any character is upper case
		/// then the search is case sensitive.
		/// </summary>
		StringComparison GetStringComparisonType(string find)
		{
			foreach (char c in find) {
				if (Char.IsUpper(c)) {
					return StringComparison.InvariantCulture;
				}
			}
			return StringComparison.InvariantCultureIgnoreCase;
		}
		
		/// <summary>
		/// Gets the text to search when doing a reverse search.
		/// </summary>
		string GetReverseSearchText(int endIndex)
		{
			if (endIndex < text.Length) {
				return text.Substring(0, endIndex);
			}
			endIndex = text.Length - 1;
			if (endIndex >= 0) {
				return text;
			}
			return String.Empty;
		}
		
		/// <summary>
		/// Checks the dialog key to see if the incremental search
		/// should be cancelled.
		/// </summary>
		/// <remarks>
		/// If the user presses the escape or enter key then the 
		/// incremental search is aborted. If the user executes any
		/// edit action via the keyboard the incremental search is aborted
		/// and the edit action is allowed to execute.
		/// </remarks>
		bool TextAreaProcessDialogKey(Keys keys)
		{
			if (keys == Keys.Escape || 
			    keys == Keys.Enter) {
				StopIncrementalSearch();
				return true;
			} else if (keys == Keys.Back) {
				// Remove last search char and try search again.
				int length = searchText.ToString().Length;
				if (length > 0) {
					searchText.Remove(length - 1, 1);
					// Run search back at original starting point.
					startIndex = originalStartIndex;
					passedEndOfDocument = false;
					RunSearch();
					return true;
				} else {
					StopIncrementalSearch();
					return false;
				}
			} else if (textEditor.IsEditAction(keys)) {
				StopIncrementalSearch();
				return false;
			}
			return false;
		}
		
		static bool IsGreaterThanKey(Keys keys)
		{
			return (int)(keys & Keys.KeyCode) == '>';
		}
		
		/// <summary>
		/// Shows the status bar message. All messages are prefixed
		/// with the standard Incremental Search string.
		/// </summary>
		void ShowTextFound(string find)
		{
			if (passedEndOfDocument) {
				ShowMessage(String.Concat(find, StringParser.Parse(" ${res:ICSharpCode.SharpDevelop.DefaultEditor.IncrementalSearch.PassedEndOfDocumentStatusBarMessage}")), true);
			} else {
				ShowMessage(find, false);
			}
		}
		
		/// <summary>
		/// Shows a highlighed status bar message.
		/// </summary>
		void ShowMessage(string message, bool highlight)
		{
			string fullMessage = String.Concat(incrementalSearchStartMessage, message);
			StatusBarService.SetMessage(fullMessage, highlight);
		}
		
		/// <summary>
		/// Shows a status bar message indicating that the specified text 
		/// was not found.
		/// </summary>
		void ShowTextNotFound(string find)
		{
			ShowMessage(String.Concat(find, StringParser.Parse(" ${res:ICSharpCode.SharpDevelop.DefaultEditor.IncrementalSearch.NotFoundStatusBarMessage}")), true);
		}
	
		/// <summary>
		/// Clears the status bar message.
		/// </summary>
		void ClearStatusBarMessage()
		{
			StatusBarService.SetMessage(String.Empty);
		}
		
		/// <summary>
		/// Gets the initial text to include in the incremental search.
		/// </summary>
		/// <remarks>
		/// If multiple lines are selected then no initial search text
		/// is set.
		/// </remarks>
		void GetInitialSearchText()
		{
			if (TextArea.SelectionManager.HasSomethingSelected) {
				ISelection selection = TextArea.SelectionManager.SelectionCollection[0];
				startIndex = selection.Offset;
				if (!IsMultilineSelection(selection)) {
					searchText.Append(selection.SelectedText);
				}
			}
		}
		
		bool IsMultilineSelection(ISelection selection)
		{
			return selection.StartPosition.Y != selection.EndPosition.Y;
		}
		
		/// <summary>
		/// Gets the cursor to be displayed in the text editor whilst doing
		/// an incremental search.
		/// </summary>
		Cursor GetCursor()
		{
			if (cursor == null) {
				string resourceName = "Resources.IncrementalSearchCursor.cur";
				if (!forwards) {
					resourceName = "Resources.ReverseIncrementalSearchCursor.cur";
				}
				cursor = new Cursor(GetType().Assembly.GetManifestResourceStream(resourceName));
			}
			return cursor;
		}
		
		/// <summary>
		/// Changes the text editor's cursor so the user knows we are in
		/// incremental search mode.
		/// </summary>
		void EnableIncrementalSearchCursor()
		{
			Cursor cursor = GetCursor();
			TextArea.Cursor = cursor;
			TextArea.TextView.Cursor = cursor;
		}
		
		void DisableIncrementalSearchCursor()
		{
			TextArea.Cursor = Cursors.IBeam;
			TextArea.TextView.Cursor = Cursors.IBeam;
		}
		
		/// <summary>
		/// Replace the existing formatting strategy with our dummy one.
		/// </summary>
		/// <remarks>
		/// Special case. We need to replace the formatting strategy to 
		/// prevent the text editor from autocompletiong xml elements and
		/// xml comment tags. The text editor always calls 
		/// IFormattingStrategy.FormatLine regardless of whether any text was 
		/// actually inserted or replaced. 
		/// </remarks>
		void AddFormattingStrategy()
		{
			IDocument document = textEditor.Document;
			previousFormattingStrategy = document.FormattingStrategy;
			textEditor.Document.FormattingStrategy = new IncrementalSearchFormattingStrategy();
		}
		
		/// <summary>
		/// Removes our dummy formatting strategy and replaces it with
		/// the original before the incremental search was triggered.
		/// </summary>
		void RemoveFormattingStrategy()
		{
			textEditor.Document.FormattingStrategy = previousFormattingStrategy;
		}
	}
}
