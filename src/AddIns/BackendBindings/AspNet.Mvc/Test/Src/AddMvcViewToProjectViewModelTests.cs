// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		FakeMvcProject fakeProject;
		
		void CreateViewModel()
		{
			string path = @"d:\projects\MyAspMvcProject\Views\Home";
			CreateViewModelWithViewFolderPath(path);
		}
		
		void CreateViewModelWithViewFolderPath(string path)
		{
			fakeSelectedMvcViewFolder = new FakeSelectedMvcFolder();
			fakeSelectedMvcViewFolder.Path = path;
			fakeSelectedMvcViewFolder.SetCSharpAsTemplateLanguage();
			fakeProject = fakeSelectedMvcViewFolder.FakeMvcProject;
			fakeViewGenerator = new FakeMvcViewFileGenerator();
			viewModel = new AddMvcViewToProjectViewModel(
				fakeSelectedMvcViewFolder,
				fakeViewGenerator);
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
		
		void AddModelClassToProject(string ns, string name)
		{
			fakeProject.AddModelClassToProject(ns, name);
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
			fakeSelectedMvcViewFolder.SetVisualBasicAsTemplateLanguage();
			viewModel.AddMvcView();
			
			IMvcProject project = fakeViewGenerator.Project;
			FakeMvcProject expectedProject = fakeSelectedMvcViewFolder.FakeMvcProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void AddMvcView_FileIsGenerated_FileIsAddedToProject()
		{
			CreateViewModel();
			viewModel.ViewName = "Index";
			fakeSelectedMvcViewFolder.SetCSharpAsTemplateLanguage();
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
		
		[Test]
		public void ModelClasses_OneAvailableModelClassIsStronglyTypedViewSetToTrue_ReturnsOneModelClass()
		{
			CreateViewModel();
			CSharpProjectSelected();
			AddModelClassToProject("ICSharpCode.MyProject", "MyModel");
			viewModel.IsStronglyTypedView = true;
			
			IEnumerable<MvcModelClassViewModel> models = viewModel.ModelClasses;
			MvcModelClassViewModel model = models.First();
			
			Assert.AreEqual(1, models.Count());
			Assert.AreEqual("MyModel (ICSharpCode.MyProject)", model.Name);
		}
		
		[Test]
		public void ModelClasses_OneAvailableModelClassAndIsStronglyTypedViewIsFalse_ReturnsNoModelClasses()
		{
			CreateViewModel();
			CSharpProjectSelected();
			AddModelClassToProject("ICSharpCode.MyProject", "MyModel");
			
			IEnumerable<MvcModelClassViewModel> models = viewModel.ModelClasses;
			int count = models.Count();
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void ModelClasses_IsStronglyTypedViewChangedToTrueTwice_ModelClassesReadOnlyOnce()
		{
			CreateViewModel();
			CSharpProjectSelected();
			AddModelClassToProject("ICSharpCode.MyProject", "MyModel");
			viewModel.IsStronglyTypedView = true;
			IEnumerable<MvcModelClassViewModel> models = viewModel.ModelClasses;
			
			viewModel.IsStronglyTypedView = false;
			models = viewModel.ModelClasses;
			
			viewModel.IsStronglyTypedView = true;
			models = viewModel.ModelClasses;
			
			int callCount = fakeProject.GetModelClassesCallCount;
			
			Assert.AreEqual(1, callCount);
		}
		
		[Test]
		public void ModelClasses_OneAvailableModelClassIsStronglyTypedViewSetToTrue_PropertyChangedEventIsFiredForModelClasses()
		{
			CreateViewModel();
			CSharpProjectSelected();
			AddModelClassToProject("ICSharpCode.MyProject", "MyModel");
			
			MonitorPropertyChangedEvents();
			viewModel.IsStronglyTypedView = true;
			
			bool contains = propertyChangedEvents.Contains("ModelClasses");
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void ModelClasses_OneAvailableModelClassIsStronglyTypedViewSetToTrue_PropertyChangedEventIsFiredForIsStronglyTypedView()
		{
			CreateViewModel();
			CSharpProjectSelected();
			AddModelClassToProject("ICSharpCode.MyProject", "MyModel");
			
			MonitorPropertyChangedEvents();
			viewModel.IsStronglyTypedView = true;
			
			bool contains = propertyChangedEvents.Contains("IsStronglyTypedView");
			Assert.IsTrue(contains);
		}
	}
}
