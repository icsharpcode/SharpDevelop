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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using TextTemplating.Tests.Helpers;

namespace TextTemplating.Tests
{
	[TestFixture]
	public class TextTemplatingFileGeneratorCustomToolTests
	{
		TestableTextTemplatingFileGeneratorCustomTool customTool;
		
		[SetUp]
		public void Init()
		{
			SD.InitializeForUnitTests();
		}
		
		[TearDown]
		public void TearDown()
		{
			SD.TearDownForUnitTests();
		}
		
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
