// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Reference3 : Reference, global::EnvDTE.Reference3
	{
		public Reference3(Project project, ReferenceProjectItem referenceProjectItem)
			: base(project, referenceProjectItem)
		{
		}
		
		public bool AutoReferenced { get; private set; }
	}
}
