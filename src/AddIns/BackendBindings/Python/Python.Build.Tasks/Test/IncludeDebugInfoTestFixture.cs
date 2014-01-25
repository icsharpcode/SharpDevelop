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
using System.Reflection.Emit;
using ICSharpCode.Python.Build.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace Python.Build.Tasks.Tests
{
	/// <summary>
	/// Tests that the compiler includes debug info in the 
	/// generated assembly.
	/// </summary>
	[TestFixture]
	public class IncludeDebugInfoTestFixture
	{
		MockPythonCompiler mockCompiler;
		TaskItem sourceTaskItem;
		PythonCompilerTask compiler;
		
		[SetUp]
		public void Init()
		{
			mockCompiler = new MockPythonCompiler();
			compiler = new PythonCompilerTask(mockCompiler);
			sourceTaskItem = new TaskItem("test.py");
			compiler.Sources = new ITaskItem[] {sourceTaskItem};
			compiler.TargetType = "Exe";
			compiler.OutputAssembly = "test.exe";
			compiler.EmitDebugInformation = true;
			
			compiler.Execute();
		}
		
		[Test]
		public void DebugInfoIncluded()
		{
			Assert.IsTrue(mockCompiler.IncludeDebugInformation);
		}

		[Test]
		public void PythonCompilerTaskTargetType()
		{
			Assert.AreEqual("Exe", compiler.TargetType);
		}
		
		[Test]
		public void PythonCompilerTaskOutputAssembly()
		{
			Assert.AreEqual("test.exe", compiler.OutputAssembly);
		}

		[Test]
		public void PythonCompilerTaskEmitDebugInfo()
		{
			Assert.AreEqual(true, compiler.EmitDebugInformation);
		}		
	}
}
