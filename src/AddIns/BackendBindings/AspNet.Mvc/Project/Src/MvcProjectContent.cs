// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcProjectContent : IMvcProjectContent
	{
		ICompilation compilation;
		IMvcProject project;
		
		public MvcProjectContent(ICompilation compilation, IMvcProject project)
		{
			this.compilation = compilation;
			this.project = project;
		}
		
		public IEnumerable<IMvcClass> GetClasses()
		{
			foreach (ITypeDefinition type in compilation.MainAssembly.TopLevelTypeDefinitions) {
				yield return new MvcClass(type, project);
			}
		}
	}
}
