// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class MockXmlSchemaCompletionDataFactoryTests
	{
		MockXmlSchemaCompletionDataFactory factory;
		
		[SetUp]
		public void Init()
		{
			factory = new MockXmlSchemaCompletionDataFactory();
		}
		
		[Test]
		public void XmlSchemaCompletionDataObjectCreated()
		{
			Assert.IsInstanceOf(typeof(XmlSchemaCompletionData), factory.CreateXmlSchemaCompletionData(String.Empty, String.Empty));
		}
		
		[Test]
		public void BaseUriRecordedWhenCreatingObject()
		{
			string schemaFileName = @"c:\projects\a.xsd";
			factory.CreateXmlSchemaCompletionData("baseUri", schemaFileName);
			
			Assert.AreEqual("baseUri", factory.GetBaseUri(schemaFileName));
		}
		
		[Test]
		public void SchemaLoadedForRegisteredFile()
		{
			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test' />";
			string schemaFileName = @"c:\projects\a.xsd";
			factory.AddSchemaXml(schemaFileName, xml);
			
			XmlSchemaCompletionData schema = factory.CreateXmlSchemaCompletionData(String.Empty, schemaFileName);
			Assert.AreEqual("http://test", schema.NamespaceUri);
		}
		
		[Test]
		public void CreateMethodThrowsExceptionIfConfigured()
		{
			ApplicationException ex = new ApplicationException("message");
			factory.ExceptionToThrowWhenCreateXmlSchemaCalled = ex;
			
			Assert.Throws<ApplicationException>(delegate {	factory.CreateXmlSchemaCompletionData(String.Empty, String.Empty); });
		}
	}
}
