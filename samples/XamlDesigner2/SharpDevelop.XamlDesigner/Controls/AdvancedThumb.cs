using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Input;
using SharpDevelop.XamlDesigner.Dom.UndoSystem;

namespace SharpDevelop.XamlDesigner.Controls
{
	public class AdvancedThumb : Control
	{
		static AdvancedThumb()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AdvancedThumb),
				new FrameworkPropertyMetadata(typeof(AdvancedThumb)));
		}

		public AdvancedThumb()
		{
			UseDelay = true;
			args = new AdvancedDragEventArgs(this);
		}

		public IInputElement RelativeTo { get; set; }
		public bool UseDelay { get; set; }
		
		Point startPoint;
		Vector delta;
		bool isMouseDown;
		bool isDragging;
		bool isCancel;
		AdvancedDragEventArgs args;

		public Point StartPoint
		{
			get { return startPoint; }
		}

		public Vector Delta
		{
			get { return delta; }
		}

		public bool IsCancel
		{
			get { return isCancel; }
		}

		public event AdvancedDragEventHandler DragStarted;
		public event AdvancedDragEventHandler DragDelta;
		public event AdvancedDragEventHandler DragCompleted;
		public event AdvancedDragEventHandler Click;

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			Focus();
			startPoint = Mouse.GetPosition(RelativeTo);
			isMouseDown = true;
			isDragging = false;
			CaptureMouse();
			e.Handled = true;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (isMouseDown) {
				delta = Mouse.GetPosition(RelativeTo) - startPoint;
				if (isDragging) {
					OnDragDelta();
				}
				else {
					if (!UseDelay ||
						Math.Abs(delta.X) > SystemParameters.MinimumHorizontalDragDistance ||
						Math.Abs(delta.Y) > SystemParameters.MinimumHorizontalDragDistance) {
						isDragging = true;
						OnDragStarted();
					}
				}
			}
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (isMouseDown) {
				ReleaseMouseCapture();
				if (!isDragging) {
					OnClick();
				}
				Complete(false);
			}
		}

		protected virtual void OnDragStarted()
		{
			UndoManager.OnDragStarted(this);
			if (DragStarted != null) {
				DragStarted(this, args);
			}
		}

		protected virtual void OnDragDelta()
		{
			if (DragDelta != null) {
				DragDelta(this, args);
			}
		}

		protected virtual void OnDragCompleted()
		{
			if (isCancel) {
				UndoManager.OnDragCanceled(this);
			}
			else {
				UndoManager.OnDragCompleted(this);
			}
			if (DragCompleted != null) {
				DragCompleted(this, args);
			}
		}

		protected virtual void OnClick()
		{
			if (Click != null) {
				Click(this, args);
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Escape) {
				CancelDrag();
			}
		}

		public void CancelDrag()
		{
			Complete(true);
		}

		void Complete(bool cancel)
		{
			if (isMouseDown) {
				isMouseDown = false;
				ReleaseMouseCapture();
				if (isDragging) {
					isDragging = false;
					isCancel = cancel;
					OnDragCompleted();
				}
			}
		}
	}
}
