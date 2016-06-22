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

using System;
using System.Xml;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// Tests the xhtml1-strict schema.
	/// </summary>
	[TestFixture]
	public class XhtmlStrictSchemaTestFixture
	{
		XmlSchemaCompletion schemaCompletion;
		XmlElementPath h1Path;
		XmlCompletionItemCollection h1Attributes;
		string namespaceURI = "http://www.w3.org/1999/xhtml";
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			schemaCompletion = new XmlSchemaCompletion(ResourceManager.ReadXhtmlStrictSchema());
			
			// Set up h1 element's path.
			h1Path = new XmlElementPath();
			h1Path.AddElement(new QualifiedName("html", namespaceURI));
			h1Path.AddElement(new QualifiedName("body", namespaceURI));
			h1Path.AddElement(new QualifiedName("h1", namespaceURI));
			
			// Get h1 element info.
			h1Attributes = schemaCompletion.GetAttributeCompletion(h1Path);
		}
		
		[Test]
		public void H1HasAttributes()
		{
			Assert.IsTrue(h1Attributes.Count > 0, "Should have at least one attribute.");
		}
	}
}
