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
			generator.Language = MvcTextTemplateLanguage.CSharp;			
		}
		
		void GenerateView()
		{
			GenerateView(@"d:\projects\myproject\Views\Home", "Index");
		}
		
		void GenerateView(string folder, string name)
		{
			var fileName = new MvcViewFileName();
			fileName.ViewFolder = folder;
			fileName.ViewName = name;
			GenerateView(fileName);
		}
		
		void GenerateView(MvcViewFileName fileName)
		{
			generator.GenerateView(fileName);
		}
		
		[Test]
		public void GenerateView_CSharpEmptyViewTemplate_MvcTextTemplateHostIsCreated()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			GenerateView();
			IProject project = fakeHostFactory.ProjectPassedToCreateMvcTextTemplateHost;
			
			Assert.AreEqual(projectUsedByGenerator, project);
		}
		
		[Test]
		public void GenerateView_CSharpEmptyViewTemplate_MvcTextTemplateHostIsDisposed()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			GenerateView();
			bool disposed = fakeHost.IsDisposed;
			
			Assert.IsTrue(disposed);
		}
		
		[Test]
		public void GenerateView_CSharpEmptyViewTemplate_OutputFileGeneratedUsingFileNamePassedToGenerator()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			
			string viewFolder = @"d:\projects\MyProject\Views\Home";
			string viewName = "Index";
			GenerateView(viewFolder, viewName);
			
			string outputFileGenerated = fakeHost.OutputFilePassedToProcessTemplate;
			string expectedOutputFileGenerated = 
				@"d:\projects\MyProject\Views\Home\Index.aspx";
			
			Assert.AreEqual(expectedOutputFileGenerated, outputFileGenerated);
		}
		
		[Test]
		public void GenerateView_CSharpEmptyViewTemplate_TemplateFileUsedIsIsCSharpEmptyViewTemplate()
		{
			string templateRootDirectory = @"d:\SharpDev\AddIns\AspNet.Mvc";
			CreateTemplateRepository(templateRootDirectory);
			CreateGenerator(templateRepository);
			ProjectPassedToGeneratorIsCSharpProject();
			
			GenerateView();
			
			string inputFileName = fakeHost.InputFilePassedToProcessTemplate;
			string expectedInputFileName = 
				@"d:\SharpDev\AddIns\AspNet.Mvc\ItemTemplates\CSharp\CodeTemplates\AddView\AspxCSharp\Empty.tt";
			
			Assert.AreEqual(expectedInputFileName, inputFileName);
		}
		
		[Test]
		public void GenerateView_CSharpEmptyViewTemplate_MvcTextTemplateHostViewNameIsSet()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			
			string viewFolder = @"d:\projects\MyProject\Views\Home";
			string viewName = "About";
			GenerateView(viewFolder, viewName);
		
			Assert.AreEqual("About", fakeHost.ViewName);
		}
	}
}
