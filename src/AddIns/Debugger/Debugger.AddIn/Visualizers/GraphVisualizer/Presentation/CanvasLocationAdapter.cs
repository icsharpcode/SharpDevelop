// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
