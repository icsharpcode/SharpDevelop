// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
