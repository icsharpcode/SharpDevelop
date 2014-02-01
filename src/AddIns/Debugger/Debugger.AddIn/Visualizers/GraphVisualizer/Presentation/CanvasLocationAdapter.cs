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
