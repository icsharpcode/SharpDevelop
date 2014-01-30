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
	/// Tests that the python compiler task compiles a single source file.
	/// </summary>
	[TestFixture]
	public class CompileSingleSourceFileTestFixture
	{
		MockPythonCompiler mockCompiler;
		TaskItem sourceTaskItem;
		TaskItem systemXmlReferenceTaskItem;
		TaskItem systemDataReferenceTaskItem;
		PythonCompilerTask compiler;
		bool success;
		
		[SetUp]
		public void Init()
		{
			mockCompiler = new MockPythonCompiler();
			compiler = new PythonCompilerTask(mockCompiler);
			sourceTaskItem = new TaskItem("test.py");
			compiler.Sources = new ITaskItem[] {sourceTaskItem};
			compiler.TargetType = "Exe";
			compiler.OutputAssembly = "test.exe";
						
			systemDataReferenceTaskItem = new TaskItem(@"C:\Windows\Microsoft.NET\Framework\2.0\System.Data.dll");
			systemXmlReferenceTaskItem = new TaskItem(@"C:\Windows\Microsoft.NET\Framework\2.0\System.Xml.dll");
			compiler.References = new ITaskItem[] {systemDataReferenceTaskItem, systemXmlReferenceTaskItem};
			
			success = compiler.Execute();
		}
		
		[Test]
		public void CompilationSucceeded()
		{
			Assert.IsTrue(success);
		}
		
		[Test]
		public void OneSourceFile()
		{
			Assert.AreEqual(1, mockCompiler.SourceFiles.Count);
		}
		
		[Test]
		public void SourceFileName()
		{
			Assert.AreEqual("test.py", mockCompiler.SourceFiles[0]);
		}
		
		[Test]
		public void IsCompileCalled()
		{
			Assert.IsTrue(mockCompiler.CompileCalled);
		}

		[Test]
		public void IsDisposeCalled()
		{
			Assert.IsTrue(mockCompiler.DisposeCalled);
		}
		
		[Test]
		public void TargetKindIsExe()
		{
			Assert.AreEqual(PEFileKinds.ConsoleApplication, mockCompiler.TargetKind);
		}
		
		[Test]
		public void OutputAssembly()
		{
			Assert.AreEqual("test.exe", mockCompiler.OutputAssembly);
		}
		
		[Test]
		public void DebugInfo()
		{
			Assert.IsFalse(mockCompiler.IncludeDebugInformation);
		}
		
		[Test]
		public void TwoReferences()
		{
			Assert.AreEqual(2, mockCompiler.ReferencedAssemblies.Count);
		}
		
		[Test]
		public void PythonCompilerTaskReferences()
		{
			ITaskItem[] references = compiler.References;
			Assert.AreEqual(systemDataReferenceTaskItem, references[0]);
			Assert.AreEqual(systemXmlReferenceTaskItem, references[1]);
		}
	}
}
