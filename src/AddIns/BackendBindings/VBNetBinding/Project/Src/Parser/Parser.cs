// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Dom.VBNet;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.VBNetBinding
{
	public class TParser : IParser
	{
		///<summary>IParser Interface</summary>
		string[] lexerTags;
		
		public string[] LexerTags {
			get { return lexerTags; }
			set { lexerTags = value; }
		}
		
		public LanguageProperties Language {
			get { return LanguageProperties.VBNet; }
		}
		
		public IExpressionFinder CreateExpressionFinder(string fileName)
		{
			return new VBNetExpressionFinder(ParserService.GetParseInformation(fileName));
		}
		
		public bool CanParse(string fileName)
		{
			return Path.GetExtension(fileName).Equals(".VB", StringComparison.OrdinalIgnoreCase);
		}
		
		public bool CanParse(IProject project)
		{
			return project.Language == "VBNet";
		}
		
		static void RetrieveRegions(ICompilationUnit cu, ICSharpCode.NRefactory.Parser.SpecialTracker tracker)
		{
			Stack<ICSharpCode.NRefactory.PreprocessingDirective> regionStartDirectives = new Stack<ICSharpCode.NRefactory.PreprocessingDirective>();
			
			foreach (ICSharpCode.NRefactory.PreprocessingDirective directive in tracker.CurrentSpecials.OfType<ICSharpCode.NRefactory.PreprocessingDirective>()) {
				if (directive.Cmd.Equals("#region", StringComparison.OrdinalIgnoreCase))
					regionStartDirectives.Push(directive);
				if (directive.Cmd.Equals("#end", StringComparison.OrdinalIgnoreCase)
				    // using StartsWith allows comments at end of line
				    // fixes http://community.sharpdevelop.net/forums/t/12252.aspx
				    && directive.Arg.StartsWith("region", StringComparison.OrdinalIgnoreCase)
				    && regionStartDirectives.Any()) {
					ICSharpCode.NRefactory.PreprocessingDirective start = regionStartDirectives.Pop();
					cu.FoldingRegions.Add(new FoldingRegion(start.Arg.TrimComments().Trim().Trim('"'), DomRegion.FromLocation(start.StartPosition, directive.EndPosition)));
				}
			}
		}
		
		public ICompilationUnit Parse(IProjectContent projectContent, string fileName, ICSharpCode.SharpDevelop.ITextBuffer fileContent)
		{
			using (ICSharpCode.NRefactory.IParser p = ICSharpCode.NRefactory.ParserFactory.CreateParser(ICSharpCode.NRefactory.SupportedLanguage.VBNet, fileContent.CreateReader())) {
				return Parse(p, fileName, projectContent);
			}
		}
		
		ICompilationUnit Parse(ICSharpCode.NRefactory.IParser p, string fileName, IProjectContent projectContent)
		{
			p.Lexer.SpecialCommentTags = lexerTags;
			p.ParseMethodBodies = false;
			p.Parse();
			
			NRefactoryASTConvertVisitor visitor = new NRefactoryASTConvertVisitor(projectContent, ICSharpCode.NRefactory.SupportedLanguage.VBNet);
			if (projectContent.Project != null) {
				visitor.VBRootNamespace = ((IProject)projectContent.Project).RootNamespace;
			}
			visitor.Specials = p.Lexer.SpecialTracker.CurrentSpecials;
			visitor.VisitCompilationUnit(p.CompilationUnit, null);
			visitor.Cu.FileName = fileName;
			visitor.Cu.ErrorsDuringCompile = p.Errors.Count > 0;
			RetrieveRegions(visitor.Cu, p.Lexer.SpecialTracker);
			AddCommentTags(visitor.Cu, p.Lexer.TagComments);
			
			return visitor.Cu;
		}
		
		static void AddCommentTags(ICompilationUnit cu, System.Collections.Generic.List<ICSharpCode.NRefactory.Parser.TagComment> tagComments)
		{
			foreach (ICSharpCode.NRefactory.Parser.TagComment tagComment in tagComments)
			{
				DomRegion tagRegion = new DomRegion(tagComment.StartPosition.Y, tagComment.StartPosition.X);
				var tag = new ICSharpCode.SharpDevelop.Dom.TagComment(tagComment.Tag, tagRegion, tagComment.CommentText);
				cu.TagComments.Add(tag);
			}
		}
		
		public IResolver CreateResolver()
		{
			return new NRefactoryResolver(LanguageProperties.VBNet);
		}
		///////// IParser Interface END
	}
}
