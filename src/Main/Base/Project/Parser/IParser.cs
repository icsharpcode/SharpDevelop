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
using System.Threading;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Parser
{
	/// <summary>
	/// Represents a language parser that produces ParseInformation
	/// and IUnresolvedFile instances for code files.
	/// </summary>
	public interface IParser
	{
		/// <summary>
		/// Gets/Sets the tags used to identify tasks.
		/// </summary>
		IReadOnlyList<string> TaskListTokens { get; set; }
		
		/// <summary>
		/// Gets if the parser can parse the specified file.
		/// This method is used to get the correct parser for a specific file and normally decides based on the file
		/// extension.
		/// </summary>
		bool CanParse(string fileName);
		
		/// <summary>
		/// Returns the content for a given filename.
		/// </summary>
		ITextSource GetFileContent(FileName fileName);
		
		/// <summary>
		/// Parses a file.
		/// </summary>
		/// <param name="fileName">The name of the file being parsed.</param>
		/// <param name="fileContent">The content of the file.</param>
		/// <param name="fullParseInformationRequested">
		/// Specifies whether full parse information were requested for this file.
		/// If this parameter is false, only the UnresolvedFile and TagComments on the parse information need to be set.
		/// </param>
		/// <param name="parentProject">The parent project for this parse run.</param>
		/// <param name="cancellationToken">Cancellation Token.</param>
		/// <returns>The parse information representing the parse results.</returns>
		/// <remarks>
		/// The SharpDevelop parser service may call IParser.Parse in parallel;
		/// even on the same IParser instance if there are two parallel parse requests for the same file.
		/// Parser implementations must be thread-safe.
		/// 
		/// The SharpDevelop main thread is allowed to wait for a parse operation to finish; thus IParser
		/// implementations must not invoke methods on the main thread and wait for their results,
		/// as that would deadlock.
		/// </remarks>
		ParseInformation Parse(FileName fileName, ITextSource fileContent, bool fullParseInformationRequested,
		                       IProject parentProject, CancellationToken cancellationToken);
		
		ResolveResult Resolve(ParseInformation parseInfo, TextLocation location, ICompilation compilation, CancellationToken cancellationToken);
		ICodeContext ResolveContext(ParseInformation parseInfo, TextLocation location, ICompilation compilation, CancellationToken cancellationToken);
		
		ResolveResult ResolveSnippet(ParseInformation parseInfo, TextLocation location, string codeSnippet, ICompilation compilation, CancellationToken cancellationToken);
		
		void FindLocalReferences(ParseInformation parseInfo, ITextSource fileContent, IVariable variable, ICompilation compilation, Action<SearchResultMatch> callback, CancellationToken cancellationToken);
		
		/// <summary>
		/// Creates a compilation for a single file that does not belong to any project.
		/// Used by <see cref="ParserService.GetCompilationForFile"/>.
		/// May return null if this operation is not supported.
		/// </summary>
		ICompilation CreateCompilationForSingleFile(FileName fileName, IUnresolvedFile unresolvedFile);
	}
}
