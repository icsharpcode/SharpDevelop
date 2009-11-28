// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Text;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	public abstract class WixElementBase : XmlElement
	{
		public WixElementBase(string localName, WixDocument document)
			: base(document.GetWixNamespacePrefix(), localName, WixNamespaceManager.Namespace, document)
		{
		}
		
		public string Id {
			get { return GetAttribute("Id"); }
			set { SetAttribute("Id", value); }
		}
		
		public string GetXml(WixTextWriter wixWriter)
		{
			StringBuilder xml = new StringBuilder();
			StringWriter stringWriter = new StringWriter(xml);
			using (XmlWriter xmlWriter = wixWriter.Create(stringWriter)) {
				WriteTo(xmlWriter);
			}
			return RemoveWixNamespace(xml.ToString());
		}
		
		string RemoveWixNamespace(string xml)
		{
			string namespaceDeclaration = String.Concat(" xmlns=\"", WixNamespaceManager.Namespace, "\"");
			return xml.Replace(namespaceDeclaration, String.Empty);
		}
		
		protected WixDocument OwnerWixDocument {
			get { return (WixDocument)OwnerDocument; }
		}
	}
}
