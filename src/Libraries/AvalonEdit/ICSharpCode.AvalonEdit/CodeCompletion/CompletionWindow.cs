// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Gui;

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
			
			startOffset = endOffset = this.TextArea.Caret.Offset;
			document = textArea.TextView.Document;
		}
		
		/// <summary>
		/// Gets/Sets the start offset of the edited text portion.
		/// </summary>
		public int StartOffset {
			get { return startOffset; }
			set { startOffset = value; }
		}
		
		/// <summary>
		/// Gets/Sets the end offset of the edited text portion.
		/// </summary>
		public int EndOffset {
			get { return endOffset; }
			set { endOffset = value; }
		}
		
		/// <inheritdoc/>
		protected override void OnSourceInitialized(EventArgs e)
		{
			// prevent CompletionWindow from growing too large
			if (this.ActualHeight > 300) {
				this.SizeToContent = SizeToContent.Manual;
				this.Height = 300;
			}
			
			base.OnSourceInitialized(e);
		}
		
		/// <inheritdoc/>
		protected override void AttachEvents()
		{
			base.AttachEvents();
			document.Changing += textArea_Document_Changing;
			this.TextArea.Caret.PositionChanged += CaretPositionChanged;
			this.TextArea.MouseWheel += textArea_MouseWheel;
			this.TextArea.ActiveInputHandler = new InputHandler(this);
		}
		
		/// <inheritdoc/>
		protected override void DetachEvents()
		{
			document.Changing -= textArea_Document_Changing;
			this.TextArea.Caret.PositionChanged -= CaretPositionChanged;
			this.TextArea.MouseWheel -= textArea_MouseWheel;
			base.DetachEvents();
			this.TextArea.ActiveInputHandler = this.TextArea.DefaultInputHandler;
		}
		
		#region InputHandler
		/// <summary>
		/// A dummy input handler (that justs invokes the default input handler).
		/// This is used to ensure the completion window closes when any other input handler
		/// becomes active.
		/// </summary>
		sealed class InputHandler : ITextAreaInputHandler
		{
			readonly CompletionWindow window;
			
			public InputHandler(CompletionWindow window)
			{
				Debug.Assert(window != null);
				this.window = window;
			}
			
			public TextArea TextArea {
				get { return window.TextArea; }
			}
			
			public void Attach()
			{
				this.TextArea.DefaultInputHandler.Attach();
			}
			
			public void Detach()
			{
				this.TextArea.DefaultInputHandler.Detach();
				window.Close();
			}
		}
		#endregion
		
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
		
		bool expectInsertionBeforeStart = true;
		
		void textArea_Document_Changing(object sender, DocumentChangeEventArgs e)
		{
			// => startOffset test required so that this startOffset/endOffset are not incremented again
			//    for BeforeStartKey characters
			if (e.Offset >= startOffset && e.Offset <= endOffset) {
				endOffset += e.InsertionLength - e.RemovalLength;
			} else if (expectInsertionBeforeStart && e.Offset == startOffset - 1 && e.InsertionLength == 1 && e.RemovalLength == 0) {
				// allow a single one-character insertion in front of StartOffset.
				// this is necessary because for dot-completion, the CompletionWindow is shown before
				// the dot is actually inserted.
				expectInsertionBeforeStart = false;
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
				completionList.SelectItemWithStart(document.GetText(startOffset, offset - startOffset));
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