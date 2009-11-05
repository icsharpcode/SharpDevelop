// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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

				Properties properties = new Properties();
				properties.Set<string>("id", ".xml");
				properties.Set<string>("namespaceUri", "http://example.com");
				addinTreeNode.Codons.Add(new Codon(addin, "SchemaAssociation", properties, new ICondition[0]));
				
				properties = new Properties();
				properties.Set<string>("id", ".xsl");
				properties.Set<string>("namespaceUri", "http://example.com/xsl");
				properties.Set<string>("namespacePrefix", "xs");
				addinTreeNode.Codons.Add(new Codon(addin, "SchemaAssociation", properties, new ICondition[0]));
				
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
