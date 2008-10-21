// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2421 $</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

using ICSharpCode.Xaml;
using NUnit.Framework;
using System.Xml.Linq;

namespace ICSharpCode.WpfDesign.Tests.Xaml
{
	[TestFixture]
	public class XamlTypeFinderTests : TestHelper
	{
		void Test(Type t, XName name)
		{
			var result = TestXamlProject.TypeFinder.FindType(name);
			Assert.AreEqual(t, result.SystemType);
		}

		void TestExtension(Type t, XName name)
		{
			var result = TestXamlProject.TypeFinder.FindExtensionType(name);
			Assert.AreEqual(t, result.SystemType);
		}
		
		[Test]
		public void FindWindow()
		{
			Test(typeof(Window), XamlConstants.Presentation2006Namespace + "Window");
		}
		
		[Test]
		public void FindButton()
		{
			Test(typeof(Button), XamlConstants.Presentation2007Namespace + "Button");
		}
		
		[Test]
		public void FindBindingMarkupExtension()
		{
			Test(typeof(StaticResourceExtension), XamlConstants.Presentation2007Namespace + "StaticResourceExtension");
		}
		
		[Test]
		public void FindNullExtension()
		{
			Test(typeof(NullExtension), XamlConstants.XamlNamespace + "NullExtension");
		}

		[Test]
		public void FindNullExtension2()
		{
			TestExtension(typeof(NullExtension), XamlConstants.XamlNamespace + "Null");
		}
		
		[Test]
		public void FindExampleClass()
		{
			Test(typeof(ExampleClass), TestHelper.TestNamespace + "ExampleClass");
		}
	}
}
