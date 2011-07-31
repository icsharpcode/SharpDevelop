// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
