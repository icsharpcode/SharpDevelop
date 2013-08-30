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
		
		public static bool GetIsHidden(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsHiddenProperty);
		}

		public static void SetIsHidden(DependencyObject obj, bool value)
		{
			obj.SetValue(IsHiddenProperty, value);
		}
		
		public static readonly DependencyProperty IsHiddenProperty =
			DependencyProperty.RegisterAttached("IsHidden", typeof(bool), typeof(DesignTimeProperties));

		#endregion

		#region IsLocked

		public static bool GetIsLocked(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsLockedProperty);
		}

		public static void SetIsLocked(DependencyObject obj, bool value)
		{
			obj.SetValue(IsLockedProperty, value);
		}

		public static readonly DependencyProperty IsLockedProperty =
			DependencyProperty.RegisterAttached("IsLocked", typeof(bool), typeof(DesignTimeProperties));

		#endregion

		#region DataContext

		public static object GetDataContext(DependencyObject obj)
		{
			return (object)obj.GetValue(DataContextProperty);
		}

		public static void SetDataContext(DependencyObject obj, bool value)
		{
			obj.SetValue(DataContextProperty, value);
		}

		public static readonly DependencyProperty DataContextProperty =
			DependencyProperty.RegisterAttached("DataContext", typeof(object), typeof(DesignTimeProperties));

		#endregion

		#region DesignSource

		public static object GetDesignSource(DependencyObject obj)
		{
			return (object)obj.GetValue(DesignSourceProperty);
		}

		public static void SetDesignSource(DependencyObject obj, bool value)
		{
			obj.SetValue(DesignSourceProperty, value);
		}

		public static readonly DependencyProperty DesignSourceProperty =
			DependencyProperty.RegisterAttached("DesignSource", typeof(object), typeof(DesignTimeProperties));

		#endregion

		#region DesignWidth
		
		public static double GetDesignWidth(DependencyObject obj)
		{
			return (double)obj.GetValue(DesignWidthProperty);
		}

		public static void SetDesignWidth(DependencyObject obj, double value)
		{
			obj.SetValue(DesignWidthProperty, value);
		}

		public static readonly DependencyProperty DesignWidthProperty =
			DependencyProperty.RegisterAttached("DesignWidth", typeof(double), typeof(DesignTimeProperties));

		#endregion

		#region DesignHeight

		public static double GetDesignHeight(DependencyObject obj)
		{
			return (double)obj.GetValue(DesignHeightProperty);
		}

		public static void SetDesignHeight(DependencyObject obj, double value)
		{
			obj.SetValue(DesignHeightProperty, value);
		}

		public static readonly DependencyProperty DesignHeightProperty =
			DependencyProperty.RegisterAttached("DesignHeight", typeof(double), typeof(DesignTimeProperties));
		
		#endregion
	}
}
