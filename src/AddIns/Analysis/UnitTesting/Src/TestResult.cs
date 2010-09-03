// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

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
		Failure = 2,
		
		/// <summary>
		/// The test was ignored.
		/// </summary>
		Ignored = 3
	}
	
	/// <summary>
	/// Holds the information about a single test result.
	/// </summary>
	public class TestResult
	{
		string name = String.Empty;
		string message = String.Empty;
		string stackTrace = String.Empty;
		TestResultType resultType = TestResultType.None;
		FilePosition stackTraceFilePosition = FilePosition.Empty;
		
		public TestResult(string name)
		{
			this.name = name;
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
		
		public FilePosition StackTraceFilePosition {
			get { return stackTraceFilePosition; }
			set { stackTraceFilePosition = value; }
		}
	}
}
