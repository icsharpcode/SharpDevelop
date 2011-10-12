// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcModelClassLocator : IMvcModelClassLocator
	{
		IMvcParserService parserService;
		
		public MvcModelClassLocator()
			: this(new MvcParserService())
		{
		}
		
		public MvcModelClassLocator(IMvcParserService parserService)
		{
			this.parserService = parserService;
		}
		
		public IEnumerable<IMvcClass> GetModelClasses(IMvcProject project)
		{
			foreach (IMvcClass c in GetAllClassesInProject(project)) {
				if (c.IsModelClass()) {
					yield return c;
				}
			}
		}
		
		IEnumerable<IMvcClass> GetAllClassesInProject(IMvcProject project)
		{
			IMvcProjectContent projectContent = GetProjectContent(project);
			return projectContent.GetClasses();
		}
		
		IMvcProjectContent GetProjectContent(IMvcProject project)
		{
			return parserService.GetProjectContent(project);
		}
	}
}
