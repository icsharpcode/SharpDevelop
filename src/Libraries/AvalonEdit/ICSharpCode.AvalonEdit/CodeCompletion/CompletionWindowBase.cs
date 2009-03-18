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
			if (parentWindow != null) {
				parentWindow.LocationChanged -= parentWindow_LocationChanged;
			}
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
		
		void CloseIfFocusLost()
		{
			Debug.WriteLine("CloseIfFocusLost");
			if (!this.IsActive && IsTextAreaFocused) {
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
