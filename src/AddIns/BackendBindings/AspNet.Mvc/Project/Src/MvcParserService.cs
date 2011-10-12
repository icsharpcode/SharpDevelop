// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcParserService : IMvcParserService
	{
		public IMvcProjectContent GetProjectContent(IMvcProject project)
		{
			IProjectContent projectContent = GetProjectContentFromParser(project);
			return new MvcProjectContent(projectContent, project);
		}
		
		IProjectContent GetProjectContentFromParser(IMvcProject mvcProject)
		{
			return ParserService.GetProjectContent(mvcProject.Project);
		}
	}
}
