// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Widgets.TreeGrid
{
	public class VerticalScrollContainer : Control
	{
		public interface IScrollable
		{
			int ScrollOffsetY { get; set; }
			int ScrollHeightY { get; }
			int Height { get; }
			event MouseEventHandler MouseWheel;
		}
		
		IScrollable scrollable;
		bool showButtonsOnlyIfRequired = true;
		ScrollButtonControl down = new ScrollButtonControl();
		ScrollButtonControl up   = new ScrollButtonControl();
		Timer timer = new Timer();
		
		int scrollSpeed = 5;
		
		public int ScrollSpeed {
			get {
				return scrollSpeed;
			}
			set {
				scrollSpeed = value;
			}
		}
		
		public VerticalScrollContainer()
		{
			up.Arrow   = ScrollButton.Up;
			down.Arrow = ScrollButton.Down;
			up.Dock    = DockStyle.Top;
			down.Dock  = DockStyle.Bottom;
			this.TabStop = false;
			this.SetStyle(ControlStyles.Selectable, false);
			Controls.Add(up);
			Controls.Add(down);
			UpdateEnabled();
			
			timer.Interval = 50;
			timer.Tick += delegate {
				ScrollBy((int)timer.Tag);
			};
			up.MouseDown += delegate {
				timer.Tag = -scrollSpeed;
				ScrollBy(-scrollSpeed);
				timer.Start();
			};
			down.MouseDown += delegate {
				timer.Tag = scrollSpeed;
				ScrollBy(scrollSpeed);
				timer.Start();
			};
			up.MouseUp   += StopTimer;
			down.MouseUp += StopTimer;
		}
		
		void StopTimer(object sender, MouseEventArgs e)
		{
			timer.Stop();
		}
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing) {
				timer.Dispose();
			}
		}
		
		void UpdateEnabled()
		{
			if (scrollable == null) {
				up.Visible = down.Visible = true;
				up.Enabled = down.Enabled = false;
			} else {
				int scrollHeightY = scrollable.ScrollHeightY;
				if (showButtonsOnlyIfRequired) {
					if (scrollHeightY <= this.Height) {
						scrollable.ScrollOffsetY = 0;
						up.Visible = down.Visible = false;
						return;
					}
					up.Visible = down.Visible = true;
				} else {
					up.Visible = down.Visible = true;
					if (scrollable.ScrollHeightY <= scrollable.Height) {
						scrollable.ScrollOffsetY = 0;
						up.Enabled = down.Enabled = false;
						return;
					}
				}
				// set enabled
				up.Enabled = scrollable.ScrollOffsetY > 0;
				down.Enabled = scrollable.ScrollOffsetY < scrollHeightY - scrollable.Height;
			}
		}
		
		void ScrollBy(int amount)
		{
			if (amount != 0) {
				scrollable.ScrollOffsetY = Math.Max(0, Math.Min(scrollable.ScrollOffsetY + amount, scrollable.ScrollHeightY - scrollable.Height));
				UpdateEnabled();
			}
		}
		
		public bool ShowButtonsOnlyIfRequired {
			get {
				return showButtonsOnlyIfRequired;
			}
			set {
				if (showButtonsOnlyIfRequired != value) {
					showButtonsOnlyIfRequired = value;
					UpdateEnabled();
				}
			}
		}
		
		MouseWheelHandler mouseWheelHandler = new MouseWheelHandler();
		
		void ScrollableWheel(object sender, MouseEventArgs e)
		{
			ScrollBy(mouseWheelHandler.GetScrollAmount(e) * -16);
		}
		
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			UpdateEnabled();
		}
		
		protected override void OnControlAdded(ControlEventArgs e)
		{
			base.OnControlAdded(e);
			if (scrollable == null && !DesignMode) {
				scrollable = e.Control as IScrollable;
				if (scrollable != null) {
					scrollable.MouseWheel += ScrollableWheel;
					Controls.SetChildIndex(e.Control, 0);
					UpdateEnabled();
				}
			}
		}
		
		protected override void OnControlRemoved(ControlEventArgs e)
		{
			base.OnControlRemoved(e);
			if (scrollable == e.Control) {
				scrollable.MouseWheel -= ScrollableWheel;
				scrollable = null;
				UpdateEnabled();
			}
		}
		
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			// HACK: Windows.Forms bug workaround
			BeginInvoke(new MethodInvoker(PerformLayout));
		}
	}
}
