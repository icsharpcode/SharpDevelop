// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Common base class for the WixDirectoryElement and WixDirectoryRefElement
	/// classes.
	/// </summary>
	public abstract class WixDirectoryElementBase : XmlElement
	{
		public WixDirectoryElementBase(string localName, WixDocument document)
			: base(document.WixNamespacePrefix, localName, WixNamespaceManager.Namespace, document)
		{
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
		/// Gets any child directory elements.
		/// </summary>
		public WixDirectoryElement[] GetDirectories()
		{
			List<WixDirectoryElement> directories = new List<WixDirectoryElement>();
			foreach (XmlNode childNode in ChildNodes) {
				WixDirectoryElement childElement = childNode as WixDirectoryElement;
				if (childElement != null) {
					if (WixDirectoryElement.IsDirectoryElement(childElement.LocalName)) {
						directories.Add(childElement);
					}
				}
			}
			return directories.ToArray();
		}

		/// <summary>
		/// Adds a new directory with the specified id.
		/// </summary>
		public WixDirectoryElement AddDirectory(string id)
		{
			WixDirectoryElement directoryElement = new WixDirectoryElement((WixDocument)OwnerDocument);
			directoryElement.Id = id;
			return (WixDirectoryElement)AppendChild(directoryElement);
		}

	}
}
