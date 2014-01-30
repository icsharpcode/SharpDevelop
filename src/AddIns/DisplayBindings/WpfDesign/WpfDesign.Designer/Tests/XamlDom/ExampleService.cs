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
		
		public static readonly DependencyProperty ExampleCollectionProperty = DependencyProperty.RegisterAttached(
			"ExampleCollection", typeof(ExampleClassList), typeof(ExampleService)
		);
		
		public static ExampleClassList GetExampleCollection(DependencyObject element)
		{
			TestHelperLog.Log("ExampleService.GetExampleCollection");
			return (ExampleClassList)element.GetValue(ExampleCollectionProperty);
		}
		
		public static void SetExampleCollection(DependencyObject element, ExampleClassList value)
		{
			TestHelperLog.Log("ExampleService.SetExampleCollection");
			element.SetValue(ExampleCollectionProperty, value);
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
