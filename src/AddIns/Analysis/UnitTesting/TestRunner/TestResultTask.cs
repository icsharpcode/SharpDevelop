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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.UnitTesting
{
	public class TestResultTask
	{
		TestResultTask()
		{
		}
		
		public static SDTask Create(TestResult result, ITestProject project)
		{
			TaskType taskType = TaskType.Warning;
			FileLineReference lineRef = null;
			string message = String.Empty;
			
			if (result.IsFailure) {
				taskType = TaskType.Error;
				if (!result.StackTraceFilePosition.IsEmpty) {
					lineRef = new FileLineReference(result.StackTraceFilePosition.FileName, result.StackTraceFilePosition.BeginLine - 1, result.StackTraceFilePosition.BeginColumn - 1);
				}
				message = GetTestFailedMessage(result);
			} else if (result.IsIgnored) {
				message = GetTestIgnoredMessage(result);
			}
			if (lineRef == null) {
				lineRef = FindTest(result.Name, project);
			}
			FileName fileName = null;
			if (lineRef != null) {
				fileName = new FileName(Path.GetFullPath(lineRef.FileName));
				int line = lineRef.Line + 1;
				return new SDTask(fileName, message, lineRef.Column, line, taskType);
			}
			return new SDTask(fileName, message, 0, 0, taskType);
		}
		
		static string GetTestFailedMessage(TestResult result)
		{
			string resource = "${res:NUnitPad.NUnitPadContent.TestTreeView.TestFailedMessage}";
			return GetTestResultMessage(result, resource);
		}
		
		static string GetTestIgnoredMessage(TestResult result)
		{
			string resource = "${res:NUnitPad.NUnitPadContent.TestTreeView.TestNotExecutedMessage}";
			return GetTestResultMessage(result, resource);
		}
		
		/// <summary>
		/// Returns the test result message if there is on otherwise
		/// uses the string resource to create a message.
		/// </summary>
		static string GetTestResultMessage(TestResult result, string stringResource)
		{
			if (result.Message.Length > 0) {
				return result.Message;
			}
			return GetTestResultMessageFromResource(result, stringResource);
		}
		
		static string GetTestResultMessageFromResource(TestResult result, string stringResource)
		{
			return StringParser.Parse(stringResource, new StringTagPair("TestCase", result.Name));
		}
		
		/// <summary>
		/// Returns the location of the specified test member in the
		/// project being tested.
		/// </summary>
		static FileLineReference FindTest(string memberName, ITestProject testProject)
		{
//			if (testProject != null) {
//				TestMember testMember = testProject.TestClasses.GetTestMember(memberName);
//				if (testMember != null) {
//					return FindTest(testMember);
//				}
//			}
			return null;
		}
		
//		static FileLineReference FindTest(TestMember testMember)
//		{
//			MemberResolveResult resolveResult = new MemberResolveResult(null, null, testMember.Member);
//			FilePosition filePos = resolveResult.GetDefinitionPosition();
//			return new FileLineReference(filePos.FileName, filePos.Line - 1, filePos.Column - 1);
//		}
	}
}
