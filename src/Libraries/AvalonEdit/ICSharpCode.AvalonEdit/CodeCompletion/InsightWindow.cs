// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;
using System.Windows.Controls;

namespace ICSharpCode.AvalonEdit.CodeCompletion
{
	/// <summary>
	/// A popup-like window.
	/// </summary>
	public class InsightWindow : CompletionWindowBase
	{
		TextDocument document;
		int startOffset;
		int endOffset;
		
		/// <summary>
		/// Creates a new InsightWindow.
		/// </summary>
		public InsightWindow(TextArea textArea) : base(textArea)
		{
			this.SizeToContent = SizeToContent.WidthAndHeight;
			// prevent user from resizing window to 0x0
			this.MinHeight = 15;
			this.MinWidth = 30;
			
			startOffset = endOffset = this.TextArea.Caret.Offset;
		}
		
		/// <summary>
		/// Gets/Sets whether the insight window should close automatically.
		/// The default value is true.
		/// </summary>
		public bool CloseAutomatically { get; set; }
		
		/// <inheritdoc/>
		protected override bool CloseOnFocusLost {
			get { return this.CloseAutomatically; }
		}
		
		/// <summary>
		/// Gets/Sets the start of the text range in which the insight window stays open.
		/// Has no effect if CloseAutomatically is false.
		/// </summary>
		public int StartOffset { get; set; }
		
		/// <summary>
		/// Gets/Sets the end of the text range in which the insight window stays open.
		/// Has no effect if CloseAutomatically is false.
		/// </summary>
		public int EndOffset { get; set; }
		
		/// <inheritdoc/>
		protected override void AttachEvents()
		{
			base.AttachEvents();
			document = this.TextArea.Document;
			if (document != null) {
				document.Changing += textArea_Document_Changing;
			}
			this.TextArea.Caret.PositionChanged += CaretPositionChanged;
		}
		
		/// <inheritdoc/>
		protected override void DetachEvents()
		{
			if (document != null) {
				document.Changing -= textArea_Document_Changing;
			}
			this.TextArea.Caret.PositionChanged -= CaretPositionChanged;
			base.DetachEvents();
		}
		
		void textArea_Document_Changing(object sender, DocumentChangeEventArgs e)
		{
			startOffset = e.GetNewOffset(startOffset, AnchorMovementType.BeforeInsertion);
			endOffset = e.GetNewOffset(endOffset, AnchorMovementType.AfterInsertion);
		}
		
		void CaretPositionChanged(object sender, EventArgs e)
		{
			int offset = this.TextArea.Caret.Offset;
			if (offset < startOffset || offset > endOffset) {
				Close();
			}
		}
	}
}
