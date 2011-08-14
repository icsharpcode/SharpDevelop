// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class AddMvcControllerToProjectControllerModelTests
	{
		AddMvcControllerToProjectViewModel viewModel;
		FakeMvcControllerFileGenerator fakeControllerGenerator;
		FakeSelectedMvcFolder fakeSelectedMvcControllerFolder;
		List<string> propertyChangedEvents;
		FakeMvcFileService fakeFileService;
		
		void CreateViewModelWithCSharpProject()
		{
			string path = @"d:\projects\MyAspMvcProject\Controllers";
			CreateViewModelWithControllerFolderPath(path, MvcTextTemplateLanguage.CSharp, "C#");
		}
		
		void CreateViewModelWithVisualBasicProject()
		{
			string path = @"d:\projects\MyAspMvcProject\Controllers";
			CreateViewModelWithControllerFolderPath(path, MvcTextTemplateLanguage.VisualBasic, "VBNet");			
		}
		
		void CreateViewModelWithControllerFolderPath(
			string path,
			MvcTextTemplateLanguage language,
			string projectLanguage)
		{
			fakeSelectedMvcControllerFolder = new FakeSelectedMvcFolder();
			fakeSelectedMvcControllerFolder.TemplateLanguage = language;
			fakeSelectedMvcControllerFolder.ProjectLanguage = projectLanguage;
			fakeSelectedMvcControllerFolder.Path = path;
			fakeControllerGenerator = new FakeMvcControllerFileGenerator();
			fakeFileService = new FakeMvcFileService();
			viewModel = new AddMvcControllerToProjectViewModel(
				fakeSelectedMvcControllerFolder,
				fakeControllerGenerator,
				fakeFileService);
		}
		
		void MonitorPropertyChangedEvents()
		{
			propertyChangedEvents = new List<string>();
			viewModel.PropertyChanged += (source, e) => propertyChangedEvents.Add(e.PropertyName);
		}
		
		bool ViewModelHasControllerTemplate(string templateName)
		{
			return viewModel.ControllerTemplates.Any(v => v.Name == templateName);
		}
		
		MvcControllerTemplateViewModel GetControllerTemplateFromViewModel(string templateName)
		{
			return viewModel.ControllerTemplates.SingleOrDefault(v => v.Name == templateName);
		}
		
		void SelectEmptyControllerTemplate()
		{
			viewModel.SelectedControllerTemplate = GetControllerTemplateFromViewModel("Empty");
		}
		
		void SelectEmptyReadWriteControllerTemplate()
		{
			viewModel.SelectedControllerTemplate = GetControllerTemplateFromViewModel("EmptyReadWrite");
		}
		
		[Test]
		public void AddMvcControllerCommand_ExecutedWhenControllerNameSpecified_MvcControllerIsGenerated()
 		{
			CreateViewModelWithCSharpProject();
			viewModel.ControllerName = "MyControllerPage";
			viewModel.AddMvcControllerCommand.Execute(null);
			
			bool generated = fakeControllerGenerator.IsGenerateFileCalled;
			
			Assert.IsTrue(generated);
		}
		
		[Test]
		public void AddMvcController_ControllerNameAndControllerFolderSpecified_ControllerFullPathUsedToGenerateFile()
		{
			CreateViewModelWithControllerFolderPath(
				@"d:\projects\MyProject\Controllers",
				MvcTextTemplateLanguage.CSharp, "C#");
			viewModel.ControllerName = "Home";
			viewModel.AddMvcController();
			
			MvcControllerFileName controllerFileName = fakeControllerGenerator.FileNamePassedToGenerateController;
			string fileName = controllerFileName.GetPath();
			string expectedFileName = @"d:\projects\MyProject\Controllers\Home.cs";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void AddMvcController_SelectedControllerFolderIsInVisualBasicProject_VisualBasicMvcControllerFileGenerated()
		{
			CreateViewModelWithVisualBasicProject();
			fakeSelectedMvcControllerFolder.TemplateLanguage = MvcTextTemplateLanguage.VisualBasic;
			viewModel.AddMvcController();
			
			MvcTextTemplateLanguage templateLanguage = fakeControllerGenerator.TemplateLanguage;
			
			Assert.AreEqual(MvcTextTemplateLanguage.VisualBasic, templateLanguage);
		}
		
		[Test]
		public void AddMvcController_SelectedControllerFolderIsInCSharpProject_CSharpMvcControllerFileGenerated()
		{
			CreateViewModelWithCSharpProject();
			viewModel.AddMvcController();
			
			MvcTextTemplateLanguage templateLanguage = fakeControllerGenerator.TemplateLanguage;
			
			Assert.AreEqual(MvcTextTemplateLanguage.CSharp, templateLanguage);
		}
		
		[Test]
		public void AddMvcController_SelectedControllerFolderIsInVisualBasicProject_VisualBasicProjectIsPassedToMvcControllerGenerator()
		{
			CreateViewModelWithCSharpProject();
			fakeSelectedMvcControllerFolder.ProjectLanguage = "VBNet";
			viewModel.AddMvcController();
			
			IProject project = fakeControllerGenerator.Project;
			TestableProject expectedProject = fakeSelectedMvcControllerFolder.FakeProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void AddMvcController_FileIsGenerated_FileIsAddedToProject()
		{
			CreateViewModelWithCSharpProject();
			viewModel.ControllerName = "Home";
			fakeSelectedMvcControllerFolder.ProjectLanguage = "C#";
			fakeSelectedMvcControllerFolder.TemplateLanguage = MvcTextTemplateLanguage.CSharp;
			fakeSelectedMvcControllerFolder.Path = @"d:\projects\MyAspMvcProject\Controllers\Home";
			viewModel.AddMvcController();
			
			string fileAddedToProject = fakeSelectedMvcControllerFolder.FileNamePassedToAddFile;
			string expectedFileAddedToProject = "Home.cs";
			
			Assert.AreEqual(expectedFileAddedToProject, fileAddedToProject);
		}
		
		[Test]
		public void AddMvcController_FileIsGenerated_WindowIsClosed()
		{
			CreateViewModelWithCSharpProject();
			viewModel.AddMvcController();
			
			bool closed = viewModel.IsClosed;
			
			Assert.IsTrue(closed);
		}
		
		[Test]
		public void IsClosed_MvcControllerFileIsNotGenerated_ReturnsFalse()
		{
			CreateViewModelWithCSharpProject();
			bool closed = viewModel.IsClosed;
			
			Assert.IsFalse(closed);
		}
		
		[Test]
		public void AddMvcController_FileIsGenerated_PropertyChangedEventIsFiredForIsClosedProperty()
		{
			CreateViewModelWithCSharpProject();
			MonitorPropertyChangedEvents();
			viewModel.AddMvcController();
			
			bool fired = propertyChangedEvents.Contains("IsClosed");
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void AddMvcController_FileIsGenerated_FileIsOpened()
		{
			CreateViewModelWithCSharpProject();
			viewModel.ControllerName = "HomeController";
			fakeSelectedMvcControllerFolder.ProjectLanguage = "C#";
			fakeSelectedMvcControllerFolder.TemplateLanguage = MvcTextTemplateLanguage.CSharp;
			fakeSelectedMvcControllerFolder.Path = @"d:\projects\MyAspMvcProject\Controllers";
			viewModel.AddMvcController();
			
			string fileNameOpened = fakeFileService.FileNamePassedToOpenFile;
			string expectedFileNameOpened = @"d:\projects\MyAspMvcProject\Controllers\HomeController.cs";
			
			Assert.AreEqual(expectedFileNameOpened, fileNameOpened);
		}
		
		[Test]
		public void AddMvcControllerCommand_ControllerNameIsEmptyString_CommandIsDisabled()
		{
			CreateViewModelWithCSharpProject();
			viewModel.ControllerName = String.Empty;
			
			bool canExecute = viewModel.AddMvcControllerCommand.CanExecute(null);
			
			Assert.IsFalse(canExecute);
		}
		
		[Test]
		public void AddMvcControllerCommand_ControllerNameIsNotEmptyString_CommandIsEnable()
		{
			CreateViewModelWithCSharpProject();
			viewModel.ControllerName = "MyController";
			
			bool canExecute = viewModel.AddMvcControllerCommand.CanExecute(null);
			
			Assert.IsTrue(canExecute);
		}
		
		[Test]
		public void AddMvcController_VisualBasicProject_VisualBasicFileIsGenerated()
		{
			CreateViewModelWithVisualBasicProject();
			viewModel.ControllerName = "HomeController";
			fakeSelectedMvcControllerFolder.Path = @"d:\projects\MyAspMvcProject\Controllers";
			viewModel.AddMvcController();
			
			string fileNameOpened = fakeFileService.FileNamePassedToOpenFile;
			string expectedFileNameOpened = @"d:\projects\MyAspMvcProject\Controllers\HomeController.vb";
			
			Assert.AreEqual(expectedFileNameOpened, fileNameOpened);
		}
		
		[Test]
		public void ControllerTemplates_DefaultTemplates_HasEmptyControllerTemplate()
		{
			CreateViewModelWithCSharpProject();
			
			bool contains = ViewModelHasControllerTemplate("Empty");
			
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void ControllerTemplates_DefaultTemplates_HasEmptyReadWriteControllerTemplate()
		{
			CreateViewModelWithCSharpProject();
			
			bool contains = ViewModelHasControllerTemplate("EmptyReadWrite");
			
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void SelectedControllerTemplate_DefaultTemplates_EmptyControllerTemplateIsSelectedByDefault()
		{
			CreateViewModelWithCSharpProject();
			
			MvcControllerTemplateViewModel controllerTemplate = viewModel.SelectedControllerTemplate;
			
			Assert.AreEqual("Empty", controllerTemplate.Name);
		}
		
		[Test]
		public void AddMvcController_EmptyReadWriteTemplateSelected_AddActionMethodsSetToTrueOnFileGenerator()
		{
			CreateViewModelWithCSharpProject();
			SelectEmptyReadWriteControllerTemplate();
			fakeControllerGenerator.AddActionMethods = false;
			viewModel.AddMvcController();
			
			Assert.IsTrue(fakeControllerGenerator.AddActionMethods);
		}
		
		[Test]
		public void AddMvcController_EmptyTemplateSelected_AddActionMethodsSetToFalseOnFileGenerator()
		{
			CreateViewModelWithCSharpProject();
			SelectEmptyControllerTemplate();
			fakeControllerGenerator.AddActionMethods = true;
			viewModel.AddMvcController();
			
			Assert.IsFalse(fakeControllerGenerator.AddActionMethods);
		}
	}
}
