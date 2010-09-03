// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	public class WixLibraryProjectItem : ProjectItem
	{
		public WixLibraryProjectItem(IProject project)
			: base(project, WixItemType.Library)
		{
		}
		
		public WixLibraryProjectItem(IProject project, IProjectItemBackendStore item)
			: base(project, item)
		{
		}
	}
}
