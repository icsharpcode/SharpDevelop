// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using TextTemplating.Tests.Helpers;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class TextTemplatingFilePreprocessorCustomToolTests
	{
		TestableTextTemplatingFilePreprocessorCustomTool customTool;
		
		void CreateCustomTool()
		{
			customTool = new TestableTextTemplatingFilePreprocessorCustomTool();
		}
		
		FileProjectItem GenerateCodeWithProjectFile()
		{
			var file = new TestableFileProjectItem("test.tt");
			customTool.GenerateCode(file, null);
			return file;
		}
		
		[Test]
		public void GenerateCode_ProjectFilePassed_ProjectFileUsedToCreateTextTemplatingFilePreprocessor()
		{
			CreateCustomTool();
			var file = GenerateCodeWithProjectFile();
			
			Assert.AreEqual(file, customTool.TemplateFilePassedToCreateTextTemplatingFilePreprocessor);
		}
		
		[Test]
		public void GenerateCode_CustomToolContextPassed_CustomToolContextUsedToCreateTextTemplatingFilePreproressor()
		{
			CreateCustomTool();
			IProject project = ProjectHelper.CreateProject();
			var context = new CustomToolContext(project);
			customTool.GenerateCode(null, context);
			
			Assert.AreEqual(context, customTool.ContextPassedToCreateTextTemplatingFilePreprocessor);
		}
		
		[Test]
		public void GenerateCode_ProjectFilePassed_TemplateIsPreprocessed()
		{
			CreateCustomTool();
			GenerateCodeWithProjectFile();
			
			Assert.IsTrue(customTool.FakeTextTemplatingFilePreprocessor.IsPreprocessTemplateCalled);
		}
	}
}
