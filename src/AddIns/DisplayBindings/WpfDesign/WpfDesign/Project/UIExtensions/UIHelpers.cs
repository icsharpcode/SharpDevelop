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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.UIExtensions
{
	/// <summary>
	/// Contains helper methods for UI. 
	/// </summary>
	public static class UIHelpers
	{
		/// <summary>
		/// Gets the parent. Which tree the parent is retrieved from depends on the parameters.
		/// </summary>
		/// <param name="child">The child to get parent for.</param>
		/// <param name="searchCompleteVisualTree">If true the parent in the visual tree is returned, if false the parent may be retrieved from another tree depending on the child type.</param>
		/// <returns>The parent element, and depending on the parameters its retrieved from either visual tree, logical tree or a tree not strictly speaking either the logical tree or the visual tree.</returns>
		public static DependencyObject GetParentObject(this DependencyObject child, bool searchCompleteVisualTree)
		{
			if (child == null) return null;

			if (!searchCompleteVisualTree) {
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
			}

			return VisualTreeHelper.GetParent(child);
		}

		/// <summary>
		/// Gets first parent element of the specified type. Which tree the parent is retrieved from depends on the parameters.
		/// </summary>
		/// <param name="child">The child to get parent for.</param>
		/// <param name="searchCompleteVisualTree">If true the parent in the visual tree is returned, if false the parent may be retrieved from another tree depending on the child type.</param>
		/// <returns>
		/// The first parent element of the specified type, and depending on the parameters its retrieved from either visual tree, logical tree or a tree not strictly speaking either the logical tree or the visual tree.
		/// null is returned if no parent of the specified type is found.
		/// </returns>
		public static T TryFindParent<T>(this DependencyObject child, bool searchCompleteVisualTree = false) where T : DependencyObject
		{
			DependencyObject parentObject = GetParentObject(child, searchCompleteVisualTree);

			if (parentObject == null) return null;

			T parent = parentObject as T;
			if (parent != null)
			{
				return parent;
			}

			return TryFindParent<T>(parentObject);
		}

		/// <summary>
		/// Returns the first child of the specified type found in the visual tree.
		/// </summary>
		/// <param name="parent">The parent element where the search is started.</param>
		/// <returns>The first child of the specified type found in the visual tree, or null if no parent of the specified type is found.</returns>
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

		/// <summary>
		/// Returns the first child of the specified type and with the specified name found in the visual tree.
		/// </summary>
		/// <param name="parent">The parent element where the search is started.</param>
		/// <param name="childName">The name of the child element to find, or an empty string or null to only look at the type.</param>
		/// <returns>The first child that matches the specified type and child name, or null if no match is found.</returns>
		public static T TryFindChild<T>(this DependencyObject parent, string childName) where T : DependencyObject
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
