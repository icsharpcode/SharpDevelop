// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using IronPython.Hosting;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Supresses exceptions thrown by the PythonParser when it
	/// finds a parsing error. By default the simple compiler sink
	/// throws an exception on a parsing error.
	/// </summary>
	public class PythonCompilerSink : CompilerSink
	{
		List<PythonCompilerError> errors = new List<PythonCompilerError>();
		
		public PythonCompilerSink()
		{
		}
		
		public override void AddError(string path, string message, string lineText, CodeSpan location, int errorCode, Severity severity)
		{
			errors.Add(new PythonCompilerError(path, message, lineText, location, errorCode, severity));
		}
		
		public List<PythonCompilerError> Errors {
			get { return errors; }
		}
	}
}
