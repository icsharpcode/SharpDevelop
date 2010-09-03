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
