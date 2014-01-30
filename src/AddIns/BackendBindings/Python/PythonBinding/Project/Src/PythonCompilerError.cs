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

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Saves information about a parser error reported by the
	/// PythonCompilerSink.
	/// </summary>
	public class PythonCompilerError
	{
		string path = String.Empty;
		string message = String.Empty;
		string lineText = String.Empty;
		SourceSpan location;
		int errorCode;
		Severity severity;
		
		public PythonCompilerError(string path, string message, string lineText, SourceSpan location, int errorCode, Severity severity)
		{
			this.path = path;
			this.message = message;
			this.lineText = lineText;
			this.location = location;
			this.errorCode = errorCode;
			this.severity = severity;
		}
		
		public override string ToString()
		{
			return String.Concat("[", errorCode, "][Sev:", severity.ToString(), "]", message, "\r\nLine: ", location.Start.Line, lineText, "\r\n", path, "\r\n");
		}
	}
}
