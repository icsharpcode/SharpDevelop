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

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Helper Class for the Design Time Properties used by VS and Blend
	/// </summary>
	public static class DesignTimeProperties
	{
		#region IsHidden
		
		/// <summary>
		/// Getter for <see cref="IsHiddenProperty"/>
		/// </summary>
		public static bool GetIsHidden(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsHiddenProperty);
		}

		/// <summary>
		/// Setter for <see cref="IsHiddenProperty"/>
		/// </summary>
		public static void SetIsHidden(DependencyObject obj, bool value)
		{
			obj.SetValue(IsHiddenProperty, value);
		}
		
		/// <summary>
		/// Design-time IsHidden property
		/// </summary>
		public static readonly DependencyProperty IsHiddenProperty =
			DependencyProperty.RegisterAttached("IsHidden", typeof(bool), typeof(DesignTimeProperties), new PropertyMetadata(new PropertyChangedCallback(OnIsHiddenPropertyChanged)));

		static void OnIsHiddenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var dpd = DependencyPropertyDescriptor.FromProperty(UIElement.VisibilityProperty, d.GetType());
			
			if ((bool)e.NewValue) {
				EnsureHidden(d);
				dpd.AddValueChanged(d, OnVisibilityPropertyChanged);
			} else {
				dpd.RemoveValueChanged(d, OnVisibilityPropertyChanged);
				d.InvalidateProperty(UIElement.VisibilityProperty);
			}
		}

		static void OnVisibilityPropertyChanged(object sender, EventArgs e)
		{
			var d = sender as DependencyObject;
			if (d != null && GetIsHidden(d)) {
				EnsureHidden(d);
			}
		}
		
		static void EnsureHidden(DependencyObject d)
		{
			if (Visibility.Visible.Equals(d.GetValue(UIElement.VisibilityProperty))) {
				d.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Hidden);
			}
		}
		
		#endregion

		#region IsLocked

		/// <summary>
		/// Getter for <see cref="IsLockedProperty"/>
		/// </summary>
		public static bool GetIsLocked(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsLockedProperty);
		}

		/// <summary>
		/// Setter for <see cref="IsLockedProperty"/>
		/// </summary>
		public static void SetIsLocked(DependencyObject obj, bool value)
		{
			obj.SetValue(IsLockedProperty, value);
		}

		/// <summary>
		/// Design-time IsLocked property.
		/// </summary>
		public static readonly DependencyProperty IsLockedProperty =
			DependencyProperty.RegisterAttached("IsLocked", typeof(bool), typeof(DesignTimeProperties));

		#endregion

		#region DataContext
		/// <summary>
		/// Getter for <see cref="DataContextProperty"/>
		/// </summary>
		public static object GetDataContext(DependencyObject obj)
		{
			return (object)obj.GetValue(DataContextProperty);
		}

		/// <summary>
		/// Setter for <see cref="DataContextProperty"/>
		/// </summary>
		public static void SetDataContext(DependencyObject obj, bool value)
		{
			obj.SetValue(DataContextProperty, value);
		}

		/// <summary>
		/// Design-time data context
		/// </summary>
		public static readonly DependencyProperty DataContextProperty =
			DependencyProperty.RegisterAttached("DataContext", typeof(object), typeof(DesignTimeProperties));

		#endregion

		#region DesignSource
		/// <summary>
		/// Getter for <see cref="DesignSourceProperty"/>
		/// </summary>
		public static object GetDesignSource(DependencyObject obj)
		{
			return (object)obj.GetValue(DesignSourceProperty);
		}
		
		/// <summary>
		/// Setter for <see cref="DesignSourceProperty"/>
		/// </summary>
		public static void SetDesignSource(DependencyObject obj, bool value)
		{
			obj.SetValue(DesignSourceProperty, value);
		}

		/// <summary>
		/// Design-time design source
		/// </summary>
		public static readonly DependencyProperty DesignSourceProperty =
			DependencyProperty.RegisterAttached("DesignSource", typeof(object), typeof(DesignTimeProperties));

		#endregion

		#region DesignWidth
		/// <summary>
		/// Getter for <see cref="DesignWidthProperty"/>
		/// </summary>
		public static double GetDesignWidth(DependencyObject obj)
		{
			return (double)obj.GetValue(DesignWidthProperty);
		}

		/// <summary>
		/// Setter for <see cref="DesignWidthProperty"/>
		/// </summary>
		public static void SetDesignWidth(DependencyObject obj, double value)
		{
			obj.SetValue(DesignWidthProperty, value);
		}

		/// <summary>
		/// Design-time width
		/// </summary>
		public static readonly DependencyProperty DesignWidthProperty =
			DependencyProperty.RegisterAttached("DesignWidth", typeof(double), typeof(DesignTimeProperties));
		#endregion

		#region DesignHeight
		/// <summary>
		/// Getter for <see cref="DesignHeightProperty"/>
		/// </summary>
		public static double GetDesignHeight(DependencyObject obj)
		{
			return (double)obj.GetValue(DesignHeightProperty);
		}

		/// <summary>
		/// Setter for <see cref="DesignHeightProperty"/>
		/// </summary>
		public static void SetDesignHeight(DependencyObject obj, double value)
		{
			obj.SetValue(DesignHeightProperty, value);
		}

		/// <summary>
		/// Design-time height
		/// </summary>
		public static readonly DependencyProperty DesignHeightProperty =
			DependencyProperty.RegisterAttached("DesignHeight", typeof(double), typeof(DesignTimeProperties));
		#endregion

		#region LayoutOverrides
		/// <summary>
		/// Getter for <see cref="LayoutOverridesProperty"/>
		/// </summary>
		public static string GetLayoutOverrides(DependencyObject obj)
		{
			return (string)obj.GetValue(LayoutOverridesProperty);
		}

		/// <summary>
		/// Setter for <see cref="LayoutOverridesProperty"/>
		/// </summary>
		public static void SetLayoutOverrides(DependencyObject obj, string value)
		{
			obj.SetValue(LayoutOverridesProperty, value);
		}

		/// <summary>
		/// Layout-Overrides
		/// </summary>
		public static readonly DependencyProperty LayoutOverridesProperty =
			DependencyProperty.RegisterAttached("LayoutOverrides", typeof(string), typeof(DesignTimeProperties));
		#endregion

		#region LayoutRounding
		/// <summary>
		/// Getter for <see cref="LayoutRoundingProperty"/>
		/// </summary>
		public static bool GetLayoutRounding(DependencyObject obj)
		{
			return (bool)obj.GetValue(LayoutRoundingProperty);
		}

		/// <summary>
		/// Setter for <see cref="LayoutRoundingProperty"/>
		/// </summary>
		public static void SetLayoutRounding(DependencyObject obj, bool value)
		{
			obj.SetValue(LayoutRoundingProperty, value);
		}

		/// <summary>
		/// Design-time layout rounding
		/// </summary>
		public static readonly DependencyProperty LayoutRoundingProperty =
			DependencyProperty.RegisterAttached("LayoutRounding", typeof(bool), typeof(DesignTimeProperties));
		#endregion
	}
}
