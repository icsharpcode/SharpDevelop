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
		
		public static void AddChild<T>(this Panel panel, T element)
		{
			var uiElement = element as UIElement;
			if (uiElement != null && !panel.Children.Contains(uiElement))
				panel.Children.Add(uiElement);
		}

		
		public static void RemoveChild<T>(this Panel panel, T element)
		{
			var uiElement = element as UIElement;
			if (uiElement != null && panel.Children.Contains(uiElement))
				panel.Children.Remove(uiElement);
		}

		
		public static void InsertChild<T>(this Panel panel, int index, T element)
		{
			var uiElement = element as UIElement;
			if (uiElement != null && !panel.Children.Contains(uiElement))
				panel.Children.Insert(index, uiElement);
		}
	}

}
