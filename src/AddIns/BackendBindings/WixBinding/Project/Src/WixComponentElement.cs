// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	public class WixComponentElement : XmlElement
	{
		public const string ComponentElementName = "Component";
		
		public WixComponentElement(WixDocument document) 
			: base(document.WixNamespacePrefix, ComponentElementName, WixNamespaceManager.Namespace, document)
		{
		}
		
		public string Guid {
			get {
				return GetAttribute("Guid");
			}
			set {
				SetAttribute("Guid", value);
			}
		}
		
		public string Id {
			get {
				return GetAttribute("Id");
			}
			set {
				SetAttribute("Id", value);
			}
		}
		
		/// <summary>
		/// Generates a new guid for this component element.
		/// </summary>
		public void GenerateNewGuid()
		{
			Guid = System.Guid.NewGuid().ToString().ToUpperInvariant();
		}
		
		/// <summary>
		/// Creates a new file element with the specified filename.
		/// </summary>
		public WixFileElement AddFile(string fileName)
		{
			WixFileElement fileElement = new WixFileElement((WixDocument)OwnerDocument, fileName);
			return (WixFileElement)AppendChild(fileElement);
		}
	}
}
