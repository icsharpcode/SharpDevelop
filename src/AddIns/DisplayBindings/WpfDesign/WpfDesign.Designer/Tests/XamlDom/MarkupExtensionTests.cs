// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;
using System.Windows.Markup;
using ICSharpCode.WpfDesign.XamlDom;
using System.IO;

namespace ICSharpCode.WpfDesign.Tests.XamlDom
{
	[TestFixture]
	public class MarkupExtensionTests : TestHelper
	{
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
		[Ignore]
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
}
