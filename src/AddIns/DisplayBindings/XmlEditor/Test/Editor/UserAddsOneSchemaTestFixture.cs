// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class UserAddsOneSchemaTestFixture
	{
		MockXmlSchemasPanel panel;
		RegisteredXmlSchemasEditor schemasEditor;
		MockXmlSchemaCompletionDataFactory factory;
		string newXmlSchemaFileName;
		RegisteredXmlSchemas registeredXmlSchemas;
		MockFileSystem fileSystem;
		
		[SetUp]
		public void Init()
		{
			fileSystem = new MockFileSystem();
			factory = new MockXmlSchemaCompletionDataFactory();
			registeredXmlSchemas = new RegisteredXmlSchemas(new string[0], @"c:\users\user\sharpdevelop\schemas", fileSystem, factory);
			XmlSchemaCompletionCollection schemas = new XmlSchemaCompletionCollection();
			XmlSchemaFileAssociations associations = new XmlSchemaFileAssociations(new Properties(), new DefaultXmlSchemaFileAssociations(null), schemas);
			panel = new MockXmlSchemasPanel();
			
			schemasEditor = new RegisteredXmlSchemasEditor(registeredXmlSchemas, new string[0], associations, panel, factory);
			schemasEditor.LoadOptions();
			
			newXmlSchemaFileName = @"c:\projects\a.xsd";
			factory.AddSchemaXml(newXmlSchemaFileName, 
				"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test' />");
			
			panel.OpenFileDialogFileNameToReturn = newXmlSchemaFileName;
			panel.OpenFileDialogResultToReturn = true;
			
			schemasEditor.AddSchemaFromFileSystem();
		}
		
		[Test]
		public void OpenFileDialogCheckFileExistsSetToTrue()
		{
			Assert.IsTrue(panel.OpenFileDialog.CheckFileExists);
		}
		
		[Test]
		public void OpenFileDialogMultiSelectSetToFalse()
		{
			Assert.IsFalse(panel.OpenFileDialog.Multiselect);
		}
		
		[Test]
		public void OpenFileDialogFilterSetToXmlSchemaFiles()
		{
			Assert.AreEqual(panel.OpenFileDialog.Filter, StringParser.Parse("${res:SharpDevelop.FileFilter.XmlSchemaFiles}|*.xsd|${res:SharpDevelop.FileFilter.AllFiles}|*.*"));
		}
		
		[Test]
		public void NewSchemaAddedToSchemaList()
		{
			Assert.AreEqual("http://test", panel.GetXmlSchemaListItem(0).NamespaceUri);
		}
		
		[Test]
		public void NewSchemaAddedToSchemaListIsNotReadOnly()
		{
			Assert.IsFalse(panel.GetXmlSchemaListItem(0).ReadOnly);
		}
		
		[Test]
		public void NewSchemaIsSelectedInSchemaList()
		{
			Assert.AreEqual("http://test", panel.GetSelectedXmlSchemaListItem().ToString());
		}
		
		[Test]
		public void BaseUriUsedWhenCreatingNewXmlSchemaObject()
		{
			string baseUri = XmlSchemaCompletion.GetUri(newXmlSchemaFileName);
			Assert.AreEqual(baseUri, factory.GetBaseUri(newXmlSchemaFileName));
		}
		
		[Test]
		public void SchemasEditorXmlSchemasChangedReturnsTrue()
		{
			Assert.IsTrue(schemasEditor.XmlSchemasChanged);
		}
		
		[Test]
		public void SchemaAddedToRegisteredXmlSchemasAfterSchemaEditorOptionsSaved()
		{
			schemasEditor.SaveOptions();
			Assert.IsNotNull(registeredXmlSchemas.Schemas["http://test"]);
		}
		
		[Test]
		public void ErrorMessageDisplayedWhenExceptionThrownLoadingSchema()
		{
			ApplicationException ex = new ApplicationException("message");
			fileSystem.ExceptionToThrowWhenCopyFileCalled = ex;
			schemasEditor.SaveOptions();
			
			ExceptionErrorMessage message = new ExceptionErrorMessage(ex, "${res:ICSharpCode.XmlEditor.XmlSchemasPanel.UnableToSaveChanges}");
			Assert.AreEqual(message, panel.ExceptionErrorMessage);
		}
		
		[Test]
		public void SaveOptionsReturnsFalseWhenExceptionThrownLoadingSchema()
		{
			ApplicationException ex = new ApplicationException("message");
			fileSystem.ExceptionToThrowWhenCopyFileCalled = ex;
			Assert.IsFalse(schemasEditor.SaveOptions());
		}
		
		[Test]
		public void RemoveSchemaAddedAndSaveChangesShouldNotLeaveSchemaInRegisteredXmlSchemas()
		{
			panel.SelectedXmlSchemaListItemIndex = 0;
			schemasEditor.RemoveSelectedSchema();
			schemasEditor.SaveOptions();
			
			Assert.AreEqual(0, registeredXmlSchemas.Schemas.Count);
		}
		
		[Test]
		public void AnotherSchemaAddedToPanelIsSortedByNamespaceInSchemasList()
		{
			string fileName = @"c:\projects\b.xsd";
			factory.AddSchemaXml(fileName, 
				"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://new' />");
			
			panel.OpenFileDialogFileNameToReturn = fileName;
			panel.OpenFileDialogResultToReturn = true;
			
			schemasEditor.AddSchemaFromFileSystem();
			
			string[] expectedSchemas = new string[] { "http://new", "http://test" };
			Assert.AreEqual(expectedSchemas, panel.GetXmlSchemaListItemNamespaces());
		}

		[Test]
		public void SchemaListItemAddedIsScrolledIntoView()
		{
			Assert.AreEqual(panel.GetSelectedXmlSchemaListItem(), panel.XmlSchemaListItemScrolledIntoView);
		}
	}
}
