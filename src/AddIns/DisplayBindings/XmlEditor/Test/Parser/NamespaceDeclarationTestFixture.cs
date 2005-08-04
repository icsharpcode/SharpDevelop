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
	/// When the user hits the '=' key we need to produce intellisense
	/// if the attribute is of the form 'xmlns' or 'xmlns:foo'.  This
	/// tests the parsing of the text before the cursor to see if the
	/// attribute is a namespace declaration.
	/// </summary>
	[TestFixture]
	public class NamespaceDeclarationTestFixture
	{		
		[Test]
		public void SuccessTest1()
		{
			string text = "<foo xmlns=";
			bool isNamespace = XmlParser.IsNamespaceDeclaration(text, text.Length);
			Assert.IsTrue(isNamespace, "Namespace should be recognised.");
		}
		
		[Test]
		public void SuccessTest2()
		{
			string text = "<foo xmlns =";
			bool isNamespace = XmlParser.IsNamespaceDeclaration(text, text.Length);
			Assert.IsTrue(isNamespace, "Namespace should be recognised.");
		}
		
		[Test]
		public void SuccessTest3()
		{
			string text = "<foo \r\nxmlns\r\n=";
			bool isNamespace = XmlParser.IsNamespaceDeclaration(text, text.Length);
			Assert.IsTrue(isNamespace, "Namespace should be recognised.");
		}		
		
		[Test]
		public void SuccessTest4()
		{
			string text = "<foo xmlns:nant=";
			bool isNamespace = XmlParser.IsNamespaceDeclaration(text, text.Length);
			Assert.IsTrue(isNamespace, "Namespace should be recognised.");
		}	
		
		[Test]
		public void SuccessTest5()
		{
			string text = "<foo xmlns";
			bool isNamespace = XmlParser.IsNamespaceDeclaration(text, text.Length);
			Assert.IsTrue(isNamespace, "Namespace should be recognised.");
		}		
		
		[Test]
		public void SuccessTest6()
		{
			string text = "<foo xmlns:nant";
			bool isNamespace = XmlParser.IsNamespaceDeclaration(text, text.Length);
			Assert.IsTrue(isNamespace, "Namespace should be recognised.");
		}		
		
		[Test]
		public void SuccessTest7()
		{
			string text = " xmlns=";
			bool isNamespace = XmlParser.IsNamespaceDeclaration(text, text.Length);
			Assert.IsTrue(isNamespace, "Namespace should be recognised.");
		}	
		
		[Test]
		public void SuccessTest8()
		{
			string text = " xmlns";
			bool isNamespace = XmlParser.IsNamespaceDeclaration(text, text.Length);
			Assert.IsTrue(isNamespace, "Namespace should be recognised.");
		}			
		
		[Test]
		public void SuccessTest9()
		{
			string text = " xmlns:f";
			bool isNamespace = XmlParser.IsNamespaceDeclaration(text, text.Length);
			Assert.IsTrue(isNamespace, "Namespace should be recognised.");
		}	
		
		[Test]
		public void FailureTest1()
		{
			string text = "<foo bar=";
			bool isNamespace = XmlParser.IsNamespaceDeclaration(text, text.Length);
			Assert.IsFalse(isNamespace, "Namespace should not be recognised.");
		}		
		
		[Test]
		public void FailureTest2()
		{
			string text = "";
			bool isNamespace = XmlParser.IsNamespaceDeclaration(text, text.Length);
			Assert.IsFalse(isNamespace, "Namespace should not be recognised.");
		}		
		
		[Test]
		public void FailureTest3()
		{
			string text = " ";
			bool isNamespace = XmlParser.IsNamespaceDeclaration(text, text.Length);
			Assert.IsFalse(isNamespace, "Namespace should not be recognised.");
		}			
	}
}
