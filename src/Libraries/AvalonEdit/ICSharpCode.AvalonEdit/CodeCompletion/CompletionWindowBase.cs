// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.CodeCompletion
{
	/// <summary>
	/// Base class for completion windows. Handles positioning the window at the caret.
	/// </summary>
	public class CompletionWindowBase : Window
	{
		static CompletionWindowBase()
		{
			WindowStyleProperty.OverrideMetadata(typeof(CompletionWindowBase), new FrameworkPropertyMetadata(WindowStyle.None));
			ShowActivatedProperty.OverrideMetadata(typeof(CompletionWindowBase), new FrameworkPropertyMetadata(Boxes.False));
			ShowInTaskbarProperty.OverrideMetadata(typeof(CompletionWindowBase), new FrameworkPropertyMetadata(Boxes.False));
		}
		
		/// <summary>
		/// Gets the parent TextArea.
		/// </summary>
		public TextArea TextArea { get; private set; }
		
		Window parentWindow;
		TextDocument document;
		int startOffset;
		int endOffset;
		
		/// <summary>
		/// Creates a new CompletionWindowBase.
		/// </summary>
		public CompletionWindowBase(TextArea textArea)
		{
			if (textArea == null)
				throw new ArgumentNullException("textArea");
			this.TextArea = textArea;
			parentWindow = Window.GetWindow(textArea);
			this.Owner = parentWindow;
			this.AddHandler(MouseUpEvent, new MouseButtonEventHandler(OnMouseUp), true);
			
			startOffset = endOffset = this.TextArea.Caret.Offset;
			
			AttachEvents();
		}
		
		#region Event Handlers
		void AttachEvents()
		{
			document = this.TextArea.Document;
			if (document != null) {
				document.Changing += textArea_Document_Changing;
			}
			this.TextArea.PreviewLostKeyboardFocus += TextAreaLostFocus;
			this.TextArea.TextView.ScrollOffsetChanged += TextViewScrollOffsetChanged;
			this.TextArea.DocumentChanged += TextAreaDocumentChanged;
			this.TextArea.PreviewKeyDown += TextAreaPreviewKeyDown;
			this.TextArea.PreviewKeyUp += TextAreaPreviewKeyUp;
			if (parentWindow != null) {
				parentWindow.LocationChanged += parentWindow_LocationChanged;
			}
		}
		
		/// <summary>
		/// Detaches events from the text area.
		/// </summary>
		protected virtual void DetachEvents()
		{
			if (document != null) {
				document.Changing -= textArea_Document_Changing;
			}
			this.TextArea.PreviewLostKeyboardFocus -= TextAreaLostFocus;
			this.TextArea.TextView.ScrollOffsetChanged -= TextViewScrollOffsetChanged;
			this.TextArea.DocumentChanged -= TextAreaDocumentChanged;
			this.TextArea.PreviewKeyDown -= TextAreaPreviewKeyDown;
			this.TextArea.PreviewKeyUp -= TextAreaPreviewKeyUp;
			if (parentWindow != null) {
				parentWindow.LocationChanged -= parentWindow_LocationChanged;
			}
		}
		
		void TextAreaPreviewKeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = RaiseEventPair(this, PreviewKeyDownEvent, KeyDownEvent,
			                           new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key));
		}
		
		void TextAreaPreviewKeyUp(object sender, KeyEventArgs e)
		{
			e.Handled = RaiseEventPair(this, PreviewKeyUpEvent, KeyUpEvent,
			                           new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key));
		}
		
		void TextViewScrollOffsetChanged(object sender, EventArgs e)
		{
			UpdatePosition();
		}
		
		void TextAreaDocumentChanged(object sender, EventArgs e)
		{
			Close();
		}
		
		void TextAreaLostFocus(object sender, RoutedEventArgs e)
		{
			Dispatcher.BeginInvoke(new Action(CloseIfFocusLost), DispatcherPriority.Background);
		}
		
		void parentWindow_LocationChanged(object sender, EventArgs e)
		{
			UpdatePosition();
		}
		
		/// <inheritdoc/>
		protected override void OnDeactivated(EventArgs e)
		{
			base.OnDeactivated(e);
			Dispatcher.BeginInvoke(new Action(CloseIfFocusLost), DispatcherPriority.Background);
		}
		#endregion
		
		/// <summary>
		/// Raises a tunnel/bubble event pair for a WPF control.
		/// </summary>
		/// <param name="target">The WPF control for which the event should be raised.</param>
		/// <param name="previewEvent">The tunneling event.</param>
		/// <param name="event">The bubbling event.</param>
		/// <param name="args">The event args to use.</param>
		/// <returns>The <see cref="RoutedEventArgs.Handled"/> value of the event args.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		protected static bool RaiseEventPair(UIElement target, RoutedEvent previewEvent, RoutedEvent @event, RoutedEventArgs args)
		{
			if (target == null)
				throw new ArgumentNullException("target");
			if (previewEvent == null)
				throw new ArgumentNullException("previewEvent");
			if (@event == null)
				throw new ArgumentNullException("event");
			if (args == null)
				throw new ArgumentNullException("args");
			args.RoutedEvent = previewEvent;
			target.RaiseEvent(args);
			args.RoutedEvent = @event;
			target.RaiseEvent(args);
			return args.Handled;
		}
		
		// Special handler: handledEventsToo
		void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			ActiveParentWindow();
		}
		
		/// <summary>
		/// Activates the parent window.
		/// </summary>
		protected virtual void ActiveParentWindow()
		{
			if (parentWindow != null)
				parentWindow.Activate();
		}
		
		void CloseIfFocusLost()
		{
			if (CloseOnFocusLost) {
				Debug.WriteLine("CloseIfFocusLost: this.IsActive=" + this.IsActive + " IsTextAreaFocused=" + IsTextAreaFocused);
				if (!this.IsActive && !IsTextAreaFocused) {
					Close();
				}
			}
		}
		
		/// <summary>
		/// Gets whether the completion window should automatically close when the text editor looses focus.
		/// </summary>
		protected virtual bool CloseOnFocusLost {
			get { return true; }
		}
		
		bool IsTextAreaFocused {
			get {
				if (parentWindow != null && !parentWindow.IsActive)
					return false;
				return this.TextArea.IsKeyboardFocused;
			}
		}
		
		/// <inheritdoc/>
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			
			if (document != null && this.StartOffset != this.TextArea.Caret.Offset) {
				SetPosition(new TextViewPosition(document.GetLocation(this.StartOffset)));
			} else {
				SetPosition(this.TextArea.Caret.Position);
			}
		}
		
		/// <inheritdoc/>
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			DetachEvents();
		}
		
		/// <inheritdoc/>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled && e.Key == Key.Escape) {
				e.Handled = true;
				Close();
			}
		}
		
		Point visualLocation, visualLocationTop;
		
		/// <summary>
		/// Positions the completion window at the specified position.
		/// </summary>
		protected void SetPosition(TextViewPosition position)
		{
			TextView textView = this.TextArea.TextView;
			
			visualLocation = textView.GetVisualPosition(position, VisualYPosition.LineBottom);
			visualLocationTop = textView.GetVisualPosition(position, VisualYPosition.LineTop);
			UpdatePosition();
		}
		
		void UpdatePosition()
		{
			TextView textView = this.TextArea.TextView;
			// PointToScreen returns device dependent units (physical pixels)
			Point location = textView.PointToScreen(visualLocation - textView.ScrollOffset);
			Point locationTop = textView.PointToScreen(visualLocationTop - textView.ScrollOffset);
			
			// Let's use device dependent units for everything
			Size completionWindowSize = new Size(this.ActualWidth, this.ActualHeight).TransformToDevice(textView);
			Rect bounds = new Rect(location, completionWindowSize);
			Rect workingScreen = System.Windows.Forms.Screen.GetWorkingArea(location.ToSystemDrawing()).ToWpf();
			if (!workingScreen.Contains(bounds)) {
				if (bounds.Left < workingScreen.Left) {
					bounds.X = workingScreen.Left;
				} else if (bounds.Right > workingScreen.Right) {
					bounds.X = workingScreen.Right - bounds.Width;
				}
				if (bounds.Bottom > workingScreen.Bottom) {
					bounds.Y = locationTop.Y - bounds.Height;
				}
				if (bounds.Y < workingScreen.Top) {
					bounds.Y = workingScreen.Top;
				}
			}
			// Convert the window bounds to device independent units
			bounds = bounds.TransformFromDevice(textView);
			this.Left = bounds.X;
			this.Top = bounds.Y;
		}
		
		/// <summary>
		/// Gets/Sets the start of the text range in which the completion window stays open.
		/// This text portion is used to determine the text used to select an entry in the completion list by typing.
		/// </summary>
		public int StartOffset {
			get { return startOffset; }
			set { startOffset = value; }
		}
		
		/// <summary>
		/// Gets/Sets the end of the text range in which the completion window stays open.
		/// This text portion is used to determine the text used to select an entry in the completion list by typing.
		/// </summary>
		public int EndOffset {
			get { return endOffset; }
			set { endOffset = value; }
		}
		
		/// <summary>
		/// Gets/sets whether the completion window should expect text insertion at the start offset,
		/// which not go into the completion region, but before it.
		/// </summary>
		/// <remarks>This property allows only a single insertion, it is reset to false
		/// when that insertion has occurred.</remarks>
		public bool ExpectInsertionBeforeStart { get; set; }
		
		void textArea_Document_Changing(object sender, DocumentChangeEventArgs e)
		{
			if (e.Offset == startOffset && e.RemovalLength == 0 && ExpectInsertionBeforeStart) {
				startOffset = e.GetNewOffset(startOffset, AnchorMovementType.AfterInsertion);
				this.ExpectInsertionBeforeStart = false;
			} else {
				startOffset = e.GetNewOffset(startOffset, AnchorMovementType.BeforeInsertion);
			}
			endOffset = e.GetNewOffset(endOffset, AnchorMovementType.AfterInsertion);
		}
	}
}
