// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop.Refactoring;
using Services.Debugger.Tooltips;

namespace Editor.AvalonEdit
{
	/// <summary>
	/// Pin layer class. This class handles the pinning and unpinning operations.
	/// </summary>
	public class PinLayer : Layer
	{
		private Canvas pinningSurface;
		
		private double verticalOffset = 0;
		
		/// <summary>
		/// PinLayer constructor.
		/// </summary>
		/// <param name="textArea">Text area for this layer.</param>
		public PinLayer(TextArea textArea) : base(textArea.TextView, KnownLayer.DataPins)
		{
			pinningSurface = new Canvas();
			this.Children.Add(pinningSurface);
			textView.VisualLinesChanged += new EventHandler(textView_VisualLinesChanged);
		}
		
		/// <summary>
		/// Pins an element;
		/// </summary>
		/// <param name="element">Element to pin.</param>
		public void Pin(PinDebuggerControl element)
		{
			if (element == null)
				throw new NullReferenceException("Element is null!");
			
			Thumb currentThumb = new Thumb();
			Canvas.SetTop(currentThumb, element.Location.Y);
			Canvas.SetLeft(currentThumb, element.Location.X);
			currentThumb.Style = element.TryFindResource("PinThumbStyle") as Style;
			currentThumb.ApplyTemplate();
			currentThumb.DragDelta += onDragDelta;
			currentThumb.DragStarted += currentThumb_DragStarted;
			currentThumb.DragCompleted += currentThumb_DragCompleted;
			
			var container = TryFindChild<StackPanel>(currentThumb);
			container.Children.Add(element);
			pinningSurface.Children.Add(currentThumb);
		}
		
		/// <summary>
		/// Unpins an element.
		/// </summary>
		/// <param name="element">Element to unpin.</param>
		public void Unpin(PinDebuggerControl element)
		{
			if (element == null)
				throw new NullReferenceException("Element is null!");
			
			foreach (var thumb in this.pinningSurface.Children) {
				PinDebuggerControl pinControl = TryFindChild<PinDebuggerControl>((DependencyObject)thumb);
				if (pinControl != null && pinControl == element)
				{
					pinningSurface.Children.Remove((UIElement)thumb);
					break;
				}
			}
		}
		
		void textView_VisualLinesChanged(object sender, EventArgs e)
		{
			foreach (var ctrl in this.pinningSurface.Children) {
				var currentThumb = ctrl as Thumb;
				PinDebuggerControl pinControl = TryFindChild<PinDebuggerControl>(currentThumb);
				if (pinControl != null)
				{
					Point location = pinControl.Location;
					location.X -= textView.HorizontalOffset;
					location.Y += verticalOffset - textView.VerticalOffset;
					
					Canvas.SetLeft(currentThumb, location.X);
					Canvas.SetTop(currentThumb, location.Y);
					
					pinControl.Location = location;
					pinControl.Mark.PopupPosition = location;
				}
			}
			
			verticalOffset = textView.VerticalOffset;
		}
		
		#region Mouse move

		void onDragDelta(object sender, DragDeltaEventArgs e)
		{
			Thumb currnetThumb = (Thumb)sender;
			currnetThumb.Cursor = Cursors.Arrow;
			double left = Canvas.GetLeft(currnetThumb) + e.HorizontalChange;
			double top = Canvas.GetTop(currnetThumb) + e.VerticalChange;
			
			Canvas.SetLeft(currnetThumb, left);
			Canvas.SetTop(currnetThumb, top);
			
			var pinControl = TryFindChild<PinDebuggerControl>(currnetThumb);
			if (pinControl != null) {
				pinControl.Location = new Point { X = left, Y = top };
				pinControl.Mark.PopupPosition = pinControl.Location;
				pinControl.Opacity = 1d;
			}
		}
		
		void currentThumb_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			Thumb currnetThumb = (Thumb)sender;
			currnetThumb.Cursor = Cursors.Arrow;
			
			var pinControl = TryFindChild<PinDebuggerControl>(currnetThumb);
			if (pinControl != null)
				pinControl.Opacity = 1d;
		}

		void currentThumb_DragStarted(object sender, DragStartedEventArgs e)
		{
			Thumb currnetThumb = (Thumb)sender;
			currnetThumb.Cursor = Cursors.Arrow;
			
			var pinControl = TryFindChild<PinDebuggerControl>(currnetThumb);
			if (pinControl != null)
				pinControl.Opacity = 1d;
		}
		
		#endregion
		
		#region Static helpers
		
		static T TryFindChild<T>(DependencyObject root) where T : DependencyObject
		{
			return ContextActionsControl.SearchVisualTree<T>(root);
		}
		
		static T TryFindParent<T>(DependencyObject child) where T : DependencyObject
		{
			if (child is T) return child as T;
			
			DependencyObject parentObject = GetParentObject(child);
			if (parentObject == null) return null;

			var parent = parentObject as T;
			if (parent != null && parent is T)
			{
				return parent;
			}
			else
			{
				return TryFindParent<T>(parentObject);
			}
		}

		static DependencyObject GetParentObject(DependencyObject child)
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
		
		#endregion
	}
}