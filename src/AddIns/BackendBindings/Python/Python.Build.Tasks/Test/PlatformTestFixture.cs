// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
