// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1683 $</version>
// </file>

using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Schema
{
	[TestFixture]
	public class GetSchemaFromFileNameTestFixture
	{
		XmlSchemaCompletionDataCollection schemas;
		string expectedNamespace;
		XmlCompletionDataProvider provider;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			schemas = new XmlSchemaCompletionDataCollection();
			XmlSchemaCompletionData completionData = new XmlSchemaCompletionData(ResourceManager.GetXsdSchema());
			expectedNamespace = completionData.NamespaceUri;
			completionData.FileName = @"C:\Schemas\MySchema.xsd";
			schemas.Add(completionData);
			
			provider = new XmlCompletionDataProvider(schemas, completionData, String.Empty);
		}
		
		[Test]
		public void RelativeFileName()
		{
			XmlSchemaCompletionData foundSchema = schemas.GetSchemaFromFileName(@"C:\Schemas\..\Schemas\MySchema.xsd");
			Assert.AreEqual(expectedNamespace, foundSchema.NamespaceUri);
		}
		
		[Test]
		public void RelativeFileNameFromProvider()
		{
			XmlSchemaCompletionData foundSchema = provider.FindSchemaFromFileName(@"C:\Schemas\..\Schemas\MySchema.xsd");
			Assert.AreEqual(expectedNamespace, foundSchema.NamespaceUri);
		}

		
		[Test]
		public void LowerCaseFileName()
		{
			XmlSchemaCompletionData foundSchema = schemas.GetSchemaFromFileName(@"C:\schemas\myschema.xsd");
			Assert.AreEqual(expectedNamespace, foundSchema.NamespaceUri);
		}
		
		[Test]
		public void MissingFileName()
		{
			Assert.IsNull(schemas.GetSchemaFromFileName(@"C:\Test\test.xsd"));
		}
	}
}
