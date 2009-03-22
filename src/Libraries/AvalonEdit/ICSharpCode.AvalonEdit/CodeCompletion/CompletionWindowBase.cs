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

using ICSharpCode.AvalonEdit.Gui;
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
		}
		
		#region Event Handlers
		/// <summary>
		/// Attaches events to the text area.
		/// </summary>
		protected virtual void AttachEvents()
		{
			this.TextArea.PreviewLostKeyboardFocus += TextAreaLostFocus;
			this.TextArea.TextView.ScrollOffsetChanged += TextViewScrollOffsetChanged;
			this.TextArea.TextView.DocumentChanged += TextViewDocumentChanged;
			this.TextArea.PreviewKeyDown += textArea_PreviewKeyDown;
			this.TextArea.PreviewKeyUp += textArea_PreviewKeyUp;
			if (parentWindow != null) {
				parentWindow.LocationChanged += parentWindow_LocationChanged;
			}
		}
		
		/// <summary>
		/// Detaches events from the text area.
		/// </summary>
		protected virtual void DetachEvents()
		{
			this.TextArea.PreviewLostKeyboardFocus -= TextAreaLostFocus;
			this.TextArea.TextView.ScrollOffsetChanged -= TextViewScrollOffsetChanged;
			this.TextArea.TextView.DocumentChanged -= TextViewDocumentChanged;
			this.TextArea.PreviewKeyDown -= textArea_PreviewKeyDown;
			this.TextArea.PreviewKeyUp -= textArea_PreviewKeyUp;
			if (parentWindow != null) {
				parentWindow.LocationChanged -= parentWindow_LocationChanged;
			}
		}
		
		void textArea_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = RaiseEventPair(this, PreviewKeyDownEvent, KeyDownEvent,
			                         new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key));
		}
		
		void textArea_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			e.Handled = RaiseEventPair(this, PreviewKeyUpEvent, KeyUpEvent,
			                         new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key));
		}
		
		void TextViewScrollOffsetChanged(object sender, EventArgs e)
		{
			UpdatePosition();
		}
		
		void TextViewDocumentChanged(object sender, EventArgs e)
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
			Debug.WriteLine("CloseIfFocusLost: this.IsActive=" + this.IsActive + " IsTextAreaFocused=" + IsTextAreaFocused);
			if (!this.IsActive && !IsTextAreaFocused) {
				Close();
			}
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
			SetPosition();
			AttachEvents();
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
		/// Positions the completion window at the caret position.
		/// </summary>
		void SetPosition()
		{
			TextView textView = this.TextArea.TextView;
			
			visualLocation = textView.GetVisualPosition(this.TextArea.Caret.Position, VisualYPosition.LineBottom);
			visualLocationTop = textView.GetVisualPosition(this.TextArea.Caret.Position, VisualYPosition.LineTop);
			UpdatePosition();
		}
		
		void UpdatePosition()
		{
			TextView textView = this.TextArea.TextView;
			Point location = textView.PointToScreen(visualLocation - textView.ScrollOffset);
			Point locationTop = textView.PointToScreen(visualLocationTop - textView.ScrollOffset);
			
			Size completionWindowSize = new Size(this.ActualWidth, this.ActualHeight);
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
			this.Left = bounds.X;
			this.Top = bounds.Y;
		}
	}
}
