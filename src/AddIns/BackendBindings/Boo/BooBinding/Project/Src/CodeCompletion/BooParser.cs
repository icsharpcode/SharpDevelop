// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
using ICSharpCode.SharpDevelop;
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
			return project.Language == BooProjectBinding.LanguageName;
		}
		
		public ICompilationUnit Parse(IProjectContent projectContent, string fileName, ITextBuffer fileContentBuffer)
		{
			string fileContent = fileContentBuffer.Text;
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
			ICompilationUnit cu;
			lock (typeof(BooCompiler)) {
				BooCompiler compiler = new BooCompiler();
				compiler.Parameters.Input.Add(new StringInput(fileName, fileContent));
				cu = Parse(projectContent, fileName, lineLength, compiler);
			}
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
			
			// Compile pipeline as of Boo 0.9.4:
			// Boo.Lang.Compiler.Pipelines.Parse:
			//   Boo.Lang.Compiler.Steps.Parsing
			// Boo.Lang.Compiler.Pipelines.ExpandMacros:
			//   Boo.Lang.Compiler.Steps.PreErrorChecking
			//   Boo.Lang.Compiler.Steps.MergePartialClasses
			//   Boo.Lang.Compiler.Steps.InitializeNameResolutionService
			//   Boo.Lang.Compiler.Steps.IntroduceGlobalNamespaces
			//   Boo.Lang.Compiler.Steps.TransformCallableDefinitions
			//   Boo.Lang.Compiler.Steps.BindTypeDefinitions
			//   Boo.Lang.Compiler.Steps.BindGenericParameters
			//   Boo.Lang.Compiler.Steps.ResolveImports
			//   Boo.Lang.Compiler.Steps.BindBaseTypes
			//   Boo.Lang.Compiler.Steps.MacroAndAttributeExpansion
			// Boo.Lang.Compiler.Pipelines.ResolveExpressions:
			//   Boo.Lang.Compiler.Steps.ExpandAstLiterals
			//   Boo.Lang.Compiler.Steps.IntroduceModuleClasses
			//   Boo.Lang.Compiler.Steps.NormalizeStatementModifiers
			//   Boo.Lang.Compiler.Steps.NormalizeTypeAndMemberDefinitions
			//   Boo.Lang.Compiler.Steps.NormalizeExpressions
			//   Boo.Lang.Compiler.Steps.BindTypeDefinitions
			//   Boo.Lang.Compiler.Steps.BindGenericParameters
			//   Boo.Lang.Compiler.Steps.BindEnumMembers
			//   Boo.Lang.Compiler.Steps.BindBaseTypes
			//   Boo.Lang.Compiler.Steps.CheckMemberTypes
			//   Boo.Lang.Compiler.Steps.BindMethods
			//   Boo.Lang.Compiler.Steps.ResolveTypeReferences
			//   Boo.Lang.Compiler.Steps.BindTypeMembers
			//   Boo.Lang.Compiler.Steps.CheckGenericConstraints
			//   Boo.Lang.Compiler.Steps.ProcessInheritedAbstractMembers
			//   Boo.Lang.Compiler.Steps.CheckMemberNames
			//   Boo.Lang.Compiler.Steps.ProcessMethodBodiesWithDuckTyping
			//   Boo.Lang.Compiler.Steps.ReifyTypes
			//   Boo.Lang.Compiler.Steps.VerifyExtensionMethods
			//   Boo.Lang.Compiler.Steps.TypeInference
			// Boo.Lang.Compiler.Pipelines.Compile:
			//   Boo.Lang.Compiler.Steps.ConstantFolding
			//   Boo.Lang.Compiler.Steps.CheckLiteralValues
			//   Boo.Lang.Compiler.Steps.OptimizeIterationStatements
			//   Boo.Lang.Compiler.Steps.BranchChecking
			//   Boo.Lang.Compiler.Steps.CheckIdentifiers
			//   Boo.Lang.Compiler.Steps.StricterErrorChecking
			//   Boo.Lang.Compiler.Steps.CheckAttributesUsage
			//   Boo.Lang.Compiler.Steps.ExpandDuckTypedExpressions
			//   Boo.Lang.Compiler.Steps.ProcessAssignmentsToValueTypeMembers
			//   Boo.Lang.Compiler.Steps.ExpandPropertiesAndEvents
			//   Boo.Lang.Compiler.Steps.CheckMembersProtectionLevel
			//   Boo.Lang.Compiler.Steps.NormalizeIterationStatements
			//   Boo.Lang.Compiler.Steps.ProcessSharedLocals
			//   Boo.Lang.Compiler.Steps.ProcessClosures
			//   Boo.Lang.Compiler.Steps.ProcessGenerators
			//   Boo.Lang.Compiler.Steps.ExpandVarArgsMethodInvocations
			//   Boo.Lang.Compiler.Steps.InjectCallableConversions
			//   Boo.Lang.Compiler.Steps.ImplementICallableOnCallableDefinitions
			//   Boo.Lang.Compiler.Steps.RemoveDeadCode
			//   Boo.Lang.Compiler.Steps.CheckNeverUsedMembers
			// Boo.Lang.Compiler.Pipelines.CompileToMemory:
			//   Boo.Lang.Compiler.Steps.EmitAssembly
			// Boo.Lang.Compiler.Pipelines.CompileToFile:
			//   Boo.Lang.Compiler.Steps.SaveAssembly
			
			
			CompilerPipeline compilePipe = new Parse();
			compilePipe.Add(new PreErrorChecking());
			compilePipe.Add(new MergePartialClasses());
			compilePipe.Add(new InitializeNameResolutionService());
			compilePipe.Add(new IntroduceGlobalNamespaces());
			// TransformCallableDefinitions: not used for CC
			compilePipe.Add(new BindTypeDefinitions());
			compilePipe.Add(new BindGenericParameters());
			compilePipe.Add(new ResolveImports());
			compilePipe.Add(new BindBaseTypes());
			compilePipe.Add(new MacroAndAttributeExpansion());
			compilePipe.Add(new IntroduceModuleClasses());
			
			var parsingStep = compilePipe[0];
			//compiler.Parameters.Environment.Provide<ParserSettings>().TabSize = 1;
			
			ConvertVisitor visitor = new ConvertVisitor(lineLength, projectContent);
			visitor.Cu.FileName = fileName;
			compilePipe.Add(visitor);
			
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
				MessageService.ShowException(ex);
			}
			return visitor.Cu;
		}
		
		public IResolver CreateResolver()
		{
			return new BooResolver();
		}
	}
}
