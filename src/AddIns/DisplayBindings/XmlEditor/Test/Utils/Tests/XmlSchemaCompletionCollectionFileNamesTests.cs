// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
