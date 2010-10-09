// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
