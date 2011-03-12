// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using TextTemplating.Tests.Helpers;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class TextTemplatingFileGeneratorCustomToolTests
	{
		TestableTextTemplatingFileGeneratorCustomTool customTool;
		
		void CreateCustomTool()
		{
			customTool = new TestableTextTemplatingFileGeneratorCustomTool();
		}
		
		FileProjectItem GenerateCodeWithProjectFile()
		{
			var file = new TestableFileProjectItem("test.tt");
			customTool.GenerateCode(file, null);
			return file;
		}
		
		[Test]
		public void GenerateCode_ProjectFilePassed_ProjectFileUsedToCreateTextTemplatingFileGenerator()
		{
			CreateCustomTool();
			var file = GenerateCodeWithProjectFile();
			
			Assert.AreEqual(file, customTool.ProjectFilePassedToCreateTextTemplatingFileGenerator);
		}
		
		[Test]
		public void GenerateCode_CustomToolContextPassed_CustomToolContextUsedToCreateTextTemplatingFileGenerator()
		{
			CreateCustomTool();
			IProject project = ProjectHelper.CreateProject();
			var context = new CustomToolContext(project);
			customTool.GenerateCode(null, context);
			
			Assert.AreEqual(context, customTool.ContextPassedToCreateTextTemplatingFileGenerator);
		}
		
		[Test]
		public void GenerateCode_ProjectFilePassed_TemplateIsProcessed()
		{
			CreateCustomTool();
			GenerateCodeWithProjectFile();
			
			Assert.IsTrue(customTool.FakeTextTemplatingFileGenerator.IsProcessTemplateCalled);
		}
		
		[Test]
		public void GenerateCode_ProjectFilePassed_TextTemplatingFileGeneratorIsDisposedAfterTemplateIsProcessed()
		{
			CreateCustomTool();
			GenerateCodeWithProjectFile();
			
			Assert.IsTrue(customTool.FakeTextTemplatingFileGenerator.IsDisposeCalledAfterTemplateProcessed);
		}
	}
}
