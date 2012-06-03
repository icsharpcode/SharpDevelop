// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeParameter : CodeElement
	{
		IParameter parameter;
		
		public CodeParameter(IParameter parameter)
		{
			this.parameter = parameter;
		}
		
		public override vsCMElement Kind {
			get { return vsCMElement.vsCMElementParameter; }
		}
		
		public override string Name {
			get { return parameter.Name; }
		}
	}
}
