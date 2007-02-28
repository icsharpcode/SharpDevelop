// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;

namespace ICSharpCode.WpfDesign.XamlDom.Tests
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
		
	}
}
