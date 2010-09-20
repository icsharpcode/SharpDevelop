// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace ICSharpCode.SharpDevelop.Gui
{
	using WindowState = System.Windows.WindowState;

	/// <summary>
	/// 
	/// </summary>
	class FullScreenEnabledWindow : Window
	{
		public static readonly DependencyProperty FullScreenProperty =
			DependencyProperty.Register("FullScreen", typeof(bool), typeof(FullScreenEnabledWindow));
		
		public bool FullScreen {
			get { return (bool)GetValue(FullScreenProperty); }
			set { SetValue(FullScreenProperty, value); }
		}
		
		System.Windows.WindowState previousWindowState = WindowState.Maximized;
		double oldLeft, oldTop, oldWidth, oldHeight;
		
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == FullScreenProperty) {
				if ((bool)e.NewValue) {
					// enable fullscreen mode
					// remember previous window state
					if (this.WindowState == WindowState.Normal || this.WindowState == WindowState.Maximized)
						previousWindowState = this.WindowState;
					oldLeft = this.Left;
					oldTop = this.Top;
					oldWidth = this.Width;
					oldHeight = this.Height;
					
					WindowInteropHelper interop = new WindowInteropHelper(this);
					interop.EnsureHandle();
					Screen screen = Screen.FromHandle(interop.Handle);
					
					Rect bounds = screen.Bounds.ToWpf().TransformFromDevice(this);
					
					this.ResizeMode = ResizeMode.NoResize;
					this.Left = bounds.Left;
					this.Top = bounds.Top;
					this.Width = bounds.Width;
					this.Height = bounds.Height;
					this.WindowState = WindowState.Normal;
					this.WindowStyle = WindowStyle.None;
					
				} else {
					ClearValue(WindowStyleProperty);
					ClearValue(ResizeModeProperty);
					ClearValue(MaxWidthProperty);
					ClearValue(MaxHeightProperty);
					this.WindowState = previousWindowState;
					
					this.Left = oldLeft;
					this.Top = oldTop;
					this.Width = oldWidth;
					this.Height = oldHeight;
				}
			}
		}
	}
}
