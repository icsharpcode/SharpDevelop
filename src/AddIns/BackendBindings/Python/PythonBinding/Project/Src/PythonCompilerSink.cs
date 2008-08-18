// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Scripting;

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
			errors.Add(new PythonCompilerError(source.Path, message, source.GetCodeLine(span.Start.Line), span, errorCode, severity));
		}
				
		public List<PythonCompilerError> Errors {
			get { return errors; }
		}
	}
}
