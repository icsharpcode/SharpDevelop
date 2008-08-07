using System;
using NUnit.Framework;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.Tests.XamlDom
{
	[TestFixture]
	public class MarkupExtensionTests : TestHelper
	{
		[Test]
		[Ignore]
		public void Test1()
		{
			TestMarkupExtension("Title=\"{Binding}\"");
		}

		[Test]
		[Ignore]
		public void Test2()
		{
			TestMarkupExtension("Title=\"{Binding Some}\"");
		}

		[Test]
		[Ignore]
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
		[Ignore]
		public void Test8()
		{
			TestMarkupExtension("Content=\"{Binding Some, RelativeSource={RelativeSource Self}}\"");
		}

		[Test]
		[Ignore]
		[ExpectedException] // Must differ from official XamlReader result
		public void Test9()
		{
			TestMarkupExtension("Content=\"{x:Static t:MyStaticClass.StaticString}\"");
		}

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