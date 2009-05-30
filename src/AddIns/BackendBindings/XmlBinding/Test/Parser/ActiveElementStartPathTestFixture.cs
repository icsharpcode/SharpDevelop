// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 3003 $</version>
// </file>

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
		public void PathTest1()
		{
			string text = "<foo xmlns='" + namespaceURI + "' ";
			elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.Elements.Add(new QualifiedName("foo", namespaceURI));
			Assert.IsTrue(elementPath.Equals(expectedElementPath), 
			              "Incorrect active element path.");
		}		
		
		[Test]
		public void PathTest2()
		{
			string text = "<foo xmlns='" + namespaceURI + "' ><bar ";
			elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.Elements.Add(new QualifiedName("foo", namespaceURI));
			expectedElementPath.Elements.Add(new QualifiedName("bar", namespaceURI));
			Assert.IsTrue(expectedElementPath.Equals(elementPath), 
			              "Incorrect active element path.");
		}			
		
		[Test]
		public void PathTest3()
		{
			string text = "<f:foo xmlns:f='" + namespaceURI + "' ><f:bar ";
			elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.Elements.Add(new QualifiedName("foo", namespaceURI, "f"));
			expectedElementPath.Elements.Add(new QualifiedName("bar", namespaceURI, "f"));
			Assert.IsTrue(expectedElementPath.Equals(elementPath), 
			              "Incorrect active element path.");
		}		
		
		[Test]
		public void PathTest4()
		{
			string text = "<x:foo xmlns:x='" + namespaceURI + "' ";
			elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.Elements.Add(new QualifiedName("foo", namespaceURI, "x"));
			Assert.IsTrue(expectedElementPath.Equals(elementPath), 
			              "Incorrect active element path.");
		}	
		
		[Test]
		public void PathTest5()
		{
			string text = "<foo xmlns='" + namespaceURI + "'>\r\n"+ 
							"<y\r\n" +
						  	"Id = 'foo' ";
	
			elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.Elements.Add(new QualifiedName("foo", namespaceURI));
			expectedElementPath.Elements.Add(new QualifiedName("y", namespaceURI));
			Assert.IsTrue(expectedElementPath.Equals(elementPath), 
			              "Incorrect active element path.");
		}
		
		[Test]
		public void PathTest6()
		{
			string text = "<bar xmlns='http://bar'>\r\n" +
							"<foo xmlns='" + namespaceURI + "' ";
	
			elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
			
			expectedElementPath = new XmlElementPath();
			expectedElementPath.Elements.Add(new QualifiedName("foo", namespaceURI));
			Assert.IsTrue(expectedElementPath.Equals(elementPath), 
			              "Incorrect active element path.");
		}
		
		/// <summary>
		/// Tests that we get no path back if we are outside the start
		/// tag.
		/// </summary>
		[Test]
		public void OutOfStartTagPathTest1()
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
