// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcParserService : IMvcParserService
	{
		public IMvcProjectContent GetProjectContent(IMvcProject project)
		{
			ICompilation compilation = GetProjectContentFromParser(project);
			return new MvcProjectContent(compilation, project);
		}
		
		ICompilation GetProjectContentFromParser(IMvcProject mvcProject)
		{
			return SD.ParserService.GetCompilation(mvcProject.Project);
		}
	}
}
