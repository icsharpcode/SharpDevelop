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
using System.ComponentModel;
using System.Windows;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Extension methods used in the WPF designer.
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>
		/// Rounds position and size of a Rect to PlacementInformation.BoundsPrecision digits. 
		/// </summary>
		public static Rect Round(this Rect rect)
		{
			return new Rect(
				Math.Round(rect.X, PlacementInformation.BoundsPrecision),
				Math.Round(rect.Y, PlacementInformation.BoundsPrecision),
				Math.Round(rect.Width, PlacementInformation.BoundsPrecision),
				Math.Round(rect.Height, PlacementInformation.BoundsPrecision)
			);
		}

		/// <summary>
		/// Gets the design item property for the specified member descriptor.
		/// </summary>
		public static DesignItemProperty GetProperty(this DesignItemPropertyCollection properties, MemberDescriptor md)
		{
			DesignItemProperty prop = null;

			var pd = md as PropertyDescriptor;
			if (pd != null)
			{
				var dpd = DependencyPropertyDescriptor.FromProperty(pd);
				if (dpd != null)
				{
					if (dpd.IsAttached)
					{
						prop = properties.GetAttachedProperty(dpd.DependencyProperty);
					}
					else
					{
						prop = properties.GetProperty(dpd.DependencyProperty);
					}
				}
			}

			if (prop == null)
			{
				prop = properties[md.Name];
			}

			return prop;
		}
		
		/// <summary>
		/// Gets if the specified design item property represents an attached dependency property.
		/// </summary>
		public static bool IsAttachedDependencyProperty(this DesignItemProperty property)
		{
			if (property.DependencyProperty != null)
			{
				var dpd = DependencyPropertyDescriptor.FromProperty(property.DependencyProperty, property.DesignItem.ComponentType);
				return dpd.IsAttached;
			}

			return false;
		}
	}
}
