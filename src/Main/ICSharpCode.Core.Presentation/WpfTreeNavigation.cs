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
using System.Windows.Media;

namespace ICSharpCode.Core.Presentation
{
	public static class WpfTreeNavigation
	{
		/// <summary>
		/// Returns the first occurence of object of type <paramref name="T" /> in the visual tree of <paramref name="dependencyObject" />.
		/// <param name="root">Start node.</param>
		/// </summary>
		public static T TryFindChild<T>(DependencyObject root) where T : DependencyObject
		{
			if (root is T)
				return (T)root;
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
			{
				var foundChild = TryFindChild<T>(VisualTreeHelper.GetChild(root, i));
				if (foundChild != null)
					return foundChild;
			}
			return null;
		}
		
		/// <summary>
		/// Returns the first occurence of object of type <paramref name="T" /> in the visual tree of <paramref name="dependencyObject" />.
		/// </summary>
		/// <param name="child">Start node</param>
		/// <returns></returns>
		public static T TryFindParent<T>(DependencyObject child, bool includeItSelf = true) where T : DependencyObject
		{
			if (includeItSelf && child is T) return child as T;
			
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
	}
}
