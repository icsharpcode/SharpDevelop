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
	public class MvcControllerFileGeneratorTests
	{
		MvcControllerFileGenerator generator;
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
			generator = new MvcControllerFileGenerator(fakeHostFactory, templateRepository);
			projectUsedByGenerator = TestableProject.CreateProject();
			generator.Project = projectUsedByGenerator;
			ProjectPassedToGeneratorIsCSharpProject();
		}
		
		void ProjectPassedToGeneratorIsCSharpProject()
		{
			projectUsedByGenerator.SetLanguage("C#");
			generator.Language = MvcTextTemplateLanguage.CSharp;			
		}
		
		void GenerateFile()
		{
			GenerateFile(@"d:\projects\myproject\Controllers", "Home");
		}
		
		void GenerateFile(string folder, string name)
		{
			var fileName = new MvcControllerFileName();
			fileName.Folder = folder;
			fileName.ControllerName = name;
			GenerateFile(fileName);
		}
		
		void GenerateFile(MvcControllerFileName fileName)
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
		public void GenerateFile_CSharpEmptyControllerTemplate_OutputFileGeneratedUsingFileNamePassedToGenerator()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			
			string controllerFolder = @"d:\projects\MyProject\Controllers";
			string controllerName = "HomeController";
			GenerateFile(controllerFolder, controllerName);
			
			string outputFileGenerated = fakeHost.OutputFilePassedToProcessTemplate;
			string expectedOutputFileGenerated = 
				@"d:\projects\MyProject\Controllers\HomeController.cs";
			
			Assert.AreEqual(expectedOutputFileGenerated, outputFileGenerated);
		}
		
		[Test]
		public void GenerateFile_CSharpControllerTemplate_TemplateFileUsedIsIsCSharpEmptyControllerTemplate()
		{
			string templateRootDirectory = @"d:\SharpDev\AddIns\AspNet.Mvc";
			CreateTemplateRepository(templateRootDirectory);
			CreateGenerator(templateRepository);
			ProjectPassedToGeneratorIsCSharpProject();
			
			GenerateFile();
			
			string inputFileName = fakeHost.InputFilePassedToProcessTemplate;
			string expectedInputFileName = 
				@"d:\SharpDev\AddIns\AspNet.Mvc\ItemTemplates\CSharp\CodeTemplates\AddController\Controller.tt";
			
			Assert.AreEqual(expectedInputFileName, inputFileName);
		}
		
		[Test]
		public void GenerateFile_CSharpControllerTemplate_MvcTextTemplateHostControllerNameIsSet()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			
			string controllerFolder = @"d:\projects\MyProject\Controllers";
			string controllerName = "AboutController";
			GenerateFile(controllerFolder, controllerName);
		
			Assert.AreEqual("AboutController", fakeHost.ControllerName);
		}
		
		[Test]
		public void GenerateFile_CSharpControllerTemplate_MvcTextTemplateHostNamespaceIsTakenFromProject()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			projectUsedByGenerator.RootNamespace = "MyProjectNamespace";
			
			string controllerFolder = @"d:\projects\MyProject\Controllers";
			string controllerName = "AboutController";
			GenerateFile(controllerFolder, controllerName);
			
			string expectedNamespace = "MyProjectNamespace.Controllers";
			
			Assert.AreEqual(expectedNamespace, fakeHost.Namespace);
		}
	}
}
