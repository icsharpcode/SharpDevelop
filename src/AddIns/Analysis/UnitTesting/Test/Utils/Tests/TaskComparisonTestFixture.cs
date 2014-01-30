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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class TaskComparisonTestFixture
	{
		TaskComparison taskComparison;
		SDTask lhs;
		SDTask rhs;
		int column = 20;
		int line = 5;
		string myTestFileName = @"c:\projects\tests\mytest.cs";
		string description = "description";
		
		[SetUp]
		public void Init()
		{
			FileName fileName = new FileName(myTestFileName);
			lhs = new SDTask(fileName, description, column, line, TaskType.Error);
			rhs = new SDTask(fileName, description, column, line, TaskType.Error);
		}
		
		[Test]
		public void IsMatchReturnsTrueWhenFileNameDescriptionColumnLineAndTaskTypeMatch()
		{
			CreateTaskComparison();
			Assert.IsTrue(taskComparison.IsMatch);
		}
		
		void CreateTaskComparison()
		{
			taskComparison = new TaskComparison(lhs, rhs);
		}
		
		[Test]
		public void IsMatchReturnsFalseWhenTaskTypesAreDifferent()
		{
			FileName fileName = new FileName(myTestFileName);
			lhs = new SDTask(fileName, description, column, line, TaskType.Warning);
			CreateTaskComparison();
			Assert.IsFalse(taskComparison.IsMatch);
		}
		
		[Test]
		public void MismatchReasonIndicatesTaskTypeMismatchWhenTaskTypesAreDifferent()
		{
			IsMatchReturnsFalseWhenTaskTypesAreDifferent();
			
			string expectedMismatchReason = 
				"TaskTypes are different.\r\n" +
				"Expected: Warning\r\n" +
				"But was: Error\r\n";
			
			Assert.AreEqual(expectedMismatchReason, taskComparison.MismatchReason);
		}
		
		[Test]
		public void IsMatchReturnsFalseWhenFileNamesAreDifferent()
		{
			FileName fileName = new FileName(@"temp.cs");
			lhs = new SDTask(fileName, rhs.Description, rhs.Column, rhs.Line, rhs.TaskType);
			CreateTaskComparison();
			Assert.IsFalse(taskComparison.IsMatch);
		}
		
		[Test]
		public void MismatchReasonIndicatesFileNameMismatchWhenFileNamesAreDifferent()
		{
			IsMatchReturnsFalseWhenFileNamesAreDifferent();
			
			string expectedMismatchReason = 
				"FileNames are different.\r\n" +
				"Expected: temp.cs\r\n" +
				"But was: c:\\projects\\tests\\mytest.cs\r\n";
			
			Assert.AreEqual(expectedMismatchReason, taskComparison.MismatchReason);
		}
		
		[Test]
		public void TaskComparisonIsMatchReturnsTrueWhenBothTasksAreNull()
		{
			lhs = null;
			rhs = null;
			CreateTaskComparison();
			
			Assert.IsTrue(taskComparison.IsMatch);
		}
		
		[Test]
		public void IsMatchReturnsFalseWhenLhsTaskIsNull()
		{
			lhs = null;
			CreateTaskComparison();
			
			Assert.IsFalse(taskComparison.IsMatch);
		}
		
		[Test]
		public void MismatchReasonIndicatesLhsTaskIsNullInComparison()
		{
			IsMatchReturnsFalseWhenLhsTaskIsNull();
			
			string expectedMismatchReason = 
				"One task is null.\r\n" +
				"Expected: (null)\r\n" +
				"But was: " + rhs.ToString() + "\r\n";
			
			Assert.AreEqual(expectedMismatchReason, taskComparison.MismatchReason);
		}
		
		[Test]
		public void IsMatchReturnsFalseWhenRhsTaskIsNull()
		{
			rhs = null;
			CreateTaskComparison();
			
			Assert.IsFalse(taskComparison.IsMatch);
		}
		
		[Test]
		public void MismatchReasonIndicatesRhsTaskIsNullInComparison()
		{
			IsMatchReturnsFalseWhenRhsTaskIsNull();
			
			string expectedMismatchReason = 
				"One task is null.\r\n" +
				"Expected: " + lhs.ToString() + "\r\n" +
				"But was: (null)\r\n";
			
			Assert.AreEqual(expectedMismatchReason, taskComparison.MismatchReason);
		}
		
		[Test]
		public void IsMatchReturnsFalseWhenDescriptionsAreDifferent()
		{
			FileName fileName = new FileName(myTestFileName);
			rhs = new SDTask(fileName, "different", column, line, TaskType.Error);
			CreateTaskComparison();
			Assert.IsFalse(taskComparison.IsMatch);
		}
		
		[Test]
		public void MismatchReasonIndicatesDescriptionMismatchWhenDescriptionsAreDifferent()
		{
			IsMatchReturnsFalseWhenDescriptionsAreDifferent();
			
			string expectedMismatchReason = 
				"Descriptions are different.\r\n" +
				"Expected: description\r\n" +
				"But was: different\r\n";
			
			Assert.AreEqual(expectedMismatchReason, taskComparison.MismatchReason);
		}
		
		[Test]
		public void IsMatchReturnsFalseWhenColumnsAreDifferent()
		{
			FileName fileName = new FileName(myTestFileName);
			rhs = new SDTask(fileName, description, 500, line, TaskType.Error);
			CreateTaskComparison();
			Assert.IsFalse(taskComparison.IsMatch);
		}
		
		[Test]
		public void MismatchReasonIndicatesColumnsMismatchWhenColumnsAreDifferent()
		{
			IsMatchReturnsFalseWhenColumnsAreDifferent();
			
			string expectedMismatchReason = 
				"Columns are different.\r\n" +
				"Expected: 20\r\n" +
				"But was: 500\r\n";
			
			Assert.AreEqual(expectedMismatchReason, taskComparison.MismatchReason);
		}
		
		[Test]
		public void IsMatchReturnsFalseWhenLinesAreDifferent()
		{
			FileName fileName = new FileName(myTestFileName);
			rhs = new SDTask(fileName, description, column, 66, TaskType.Error);
			CreateTaskComparison();
			Assert.IsFalse(taskComparison.IsMatch);
		}
		
		[Test]
		public void MismatchReasonIndicatesLinesMismatchWhenColumnsAreDifferent()
		{
			IsMatchReturnsFalseWhenLinesAreDifferent();
			
			string expectedMismatchReason = 
				"Lines are different.\r\n" +
				"Expected: 5\r\n" +
				"But was: 66\r\n";
			
			Assert.AreEqual(expectedMismatchReason, taskComparison.MismatchReason);
		}
	}
}
