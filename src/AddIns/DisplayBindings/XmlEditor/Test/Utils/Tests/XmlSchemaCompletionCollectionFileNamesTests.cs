// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class XmlSchemaCompletionCollectionFileNamesTests
	{
		XmlSchemaCompletionCollection schemas;
		
		[SetUp]
		public void Init()
		{
			schemas = new XmlSchemaCompletionCollection();
		}
		
		[Test]
		public void NoItemsInSchemaCollection()
		{
			Assert.AreEqual(0, XmlSchemaCompletionCollectionFileNames.GetFileNames(schemas).Length);
		}
		
		[Test]
		public void OneSchemaWithFileName()
		{
			XmlSchemaCompletion schema = new XmlSchemaCompletion();
			schema.FileName = "a.xsd";
			schemas.Add(schema);
			
			string[] expectedFileNames = new string[] {"a.xsd"};
			Assert.AreEqual(expectedFileNames, XmlSchemaCompletionCollectionFileNames.GetFileNames(schemas));
		}
		
		[Test]
		public void TwoSchemasWithFileName()
		{
			XmlSchemaCompletion schema = new XmlSchemaCompletion();
			schema.FileName = "a.xsd";
			schemas.Add(schema);
			
			schema = new XmlSchemaCompletion();
			schema.FileName = "b.xsd";
			schemas.Add(schema);
			
			string[] expectedFileNames = new string[] {"a.xsd", "b.xsd"};
			Assert.AreEqual(expectedFileNames, XmlSchemaCompletionCollectionFileNames.GetFileNames(schemas));
		}

	}
}
