// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public static class IParameterExtensions
	{
		public static bool IsIn(this IParameter parameter)
		{
			return (parameter.Modifiers & ParameterModifiers.In) == ParameterModifiers.In;
		}
	}
}
