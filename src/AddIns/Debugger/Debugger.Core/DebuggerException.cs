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

namespace Debugger
{
	/// <summary>
	/// Type of exception thrown by the Debugger.
	/// </summary>
	public class DebuggerException: System.Exception
	{
		public DebuggerException() {}
		public DebuggerException(string message): base(message) {}
		public DebuggerException(string message, params object[] args): base(string.Format(message, args)) {}
		public DebuggerException(string message, System.Exception inner): base(message, inner) {}
	}
	
	/// <summary>
	/// An exception that is thrown when the debugged process unexpectedly exits.
	/// </summary>
	public class ProcessExitedException: DebuggerException
	{
		string processName = null;
		
		/// <summary>
		/// The name of the process that has exited.
		/// </summary>
		public string ProcessName {
			get { return processName; }
		}
		
		/// <summary>
		/// Creates a ProcessExitedException for an unnamed process.
		/// </summary>
		public ProcessExitedException(): base("Process exited") {}
		
		/// <summary>
		/// Creates a ProcessExitedException for a process.
		/// </summary>
		/// <param name="processName">The name of the process</param>
		public ProcessExitedException(string processName): base(string.Format("Process '{0}' exited.", processName)) {
			this.processName = processName;
		}
		
	}
}
