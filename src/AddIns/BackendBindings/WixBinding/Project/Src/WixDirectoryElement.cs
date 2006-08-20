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
	public class WixDirectoryElement : WixDirectoryElementBase
	{
		public const string DirectoryElementName = "Directory";
		public const string RootDirectoryId = "TARGETDIR";
		
		public WixDirectoryElement(WixDocument document) 
			: base(DirectoryElementName, document)
		{
		}
		
		/// <summary>
		/// Determines whether the specified element name refers to a Directory element.
		/// </summary>
		public static bool IsDirectoryElement(string name)
		{
			return name == DirectoryElementName;
		}
		
		/// <summary>
		/// Creates the directory element and sets its Id and SourceName.
		/// </summary>
		public static WixDirectoryElement CreateRootDirectory(WixDocument document)
		{
			WixDirectoryElement rootDirectory = new WixDirectoryElement(document);
			rootDirectory.Id = RootDirectoryId;
			rootDirectory.SourceName = "SourceDir";
			return rootDirectory;
		}
			
		/// <summary>
		/// Adds a new component element to this directory element.
		/// </summary>
		public WixComponentElement AddComponent()
		{
			return AddComponent(String.Empty);
		}
		
		/// <summary>
		/// Adds a new component element to this directory element.
		/// </summary>
		public WixComponentElement AddComponent(string id)
		{
			WixComponentElement componentElement = new WixComponentElement((WixDocument)OwnerDocument);
			componentElement.GenerateNewGuid();
			componentElement.Id = id;
			return (WixComponentElement)AppendChild(componentElement);			
		}
		
		public string SourceName {
			get {
				return GetAttribute("SourceName");
			}
			set {
				SetAttribute("SourceName", value);
			}
		}
			
		public string DirectoryName {
			get {
				return GetAttribute("Name");
			}
			set {
				SetAttribute("Name", value);
			}
		}
	}
}
