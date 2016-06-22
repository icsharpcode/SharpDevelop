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
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.UnitTesting
{
	public enum TestResultType {
		/// <summary>
		/// The test has not been run.
		/// </summary>
		None    = 0,
		
		/// <summary>
		/// The test passed.
		/// </summary>
		Success = 1,
		
		/// <summary>
		/// The test failed.
		/// </summary>
		Failure = 3,
		
		/// <summary>
		/// The test was ignored.
		/// </summary>
		Ignored = 2
	}
	
	/// <summary>
	/// Holds the information about a single test result.
	/// </summary>
	public class TestResult
	{
		string name = string.Empty;
		string values = string.Empty;
		string message = string.Empty;
		string stackTrace = string.Empty;
		TestResultType resultType = TestResultType.None;
		DomRegion stackTraceFilePosition = DomRegion.Empty;
		
		public TestResult(string name)
		{
			this.name = ParseName(name, out values);
		}
		
		static string ParseName(string name, out string values)
		{
			int i = name.IndexOf('(');
			values = "";
			if (i > -1) {
				values = name.Substring(i);
				return name.Substring(0, i);
			} else
				return name;
		}
		
		public string Name {
			get { return name; }
		}
		
		public bool IsSuccess {
			get { return resultType == TestResultType.Success; }
		}
		
		public bool IsFailure {
			get { return resultType == TestResultType.Failure; }
		}
		
		public bool IsIgnored {
			get { return resultType == TestResultType.Ignored; }
		}
		
		public TestResultType ResultType {
			get { return resultType; }
			set { resultType = value; }
		}
		
		public string Message {
			get { return message; }
			set { message = value; }
		}
		
		public string StackTrace {
			get { return stackTrace; }
			set {
				if (stackTrace != value) {
					stackTrace = value;
					OnStackTraceChanged();
				}
			}
		}
		
		protected virtual void OnStackTraceChanged()
		{
		}
		
		public DomRegion StackTraceFilePosition {
			get { return stackTraceFilePosition; }
			set { stackTraceFilePosition = value; }
		}
	}
}
