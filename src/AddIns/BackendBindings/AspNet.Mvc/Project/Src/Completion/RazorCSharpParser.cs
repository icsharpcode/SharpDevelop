// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc.Completion
{
	public class RazorCSharpParser : IParser
	{
		public RazorCSharpParser()
		{
		}
		
		public string[] LexerTags { get; set; }
		
		public LanguageProperties Language {
			get { return LanguageProperties.CSharp; }
		}
		
		public IExpressionFinder CreateExpressionFinder(string fileName)
		{
			return new RazorCSharpExpressionFinder();
		}
		
		public bool CanParse(string fileName)
		{
			return Path.GetExtension(fileName).Equals(".cshtml", StringComparison.OrdinalIgnoreCase);
		}
		
		public bool CanParse(IProject project)
		{
			return project.Language == "C#";
		}
		
		public ICompilationUnit Parse(IProjectContent projectContent, string fileName, ITextBuffer fileContent)
		{
			var modelTypeLocater = new RazorCSharpModelTypeLocater(fileContent);
			return new RazorCompilationUnit(projectContent) {
				ModelTypeName = modelTypeLocater.ModelTypeName
			};
		}
		
		public IResolver CreateResolver()
		{
			return new RazorCSharpResolver();
		}
	}
}
