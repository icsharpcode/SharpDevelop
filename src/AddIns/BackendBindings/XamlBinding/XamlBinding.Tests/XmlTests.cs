// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using NUnit.Framework;
using System;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	public class XmlTests
	{
		XmlElementPath elementPath;
		XmlElementPath expectedElementPath;
		string namespaceURI = "http://foo.com/foo.xsd";

		[Test]
		public void PathTest()
		{
			string text = "<foo xmlns='" + namespaceURI + "'><bar";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.Elements.Add(new QualifiedName("foo", namespaceURI));
			expectedElementPath.Elements.Add(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath),
			              "Incorrect active element path.");
		}
		
		[Test]
		public void ComplexPathNewLineTest()
		{
			string text = "<foo xmlns='" + namespaceURI + "'><bar";
			string text2 = "\n</foo>";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text + text2, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.Elements.Add(new QualifiedName("foo", namespaceURI));
			expectedElementPath.Elements.Add(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath),
			              "Incorrect active element path.");
		}
		
		[Test]
		public void ComplexPathTabTest()
		{
			string text = "<foo xmlns='" + namespaceURI + "'><bar";
			string text2 = "\t</foo>";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text + text2, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.Elements.Add(new QualifiedName("foo", namespaceURI));
			expectedElementPath.Elements.Add(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath),
			              "Incorrect active element path.");
		}
		
		[Test]
		public void InMarkupExtensionTest()
		{
			string xaml = "<Test val1=\"{Binding Value}\" />";
			int offset = "<Test val1=\"{Bin".Length;
			
			Assert.AreEqual(true, XmlParser.IsInsideAttributeValue(xaml, offset));
			Assert.AreEqual("{Binding Value}", XmlParser.GetAttributeValueAtIndex(xaml, offset));
			Assert.AreEqual("val1", XmlParser.GetAttributeNameAtIndex(xaml, offset));
		}
		
		[Test]
		public void InMarkupExtensionNamedParameterTest()
		{
			string xaml = "<Test val1=\"{Binding Value, Path=Control}\" />";
			int offset = "<Test val1=\"{Binding Value, Path=".Length;
			
			Assert.AreEqual(true, XmlParser.IsInsideAttributeValue(xaml, offset));
			Assert.AreEqual("{Binding Value, Path=Control}", XmlParser.GetAttributeValueAtIndex(xaml, offset));
			Assert.AreEqual("val1", XmlParser.GetAttributeNameAtIndex(xaml, offset));
		}
		
		[Test]
		public void AtEqualSignTest1()
		{
			string xaml = "<Test val1 = \"{Binding Value, Path=Control}\" />";
			int offset = "<Test val1 =".Length;
			
			Assert.AreEqual("val1", XmlParser.GetAttributeNameAtIndex(xaml, offset));
		}
		
		[Test]
		public void AtEqualSignTest2()
		{
			string xaml = "<Test val1 = \"{Binding Value, Path=Control}\" />";
			int offset = "<Te".Length;
			
			Assert.AreEqual("", XmlParser.GetAttributeNameAtIndex(xaml, offset));
		}
		
		[Test]
		public void AtEqualSignTest3()
		{
			string xaml = "<Test val1   = \"{Binding Value, Path=Control}\" />";
			int offset = "<Test val1   = \"{Binding Value, Path".Length;
			
			Assert.AreEqual("val1", XmlParser.GetAttributeNameAtIndex(xaml, offset));
		}
		
		[Test]
		public void AtEqualSignTest4()
		{
			string xaml = "<Test val1   = \"{Binding Value, Path=Control}\" />";
			int offset = "<Test val1  ".Length;
			
			Assert.AreEqual("val1", XmlParser.GetAttributeNameAtIndex(xaml, offset));
		}
		
		[Test]
		public void AtEqualSignTest5()
		{
			string xaml = "<Test val1   = \"{Binding Value, Path=Control}\" />";
			int offset = "<Test val1   = \"{Binding Value, Path=".Length;
			
			Assert.AreEqual("val1", XmlParser.GetAttributeNameAtIndex(xaml, offset));
		}
		
		[Test]
		public void AtEqualSignTest6()
		{
			string xaml = "<Test val1   = \"{Binding Value, Path=Control}\" />";
			int offset = "<Test val1   = \"{Binding Value, Path=C".Length;
			
			Assert.AreEqual("val1", XmlParser.GetAttributeNameAtIndex(xaml, offset));
		}
	}
}
