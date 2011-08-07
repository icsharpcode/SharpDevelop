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
	public class AddMvcViewToProjectViewModelTests
	{
		AddMvcViewToProjectViewModel viewModel;
		FakeMvcViewFileGenerator fakeViewGenerator;
		FakeSelectedMvcFolder fakeSelectedMvcViewFolder;
		List<string> propertyChangedEvents;
		FakeMvcFileService fakeFileService;
		
		void CreateViewModel()
		{
			string path = @"d:\projects\MyAspMvcProject\Views\Home";
			CreateViewModelWithViewFolderPath(path);
		}
		
		void CreateViewModelWithViewFolderPath(string path)
		{
			fakeSelectedMvcViewFolder = new FakeSelectedMvcFolder();
			fakeSelectedMvcViewFolder.Path = path;
			fakeSelectedMvcViewFolder.ProjectLanguage = "C#";
			fakeSelectedMvcViewFolder.TemplateLanguage = MvcTextTemplateLanguage.CSharp;
			fakeViewGenerator = new FakeMvcViewFileGenerator();
			fakeFileService = new FakeMvcFileService();
			viewModel = new AddMvcViewToProjectViewModel(
				fakeSelectedMvcViewFolder,
				fakeViewGenerator,
				fakeFileService);
		}
		
		void MonitorPropertyChangedEvents()
		{
			propertyChangedEvents = new List<string>();
			viewModel.PropertyChanged += (source, e) => propertyChangedEvents.Add(e.PropertyName);
		}
		
		[Test]
		public void AddMvcViewCommand_ExecutedWhenViewNameSpecified_MvcViewIsGenerated()
		{
			CreateViewModel();
			viewModel.ViewName = "MyViewPage";
			viewModel.AddMvcViewCommand.Execute(null);
			
			bool generated = fakeViewGenerator.IsGenerateFileCalled;
			
			Assert.IsTrue(generated);
		}
		
		[Test]
		public void AddMvcView_ViewNameAndViewFolderSpecified_ViewFullPathUsedToGenerateFile()
		{
			CreateViewModelWithViewFolderPath(@"d:\projects\MyProject\Views\Home");
			viewModel.ViewName = "Index";
			viewModel.AddMvcView();
			
			MvcViewFileName viewFileName = fakeViewGenerator.FileNamePassedToGenerateFile;
			string fileName = viewFileName.GetPath();
			string expectedFileName = @"d:\projects\MyProject\Views\Home\Index.aspx";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void AddMvcView_SelectedViewFolderIsInVisualBasicProject_VisualBasicMvcViewFileGenerated()
		{
			CreateViewModel();
			fakeSelectedMvcViewFolder.TemplateLanguage = MvcTextTemplateLanguage.VisualBasic;
			viewModel.AddMvcView();
			
			MvcTextTemplateLanguage templateLanguage = fakeViewGenerator.Language;
			
			Assert.AreEqual(MvcTextTemplateLanguage.VisualBasic, templateLanguage);
		}
		
		[Test]
		public void AddMvcView_SelectedViewFolderIsInCSharpProject_CSharpMvcViewFileGenerated()
		{
			CreateViewModel();
			fakeSelectedMvcViewFolder.TemplateLanguage = MvcTextTemplateLanguage.CSharp;
			viewModel.AddMvcView();
			
			MvcTextTemplateLanguage templateLanguage = fakeViewGenerator.Language;
			
			Assert.AreEqual(MvcTextTemplateLanguage.CSharp, templateLanguage);
		}
		
		[Test]
		public void AddMvcView_SelectedViewFolderIsInVisualBasicProject_VisualBasicProjectIsPassedToMvcViewGenerator()
		{
			CreateViewModel();
			fakeSelectedMvcViewFolder.ProjectLanguage = "VBNet";
			viewModel.AddMvcView();
			
			IProject project = fakeViewGenerator.Project;
			TestableProject expectedProject = fakeSelectedMvcViewFolder.FakeProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void AddMvcView_FileIsGenerated_FileIsAddedToProject()
		{
			CreateViewModel();
			viewModel.ViewName = "Index";
			fakeSelectedMvcViewFolder.ProjectLanguage = "C#";
			fakeSelectedMvcViewFolder.Path = @"d:\projects\MyAspMvcProject\Views\Home";
			viewModel.AddMvcView();
			
			string fileAddedToProject = fakeSelectedMvcViewFolder.FileNamePassedToAddFile;
			string expectedFileAddedToProject = "Index.aspx";
			
			Assert.AreEqual(expectedFileAddedToProject, fileAddedToProject);
		}
		
		[Test]
		public void AddMvcView_FileIsGenerated_WindowIsClosed()
		{
			CreateViewModel();
			viewModel.AddMvcView();
			
			bool closed = viewModel.IsClosed;
			
			Assert.IsTrue(closed);
		}
		
		[Test]
		public void IsClosed_MvcViewFileIsNotGenerated_ReturnsFalse()
		{
			CreateViewModel();
			bool closed = viewModel.IsClosed;
			
			Assert.IsFalse(closed);
		}
		
		[Test]
		public void AddMvcView_FileIsGenerated_PropertyChangedEventIsFiredForIsClosedProperty()
		{
			CreateViewModel();
			MonitorPropertyChangedEvents();
			viewModel.AddMvcView();
			
			bool fired = propertyChangedEvents.Contains("IsClosed");
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void AddMvcView_FileIsGenerated_FileIsOpened()
		{
			CreateViewModel();
			viewModel.ViewName = "Index";
			fakeSelectedMvcViewFolder.ProjectLanguage = "C#";
			fakeSelectedMvcViewFolder.Path = @"d:\projects\MyAspMvcProject\Views\Home";
			viewModel.AddMvcView();
			
			string fileNameOpened = fakeFileService.FileNamePassedToOpenFile;
			string expectedFileNameOpened = @"d:\projects\MyAspMvcProject\Views\Home\Index.aspx";
			
			Assert.AreEqual(expectedFileNameOpened, fileNameOpened);
		}
		
		[Test]
		public void AddMvcViewCommand_ViewNameIsEmptyString_CommandIsDisabled()
		{
			CreateViewModel();
			viewModel.ViewName = String.Empty;
			
			bool canExecute = viewModel.AddMvcViewCommand.CanExecute(null);
			
			Assert.IsFalse(canExecute);
		}
		
		[Test]
		public void AddMvcViewCommand_ViewNameIsNotEmptyString_CommandIsEnable()
		{
			CreateViewModel();
			viewModel.ViewName = "MyView";
			
			bool canExecute = viewModel.AddMvcViewCommand.CanExecute(null);
			
			Assert.IsTrue(canExecute);
		}
	}
}
