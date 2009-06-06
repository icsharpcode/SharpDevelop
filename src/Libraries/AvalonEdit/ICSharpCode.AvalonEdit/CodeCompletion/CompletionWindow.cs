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
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace ICSharpCode.AvalonEdit.CodeCompletion
{
	/// <summary>
	/// The code completion window.
	/// </summary>
	public class CompletionWindow : CompletionWindowBase
	{
		readonly CompletionList completionList = new CompletionList();
		ToolTip toolTip = new ToolTip();
		
		/// <summary>
		/// Creates a new code completion window.
		/// </summary>
		public CompletionWindow(TextArea textArea) : base(textArea)
		{
			// keep height automatic
			this.CloseAutomatically = true;
			this.SizeToContent = SizeToContent.Height;
			this.Width = 175;
			this.Content = completionList;
			// prevent user from resizing window to 0x0
			this.MinHeight = 15;
			this.MinWidth = 30;
			
			toolTip.PlacementTarget = this;
			toolTip.Placement = PlacementMode.Right;
			toolTip.Closed += toolTip_Closed;
			
			completionList.InsertionRequested += completionList_InsertionRequested;
			completionList.SelectionChanged += completionList_SelectionChanged;
			AttachEvents();
		}
		
		#region ToolTip handling
		void toolTip_Closed(object sender, RoutedEventArgs e)
		{
			// Clear content after tooltip is closed.
			// We cannot clear is immediately when setting IsOpen=false
			// because the tooltip uses an animation for closing.
			if (toolTip != null)
				toolTip.Content = null;
		}
		
		void completionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var item = completionList.SelectedItem;
			if (item == null)
				return;
			object description = item.Description;
			if (description != null) {
				toolTip.Content = description;
				toolTip.IsOpen = true;
			} else {
				toolTip.IsOpen = false;
			}
		}
		#endregion
		
		void completionList_InsertionRequested(object sender, EventArgs e)
		{
			var item = completionList.SelectedItem;
			if (item != null)
				item.Complete(this.TextArea, new AnchorSegment(this.TextArea.Document, this.StartOffset, this.EndOffset - this.StartOffset), e);
			Close();
		}
		
		/// <inheritdoc/>
		protected override void OnSourceInitialized(EventArgs e)
		{
			// prevent CompletionWindow from growing too large
			if (this.ActualHeight > 300) {
				if (this.SizeToContent == SizeToContent.Height)
					this.SizeToContent = SizeToContent.Manual;
				else if (this.SizeToContent == SizeToContent.WidthAndHeight)
					this.SizeToContent = SizeToContent.Width;
				this.Height = 300;
				
			}
			
			base.OnSourceInitialized(e);
		}
		
		void AttachEvents()
		{
			this.TextArea.Caret.PositionChanged += CaretPositionChanged;
			this.TextArea.MouseWheel += textArea_MouseWheel;
			this.TextArea.PreviewTextInput += textArea_PreviewTextInput;
			this.TextArea.ActiveInputHandler = new InputHandler(this);
		}
		
		/// <inheritdoc/>
		protected override void DetachEvents()
		{
			this.TextArea.Caret.PositionChanged -= CaretPositionChanged;
			this.TextArea.MouseWheel -= textArea_MouseWheel;
			this.TextArea.PreviewTextInput -= textArea_PreviewTextInput;
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
				// close with dispatcher so we don't get reentrance problems in input handler Detach/Attach
				window.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(window.Close));
			}
		}
		#endregion
		
		/// <inheritdoc/>
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			if (toolTip != null) {
				toolTip.IsOpen = false;
				toolTip = null;
			}
		}
		
		/// <inheritdoc/>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled) {
				completionList.HandleKey(e);
			}
		}
		
		void textArea_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = RaiseEventPair(this, PreviewTextInputEvent, TextInputEvent,
			                           new TextCompositionEventArgs(e.Device, e.TextComposition));
		}
		
		void textArea_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			e.Handled = RaiseEventPair(GetScrollEventTarget(),
			                           PreviewMouseWheelEvent, MouseWheelEvent,
			                           new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta));
		}
		
		UIElement GetScrollEventTarget()
		{
			if (completionList == null)
				return this;
			return completionList.ScrollViewer ?? completionList.ListBox ?? (UIElement)completionList;
		}
		
		/// <summary>
		/// Gets/Sets whether the completion window should close automatically.
		/// The default value is true.
		/// </summary>
		public bool CloseAutomatically { get; set; }
		
		/// <inheritdoc/>
		protected override bool CloseOnFocusLost {
			get { return this.CloseAutomatically; }
		}
		
		/// <summary>
		/// When this flag is set, code completion closes if the caret moves to the
		/// beginning of the allowed range. This is useful in Ctrl+Space and "complete when typing",
		/// but not in dot-completion.
		/// Has no effect if CloseAutomatically is false.
		/// </summary>
		public bool CloseWhenCaretAtBeginning { get; set; }
		
		void CaretPositionChanged(object sender, EventArgs e)
		{
			int offset = this.TextArea.Caret.Offset;
			if (offset == this.StartOffset) {
				if (CloseAutomatically && CloseWhenCaretAtBeginning)
					Close();
				return;
			}
			if (offset < this.StartOffset || offset > this.EndOffset) {
				if (CloseAutomatically) {
					Close();
				}
			} else {
				TextDocument document = this.TextArea.Document;
				if (document != null) {
					completionList.SelectItemWithStart(document.GetText(this.StartOffset, offset - this.StartOffset));
				}
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