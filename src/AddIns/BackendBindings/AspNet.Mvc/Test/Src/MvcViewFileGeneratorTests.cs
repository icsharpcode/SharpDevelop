// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using Rhino.Mocks;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcViewFileGeneratorTests
	{
		MvcViewFileGenerator generator;
		FakeMvcProject projectUsedByGenerator;
		MvcTextTemplateRepository templateRepository;
		FakeMvcTextTemplateHostFactory fakeHostFactory;
		FakeMvcTextTemplateHost fakeHost;
		FakeMvcTextTemplateAppDomainFactory fakeAppDomainFactory;
		IMvcFileGenerationErrorReporter fakeErrorReporter;
		
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
			fakeAppDomainFactory = new FakeMvcTextTemplateAppDomainFactory();
			fakeErrorReporter = MockRepository.GenerateStub<IMvcFileGenerationErrorReporter>();
			generator = new MvcViewFileGenerator(fakeHostFactory, templateRepository, fakeAppDomainFactory, fakeErrorReporter);
			projectUsedByGenerator = new FakeMvcProject();
			generator.Project = projectUsedByGenerator;
			ProjectPassedToGeneratorIsCSharpProject();
		}
		
		void ProjectPassedToGeneratorIsCSharpProject()
		{
			projectUsedByGenerator.SetCSharpAsTemplateLanguage();
			generator.TemplateLanguage = MvcTextTemplateLanguage.CSharp;			
		}
		
		void GenerateFile()
		{
			GenerateFile(@"d:\projects\myproject\Views\Home", "Index");
		}
		
		void GenerateFile(string folder, string name)
		{
			MvcViewFileName fileName = CreateMvcViewFileName(folder, name);
			GenerateFile(fileName);
		}
		
		MvcViewFileName CreateMvcViewFileName(string folder, string name)
		{
			var fileName = new MvcViewFileName();
			fileName.Folder = folder;
			fileName.ViewName = name;
			return fileName;			
		}
		
		void GenerateFile(MvcViewFileName fileName)
		{
			generator.GenerateFile(fileName);
		}
		
		MvcViewTextTemplate CreateViewTemplate(string fileName)
		{
			return new MvcViewTextTemplate() {
				FileName = fileName
			};
		}
		
		CompilerError AddCompilerErrorToTemplateHost()
		{
			return fakeHost.AddCompilerError();
		}
		
		[Test]
		public void GenerateFile_CSharpEmptyViewTemplate_MvcTextTemplateHostIsCreated()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			GenerateFile();
			IMvcProject project = fakeHostFactory.ProjectPassedToCreateMvcTextTemplateHost;
			
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
			
			string viewFolder = @"d:\projects\MyProject\Views\Home";
			string viewName = "Index";
			GenerateFile(viewFolder, viewName);
			
			string outputFileGenerated = fakeHost.OutputFilePassedToProcessTemplate;
			string expectedOutputFileGenerated = 
				@"d:\projects\MyProject\Views\Home\Index.aspx";
			
			Assert.AreEqual(expectedOutputFileGenerated, outputFileGenerated);
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
		public void GenerateFile_IsPartialViewIsTrue_MvcTextTemplateHostIsPartialViewIsSetToTrue()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			string viewFolder = @"d:\projects\MyProject\Views\Home";
			string viewName = "Index";
			MvcViewFileName fileName = CreateMvcViewFileName(viewFolder, viewName);
			fileName.IsPartialView = true;
			GenerateFile(fileName);
			
			Assert.IsTrue(fakeHost.IsPartialView);
		}
		
		[Test]
		public void GenerateFile_IsPartialViewIsFalse_MvcTextTemplateHostIsPartialViewIsSetToFalse()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			string viewFolder = @"d:\projects\MyProject\Views\Home";
			string viewName = "Index";
			MvcViewFileName fileName = CreateMvcViewFileName(viewFolder, viewName);
			fileName.IsPartialView = false;
			GenerateFile(fileName);
		
			Assert.IsFalse(fakeHost.IsPartialView);
		}
		
		[Test]
		public void GenerateFile_CSharpEmptyAspxViewTemplateAndIsPartialViewIsSet_AspxUserControlFileGenerated()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			
			string viewFolder = @"d:\projects\MyProject\Views\Home";
			string viewName = "Index";
			MvcViewFileName fileName = CreateMvcViewFileName(viewFolder, viewName);
			fileName.IsPartialView = true;
			GenerateFile(fileName);
			
			string outputFileGenerated = fakeHost.OutputFilePassedToProcessTemplate;
			string expectedOutputFileGenerated = 
				@"d:\projects\MyProject\Views\Home\Index.ascx";
			
			Assert.AreEqual(expectedOutputFileGenerated, outputFileGenerated);
		}
		
		[Test]
		public void GenerateFile_ModelClassNameIsSet_MvcTextTemplateHostHasViewDataTypeNameIsSetToModelClassName()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			generator.ModelClassName = "MyNamespace.MyModel";
			GenerateFile();
			
			string viewDataTypeName = fakeHost.ViewDataTypeName;
			
			Assert.AreEqual("MyNamespace.MyModel", viewDataTypeName);
		}
		
		[Test]
		public void ModelClassName_DefaultValue_ReturnsEmptyString()
		{
			CreateGenerator();
			string modelClassName = generator.ModelClassName;
			
			Assert.AreEqual(String.Empty, modelClassName);
		}
		
		[Test]
		public void GenerateFile_IsContentPageIsTrue_IsContentPageIsSetOnMvcTextTemplateHost()
		{
			CreateGenerator();
			generator.IsContentPage = true;
			GenerateFile();
			
			bool contentPage = fakeHost.IsContentPage;
			
			Assert.IsTrue(contentPage);
		}
		
		[Test]
		public void MasterPageFile_DefaultValue_ReturnsEmptyString()
		{
			CreateGenerator();
			string masterPage = generator.MasterPageFile;
			
			Assert.AreEqual(String.Empty, masterPage);
		}
		
		[Test]
		public void GenerateFile_MasterPageFileIsSet_MasterPageFileIsSetOnMvcTextTemplateHost()
		{
			CreateGenerator();
			generator.IsContentPage = true;
			generator.MasterPageFile = "~/Views/Shared/Site.Master";
			GenerateFile();
			
			string masterPage = fakeHost.MasterPageFile;
			
			Assert.AreEqual("~/Views/Shared/Site.Master", masterPage);
		}
		
		[Test]
		public void PrimaryContentPlaceHolderId_DefaultValue_ReturnsEmptyString()
		{
			CreateGenerator();
			string id = generator.PrimaryContentPlaceHolderId;
			
			Assert.AreEqual(String.Empty, id);
		}
		
		[Test]
		public void GenerateFile_PrimaryContentPlaceHolderIdIsSet_PrimaryContentPlaceHolderIdIsSetOnMvcTextTemplateHost()
		{
			CreateGenerator();
			generator.IsContentPage = true;
			generator.PrimaryContentPlaceHolderId = "MainContent";
			GenerateFile();
			
			string id = fakeHost.PrimaryContentPlaceHolderID;
			
			Assert.AreEqual("MainContent", id);
		}
		
		[Test]
		public void GenerateFile_MvcViewTextTemplateSet_MvcViewTextTemplateUsedWhenGeneratingFile()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			string expectedFileName = @"d:\templates\controller.tt";
			MvcViewTextTemplate template = CreateViewTemplate(expectedFileName);
			generator.Template = template;
			GenerateFile();
			
			string fileName = fakeHost.InputFilePassedToProcessTemplate;
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GenerateFile_CSharpEmptyViewTemplate_AppDomainIsDisposed()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			GenerateFile();
			bool disposed = fakeAppDomainFactory.FakeAppDomain.IsDisposed;
			
			Assert.IsTrue(disposed);
		}
		
		[Test]
		public void GenerateFile_CSharpEmptyViewTemplate_MvcTextTemplateHostIsCreatedWithAppDomain()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			GenerateFile();
			
			IMvcTextTemplateHostAppDomain appDomain = fakeHostFactory.AppDomainPassedToCreateMvcTextTemplateHost;
			FakeMvcTextTemplateAppDomain expectedAppDomain = fakeAppDomainFactory.FakeAppDomain;
			
			Assert.AreEqual(expectedAppDomain, appDomain);
		}
		
		[Test]
		public void GenerateFile_ModelClassAssemblyLocationIsSet_MvcTextTemplateHostHasViewDataTypeAssemblyLocationSetToModelClassAssemblyLocation()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			generator.ModelClassAssemblyLocation = @"d:\projects\MyProject\bin\debug\MyProject.dll";
			GenerateFile();
			
			string assemblyLocation = fakeHost.ViewDataTypeAssemblyLocation;
			
			Assert.AreEqual(@"d:\projects\MyProject\bin\debug\MyProject.dll", assemblyLocation);
		}
		
		[Test]
		public void ModelClassAssemblyLocation_DefaultValue_ReturnsEmptyString()
		{
			CreateGenerator();
			string assemblyLocation = generator.ModelClassAssemblyLocation;
			
			Assert.AreEqual(String.Empty, assemblyLocation);
		}
		
		[Test]
		public void GenerateFile_TemplateProcessedWithCompilerError_ErrorsSavedByGenerator()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			CompilerError error = AddCompilerErrorToTemplateHost();
			
			GenerateFile();
			
			Assert.AreEqual(1, generator.Errors.Count);
			Assert.AreEqual(error, generator.Errors[0]);
		}
		
		[Test]
		public void HasErrors_NoErrors_ReturnsFalse()
		{
			CreateGenerator();
			GenerateFile();
			
			bool result = generator.HasErrors;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void HasErrors_OneError_ReturnsTrue()
		{
			CreateGenerator();
			AddCompilerErrorToTemplateHost();
			GenerateFile();
			
			bool result = generator.HasErrors;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void GenerateFile_TemplateProcessedWithCompilerError_ErrorsReported()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			CompilerError error = AddCompilerErrorToTemplateHost();
			
			GenerateFile();
			
			fakeErrorReporter.AssertWasCalled(reporter => reporter.ShowErrors(generator.Errors));
		}
		
		[Test]
		public void GenerateFile_TemplateProcessedNoErrors_NoErrorsReported()
		{
			CreateGenerator();
			ProjectPassedToGeneratorIsCSharpProject();
			
			GenerateFile();
			
			fakeErrorReporter.AssertWasNotCalled(reporter => reporter.ShowErrors(Arg<CompilerErrorCollection>.Is.Anything));
		}
	}
}
