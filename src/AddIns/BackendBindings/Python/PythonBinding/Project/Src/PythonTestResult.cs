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

namespace ICSharpCode.PythonBinding
{
	public class PythonTestResult : TestResult
	{
		public PythonTestResult(TestResult testResult)
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
		/// Stack trace: 
		/// Traceback (most recent call last):
		///  File "d:\temp\test\PyTests\Tests\MyClassTest.py", line 19, in testRaiseException
		///    raise 'abc'
		/// </summary>
		void GetFilePositionFromStackTrace()
		{
			Match match = Regex.Match(StackTrace, "\\sFile\\s\"(.*?)\",\\sline\\s(\\d+),", RegexOptions.Multiline);
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
