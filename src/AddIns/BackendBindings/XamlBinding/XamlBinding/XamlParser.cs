// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Parses xaml files to partial classes for the Dom.
	/// </summary>
	public class XamlParser : IParser
	{
		public IReadOnlyList<string> TaskListTokens { get; set; }

//		public LanguageProperties Language
//		{
//			get { return LanguageProperties.CSharp; }
//		}
		
		public XamlParser()
		{
		}

		public bool CanParse(string fileName)
		{
			return Path.GetExtension(fileName).Equals(".xaml", StringComparison.OrdinalIgnoreCase);
		}

		public bool CanParse(IProject project)
		{
			return false;
		}
		
		volatile IncrementalParserState parserState;
		
		public ParseInformation Parse(FileName fileName, ITextSource fileContent, bool fullParseInformationRequested,
		                              IProject parentProject, CancellationToken cancellationToken)
		{
			AXmlParser parser = new AXmlParser();
			AXmlDocument document;
			IncrementalParserState newParserState;
			if (fileContent.Version is OnDiskTextSourceVersion) {
				document = parser.Parse(fileContent, cancellationToken);
				newParserState = null;
			} else {
				document = parser.ParseIncremental(parserState, fileContent, out newParserState, cancellationToken);
			}
			parserState = newParserState;
			XamlUnresolvedFile unresolvedFile = XamlUnresolvedFile.Create(fileName, fileContent, document);
			ParseInformation parseInfo;
			if (fullParseInformationRequested)
				parseInfo = new XamlFullParseInformation(unresolvedFile, document, fileContent.CreateSnapshot());
			else
				parseInfo = new ParseInformation(unresolvedFile, fileContent.Version, false);
			AddTagComments(document, parseInfo, fileContent);
			return parseInfo;
		}
		
		void AddTagComments(AXmlDocument xmlDocument, ParseInformation parseInfo, ITextSource fileContent)
		{
			IDocument document = null;
			foreach (var tag in TreeTraversal.PreOrder<AXmlObject>(xmlDocument, node => node.Children).OfType<AXmlTag>().Where(t => t.IsComment)) {
				int matchLength;
				AXmlText comment = tag.Children.OfType<AXmlText>().First();
				int index = comment.Value.IndexOfAny(TaskListTokens, 0, out matchLength);
				if (index > -1) {
					if (document == null)
						document = fileContent as IDocument ?? new ReadOnlyDocument(fileContent, parseInfo.FileName);
					do {
						TextLocation startLocation = document.GetLocation(comment.StartOffset + index);
						int startOffset = index + comment.StartOffset;
						int endOffset = Math.Min(document.GetLineByOffset(startOffset).EndOffset, comment.EndOffset);
						string content = document.GetText(startOffset, endOffset - startOffset);
						parseInfo.TagComments.Add(new TagComment(content.Substring(0, matchLength), new DomRegion(parseInfo.FileName, startLocation.Line, startLocation.Column), content.Substring(matchLength)));
						index = comment.Value.IndexOfAny(TaskListTokens, endOffset - comment.StartOffset, out matchLength);
					} while (index > -1);
				}
			}
		}
		
		public ResolveResult Resolve(ParseInformation parseInfo, TextLocation location, ICompilation compilation, CancellationToken cancellationToken)
		{
			return new XamlAstResolver(compilation, (XamlFullParseInformation)parseInfo)
				.ResolveAtLocation(location, cancellationToken);
		}
		
		public void FindLocalReferences(ParseInformation parseInfo, ITextSource fileContent, IVariable variable, ICompilation compilation, Action<Reference> callback, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
		
		public ICompilation CreateCompilationForSingleFile(FileName fileName, IUnresolvedFile unresolvedFile)
		{
			// TODO: create a simple compilation with WPF references?
			return null;
		}
		
		public ResolveResult ResolveSnippet(ParseInformation parseInfo, TextLocation location, string codeSnippet, ICompilation compilation, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
