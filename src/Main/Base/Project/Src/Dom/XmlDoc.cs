using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Description of XmlDoc.
	/// </summary>
	public class XmlDoc
	{
		Dictionary<string, string> xmlDescription = new Dictionary<string, string>();
		
		public Dictionary<string, string> XmlDescription {
			get {
				return xmlDescription;
			}
		}
		public XmlDoc()
		{
		}
		
		void ReadMembersSection(XmlTextReader reader)
		{
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.EndElement:
						if (reader.LocalName == "members") {
							return;
						}
						break;
					case XmlNodeType.Element:
						if (reader.LocalName == "member") {
							string memberAttr = reader.GetAttribute(0);
							string innerXml   = reader.ReadInnerXml();
							xmlDescription[memberAttr] = innerXml;
						}
						break;
				}
			}
		}
		
		public static XmlDoc Load(TextReader textReader)
		{
			XmlDoc newXmlDoc = new XmlDoc();
			using (XmlTextReader reader = new XmlTextReader(textReader)) {
				while (reader.Read()) {
					if (reader.IsStartElement()) {
						switch (reader.LocalName) {
							case "members":
								newXmlDoc.ReadMembersSection(reader);
								break;
						}
					}
				}
			}
			return newXmlDoc;
		}
		
		public static XmlDoc Load(string fileName)
		{
			using (TextReader textReader = File.OpenText(fileName)) {
				return Load(textReader);
			}
		}
	}
}
