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
	/// Tests that references with a relative path are converted to a full path before being
	/// passed to the PythonCompiler.
	/// </summary>
	[TestFixture]
	public class RelativeReferenceTestFixture
	{
		MockPythonCompiler mockCompiler;
		TaskItem referenceTaskItem;
		TaskItem fullPathReferenceTaskItem;
		DummyPythonCompilerTask compiler;
		
		[SetUp]
		public void Init()
		{
			mockCompiler = new MockPythonCompiler();
			compiler = new DummyPythonCompilerTask(mockCompiler, @"C:\Projects\MyProject");
			compiler.TargetType = "Exe";
			compiler.OutputAssembly = "test.exe";
			
			referenceTaskItem = new TaskItem(@"..\RequiredLibraries\MyReference.dll");
			fullPathReferenceTaskItem = new TaskItem(@"C:\Projects\Test\MyTest.dll");
			compiler.References = new ITaskItem[] {referenceTaskItem, fullPathReferenceTaskItem};
			
			compiler.Execute();
		}
		
		[Test]
		public void RelativePathReferenceItemPassedToCompilerWithFullPath()
		{
			string fileName = mockCompiler.ReferencedAssemblies[0];
			Assert.AreEqual(@"C:\Projects\RequiredLibraries\MyReference.dll", fileName);
		}
		
		[Test]
		public void FullPathReferenceItemUnchangedWhenPassedToCompiler()
		{
			string fileName = mockCompiler.ReferencedAssemblies[1];
			Assert.AreEqual(fullPathReferenceTaskItem.ItemSpec, fileName);
		}
	}
}
