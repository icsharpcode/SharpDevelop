// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
