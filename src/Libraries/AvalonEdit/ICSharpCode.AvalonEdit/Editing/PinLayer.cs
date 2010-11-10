// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Editing
{
	/// <summary>
	/// Pin layer class. This class handles the pinning and unpinning operations.
	/// </summary>
	public class PinLayer : Layer
	{
		double initialHorizontalOffset;
		double initialVerticalOffset;
		int currentIndex = 0;
		
		private Canvas pinningSurface;
		
		/// <summary>
		/// PinLayer constructor.
		/// </summary>
		/// <param name="textArea">Text area for this layer.</param>
		public PinLayer(TextArea textArea) : base(textArea.TextView, KnownLayer.Pins)
		{
			pinningSurface = new Canvas();
			this.Children.Add(pinningSurface);
			textView.VisualLinesChanged += new EventHandler(textView_VisualLinesChanged);	
			
			initialHorizontalOffset = textView.HorizontalOffset;
			initialVerticalOffset = textView.VerticalOffset;
			
			pinningSurface.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(pinningSurface_PreviewMouseLeftButtonDown);
			pinningSurface.PreviewMouseMove += new MouseEventHandler(pinningSurface_PreviewMouseMove);
		}

		void textView_VisualLinesChanged(object sender, EventArgs e)
		{
			foreach (var ctrl in this.pinningSurface.Children) {
				var popup = ctrl as FrameworkElement;
				if (popup == null) continue;
				
				Point location = (Point)popup.Tag;
				if (location == null) continue;
				
				location.X += textView.HorizontalOffset - initialHorizontalOffset;
				location.Y += textView.VerticalOffset - initialVerticalOffset;
				
				Canvas.SetLeft(popup, location.X);
				Canvas.SetTop(popup, location.Y);
				
				initialHorizontalOffset = textView.HorizontalOffset;
				initialVerticalOffset = textView.VerticalOffset;
			}
		}
		
		/// <summary>
		/// Pins an element;
		/// </summary>
		/// <param name="element">Element to pin.</param>
		public void Pin(FrameworkElement element)
		{
			if (element == null)
				throw new NullReferenceException("Element is null!");
			
			Point location = (Point)element.Tag;
			
			Canvas.SetTop(element, location.Y);
			Canvas.SetLeft(element, location.X);
			Canvas.SetZIndex(element, currentIndex++);
			pinningSurface.Children.Add(element);
		}
		
		/// <summary>
		/// Unpins an element.
		/// </summary>
		/// <param name="element">Element to unpin.</param>
		public void Unpin(FrameworkElement element)
		{
			if (element == null)
				throw new NullReferenceException("Element is null!");

			pinningSurface.Children.Remove(element);
		}
		
		#region Mouse move
		
		Point dragStartedPoint;

		void pinningSurface_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			var element = e.OriginalSource as FrameworkElement;
			
			if (TryFindButton(element) == null)
			{
				if (e.LeftButton == MouseButtonState.Pressed) {
					element = TryFindTag(element);
					
					var point = e.GetPosition(element);
					Point location = (Point)element.Tag;
					location.X += point.X - dragStartedPoint.X;
					location.Y += point.Y - dragStartedPoint.Y;
					Canvas.SetTop(element, location.Y);
					Canvas.SetLeft(element, location.X);
					element.Tag = location;
					dragStartedPoint = point;
				}
				
				e.Handled = true;
			}
		}

		void pinningSurface_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var element = e.OriginalSource as FrameworkElement;		
			
			if (TryFindButton(element) == null)
			{
				dragStartedPoint = e.GetPosition(element);
				e.Handled = true;
			}
		}
		
		#endregion
		
		public static FrameworkElement TryFindTag(DependencyObject child)
		{
			DependencyObject parentObject = GetParentObject(child);
			if (parentObject == null) return null;
			
			var parent = parentObject as FrameworkElement;
			if (parent != null && parent.Tag != null)
			{
				return parent;
			}
			else
			{
				return TryFindTag(parentObject);
			}
		}
		
		public static FrameworkElement TryFindButton(DependencyObject child)
		{
			DependencyObject parentObject = GetParentObject(child);
			if (parentObject == null) return null;
			
			var parent = parentObject as FrameworkElement;
			if (parent != null && parent is ToggleButton)
			{
				return parent;
			}
			else
			{
				return TryFindButton(parentObject);
			}
		}
		
		public static DependencyObject GetParentObject(DependencyObject child)
		{
			if (child == null) return null;
			
			ContentElement contentElement = child as ContentElement;
			if (contentElement != null)
			{
				DependencyObject parent = ContentOperations.GetParent(contentElement);
				if (parent != null) return parent;
				
				FrameworkContentElement fce = contentElement as FrameworkContentElement;
				return fce != null ? fce.Parent : null;
			}
			
			FrameworkElement frameworkElement = child as FrameworkElement;
			if (frameworkElement != null)
			{
				DependencyObject parent = frameworkElement.Parent;
				if (parent != null) return parent;
			}
			
			return VisualTreeHelper.GetParent(child);
		}
	}
}