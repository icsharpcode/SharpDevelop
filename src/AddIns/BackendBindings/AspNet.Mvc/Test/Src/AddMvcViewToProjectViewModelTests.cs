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
		
		bool ViewModelHasViewEngine(string viewTypeName)
		{
			return viewModel.ViewEngines.Any(v => v.Name == viewTypeName);
		}
		
		MvcViewEngineViewModel GetViewEngineFromViewModel(string viewTypeName)
		{
			return viewModel.ViewEngines.SingleOrDefault(v => v.Name == viewTypeName);
		}
		
		void CSharpProjectSelected()
		{
			fakeSelectedMvcViewFolder.TemplateLanguage = MvcTextTemplateLanguage.CSharp;
		}
		
		void VisualBasicProjectSelected()
		{
			fakeSelectedMvcViewFolder.TemplateLanguage = MvcTextTemplateLanguage.VisualBasic;
		}
		
		void SelectAspxViewEngine()
		{
			viewModel.SelectedViewEngine = GetViewEngineFromViewModel("ASPX");
		}
		
		void SelectRazorViewEngine()
		{
			viewModel.SelectedViewEngine = GetViewEngineFromViewModel("Razor");
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
			
			MvcTextTemplateLanguage templateLanguage = fakeViewGenerator.TemplateLanguage;
			
			Assert.AreEqual(MvcTextTemplateLanguage.VisualBasic, templateLanguage);
		}
		
		[Test]
		public void AddMvcView_SelectedViewFolderIsInCSharpProject_CSharpMvcViewFileGenerated()
		{
			CreateViewModel();
			fakeSelectedMvcViewFolder.TemplateLanguage = MvcTextTemplateLanguage.CSharp;
			viewModel.AddMvcView();
			
			MvcTextTemplateLanguage templateLanguage = fakeViewGenerator.TemplateLanguage;
			
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
		
		[Test]
		public void ViewEngines_RazorAndAspxViewEngines_ContainsRazorViewEngine()
		{
			CreateViewModel();
			
			bool contains = ViewModelHasViewEngine("Razor");
			
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void ViewEngines_RazorAndAspxViewEngines_RazorViewEngineViewModelHasRazorTemplateType()
		{
			CreateViewModel();
			
			MvcViewEngineViewModel viewEngine = GetViewEngineFromViewModel("Razor");
			
			Assert.AreEqual(MvcTextTemplateType.Razor, viewEngine.TemplateType);
		}
		
		[Test]
		public void ViewEngines_RazorAndAspxViewTypes_ContainsAspxViewType()
		{
			CreateViewModel();
			
			bool contains = ViewModelHasViewEngine("ASPX");
			
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void ViewEngines_RazorAndAspxViewEngines_AspxViewEngineViewModelHasAspxTemplateType()
		{
			CreateViewModel();
			
			MvcViewEngineViewModel viewEngine = GetViewEngineFromViewModel("ASPX");
			
			Assert.AreEqual(MvcTextTemplateType.Aspx, viewEngine.TemplateType);
		}
		
		[Test]
		public void ViewEngines_RazorAndAspxViewEngines_FirstViewEngineInListIsAspxViewEngine()
		{
			CreateViewModel();
			
			MvcViewEngineViewModel viewEngine = viewModel.ViewEngines.First();
			
			Assert.AreEqual(MvcTextTemplateType.Aspx, viewEngine.TemplateType);
		}
		
		[Test]
		public void SelectedViewEngine_RazorAndAspxViewEngines_ReturnsAspxByDefault()
		{
			CreateViewModel();
			
			MvcViewEngineViewModel viewEngine = viewModel.SelectedViewEngine;
			
			Assert.AreEqual(MvcTextTemplateType.Aspx, viewEngine.TemplateType);			
		}
		
		[Test]
		public void AddMvcView_CSharpProjectAndRazorViewEngineIsSelected_CSharpRazorViewFileNamePassedToGenerator()
		{
			CreateViewModelWithViewFolderPath(@"d:\projects\MyProject\Views\Home");
			viewModel.ViewName = "Index";
			CSharpProjectSelected();
			SelectRazorViewEngine();
			viewModel.AddMvcView();
			
			MvcViewFileName viewFileName = fakeViewGenerator.FileNamePassedToGenerateFile;
			string fileName = viewFileName.GetPath();
			string expectedFileName = @"d:\projects\MyProject\Views\Home\Index.cshtml";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void AddMvcView_VisualBasicProjectAndRazorViewEngineIsSelected_VisualBasicRazorViewFileNamePassedToGenerator()
		{
			CreateViewModelWithViewFolderPath(@"d:\projects\MyProject\Views\Home");
			viewModel.ViewName = "Index";
			VisualBasicProjectSelected();
			SelectRazorViewEngine();
			viewModel.AddMvcView();
			
			MvcViewFileName viewFileName = fakeViewGenerator.FileNamePassedToGenerateFile;
			string fileName = viewFileName.GetPath();
			string expectedFileName = @"d:\projects\MyProject\Views\Home\Index.vbhtml";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void AddMvcView_RazorViewEngineIsSelected_RazorViewFileGenerated()
		{
			CreateViewModel();
			fakeViewGenerator.TemplateType = MvcTextTemplateType.Aspx;
			CSharpProjectSelected();
			viewModel.ViewName = "Index";
			SelectRazorViewEngine();
			viewModel.AddMvcView();
			
			MvcTextTemplateType templateType = fakeViewGenerator.TemplateType;
			
			Assert.AreEqual(MvcTextTemplateType.Razor, templateType);
		}
		
		[Test]
		public void AddMvcView_AspxViewEngineIsSelected_AspxViewFileGenerated()
		{
			CreateViewModel();
			fakeViewGenerator.TemplateType = MvcTextTemplateType.Razor;
			CSharpProjectSelected();
			viewModel.ViewName = "Index";
			SelectAspxViewEngine();
			viewModel.AddMvcView();
			
			MvcTextTemplateType templateType = fakeViewGenerator.TemplateType;
			
			Assert.AreEqual(MvcTextTemplateType.Aspx, templateType);
		}
		
		[Test]
		public void AddMvcView_CreateAsPartialViewIsSelected_ViewFileIsPartialView()
		{
			CreateViewModel();
			CSharpProjectSelected();
			viewModel.IsPartialView = true;
			viewModel.AddMvcView();
			
			bool partialView = fakeViewGenerator.FileNamePassedToGenerateFile.IsPartialView;
			
			Assert.IsTrue(partialView);
		}
		
		[Test]
		public void AddMvcView_CreateAsPartialViewIsNotSelected_ViewFileIsNotPartialView()
		{
			CreateViewModel();
			CSharpProjectSelected();
			viewModel.IsPartialView = false;
			viewModel.AddMvcView();
			
			bool partialView = fakeViewGenerator.FileNamePassedToGenerateFile.IsPartialView;
			
			Assert.IsFalse(partialView);
		}
	}
}
