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

namespace XmlEditor.Tests.Completion
{
	[TestFixture]
	public class GetCompletionDataProviderTestFixture
	{
		XmlCompletionDataProvider completionProvider;
		XmlSchemaCompletionData defaultSchemaCompletionData;
		XmlEditorOptions options;
		XmlSchemaCompletionData otherSchema;
		
		[SetUp]
		public void Init()
		{
			XmlSchemaCompletionDataCollection schemas = new XmlSchemaCompletionDataCollection();
			options = new XmlEditorOptions(new Properties(), new DefaultXmlSchemaFileAssociations(new AddInTreeNode()), schemas);

			XmlSchemaFileAssociation schemaFileAssociation = new XmlSchemaFileAssociation(".xml", "http://test", "x");
			options.SetSchemaFileAssociation(schemaFileAssociation);
			
			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test' />";
			defaultSchemaCompletionData = new XmlSchemaCompletionData(new StringReader(xml));
			schemas.Add(defaultSchemaCompletionData);

			xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://other-schema' />";
			otherSchema = new XmlSchemaCompletionData(new StringReader(xml));
			schemas.Add(otherSchema);
			
			completionProvider = options.GetProvider(".xml");
		}
		
		[Test]
		public void XmlCompletionDataProviderCreatedWithCorrectDefaultNamespacePrefix()
		{
			Assert.AreEqual("x", completionProvider.DefaultNamespacePrefix);
		}
		
		[Test]
		public void XmlCompletionDataProviderCreatedWithCorrectDefaultSchemaCompletionData()
		{
			Assert.AreSame(defaultSchemaCompletionData, completionProvider.DefaultSchemaCompletionData);
		}
		
		[Test]
		public void XmlCompletionDataProviderCreatedWithSchemasCollection()
		{
			Assert.AreSame(otherSchema, completionProvider.FindSchema("http://other-schema"));
		}
	}
}
