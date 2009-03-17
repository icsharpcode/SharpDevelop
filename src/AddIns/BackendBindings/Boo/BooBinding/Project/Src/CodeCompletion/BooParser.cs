// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Parser;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
namespace Grunwald.BooBinding.CodeCompletion
{
	public class BooParser : IParser
	{
		string[] lexerTags;
		
		public string[] LexerTags {
			get {
				return lexerTags;
			}
			set {
				lexerTags = value;
			}
		}
		
		public LanguageProperties Language {
			get {
				return BooLanguageProperties.Instance;
			}
		}
		
		public IExpressionFinder CreateExpressionFinder(string fileName)
		{
			return new ExpressionFinder(fileName);
		}
		
		public bool CanParse(string fileName)
		{
			return string.Equals(Path.GetExtension(fileName), ".boo", StringComparison.OrdinalIgnoreCase);
		}
		
		public bool CanParse(IProject project)
		{
			return project.Language == BooLanguageBinding.LanguageName;
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
			ICompilationUnit cu = Parse(projectContent, fileName, lineLength, compiler);
			AddCommentsAndRegions(cu, fileContent, fileName);
			return cu;
		}
		
		void AddCommentsAndRegions(ICompilationUnit cu, string fileContent, string fileName)
		{
			ExpressionFinder ef = new ExpressionFinder(fileName);
			ef.ResetStateMachine();
			int state = 0;
			StringBuilder commentBuilder = null;
			char commentStartChar = '\0';
			int commentStartColumn = 0;
			
			Stack<string> regionTitleStack  = new Stack<string>();
			Stack<int>    regionLineStack   = new Stack<int>();
			Stack<int>    regionColumnStack = new Stack<int>();
			
			int line = 1;
			int column = 0;
			for (int i = 0; i < fileContent.Length; i++) {
				column += 1;
				char c = fileContent[i];
				if (c == '\n') {
					column = 0;
					line += 1;
				}
				state = ef.FeedStateMachine(state, c);
				if (state == ExpressionFinder.PossibleRegexStart) {
					// after / could be a regular expression, do a special check for that
					int regexEnd = ef.SkipRegularExpression(fileContent, i, fileContent.Length - 1);
					if (regexEnd > 0) {
						i = regexEnd;
					} else if (regexEnd == -1) {
						// end of file is in regex
						return;
					} // else: regexEnd is 0 if its not a regex
				}
				if (state == ExpressionFinder.LineCommentState) {
					if (commentBuilder == null) {
						commentStartChar = c;
						commentStartColumn = column;
						commentBuilder = new StringBuilder();
					} else {
						if (commentBuilder.Length > 0) {
							commentBuilder.Append(c);
						} else if (!char.IsWhiteSpace(c)) {
							commentStartColumn = column;
							commentBuilder.Append(c);
						}
					}
				} else if (commentBuilder != null) {
					string text = commentBuilder.ToString();
					commentBuilder = null;
					if (commentStartChar == '#' && text.StartsWith("region ")) {
						regionTitleStack.Push(text.Substring(7));
						regionLineStack.Push(line);
						regionColumnStack.Push(commentStartColumn - 1);
					} else if (commentStartChar == '#' && text.StartsWith("endregion") && regionTitleStack.Count > 0) {
						// Add folding region
						cu.FoldingRegions.Add(new FoldingRegion(regionTitleStack.Pop(),
						                                        new DomRegion(regionLineStack.Pop(),
						                                                      regionColumnStack.Pop(),
						                                                      line, column)));
					} else {
						foreach (string tag in lexerTags) {
							if (text.StartsWith(tag)) {
								string commentString = text.Substring(tag.Length);
								cu.TagComments.Add(new TagComment(tag, new DomRegion(line, commentStartColumn), commentString));
								break;
							}
						}
					}
				}
			}
		}
		
		private ICompilationUnit Parse(IProjectContent projectContent, string fileName, int[] lineLength, BooCompiler compiler)
		{
			compiler.Parameters.OutputWriter = new StringWriter();
			compiler.Parameters.TraceLevel = System.Diagnostics.TraceLevel.Off;
			
			Compile compilePipe = new Compile();
			BooParsingStep parsingStep = (BooParsingStep)compilePipe[0];
			parsingStep.TabSize = 1;
			
			ConvertVisitor visitor = new ConvertVisitor(lineLength, projectContent);
			visitor.Cu.FileName = fileName;
			
			// Remove unneccessary compiler steps
			int num = 1 + compilePipe.Find(typeof(NormalizeStatementModifiers));
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
