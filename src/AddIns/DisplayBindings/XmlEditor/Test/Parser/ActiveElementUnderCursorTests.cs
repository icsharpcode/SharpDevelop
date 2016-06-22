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

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using System.Xml;

namespace XmlEditor.Tests.Parser
{
	/// <summary>
	/// Tests the XmlParser.GetActiveElementStartPathAtIndex which finds the element
	/// path where the index is at. The index may be in the middle or start of the element
	/// tag.
	/// </summary>
	[TestFixture]
	public class ActiveElementUnderCursorTests
	{
		XmlElementPath elementPath;
		XmlElementPath expectedElementPath;
		string namespaceURI = "http://foo.com/foo.xsd";
		
		[Test]
		public void EmptyDocumentTest()
		{
			elementPath = XmlParser.GetActiveElementStartPathAtIndex("", 0);
			expectedElementPath = new XmlElementPath();
			Assert.IsTrue(elementPath.Equals(expectedElementPath), 
			              "Incorrect active element path.");
		}
		
		[Test]
		public void PathTest1()
		{
			string text = "<foo xmlns='" + namespaceURI + "'><bar ";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text, text.IndexOf("bar "));
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			expectedElementPath.AddElement(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath), 
			              "Incorrect active element path.");
		}		
		
		[Test]
		public void PathTest2()
		{
			string text = "<foo xmlns='" + namespaceURI + "'><bar>";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text, text.IndexOf("bar>"));
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			expectedElementPath.AddElement(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath), 
			              "Incorrect active element path.");
		}
		
		[Test]
		public void PathTest3()
		{
			string text = "<foo xmlns='" + namespaceURI + "'><bar>";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text, text.IndexOf("ar>"));
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			expectedElementPath.AddElement(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath), 
			              "Incorrect active element path.");
		}
		
		[Test]
		public void PathTest4()
		{
			string text = "<foo xmlns='" + namespaceURI + "'><bar>";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text, text.Length - 1);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			expectedElementPath.AddElement(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath), 
			              "Incorrect active element path.");
		}
		
		[Test]
		public void PathTest5()
		{
			string text = "<foo xmlns='" + namespaceURI + "'><bar a='a'>";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text, text.IndexOf("='a'"));
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			expectedElementPath.AddElement(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath), 
			              "Incorrect active element path.");
		}
		
		[Test]
		public void PathTest6()
		{
			string text = "<foo xmlns='" + namespaceURI + "'><bar a='a'";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text, text.Length - 1);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			expectedElementPath.AddElement(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath), 
			              "Incorrect active element path.");
		}
		
		[Test]
		public void PathTest7()
		{
			string text = "<foo xmlns='" + namespaceURI + "'><bar a='a'";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			expectedElementPath.AddElement(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath), 
			              "Incorrect active element path.");
		}
		
		[Test]
		public void PathTest8()
		{
			string text = "<foo xmlns='" + namespaceURI + "'><bar>";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			expectedElementPath.AddElement(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath), 
			              "Incorrect active element path.");
		}
		
		[Test]
		public void PathTest9()
		{
			string text = "<foo xmlns='" + namespaceURI + "'><bar ";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			expectedElementPath.AddElement(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath), 
			              "Incorrect active element path.");
		}
		
		[Test]
		public void PathTest10()
		{
			string text = "<foo xmlns='" + namespaceURI + "'><bar Id=\r\n</foo>";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text, text.IndexOf("Id="));
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			expectedElementPath.AddElement(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath), 
			              "Incorrect active element path.");
		}
		
		[Test]
		public void PathTest11()
		{
			string text = "<foo xmlns='" + namespaceURI + "'>";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text, 2);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath), 
			              "Incorrect active element path.");
		}

		[Test]
		public void PathWithNewLine()

		{
			string text = "<foo xmlns='" + namespaceURI + "'><bar";
			string text2 = "\n</foo>";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text + text2, text.Length);

			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			expectedElementPath.AddElement(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath), "Incorrect active element path.");
		}
		
		[Test]
		public void PathWithTab()

		{
			string text = "<foo xmlns='" + namespaceURI + "'><bar";
			string text2 = "\t</foo>";
			elementPath = XmlParser.GetActiveElementStartPathAtIndex(text + text2, text.Length);

			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			expectedElementPath.AddElement(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath),  "Incorrect active element path.");
		}
	}
}
