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
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class AddUserSchemaTestFixture
	{
		RegisteredXmlSchemas registeredSchemas;
		string userDefinedSchemaFolder;
		XmlSchemaCompletion schema;
		int userSchemaAddedEventFiredCount;
		MockFileSystem fileSystem;
		MockXmlSchemaCompletionDataFactory factory;
		
		[SetUp]
		public void Init()
		{
			userDefinedSchemaFolder = @"c:\users\user\sharpdevelop\schemas";
			
			schema = LoadTestSchema();

			userSchemaAddedEventFiredCount = 0;
			fileSystem = new MockFileSystem();
			factory = new MockXmlSchemaCompletionDataFactory();
			registeredSchemas = new RegisteredXmlSchemas(new string[0], userDefinedSchemaFolder, fileSystem, factory);
			registeredSchemas.UserDefinedSchemaAdded += UserSchemaAdded;
			registeredSchemas.AddUserSchema(schema);
		}

		XmlSchemaCompletion LoadTestSchema()
		{
			string xml = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://test'>\r\n" +
				"</xs:schema>";
			
			XmlSchemaCompletion schema = new XmlSchemaCompletion(new StringReader(xml));
			schema.FileName = FileName.Create(@"c:\projects\schemas\test.xsd");
			return schema;
		}
		
		[Test]
		public void SchemaAddedToSchemaCollection()
		{
			Assert.AreEqual(schema, registeredSchemas.Schemas[0]);
		}
		
		[Test]
		public void SchemaFileNameChangedToNewLocationInUserDefinedSchemaFolder()
		{
			string expectedFileName = Path.Combine(userDefinedSchemaFolder, "test.xsd");
			Assert.AreEqual(expectedFileName, schema.FileName.ToString());
		}
		
		[Test]
		public void SchemaCopiedToUserDefinedSchemaFolder()
		{
			string fileName = Path.Combine(userDefinedSchemaFolder, "test.xsd");
			Assert.AreEqual(fileName, fileSystem.CopiedFileLocations[@"c:\projects\schemas\test.xsd"]);
		}
		
		[Test]
		public void UserDefinedSchemaFolderCreated()
		{
			Assert.AreEqual(userDefinedSchemaFolder, fileSystem.CreatedFolders[0]);
		}
		
		[Test]
		public void AddedSchemaNamespaceExistsInRegisteredSchemas()
		{
			Assert.IsTrue(registeredSchemas.SchemaExists("http://test"));
		}
		
		[Test]
		public void UserSchemaAddedEventFired()
		{
			Assert.AreEqual(1, userSchemaAddedEventFiredCount);
		}
		
		void UserSchemaAdded(object source, EventArgs e)
		{
			userSchemaAddedEventFiredCount++;
		}
	}
}
