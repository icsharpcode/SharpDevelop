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
	/// Tests that the main entry point is set in the PythonCompiler
	/// by the PythonCompilerTask.
	/// </summary>
	[TestFixture]
	public class MainEntryPointTestFixture
	{
		MockPythonCompiler mockCompiler;
		PythonCompilerTask compilerTask;
		TaskItem mainTaskItem;
		TaskItem classTaskItem;
		
		[SetUp]
		public void Init()
		{
			mockCompiler = new MockPythonCompiler();
			compilerTask = new PythonCompilerTask(mockCompiler);
			mainTaskItem = new TaskItem("main.py");
			classTaskItem = new TaskItem("class1.py");
			compilerTask.Sources = new ITaskItem[] {mainTaskItem, classTaskItem};
			compilerTask.MainFile = "main.py";
			compilerTask.Execute();
		}
		
		[Test]
		public void MainFile()
		{
			Assert.AreEqual("main.py", mockCompiler.MainFile);
		}
		
		[Test]
		public void TaskMainFile()
		{
			Assert.AreEqual("main.py", compilerTask.MainFile);
		}
		
		[Test]
		public void TaskSources()
		{
			ITaskItem[] sources = compilerTask.Sources;
			Assert.AreEqual(sources[0], mainTaskItem);
			Assert.AreEqual(sources[1], classTaskItem);
		}
	}
}
