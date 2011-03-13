// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextTemplating;
using NUnit.Framework;
using TextTemplating.Tests.Helpers;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class TextTemplatingFileGeneratorTests
	{
		TextTemplatingFileGenerator generator;
		FakeTextTemplatingHost templatingHost;
		FakeTextTemplatingCustomToolContext customToolContext;
		
		TestableFileProjectItem ProcessTemplate(string fileName)
		{
			var projectFile = CreateGenerator(fileName);
			generator.ProcessTemplate();
			
			return projectFile;
		}
		
		TestableFileProjectItem CreateGenerator(string fileName)
		{
			templatingHost = new FakeTextTemplatingHost();
			var projectFile = new TestableFileProjectItem(fileName);
			customToolContext = new FakeTextTemplatingCustomToolContext();
			
			generator = new TextTemplatingFileGenerator(templatingHost, projectFile, customToolContext);
			
			return projectFile;
		}

		[Test]
		public void ProcessTemplate_TemplateFileInProjectPassed_TemplatingHostProcessesFile()
		{
			string fileName = @"d:\projects\MyProject\template.tt";
			ProcessTemplate(fileName);
			
			Assert.AreEqual(fileName, templatingHost.InputFilePassedToProcessTemplate);
		}
		
		[Test]
		public void ProcessTemplate_TemplateFileInProjectPassed_OutputFileNamePassedIsInputFileWithFileExtensionChangedToCSharpFileExtension()
		{
			ProcessTemplate(@"d:\projects\MyProject\template.tt");
			
			string expectedOutputFileName = @"d:\projects\MyProject\template.cs";
			Assert.AreEqual(expectedOutputFileName, templatingHost.OutputFilePassedToProcessTemplate);
		}
		
		[Test]
		public void Dispose_TemplateHostUsedToCreateFileGenerator_TemplateHostIsDisposed()
		{
			CreateGenerator(@"d:\template.tt");
			generator.Dispose();
			
			Assert.IsTrue(templatingHost.IsDisposeCalled);
		}
		
		[Test]
		public void ProcessTemplate_TemplateFileInProjectPassed_TemplateFileInProjectUsedWhenCheckingIfOutputFileExistsInProject()
		{
			var file = ProcessTemplate(@"d:\template.tt");
			
			Assert.AreEqual(file, customToolContext.BaseItemPassedToEnsureOutputFileIsInProject);
		}
		
		[Test]
		public void ProcessTemplate_OutputFileNameChangedWhenTemplateProcessed_NewOutputFileNameIsUsedWhenCheckingIfOutputFileExistsInProject()
		{
			CreateGenerator(@"d:\template.tt");
			templatingHost.OutputFile = @"d:\changed-output.test";
			generator.ProcessTemplate();
			
			Assert.AreEqual(@"d:\changed-output.test", customToolContext.OutputFileNamePassedToEnsureOutputFileIsInProject);
		}
		
		[Test]
		public void ProcessTemplate_TemplateFileInProjectPassed_TaskServiceHasClearedTasksExceptForCommentTasks()
		{
			var file = ProcessTemplate(@"d:\template.tt");
			
			Assert.IsTrue(customToolContext.IsClearTasksExceptCommentTasksCalled);
		}
		
		[Test]
		public void ProcessTemplate_TemplateHostHasOneErrorAfterProcessing_ErrorTaskAddedToTaskList()
		{
			CreateGenerator(@"d:\a.tt");
			templatingHost.ErrorsCollection.Add(new CompilerError());
			generator.ProcessTemplate();
			
			Task task = customToolContext.FirstTaskAdded;
			
			Assert.AreEqual(TaskType.Error, task.TaskType);
		}
		
		[Test]
		public void ProcessTemplate_TemplateHostHasOneErrorAfterProcessing_ErrorDescriptionIsAddedToTaskList()
		{
			CreateGenerator(@"d:\a.tt");
			var error = new CompilerError();
			error.ErrorText = "error text";
			templatingHost.ErrorsCollection.Add(error);
			generator.ProcessTemplate();
			
			Task task = customToolContext.FirstTaskAdded;
			
			Assert.AreEqual("error text", task.Description);
		}
		
		[Test]
		public void ProcessTemplate_ThrowsInvalidOperationException_ErrorTaskAddedToTaskList()
		{
			CreateGenerator(@"d:\a.tt");
			var ex = new InvalidOperationException("invalid operation error");
			templatingHost.ExceptionToThrowWhenProcessTemplateCalled = ex;
			generator.ProcessTemplate();
			
			Task task = customToolContext.FirstTaskAdded;
			
			Assert.AreEqual("invalid operation error", task.Description);
		}
		
		[Test]
		public void ProcessTemplate_ThrowsInvalidOperationException_ErrorTaskAddedToTaskListWithTemplateFileName()
		{
			CreateGenerator(@"d:\a.tt");
			var ex = new InvalidOperationException("invalid operation error");
			templatingHost.ExceptionToThrowWhenProcessTemplateCalled = ex;
			generator.ProcessTemplate();
			
			Task task = customToolContext.FirstTaskAdded;
			var expectedFileName = new FileName(@"d:\a.tt");
			
			Assert.AreEqual(expectedFileName, task.FileName);
		}
		
		[Test]
		public void ProcessTemplate_MethodReturnsFalse_OutputFileNotAddedToProject()
		{
			CreateGenerator(@"d:\a.tt");
			templatingHost.ProcessTemplateReturnValue = false;
			generator.ProcessTemplate();

			Assert.IsFalse(customToolContext.IsOutputFileNamePassedToEnsureOutputFileIsInProject);
		}
		
		[Test]
		public void ProcessTemplate_TemplateHostHasOneErrorAfterProcessing_ErrorsPadBroughtToFront()
		{
			CreateGenerator(@"d:\a.tt");
			templatingHost.Errors.Add(new CompilerError());
			generator.ProcessTemplate();
			
			Assert.IsTrue(customToolContext.IsBringErrorsPadToFrontCalled);
		}
		
		[Test]
		public void ProcessTemplate_NoErrorsAfterProcessing_ErrorsPadNotBroughtToFront()
		{
			CreateGenerator(@"d:\a.tt");
			generator.ProcessTemplate();
			
			Assert.IsFalse(customToolContext.IsBringErrorsPadToFrontCalled);
		}
		
		[Test]
		public void ProcessTemplate_ThrowsInvalidOperationException_ExceptionLogged()
		{
			CreateGenerator(@"d:\a.tt");
			var ex = new InvalidOperationException("invalid operation error");
			templatingHost.ExceptionToThrowWhenProcessTemplateCalled = ex;
			generator.ProcessTemplate();
			
			Exception exceptionLogged = customToolContext.ExceptionPassedToDebugLog;
			
			Assert.AreEqual(ex, exceptionLogged);
		}
		
		[Test]
		public void ProcessTemplate_ThrowsInvalidOperationException_MessageLoggedIncludesInformationAboutProcessingTemplateFailure()
		{
			CreateGenerator(@"d:\a.tt");
			var ex = new InvalidOperationException("invalid operation error");
			templatingHost.ExceptionToThrowWhenProcessTemplateCalled = ex;
			generator.ProcessTemplate();
			
			string message = customToolContext.MessagePassedToDebugLog;
			string expectedMessage = "Exception thrown when processing template 'd:\\a.tt'.";
			
			Assert.AreEqual(expectedMessage, message);
		}
		
		[Test]
		public void ProcessTemplate_ThrowsException_ErrorTaskAddedToTaskList()
		{
			CreateGenerator(@"d:\a.tt");
			var ex = new Exception("error");
			templatingHost.ExceptionToThrowWhenProcessTemplateCalled = ex;
			generator.ProcessTemplate();
			
			Task task = customToolContext.FirstTaskAdded;
			
			Assert.AreEqual("error", task.Description);
		}
		
		[Test]
		public void ProcessTemplate_CustomToolNamespaceNotSet_LogicalCallContextNamespaceHintDataIsSet()
		{
			var templateFile = CreateGenerator(@"d:\a.tt");
			templateFile.Project.Name = "Test";
			generator.ProcessTemplate();
			
			Assert.AreEqual("NamespaceHint", customToolContext.NamePassedToSetLogicalCallContextData);
		}
		
		[Test]
		public void ProcessTemplate_CustomToolNamespaceNotSet_LogicalCallContextNamespaceHintDataIsSetBefore()
		{
			var templateFile = CreateGenerator(@"d:\a.tt");
			templateFile.Project.RootNamespace = "Test";
			generator.ProcessTemplate();
			
			string data = customToolContext.DataPassedToSetLogicalCallContextData as String;
			
			Assert.AreEqual("Test", data);
		}
	}
}