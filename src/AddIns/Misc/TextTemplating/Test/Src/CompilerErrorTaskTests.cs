// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.TextTemplating;
using NUnit.Framework;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class CompilerErrorTaskTests
	{
		[Test]
		public void FileName_CompilerErrorFileNameIsTestTxt_ReturnsTestTxt()
		{
			var error = new CompilerError();
			error.FileName = "test.txt";
			var task = new CompilerErrorTask(error);
			
			FileName expectedFileName = new FileName("test.txt");
			Assert.AreEqual(expectedFileName, task.FileName);
		}
		
		[Test]
		public void Column_CompilerErrorColumnIsTwo_ReturnsTwo()
		{
			var error = new CompilerError();
			error.FileName = "test.txt";
			error.Line = 1;
			error.Column = 2;
			var task = new CompilerErrorTask(error);
			
			Assert.AreEqual(2, task.Column);
		}
		
		[Test]
		public void Line_CompilerErrorLineIsThree_ReturnsThree()
		{
			var error = new CompilerError();
			error.FileName = "test.txt";
			error.Line = 3;
			var task = new CompilerErrorTask(error);
			
			Assert.AreEqual(3, task.Line);
		}
		
		[Test]
		public void TaskType_CompilerErrorIsWarning_ReturnsWarning()
		{
			var error = new CompilerError();
			error.IsWarning = true;
			var task = new CompilerErrorTask(error);
			
			Assert.AreEqual(TaskType.Warning, task.TaskType);
		}
		
		[Test]
		public void TaskType_CompilerErrorIsNotWarning_ReturnsError()
		{
			var error = new CompilerError();
			var task = new CompilerErrorTask(error);
			
			Assert.AreEqual(TaskType.Error, task.TaskType);
		}
	}
}
