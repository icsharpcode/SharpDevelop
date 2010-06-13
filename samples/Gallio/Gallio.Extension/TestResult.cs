// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Gallio.Extension
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
			set { stackTrace = value;	}
		}
	}
}
