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
using System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.Python.Build.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace Python.Build.Tasks.Tests
{
	/// <summary>
	/// Tests the platform information (e.g. x86) is correctly passed to the IPythonCompiler.
	/// </summary>
	[TestFixture]
	public class PlatformTestFixture
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
		public void DefaultPlatformIsILOnly()
		{
			compilerTask.Execute();
			Assert.AreEqual(PortableExecutableKinds.ILOnly, mockCompiler.ExecutableKind);
		}
		
		[Test]
		public void DefaultMachineIs386()
		{
			compilerTask.Execute();
			Assert.AreEqual(ImageFileMachine.I386, mockCompiler.Machine);
		}
		
		[Test]
		public void ExecutableIsCompiledTo32Bit()
		{
			compilerTask.Platform = "x86";
			compilerTask.Execute();
			
			Assert.AreEqual(PortableExecutableKinds.ILOnly | PortableExecutableKinds.Required32Bit, mockCompiler.ExecutableKind);
		}		
		
		[Test]
		public void MachineWhenExecutableIsCompiledTo32Bit()
		{
			compilerTask.Platform = "x86";
			compilerTask.Execute();
			
			Assert.AreEqual(ImageFileMachine.I386, mockCompiler.Machine);
		}
		
		[Test]
		public void ExecutableIsCompiledToItanium()
		{
			compilerTask.Platform = "Itanium";
			compilerTask.Execute();
			
			Assert.AreEqual(PortableExecutableKinds.ILOnly | PortableExecutableKinds.PE32Plus, mockCompiler.ExecutableKind);
		}		

		[Test]
		public void MachineWhenExecutableIsCompiledToItanium()
		{
			compilerTask.Platform = "Itanium";
			compilerTask.Execute();
			
			Assert.AreEqual(ImageFileMachine.IA64, mockCompiler.Machine);
		}		
		
		[Test]
		public void ExecutableIsCompiledTo64Bit()
		{
			compilerTask.Platform = "x64";
			compilerTask.Execute();
			
			Assert.AreEqual(PortableExecutableKinds.ILOnly | PortableExecutableKinds.PE32Plus, mockCompiler.ExecutableKind);
		}		

		[Test]
		public void MachineWhenExecutableIsCompiledTo64Bit()
		{
			compilerTask.Platform = "x64";
			compilerTask.Execute();
			
			Assert.AreEqual(ImageFileMachine.AMD64, mockCompiler.Machine);
		}			
	}
}
