using ICSharpCode.AvalonEdit.Document;
using System;

namespace ICSharpCode.AvalonEdit.CodeCompletion
{
	/// <summary>
	/// The code completion window.
	/// </summary>
	public partial class CompletionWindow : CompletionWindowBase
	{
		TextDocument document;
		int startOffset;
		int endOffset;
		
		/// <summary>
		/// Creates a new code completion window.
		/// </summary>
		public CompletionWindow(TextArea textArea) : base(textArea)
		{
			InitializeComponent();
			
			document = textArea.TextView.Document;
			startOffset = endOffset = textArea.Caret.Offset;
		}
		
		/// <inheritdoc/>
		protected override void AttachEvents()
		{
			base.AttachEvents();
			document.Changing += textArea_Document_Changing;
			this.TextArea.Caret.PositionChanged += CaretPositionChanged;
		}

		/// <inheritdoc/>
		protected override void DetachEvents()
		{
			document.Changing -= textArea_Document_Changing;
			this.TextArea.Caret.PositionChanged -= CaretPositionChanged;
			base.DetachEvents();
		}
		
		void textArea_Document_Changing(object sender, DocumentChangeEventArgs e)
		{
			// => startOffset test required so that this startOffset/endOffset are not incremented again
			//    for BeforeStartKey characters
			if (e.Offset >= startOffset && e.Offset <= endOffset) {
				endOffset += e.InsertionLength - e.RemovalLength;
			} else {
				Close();
			}
		}
		
		/// <summary>
		/// When this flag is set, code completion closes if the caret moves to the
		/// beginning of the allowed range. This is useful in Ctrl+Space and "complete when typing",
		/// but not in dot-completion.
		/// </summary>
		public bool CloseWhenCaretAtBeginning { get; set; }
		
		void CaretPositionChanged(object sender, EventArgs e)
		{
			int offset = this.TextArea.Caret.Offset;
			if (offset == startOffset) {
				if (CloseWhenCaretAtBeginning)
					Close();
				return;
			}
			if (offset < startOffset || offset > endOffset) {
				Close();
			} else {
				//codeCompletionListView.SelectItemWithStart(document.GetText(startOffset, offset - startOffset));
			}
		}
	}
}