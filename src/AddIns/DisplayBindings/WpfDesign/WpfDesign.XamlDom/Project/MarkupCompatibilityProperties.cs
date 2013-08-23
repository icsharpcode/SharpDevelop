// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.Security.Cryptography;
using System.Windows;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Helper Class for the Markup Compatibility Properties used by VS and Blend
	/// </summary>
	public class MarkupCompatibilityProperties : FrameworkElement
	{
		#region Ignorable

		public static string GetIgnorable(DependencyObject obj)
		{
			return (string)obj.GetValue(IgnorableProperty);
		}

		public static void SetIgnorable(DependencyObject obj, string value)
		{
			obj.SetValue(IgnorableProperty, value);
		}

		public static readonly DependencyProperty IgnorableProperty =
			DependencyProperty.RegisterAttached("Ignorable", typeof(string), typeof(MarkupCompatibilityProperties));
		
		#endregion
	}
}
