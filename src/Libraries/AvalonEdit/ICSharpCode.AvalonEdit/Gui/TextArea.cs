// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Gui;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit
{
	/// <summary>
	/// Control that wraps a TextView and adds support for user input and the caret.
	/// </summary>
	public class TextArea : Control, IScrollInfo, IWeakEventListener
	{
		#region Constructor
		static TextArea()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TextArea),
			                                         new FrameworkPropertyMetadata(typeof(TextArea)));
			KeyboardNavigation.IsTabStopProperty.OverrideMetadata(
				typeof(TextArea), new FrameworkPropertyMetadata(Boxes.True));
			KeyboardNavigation.TabNavigationProperty.OverrideMetadata(
				typeof(TextArea), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
			FocusableProperty.OverrideMetadata(
				typeof(TextArea), new FrameworkPropertyMetadata(Boxes.True));
			FocusVisualStyleProperty.OverrideMetadata(
				typeof(TextArea), new FrameworkPropertyMetadata(null));
		}
		
		/// <summary>
		/// Creates a new TextArea instance.
		/// </summary>
		public TextArea() : this(new TextView())
		{
		}
		
		/// <summary>
		/// Creates a new TextArea instance.
		/// </summary>
		protected TextArea(TextView textView)
		{
			if (textView == null)
				throw new ArgumentNullException("textView");
			this.textView = textView;
			textView.SetBinding(TextView.DocumentProperty, new Binding("Document") { Source = this });
			
			leftMargins.Add(new LineNumberMargin { TextView = textView, TextArea = this } );
			leftMargins.Add(new Line {
			                	X1 = 0, Y1 = 0, X2 = 0, Y2 = 1,
			                	StrokeDashArray = { 0, 2 },
			                	Stretch = Stretch.Fill,
			                	Stroke = Brushes.Gray,
			                	StrokeThickness = 1,
			                	StrokeDashCap = PenLineCap.Round,
			                	Margin = new Thickness(2, 0, 2, 0)
			                });
			
			textView.LineTransformers.Add(new SelectionColorizer(this));
			textView.InsertLayer(new SelectionLayer(this), KnownLayer.Selection, LayerInsertionPosition.Replace);
			
			caret = new Caret(this);
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, ExecuteUndo, CanExecuteUndo));
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, ExecuteRedo, CanExecuteRedo));
			
			this.CommandBindings.AddRange(CaretNavigationCommandHandler.CommandBindings);
			this.InputBindings.AddRange(CaretNavigationCommandHandler.InputBindings);
			
			this.CommandBindings.AddRange(EditingCommandHandler.CommandBindings);
			this.InputBindings.AddRange(EditingCommandHandler.InputBindings);
			
			new SelectionMouseHandler(this).Attach();
		}
		#endregion
		
		#region Document property
		/// <summary>
		/// Document property.
		/// </summary>
		public static readonly DependencyProperty DocumentProperty
			= TextEditor.DocumentProperty.AddOwner(typeof(TextArea), new FrameworkPropertyMetadata(OnDocumentChanged));
		
		/// <summary>
		/// Gets/Sets the document displayed by the text editor.
		/// </summary>
		public TextDocument Document {
			get { return (TextDocument)GetValue(DocumentProperty); }
			set { SetValue(DocumentProperty, value); }
		}
		
		static void OnDocumentChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			((TextArea)dp).OnDocumentChanged((TextDocument)e.OldValue, (TextDocument)e.NewValue);
		}
		
		void OnDocumentChanged(TextDocument oldValue, TextDocument newValue)
		{
			if (oldValue != null) {
				TextDocumentWeakEventManager.Changing.RemoveListener(oldValue, this);
				TextDocumentWeakEventManager.Changed.RemoveListener(oldValue, this);
				TextDocumentWeakEventManager.UpdateStarted.RemoveListener(oldValue, this);
			}
			if (newValue != null) {
				TextDocumentWeakEventManager.Changing.AddListener(newValue, this);
				TextDocumentWeakEventManager.Changed.AddListener(newValue, this);
				TextDocumentWeakEventManager.UpdateStarted.AddListener(newValue, this);
			}
			CommandManager.InvalidateRequerySuggested();
		}
		#endregion
		
		#region Caret handling on document changes
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			if (managerType == typeof(TextDocumentWeakEventManager.Changing)) {
				caret.OnDocumentChanging();
				return true;
			} else if (managerType == typeof(TextDocumentWeakEventManager.Changed)) {
				OnDocumentChanged((DocumentChangeEventArgs)e);
				return true;
			} else if (managerType == typeof(TextDocumentWeakEventManager.UpdateStarted)) {
				OnUpdateStarted();
				return true;
			}
			return false;
		}
		
		void OnDocumentChanged(DocumentChangeEventArgs e)
		{
			caret.OnDocumentChanged(e);
			this.Selection = selection.UpdateOnDocumentChange(e);
		}
		
		void OnUpdateStarted()
		{
			Document.UndoStack.PushOptional(new RestoreCaretAndSelectionUndoAction(this));
		}
		
		sealed class RestoreCaretAndSelectionUndoAction : IUndoableOperation
		{
			// keep textarea in weak reference because the IUndoableOperation is stored with the document
			WeakReference textAreaReference;
			TextViewPosition caretPosition;
			Selection selection;
			
			public RestoreCaretAndSelectionUndoAction(TextArea textArea)
			{
				this.textAreaReference = new WeakReference(textArea);
				this.caretPosition = textArea.Caret.Position;
				this.selection = textArea.Selection;
			}
			
			public void Undo()
			{
				TextArea textArea = (TextArea)textAreaReference.Target;
				if (textArea != null) {
					textArea.Caret.Position = caretPosition;
					textArea.Selection = selection;
				}
			}
			
			public void Redo()
			{
				// redo=undo: we just restore the caret/selection state
				Undo();
			}
		}
		#endregion
		
		#region TextView property
		readonly TextView textView;
		IScrollInfo scrollInfo;

		/// <summary>
		/// Gets the text view used to display text in this text area.
		/// </summary>
		public TextView TextView {
			get {
				return textView;
			}
		}
		/// <inheritdoc/>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			scrollInfo = textView;
			ApplyScrollInfo();
		}
		#endregion
		
		#region Selection property
		Selection selection = Selection.Empty;
		
		/// <summary>
		/// Occurs when the selection has changed.
		/// </summary>
		public event EventHandler SelectionChanged;
		
		/// <summary>
		/// Gets/Sets the selection in this text area.
		/// </summary>
		public Selection Selection {
			get { return selection; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				if (!object.Equals(selection, value)) {
					Debug.WriteLine("Selection change from " + selection + " to " + value);
					if (textView != null) {
						textView.Redraw(selection.SurroundingSegment, DispatcherPriority.Background);
						textView.Redraw(value.SurroundingSegment, DispatcherPriority.Background);
					}
					selection = value;
					if (SelectionChanged != null)
						SelectionChanged(this, EventArgs.Empty);
				}
			}
		}
		#endregion
		
		#region Properties
		readonly Caret caret;
		
		/// <summary>
		/// Gets the Caret used for this text area.
		/// </summary>
		public Caret Caret {
			get { return caret; }
		}
		
		ObservableCollection<UIElement> leftMargins = new ObservableCollection<UIElement>();
		
		/// <summary>
		/// Gets the collection of margins displayed to the left of the text view.
		/// </summary>
		public ObservableCollection<UIElement> LeftMargins {
			get {
				return leftMargins;
			}
		}
		
		IReadOnlySectionProvider readOnlySectionProvider = NoReadOnlySections.Instance;
		
		/// <summary>
		/// Gets/Sets an object that provides read-only sections for the text area.
		/// </summary>
		public IReadOnlySectionProvider ReadOnlySectionProvider {
			get { return readOnlySectionProvider; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				readOnlySectionProvider = value;
			}
		}
		#endregion
		
		#region Undo / Redo
		UndoStack GetUndoStack()
		{
			TextDocument document = this.Document;
			if (document != null)
				return document.UndoStack;
			else
				return null;
		}
		
		void ExecuteUndo(object sender, ExecutedRoutedEventArgs e)
		{
			var undoStack = GetUndoStack();
			if (undoStack != null && undoStack.CanUndo)
				undoStack.Undo();
		}
		
		void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
		{
			var undoStack = GetUndoStack();
			e.CanExecute = undoStack != null && undoStack.CanUndo;
		}
		
		void ExecuteRedo(object sender, ExecutedRoutedEventArgs e)
		{
			var undoStack = GetUndoStack();
			if (undoStack != null && undoStack.CanRedo)
				undoStack.Redo();
		}
		
		void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e)
		{
			var undoStack = GetUndoStack();
			e.CanExecute = undoStack != null && undoStack.CanRedo;
		}
		#endregion
		
		#region IScrollInfo implementation
		ScrollViewer scrollOwner;
		bool canVerticallyScroll, canHorizontallyScroll;
		
		void ApplyScrollInfo()
		{
			if (scrollInfo != null) {
				scrollInfo.ScrollOwner = scrollOwner;
				scrollInfo.CanVerticallyScroll = canVerticallyScroll;
				scrollInfo.CanHorizontallyScroll = canHorizontallyScroll;
				scrollOwner = null;
			}
		}
		
		bool IScrollInfo.CanVerticallyScroll {
			get { return scrollInfo != null ? scrollInfo.CanVerticallyScroll : false; }
			set {
				canVerticallyScroll = value;
				if (scrollInfo != null)
					scrollInfo.CanVerticallyScroll = value;
			}
		}
		
		bool IScrollInfo.CanHorizontallyScroll {
			get { return scrollInfo != null ? scrollInfo.CanHorizontallyScroll : false; }
			set {
				canHorizontallyScroll = value;
				if (scrollInfo != null)
					scrollInfo.CanHorizontallyScroll = value;
			}
		}
		
		double IScrollInfo.ExtentWidth {
			get { return scrollInfo != null ? scrollInfo.ExtentWidth : 0; }
		}
		
		double IScrollInfo.ExtentHeight {
			get { return scrollInfo != null ? scrollInfo.ExtentHeight : 0; }
		}
		
		double IScrollInfo.ViewportWidth {
			get { return scrollInfo != null ? scrollInfo.ViewportWidth : 0; }
		}
		
		double IScrollInfo.ViewportHeight {
			get { return scrollInfo != null ? scrollInfo.ViewportHeight : 0; }
		}
		
		double IScrollInfo.HorizontalOffset {
			get { return scrollInfo != null ? scrollInfo.HorizontalOffset : 0; }
		}
		
		double IScrollInfo.VerticalOffset {
			get { return scrollInfo != null ? scrollInfo.VerticalOffset : 0; }
		}
		
		ScrollViewer IScrollInfo.ScrollOwner {
			get { return scrollInfo != null ? scrollInfo.ScrollOwner : null; }
			set {
				if (scrollInfo != null)
					scrollInfo.ScrollOwner = value;
				else
					scrollOwner = value;
			}
		}
		
		void IScrollInfo.LineUp()
		{
			if (scrollInfo != null) scrollInfo.LineUp();
		}
		
		void IScrollInfo.LineDown()
		{
			if (scrollInfo != null) scrollInfo.LineDown();
		}
		
		void IScrollInfo.LineLeft()
		{
			if (scrollInfo != null) scrollInfo.LineLeft();
		}
		
		void IScrollInfo.LineRight()
		{
			if (scrollInfo != null) scrollInfo.LineRight();
		}
		
		void IScrollInfo.PageUp()
		{
			if (scrollInfo != null) scrollInfo.PageUp();
		}
		
		void IScrollInfo.PageDown()
		{
			if (scrollInfo != null) scrollInfo.PageDown();
		}
		
		void IScrollInfo.PageLeft()
		{
			if (scrollInfo != null) scrollInfo.PageLeft();
		}
		
		void IScrollInfo.PageRight()
		{
			if (scrollInfo != null) scrollInfo.PageRight();
		}
		
		void IScrollInfo.MouseWheelUp()
		{
			if (scrollInfo != null) scrollInfo.MouseWheelUp();
		}
		
		void IScrollInfo.MouseWheelDown()
		{
			if (scrollInfo != null) scrollInfo.MouseWheelDown();
		}
		
		void IScrollInfo.MouseWheelLeft()
		{
			if (scrollInfo != null) scrollInfo.MouseWheelLeft();
		}
		
		void IScrollInfo.MouseWheelRight()
		{
			if (scrollInfo != null) scrollInfo.MouseWheelRight();
		}
		
		void IScrollInfo.SetHorizontalOffset(double offset)
		{
			if (scrollInfo != null) scrollInfo.SetHorizontalOffset(offset);
		}
		
		void IScrollInfo.SetVerticalOffset(double offset)
		{
			if (scrollInfo != null) scrollInfo.SetVerticalOffset(offset);
		}
		
		Rect IScrollInfo.MakeVisible(System.Windows.Media.Visual visual, Rect rectangle)
		{
			if (scrollInfo != null)
				return scrollInfo.MakeVisible(visual, rectangle);
			else
				return Rect.Empty;
		}
		#endregion
		
		#region Focus Handling (Show/Hide Caret)
		/// <inheritdoc/>
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			Focus();
		}
		
		/// <inheritdoc/>
		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			base.OnGotKeyboardFocus(e);
			caret.Show();
			e.Handled = true;
		}
		
		/// <inheritdoc/>
		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			base.OnLostKeyboardFocus(e);
			caret.Hide();
			e.Handled = true;
		}
		#endregion
		
		#region OnTextInput / RemoveSelectedText / ReplaceSelectionWithText
		/// <inheritdoc/>
		protected override void OnTextInput(TextCompositionEventArgs e)
		{
			base.OnTextInput(e);
			if (!e.Handled) {
				TextDocument document = this.Document;
				if (document != null) {
					ReplaceSelectionWithText(e.Text);
					caret.BringCaretToView();
					e.Handled = true;
				}
			}
		}
		
		internal void RemoveSelectedText()
		{
			selection.RemoveSelectedText(this);
			#if DEBUG
			if (!selection.IsEmpty) {
				foreach (ISegment s in selection.Segments) {
					Debug.Assert(ReadOnlySectionProvider.GetDeletableSegments(s).Count() == 0);
				}
			}
			#endif
		}
		
		internal void ReplaceSelectionWithText(string newText)
		{
			Document.BeginUpdate();
			try {
				RemoveSelectedText();
				if (ReadOnlySectionProvider.CanInsert(Caret.Offset)) {
					Document.Insert(Caret.Offset, newText);
				}
			} finally {
				Document.EndUpdate();
			}
		}
		#endregion
	}
}
