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

using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Schema
{
	[TestFixture]
	public class GetSchemaFromFileNameTestFixture
	{
		XmlSchemaCompletionCollection schemas;
		string expectedNamespace;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			schemas = new XmlSchemaCompletionCollection();
			XmlSchemaCompletion completionData = new XmlSchemaCompletion(ResourceManager.ReadXsdSchema());
			expectedNamespace = completionData.NamespaceUri;
			completionData.FileName = FileName.Create(@"C:\Schemas\MySchema.xsd");
			schemas.Add(completionData);
		}
		
		[Test]
		public void RelativeFileName()
		{
			XmlSchemaCompletion foundSchema = schemas.GetSchemaFromFileName(@"C:\Schemas\..\Schemas\MySchema.xsd");
			Assert.AreEqual(expectedNamespace, foundSchema.NamespaceUri);
		}

		[Test]
		public void LowerCaseFileName()
		{
			XmlSchemaCompletion foundSchema = schemas.GetSchemaFromFileName(@"C:\schemas\myschema.xsd");
			Assert.AreEqual(expectedNamespace, foundSchema.NamespaceUri);
		}
		
		[Test]
		public void MissingFileName()
		{
			Assert.IsNull(schemas.GetSchemaFromFileName(@"C:\Test\test.xsd"));
		}
	}
}
