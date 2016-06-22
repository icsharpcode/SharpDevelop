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
		
		[Test]
		public void NullString()
		{
			Assert.IsFalse(XmlParser.IsNamespaceDeclaration(null, 0));
		}
	}
}
