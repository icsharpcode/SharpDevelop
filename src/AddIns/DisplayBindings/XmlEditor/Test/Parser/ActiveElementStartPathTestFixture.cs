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
	[TestFixture]
	public class ActiveElementStartPathTestFixture 
	{		
		XmlElementPath elementPath;
		XmlElementPath expectedElementPath;
		string namespaceURI = "http://foo.com/foo.xsd";
		
		[Test]
		public void GetActiveElementStartPathForRootElement()
		{
			string text = "<foo xmlns='" + namespaceURI + "' ";
			elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath), 
			              "Incorrect active element path.");
		}		
		
		[Test]
		public void GetActiveElementStartPathForChildElement()
		{
			string text = "<foo xmlns='" + namespaceURI + "' ><bar ";
			elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			expectedElementPath.AddElement(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(expectedElementPath.Equals(elementPath), 
			              "Incorrect active element path.");
		}			
		
		[Test]
		public void GetActiveElementStartPathForChildElementWithNamespacePrefix()
		{
			string text = "<f:foo xmlns:f='" + namespaceURI + "' ><f:bar ";
			elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI, "f"));
			expectedElementPath.AddElement(new QualifiedName("bar", namespaceURI, "f"));
			Assert.IsTrue(expectedElementPath.Equals(elementPath), 
			              "Incorrect active element path.");
		}		
		
		[Test]
		public void GetActiveElementStartPathForRootElementWithNamespacePrefix()
		{
			string text = "<x:foo xmlns:x='" + namespaceURI + "' ";
			elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI, "x"));
			Assert.IsTrue(expectedElementPath.Equals(elementPath), 
			              "Incorrect active element path.");
		}	
		
		[Test]
		public void GetActiveElementStartPathWithTextIncludingChildElementAttributesOnDifferentLine()
		{
			string text = "<foo xmlns='" + namespaceURI + "'>\r\n"+ 
							"<y\r\n" +
						  	"Id = 'foo' ";
	
			elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			expectedElementPath.AddElement(new QualifiedName("y", namespaceURI));
			Assert.IsTrue(expectedElementPath.Equals(elementPath), 
			              "Incorrect active element path.");
		}
		
		[Test]
		public void GetActiveElementStartPathWithTwoElementsInDifferentNamespaces()
		{
			string text = "<bar xmlns='http://bar'>\r\n" +
							"<foo xmlns='" + namespaceURI + "' ";
	
			elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.AddElement(new QualifiedName("bar", "http://bar"));
			expectedElementPath.AddElement(new QualifiedName("foo", namespaceURI));
			Assert.IsTrue(expectedElementPath.Equals(elementPath), 
			              "Incorrect active element path.");
		}
		
		/// <summary>
		/// Tests that we get no path back if we are outside the start
		/// tag.
		/// </summary>
		[Test]
		public void GetActiveElementStartPathWhenOutOfStartTagPath()
		{
			string text = "<foo xmlns='" + namespaceURI + "'> ";
	
			elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
			Assert.AreEqual(0, elementPath.Elements.Count, "Should have no path.");
		}
		
		/// <summary>
		/// If the user has entered just the opening tag and then tries to go to the schema definition of the active
		/// element the XmlParser throws a null exception. The user is also not indenting the xml with tabs but
		/// spaces. This test fixture tests checks for that bug. Note at the bug does not appear if the xml is indented with
		/// tabs.
		/// </summary>
		[Test]
		public void ActiveElementIsEmptyElementFollowedBySpaces()
		{
			string xml = "<Page>\r\n" +
						"    <Grid><\r\n" + // Cursor position is after the opening tag < at the end of this line.
						"    </Grid>\r\n" +
						"</Page>";
			
			string textToFind = "<Grid><";
			int index = xml.IndexOf(textToFind) + textToFind.Length;

			// Sanity check that the index position is correct.
			Assert.AreEqual('\r', xml[index]);
			Assert.AreEqual('<', xml[index - 1]);
			
			XmlElementPath path = XmlParser.GetActiveElementStartPathAtIndex(xml, index);
			Assert.AreEqual(0, path.Elements.Count, "Should be no elements since there is no element at the index.");
		}		
		
		/// <summary>
		/// If the user presses the space bar only having typed in the start tag then there is null
		/// reference exception. 
		/// </summary>
		[Test]
		public void EmptyElementStartTagFollowedBySpace()
		{
			string xml = "<Xml1>\r\n" +
						"\t< ";
			
			XmlElementPath path = XmlParser.GetActiveElementStartPath(xml, xml.Length - 1);
			Assert.AreEqual(0, path.Elements.Count, "Should be no elements since there is no element at the index.");
		}
		
		/// <summary>
		/// If the user presses the space bar after typing in an element name starting with a space
		/// character then a null exception occurs.
		/// </summary>
		[Test]
		public void SpacesAroundElementName()
		{
			string xml = "<Xml1>\r\n" +
						"\t< a ";
			
			XmlElementPath path = XmlParser.GetActiveElementStartPath(xml, xml.Length - 1);
			Assert.AreEqual(0, path.Elements.Count, "Should be no elements since there is no element at the index.");			
		}
	}
}
