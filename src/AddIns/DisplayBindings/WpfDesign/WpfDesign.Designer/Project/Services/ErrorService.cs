// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
			Window ownerWindow = Window.GetWindow(attachTo);
			b.Owner = ownerWindow;
			b.Show();
			
			if (ownerWindow != null) {
				ownerWindow.Activate();
			}
			
			b.AttachEvents();
		}
	}
}
