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
using System.IO;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MSTest
{
	public class MSTestResult
	{
		public MSTestResult(string id)
		{
			this.Id = id;
		}
		
		public string Id { get; private set; }
		public string Name { get; private set; }
		public TestResultType ResultType { get; private set; }
		public string StackTrace { get; set; }
		public string Message { get; set; }
		
		public void UpdateTestName(string qualifiedClassName, string methodName)
		{
			UpdateTestName(new MSTestQualifiedClassName(qualifiedClassName), methodName);
		}
		
		public void UpdateTestName(MSTestQualifiedClassName qualifiedClassName, string methodName)
		{
			this.Name = qualifiedClassName.GetQualifiedMethodName(methodName);
		}
		
		public void UpdateResult(string result)
		{
			if (result == "Passed") {
				this.ResultType = TestResultType.Success;
			} else if (result == "Failed") {
				this.ResultType = TestResultType.Failure;
			}
		}
		
		public bool IsError {
			get { return ResultType == TestResultType.Failure; }
		}
		
		public TestResult ToTestResult()
		{
			return new TestResult(Name) {
				ResultType = ResultType,
				Message = GetMessage(),
				StackTrace = StackTrace,
				StackTraceFilePosition = GetStackTraceFilePosition()
			};
		}
		
		string GetMessage()
		{
			if (String.IsNullOrEmpty(Message))
				return String.Empty;
			
			int index = Message.IndexOf('\n');
			if (index > 0) {
				return Message.Substring(index + 1);
			}
			return Message;
		}
		
		DomRegion GetStackTraceFilePosition()
		{
			if (!String.IsNullOrEmpty(StackTrace)) {
				return ParseFilePositionFromStackTrace();
			}
			return DomRegion.Empty;
		}
		
		DomRegion ParseFilePositionFromStackTrace()
		{
			FileLineReference fileLineRef = OutputTextLineParser.GetNUnitOutputFileLineReference(StackTrace, true);
			if (fileLineRef != null) {
				return CreateFilePosition(fileLineRef);
			}
			return DomRegion.Empty;
		}
		
		DomRegion CreateFilePosition(FileLineReference fileLineRef)
		{
			string fileName = Path.GetFullPath(fileLineRef.FileName);
			return new DomRegion(fileName, fileLineRef.Line, fileLineRef.Column + 1);
		}
	}
}
