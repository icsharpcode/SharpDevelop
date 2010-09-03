// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
