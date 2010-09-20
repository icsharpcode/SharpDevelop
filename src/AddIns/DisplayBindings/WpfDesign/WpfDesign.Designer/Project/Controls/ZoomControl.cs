// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		}
		
		static Cursor GetCursor(string path)
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
