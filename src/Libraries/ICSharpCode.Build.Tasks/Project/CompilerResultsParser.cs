// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
	public class CompilerResultsParser : ICompilerResultsParser
	{
		public const string NormalErrorPattern = @"(?<file>.*)\((?<line>\d+),(?<column>\d+)\):\s+(?<error>\w+)\s+(?<number>[\d\w]+):\s+(?<message>.*)";
		public const string GeneralErrorPattern = @"(?<error>.+?)\s+(?<number>[\d\w]+?):\s+(?<message>.*)";

		Regex normalError = new Regex(NormalErrorPattern, RegexOptions.Compiled);
		Regex generalError = new Regex(GeneralErrorPattern, RegexOptions.Compiled);
		
		public CompilerResultsParser()
		{
		}
		
		public CompilerResults Parse(TempFileCollection tempFiles, string fileName)
		{
			CompilerResults results = new CompilerResults(tempFiles);
	
			StringBuilder compilerOutput = new StringBuilder();
			StreamReader resultsReader = File.OpenText(fileName);
						
			while (true) {
				string curLine = resultsReader.ReadLine();
				compilerOutput.Append(curLine);
				compilerOutput.Append('\n');
				if (curLine == null) {
					break;
				}
				curLine = curLine.Trim();
				if (curLine.Length == 0) {
					continue;
				}
				
				CompilerError error = new CompilerError();
				
				// try to match standard mono errors
				Match match = normalError.Match(curLine); 
				if (match.Success) {
					error.Column      = Int32.Parse(match.Result("${column}"));
					error.Line        = Int32.Parse(match.Result("${line}"));
					error.FileName    = Path.GetFullPath(match.Result("${file}"));
					error.IsWarning   = match.Result("${error}") == "warning";
					error.ErrorNumber = match.Result("${number}");
					error.ErrorText   = match.Result("${message}");
	
					results.Errors.Add(error);
				} else {
					match = generalError.Match(curLine);
					if (match.Success) {
						error.IsWarning   = match.Result("${error}") == "warning";
						error.ErrorNumber = match.Result("${number}");
						error.ErrorText   = match.Result("${message}");
						
						results.Errors.Add(error);
					}
				}
			}
			resultsReader.Close();
					
			return results;
		}
	}
}
