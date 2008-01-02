// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Xml;

using ICSharpCode.XmlEditor;

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
			get { return GetAttribute("Id"); }
			set { SetAttribute("Id", value); }
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
		/// Gets any child component elements.
		/// </summary>
		public WixComponentElement[] GetComponents()
		{
			List<WixComponentElement> components = new List<WixComponentElement>();
			foreach (XmlNode childNode in ChildNodes) {
				WixComponentElement childElement = childNode as WixComponentElement;
				if (childElement != null) {
					components.Add(childElement);
				}
			}
			return components.ToArray();
		}

		/// <summary>
		/// Adds a new directory with the specified name and id. A short name
		/// will be generated if the name is too long.
		/// </summary>
		public WixDirectoryElement AddDirectory(string name)
		{
			WixDirectoryElement directoryElement = new WixDirectoryElement((WixDocument)OwnerDocument);
			directoryElement.Id = WixFileElement.GenerateId(name);
			directoryElement.DirectoryName = name;
			return (WixDirectoryElement)AppendChild(directoryElement);
		}		
	}
}
