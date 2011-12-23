// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ServiceReferenceProjectItem : ProjectItem
	{
		public ServiceReferenceProjectItem(IProject project)
			: base(project, ItemType.ServiceReference)
		{
		}
		
		internal ServiceReferenceProjectItem(IProject project, IProjectItemBackendStore buildItem)
			: base(project, buildItem)
		{
		}
	}
}
