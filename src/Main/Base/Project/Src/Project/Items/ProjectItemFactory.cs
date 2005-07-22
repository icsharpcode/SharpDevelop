// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of ProjectItemFactory.
	/// </summary>
	public static class ProjectItemFactory
	{
		public static ProjectItem CreateProjectItem(IProject project, string itemType)
		{
			switch (itemType) {
				case "Reference":
					return new ReferenceProjectItem(project);
				case "ProjectReference":
					return new ProjectReferenceProjectItem(project);
				case "COMReference":
					return new ComReferenceProjectItem(project);
				case "Import":
					return new ImportProjectItem(project);
					
				case "None":
				case "Compile":
				case "EmbeddedResource":
				case "Content":
				case "WebReferences":
				case "Folder":
				case "BootstrapperFile":
					return new FileProjectItem(project, (ItemType)Enum.Parse(typeof(ItemType), itemType));
				
				case "WebReferenceUrl":
					return new WebReferenceUrl(project);
					
				default:
					return new UnknownProjectItem(project, itemType);
			}
		}
	}
}
