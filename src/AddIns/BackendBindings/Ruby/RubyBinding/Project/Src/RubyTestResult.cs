// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using System.Text.RegularExpressions;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.RubyBinding
{
	public class RubyTestResult : TestResult
	{
		public RubyTestResult(TestResult testResult)
			: base(testResult.Name)
		{
			ResultType = testResult.ResultType;
			Message = testResult.Message;
			StackTrace = testResult.StackTrace;
		}
		
		protected override void OnStackTraceChanged()
		{
			if (String.IsNullOrEmpty(StackTrace)) {
				ResetStackTraceFilePosition();
			} else {
				GetFilePositionFromStackTrace();
			}
		}
		
		void ResetStackTraceFilePosition()
		{
			StackTraceFilePosition = FilePosition.Empty;
		}
		
		/// <summary>
		/// Stack trace: Failure:
		/// test_fail(SecondTests)
		///     [d:\temp\test\rubytests\SecondTests.rb:6:in `test_fail'
		///      d:\temp\test\rubytests/sdtestrunner.rb:73:in `start_mediator'
		///      d:\temp\test\rubytests/sdtestrunner.rb:47:in `start']:
		/// Assertion was false.
		/// <false> is not true.
		/// </summary>
		void GetFilePositionFromStackTrace()
		{
			Match match = Regex.Match(StackTrace, "\\s\\[(.*?):(\\d+):", RegexOptions.Multiline);
			if (match.Success) {
				try {
					SetStackTraceFilePosition(match.Groups);
				} catch (OverflowException) {
					// Ignore.
				}
			}
		}
		
		void SetStackTraceFilePosition(GroupCollection groups)
		{
			string fileName = groups[1].Value;
			int line = Convert.ToInt32(groups[2].Value);
			int column = 1;
			
			StackTraceFilePosition = new FilePosition(fileName, line, column);
		}
	}
}
