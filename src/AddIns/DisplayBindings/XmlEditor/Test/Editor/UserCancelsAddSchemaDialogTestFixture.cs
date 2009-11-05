// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class UserCancelsAddSchemaDialogTestFixture
	{
		MockXmlSchemasPanel panel;
		RegisteredXmlSchemasEditor schemasEditor;
		MockXmlSchemaCompletionDataFactory factory;
		XmlSchemaManager schemaManager;
		
		[SetUp]
		public void Init()
		{
			MockFileSystem fileSystem = new MockFileSystem();
			factory = new MockXmlSchemaCompletionDataFactory();
			schemaManager = new XmlSchemaManager(new string[0], @"c:\users\user\sharpdevelop\schemas", fileSystem, factory);
			XmlSchemaCompletionDataCollection schemas = new XmlSchemaCompletionDataCollection();
			XmlEditorOptions options = new XmlEditorOptions(new Properties(), new DefaultXmlSchemaFileAssociations(null), schemas);
			panel = new MockXmlSchemasPanel();
			
			schemasEditor = new RegisteredXmlSchemasEditor(schemaManager, new string[0], options, panel, factory);
			schemasEditor.LoadOptions();
						
			panel.OpenFileDialogFileNameToReturn = @"c:\temp\a.xsd";
			panel.OpenFileDialogResultToReturn = false;

			schemasEditor.AddSchemaFromFileSystem();
		}
		
		[Test]
		public void XmlSchemasAreNotChanged()
		{
			Assert.IsFalse(schemasEditor.XmlSchemasChanged);
		}
	}
}
