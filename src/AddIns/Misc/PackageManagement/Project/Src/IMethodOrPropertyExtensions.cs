// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement
{
	public static class IMethodOrPropertyExtensions
	{
		public static FilePosition GetStartPosition(this IMethodOrProperty method)
		{
			return method.Region.ToStartPosition(method.CompilationUnit);
		}
		
		public static FilePosition GetEndPosition(this IMethodOrProperty method)
		{
			if (method.DeclaringTypeIsInterface()) {
				return method.Region.ToEndPosition(method.CompilationUnit);
			}
			return method.BodyRegion.ToEndPosition(method.CompilationUnit);
		}
		
		public static bool DeclaringTypeIsInterface(this IMethodOrProperty method)
		{
			return method.DeclaringType.ClassType == ClassType.Interface;
		}
	}
}
