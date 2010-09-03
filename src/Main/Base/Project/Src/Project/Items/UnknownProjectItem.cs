// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// A project item whose type is not known by SharpDevelop.
	/// </summary>
	public sealed class UnknownProjectItem : ProjectItem
	{
		internal UnknownProjectItem(IProject project, IProjectItemBackendStore buildItem)
			: base(project, buildItem)
		{
		}
		
		/// <summary>
		/// Constructor for internal use in ProjectDescriptor.
		/// </summary>
		internal UnknownProjectItem(IProject project, string itemType, string include)
			: this(project, itemType, include, false)
		{
		}

		/// <summary>
		/// Constructor for internal use in ProjectDescriptor.
		/// </summary>
		internal UnknownProjectItem(IProject project, string itemType, string include, bool treatIncludeAsLiteral)
			: base(project, new ItemType(itemType), include, treatIncludeAsLiteral)
		{
		}		
	}
}
