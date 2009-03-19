// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.CodeCompletion
{
	/// <summary>
	/// The code completion window.
	/// </summary>
	public class CompletionWindow : CompletionWindowBase
	{
		TextDocument document;
		int startOffset;
		int endOffset;
		readonly CompletionList completionList = new CompletionList();
		
		/// <summary>
		/// Creates a new code completion window.
		/// </summary>
		public CompletionWindow(TextArea textArea) : base(textArea)
		{
			// keep height automatic
			this.SizeToContent = SizeToContent.Height;
			this.Width = 200;
			this.Content = completionList;
			
			completionList.SizeChanged += completionList_SizeChanged;
			
			document = textArea.TextView.Document;
			startOffset = endOffset = textArea.Caret.Offset;
		}
		
		void completionList_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (completionList.ActualHeight < 200) {
				this.SizeToContent = SizeToContent.Height;
				this.Height = double.NaN;
			} else if (completionList.ActualHeight > 300) {
				this.SizeToContent = SizeToContent.Manual;
				this.Height = 300;
			}
		}
		
		/// <inheritdoc/>
		protected override void AttachEvents()
		{
			base.AttachEvents();
			document.Changing += textArea_Document_Changing;
			this.TextArea.Caret.PositionChanged += CaretPositionChanged;
			this.TextArea.MouseWheel += textArea_MouseWheel;
		}
		
		/// <inheritdoc/>
		protected override void DetachEvents()
		{
			document.Changing -= textArea_Document_Changing;
			this.TextArea.Caret.PositionChanged -= CaretPositionChanged;
			this.TextArea.MouseWheel -= textArea_MouseWheel;
			base.DetachEvents();
		}
		
		/// <inheritdoc/>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled) {
				completionList.HandleKey(e);
			}
		}
		
		void textArea_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			e.Handled = ForwardEvent(PreviewMouseWheelEvent, MouseWheelEvent,
			                         new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta));
		}
		
		bool ForwardEvent(RoutedEvent previewEvent, RoutedEvent @event, RoutedEventArgs args)
		{
			UIElement target = GetEventTarget();
			
			args.RoutedEvent = previewEvent;
			target.RaiseEvent(args);
			args.RoutedEvent = @event;
			target.RaiseEvent(args);
			return args.Handled;
		}
		
		UIElement GetEventTarget()
		{
			if (completionList == null)
				return this;
			return completionList.ScrollViewer ?? completionList.ListBox ?? (UIElement)completionList;
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
		
		/// <summary>
		/// Gets the completion list used in this completion window.
		/// </summary>
		public CompletionList CompletionList {
			get { return completionList; }
		}
	}
}