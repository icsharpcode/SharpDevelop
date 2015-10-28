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
using System.Windows.Controls;
using System.Windows.Data;
using NUnit.Framework;
using System.Windows.Markup;
using ICSharpCode.WpfDesign.XamlDom;
using System.IO;

namespace ICSharpCode.WpfDesign.Tests.XamlDom
{
	[TestFixture]
	public class MarkupExtensionTests : TestHelper
	{
		private const string PathWithSpaces = @"C:\\Folder A\\SubFolder A\\SubFolder B\\file with spaces.txt";
		private const string PathWithoutSpaces = @"C:\\FolderA\\SubFolderA\\SubFolderB\\file.txt";
		private const string PathWithCommasAndSpaces = @"C:\\Folder A\\Sub,Folder,A\\SubFolderB\\file,with,commas and spaces.txt";
		
		[Test]
		public void Test1()
		{
			TestMarkupExtension("Title=\"{Binding}\"");
		}

		[Test]
		public void Test2()
		{
			TestMarkupExtension("Title=\"{Binding Some}\"");
		}

		[Test]
		public void Test3()
		{
			TestMarkupExtension("Title=\"{ Binding  Some , ElementName = Some , Mode = TwoWay }\"");
		}

		[Test]
		public void Test4()
		{
			TestMarkupExtension("Content=\"{x:Type Button}\"");
		}		

		[Test]
		public void Test5()
		{
			TestMarkupExtension("Content=\"{t:MyExtension 1, 2}\"");
		}

		[Test]
		public void Test6()
		{
			TestMarkupExtension("Background=\"{x:Static SystemColors.ControlBrush}\"");
		}

		[Test]
		public void Test7()
		{
			TestMarkupExtension("Background=\"{DynamicResource {x:Static SystemColors.ControlBrushKey}}\"");
		}

		[Test]
		public void Test8()
		{
			TestMarkupExtension("Content=\"{Binding Some, RelativeSource={RelativeSource Self}}\"");
		}

		[Test]
		//[ExpectedException] 
		// Must differ from official XamlReader result, because Static dereference
		// To catch this we should use XamlDocument.Save() instead of XamlWriter.Save(instance)
		public void Test9()
		{
			TestMarkupExtension("Content=\"{x:Static t:MyStaticClass.StaticString}\"");
		}
		
		[Test]
		public void TestPathWithSpaces()
		{
			TestMarkupExtension("Content=\"{t:String " + PathWithSpaces + "}\"");
		}
		
		[Test]
		public void TestQuotedPathWithSpaces()
		{
			TestMarkupExtension("Content=\"{t:String '" + PathWithSpaces + "'}\"");
		}
		
		[Test]
		public void TestPathWithoutSpaces()
		{
			TestMarkupExtension("Content=\"{t:String " + PathWithoutSpaces + "}\"");
		}
		
		[Test]
		public void TestQuotedPathWithoutSpaces()
		{
			TestMarkupExtension("Content=\"{t:String '" + PathWithoutSpaces + "'}\"");
		}
		
		[Test]
		public void TestQuotedPathWithCommasAndSpaces()
		{
			TestMarkupExtension("Content=\"{t:String '" + PathWithCommasAndSpaces + "'}\"");
		}
		
		[Test]
		public void TestMultiBinding()
		{			
			string xaml = @"
<Window xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @""">
  <TextBox>
    <MultiBinding>
      <MultiBinding.Converter>
        <t:MyMultiConverter />
      </MultiBinding.Converter>
      <Binding Path=""SomeProperty"" />
      <t:MyBindingExtension />
    </MultiBinding>
  </TextBox>
</Window>";
			
			TestLoading(xaml);
			
			TestWindowMultiBinding((Window)XamlReader.Parse(xaml));
			
			var doc = XamlParser.Parse(new StringReader(xaml));
			
			TestWindowMultiBinding((Window)doc.RootInstance);
		}
		

		void TestWindowMultiBinding(Window w)
		{
			var textBox = (TextBox)w.Content;
			
			var expr = BindingOperations.GetMultiBindingExpression(textBox, TextBox.TextProperty);
			Assert.IsNotNull(expr);

			var converter = expr.ParentMultiBinding.Converter as MyMultiConverter;
			Assert.IsNotNull(converter);
			
			Assert.AreEqual(expr.ParentMultiBinding.Bindings.Count, 2);
		}
		
		[Test]
		public void TestPriorityBinding()
		{			
			string xaml = @"
<Window xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @""">
  <TextBox>
    <PriorityBinding>
      <Binding Path=""SomeProperty"" />
      <Binding Path=""OtherProperty"" />
    </PriorityBinding>
  </TextBox>
</Window>";
			
			TestLoading(xaml);
			
			TestWindowPriorityBinding((Window)XamlReader.Parse(xaml));
			
			var doc = XamlParser.Parse(new StringReader(xaml));
			
			TestWindowPriorityBinding((Window)doc.RootInstance);
		}
		
		
		void TestWindowPriorityBinding(Window w)
		{
			var textBox = (TextBox)w.Content;
			
			var expr = BindingOperations.GetPriorityBindingExpression(textBox, TextBox.TextProperty);
			Assert.IsNotNull(expr);
			
			Assert.AreEqual(expr.ParentPriorityBinding.Bindings.Count, 2);
		}

//        [Test]
//        public void Test10()
//        {
//            var s =			
//@"<Window
//	xmlns='http://schemas.microsoft.com/netfx/2007/xaml/presentation'
//	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
//	Content='{Binding}'";
//            var doc = XamlParser.Parse(new StringReader(s));
//            var binding = doc.RootElement.FindOrCreateProperty("Content").PropertyValue as XamlObject;
//            binding.FindOrCreateProperty("ElementName").PropertyValue = doc.CreatePropertyValue("name1", null);
//            Assert.AreEqual(binding.XmlAttribute.Value, "{Binding ElementName=name1}");

//        }

		static void TestMarkupExtension(string s)
		{
			TestLoading(@"<Window
	xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
	xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
	xmlns:t=""" + XamlTypeFinderTests.XamlDomTestsNamespace + @"""
	" + s + @"/>");
		}
	}

	public static class MyStaticClass
	{
		public static string StaticString = "a";
	}
	
	public class StringExtension : MarkupExtension
	{
		readonly string s;
		
		public StringExtension(string s)
		{
			TestHelperLog.Log(this.GetType().Name + " " + s);
			
			this.s = s;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return s;
		}
	}

	public class MyExtension : MarkupExtension
	{
		public MyExtension(object p1, object p2)
		{
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return null;
		}
	}
	
	public class MyMultiConverter : IMultiValueConverter
	{
		#region IMultiValueConverter implementation
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return System.Windows.DependencyProperty.UnsetValue;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		#endregion
		
	}
	
	public class MyBindingExtension : MarkupExtension
	{
		readonly Binding binding = new Binding();
		
		public MyBindingExtension()
		{
			var exampleClass = new ExampleClass();
			exampleClass.StringProp = "Test";
			
			binding.Source = exampleClass;
			binding.Path = new PropertyPath("StringProp");
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return binding.ProvideValue(serviceProvider);
		}
	}
}
