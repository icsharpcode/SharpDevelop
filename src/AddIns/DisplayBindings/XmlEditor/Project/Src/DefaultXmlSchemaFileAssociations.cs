// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	public class DefaultXmlSchemaFileAssociations : List<XmlSchemaFileAssociation>
	{
		public DefaultXmlSchemaFileAssociations()
		{
			AddInTreeNode node = AddInTree.GetTreeNode("/AddIns/XmlEditor/DefaultSchemaFileAssociations", false);
			GetDefaultAssociations(node);
		}
		
		public DefaultXmlSchemaFileAssociations(AddInTreeNode node)
		{
			GetDefaultAssociations(node);
		}
		
		void GetDefaultAssociations(AddInTreeNode node)
		{
			if (node != null) {
				foreach (Codon codon in node.Codons) {
					string fileExtension = codon.Id;
					string namespaceUri = codon.Properties["namespaceUri"];
					string namespacePrefix = codon.Properties["namespacePrefix"];
					Add(new XmlSchemaFileAssociation(fileExtension, namespaceUri, namespacePrefix));
				}			
			}
		}

		public XmlSchemaFileAssociation Find(string fileExtension)
		{
			fileExtension = fileExtension.ToLowerInvariant();
			foreach (XmlSchemaFileAssociation schemaAssociation in this) {
				if (schemaAssociation.FileExtension == fileExtension) {
					return schemaAssociation;
				}
			}
			return new XmlSchemaFileAssociation(String.Empty, String.Empty);
		}
	}
}
