// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision: 6083 $</version>
// </file>

using System;

namespace ICSharpCode.NRefactory.Parser.VB
{
	public class XmlModeInfo : ICloneable
	{
		public bool inXmlTag, inXmlCloseTag, isDocumentStart;
		public int level;
		
		public XmlModeInfo(bool isSpecial)
		{
			level = isSpecial ? -1 : 0;
			inXmlTag = inXmlCloseTag = isDocumentStart = false;
		}
		
		public object Clone()
		{
			return new XmlModeInfo(false) {
				inXmlCloseTag = this.inXmlCloseTag,
				inXmlTag = this.inXmlTag,
				isDocumentStart = this.isDocumentStart,
				level = this.level
			};
		}
	}
}
