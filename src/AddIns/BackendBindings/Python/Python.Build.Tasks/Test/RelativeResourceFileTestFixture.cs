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
	/// Tests that resoures with a relative path are converted to a full path before being
	/// passed to the PythonCompiler.
	/// </summary>
	[TestFixture]
	public class RelativeResourceFileTestFixture
	{
		MockPythonCompiler mockCompiler;
		TaskItem resourceTaskItem;
		TaskItem fullPathResourceTaskItem;
		DummyPythonCompilerTask compiler;
		
		[SetUp]
		public void Init()
		{
			mockCompiler = new MockPythonCompiler();
			compiler = new DummyPythonCompilerTask(mockCompiler, @"C:\Projects\MyProject");
			compiler.TargetType = "Exe";
			compiler.OutputAssembly = "test.exe";
			
			resourceTaskItem = new TaskItem(@"..\RequiredLibraries\MyResource.resx");
			fullPathResourceTaskItem = new TaskItem(@"C:\Projects\Test\MyTest.resx");
			compiler.Resources = new ITaskItem[] {resourceTaskItem, fullPathResourceTaskItem};
			
			compiler.Execute();
		}
		
		[Test]
		public void RelativePathReferenceItemPassedToCompilerWithFullPath()
		{
			string fileName = mockCompiler.ResourceFiles[0].FileName;
			Assert.AreEqual(@"C:\Projects\RequiredLibraries\MyResource.resx", fileName);
		}
		
		[Test]
		public void FullPathReferenceItemUnchangedWhenPassedToCompiler()
		{
			string fileName = mockCompiler.ResourceFiles[1].FileName;
			Assert.AreEqual(fullPathResourceTaskItem.ItemSpec, fileName);
		}
	}
}
