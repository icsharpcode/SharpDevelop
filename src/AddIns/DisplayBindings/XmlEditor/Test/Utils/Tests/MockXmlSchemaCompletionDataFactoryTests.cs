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
			Assert.IsInstanceOf(typeof(XmlSchemaCompletion), factory.CreateXmlSchemaCompletionData(String.Empty, String.Empty));
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
			
			XmlSchemaCompletion schema = factory.CreateXmlSchemaCompletionData(String.Empty, schemaFileName);
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
