// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement
{
	public static class IFieldExtensions
	{
		public static FilePosition GetStartPosition(this IField field)
		{
			return field.Region.ToStartPosition(field.CompilationUnit);
		}
		
		public static FilePosition GetEndPosition(this IField field)
		{
			return field.Region.ToEndPosition(field.CompilationUnit);
		}
	}
}
