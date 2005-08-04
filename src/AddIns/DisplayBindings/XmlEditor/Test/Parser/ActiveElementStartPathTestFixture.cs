//
// SharpDevelop Xml Editor
//
// Copyright (C) 2005 Matthew Ward
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Matthew Ward (mrward@users.sourceforge.net)

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
	}
}
