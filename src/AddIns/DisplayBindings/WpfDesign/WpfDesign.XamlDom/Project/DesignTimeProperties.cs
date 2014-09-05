// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.Security.Cryptography;
using System.Windows;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Helper Class for the Design Time Properties used by VS and Blend
	/// </summary>
	public class DesignTimeProperties : FrameworkElement
	{
		#region IsHidden
		
		/// <summary>
		/// Getter for the <see cref="IsHiddenProperty"/>
		/// </summary>
		public static bool GetIsHidden(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsHiddenProperty);
		}

		/// <summary>
		/// Setter for the <see cref="IsHiddenProperty"/>
		/// </summary>
		public static void SetIsHidden(DependencyObject obj, bool value)
		{
			obj.SetValue(IsHiddenProperty, value);
		}
		
		/// <summary>
		/// Design-time IsHidden property
		/// </summary>
		public static readonly DependencyProperty IsHiddenProperty =
			DependencyProperty.RegisterAttached("IsHidden", typeof(bool), typeof(DesignTimeProperties));

		#endregion

		#region IsLocked

		/// <summary>
		/// Getter for the <see cref="IsLockedProperty"/>
		/// </summary>
		public static bool GetIsLocked(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsLockedProperty);
		}

		/// <summary>
		/// Setter for the <see cref="IsLockedProperty"/>
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
		/// Getter for the <see cref="DataContextProperty"/>
		/// </summary>
		public static object GetDataContext(DependencyObject obj)
		{
			return (object)obj.GetValue(DataContextProperty);
		}

		/// <summary>
		/// Setter for the <see cref="DataContextProperty"/>
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
		/// Getter for the <see cref="DesignSourceProperty"/>
		/// </summary>
		public static object GetDesignSource(DependencyObject obj)
		{
			return (object)obj.GetValue(DesignSourceProperty);
		}
		
		/// <summary>
		/// Setter for the <see cref="DesignSourceProperty"/>
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
		/// Getter for the <see cref="DesignWidthProperty"/>
		/// </summary>
		public static double GetDesignWidth(DependencyObject obj)
		{
			return (double)obj.GetValue(DesignWidthProperty);
		}

		/// <summary>
		/// Setter for the <see cref="DesignWidthProperty"/>
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
		/// Getter for the <see cref="DesignHeightProperty"/>
		/// </summary>
		public static double GetDesignHeight(DependencyObject obj)
		{
			return (double)obj.GetValue(DesignHeightProperty);
		}

		/// <summary>
		/// Setter for the <see cref="DesignHeightProperty"/>
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
<<<<<<< HEAD
		
		#region LayoutRounding
		public static bool GetLayoutRounding(DependencyObject obj)
		{
			return (bool)obj.GetValue(DesignLayoutRounding);
		}

		public static void SetLayoutRounding(DependencyObject obj, bool value)
		{
			obj.SetValue(DesignLayoutRounding, value);
		}

		public static readonly DependencyProperty DesignLayoutRounding =
			DependencyProperty.RegisterAttached("LayoutRounding", typeof(bool), typeof(DesignTimeProperties));
		
		#endregion
=======
>>>>>>> parent of 77bb8e4... Layout Rounding Design Time Property
	}
}
