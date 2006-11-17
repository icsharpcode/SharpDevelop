// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// A project item whose type is not known by SharpDevelop.
	/// </summary>
	public sealed class UnknownProjectItem : ProjectItem
	{
		internal UnknownProjectItem(IProject project, Microsoft.Build.BuildEngine.BuildItem buildItem)
			: base(project, buildItem)
		{
		}
		
		/// <summary>
		/// Constructor for internal use in ProjectDescriptor.
		/// </summary>
		internal UnknownProjectItem(IProject project, string itemType, string include)
			: base(project, new ItemType(itemType), include)
		{
		}
	}
}
