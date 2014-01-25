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
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using Rhino.Mocks;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class DefaultSchemaFileAssociationsTestFixture
	{
		DefaultXmlSchemaFileAssociations schemaAssociations;
		
		[SetUp]
		public void Init()
		{
			string addinXml = "<AddIn name     = 'Xml Editor'\r\n" +
       								"author      = ''\r\n" +
       								"copyright   = 'prj:///doc/copyright.txt'\r\n" +
       								"description = ''\r\n" +
       								"addInManagerHidden = 'preinstalled'>\r\n" +
								"</AddIn>";

			using (StringReader reader = new StringReader(addinXml)) {
				var addInTree = MockRepository.GenerateStrictMock<IAddInTree>();
				AddIn addin = AddIn.Load(addInTree, reader);
				
				AddInTreeNode addinTreeNode = new AddInTreeNode();

				Properties properties1 = new Properties();
				properties1.Set<string>("id", ".xml");
				properties1.Set<string>("namespaceUri", "http://example.com");
				
				Properties properties2 = new Properties();
				properties2.Set<string>("id", ".xsl");
				properties2.Set<string>("namespaceUri", "http://example.com/xsl");
				properties2.Set<string>("namespacePrefix", "xs");
				
				addinTreeNode.AddCodons(
					new Codon[] {
						new Codon(addin, "SchemaAssociation", properties1, new ICondition[0]),
						new Codon(addin, "SchemaAssociation", properties2, new ICondition[0])
					});
				
				schemaAssociations = new DefaultXmlSchemaFileAssociations(addinTreeNode);
			}
		}
		
		[Test]
		public void FirstSchemaAssociationIsForXmlFiles()
		{
			XmlSchemaFileAssociation expectedSchemaAssociation = new XmlSchemaFileAssociation(".xml", "http://example.com", String.Empty);
			Assert.AreEqual(expectedSchemaAssociation, schemaAssociations[0]);
		}
		
		[Test]
		public void SecondSchemaAssociationIsForXslFiles()
		{
			XmlSchemaFileAssociation expectedSchemaAssociation = new XmlSchemaFileAssociation(".xsl", "http://example.com/xsl", "xs");
			Assert.AreEqual(expectedSchemaAssociation, schemaAssociations[1]);
		}
		
		[Test]
		public void FindSchemaAssociationByFileExtension()
		{
			XmlSchemaFileAssociation expectedSchemaAssociation = new XmlSchemaFileAssociation(".xsl", "http://example.com/xsl", "xs");
			Assert.AreEqual(expectedSchemaAssociation, schemaAssociations.Find(".xsl"));
		}
		
		[Test]
		public void FindSchemaAssociationByFileExtensionIsCaseInsensitive()
		{
			XmlSchemaFileAssociation expectedSchemaAssociation = new XmlSchemaFileAssociation(".xsl", "http://example.com/xsl", "xs");
			Assert.AreEqual(expectedSchemaAssociation, schemaAssociations.Find(".XSL"));
		}	
	}
}
