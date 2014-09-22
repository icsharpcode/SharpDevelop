/*
 * Created by SharpDevelop.
 * User: Abdelkarim
 * Date: 9/22/2014
 * Time: 8:07 AM
 * 
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace ICSharpCode.AvalonEdit.AddIn
{
	[ContentProperty("PrimaryView")]
	public class EditorSplitContainer : FrameworkElement
	{
		#region "Fields"

		private readonly Rect _emptyRect = new Rect();
		private UIElement _secondaryViewContainer;
		private UIElement _primaryViewContainer;
		private SpliterGrip _grip;
		private readonly UIElementCollection _visualChildren;
		private double _gripOffset;
		// in percent
		private bool _isSecondaryViewCollapsing;
		private bool _isPrimaryViewCollapsing;
		private bool _hideSpliterGrip;
		private bool _isGripDragging;

		#endregion

		#region "Constructors"

		/// <summary>
		/// Initializes static members of the <see cref="EditorSplitContainer"/> class.
		/// </summary>
		static EditorSplitContainer()
		{
			ClipToBoundsProperty.OverrideMetadata(typeof(EditorSplitContainer), new FrameworkPropertyMetadata(BooleanBoxes.True));
			InitCommands();
			InitEventHandlers();
		}

		/// <summary>
		/// Initializes instance members of the <see cref="EditorSplitContainer"/> class.
		/// </summary>
		public EditorSplitContainer()
		{
			InitSplitterHandle();
			_visualChildren = new UIElementCollection(this, this) { _grip };
			LoadInInitialLayout();
		}

		#endregion

		#region "Events"

		#region RequestSecondaryView

		/// <summary>
		/// RequestSecondaryView Routed Event
		/// </summary>
		public static readonly RoutedEvent RequestSecondaryViewEvent = EventManager.RegisterRoutedEvent("RequestSecondaryView",
			                                                                     RoutingStrategy.Bubble, typeof(RequestSecondaryViewEventHandler), typeof(EditorSplitContainer));

		/// <summary>
		/// Occurs when ...
		/// </summary>
		public event RequestSecondaryViewEventHandler RequestSecondaryView {
			add { AddHandler(RequestSecondaryViewEvent, value); }
			remove { RemoveHandler(RequestSecondaryViewEvent, value); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected RequestSecondaryViewEventArgs RaiseRequestSecondaryViewEvent()
		{
			var args = new RequestSecondaryViewEventArgs { RoutedEvent = RequestSecondaryViewEvent };
			this.RaiseEvent(args);
			return args;
		}

		#endregion

		#region DisposeSecondaryView

		/// <summary>
		/// DisposeSecondaryView Routed Event
		/// </summary>
		public static readonly RoutedEvent DisposeSecondaryViewEvent = EventManager.RegisterRoutedEvent(
			                                                                     "DisposeSecondaryView",
			                                                                     RoutingStrategy.Bubble,
			                                                                     typeof(DisposeSecondaryViewEventHandler),
			                                                                     typeof(EditorSplitContainer));

		/// <summary>
		/// Occurs when ...
		/// </summary>
		public event DisposeSecondaryViewEventHandler DisposeSecondaryView {
			add { AddHandler(DisposeSecondaryViewEvent, value); }
			remove { RemoveHandler(DisposeSecondaryViewEvent, value); }
		}

		/// <summary>
		/// A helper method to raise the <see cref="DisposeSecondaryView"/> event.
		/// </summary>
		protected void RaiseDisposeSecondaryViewEvent(object container)
		{
			// It is not immediately raised, but when idle.
			// In case the dispose operation is expensive.

			this.Dispatcher.BeginInvoke(
				DispatcherPriority.ApplicationIdle,
				new Action<object>(c => {
					var args = new DisposeSecondaryViewEventArgs {
						RoutedEvent = DisposeSecondaryViewEvent,
						Container = c
					};
					this.RaiseEvent(args);
				}),
				container);
		}

		#endregion

		#endregion

		#region "Static Methods"

		private static void InitCommands()
		{
			CommandManager.RegisterClassCommandBinding(typeof(EditorSplitContainer), new CommandBinding(
				EditorSplitContainerCommands.SplitEditorCommand,
				(sender, e) => {
					var container = sender as EditorSplitContainer;
					if (container == null)
						return;
					container.OnSplitEditor();
				},
				(sender, e) => {
					var container = sender as EditorSplitContainer;
					if (container == null)
						return;

					e.CanExecute = container.IsSplit;
				}));

			CommandManager.RegisterClassCommandBinding(typeof(EditorSplitContainer), new CommandBinding(
				EditorSplitContainerCommands.RemoveSplitCommand,
				(sender, e) => {
					var container = sender as EditorSplitContainer;
					if (container == null)
						return;
					container.OnRemoveSplit();
				},
				(sender, e) => {
					var container = sender as EditorSplitContainer;
					if (container == null)
						return;

					e.CanExecute = !container.IsSplit;
				}));
		}

		private static void InitEventHandlers()
		{
			EventManager.RegisterClassHandler(typeof(EditorSplitContainer), Thumb.DragStartedEvent, new DragStartedEventHandler(OnEditorGripDragStarted));
			EventManager.RegisterClassHandler(typeof(EditorSplitContainer), Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnEditorGripDragDelta));
			EventManager.RegisterClassHandler(typeof(EditorSplitContainer), Thumb.DragCompletedEvent, new DragCompletedEventHandler(OnEditorGripDragCompleted));
		}

		private static void OnEditorGripDragCompleted(object sender, DragCompletedEventArgs e)
		{
			var splitContainer = sender as EditorSplitContainer;
			if (splitContainer == null)
				return;

			var thumb = e.OriginalSource as Thumb;
			if (thumb == null || thumb == splitContainer._grip || thumb.Name != "PART_EditorGrip")
				return;

			splitContainer.OnDragCompleted();
		}

		private static void OnEditorGripDragDelta(object sender, DragDeltaEventArgs e)
		{
			var splitContainer = sender as EditorSplitContainer;
			if (splitContainer == null)
				return;

			var thumb = e.OriginalSource as Thumb;
			if (thumb == null || thumb == splitContainer._grip || thumb.Name != "PART_EditorGrip")
				return;

			splitContainer.OnEditorGripDragDelta(splitContainer.Orientation == Orientation.Horizontal
                ? e.VerticalChange
                : e.HorizontalChange);

			e.Handled = true;
		}

		private static void OnEditorGripDragStarted(object sender, DragStartedEventArgs e)
		{
			var splitContainer = sender as EditorSplitContainer;
			if (splitContainer == null)
				return;

			var thumb = e.OriginalSource as Thumb;
			if (thumb == null || thumb == splitContainer._grip || thumb.Name != "PART_EditorGrip")
				return;

			// when this occur the editor grip is visible, and the split container has only
			// one view in it, the 2nd view.
			splitContainer.OnDragStarted();
		}

		#endregion

		#region "Properties"

		#region Orientation

		/// <summary>
		/// Identifies the <see cref="Orientation"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
			                                                                      "Orientation",
			                                                                      typeof(Orientation),
			                                                                      typeof(EditorSplitContainer),
			                                                                      new FrameworkPropertyMetadata(Orientation.Horizontal,
				                                                                      FrameworkPropertyMetadataOptions.AffectsArrange,
				                                                                      (o, e) => {
					var splitContainer = o as EditorSplitContainer;
					if (splitContainer == null || splitContainer._grip == null)
						return;

					splitContainer._grip.Orientation = (Orientation)e.NewValue;
				}),
			                                                                      IsValidOrientation);

		private static bool IsValidOrientation(object value)
		{
			var orientation = (Orientation)value;
			return orientation == Orientation.Horizontal || orientation == Orientation.Vertical;
		}

		/// <summary>
		/// Gets or sets the Orientation property. This is a dependency property.
		/// </summary>
		/// <value>
		///
		/// </value>
		[Bindable(true)]
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}

		#endregion

		#region IsSplit

		private static readonly DependencyPropertyKey IsSplitPropertyKey = DependencyProperty.RegisterReadOnly(
			                                                                         "IsSplit",
			                                                                         typeof(bool),
			                                                                         typeof(EditorSplitContainer),
			                                                                         new FrameworkPropertyMetadata(
				                                                                         BooleanBoxes.False,
				                                                                         (o, args) => {
					var container = (EditorSplitContainer)o;
					container.OnIsSplitChanged((bool)args.NewValue);
				}));

		public static readonly DependencyProperty IsSplitProperty = IsSplitPropertyKey.DependencyProperty;

		/// <summary>
		/// Gets a value that indicates whether the editor is split into two.
		/// </summary>
		public bool IsSplit {
			get { return (bool)GetValue(IsSplitProperty); }
			internal set { SetValue(IsSplitPropertyKey, value); }
		}

		#endregion

		#endregion

		#region "Plain Properties"

		/// <summary>
		/// Gets the number of visual child elements within this element.
		/// </summary>
		/// <returns>
		/// The number of visual child elements for this element.
		/// </returns>
		protected override int VisualChildrenCount {
			get {
				if (_visualChildren == null)
					return 0;

				return _visualChildren.Count;
			}
		}

		/// <summary>
		/// Gets an enumerator for logical child elements of this element. 
		/// </summary>
		/// <returns>
		/// An enumerator for logical child elements of this element.
		/// </returns>
		protected override IEnumerator LogicalChildren {
			get {
				var arrayList = new ArrayList(3);

				foreach (var child in _visualChildren)
					arrayList.Add(child);

				return arrayList.GetEnumerator();
			}
		}

		/// <summary>
		/// The element that hosts the secondary view.
		/// </summary>
		public UIElement SecondaryView {
			get { return _secondaryViewContainer == null ? null : _secondaryViewContainer; }
			private set {
				UpdateView(_secondaryViewContainer, value);
				_secondaryViewContainer = value;
			}
		}

		/// <summary>
		/// The element that hosts the primary view.
		/// </summary>
		public UIElement PrimaryView {
			get { return _primaryViewContainer; }
			set {
				if (_primaryViewContainer == value)
					throw new NotSupportedException("");

				UpdateView(_primaryViewContainer, value);
				_primaryViewContainer = value;
			}
		}

		/// <summary>
		/// Gets a value that indicates whether we can snap the spliter grip to the top edge.
		/// </summary>
		private bool CanCollapseSecondaryView {
			get {
				if (Density > 0) {
					var offset = 20 * Density;
					return _gripOffset <= offset;
				}

				return false;
			}
		}

		/// <summary>
		/// Gets a value that indicates whether we can snap the spliter grip to the bottom edge
		/// </summary>
		private bool CanCollapsePrimaryView {
			get {
				if (Density > 0) {
					var offset = 20 * Density;
					return _gripOffset >= (100 - offset);
				}
				return false;
			}
		}

		private double Density { get; set; }
		private bool InternalIsSplit { get; set; }
        
		#endregion

		#region "Public Methods"

		/// <summary>
		/// Will split the editor into half.
		/// </summary>
		public void OnSplitEditor()
		{
			if (!IsSplit)
				return;

			_gripOffset = 50;
			ComputeOffset(0);

			InternalIsSplit = false;
			IsSplit = false;

			InvalidateVisual();
		}

		/// <summary>
		/// Will remove the split from the editor if it is present
		/// </summary>
		public void OnRemoveSplit()
		{
			if (IsSplit)
				return;

			_gripOffset = 0;
			ComputeOffset(0);

			InternalIsSplit = true;
			IsSplit = true;

			InvalidateVisual();
		}

		#endregion

		#region "Protected Overrides"

		protected override Visual GetVisualChild(int index)
		{
			if (_visualChildren == null || (index < 0 || index >= _visualChildren.Count))
				throw new ArgumentOutOfRangeException("index");

			return _visualChildren[index];
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			bool isHorizontal = Orientation == Orientation.Horizontal;

			if (isHorizontal && (double.IsPositiveInfinity(availableSize.Height) || double.IsInfinity(availableSize.Height)))
				return base.MeasureOverride(availableSize);

			if (!isHorizontal && (double.IsPositiveInfinity(availableSize.Width) || double.IsInfinity(availableSize.Width)))
				return base.MeasureOverride(availableSize);

			double startPos, handleSize, secondaryViewSize, primaryViewSize;
			this.ComputePartsSizes(availableSize, isHorizontal, out secondaryViewSize, out primaryViewSize, out handleSize, out startPos);

			var secondaryView = SecondaryView;
			if (secondaryView != null)
				secondaryView.Measure(isHorizontal
                    ? new Size(availableSize.Width, secondaryViewSize)
                    : new Size(secondaryViewSize, availableSize.Height));

			var view2 = PrimaryView;
			if (view2 != null)
				view2.Measure(isHorizontal
                    ? new Size(availableSize.Width, primaryViewSize)
                    : new Size(primaryViewSize, availableSize.Height));

			_grip.Measure(availableSize);
			return _grip.DesiredSize;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			// initialization
			var isHorizontal = Orientation == Orientation.Horizontal;
			var secondaryView = SecondaryView;
			var primaryView = PrimaryView;

			double secondaryViewSize, primaryViewSize, gripSize, startPos;
			ComputePartsSizes(finalSize, isHorizontal, out secondaryViewSize, out primaryViewSize, out gripSize, out startPos);


			Rect finalRect = isHorizontal
                ? new Rect {
				Location = new Point(0, startPos),
				Width = finalSize.Width
			}
                : new Rect {
				Location = new Point(startPos, 0),
				Height = finalSize.Height
			};

			if (InternalIsSplit) {
				if (_isSecondaryViewCollapsing) {
					// SecondaryView
					if (secondaryView != null)
						secondaryView.Arrange(_emptyRect);

					// Grip
					if (isHorizontal) {
						finalRect.Y = startPos;
						finalRect.Height = gripSize;
					} else {
						finalRect.X = startPos;
						finalRect.Width = gripSize;
					}
					_grip.Arrange(!_hideSpliterGrip ? finalRect : _emptyRect);

					// view2
					if (isHorizontal) {
						finalRect.Y += gripSize;
						finalRect.Height = primaryViewSize;
					} else {
						finalRect.X += gripSize;
						finalRect.Width = primaryViewSize;
					}

					if (primaryView != null)
						primaryView.Arrange(finalRect);
				}

				if (_isPrimaryViewCollapsing) {
					// SecondaryView
					if (isHorizontal)
						finalRect.Height = secondaryViewSize;
					else
						finalRect.Width = secondaryViewSize;
					if (secondaryView != null)
						secondaryView.Arrange(finalRect);

					// Grip
					if (isHorizontal) {
						finalRect.Y += secondaryViewSize;
						finalRect.Height = gripSize;
					} else {
						finalRect.X += secondaryViewSize;
						finalRect.Width = gripSize;
					}
					_grip.Arrange(!_hideSpliterGrip ? finalRect : _emptyRect);

					// PrimaryView
					if (primaryView != null)
						primaryView.Arrange(_emptyRect);
				}
			} else {
				// SecondaryView
				if (isHorizontal)
					finalRect.Height = secondaryViewSize;
				else
					finalRect.Width = secondaryViewSize;


				if (secondaryView != null)
					secondaryView.Arrange(finalRect);

				// grip
				if (isHorizontal) {
					finalRect.Y += secondaryViewSize;
					finalRect.Height = gripSize;
				} else {
					finalRect.X += secondaryViewSize;
					finalRect.Width = gripSize;
				}
				_grip.Arrange(finalRect);

				// PrimaryView
				if (isHorizontal) {
					finalRect.Y += gripSize;
					finalRect.Height = primaryViewSize;
				} else {
					finalRect.X += gripSize;
					finalRect.Width = primaryViewSize;
				}

				if (primaryView != null)
					primaryView.Arrange(finalRect);
			}

			return finalSize;
		}

		#endregion

		#region "Methods"

		private void InitSplitterHandle()
		{
			_grip = new SpliterGrip {
				Orientation = this.Orientation
			};

			_grip.DragStarted += OnDragStarted;
			_grip.DragDelta += OnGripDragDelta;
			_grip.DragCompleted += OnDragCompleted;
		}

		private void OnIsSplitChanged(bool newValue)
		{
			if (!newValue) {
				var args = RaiseRequestSecondaryViewEvent();
				var container = args.Container as UIElement;
				if (container == null)
					return;

				SecondaryView = container;
			} else {
				var secondaryView = this.SecondaryView;
				if (secondaryView == null)
					return;

				SecondaryView = null; // will cause it to be removed from the logical/visual trees.

				RaiseDisposeSecondaryViewEvent(secondaryView);
			}
		}

		private void OnDragStarted(object sender, DragStartedEventArgs e)
		{
			OnDragStarted();
		}

		private void OnDragStarted()
		{
			_hideSpliterGrip = false;
			_isGripDragging = true;
			IsSplit = false;
		}

		private void LoadInInitialLayout()
		{
			ComputeOffset(0);
			IsSplit = InternalIsSplit;
		}

		private void OnDragCompleted(object sender, DragCompletedEventArgs e)
		{
			OnDragCompleted();
		}

		/// <summary>
		/// Common method between split container grip, and editor grip
		/// </summary>
		private void OnDragCompleted()
		{
			_isGripDragging = false;

			if (!_isSecondaryViewCollapsing && !_isPrimaryViewCollapsing) {
				// we need to check if we are close to be collapsed, we snap one of them.
				if (CanCollapseSecondaryView) {
					_isSecondaryViewCollapsing = true;
					InternalIsSplit = true;
				} else if (CanCollapsePrimaryView) {
					_isPrimaryViewCollapsing = true;
					InternalIsSplit = true;
				} else {
					IsSplit = false;
					return;
				}
			}

			_hideSpliterGrip = InternalIsSplit;

			// a final layout pass is to be made, in order to ensure
			// that the active view has been measure/arrange against the available
			// space.

			if (InternalIsSplit) {
				if (_isPrimaryViewCollapsing)
					SwapViews();

				// reset the offset
				_gripOffset = -(6 * Density);
				_isSecondaryViewCollapsing = true;
				_isPrimaryViewCollapsing = false;

				InvalidateMeasure();
				InvalidateArrange();
			}

			IsSplit = InternalIsSplit;
		}

		private void OnGripDragDelta(object sender, DragDeltaEventArgs e) {
			bool isHorizontal = Orientation == Orientation.Horizontal;
			double change = isHorizontal ? e.VerticalChange : e.HorizontalChange;
            
			ComputeOffset(change);
			e.Handled = true;
		}

		protected void OnEditorGripDragDelta(double change) {
			ComputeOffset(change);
		}

		private void ComputeOffset(double change) {
			//double offset = double.IsNaN(_gripOffset) ? 50 : _gripOffset;
			double offset = _gripOffset;
			offset += Density * change;

			CoerceOffset(ref offset);

			_gripOffset = offset;
			InvalidateMeasure();
			InvalidateArrange();
		}

		private void CoerceOffset(ref double offset)
		{
			_isPrimaryViewCollapsing = _isSecondaryViewCollapsing = false;
			bool isCollapsed = false;
			double min = 0;
			double max = 100;
            
			var gripOffset = Density * 6;

			min -= gripOffset;
			max += gripOffset;


			if (offset < min) {
				offset = min;
			} else if (offset > max) {
				offset = max;
			}

			if (offset <= 0) {
				isCollapsed = true;
				_isSecondaryViewCollapsing = true;
				_isPrimaryViewCollapsing = false;
			} else if (offset >= 100) {
				isCollapsed = true;
				_isSecondaryViewCollapsing = false;
				_isPrimaryViewCollapsing = true;
			}

			InternalIsSplit = isCollapsed;
		}

		private void UpdateView(UIElement oldView, UIElement newView)
		{
			if (oldView == newView)
				return;

			if (oldView != null)
				_visualChildren.Remove(oldView);

			if (newView != null)
				_visualChildren.Add(newView);

			InvalidateMeasure();
			InvalidateArrange();
		}

		private void ComputePartsSizes(Size arrangeSize,
			bool isHorizontal,
			out double secondaryViewSize,
			out double primaryViewSize,
			out double handleSize,
			out double startPos)
		{
			double offset = double.IsNaN(_gripOffset) ? 50 : _gripOffset;
			startPos = 0;

			// ensure proper sizes are selected.
			double length;
			if (isHorizontal) {
				length = arrangeSize.Height;
				handleSize = _grip.DesiredSize.Height;
			} else {
				length = arrangeSize.Width;
				handleSize = _grip.DesiredSize.Width;
			}

			// compute density;



			double remainingLength;
			if (InternalIsSplit && !_isGripDragging) {
				// only a single view is rendered, PrimaryView
				remainingLength = length;
				secondaryViewSize = 0;
				primaryViewSize = remainingLength;
				startPos = -handleSize;

				// ensure that grip offset is correctly set.
				var density = 100 / remainingLength;
				_gripOffset = -(density * handleSize);
			} else {
				remainingLength = length - handleSize;
				if (offset >= 0 && offset <= 100) {
					secondaryViewSize = remainingLength * (offset / 100);
					primaryViewSize = remainingLength - secondaryViewSize;
				} else if (offset < 0) {
					double handleVisiblePart = Math.Max(0.0, handleSize - (Math.Abs(offset) / 100) * remainingLength);
					startPos = (handleSize - handleVisiblePart) * -1;
					secondaryViewSize = primaryViewSize = length - handleVisiblePart;
				} else { //offset > 100
					double handleVisiblePart = Math.Max(0.0, handleSize - ((offset - 100) / 100) * remainingLength);
					primaryViewSize = secondaryViewSize = length - handleVisiblePart;
				}
			}

			Density = 100 / remainingLength;
		}

		private void SwapViews()
		{
			var temp = _secondaryViewContainer;
			_secondaryViewContainer = _primaryViewContainer;
			_primaryViewContainer = temp;
		}

		#endregion
	}

	public class RequestSecondaryViewEventArgs : RoutedEventArgs
	{
		public object Container { get; set; }
	}
	public delegate void RequestSecondaryViewEventHandler(object sender, RequestSecondaryViewEventArgs e);

	public class DisposeSecondaryViewEventArgs : RoutedEventArgs
	{
		public object Container { get; set; }
	}
	public delegate void DisposeSecondaryViewEventHandler(object sender, DisposeSecondaryViewEventArgs e);

	internal static class BooleanBoxes
	{
		public static object True = true;
		public static object False = false;

		public static object Box(bool value)
		{
			return value ? True : False;
		}
	}
}
