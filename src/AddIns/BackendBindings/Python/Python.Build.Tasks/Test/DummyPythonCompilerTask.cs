// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
