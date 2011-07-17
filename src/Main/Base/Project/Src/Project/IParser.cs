// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Represents a language parser that produces ICompilationUnit instances
	/// for code files.
	/// </summary>
	public interface IParser
	{
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
		/// Gets if the parser can parse the specified project.
		/// Only when no parser for a project is found, the assembly is loaded.
		/// </summary>
		bool CanParse(IProject project);
		
		/// <summary>
		/// Parses a file.
		/// </summary>
		/// <param name="projectContent">The parent project of the file.</param>
		/// <param name="fileName">The name of the file being parsed.</param>
		/// <param name="fileContent">The content of the file.</param>
		/// <returns>The compilation unit representing the parse results.</returns>
		/// <remarks>
		/// SharpDevelop may call IParser.Parse in parallel. This will be done on the same IParser instance
		/// if there are two parallel parse requests for the same file. Parser implementations must be thread-safe.
		/// </remarks>
		IParsedFile Parse(IProjectContent projectContent, string fileName, ITextSource fileContent);
		
		//IResolver CreateResolver();
	}
}
