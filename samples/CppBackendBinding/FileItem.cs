// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using ICSharpCode.SharpDevelop.Project;

namespace CppBackendBinding
{
	/// <summary>
	/// Represents a {File} element in the .vcproj file.
	/// </summary>
	sealed class FileItem
	{
		public readonly ProjectItem ProjectItem;
		public readonly XmlElement XmlElement;
		
		/// <summary>
		/// Loads a file item from XML.
		/// </summary>
		public FileItem(FileGroup group, XmlElement fileElement)
		{
			this.XmlElement = fileElement;
			string relativePath = fileElement.GetAttribute("RelativePath");
			if (relativePath.StartsWith(".\\")) {
				// SharpDevelop doesn't like paths starting with ".\", so strip it away:
				relativePath = relativePath.Substring(2);
			}
			this.ProjectItem = new FileProjectItem(group.Project, group.ItemType, relativePath);
		}
		
		/// <summary>
		/// Creates a new file item.
		/// </summary>
		public FileItem(XmlDocument document, ProjectItem item)
		{
			this.ProjectItem = item;
			this.XmlElement = document.CreateElement("File");
			SaveChanges();
		}
		
		public void SaveChanges()
		{
			this.XmlElement.SetAttribute("RelativePath", this.ProjectItem.Include);
		}
	}
}
