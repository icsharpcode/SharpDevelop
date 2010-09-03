// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using Microsoft.Scripting;
using System;
using System.Collections.Generic;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Supresses exceptions thrown by the PythonParser when it
	/// finds a parsing error. By default the simple compiler sink
	/// throws an exception on a parsing error.
	/// </summary>
	public class PythonCompilerSink : ErrorSink
	{
		List<PythonCompilerError> errors = new List<PythonCompilerError>();
		
		public PythonCompilerSink()
		{
		}
		
		public override void Add(SourceUnit source, string message, SourceSpan span, int errorCode, Severity severity)
		{
			int line = GetLine(span.Start.Line);
			errors.Add(new PythonCompilerError(source.Path, message, source.GetCodeLine(line), span, errorCode, severity));
		}
				
		public List<PythonCompilerError> Errors {
			get { return errors; }
		}
		
		/// <summary>
		/// Ensure the line number is valid.
		/// </summary>
		static int GetLine(int line)
		{
			if (line > 0) {
				return line;
			}
			return 1;
		}
	}
}
