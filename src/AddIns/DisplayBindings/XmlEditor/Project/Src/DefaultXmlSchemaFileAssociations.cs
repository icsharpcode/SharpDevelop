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
