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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom.ClassBrowser;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.ILSpyAddIn
{
	/// <summary>
	/// This class "parses" a decompiled type to provide the information required
	/// by the ParserService.
	/// </summary>
	public class ILSpyParser : IParser
	{
		public bool CanParse(string fileName)
		{
			return fileName != null && fileName.StartsWith("ilspy://", StringComparison.OrdinalIgnoreCase);
		}
		
		readonly ITextSource EmptyFileContent = new StringTextSource("", new OnDiskTextSourceVersion(DateTime.MinValue));
		
		public ITextSource GetFileContent(FileName fileName)
		{
			return EmptyFileContent;
		}
		
		public ParseInformation Parse(FileName fileName, ITextSource fileContent, bool fullParseInformationRequested, IProject parentProject, CancellationToken cancellationToken)
		{
			return ILSpyDecompilerService.DecompileType(DecompiledTypeReference.FromFileName(fileName), cancellationToken);
		}
		
		public ResolveResult Resolve(ParseInformation parseInfo, TextLocation location, ICompilation compilation, CancellationToken cancellationToken)
		{
			var decompiledParseInfo = parseInfo as ILSpyFullParseInformation;
			if (decompiledParseInfo == null)
				throw new ArgumentException("ParseInfo does not have SyntaxTree");
			return ResolveAtLocation.Resolve(compilation, null, decompiledParseInfo.SyntaxTree, location, cancellationToken);
		}
		
		public ICodeContext ResolveContext(ParseInformation parseInfo, TextLocation location, ICompilation compilation, CancellationToken cancellationToken)
		{
			var decompiledParseInfo = parseInfo as ILSpyFullParseInformation;
			if (decompiledParseInfo == null)
				throw new ArgumentException("ParseInfo does not have SyntaxTree");
			var syntaxTree = decompiledParseInfo.SyntaxTree;
			var node = syntaxTree.GetNodeAt(location);
			if (node == null)
				return null; // null result is allowed; the parser service will substitute a dummy context
			var resolver = new CSharpAstResolver(compilation, syntaxTree, null);
			return resolver.GetResolverStateBefore(node);
		}
		
		public ResolveResult ResolveSnippet(ParseInformation parseInfo, TextLocation location, string codeSnippet, ICompilation compilation, CancellationToken cancellationToken)
		{
			var decompiledParseInfo = parseInfo as ILSpyFullParseInformation;
			if (decompiledParseInfo == null)
				throw new ArgumentException("ParseInfo does not have SyntaxTree");
			CSharpAstResolver contextResolver = new CSharpAstResolver(compilation, decompiledParseInfo.SyntaxTree, null);
			var node = decompiledParseInfo.SyntaxTree.GetNodeAt(location);
			CSharpResolver context;
			if (node != null)
				context = contextResolver.GetResolverStateAfter(node, cancellationToken);
			else
				context = new CSharpResolver(compilation);
			CSharpParser parser = new CSharpParser();
			var expr = parser.ParseExpression(codeSnippet);
			if (parser.HasErrors)
				return new ErrorResolveResult(SpecialType.UnknownType, PrintErrorsAsString(parser.Errors), TextLocation.Empty);
			CSharpAstResolver snippetResolver = new CSharpAstResolver(context, expr);
			return snippetResolver.Resolve(expr, cancellationToken);
		}
		
		string PrintErrorsAsString(IEnumerable<Error> errors)
		{
			StringBuilder builder = new StringBuilder();
			
			foreach (var error in errors)
				builder.AppendLine(error.Message);
			
			return builder.ToString();
		}
		
		public void FindLocalReferences(ParseInformation parseInfo, ITextSource fileContent, IVariable variable, ICompilation compilation, Action<SearchResultMatch> callback, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
		
		public ICompilation CreateCompilationForSingleFile(FileName fileName, IUnresolvedFile unresolvedFile)
		{
			DecompiledTypeReference reference = DecompiledTypeReference.FromFileName(fileName);
			if (reference != null) {
				var model = SD.GetService<IClassBrowser>().FindAssemblyModel(reference.AssemblyFile);
				if (model == null)
					model = SD.AssemblyParserService.GetAssemblyModelSafe(reference.AssemblyFile, true);
				if (model != null)
					return model.Context.GetCompilation();
			}
			return new CSharpProjectContent()
				.AddOrUpdateFiles(unresolvedFile)
				.CreateCompilation();
		}
		
		public IReadOnlyList<string> TaskListTokens { get; set; }
	}
}
