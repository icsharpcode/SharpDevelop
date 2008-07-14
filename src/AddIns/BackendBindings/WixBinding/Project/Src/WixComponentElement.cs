// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
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
			get { return GetAttribute("Guid"); }
			set { SetAttribute("Guid", value); }
		}
		
		public string Id {
			get { return GetAttribute("Id"); }
			set { SetAttribute("Id", value); }
		}
		
		public string DiskId {
			get { return GetAttribute("DiskId"); }
			set { SetAttribute("DiskId", value); }
		}
		
		/// <summary>
		/// Checks whether the disk id has already been set for this component.
		/// </summary>
		public bool HasDiskId {
			get { return HasAttribute("DiskId"); }
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
			WixFileElement fileElement = new WixFileElement(this, fileName);
			return (WixFileElement)AppendChild(fileElement);
		}
		
		/// <summary>
		/// Creates an id from the filename.
		/// </summary>
		/// <remarks>
		/// Takes the filename, removes all periods, and 
		/// capitalises the first character and first extension character.
		/// </remarks>
		/// <param name="document">The Wix document is used to make sure the
		/// id generated is unique for that document.</param>
		/// <param name="fileName">The full filename including the directory to
		/// use when generating the id.</param>
		public static string GenerateIdFromFileName(WixDocument document, string fileName)
		{
			string id = GenerateIdFromFileName(fileName);
			if (!document.ComponentIdExists(id)) {
				return id;
			}
			
			// Add the parent folder to the id.
			string parentDirectory = WixDirectoryElement.GetLastDirectoryName(Path.GetDirectoryName(fileName));
			parentDirectory = FirstCharacterToUpperInvariant(parentDirectory);
			parentDirectory = WixFileElement.GenerateId(parentDirectory).Replace(".", String.Empty);
			id = String.Concat(parentDirectory, id);
			if (!document.ComponentIdExists(id)) {
				return id;
			}
			
			// Add a number to the end until we generate a unique id.
			int count = 0;
			string baseId = id;
			do {
				++count;
				id = String.Concat(baseId, count);
			} while (document.ComponentIdExists(id));
			
			return id;
		}
		
		/// <summary>
		/// Creates an id from the filename.
		/// </summary>
		/// <remarks>
		/// Takes the filename, removes all periods, and 
		/// capitalises the first character and first extension character.
		/// </remarks>
		public static string GenerateIdFromFileName(string fileName)
		{
			string fileNameNoExtension = Path.GetFileNameWithoutExtension(fileName);
			string idStart = String.Empty;
			if (fileNameNoExtension.Length > 0) {
				idStart = FirstCharacterToUpperInvariant(fileNameNoExtension).Replace(".", String.Empty);
			}
			
			// Remove period from extension and uppercase first extension char.
			string extension = Path.GetExtension(fileName);
			string idEnd = String.Empty;
			if (extension.Length > 1) {
				idEnd = FirstCharacterToUpperInvariant(extension.Substring(1));
			}
			return WixFileElement.GenerateId(String.Concat(idStart, idEnd));
		}
		
		/// <summary>
		/// Gets any child file elements.
		/// </summary>
		public WixFileElement[] GetFiles()
		{
			List<WixFileElement> files = new List<WixFileElement>();
			foreach (XmlNode childNode in ChildNodes) {
				WixFileElement childElement = childNode as WixFileElement;
				if (childElement != null) {
					files.Add(childElement);
				}
			}
			return files.ToArray();
		}
		
		/// <summary>
		/// Upper cases first character of string.
		/// </summary>
		static string FirstCharacterToUpperInvariant(string s)
		{
			return String.Concat(s.Substring(0, 1).ToUpperInvariant(), s.Substring(1));
		}
	}
}
