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
using System.CodeDom.Compiler;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.TextTemplating;
using NUnit.Framework;
using TextTemplating.Tests.Helpers;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class CompilerErrorTaskTests
	{
		[SetUp]
		public void Init()
		{
			SD.InitializeForUnitTests();
		}
		
		[TearDown]
		public void TearDown()
		{
			SD.TearDownForUnitTests();
		}
		
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
