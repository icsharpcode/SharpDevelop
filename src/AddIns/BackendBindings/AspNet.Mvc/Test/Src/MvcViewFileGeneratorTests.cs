// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcViewFileGeneratorTests
	{
		MvcViewFileGenerator generator;
		TestableProject projectUsedByGenerator;
		MvcTextTemplateRepository templateRepository;
		FakeMvcTextTemplateHostFactory fakeHostFactory;
		FakeMvcTextTemplateHost fakeHost;
		
		void CreateTemplateRepository(string templateRootDirectory)
		{
			templateRepository = new MvcTextTemplateRepository(templateRootDirectory);
		}
		
		void CreateGenerator()
		{
			CreateTemplateRepository(@"d:\sd\addins\AspNet.Mvc");
			CreateGenerator(templateRepository);
		}
		
		void CreateGenerator(MvcTextTemplateRepository templateRepository)
		{
			fakeHostFactory = new FakeMvcTextTemplateHostFactory();
			fakeHost = fakeHostFactory.FakeMvcTextTemplateHost;
			generator = new MvcViewFileGenerator(fakeHostFactory, templateRepository);
			projectUsedByGenerator = TestableProject.CreateProject();
			generator.Project = projectUsedByGenerator;
			ProjectPassedToGeneratorIsCSharpProject();
		}
		
		void ProjectPassedToGeneratorIsCSharpProject()
		{
			projectUsedByGenerator.SetLanguage("C#");
			generator.TemplateLanguage = MvcTextTemplateLanguage.CSharp;			
		}
		
		void SelectAspxTemplate()
		{
			generator.TemplateType = MvcTextTemplateType.Aspx;
		}
		
		void SelectRazorTemplate()
		{
			generator.TemplateType = MvcTextTemplateType.Razor;
		}
		
		void GenerateFile()
		{
			GenerateFile(@"d:\projects\myproject\Views\Home", "Index");
		}
		
		void GenerateFile(string folder, string name)
		{
			var fileName = new MvcViewFileName();
			fileName.Folder = folder;
			fileName.ViewName = name;
			GenerateFile(fileName);
		}
		
		void GenerateFile(MvcViewFileName fileName)
		{
			generator.GenerateFile(fileName);
		}
		
		[Test]
		public void GenerateFile_CSharpEmptyViewTemplate_MvcTextTemplateHostIsCreated()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			GenerateFile();
			IProject project = fakeHostFactory.ProjectPassedToCreateMvcTextTemplateHost;
			
			Assert.AreEqual(projectUsedByGenerator, project);
		}
		
		[Test]
		public void GenerateFile_CSharpEmptyViewTemplate_MvcTextTemplateHostIsDisposed()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			GenerateFile();
			bool disposed = fakeHost.IsDisposed;
			
			Assert.IsTrue(disposed);
		}
		
		[Test]
		public void GenerateFile_CSharpEmptyAspxViewTemplate_OutputFileGeneratedUsingFileNamePassedToGenerator()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			SelectAspxTemplate();
			
			string viewFolder = @"d:\projects\MyProject\Views\Home";
			string viewName = "Index";
			GenerateFile(viewFolder, viewName);
			
			string outputFileGenerated = fakeHost.OutputFilePassedToProcessTemplate;
			string expectedOutputFileGenerated = 
				@"d:\projects\MyProject\Views\Home\Index.aspx";
			
			Assert.AreEqual(expectedOutputFileGenerated, outputFileGenerated);
		}
		
		[Test]
		public void GenerateFile_CSharpEmptyAspxViewTemplate_TemplateFileUsedIsIsCSharpEmptyAspxViewTemplate()
		{
			string templateRootDirectory = @"d:\SharpDev\AddIns\AspNet.Mvc";
			CreateTemplateRepository(templateRootDirectory);
			CreateGenerator(templateRepository);
			ProjectPassedToGeneratorIsCSharpProject();
			SelectAspxTemplate();
			
			GenerateFile();
			
			string inputFileName = fakeHost.InputFilePassedToProcessTemplate;
			string expectedInputFileName = 
				@"d:\SharpDev\AddIns\AspNet.Mvc\ItemTemplates\CSharp\CodeTemplates\AddView\AspxCSharp\Empty.tt";
			
			Assert.AreEqual(expectedInputFileName, inputFileName);
		}
		
		[Test]
		public void GenerateFile_CSharpEmptyViewTemplate_MvcTextTemplateHostViewNameIsSet()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			
			string viewFolder = @"d:\projects\MyProject\Views\Home";
			string viewName = "About";
			GenerateFile(viewFolder, viewName);
		
			Assert.AreEqual("About", fakeHost.ViewName);
		}
		
		[Test]
		public void GenerateFile_CSharpEmptyRazorViewTemplate_TemplateFileUsedIsIsCSharpRazorEmptyViewTemplate()
		{
			string templateRootDirectory = @"d:\SharpDev\AddIns\AspNet.Mvc";
			CreateTemplateRepository(templateRootDirectory);
			CreateGenerator(templateRepository);
			ProjectPassedToGeneratorIsCSharpProject();
			SelectRazorTemplate();
			
			GenerateFile();
			
			string inputFileName = fakeHost.InputFilePassedToProcessTemplate;
			string expectedInputFileName = 
				@"d:\SharpDev\AddIns\AspNet.Mvc\ItemTemplates\CSharp\CodeTemplates\AddView\CSHTML\Empty.tt";
			
			Assert.AreEqual(expectedInputFileName, inputFileName);
		}
	}
}
