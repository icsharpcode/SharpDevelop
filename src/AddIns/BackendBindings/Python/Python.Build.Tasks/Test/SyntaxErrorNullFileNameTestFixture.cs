// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;
using System.Reflection.Emit;
using IronPython.Runtime;
using ICSharpCode.Python.Build.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;
using NUnit.Framework;

namespace Python.Build.Tasks.Tests
{
	[TestFixture]
	public class SyntaxErrorNullFileNameTestFixture
	{
		MockPythonCompiler mockCompiler;
		DummyPythonCompilerTask compiler;
		bool success;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			ScriptEngine engine = IronPython.Hosting.Python.CreateEngine();
		}
		
		[SetUp]
		public void Init()
		{
			mockCompiler = new MockPythonCompiler();
			compiler = new DummyPythonCompilerTask(mockCompiler, @"C:\Projects\MyProject");
			compiler.TargetType = "Exe";
			compiler.OutputAssembly = "test.exe";
			
			TaskItem sourceFile = new TaskItem(@"D:\Projects\MyProject\test.py");
			compiler.Sources = new ITaskItem[] {sourceFile};
			
			SourceUnit source = DefaultContext.DefaultPythonContext.CreateSourceUnit(NullTextContentProvider.Null, @"test", SourceCodeKind.InteractiveCode);
			
			SyntaxErrorException ex = new SyntaxErrorException("Error", null, SourceSpan.None, 1000, Severity.FatalError);
			mockCompiler.ThrowExceptionAtCompile = ex;
			
			success = compiler.Execute();
		}

		[Test]
		public void IsExceptionMessageLogged()
		{
			Assert.AreEqual("Error", compiler.LoggedErrorMessage);
		}
	}
}
