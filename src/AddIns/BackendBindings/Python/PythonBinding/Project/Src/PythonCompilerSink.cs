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
