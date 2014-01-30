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

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class ErrorWhenReadingSchemaTestFixture
	{
		MockXmlSchemaCompletionDataFactory factory;
		RegisteredXmlSchemas registeredXmlSchemas;
		MockFileSystem fileSystem;
		ApplicationException exceptionThrownWhenReadingSchema;
		
		[SetUp]
		public void Init()
		{
			string[] schemaFolders = new string[] { @"d:\projects\schemas" };
			
			exceptionThrownWhenReadingSchema = new ApplicationException("Read schema failed.");
			
			factory = new MockXmlSchemaCompletionDataFactory();
			factory.ExceptionToThrowWhenCreateXmlSchemaCalled = exceptionThrownWhenReadingSchema;
			
			fileSystem = new MockFileSystem();
			fileSystem.AddExistingFolders(schemaFolders);
			
			string[] sharpDevelopSchemaFiles = new string[] { @"d:\projects\schemas\addin.xsd" };
			fileSystem.AddDirectoryFiles(@"d:\projects\schemas", sharpDevelopSchemaFiles);
			
			registeredXmlSchemas = new RegisteredXmlSchemas(schemaFolders, String.Empty, fileSystem, factory);
			registeredXmlSchemas.ReadSchemas();
		}
		
		[Test]
		public void OneErrorReportedWhenReadingSchemas()
		{
			string message = @"Unable to read schema 'd:\projects\schemas\addin.xsd'.";
			RegisteredXmlSchemaError error = new RegisteredXmlSchemaError(message, exceptionThrownWhenReadingSchema);
			RegisteredXmlSchemaError[] expectedErrors = new RegisteredXmlSchemaError[] { error };
			Assert.AreEqual(expectedErrors, registeredXmlSchemas.GetSchemaErrors());
		}
	}
}
