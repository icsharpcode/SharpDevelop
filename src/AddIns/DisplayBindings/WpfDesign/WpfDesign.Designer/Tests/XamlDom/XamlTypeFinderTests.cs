// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

using ICSharpCode.WpfDesign.XamlDom;
using NUnit.Framework;

namespace ICSharpCode.WpfDesign.Tests.XamlDom
{
	[TestFixture]
	public class XamlTypeFinderTests : TestHelper
	{
		XamlTypeFinder typeFinder;
		
		[SetUp]
		public void FixtureSetUp()
		{
			typeFinder = XamlTypeFinder.CreateWpfTypeFinder();
		}
		
		[Test]
		public void FindWindow()
		{
			Assert.AreEqual(typeof(Window),
			                typeFinder.GetType(XamlConstants.PresentationNamespace, "Window"));
		}
		
		[Test]
		public void FindButton()
		{
			Assert.AreEqual(typeof(Button),
			                typeFinder.GetType(XamlConstants.PresentationNamespace, "Button"));
		}
		
		[Test]
		public void FindBindingMarkupExtension()
		{
			Assert.AreEqual(typeof(StaticResourceExtension),
			                typeFinder.GetType(XamlConstants.PresentationNamespace, "StaticResourceExtension"));
		}
		
		[Test]
		public void FindNullExtension()
		{
			Assert.AreEqual(typeof(NullExtension),
			                typeFinder.GetType(XamlConstants.XamlNamespace, "NullExtension"));
		}
		
		public const string XamlDomTestsNamespace = "clr-namespace:ICSharpCode.WpfDesign.Tests.XamlDom;assembly=ICSharpCode.WpfDesign.Tests";
		
		[Test]
		public void FindExampleClass()
		{
			Assert.AreEqual(typeof(ExampleClass),
			                typeFinder.GetType(XamlDomTestsNamespace,
			                                   "ExampleClass"));
		}
	}
}
