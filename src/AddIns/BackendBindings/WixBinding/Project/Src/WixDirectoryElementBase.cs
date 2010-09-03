// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	public abstract class WixDirectoryElementBase : WixElementBase
	{
		public WixDirectoryElementBase(string localName, WixDocument document)
			: base(localName, document)
		{
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
					directories.Add(childElement);
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
		/// Adds a new directory with the specified name and generates a unique id 
		/// for the directory.
		/// </summary>
		public WixDirectoryElement AddDirectory(string name)
		{
			WixDirectoryElement directoryElement = new WixDirectoryElement(OwnerWixDocument);
			directoryElement.Id = GenerateUniqueChildDirectoryId(name);
			directoryElement.DirectoryName = name;
			AppendChild(directoryElement);
			return directoryElement;
		}
		
		string GenerateUniqueChildDirectoryId(string childDirectoryId)
		{
			childDirectoryId = WixFileElement.GenerateId(childDirectoryId);
			if (!OwnerWixDocument.DirectoryIdExists(childDirectoryId)) {
				return childDirectoryId;
			}
			return GenerateUniqueChildDirectoryIdUsingParentDirectoryId(childDirectoryId);
		}
		
		string GenerateUniqueChildDirectoryIdUsingParentDirectoryId(string childDirectoryId)
		{
			return Id + childDirectoryId;
		}
	}
}
