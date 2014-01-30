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

using System;
using ICSharpCode.Python.Build.Tasks;

namespace Python.Build.Tasks.Tests
{
	/// <summary>
	/// Overrides the GetCurrentFolder to return a predefined string.
	/// </summary>
	public class DummyPythonCompilerTask : PythonCompilerTask
	{
		string currentFolder;
		string loggedErrorMessage;
		string loggedErrorCode;
		string loggedErrorFile;
		int loggedStartColumn = -1;
		int loggedStartLine = -1;
		int loggedEndLine = -1;
		int loggedEndColumn = -1;
		
		public DummyPythonCompilerTask(IPythonCompiler compiler, string currentFolder)
			: base(compiler)
		{
			this.currentFolder = currentFolder;
		}
	
		/// <summary>
		/// Gets the error message passed to the LogError method.
		/// </summary>
		public string LoggedErrorMessage {
			get { return loggedErrorMessage; }
		}
		
		/// <summary>
		/// Gets the error code passed to the LogError method.
		/// </summary>
		public string LoggedErrorCode {
			get { return loggedErrorCode; }
		}		
		
		/// <summary>
		/// Gets the file passed to the LogError method.
		/// </summary>
		public string LoggedErrorFile {
			get { return loggedErrorFile; }
		}		
		
		/// <summary>
		/// Gets the start line passed to the LogError method.
		/// </summary>
		public int LoggedStartLine {
			get { return loggedStartLine; }
		}
		
		/// <summary>
		/// Gets the end line passed to the LogError method.
		/// </summary>
		public int LoggedEndLine {
			get { return loggedEndLine; }
		}		
		
		/// <summary>
		/// Gets the start column passed to the LogError method.
		/// </summary>
		public int LoggedStartColumn {
			get { return loggedStartColumn; }
		}		
		
		/// <summary>
		/// Gets the end column passed to the LogError method.
		/// </summary>
		public int LoggedEndColumn {
			get { return loggedEndColumn; }
		}		
				
		protected override string GetCurrentFolder()
		{
			return currentFolder;
		}
		
		protected override void LogError(string message, string errorCode, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber)
		{
			loggedErrorMessage = message;
			loggedErrorCode = errorCode;
			loggedErrorFile = file;
			loggedStartColumn = columnNumber;
			loggedStartLine = lineNumber;
			loggedEndColumn = endColumnNumber;
			loggedEndLine = endLineNumber;
		}
	}
}
