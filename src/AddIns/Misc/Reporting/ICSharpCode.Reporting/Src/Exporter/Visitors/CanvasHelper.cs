// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Windows;
using System.Windows.Controls;


namespace ICSharpCode.Reporting.Exporter.Visitors
{
	/// <summary>
	/// Description of CanvasHelper.
	/// http://denisvuyka.wordpress.com/2007/12/15/wpf-simplify-your-life-with-linq-extension-methods-canvas-and-visual-tree-helpers/
	/// </summary>
	/// 
	internal static class CanvasHelper
	{
		public static double GetLeft<T>(T element)
		{
			UIElement uiElement = element as UIElement;
			if (uiElement == null)
				throw new ArgumentNullException("element");
			return (double)uiElement.GetValue(Canvas.LeftProperty);
		}

		public static double GetTop<T>(T element)
		{
			UIElement uiElement = element as UIElement;
			if (uiElement == null)
				throw new ArgumentNullException("element");
			return (double)uiElement.GetValue(Canvas.TopProperty);
		}

		public static Point GetPosition<T>(T element)
		{
			UIElement uiElement = element as UIElement;
			if (uiElement == null)
				throw new ArgumentNullException("element");

			return new Point(
				(double)uiElement.GetValue(Canvas.LeftProperty),
				(double)uiElement.GetValue(Canvas.TopProperty));
		}

		public static void SetLeft<T>(T element, double length)
		{
			UIElement uiElement = element as UIElement;
			if (uiElement == null)
				throw new ArgumentNullException("element");
			uiElement.SetValue(Canvas.LeftProperty, length);
		}

		public static void SetTop<T>(T element, double length)
		{
			UIElement uiElement = element as UIElement;
			if (uiElement == null)
				throw new ArgumentNullException("element");
			uiElement.SetValue(Canvas.TopProperty, length);
		}

		public static void SetPosition<T>(T element, Point value)
		{
			UIElement uiElement = element as UIElement;
			if (uiElement == null)
				throw new ArgumentNullException("element");
			uiElement.SetValue(Canvas.LeftProperty, value.X);
			uiElement.SetValue(Canvas.TopProperty, value.Y);
		}
	}
}
