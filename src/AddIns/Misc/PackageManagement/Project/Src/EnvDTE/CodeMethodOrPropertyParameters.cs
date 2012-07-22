// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeParameters : CodeElementsList
	{
		public CodeParameters(IEnumerable<IParameter> parameters)
		{
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
			AddCodeElement(new CodeParameter(parameter));
		}
	}
}
