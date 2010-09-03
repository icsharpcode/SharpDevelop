// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.UnitTesting
{
	public class TestResultTask
	{
		TestResultTask()
		{
		}
		
		public static Task Create(TestResult result, TestProject project)
		{
			TaskType taskType = TaskType.Warning;
			FileLineReference lineRef = null;
			string message = String.Empty;
			
			if (result.IsFailure) {
				taskType = TaskType.Error;
				if (!result.StackTraceFilePosition.IsEmpty) {
					lineRef = new FileLineReference(result.StackTraceFilePosition.FileName, result.StackTraceFilePosition.Line - 1, result.StackTraceFilePosition.Column - 1);
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
				return new Task(fileName, message, lineRef.Column, line, taskType);
			}
			return new Task(fileName, message, 0, 0, taskType);
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
			string [,] customTags = new string[,] {{"TestCase", result.Name}};
			return StringParser.Parse(stringResource, customTags);
		}
		
		/// <summary>
		/// Returns the location of the specified test method in the
		/// project being tested.
		/// </summary>
		static FileLineReference FindTest(string methodName, TestProject testProject)
		{
			if (testProject != null) {
				TestMethod testMethod = testProject.TestClasses.GetTestMethod(methodName);
				if (testMethod != null) {
					return FindTest(testMethod);
				}
			}
			return null;
		}
		
		static FileLineReference FindTest(TestMethod testMethod)
		{
			MemberResolveResult resolveResult = new MemberResolveResult(null, null, testMethod.Method);
			FilePosition filePos = resolveResult.GetDefinitionPosition();
			return new FileLineReference(filePos.FileName, filePos.Line - 1, filePos.Column - 1);
		}
	}
}
