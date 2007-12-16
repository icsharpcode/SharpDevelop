// SharpDevelop samples
// Copyright (c) 2006, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text.RegularExpressions;

namespace Mono.Build.Tasks
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
