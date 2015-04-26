// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	/// <summary>
	/// Base class for classes handling mouse gestures on the design surface.
	/// </summary>
	public abstract class MouseGestureBase
	{
		/// <summary>
		/// Checks if <paramref name="button"/> is the only button that is currently pressed.
		/// </summary>
		internal static bool IsOnlyButtonPressed(MouseEventArgs e, MouseButton button)
		{
			return e.LeftButton == (button == MouseButton.Left ? MouseButtonState.Pressed : MouseButtonState.Released)
				&& e.MiddleButton == (button == MouseButton.Middle ? MouseButtonState.Pressed : MouseButtonState.Released)
				&& e.RightButton == (button == MouseButton.Right ? MouseButtonState.Pressed : MouseButtonState.Released)
				&& e.XButton1 == (button == MouseButton.XButton1 ? MouseButtonState.Pressed : MouseButtonState.Released)
				&& e.XButton2 == (button == MouseButton.XButton2 ? MouseButtonState.Pressed : MouseButtonState.Released);
		}
		
		protected IDesignPanel designPanel;
		protected ServiceContainer services;
		protected bool canAbortWithEscape = true;
		bool isStarted;
		
		public void Start(IDesignPanel designPanel, MouseButtonEventArgs e)
		{
			if (designPanel == null)
				throw new ArgumentNullException("designPanel");
			if (e == null)
				throw new ArgumentNullException("e");
			if (isStarted)
				throw new InvalidOperationException("Gesture already was started");
			
			isStarted = true;
			this.designPanel = designPanel;
			this.services = designPanel.Context.Services;
			if (designPanel.CaptureMouse()) {
				RegisterEvents();
				OnStarted(e);
			} else {
				Stop();
			}
		}
		
		void RegisterEvents()
		{
			designPanel.LostMouseCapture += OnLostMouseCapture;
			designPanel.MouseDown += OnMouseDown;
			designPanel.MouseMove += OnMouseMove;
			designPanel.MouseUp += OnMouseUp;
			designPanel.KeyDown += OnKeyDown;
			designPanel.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
			designPanel.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
			designPanel.PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
			designPanel.PreviewMouseRightButtonUp += OnPreviewMouseRightButtonUp;
		}
		
		void UnRegisterEvents()
		{
			designPanel.LostMouseCapture -= OnLostMouseCapture;
			designPanel.MouseDown -= OnMouseDown;
			designPanel.MouseMove -= OnMouseMove;
			designPanel.MouseUp -= OnMouseUp;
			designPanel.KeyDown -= OnKeyDown;
			designPanel.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
			designPanel.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
			designPanel.PreviewMouseRightButtonDown -= OnPreviewMouseRightButtonDown;
			designPanel.PreviewMouseRightButtonUp -= OnPreviewMouseRightButtonUp;
		}
		
		void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (canAbortWithEscape && e.Key == Key.Escape) {
				e.Handled = true;
				Stop();
			}
		}
		
		void OnLostMouseCapture(object sender, MouseEventArgs e)
		{
			Stop();
		}
		
		protected virtual void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (MouseButtonHelper.IsDoubleClick(sender, e))
				OnMouseDoubleClick(sender, e);
		}
		
		protected virtual void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{ }
		
		protected virtual void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{ }
		
		protected virtual void OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{ }

		protected virtual void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{ }
		
		protected virtual void OnMouseDown(object sender, MouseButtonEventArgs e)
		{ }
		
		protected virtual void OnMouseMove(object sender, MouseEventArgs e)
		{
		}
		
		protected virtual void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			Stop();
		}
		
		protected void Stop()
		{
			if (!isStarted) return;
			isStarted = false;
			designPanel.ReleaseMouseCapture();
			UnRegisterEvents();
			OnStopped();
		}
		
		protected virtual void OnStarted(MouseButtonEventArgs e) {}
		protected virtual void OnStopped() {}
		
		static class MouseButtonHelper
		{
			[DllImport("user32.dll")]
			static extern uint GetDoubleClickTime();
			
			static MouseButtonHelper()
			{
				k_DoubleClickSpeed = GetDoubleClickTime();
			}
			
			private static readonly uint k_DoubleClickSpeed;
			
			private const double k_MaxMoveDistance = 10;
			
			private static long _LastClickTicks = 0;
			private static Point _LastPosition;
			private static WeakReference _LastSender;
			
			internal static bool IsDoubleClick(object sender, MouseButtonEventArgs e)
			{
				Point position = e.GetPosition(null);
				long clickTicks = DateTime.Now.Ticks;
				long elapsedTicks = clickTicks - _LastClickTicks;
				long elapsedTime = elapsedTicks / TimeSpan.TicksPerMillisecond;
				bool quickClick = (elapsedTime <= k_DoubleClickSpeed );
				bool senderMatch = (_LastSender != null && sender.Equals(_LastSender.Target));
				
				if (senderMatch && quickClick && Distance(position, _LastPosition) <= k_MaxMoveDistance)
				{
					// Double click!
					_LastClickTicks = 0;
					_LastSender = null;
					return true;
				}
				
				// Not a double click
				_LastClickTicks = clickTicks;
				_LastPosition = position;
				if (!quickClick)
					_LastSender = new WeakReference(sender);
				return false;
			}
			
			private static double Distance(Point pointA, Point pointB)
			{
				double x = pointA.X - pointB.X;
				double y = pointA.Y - pointB.Y;
				return Math.Sqrt(x * x + y * y);
			}
		}
	}
}
