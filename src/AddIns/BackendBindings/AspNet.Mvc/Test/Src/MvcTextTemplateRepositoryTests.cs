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
using System.Collections.Generic;
using System.Linq;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;
using Rhino.Mocks;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcTextTemplateRepositoryTests
	{
		MvcTextTemplateRepository repository;
		IFileSystem fakeFileSystem;
		
		void CreateRepositoryWithAspNetMvcAddInDirectory(string mvcAddInPath)
		{
			fakeFileSystem = MockRepository.GenerateStub<IFileSystem>();
			repository = new MvcTextTemplateRepository(mvcAddInPath, fakeFileSystem);
		}
		
		void AddTextTemplatesToFolder(string path, params string[] fileNames)
		{
			DirectoryName templateFolder = DirectoryName.Create(path);
			var templateFileNames = fileNames.Select(FileName.Create).ToArray();
			fakeFileSystem.Stub(f => f.GetFiles(templateFolder, "*.tt")).Return(templateFileNames);
		}
		
		[Test]
		public void GetMvcViewTextTemplateFileName_CSharpEmptyTemplateRequested_ReturnsFileNameToCSharpEmptyTextTemplate()
		{
			CreateRepositoryWithAspNetMvcAddInDirectory(@"C:\SD\AddIns\AspNet.Mvc");
			
			var templateCriteria = new MvcTextTemplateCriteria() {
				TemplateLanguage = MvcTextTemplateLanguage.CSharp,
				TemplateName = "Empty",
				TemplateType = MvcTextTemplateType.Aspx
			};
			
			string fileName = repository.GetMvcViewTextTemplateFileName(templateCriteria);
			
			string expectedFileName = 
				@"C:\SD\AddIns\AspNet.Mvc\ItemTemplates\CSharp\CodeTemplates\AddView\AspxCSharp\Empty.tt";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetMvcViewTextTemplateFileName_VisualBasicEmptyTemplateRequested_ReturnsFileNameToVisualBasicEmptyTextTemplate()
		{
			CreateRepositoryWithAspNetMvcAddInDirectory(@"C:\SD\AddIns\AspNet.Mvc");
			
			var templateCriteria = new MvcTextTemplateCriteria() {
				TemplateLanguage = MvcTextTemplateLanguage.VisualBasic,
				TemplateName = "Empty",
				TemplateType = MvcTextTemplateType.Aspx
			};
			
			string fileName = repository.GetMvcViewTextTemplateFileName(templateCriteria);
			
			string expectedFileName = 
				@"C:\SD\AddIns\AspNet.Mvc\ItemTemplates\VisualBasic\CodeTemplates\AddView\AspxVisualBasic\Empty.tt";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetMvcControllerTextTemplateFileName_CSharpControllerTemplateRequested_ReturnsFileNameToCSharpControllerTextTemplate()
		{
			CreateRepositoryWithAspNetMvcAddInDirectory(@"C:\SD\AddIns\AspNet.Mvc");
			
				var templateCriteria = new MvcTextTemplateCriteria() {
				TemplateLanguage = MvcTextTemplateLanguage.CSharp,
				TemplateName = "Controller",
				TemplateType = MvcTextTemplateType.Aspx
			};

			string fileName = repository.GetMvcControllerTextTemplateFileName(templateCriteria);
			
			string expectedFileName = 
				@"C:\SD\AddIns\AspNet.Mvc\ItemTemplates\CSharp\CodeTemplates\AddController\Controller.tt";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetMvcControllerTextTemplateFileName_VisualBasicControllerTemplateRequested_ReturnsFileNameToVisualBasicControllerTextTemplate()
		{
			CreateRepositoryWithAspNetMvcAddInDirectory(@"C:\SD\AddIns\AspNet.Mvc");
			
			var templateCriteria = new MvcTextTemplateCriteria() {
				TemplateLanguage = MvcTextTemplateLanguage.VisualBasic,
				TemplateName = "Controller",
				TemplateType = MvcTextTemplateType.Aspx
			};
			
			string fileName = repository.GetMvcControllerTextTemplateFileName(templateCriteria);
			
			string expectedFileName = 
				@"C:\SD\AddIns\AspNet.Mvc\ItemTemplates\VisualBasic\CodeTemplates\AddController\Controller.tt";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetMvcViewTextTemplateFileName_CSharpEmptyRazorTemplateRequested_ReturnsFileNameToCSharpEmptyRazorTextTemplate()
		{
			CreateRepositoryWithAspNetMvcAddInDirectory(@"C:\SD\AddIns\AspNet.Mvc");
			
			var templateCriteria = new MvcTextTemplateCriteria() {
				TemplateLanguage = MvcTextTemplateLanguage.CSharp,
				TemplateName = "Empty",
				TemplateType = MvcTextTemplateType.Razor
			};
			
			string fileName = repository.GetMvcViewTextTemplateFileName(templateCriteria);
			
			string expectedFileName = 
				@"C:\SD\AddIns\AspNet.Mvc\ItemTemplates\CSharp\CodeTemplates\AddView\CSHTML\Empty.tt";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetMvcViewTextTemplateFileName_VisualBasicEmptyRazorTemplateRequested_ReturnsFileNameToVisualBasicEmptyRazorTextTemplate()
		{
			CreateRepositoryWithAspNetMvcAddInDirectory(@"C:\SD\AddIns\AspNet.Mvc");
			
			var templateCriteria = new MvcTextTemplateCriteria() {
				TemplateLanguage = MvcTextTemplateLanguage.VisualBasic,
				TemplateName = "Empty",
				TemplateType = MvcTextTemplateType.Razor
			};
			
			string fileName = repository.GetMvcViewTextTemplateFileName(templateCriteria);
			
			string expectedFileName = 
				@"C:\SD\AddIns\AspNet.Mvc\ItemTemplates\VisualBasic\CodeTemplates\AddView\VBHTML\Empty.tt";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetMvcControllerTextTemplateFileName_RazorCSharpControllerTemplateRequested_ReturnsFileNameToCSharpControllerTextTemplate()
		{
			CreateRepositoryWithAspNetMvcAddInDirectory(@"C:\SD\AddIns\AspNet.Mvc");
			
			var templateCriteria = new MvcTextTemplateCriteria() {
				TemplateLanguage = MvcTextTemplateLanguage.CSharp,
				TemplateName = "Controller",
				TemplateType = MvcTextTemplateType.Razor
			};
			
			string fileName = repository.GetMvcControllerTextTemplateFileName(templateCriteria);
			
			string expectedFileName = 
				@"C:\SD\AddIns\AspNet.Mvc\ItemTemplates\CSharp\CodeTemplates\AddController\Controller.tt";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetMvcControllerTextTemplates_CSharpAspxTemplatesRequestedAndOneControllerTemplateInFolder_ReturnsTwoCSharpAspxControllerTextTemplates()
		{
			CreateRepositoryWithAspNetMvcAddInDirectory(@"C:\SD\AddIns\AspNet.Mvc");
			
			var templateCriteria = new MvcTextTemplateCriteria() {
				TemplateLanguage = MvcTextTemplateLanguage.CSharp,
				TemplateType = MvcTextTemplateType.Aspx
			};
			
			List<MvcControllerTextTemplate> templates = repository.GetMvcControllerTextTemplates(templateCriteria).ToList();
			
			string existingTemplateFileName = 
				@"C:\SD\AddIns\AspNet.Mvc\ItemTemplates\CSharp\CodeTemplates\AddController\Controller.tt";
			
			var expectedTemplate1 = new MvcControllerTextTemplate() {
				Name = "Empty",
				Description = "Empty controller",
				FileName = existingTemplateFileName,
				AddActionMethods = false
			};
			var expectedTemplate2 = new MvcControllerTextTemplate() {
				Name = "EmptyReadWrite",
				Description = "Controller with create, read, update and delete actions",
				FileName = existingTemplateFileName,
				AddActionMethods = true
			};
			var expectedTemplates = new MvcControllerTextTemplate[] {
				expectedTemplate1,
				expectedTemplate2
			};
			
			MvcControllerTextTemplateCollectionAssert.AreEqual(expectedTemplates, templates);
		}
		
		[Test]
		public void GetMvcViewTextTemplates_CSharpAspxTemplatesRequestedAndOneViewTemplateInFolder_ReturnsOneViewTextTemplate()
		{
			CreateRepositoryWithAspNetMvcAddInDirectory(@"C:\SD\AddIns\AspNet.Mvc");
			string templateFolder = 
				@"C:\SD\AddIns\AspNet.Mvc\ItemTemplates\CSharp\CodeTemplates\AddView\AspxCSharp";
			string existingTemplateFileName = 
				@"C:\SD\AddIns\AspNet.Mvc\ItemTemplates\CSharp\CodeTemplates\AddView\AspxCSharp\Empty.tt";
			AddTextTemplatesToFolder(templateFolder, existingTemplateFileName);
			
			var templateCriteria = new MvcTextTemplateCriteria() {
				TemplateLanguage = MvcTextTemplateLanguage.CSharp,
				TemplateType = MvcTextTemplateType.Aspx
			};
			
			List<MvcViewTextTemplate> templates = repository.GetMvcViewTextTemplates(templateCriteria).ToList();
			
			var expectedTemplate = new MvcViewTextTemplate() {
				Name = "Empty",
				FileName = existingTemplateFileName
			};
			var expectedTemplates = new MvcViewTextTemplate[] {
				expectedTemplate
			};
			
			MvcViewTextTemplateCollectionAssert.AreEqual(expectedTemplates, templates);
		}
		
		[Test]
		public void GetMvcViewTextTemplates_CSharpRazorTemplatesRequestedAndTwoViewTemplatesInFolder_ReturnsTwoViewTextTemplates()
		{
			CreateRepositoryWithAspNetMvcAddInDirectory(@"C:\SD\AddIns\AspNet.Mvc");
			string templateFolder = 
				@"C:\SD\AddIns\AspNet.Mvc\ItemTemplates\CSharp\CodeTemplates\AddView\CSHTML";
			string[] templateFileNames = new string[] {
				@"C:\SD\AddIns\AspNet.Mvc\ItemTemplates\CSharp\CodeTemplates\AddView\CSHTML\B.tt",
				@"C:\SD\AddIns\AspNet.Mvc\ItemTemplates\CSharp\CodeTemplates\AddView\CSHTML\A.tt"
			};
			AddTextTemplatesToFolder(templateFolder, templateFileNames);
			
			var templateCriteria = new MvcTextTemplateCriteria() {
				TemplateLanguage = MvcTextTemplateLanguage.CSharp,
				TemplateType = MvcTextTemplateType.Razor
			};
			
			List<MvcViewTextTemplate> templates = repository.GetMvcViewTextTemplates(templateCriteria).ToList();
			
			var expectedTemplate1 = new MvcViewTextTemplate() {
				Name = "B",
				FileName = templateFileNames[0]
			};
			var expectedTemplate2 = new MvcViewTextTemplate() {
				Name = "A",
				FileName = templateFileNames[1]
			};
			var expectedTemplates = new MvcViewTextTemplate[] {
				expectedTemplate1,
				expectedTemplate2
			};
			
			MvcViewTextTemplateCollectionAssert.AreEqual(expectedTemplates, templates);
		}
	}
}
