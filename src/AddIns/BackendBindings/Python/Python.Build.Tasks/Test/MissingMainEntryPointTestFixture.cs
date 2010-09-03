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
	/// Tests that an error is reported when the mainFile is missing and we are trying to compile
	/// an executable.
	/// </summary>
	[TestFixture]
	public class MissingMainEntryPointTestFixture
	{
		MockPythonCompiler mockCompiler;
		DummyPythonCompilerTask compiler;
		bool success;
		
		[SetUp]
		public void Init()
		{
			mockCompiler = new MockPythonCompiler();
			compiler = new DummyPythonCompilerTask(mockCompiler, @"C:\Projects\MyProject");
			compiler.TargetType = "Exe";
			compiler.OutputAssembly = "test.exe";
			
			TaskItem sourceFile = new TaskItem(@"D:\Projects\MyProject\test.py");
			compiler.Sources = new ITaskItem[] {sourceFile};
			
			mockCompiler.ThrowExceptionAtCompile = new PythonCompilerException("Missing main file.");
			
			success = compiler.Execute();
		}
		
		[Test]
		public void ExecuteFailed()
		{
			Assert.IsFalse(success);
		}
		
		[Test]
		public void IsExceptionMessageLogged()
		{
			Assert.AreEqual("Missing main file.", compiler.LoggedErrorMessage);
		}
	}
}
