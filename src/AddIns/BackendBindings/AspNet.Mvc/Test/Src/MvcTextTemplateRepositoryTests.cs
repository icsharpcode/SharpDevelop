// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcTextTemplateRepositoryTests
	{
		MvcTextTemplateRepository repository;
		
		void CreateRepositoryWithAspNetMvcAddInDirectory(string mvcAddInPath)
		{
			repository = new MvcTextTemplateRepository(mvcAddInPath);
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
	}
}
