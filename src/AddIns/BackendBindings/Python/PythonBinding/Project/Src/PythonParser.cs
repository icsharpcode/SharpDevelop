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

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;
using System;
using System.IO;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using IronPython;
using IronPython.Compiler;
using IronPython.Compiler.Ast;
using IronPython.Runtime;
using IronPython.Hosting;
using IronPython.Runtime.Exceptions;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Python parser.
	/// </summary>
	public class PythonParser : IParser
	{
		string[] lexerTags = new string[0];
		ScriptEngine scriptEngine;
		
		public string[] LexerTags {
			get { return lexerTags;	}
			set { lexerTags = value; }
		}
		
		public LanguageProperties Language {
			get { return LanguageProperties.None; }
		}
		
		public IExpressionFinder CreateExpressionFinder(string fileName)
		{
			return new PythonExpressionFinder();
		}
		
		/// <summary>
		/// Returns true if the filename has a .py extension.
		/// </summary>
		public bool CanParse(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (extension != null) {
				return extension.ToLower() == ".py";
			}
			return false;
		}
		
		/// <summary>
		/// Returns true if the project's language is python.
		/// </summary>
		public bool CanParse(IProject project)
		{
			if (project != null) {
				return project.Language == PythonProjectBinding.LanguageName;
			}
			return false;
		}
		
		/// <summary>
		/// Parses a python file and creates a PythonAst.
		/// </summary>
		public PythonAst CreateAst(string fileName, ITextBuffer textBuffer)
		{
			return CreateAst(fileName, textBuffer.Text);
		}
		
		/// <summary>
		/// Parses a python file and creates a PythonAst.
		/// </summary>
		public PythonAst CreateAst(string fileName, string fileContent)
		{
			if (scriptEngine == null) {
				scriptEngine = IronPython.Hosting.Python.CreateEngine();
			}

			PythonCompilerSink sink = new PythonCompilerSink();
			SourceUnit source = DefaultContext.DefaultPythonContext.CreateFileUnit(fileName, fileContent);
			CompilerContext context = new CompilerContext(source, new PythonCompilerOptions(), sink);
			using (Parser parser = Parser.CreateParser(context, new PythonOptions())) {
				return parser.ParseFile(false);	
			}
		}
		
		/// <summary>
		/// Parses the python code and returns an ICompilationUnit.
		/// </summary>
		public ICompilationUnit Parse(IProjectContent projectContent, string fileName, ITextBuffer textBuffer)
		{
			string fileContent = GetFileContent(textBuffer);
			return Parse(projectContent, fileName, fileContent);
		}
		
		string GetFileContent(ITextBuffer textBuffer)
		{
			if (textBuffer != null) {
				return textBuffer.Text;
			}
			return null;	
		}
		
		/// <summary>
		/// Parses the python code and returns an ICompilationUnit.
		/// </summary>
		public ICompilationUnit Parse(IProjectContent projectContent, string fileName, string fileContent)
		{
			if (fileContent != null) {
				try { 
					PythonAst ast = CreateAst(fileName, fileContent);
					PythonAstWalker walker = new PythonAstWalker(projectContent, fileName);
					walker.Walk(ast);
					return walker.CompilationUnit;
				} catch (InvalidCastException) {
					// Ignore.
				}
			}
			
			DefaultCompilationUnit compilationUnit = new DefaultCompilationUnit(projectContent);
			compilationUnit.FileName = fileName;
			return compilationUnit;
		}
		
		public IResolver CreateResolver()
		{
			return new PythonResolver();
		}
	}
}
