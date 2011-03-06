// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
		
		FileProjectItem ProcessTemplate(string fileName)
		{
			var projectFile = CreateGenerator(fileName);
			generator.ProcessTemplate();
			
			return projectFile;
		}
		
		FileProjectItem CreateGenerator(string fileName)
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
	}
}