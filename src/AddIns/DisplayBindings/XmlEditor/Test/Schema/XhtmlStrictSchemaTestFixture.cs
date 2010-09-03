// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
