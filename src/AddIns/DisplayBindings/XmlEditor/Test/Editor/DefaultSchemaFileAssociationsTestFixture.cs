// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

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
				AddIn addin = AddIn.Load(reader);
				
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
