// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement
{
	public static class DomRegionExtensions
	{
		public static FilePosition ToStartPosition(this DomRegion region, ICompilationUnit compilationUnit)
		{
			return new FilePosition(compilationUnit, region.BeginLine, region.BeginColumn);
		}
		
		public static FilePosition ToEndPosition(this DomRegion region, ICompilationUnit compilationUnit)
		{
			return new FilePosition(compilationUnit, region.EndLine, region.EndColumn);
		}
	}
}
