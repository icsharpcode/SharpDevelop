// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeSetterFunction : CodeGetterOrSetterFunction
	{
		public CodeSetterFunction(IProperty property)
			: base(property, property.SetterModifiers)
		{
		}
	}
}
