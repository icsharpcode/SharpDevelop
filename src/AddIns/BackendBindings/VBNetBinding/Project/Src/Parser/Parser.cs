// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Text;
using System.IO;
using System.Drawing;
using System.Collections;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
//using ICSharpCode.NRefactory.Parser;

namespace VBNetBinding.Parser
{
	public class TParser : IParser
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
		
		public IExpressionFinder ExpressionFinder {
			get {
				return new ExpressionFinder();
			}
		}
		
		public bool CanParse(string fileName)
		{
			return Path.GetExtension(fileName).ToUpper() == ".CS";
		}
		
		public bool CanParse(IProject project)
		{
			return project.Language == "C#";
		}
		
		void RetrieveRegions(ICompilationUnit cu, ICSharpCode.NRefactory.Parser.SpecialTracker tracker)
		{
			for (int i = 0; i < tracker.CurrentSpecials.Count; ++i) {
				ICSharpCode.NRefactory.Parser.PreProcessingDirective directive = tracker.CurrentSpecials[i] as ICSharpCode.NRefactory.Parser.PreProcessingDirective;
				if (directive != null) {
					if (directive.Cmd == "#region") {
						int deep = 1; 
						for (int j = i + 1; j < tracker.CurrentSpecials.Count; ++j) {
							ICSharpCode.NRefactory.Parser.PreProcessingDirective nextDirective = tracker.CurrentSpecials[j] as ICSharpCode.NRefactory.Parser.PreProcessingDirective;
							if (nextDirective != null) {
								switch (nextDirective.Cmd) {
									case "#region":
										++deep;
										break;
									case "#endregion":
										--deep;
										if (deep == 0) {
											cu.FoldingRegions.Add(new FoldingRegion(directive.Arg.Trim(), new DefaultRegion(directive.Start, new Point(nextDirective.End.X, nextDirective.End.Y))));
											goto end;
										}
										break;
								}
							}
						}
						end: ;
					}
				}
			}
		}
		
		public ICompilationUnitBase Parse(string fileName)
		{
//			ICSharpCode.NRefactory.Parser.Parser p = new ICSharpCode.NRefactory.Parser.Parser();
//			
//			Lexer lexer = new Lexer(new FileReader(fileName));
//			lexer.SpecialCommentTags = lexerTags;
//			p.Parse(lexer);
			
			Properties textEditorProperties = ((Properties)PropertyService.Get("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new Properties()));
			ICSharpCode.NRefactory.Parser.IParser p = ICSharpCode.NRefactory.Parser.ParserFactory.CreateParser(fileName, Encoding.GetEncoding(textEditorProperties.Get("Encoding", 1252)));
			p.Lexer.SpecialCommentTags = lexerTags;
			p.Parse();
			
			NRefactoryASTConvertVisitor visitor = new NRefactoryASTConvertVisitor();
			visitor.Visit(p.CompilationUnit, null);
			visitor.Cu.FileName = fileName;
			visitor.Cu.ErrorsDuringCompile = p.Errors.count > 0;
			RetrieveRegions(visitor.Cu, p.Lexer.SpecialTracker);
			AddCommentTags(visitor.Cu, p.Lexer.TagComments);
			return visitor.Cu;
		}
		
		public ICompilationUnitBase Parse(string fileName, string fileContent)
		{
//			ICSharpCode.NRefactory.Parser.Parser p = new ICSharpCode.NRefactory.Parser.Parser();
//			
//			Lexer lexer = new Lexer(new StringReader(fileContent));
//			lexer.SpecialCommentTags = lexerTags;
//			p.Parse(lexer);
			
			ICSharpCode.NRefactory.Parser.IParser p = ICSharpCode.NRefactory.Parser.ParserFactory.CreateParser(ICSharpCode.NRefactory.Parser.SupportedLanguages.CSharp, new StringReader(fileContent));
			p.Lexer.SpecialCommentTags = lexerTags;
			p.Parse();
			
			NRefactoryASTConvertVisitor visitor = new NRefactoryASTConvertVisitor();
			visitor.Visit(p.CompilationUnit, null);
			visitor.Cu.FileName = fileName;
			visitor.Cu.ErrorsDuringCompile = p.Errors.count > 0;
			visitor.Cu.Tag = p.CompilationUnit;
			
			RetrieveRegions(visitor.Cu, p.Lexer.SpecialTracker);
			AddCommentTags(visitor.Cu, p.Lexer.TagComments);
			return visitor.Cu;
		}
		
		void AddCommentTags(ICompilationUnit cu, ArrayList tagComments)
		{
			foreach (ICSharpCode.NRefactory.Parser.TagComment tagComment in tagComments) {
				DefaultRegion tagRegion = new DefaultRegion(tagComment.StartPosition.Y, tagComment.StartPosition.X);
				ICSharpCode.SharpDevelop.Dom.Tag tag = new ICSharpCode.SharpDevelop.Dom.Tag(tagComment.Tag, tagRegion);
				tag.CommentString = tagComment.CommentText;
				cu.TagComments.Add(tag);
			}
		}
		
		public IResolver CreateResolver()
		{
			return new  ICSharpCode.SharpDevelop.Dom.NRefactoryResolver.NRefactoryResolver(ICSharpCode.NRefactory.Parser.SupportedLanguages.CSharp);
		}
		///////// IParser Interface END
	}
}
