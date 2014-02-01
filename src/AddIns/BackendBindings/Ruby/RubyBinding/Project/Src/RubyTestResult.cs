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
