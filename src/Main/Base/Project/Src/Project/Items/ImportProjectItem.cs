// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Project item for default namespace import (e.g in VB)
	/// </summary>
	public sealed class ImportProjectItem : ProjectItem
	{
		public ImportProjectItem(IProject project, string include)
			: base(project, ItemType.Import, include)
		{
		}
		
		internal ImportProjectItem(IProject project, IProjectItemBackendStore buildItem)
			: base(project, buildItem)
		{
		}
	}
}
