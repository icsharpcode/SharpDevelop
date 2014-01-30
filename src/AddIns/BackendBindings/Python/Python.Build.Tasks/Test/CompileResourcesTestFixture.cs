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
