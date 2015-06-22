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
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Input;

using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	public class ZoomControl : ZoomScrollViewer
	{
		static ZoomControl()
		{
			PanToolCursor = GetCursor("Images/PanToolCursor.cur");
			PanToolCursorMouseDown = GetCursor("Images/PanToolCursorMouseDown.cur");
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomControl), new FrameworkPropertyMetadata(typeof(ZoomControl)));
		}

		public object AdditionalControls
		{
			get { return (object)GetValue(AdditionalControlsProperty); }
			set { SetValue(AdditionalControlsProperty, value); }
		}
		
		public static readonly DependencyProperty AdditionalControlsProperty =
			DependencyProperty.Register("AdditionalControls", typeof(object), typeof(ZoomControl), new PropertyMetadata(null));

		internal static Cursor GetCursor(string path)
		{
			var a = Assembly.GetExecutingAssembly();
			var m = new ResourceManager(a.GetName().Name + ".g", a);
			using (Stream s = m.GetStream(path.ToLowerInvariant())) {
				return new Cursor(s);
			}
		}

		static Cursor PanToolCursor;
		static Cursor PanToolCursorMouseDown;
		
		double startHorizontalOffset;
		double startVericalOffset;
		Point startPoint;
		bool isMouseDown;
		bool pan;

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (!pan && e.Key == Key.Space) {
				pan = true;
				Mouse.UpdateCursor();
			}
			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.Key == Key.Space) {
				pan = false;
				Mouse.UpdateCursor();
			}
			base.OnKeyUp(e);
		}

		protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
		{
			if (!pan && e.MiddleButton == MouseButtonState.Pressed)
			{
				pan = true;
				Mouse.UpdateCursor();
			}
			
			if (pan && !e.Handled) {
				if (Mouse.Capture(this)) {
					isMouseDown = true;
					e.Handled = true;
					startPoint = e.GetPosition(this);
					PanStart();
					Mouse.UpdateCursor();
				}
			}
			base.OnPreviewMouseDown(e);
		}

		protected override void OnPreviewMouseMove(MouseEventArgs e)
		{
			if (isMouseDown) {
				var endPoint = e.GetPosition(this);
				PanContinue(endPoint - startPoint);
			}
			base.OnPreviewMouseMove(e);
		}

		protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
		{
			if (pan && e.MiddleButton != MouseButtonState.Pressed && !Keyboard.IsKeyDown(Key.Space))
			{
				pan = false;
				Mouse.UpdateCursor();
			}
			
			if (isMouseDown) {
				isMouseDown = false;
				ReleaseMouseCapture();
				Mouse.UpdateCursor();
			}
			base.OnPreviewMouseUp(e);
		}
		
		protected override void OnLostMouseCapture(MouseEventArgs e)
		{
			if (isMouseDown) {
				isMouseDown = false;
				ReleaseMouseCapture();
				Mouse.UpdateCursor();
			}
			base.OnLostMouseCapture(e);
		}
		
		protected override void OnQueryCursor(QueryCursorEventArgs e)
		{
			base.OnQueryCursor(e);
			if (!e.Handled && (pan || isMouseDown)) {
				e.Handled = true;
				e.Cursor = isMouseDown ? PanToolCursorMouseDown : PanToolCursor;
			}
		}
		
		void PanStart()
		{
			startHorizontalOffset = this.HorizontalOffset;
			startVericalOffset = this.VerticalOffset;
		}

		void PanContinue(Vector delta)
		{
			this.ScrollToHorizontalOffset(startHorizontalOffset - delta.X / this.CurrentZoom);
			this.ScrollToVerticalOffset(startVericalOffset - delta.Y / this.CurrentZoom);
		}
	}
}
