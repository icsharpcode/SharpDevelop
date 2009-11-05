// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Editor
{
	[TestFixture]
	public class SchemaAssociationConvertToStringTests
	{
		[Test]
		public void SchemaAssociationWithoutPrefixConvertedToString()
		{
			XmlSchemaFileAssociation schemaAssocation = new XmlSchemaFileAssociation(".xml", "namespaceUri");
			Assert.AreEqual(".xml|namespaceUri|", schemaAssocation.ToString());
		}
		
		[Test]
		public void SchemaAssociationWithPrefixConvertedToString()
		{
			XmlSchemaFileAssociation schemaAssocation = new XmlSchemaFileAssociation(".xml", "namespaceUri", "prefix");
			Assert.AreEqual(".xml|namespaceUri|prefix", schemaAssocation.ToString());
		}	
	}
}
