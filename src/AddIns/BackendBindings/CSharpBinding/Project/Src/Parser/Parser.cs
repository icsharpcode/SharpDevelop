// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace CSharpBinding.Parser
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
		
		public bool CanParse(string fileName)
		{
			return Path.GetExtension(fileName).Equals(".CS", StringComparison.OrdinalIgnoreCase);
		}
		
		/*
		void RetrieveRegions(ICompilationUnit cu, ICSharpCode.NRefactory.Parser.SpecialTracker tracker)
		{
			for (int i = 0; i < tracker.CurrentSpecials.Count; ++i) {
				ICSharpCode.NRefactory.PreprocessingDirective directive = tracker.CurrentSpecials[i] as ICSharpCode.NRefactory.PreprocessingDirective;
				if (directive != null) {
					if (directive.Cmd == "#region") {
						int deep = 1;
						for (int j = i + 1; j < tracker.CurrentSpecials.Count; ++j) {
							ICSharpCode.NRefactory.PreprocessingDirective nextDirective = tracker.CurrentSpecials[j] as ICSharpCode.NRefactory.PreprocessingDirective;
							if (nextDirective != null) {
								switch (nextDirective.Cmd) {
									case "#region":
										++deep;
										break;
									case "#endregion":
										--deep;
										if (deep == 0) {
											cu.FoldingRegions.Add(new FoldingRegion(directive.Arg.Trim(), DomRegion.FromLocation(directive.StartPosition, nextDirective.EndPosition)));
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
		 */
		
		public ParseInformation Parse(FileName fileName, ITextSource fileContent, bool fullParseInformationRequested)
		{
			CSharpParser parser = new CSharpParser();
			parser.GenerateTypeSystemMode = !fullParseInformationRequested;
			CompilationUnit cu = parser.Parse(fileContent.CreateReader(), fileName);
			
			CSharpParsedFile file = cu.ToTypeSystem();
			ParseInformation parseInfo;
			
			if (fullParseInformationRequested)
				parseInfo = new CSharpFullParseInformation(file, cu);
			else
				parseInfo = new ParseInformation(file, fullParseInformationRequested);
			
			AddCommentTags(cu, parseInfo.TagComments, fileContent);
			
			return parseInfo;
		}
		
		void AddCommentTags(CompilationUnit cu, IList<TagComment> tagComments, ITextSource fileContent)
		{
			ReadOnlyDocument document = null;
			foreach (var comment in cu.Descendants.OfType<Comment>().Where(c => c.CommentType != CommentType.InactiveCode)) {
				int matchLength;
				int index = IndexOfAny(comment.Content, lexerTags, 0, out matchLength);
				if (index > -1) {
					if (document == null)
						document = new ReadOnlyDocument(fileContent);
					int startOffset = document.GetOffset(comment.StartLocation);
					int commentSignLength = comment.CommentType == CommentType.Documentation || comment.CommentType == CommentType.MultiLineDocumentation ? 3 : 2;
					int commentEndSignLength = comment.CommentType == CommentType.MultiLine || comment.CommentType == CommentType.MultiLineDocumentation ? 2 : 0;
					do {
						int absoluteOffset = startOffset + index + commentSignLength;
						var startLocation = document.GetLocation(absoluteOffset);
						int endOffset = Math.Min(document.GetLineByNumber(startLocation.Line).EndOffset, document.GetOffset(comment.EndLocation) - commentEndSignLength);
						string content = document.GetText(absoluteOffset, endOffset - absoluteOffset);
						tagComments.Add(new TagComment(content.Substring(0, matchLength), new DomRegion(cu.FileName, startLocation.Line, startLocation.Column), content.Substring(matchLength)));
						index = IndexOfAny(comment.Content, lexerTags, endOffset - startOffset - commentSignLength, out matchLength);
					} while (index > -1);
				}
			}
		}
		
		static int IndexOfAny(string haystack, string[] needles, int startIndex, out int matchLength)
		{
			if (haystack == null)
				throw new ArgumentNullException("haystack");
			if (needles == null)
				throw new ArgumentNullException("needles");
			int index = -1;
			matchLength = 0;
			foreach (var needle in needles) {
				int i = haystack.IndexOf(needle, startIndex, StringComparison.Ordinal);
				if (i != -1 && (index == -1 || index > i)) {
					index = i;
					matchLength = needle.Length;
				}
			}
			return index;
		}
		
		public ResolveResult Resolve(ParseInformation parseInfo, TextLocation location, ICompilation compilation, CancellationToken cancellationToken)
		{
			var csParseInfo = parseInfo as CSharpFullParseInformation;
			if (csParseInfo == null)
				throw new ArgumentException("Parse info does not have CompilationUnit");
			
			return ResolveAtLocation.Resolve(compilation, csParseInfo.ParsedFile, csParseInfo.CompilationUnit, location, cancellationToken);
		}
		
		public void FindLocalReferences(ParseInformation parseInfo, ITextSource fileContent, IVariable variable, ICompilation compilation, Action<Reference> callback, CancellationToken cancellationToken)
		{
			var csParseInfo = parseInfo as CSharpFullParseInformation;
			if (csParseInfo == null)
				throw new ArgumentException("Parse info does not have CompilationUnit");
			
			ReadOnlyDocument document = null;
			DocumentHighlighter highlighter = null;
			
			new FindReferences().FindLocalReferences(
				variable, csParseInfo.ParsedFile, csParseInfo.CompilationUnit, compilation,
				delegate (AstNode node, ResolveResult result) {
					if (document == null) {
						document = new ReadOnlyDocument(fileContent);
						var highlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(csParseInfo.FileName));
						if (highlighting != null)
							highlighter = new DocumentHighlighter(document, highlighting.MainRuleSet);
						else
							highlighter = null;
					}
					var region = new DomRegion(parseInfo.FileName, node.StartLocation, node.EndLocation);
					int offset = document.GetOffset(node.StartLocation);
					int length = document.GetOffset(node.EndLocation) - offset;
					var builder = SearchResultsPad.CreateInlineBuilder(node.StartLocation, node.EndLocation, document, highlighter);
					callback(new Reference(region, result, offset, length, builder));
				}, cancellationToken);
		}
	}
}
