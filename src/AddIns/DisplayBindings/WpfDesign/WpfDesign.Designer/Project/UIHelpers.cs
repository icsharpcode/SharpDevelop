// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.Designer
{
	static class UIHelpers
	{
		public static DependencyObject GetParentObject(this DependencyObject child)
		{
			if (child == null) return null;

			var contentElement = child as ContentElement;
			if (contentElement != null)
			{
				DependencyObject parent = ContentOperations.GetParent(contentElement);
				if (parent != null) return parent;

				var fce = contentElement as FrameworkContentElement;
				return fce != null ? fce.Parent : null;
			}

			var frameworkElement = child as FrameworkElement;
			if (frameworkElement != null)
			{
				DependencyObject parent = frameworkElement.Parent;
				if (parent != null) return parent;
			}

			return VisualTreeHelper.GetParent(child);
		}

		public static T TryFindParent<T>(this DependencyObject child) where T : DependencyObject
		{
			DependencyObject parentObject = GetParentObject(child);

			if (parentObject == null) return null;

			T parent = parentObject as T;
			if (parent != null)
			{
				return parent;
			}

			return TryFindParent<T>(parentObject);
		}

		public static T TryFindChild<T>(this DependencyObject parent) where T : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(parent, i);

				if (child is T)
				{
					return (T)child;
				}
				child = TryFindChild<T>(child);
				if (child != null)
				{
					return (T)child;
				}
			}
			return null;
		}

		public static T TryFindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
		{
			if (parent == null) return null;
			T foundChild = null;
			var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
			for (var i = 0; i < childrenCount; i++)
			{
				var child = VisualTreeHelper.GetChild(parent, i);

				var childType = child as T;
				if (childType == null)
				{
					foundChild = TryFindChild<T>(child, childName);
					if (foundChild != null) break;
				}
				else if (!string.IsNullOrEmpty(childName))
				{
					var frameworkElement = child as FrameworkElement;
					if (frameworkElement != null && frameworkElement.Name == childName)
					{
						foundChild = (T)child;
						break;
					}
				}
				else
				{
					foundChild = (T)child;
					break;
				}
			}
			return foundChild;
		}
	}
}
