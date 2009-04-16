// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
		}
	}
}
