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
using System.Windows;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	sealed class DefaultErrorService : IErrorService
	{
		sealed class AttachedErrorBalloon : ErrorBalloon
		{
			FrameworkElement attachTo;
			
			public AttachedErrorBalloon(FrameworkElement attachTo, UIElement errorElement)
			{
				this.attachTo = attachTo;
				this.Content = errorElement;
			}
			
			internal void AttachEvents()
			{
				attachTo.Unloaded += OnCloseEvent;
				attachTo.PreviewKeyDown += OnCloseEvent;
				attachTo.PreviewMouseDown += OnCloseEvent;
				attachTo.LostFocus += OnCloseEvent;
			}
			
			void OnCloseEvent(object sender, EventArgs e)
			{
				this.Close();
			}
			
			protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
			{
				attachTo.Unloaded -= OnCloseEvent;
				attachTo.PreviewKeyDown -= OnCloseEvent;
				attachTo.PreviewMouseDown -= OnCloseEvent;
				attachTo.LostFocus -= OnCloseEvent;
				base.OnClosing(e);
			}
			
			protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
			{
				base.OnMouseDown(e);
				Close();
			}
		}
		
		ServiceContainer services;
		
		public DefaultErrorService(DesignContext context)
		{
			this.services = context.Services;
		}
		
		public void ShowErrorTooltip(FrameworkElement attachTo, UIElement errorElement)
		{
			if (attachTo == null)
				throw new ArgumentNullException("attachTo");
			if (errorElement == null)
				throw new ArgumentNullException("errorElement");
			
			AttachedErrorBalloon b = new AttachedErrorBalloon(attachTo, errorElement);
			Point pos = attachTo.PointToScreen(new Point(0, attachTo.ActualHeight));
			b.Left = pos.X;
			b.Top = pos.Y - 8;
			b.Focusable = false;
			ITopLevelWindowService windowService = services.GetService<ITopLevelWindowService>();
			ITopLevelWindow ownerWindow = (windowService != null) ? windowService.GetTopLevelWindow(attachTo) : null;
			if (ownerWindow != null) {
				ownerWindow.SetOwner(b);
			}
			b.Show();
			
			if (ownerWindow != null) {
				ownerWindow.Activate();
			}
			
			b.AttachEvents();
		}
	}
}
