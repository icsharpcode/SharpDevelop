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

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Tests that the standard W3C namespace for XSD files is recognised.
	/// </summary>
	[TestFixture]
	public class XmlSchemaNamespaceTests
	{
		[Test]
		public void IsXmlSchemaNamespace()
		{
			Assert.IsTrue(XmlSchemaDefinition.IsXmlSchemaNamespace("http://www.w3.org/2001/XMLSchema"));
		}
		
		[Test]
		public void IsNotXmlSchemaNamespace()
		{
			Assert.IsFalse(XmlSchemaDefinition.IsXmlSchemaNamespace("http://foo.com"));
		}
		
		[Test]
		public void EmptyString()
		{
			Assert.IsFalse(XmlSchemaDefinition.IsXmlSchemaNamespace(String.Empty));
		}
	}
}
