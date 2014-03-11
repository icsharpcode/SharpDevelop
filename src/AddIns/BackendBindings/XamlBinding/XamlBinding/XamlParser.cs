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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Parses xaml files to partial classes for the Dom.
	/// </summary>
	public class XamlParser : IParser
	{
		public IReadOnlyList<string> TaskListTokens { get; set; }
		
		public XamlParser()
		{
			TaskListTokens = EmptyList<string>.Instance;
		}

		public bool CanParse(string fileName)
		{
			return Path.GetExtension(fileName).Equals(".xaml", StringComparison.OrdinalIgnoreCase);
		}

		public ITextSource GetFileContent(FileName fileName)
		{
			return SD.FileService.GetFileContent(fileName);
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
				string commentText = fileContent.GetText(tag.StartOffset, tag.Length);
				int index = commentText.IndexOfAny(TaskListTokens, 0, out matchLength);
				if (index > -1) {
					if (document == null)
						document = fileContent as IDocument ?? new ReadOnlyDocument(fileContent, parseInfo.FileName);
					do {
						TextLocation startLocation = document.GetLocation(tag.StartOffset + index);
						int startOffset = index + tag.StartOffset;
						int endOffset = Math.Min(document.GetLineByOffset(startOffset).EndOffset, tag.EndOffset);
						string content = document.GetText(startOffset, endOffset - startOffset);
						parseInfo.TagComments.Add(new TagComment(content.Substring(0, matchLength), new DomRegion(parseInfo.FileName, startLocation.Line, startLocation.Column), content.Substring(matchLength)));
						index = commentText.IndexOfAny(TaskListTokens, endOffset - tag.StartOffset, out matchLength);
					} while (index > -1);
				}
			}
		}
		
		public ResolveResult Resolve(ParseInformation parseInfo, TextLocation location, ICompilation compilation, CancellationToken cancellationToken)
		{
			return new XamlAstResolver(compilation, (XamlFullParseInformation)parseInfo)
				.ResolveAtLocation(location, cancellationToken);
		}

		public ICodeContext ResolveContext(ParseInformation parseInfo, TextLocation location, ICompilation compilation, CancellationToken cancellationToken)
		{
			return null; // null result is allowed; the parser service will substitute a dummy context
		}
		
		public void FindLocalReferences(ParseInformation parseInfo, ITextSource fileContent, IVariable variable, ICompilation compilation, Action<SearchResultMatch> callback, CancellationToken cancellationToken)
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
			return null;
		}
	}
}
