// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	public class DefaultXmlFileExtensions : List<string>
	{
		public DefaultXmlFileExtensions()
		{
			AddInTreeNode node = AddInTree.GetTreeNode("/AddIns/DefaultTextEditor/CodeCompletion", false);
			GetXmlFileExtensions(node);
		}
		
		public DefaultXmlFileExtensions(AddInTreeNode node)
		{
			GetXmlFileExtensions(node);
		}
		
		void GetXmlFileExtensions(AddInTreeNode node)
		{
			if (node != null) {
				foreach (Codon codon in node.Codons) {
					if (codon.Id == "Xml") {
						foreach (string ext in codon.Properties["extensions"].Split(';')) {
							Add(ext.Trim());
						}
					}
				}
			}
		}		
	}
}
