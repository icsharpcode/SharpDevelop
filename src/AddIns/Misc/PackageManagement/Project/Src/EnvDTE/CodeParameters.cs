// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeParameters : CodeElementsList
	{
		IProjectContent projectContent;
		
		public CodeParameters(IProjectContent projectContent, IEnumerable<IParameter> parameters)
		{
			this.projectContent = projectContent;
			AddParameters(parameters);
		}
		
		void AddParameters(IEnumerable<IParameter> parameters)
		{
			foreach (IParameter parameter in parameters) {
				AddParameters(parameter);
			}
		}
		
		void AddParameters(IParameter parameter)
		{
			AddCodeElement(new CodeParameter2(projectContent, parameter));
		}
	}
}
