// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingAssemblyParserService : IAssemblyParserService
	{
		public IReflectionProjectContent GetReflectionProjectContentForReference(ReferenceProjectItem item)
		{
			IProjectContent projectContent = AssemblyParserService.GetProjectContentForReference(item);
			return new TextTemplatingReflectionProjectContent(projectContent);
		}
	}
}
