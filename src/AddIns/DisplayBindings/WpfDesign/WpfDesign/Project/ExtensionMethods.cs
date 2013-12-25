// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
