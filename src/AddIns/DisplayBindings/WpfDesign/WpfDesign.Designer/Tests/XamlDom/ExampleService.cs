// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;

namespace ICSharpCode.WpfDesign.Tests.XamlDom
{
	/// <summary>
	/// Provides an example attached property.
	/// </summary>
	public static class ExampleService
	{
		public static readonly DependencyProperty ExampleProperty = DependencyProperty.RegisterAttached(
			"Example", typeof(string), typeof(ExampleService)
		);
		
		public static string GetExample(DependencyObject element)
		{
			TestHelperLog.Log("ExampleService.GetExample");
			return (string)element.GetValue(ExampleProperty);
		}
		
		public static void SetExample(DependencyObject element, string value)
		{
			TestHelperLog.Log("ExampleService.SetExample");
			element.SetValue(ExampleProperty, value);
		}
	}
	
	public class ExampleDependencyObject : DependencyObject
	{
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			// TODO: add this test, check for correct setting of NameScope
			//TestHelperLog.Log("ExampleDependencyObject.OnPropertyChanged " + e.Property.Name);
		}
	}
}
