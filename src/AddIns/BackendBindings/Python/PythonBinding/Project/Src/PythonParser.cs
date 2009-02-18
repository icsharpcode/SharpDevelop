// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;
using System;
using System.IO;

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
		
		public PythonParser()
		{
		}
		
		public string[] LexerTags {
			get { return lexerTags;	}
			set { lexerTags = value; }
		}
		
		public LanguageProperties Language {
			get { return LanguageProperties.None; }
		}
		
		
		/// <summary>
		/// Creates a PythonExpressionFinder.
		/// </summary>
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
				return project.Language == PythonLanguageBinding.LanguageName;
			}
			return false;
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
		
		/// <summary>
		/// Creates a new PythonResolver.
		/// </summary>
		public IResolver CreateResolver()
		{
			return new PythonResolver();
		}
	}
}
