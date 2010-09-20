// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
