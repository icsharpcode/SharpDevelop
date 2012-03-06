// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.JavaScriptBinding
{
	public class JavaScriptParser : IParser
	{
		public JavaScriptParser()
		{
			LexerTags = new string[0];
		}
		
		public string[] LexerTags { get; set; }
		
		public LanguageProperties Language {
			get { return LanguageProperties.None; }
		}
		
		public IExpressionFinder CreateExpressionFinder(string fileName)
		{
			return null;
		}
		
		public bool CanParse(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (extension != null) {
				return IsJavaScriptFileExtension(extension);
			}
			return false;
		}
		
		bool IsJavaScriptFileExtension(string extension)
		{
			return extension.Equals(".js", StringComparison.InvariantCultureIgnoreCase);
		}
		
		public bool CanParse(IProject project)
		{
			return true;
		}
		
		public ICompilationUnit Parse(IProjectContent projectContent, string fileName, ITextBuffer fileContent)
		{
			try {
				var astFactory = new JavaScriptAstFactory(fileContent);
				JavaScriptAst ast = astFactory.Create();
				
				var unit = new JavaScriptCompilationUnit(projectContent, fileName);
				var walker = new JavaScriptAstWalker(unit, ast);
				walker.Walk();
				
				return unit;
			} catch (Exception ex) {
				LoggingService.Debug(ex.ToString());
			}
			
			return new DefaultCompilationUnit(projectContent) { FileName = fileName };
		}
		
		public IResolver CreateResolver()
		{
			return null;
		}
	}
}
