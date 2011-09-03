// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Represents a language parser that produces ParseInformation
	/// and IParsedFile instances for code files.
	/// </summary>
	public interface IParser
	{
		/// <summary>
		/// Gets/Sets the tags used to identify tasks.
		/// </summary>
		string[] LexerTags {
			get;
			set;
		}
		
		/// <summary>
		/// Gets if the parser can parse the specified file.
		/// This method is used to get the correct parser for a specific file and normally decides based on the file
		/// extension.
		/// </summary>
		bool CanParse(string fileName);
		
		/// <summary>
		/// Parses a file.
		/// </summary>
		/// <param name="projectContent">The parent project of the file.</param>
		/// <param name="fileName">The name of the file being parsed.</param>
		/// <param name="fileContent">The content of the file.</param>
		/// <param name="fullParseInformationRequested">
		/// Specifies whether full parse information were requested for this file.
		/// If this parameter is false, only the ParsedFile and TagComments on the parse information need to be set.
		/// </param>
		/// <returns>The parse information representing the parse results.</returns>
		/// <remarks>
		/// SharpDevelop may call IParser.Parse in parallel. This will be done on the same IParser instance
		/// if there are two parallel parse requests for the same file. Parser implementations must be thread-safe.
		/// </remarks>
		ParseInformation Parse(IProjectContent projectContent, FileName fileName, ITextSource fileContent,
		                       bool fullParseInformationRequested);
		
		ResolveResult Resolve(ParseInformation parseInfo, TextLocation location, ITypeResolveContext context, CancellationToken cancellationToken);
		
		void FindLocalReferences(ParseInformation parseInfo, IVariable variable, ITypeResolveContext context, Action<Reference> callback);
	}
}
