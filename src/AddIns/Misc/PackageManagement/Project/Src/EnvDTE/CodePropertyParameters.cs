// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodePropertyParameters : CodeElementsList
	{
		IProperty property;
		
		public CodePropertyParameters(IProperty property)
		{
			this.property = property;
			AddParameters();
		}
		
		void AddParameters()
		{
			foreach (IParameter parameter in property.Parameters) {
				AddParameters(parameter);
			}
		}
		
		void AddParameters(IParameter parameter)
		{
			AddCodeElement(new CodeParameter(parameter));
		}
	}
}
