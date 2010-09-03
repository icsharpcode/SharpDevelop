// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection.Emit;
using ICSharpCode.Python.Build.Tasks;
using IronPython.Hosting;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace Python.Build.Tasks.Tests
{
	/// <summary>
	/// Tests that resources are compiled using the PythonCompilerTask.
	/// </summary>
	[TestFixture]
	public class CompileResourcesTestFixture
	{
		MockPythonCompiler mockCompiler;
		TaskItem resourceTaskItem;
		ResourceFile resourceFile;
		PythonCompilerTask compiler;
		
		[SetUp]
		public void Init()
		{
			mockCompiler = new MockPythonCompiler();
			compiler = new PythonCompilerTask(mockCompiler);
			TaskItem sourceTaskItem = new TaskItem("test.py");
			
			compiler.Sources = new ITaskItem[] {sourceTaskItem};
			compiler.TargetType = "Exe";
			compiler.OutputAssembly = "test.exe";
			
			resourceTaskItem = new TaskItem(@"C:\Projects\Test\Test.resources");
			compiler.Resources = new ITaskItem[] {resourceTaskItem};
			
			compiler.Execute();
			
			if (mockCompiler.ResourceFiles != null && mockCompiler.ResourceFiles.Count > 0) {
				resourceFile = mockCompiler.ResourceFiles[0];
			}
		}
		
		[Test]
		public void OneResourceFile()
		{
			Assert.AreEqual(1, mockCompiler.ResourceFiles.Count);
		}
		
		[Test]
		public void ResourceFileName()
		{
			Assert.AreEqual(resourceTaskItem.ItemSpec, resourceFile.FileName);
		}
		
		/// <summary>
		/// The resource name should be the same as the filename without
		/// any preceding path information.
		/// </summary>
		[Test]
		public void ResourceName()
		{
			Assert.AreEqual("Test.resources", resourceFile.Name);
		}
		
		[Test]
		public void CompilerTaskResources()
		{
			ITaskItem[] resources = compiler.Resources;
			Assert.AreEqual(resourceTaskItem, resources[0]);
		}
	}
}
