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
		/// <summary>
		/// Maximum short name length that the directory element will use when
		/// generating a short name from the long name.
		/// </summary>
		public const int MaximumDirectoryShortNameLength = 8;
		
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
			SetDirectoryName(directoryElement, name);
			return (WixDirectoryElement)AppendChild(directoryElement);
		}
		
		/// <summary>
		/// Sets the directory name. Generates a short name if required.
		/// </summary>
		void SetDirectoryName(WixDirectoryElement directoryElement, string name)
		{
			if (name.Length > MaximumDirectoryShortNameLength) {
				directoryElement.ShortName = GetUniqueShortName(name);
				directoryElement.LongName = name;
			} else {
				directoryElement.ShortName = name;
			}
		}
		
		string GetUniqueShortName(string name)
		{	
			// Try the truncated name on its own first.
			name = name.Replace(".", String.Empty);
			name = name.Substring(0, MaximumDirectoryShortNameLength);
			if (!ShortNameExists(name)) {
				return name;
			}
			
			// Add a digit to the name until a unique one is found.
			return ShortFileName.GetUniqueName(name.Substring(0, name.Length - 1), ShortNameExists);
		}
		
		/// <summary>
		/// Checks whether the short directory name exists in the document.
		/// </summary>
		bool ShortNameExists(string name)
		{
			string xpath = String.Concat("w:Directory[@Name='", XmlEncoder.Encode(name, '\''), "']");
			XmlNodeList nodes = SelectNodes(xpath, new WixNamespaceManager(OwnerDocument.NameTable));
			return nodes.Count > 0;
		}
	}
}
