// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			completionData.FileName = @"C:\Schemas\MySchema.xsd";
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
