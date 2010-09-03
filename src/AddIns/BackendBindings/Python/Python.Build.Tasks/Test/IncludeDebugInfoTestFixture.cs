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
