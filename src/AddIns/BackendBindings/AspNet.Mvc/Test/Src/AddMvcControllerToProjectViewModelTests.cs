// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
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
		
		void CreateViewModel()
		{
			string path = @"d:\projects\MyAspMvcProject\Controllers";
			CreateViewModelWithControllerFolderPath(path);
		}
		
		void CreateViewModelWithControllerFolderPath(string path)
		{
			fakeSelectedMvcControllerFolder = new FakeSelectedMvcFolder();
			fakeSelectedMvcControllerFolder.TemplateLanguage = MvcTextTemplateLanguage.CSharp;
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
		
		[Test]
		public void AddMvcControllerCommand_ExecutedWhenControllerNameSpecified_MvcControllerIsGenerated()
		{
			CreateViewModel();
			viewModel.ControllerName = "MyControllerPage";
			viewModel.AddMvcControllerCommand.Execute(null);
			
			bool generated = fakeControllerGenerator.IsGenerateFileCalled;
			
			Assert.IsTrue(generated);
		}
		
		[Test]
		public void AddMvcController_ControllerNameAndControllerFolderSpecified_ControllerFullPathUsedToGenerateFile()
		{
			CreateViewModelWithControllerFolderPath(@"d:\projects\MyProject\Controllers");
			fakeSelectedMvcControllerFolder.TemplateLanguage = MvcTextTemplateLanguage.CSharp;
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
			CreateViewModel();
			fakeSelectedMvcControllerFolder.TemplateLanguage = MvcTextTemplateLanguage.VisualBasic;
			viewModel.AddMvcController();
			
			MvcTextTemplateLanguage templateLanguage = fakeControllerGenerator.Language;
			
			Assert.AreEqual(MvcTextTemplateLanguage.VisualBasic, templateLanguage);
		}
		
		[Test]
		public void AddMvcController_SelectedControllerFolderIsInCSharpProject_CSharpMvcControllerFileGenerated()
		{
			CreateViewModel();
			fakeSelectedMvcControllerFolder.TemplateLanguage = MvcTextTemplateLanguage.CSharp;
			viewModel.AddMvcController();
			
			MvcTextTemplateLanguage templateLanguage = fakeControllerGenerator.Language;
			
			Assert.AreEqual(MvcTextTemplateLanguage.CSharp, templateLanguage);
		}
		
		[Test]
		public void AddMvcController_SelectedControllerFolderIsInVisualBasicProject_VisualBasicProjectIsPassedToMvcControllerGenerator()
		{
			CreateViewModel();
			fakeSelectedMvcControllerFolder.ProjectLanguage = "VBNet";
			viewModel.AddMvcController();
			
			IProject project = fakeControllerGenerator.Project;
			TestableProject expectedProject = fakeSelectedMvcControllerFolder.FakeProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void AddMvcController_FileIsGenerated_FileIsAddedToProject()
		{
			CreateViewModel();
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
			CreateViewModel();
			viewModel.AddMvcController();
			
			bool closed = viewModel.IsClosed;
			
			Assert.IsTrue(closed);
		}
		
		[Test]
		public void IsClosed_MvcControllerFileIsNotGenerated_ReturnsFalse()
		{
			CreateViewModel();
			bool closed = viewModel.IsClosed;
			
			Assert.IsFalse(closed);
		}
		
		[Test]
		public void AddMvcController_FileIsGenerated_PropertyChangedEventIsFiredForIsClosedProperty()
		{
			CreateViewModel();
			MonitorPropertyChangedEvents();
			viewModel.AddMvcController();
			
			bool fired = propertyChangedEvents.Contains("IsClosed");
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void AddMvcController_FileIsGenerated_FileIsOpened()
		{
			CreateViewModel();
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
			CreateViewModel();
			viewModel.ControllerName = String.Empty;
			
			bool canExecute = viewModel.AddMvcControllerCommand.CanExecute(null);
			
			Assert.IsFalse(canExecute);
		}
		
		[Test]
		public void AddMvcControllerCommand_ControllerNameIsNotEmptyString_CommandIsEnable()
		{
			CreateViewModel();
			viewModel.ControllerName = "MyController";
			
			bool canExecute = viewModel.AddMvcControllerCommand.CanExecute(null);
			
			Assert.IsTrue(canExecute);
		}
	}
}
