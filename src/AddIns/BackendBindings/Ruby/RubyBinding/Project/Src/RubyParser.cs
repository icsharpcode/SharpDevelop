// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Providers;
using Microsoft.Scripting.Runtime;
using System;
using System.IO;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using IronRuby;
using IronRuby.Compiler;
using IronRuby.Compiler.Ast;
using IronRuby.Runtime;
using IronRuby.Hosting;

namespace ICSharpCode.RubyBinding
{
	public class RubyParser : IParser
	{
		string[] lexerTags = new string[0];
		ScriptEngine scriptEngine;
		SourceUnit sourceUnit;

		public RubyParser()
		{
		}
		
		public string[] LexerTags {
			get { return lexerTags;	}
			set { lexerTags = value; }
		}
		
		public LanguageProperties Language {
			get { return LanguageProperties.None; }
		}
		
		public IExpressionFinder CreateExpressionFinder(string fileName)
		{
			return null;
		}
		
		/// <summary>
		/// Returns true if the file extension is .rb
		/// </summary>
		public bool CanParse(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (extension != null) {
				return extension.ToLower() == ".rb";
			}
			return false;
		}
		
		/// <summary>
		/// Returns true if the project's language is Ruby.
		/// </summary>
		public bool CanParse(IProject project)
		{
			if (project != null) {
				return project.Language == RubyLanguageBinding.LanguageName;
			}
			return false;
		}
		
		public SourceUnitTree CreateAst(string fileName, string fileContent)
		{
			if (scriptEngine == null) {
				scriptEngine = Ruby.CreateEngine();
			}
			
			RubyContext rubyContext = HostingHelpers.GetLanguageContext(scriptEngine) as RubyContext;
			sourceUnit = rubyContext.CreateFileUnit(fileName, fileContent);
			RubyCompilerSink sink = new RubyCompilerSink();
			RubyCompilerOptions compilerOptions = new RubyCompilerOptions((RubyOptions)rubyContext.Options);
			Parser parser = new Parser();
			return parser.Parse(sourceUnit, compilerOptions, sink);
		}		
		
		public ICompilationUnit Parse(IProjectContent projectContent, string fileName, string fileContent)
		{
			if (fileContent != null) { 
				try {
					SourceUnitTree ast = CreateAst(fileName, fileContent);
					RubyAstWalker walker = new RubyAstWalker(projectContent, fileName, sourceUnit);
					walker.Walk(ast);
					return walker.CompilationUnit;
				} catch (SyntaxErrorException) {
					// Ignore.
				}
			}
			
			DefaultCompilationUnit compilationUnit = new DefaultCompilationUnit(projectContent);
			compilationUnit.FileName = fileName;
			return compilationUnit;			
		}
		
		public IResolver CreateResolver()
		{
			return null;
		}
	}
}
