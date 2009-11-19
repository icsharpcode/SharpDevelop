// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// IncrementalSearch handler for AvalonEdit.
	/// </summary>
	public sealed class IncrementalSearch : ITextAreaInputHandler
	{
		readonly LogicalDirection direction;
		readonly TextArea textArea;
		
		StringBuilder searchText = new StringBuilder();
		string text;
		int startIndex;
		int originalStartIndex;
		bool passedEndOfDocument;
		
		public IncrementalSearch(TextArea textArea, LogicalDirection direction)
		{
			if (textArea == null)
				throw new ArgumentNullException("textArea");
			this.textArea = textArea;
			this.direction = direction;
		}
		
		#region Attach + Detach
		TextArea ITextAreaInputHandler.TextArea {
			get { return textArea; }
		}
		
		void ITextAreaInputHandler.Attach()
		{
			textArea.PreviewKeyDown += textArea_PreviewKeyDown;
			textArea.TextEntering += textArea_TextEntering;
			textArea.LostFocus += textArea_LostFocus;
			textArea.PreviewMouseDown += textArea_PreviewMouseDown;
			
			EnableIncrementalSearchCursor();
			
			// Get text to search and initial search position.
			text = textArea.Document.Text;
			startIndex = textArea.Caret.Offset;
			originalStartIndex = startIndex;
			
			GetInitialSearchText();
			ShowTextFound(searchText.ToString());
		}

		void ITextAreaInputHandler.Detach()
		{
			textArea.PreviewKeyDown -= textArea_PreviewKeyDown;
			textArea.TextEntering -= textArea_TextEntering;
			textArea.LostFocus -= textArea_LostFocus;
			textArea.PreviewMouseDown -= textArea_PreviewMouseDown;
			
			DisableIncrementalSearchCursor();
			ClearStatusBarMessage();
		}
		
		public void StopIncrementalSearch()
		{
			if (textArea.ActiveInputHandler == this) {
				textArea.ActiveInputHandler = textArea.DefaultInputHandler;
			}
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
			if (!textArea.Selection.IsEmpty) {
				startIndex = textArea.Selection.SurroundingSegment.Offset;
				if (!textArea.Selection.IsMultiline(textArea.Document)) {
					searchText.Append(textArea.Selection.GetText(textArea.Document));
				}
			}
		}
		
		/// <summary>
		/// Changes the text editor's cursor so the user knows we are in
		/// incremental search mode.
		/// </summary>
		void EnableIncrementalSearchCursor()
		{
			string resourceName = "ICSharpCode.AvalonEdit.AddIn.Resources.IncrementalSearchCursor.cur";
			if (direction == LogicalDirection.Backward) {
				resourceName = "ICSharpCode.AvalonEdit.AddIn.Resources.ReverseIncrementalSearchCursor.cur";
			}
			textArea.Cursor = new Cursor(GetType().Assembly.GetManifestResourceStream(resourceName));
		}
		
		void DisableIncrementalSearchCursor()
		{
			textArea.ClearValue(TextArea.CursorProperty);
		}
		#endregion
		
		#region Status bar functions
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
			string incrementalSearchStartMessage;
			if (direction == LogicalDirection.Forward) {
				incrementalSearchStartMessage = StringParser.Parse("${res:ICSharpCode.SharpDevelop.DefaultEditor.IncrementalSearch.ForwardsSearchStatusBarMessage} ");
			} else {
				incrementalSearchStartMessage = StringParser.Parse("${res:ICSharpCode.SharpDevelop.DefaultEditor.IncrementalSearch.ReverseSearchStatusBarMessage} ");
			}
			
			string fullMessage = incrementalSearchStartMessage + message;
			StatusBarService.SetMessage(fullMessage, highlight);
		}
		
		/// <summary>
		/// Shows a status bar message indicating that the specified text
		/// was not found.
		/// </summary>
		void ShowTextNotFound(string find)
		{
			ShowMessage(find + StringParser.Parse(" ${res:ICSharpCode.SharpDevelop.DefaultEditor.IncrementalSearch.NotFoundStatusBarMessage}"), true);
		}
		
		/// <summary>
		/// Clears the status bar message.
		/// </summary>
		void ClearStatusBarMessage()
		{
			StatusBarService.SetMessage(String.Empty);
		}
		#endregion
		
		#region Running the search
		void HighlightText(int offset, int length)
		{
			int endOffset = offset + length;
			textArea.Caret.Offset = endOffset;
			textArea.Selection = new SimpleSelection(offset, endOffset);
		}
		
		/// <summary>
		/// Runs the incremental search either forwards or backwards.
		/// </summary>
		void RunSearch()
		{
			string find = searchText.ToString();
			int index = FindText(find, startIndex);
			if (index == -1) {
				index = FindText(find, GetWrapAroundStartIndex());
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
			if (direction == LogicalDirection.Backward) {
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
		int FindText(string find, int startIndex)
		{
			StringComparison stringComparison = GetStringComparisonType(find);
			if (direction == LogicalDirection.Forward) {
				return text.IndexOf(find, startIndex, stringComparison);
			} else {
				// Reverse search.
				string searchText = GetReverseSearchText(startIndex + find.Length);
				return searchText.LastIndexOf(find, stringComparison);
			}
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
		#endregion
		
		#region Event Handlers
		void textArea_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key) {
				case Key.Enter:
				case Key.Escape:
					e.Handled = true;
					StopIncrementalSearch();
					break;
				case Key.Back:
					// Remove last search char and try search again.
					int length = searchText.Length;
					if (length > 0) {
						searchText.Remove(length - 1, 1);
						// Run search back at original starting point.
						startIndex = originalStartIndex;
						passedEndOfDocument = false;
						RunSearch();
						e.Handled = true;
					} else {
						StopIncrementalSearch();
					}
					break;
				default:
					TextAreaInputHandler[] rootHandlers = { textArea.DefaultInputHandler };
					var allHandlers = rootHandlers.Flatten(h => h.NestedInputHandlers.OfType<TextAreaInputHandler>());
					var keyGestures = allHandlers.SelectMany(h => h.InputBindings).Select(h => h.Gesture).OfType<KeyGesture>();
					foreach (KeyGesture gesture in keyGestures) {
						if (gesture.Key == e.Key) {
							StopIncrementalSearch();
							break;
						}
					}
					break;
			}
		}
		
		void textArea_TextEntering(object sender, TextCompositionEventArgs e)
		{
			searchText.Append(e.Text);
			e.Handled = true;
			RunSearch();
		}
		
		void textArea_LostFocus(object sender, RoutedEventArgs e)
		{
			StopIncrementalSearch();
		}
		
		void textArea_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			StopIncrementalSearch();
		}
		#endregion
	}
}
