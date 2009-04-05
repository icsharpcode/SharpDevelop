// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.XmlEditor;
using System;
using System.IO;
using System.Xml;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of Utils.
	/// </summary>
	public static class Utils
	{
		public static bool HasMatchingEndTag(string tagname, string text, int offset)
		{
			int index = XmlParser.GetActiveElementStartIndex(text, offset);
			if (index == -1)
				return false;
			
			text = text.Substring(index);
			
			XmlReader reader = XmlTextReader.Create(new StringReader(text));
			int startTags = 0;
			try {
				while (reader.Read()) {	
					switch (reader.NodeType) {
						case XmlNodeType.Element:
							startTags++;
							break;
						case XmlNodeType.EndElement:
							startTags--;
							if (startTags == 0 && tagname == reader.Name) {
								return true;
							}
							break;
					}
				}
			} catch (XmlException) {
				return false;
			}
			
			return false;
		}
	}
}
