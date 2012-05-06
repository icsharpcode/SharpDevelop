// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
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
		
		FilePosition GetStackTraceFilePosition()
		{
			if (!String.IsNullOrEmpty(StackTrace)) {
				return ParseFilePositionFromStackTrace();
			}
			return FilePosition.Empty;
		}
		
		FilePosition ParseFilePositionFromStackTrace()
		{
			FileLineReference fileLineRef = OutputTextLineParser.GetNUnitOutputFileLineReference(StackTrace, true);
			if (fileLineRef != null) {
				return CreateFilePosition(fileLineRef);
			}
			return FilePosition.Empty;
		}
		
		FilePosition CreateFilePosition(FileLineReference fileLineRef)
		{
			string fileName = Path.GetFullPath(fileLineRef.FileName);
			return new FilePosition(fileName, fileLineRef.Line, fileLineRef.Column + 1);
		}
	}
}
