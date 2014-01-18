// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.Reporting.WpfReportViewer.Visitor 
{
	/// <summary>
	/// Description of CanvasExtension.
	/// http://denisvuyka.wordpress.com/2007/12/15/wpf-simplify-your-life-with-linq-extension-methods-canvas-and-visual-tree-helpers/
	/// </summary>
	public static class CanvasExtension
	{
		
		public static void AddChild<T>(this Canvas canvas, T element)
		{
			var uiElement = element as UIElement;
			if (uiElement != null && !canvas.Children.Contains(uiElement))
				canvas.Children.Add(uiElement);
		}

		
		public static void RemoveChild<T>(this Canvas canvas, T element)
		{
			var uiElement = element as UIElement;
			if (uiElement != null && canvas.Children.Contains(uiElement))
				canvas.Children.Remove(uiElement);
		}

		
		public static void InsertChild<T>(this Canvas canvas, int index, T element)
		{
			var uiElement = element as UIElement;
			if (uiElement != null && !canvas.Children.Contains(uiElement))
				canvas.Children.Insert(index, uiElement);
		}
	}

}
