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
	/// Tests that the PythonCompiler correctly compiles to a 
	/// windows app when the TargetType is set to WinExe".
	/// </summary>
	[TestFixture]
	public class DifferentTargetTypesTestFixture
	{
		MockPythonCompiler mockCompiler;
		TaskItem sourceTaskItem;
		PythonCompilerTask compilerTask;
		
		[SetUp]
		public void Init()
		{
			mockCompiler = new MockPythonCompiler();
			compilerTask = new PythonCompilerTask(mockCompiler);
			sourceTaskItem = new TaskItem("test.py");
			compilerTask.Sources = new ITaskItem[] {sourceTaskItem};
		}
		
		[Test]
		public void CompiledToWindowsApp()
		{
			compilerTask.TargetType = "WinExe";
			compilerTask.Execute();
			
			Assert.AreEqual(PEFileKinds.WindowApplication, mockCompiler.TargetKind);
		}
		
		[Test]
		public void CompiledToWindowsAppWhenTargetTypeLowerCase()
		{
			compilerTask.TargetType = "winexe";
			compilerTask.Execute();
			
			Assert.AreEqual(PEFileKinds.WindowApplication, mockCompiler.TargetKind);
		}
		
		[Test]
		public void CompiledToDll()
		{
			compilerTask.TargetType = "Library";
			compilerTask.Execute();
			
			Assert.AreEqual(PEFileKinds.Dll, mockCompiler.TargetKind);
		}		
		
		[Test]
		public void NullTargetTypeCompilesToConsoleApp()
		{
			compilerTask.TargetType = null;
			compilerTask.Execute();
			
			Assert.AreEqual(PEFileKinds.ConsoleApplication, mockCompiler.TargetKind);			
		}
	}
}
