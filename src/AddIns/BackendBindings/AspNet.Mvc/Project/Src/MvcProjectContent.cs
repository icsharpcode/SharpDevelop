// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcProjectContent : IMvcProjectContent
	{
		IProjectContent projectContent;
		IMvcProject project;
		
		public MvcProjectContent(IProjectContent projectContent, IMvcProject project)
		{
			this.projectContent = projectContent;
			this.project = project;
		}
		
		public IEnumerable<IMvcClass> GetClasses()
		{
			foreach (IClass c in projectContent.Classes) {
				yield return new MvcClass(c, project);
			}
		}
	}
}
