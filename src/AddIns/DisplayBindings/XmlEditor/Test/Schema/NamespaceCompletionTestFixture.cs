// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Schema
{
	/// <summary>
	/// The collection of schemas should provide completion data for the
	/// namespaces it holds.
	/// </summary>
	[TestFixture]
	public class NamespaceCompletionTestFixture
	{
		XmlCompletionItemCollection namespaceCompletionItems;
		string firstNamespace = "http://foo.com/foo.xsd";
		string secondNamespace = "http://bar.com/bar.xsd";
		
		[TestFixtureSetUp]
		public void FixtureInit()
		{
			XmlSchemaCompletionCollection items = new XmlSchemaCompletionCollection();
			
			StringReader reader = new StringReader(GetSchema(firstNamespace));
			XmlSchemaCompletion schema = new XmlSchemaCompletion(reader);
			items.Add(schema);
			
			reader = new StringReader(GetSchema(secondNamespace));
			schema = new XmlSchemaCompletion(reader);
			items.Add(schema);
			namespaceCompletionItems = items.GetNamespaceCompletion();
		}
		
		[Test]
		public void ShouldHaveTwoNamespaceCompletionItems()
		{
			Assert.AreEqual(2, namespaceCompletionItems.ToArray().Length);
		}
		
		[Test]
		public void NamespaceCompletionItemsCollectionContainsFirstNamespace()
		{
			Assert.IsTrue(namespaceCompletionItems.Contains(firstNamespace));
		}
		
		[Test]
		public void NamespaceCompletionItemsCollectionContainsSecondNamespace()
		{
			Assert.IsTrue(namespaceCompletionItems.Contains(secondNamespace));
		}		
		
		string GetSchema(string namespaceURI)
		{
			return "<?xml version=\"1.0\"?>\r\n" +
				"<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"\r\n" +
				"targetNamespace=\"" + namespaceURI + "\"\r\n" +
				"xmlns=\"" + namespaceURI + "\"\r\n" +
				"elementFormDefault=\"qualified\">\r\n" +
				"<xs:element name=\"note\">\r\n" +
				"</xs:element>\r\n" +
				"</xs:schema>";
		}
	}
}
