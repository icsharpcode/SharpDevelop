// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Windows;
using System.Windows.Controls;

namespace Debugger.AddIn.Visualizers.Graph.Drawing
{
	/// <summary>
	/// Provides LocationProperty, which enables animating Canvas.LeftProperty and Canvas.TopProperty
	/// at the same time using PointAnimation.
	/// </summary>
	public class CanvasLocationAdapter : UIElement
	{
		/// <summary>
		/// Add this dependency property to UIElement object and animate it using PointAnimation.
		/// </summary>
		public static DependencyProperty LocationProperty = DependencyProperty.RegisterAttached("Location", typeof(Point), typeof(Canvas), new FrameworkPropertyMetadata(new Point(0, 0), new PropertyChangedCallback(locationChanged)), new ValidateValueCallback(IsPointValid));

		private static void locationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Canvas.SetLeft((UIElement)d, ((Point)e.NewValue).X);
			Canvas.SetTop((UIElement)d, ((Point)e.NewValue).Y);
		}

		private static bool IsPointValid(object o)
		{
			return true;
		}
	}
}
