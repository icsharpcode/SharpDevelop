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
	/// <summary>
	/// Tests that we can detect the attribute's name.
	/// </summary>
	[TestFixture]
	public class AttributeNameTestFixture
	{		
		[Test]
		public void SuccessTest1()
		{
			string text = " foo='a";
			Assert.AreEqual("foo", XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}

		[Test]
		public void SuccessTest2()
		{
			string text = " foo='";
			Assert.AreEqual("foo", XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}		
		
		[Test]
		public void SuccessTest3()
		{
			string text = " foo=";
			Assert.AreEqual("foo", XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}			
		
		[Test]
		public void SuccessTest4()
		{
			string text = " foo=\"";
			Assert.AreEqual("foo", XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}	
		
		[Test]
		public void SuccessTest5()
		{
			string text = " foo = \"";
			Assert.AreEqual("foo", XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}			
		
		[Test]
		public void SuccessTest6()
		{
			string text = " foo = '#";
			Assert.AreEqual("foo", XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}	
		
		[Test]
		public void FailureTest1()
		{
			string text = "foo=";
			Assert.AreEqual(String.Empty, XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}		
		
		[Test]
		public void FailureTest2()
		{
			string text = "foo=<";
			Assert.AreEqual(String.Empty, XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}		
		
		[Test]
		public void FailureTest3()
		{
			string text = "a";
			Assert.AreEqual(String.Empty, XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}	
		
		[Test]
		public void FailureTest4()
		{
			string text = " a";
			Assert.AreEqual(String.Empty, XmlParser.GetAttributeName(text, text.Length), "Should have retrieved the attribute name 'foo'");
		}		
	}
}
