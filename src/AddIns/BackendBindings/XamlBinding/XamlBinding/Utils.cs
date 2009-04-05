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
		
		public static string GetNameAttributeValue(string text, int offset)
		{
			int index = XmlParser.GetActiveElementStartIndex(text, offset);
			if (index == -1)
				return null;
			index = text.IndexOf(' ', index);
			
			text = text.Substring(index);
			int endIndex = text.IndexOfAny(new char[] { '<', '>' });
			text = text.Substring(0, endIndex).Trim(' ', '\t', '\n', '\r');
			
			string[] attributes = text.Split(new string[] {"=\"", "\""}, StringSplitOptions.None);
			
			for (int i = 0; i < attributes.Length; i += 2) {
				if (i + 1 < attributes.Length) {
					if (attributes[i].Trim(' ', '\t', '\n', '\r').ToLowerInvariant() == "name")
						return attributes[i + 1];
				} else
					break;
			}
			
			return null;
		}
	}
}
