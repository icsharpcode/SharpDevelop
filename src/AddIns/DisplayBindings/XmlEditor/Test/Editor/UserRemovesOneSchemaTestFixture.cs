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
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class UserRemovesOneSchemaTestFixture
	{
		RegisteredXmlSchemasEditor schemasEditor;
		MockXmlSchemaCompletionDataFactory factory;
		XmlSchemaFileAssociations associations;
		XmlSchemaCompletionCollection schemas;
		Properties properties;
		MockXmlSchemasPanel panel;
		RegisteredXmlSchemas registeredXmlSchemas;
		string testSchemaXml;
		
		[SetUp]
		public void Init()
		{
			testSchemaXml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test' />";
			XmlSchemaCompletion testSchema = new XmlSchemaCompletion(new StringReader(testSchemaXml));
			testSchema.IsReadOnly = false;

			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://xml' />";
			XmlSchemaCompletion xmlSchema = new XmlSchemaCompletion(new StringReader(xml));
			xmlSchema.IsReadOnly = false;
			
			schemas = new XmlSchemaCompletionCollection();
			schemas.Add(testSchema);
			schemas.Add(xmlSchema);
			
			string userDirectory = @"c:\users\user\schemas";
			factory = new MockXmlSchemaCompletionDataFactory();
			MockFileSystem fileSystem = new MockFileSystem();
			registeredXmlSchemas = new RegisteredXmlSchemas(new string[0], userDirectory, fileSystem, factory);
			registeredXmlSchemas.Schemas.AddRange(schemas);
			
			properties = new Properties();
			associations = new XmlSchemaFileAssociations(properties, new DefaultXmlSchemaFileAssociations(new AddInTreeNode()), schemas);
			
			panel = new MockXmlSchemasPanel();
			schemasEditor = new RegisteredXmlSchemasEditor(registeredXmlSchemas, new string[0], associations, panel, factory);
			schemasEditor.LoadOptions();
			
			panel.SelectedXmlSchemaListItemIndex = 0;
			schemasEditor.RemoveSelectedSchema();
		}

		[Test]
		public void TestSchemaRemovedFromRegisteredXmlSchemas()
		{
			schemasEditor.SaveOptions();
			Assert.IsFalse(registeredXmlSchemas.SchemaExists("http://test"));
		}
		
		[Test]
		public void XmlSchemaNotRemovedFromRegisteredXmlSchemas()
		{
			schemasEditor.SaveOptions();
			Assert.IsTrue(registeredXmlSchemas.SchemaExists("http://xml"));
		}
		
		[Test]
		public void XmlSchemasChanged()
		{
			Assert.IsTrue(schemasEditor.XmlSchemasChanged);
		}
		
		[Test]
		public void TestSchemaRemoveFromSchemaListBoxLeavingOnlyOneItemInList()
		{
			schemasEditor.SaveOptions();
			Assert.AreEqual(1, panel.XmlSchemaListItemCount);
		}
		
		[Test]
		public void TestSchemaRemovedFromListBox()
		{
			Assert.AreEqual("http://xml", panel.GetXmlSchemaListItem(0).ToString());
		}
		
		[Test]
		public void RemoveSchemaWhenNothingSelectedDoesNothing()
		{
			panel.SelectedXmlSchemaListItemIndex = -1;
			schemasEditor.RemoveSelectedSchema();
			
			Assert.AreEqual(1, panel.XmlSchemaListItemCount);
		}
		
		[Test]
		public void TestSchemaShouldStillExistAfterAddingTestSchemaAgainAfterBeingRemoved()
		{
			panel.OpenFileDialogResultToReturn = true;
			panel.OpenFileDialogFileNameToReturn = @"c:\users\user\schemas\test.xsd";
			factory.AddSchemaXml(panel.OpenFileDialogFileNameToReturn, testSchemaXml);
			schemasEditor.AddSchemaFromFileSystem();
			schemasEditor.SaveOptions();
			
			Assert.IsTrue(registeredXmlSchemas.SchemaExists("http://test"));
		}
	}
}
