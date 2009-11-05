// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		XmlEditorOptions options;
		XmlSchemaCompletionDataCollection schemas;
		Properties properties;
		MockXmlSchemasPanel panel;
		XmlSchemaManager schemaManager;
		string testSchemaXml;
		
		[SetUp]
		public void Init()
		{
			testSchemaXml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test' />";
			XmlSchemaCompletionData testSchema = new XmlSchemaCompletionData(new StringReader(testSchemaXml));
			testSchema.ReadOnly = false;

			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://xml' />";
			XmlSchemaCompletionData xmlSchema = new XmlSchemaCompletionData(new StringReader(xml));
			xmlSchema.ReadOnly = false;
			
			schemas = new XmlSchemaCompletionDataCollection();
			schemas.Add(testSchema);
			schemas.Add(xmlSchema);
			
			string userDirectory = @"c:\users\user\schemas";
			factory = new MockXmlSchemaCompletionDataFactory();
			MockFileSystem fileSystem = new MockFileSystem();
			schemaManager = new XmlSchemaManager(new string[0], userDirectory, fileSystem, factory);
			schemaManager.Schemas.AddRange(schemas);
			
			properties = new Properties();
			options = new XmlEditorOptions(properties, new DefaultXmlSchemaFileAssociations(new AddInTreeNode()), schemas);
			
			panel = new MockXmlSchemasPanel();
			schemasEditor = new RegisteredXmlSchemasEditor(schemaManager, new string[0], options, panel, factory);
			schemasEditor.LoadOptions();
			
			panel.SelectedXmlSchemaListItemIndex = 0;
			schemasEditor.RemoveSelectedSchema();
		}

		[Test]
		public void TestSchemaRemovedFromSchemaManager()
		{
			schemasEditor.SaveOptions();
			Assert.IsFalse(schemaManager.SchemaExists("http://test"));
		}
		
		[Test]
		public void XmlSchemaNotRemovedFromSchemaManager()
		{
			schemasEditor.SaveOptions();
			Assert.IsTrue(schemaManager.SchemaExists("http://xml"));
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
			
			Assert.IsTrue(schemaManager.SchemaExists("http://test"));
		}
	}
}
