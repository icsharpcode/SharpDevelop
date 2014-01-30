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
