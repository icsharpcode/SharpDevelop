// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using System.Text;

namespace ICSharpCode.TextTemplating
{
	public interface ITextTemplatingHost : IDisposable
	{
		string OutputFile { get; }
		CompilerErrorCollection Errors { get; }
		
		bool ProcessTemplate(string inputFile, string outputFile);
		
		bool PreprocessTemplate (
			string inputFile,
			string className,
			string classNamespace,
			string outputFile,
			Encoding encoding,
			out string language,
			out string[] references);
	}
}
