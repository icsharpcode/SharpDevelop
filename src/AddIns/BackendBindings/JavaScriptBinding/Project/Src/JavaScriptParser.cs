// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
