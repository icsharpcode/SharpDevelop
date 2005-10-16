// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Parser;

namespace Grunwald.BooBinding.CodeCompletion
{
	public class BooParser : IParser
	{
		///<summary>IParser Interface</summary>
		string[] lexerTags;
		
		public string[] LexerTags {
			get {
				return lexerTags;
			}
			set {
				lexerTags = value;
			}
		}
		
		public IExpressionFinder CreateExpressionFinder(string fileName)
		{
			return new ExpressionFinder(fileName);
		}
		
		public bool CanParse(string fileName)
		{
			return string.Equals(Path.GetExtension(fileName), ".boo", StringComparison.InvariantCultureIgnoreCase);
		}
		
		public bool CanParse(IProject project)
		{
			return project.Language == BooLanguageBinding.LanguageName;
		}
		
		public ICompilationUnit Parse(IProjectContent projectContent, string fileName)
		{
			string content;
			using (StreamReader reader = new StreamReader(fileName)) {
				content = reader.ReadToEnd();
			}
			return Parse(projectContent, fileName, content);
		}
		
		public ICompilationUnit Parse(IProjectContent projectContent, string fileName, string fileContent)
		{
			LoggingService.Debug("Parse " + fileName);
			int lineCount = 1;
			foreach (char c in fileContent) {
				if (c == '\n') {
					lineCount++;
				}
			}
			int[] lineLength = new int[lineCount];
			int length = 0;
			int i = 0;
			foreach (char c in fileContent) {
				if (c == '\n') {
					lineLength[i] = length;
					i += 1;
					length = 0;
				} else if (c != '\r') {
					length += 1;
				}
			}
			lineLength[i] = length;
			BooCompiler compiler = new BooCompiler();
			compiler.Parameters.Input.Add(new StringInput(fileName, fileContent));
			return Parse(projectContent, fileName, lineLength, compiler);
		}
		
		private ICompilationUnit Parse(IProjectContent projectContent, string fileName, int[] lineLength, BooCompiler compiler)
		{
			compiler.Parameters.OutputWriter = new StringWriter();
			compiler.Parameters.TraceSwitch.Level = System.Diagnostics.TraceLevel.Warning;
			
			Compile compilePipe = new Compile();
			BooParsingStep parsingStep = (BooParsingStep)compilePipe[0];
			parsingStep.TabSize = 1;
			
			ConvertVisitor visitor = new ConvertVisitor(lineLength, projectContent);
			visitor.Cu.FileName = fileName;
			
			// Remove unneccessary compiler steps
			int num = 1 + compilePipe.Find(typeof(NormalizeTypeAndMemberDefinitions));
			compilePipe[num] = visitor;
			while (compilePipe.Count > num + 1)
				compilePipe.RemoveAt(compilePipe.Count - 1);
			num = compilePipe.Find(typeof(TransformCallableDefinitions));
			compilePipe.RemoveAt(num);
			
			//for (int i = 0; i < compilePipe.Count; i++) {
			//	Console.WriteLine(compilePipe[i]);
			//}
			
			compilePipe.BreakOnErrors = false;
			compiler.Parameters.Pipeline = compilePipe;
			compiler.Parameters.References.Add(typeof(Boo.Lang.Useful.Attributes.SingletonAttribute).Assembly);
			
			int errorCount = 0;
			compilePipe.AfterStep += delegate(object sender, CompilerStepEventArgs args) {
				if (args.Step == parsingStep)
					errorCount = args.Context.Errors.Count;
			};
			try {
				compiler.Run();
				visitor.Cu.ErrorsDuringCompile = errorCount > 0;
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
			return visitor.Cu;
		}
		
		public IResolver CreateResolver()
		{
			return new BooResolver();
		}
	}
}
