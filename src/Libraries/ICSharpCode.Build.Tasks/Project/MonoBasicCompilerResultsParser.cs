// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ICSharpCode.Build.Tasks
{
	public class MonoBasicCompilerResultsParser
	{
		public const string NormalErrorPattern = @"(?<file>.*)\((?<line>\d+),(?<column>\d+)\)\s+(?<error>\w+)\s+(?<number>[\d\w]+):\s+(?<message>.*)";

		Regex normalError = new Regex(NormalErrorPattern, RegexOptions.Compiled);
		
		public MonoBasicCompilerResultsParser()
		{
		}
		
		public CompilerError ParseLine(string line)
		{
			// try to match standard mono errors
			Match match = normalError.Match(line); 
			if (match.Success) {
				CompilerError error = new CompilerError();
				error.Column      = Int32.Parse(match.Result("${column}"));
				error.Line        = Int32.Parse(match.Result("${line}"));
				error.FileName    = Path.GetFullPath(match.Result("${file}"));
				error.IsWarning   = match.Result("${error}") == "warning";
				error.ErrorNumber = match.Result("${number}");
				error.ErrorText   = match.Result("${message}");
				return error;
			}
			return null;
		}
	}
}
