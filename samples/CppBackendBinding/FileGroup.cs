// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;
using ICSharpCode.SharpDevelop.Project;

namespace CppBackendBinding
{
	/// <summary>
	/// Represents a {Filter} element in the .vcproj file.
	/// </summary>
	sealed class FileGroup
	{
		public readonly CppProject Project;
		public readonly ItemType ItemType;
		public readonly string[] Extensions;
		public readonly XmlElement XmlElement;
		
		public FileGroup(CppProject project, XmlElement filterElement)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (filterElement == null)
				throw new ArgumentNullException("filterElement");
			this.Project = project;
			this.XmlElement = filterElement;
			switch (filterElement.GetAttribute("UniqueIdentifier")) {
				case "{4FC737F1-C7A5-4376-A066-2A32D752A2FF}":
					ItemType = ItemType.Compile;
					break;
				case "{93995380-89BD-4b04-88EB-625FBE52EBFB}":
					ItemType = ItemType.Header;
					break;
				case "{67DA6AB6-F800-4c08-8B7A-83BB121AAD01}":
					ItemType = ItemType.Resource;
					break;
				default:
					ItemType = new ItemType(filterElement.GetAttribute("Name"));
					break;
			}
			Extensions = filterElement.GetAttribute("Filter").Split(';');
		}
	}
}
