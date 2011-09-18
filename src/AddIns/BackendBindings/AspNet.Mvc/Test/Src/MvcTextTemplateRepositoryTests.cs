// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcTextTemplateRepositoryTests
	{
		MvcTextTemplateRepository repository;
		FakeFileSystem fakeFileSystem;
		
		void CreateRepositoryWithAspNetMvcAddInDirectory(string mvcAddInPath)
		{
			fakeFileSystem = new FakeFileSystem();
			repository = new MvcTextTemplateRepository(mvcAddInPath, fakeFileSystem);
		}
		
		void AddCSharpAspxTemplateToFolder(string path, string fileName)
		{
			string[] fileNames = new string[] {
				fileName
			};
			AddCSharpAspxTemplatesToFolder(path, fileNames);
		}
		
		void AddCSharpAspxTemplatesToFolder(string path, string[] fileNames)
		{
			fakeFileSystem.AddFakeFiles(path, "*.tt", fileNames);
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
	}
}
