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
using ICSharpCode.Python.Build.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace Python.Build.Tasks.Tests
{
	[TestFixture]
	public class LogicalResourceNamesTests
	{
		MockPythonCompiler mockCompiler;
		PythonCompilerTask compilerTask;
		
		void CreatePythonCompilerTask()
		{
			mockCompiler = new MockPythonCompiler();
			compilerTask = new PythonCompilerTask(mockCompiler);
			compilerTask.TargetType = "Exe";
			compilerTask.OutputAssembly = "test.exe";
		}
		
		[Test]
		public void Execute_ResourceHasLogicalNameSetInTaskItemMetadata_ResourceNamePassedToCompilerUsesLogicalName()
		{
			CreatePythonCompilerTask();
			
			TaskItem resourceTaskItem = new TaskItem("test.xaml");
			resourceTaskItem.SetMetadata("LogicalName", "MyLogicalResourceName");
			compilerTask.Resources = new ITaskItem[] {resourceTaskItem};
			compilerTask.Execute();
			
			ResourceFile resourceFile = mockCompiler.ResourceFiles[0];
			string resourceName = resourceFile.Name;
			
			string expectedResourceName = "MyLogicalResourceName";
			
			Assert.AreEqual(expectedResourceName, resourceName);
		}
	}
}
