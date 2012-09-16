// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.AspNet.Mvc.Completion
{
	public class RazorCompilationUnit : DefaultCompilationUnit
	{
		public RazorCompilationUnit(IProjectContent projectContent)
			: base(projectContent)
		{
		}
		
		public static RazorCompilationUnit CreateFromParseInfo(ParseInformation parseInformation)
		{
			return new RazorCompilationUnit(parseInformation.CompilationUnit.ProjectContent) {
				ModelTypeName = GetModelTypeName(parseInformation.CompilationUnit)
			};
		}
		
		static string GetModelTypeName(ICompilationUnit compilationUnit)
		{
			var originalRazorCompilationUnit = compilationUnit as RazorCompilationUnit;
			if (originalRazorCompilationUnit != null) {
				return originalRazorCompilationUnit.ModelTypeName;
			}
			return String.Empty;
		}
		
		public string ModelTypeName { get; set; }
	}
}
