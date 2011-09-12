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
		
		void AddModelClassToProject(string fullyQualifiedClassName)
		{
			fakeProject.AddModelClassToProject(fullyQualifiedClassName);
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
		public void AddMvcViewCommand_ViewNameIsNotEmptyString_CommandIsEnabled()
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
		
		[Test]
		public void AddMvcView_ModelClassSelected_ModelClassNameSetInGenerator()
		{
			CreateViewModel();
			CSharpProjectSelected();
			AddModelClassToProject("ICSharpCode.MyProject.MyModel");
			viewModel.IsStronglyTypedView = true;
			viewModel.SelectedModelClass = viewModel.ModelClasses.First();
			viewModel.AddMvcView();
			
			string modelClassName = fakeViewGenerator.ModelClassName;
			
			Assert.AreEqual("ICSharpCode.MyProject.MyModel", modelClassName);
		}
		
		[Test]
		public void AddMvcView_ModelClassSelectedThenIsStronglyTypedIsSetToFalse_ModelClassNameIsInGeneratorIsEmptyString()
		{
			CreateViewModel();
			CSharpProjectSelected();
			AddModelClassToProject("ICSharpCode.MyProject.MyModel");
			viewModel.IsStronglyTypedView = true;
			viewModel.SelectedModelClass = viewModel.ModelClasses.First();
			viewModel.IsStronglyTypedView = false;
			viewModel.AddMvcView();
			
			string modelClassName = fakeViewGenerator.ModelClassName;
			
			Assert.AreEqual(String.Empty, modelClassName);
		}
		
		[Test]
		public void AddMvcView_ModelClassNameTypedInByUser_ModelClassNameIsUsedInGenerator()
		{
			CreateViewModel();
			CSharpProjectSelected();
			AddModelClassToProject("ICSharpCode.MyProject.MyModel");
			viewModel.IsStronglyTypedView = true;
			viewModel.SelectedModelClass = null;
			viewModel.ModelClassName = "MyNamespace.MyClass";
			viewModel.AddMvcView();
			
			string modelClassName = fakeViewGenerator.ModelClassName;
			
			Assert.AreEqual("MyNamespace.MyClass", modelClassName);
		}
		
		[Test]
		public void ModelClassName_NewViewModelInstanceCreated_ReturnsEmptyStringByDefault()
		{
			CreateViewModel();
			
			string modelClassName = viewModel.ModelClassName;
			
			Assert.AreEqual(String.Empty, modelClassName);
		}
		
		[Test]
		public void AddMvcView_ModelClassNameTypedInByUserHasSpacesAtStartAndEnd_ModelClassNameWithoutSpacesIsUsedInGenerator()
		{
			CreateViewModel();
			CSharpProjectSelected();
			AddModelClassToProject("ICSharpCode.MyProject.MyModel");
			viewModel.IsStronglyTypedView = true;
			viewModel.SelectedModelClass = null;
			viewModel.ModelClassName = "   MyNamespace.MyClass     ";
			viewModel.AddMvcView();
			
			string modelClassName = fakeViewGenerator.ModelClassName;
			
			Assert.AreEqual("MyNamespace.MyClass", modelClassName);
		}
		
		[Test]
		public void IsRazorViewEngineSelected_RazorViewEngineSelected_ReturnsTrue()
		{
			CreateViewModel();
			CSharpProjectSelected();
			SelectRazorViewEngine();
			
			bool selected = viewModel.IsRazorViewEngineSelected;
			
			Assert.IsTrue(selected);
		}
		
		[Test]
		public void IsAspxViewEngineSelected_RazorViewEngineSelected_ReturnsFalse()
		{
			CreateViewModel();
			CSharpProjectSelected();
			SelectRazorViewEngine();
			
			bool selected = viewModel.IsAspxViewEngineSelected;
			
			Assert.IsFalse(selected);
		}
		
		[Test]
		public void IsAspxViewEngineSelected_AspxViewEngineSelected_ReturnsTrue()
		{
			CreateViewModel();
			CSharpProjectSelected();
			SelectAspxViewEngine();
			
			bool selected = viewModel.IsAspxViewEngineSelected;
			
			Assert.IsTrue(selected);
		}
		
		[Test]
		public void IsRazorViewEngineSelected_AspxViewEngineSelected_ReturnsFalse()
		{
			CreateViewModel();
			CSharpProjectSelected();
			SelectAspxViewEngine();
			
			bool selected = viewModel.IsRazorViewEngineSelected;
			
			Assert.IsFalse(selected);
		}
		
		[Test]
		public void IsAspxViewEngineSelected_NewViewModelCreatedAndUserHasNotMadeSelection_ReturnsTrue()
		{
			CreateViewModel();
			CSharpProjectSelected();
			
			bool selected = viewModel.IsAspxViewEngineSelected;
			
			Assert.IsTrue(selected);
		}
		
		[Test]
		public void SelectedViewEngine_ChangedToAspxViewEngineFromRazorViewEngine_PropertyChangedEventsFiredForIsAspxViewEngineSelected()
		{
			CreateViewModel();
			CSharpProjectSelected();
			SelectRazorViewEngine();
			MonitorPropertyChangedEvents();
			SelectAspxViewEngine();
			
			bool fired = propertyChangedEvents.Contains("IsAspxViewEngineSelected");
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void SelectedViewEngine_ChangedToRazorViewEngineFromAspxViewEngine_PropertyChangedEventsFiredForIsRazorViewEngineSelected()
		{
			CreateViewModel();
			CSharpProjectSelected();
			SelectAspxViewEngine();
			MonitorPropertyChangedEvents();
			SelectRazorViewEngine();
			
			bool fired = propertyChangedEvents.Contains("IsRazorViewEngineSelected");
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void IsContentPage_ChangedFromFalseToTrue_PropertyChangedEventFiresForIsContentPage()
		{
			CreateViewModel();
			CSharpProjectSelected();
			viewModel.IsContentPage = false;
			MonitorPropertyChangedEvents();
			viewModel.IsContentPage = true;
			
			bool fired = propertyChangedEvents.Contains("IsContentPage");
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void AddMvcView_IsContentPageIsTrue_IsContentPageIsSetToTrueOnViewGenerator()
		{
			CreateViewModel();
			viewModel.IsContentPage = true;
			viewModel.AddMvcView();
			
			bool contentPage = fakeViewGenerator.IsContentPage;
			
			Assert.IsTrue(contentPage);
		}
		
		[Test]
		public void AddMvcView_ContentPageIsDefinedAndIsContentPageIsTrue_ContentPageIsConfiguredOnViewGenerator()
		{
			CreateViewModel();
			viewModel.IsContentPage = true;
			viewModel.MasterPageFile = "~/Views/Shared/Site.Master";
			viewModel.AddMvcView();
			
			string masterPage = fakeViewGenerator.MasterPageFile;
			
			Assert.AreEqual("~/Views/Shared/Site.Master", masterPage);
		}
		
		[Test]
		public void AddMvcView_ContentPageIsDefinedAndIsContentPageIsFalse_ContentPageIsEmptyStringOnViewGenerator()
		{
			CreateViewModel();
			viewModel.IsContentPage = false;
			viewModel.MasterPageFile = "~/Views/Shared/Site.Master";
			viewModel.AddMvcView();
			
			string masterPage = fakeViewGenerator.MasterPageFile;
			
			Assert.AreEqual(String.Empty, masterPage);
		}
		
		[Test]
		public void MasterPageFile_DefaultValue_ReturnsSharedSiteMasterPageFile()
		{
			CreateViewModel();
			string masterPage = viewModel.MasterPageFile;
			
			Assert.AreEqual("~/Views/Shared/Site.Master", masterPage);
		}
		
		[Test]
		public void MasterPageFile_ViewEngineChangedToRazor_ReturnsEmptyString()
		{
			CreateViewModel();
			SelectRazorViewEngine();
			string masterPage = viewModel.MasterPageFile;
			
			Assert.AreEqual(String.Empty, masterPage);
		}
		
		[Test]
		public void MasterPageFile_ViewEngineChangedToRazorAndThenBackToAspx_ReturnsSharedSiteMasterPageFile()
		{
			CreateViewModel();
			SelectRazorViewEngine();
			viewModel.MasterPageFile = "Test";
			SelectAspxViewEngine();
			
			string masterPage = viewModel.MasterPageFile;
			
			Assert.AreEqual("~/Views/Shared/Site.Master", masterPage);
		}
		
		[Test]
		public void MasterPageFile_ViewEngineChangedToRazor_PropertyChangedEventFiredForMasterPageFile()
		{
			CreateViewModel();
			MonitorPropertyChangedEvents();
			SelectRazorViewEngine();
			bool fired = propertyChangedEvents.Contains("MasterPageFile");
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void PrimaryContentPlaceHolderId_DefaultValue_ReturnsMain()
		{
			CreateViewModel();
			string id = viewModel.PrimaryContentPlaceHolderId;
			
			Assert.AreEqual("Main", id);
		}
		
		[Test]
		public void AddMvcView_PrimaryContentPlaceHolderIdIsDefined_PrimaryContentPlaceHolderIdIsSetOnViewGenerator()
		{
			CreateViewModel();
			viewModel.IsContentPage = true;
			viewModel.PrimaryContentPlaceHolderId = "Test";
			viewModel.AddMvcView();
			
			string id = fakeViewGenerator.PrimaryContentPlaceHolderId;
			
			Assert.AreEqual("Test", id);
		}
		
		[Test]
		public void AddMvcView_PrimaryContentPlaceHolderIdIsDefinedAndIsContentPageIsFalse_PrimaryContentPlaceHolderIdIsEmptyStringOnViewGenerator()
		{
			CreateViewModel();
			viewModel.IsContentPage = false;
			viewModel.PrimaryContentPlaceHolderId = "Test";
			viewModel.AddMvcView();
			
			string id = fakeViewGenerator.PrimaryContentPlaceHolderId;
			
			Assert.AreEqual(String.Empty, id);
		}
		
		[Test]
		public void OpenSelectMasterPageViewCommand_Executed_SelectMasterPageViewModelIsOpened()
		{
			CreateViewModel();
			viewModel.OpenSelectMasterPageViewCommand.Execute(null);
			
			bool open = viewModel.IsSelectMasterPageViewOpen;
			
			Assert.IsTrue(open);
		}
		
		[Test]
		public void OpenSelectMasterPageViewCommand_Executed_IsSelectMasterPageViewOpenPropertyChanged()
		{
			CreateViewModel();
			MonitorPropertyChangedEvents();
			viewModel.OpenSelectMasterPageViewCommand.Execute(null);
			
			bool contains = propertyChangedEvents.Contains("IsSelectMasterPageViewOpen");
			
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void IsSelectMasterPageViewOpen_NotExecuted_SelectMasterPageViewModelIsNotOpened()
		{
			CreateViewModel();
			
			bool open = viewModel.IsSelectMasterPageViewOpen;
			
			Assert.IsFalse(open);
		}
		
		[Test]
		public void CloseSelectMasterPageViewCommand_Executed_SelectMasterPageViewModelIsClosed()
		{
			CreateViewModel();
			viewModel.OpenSelectMasterPageView();
			viewModel.CloseSelectMasterPageViewCommand.Execute(null);
			
			bool open = viewModel.IsSelectMasterPageViewOpen;
			
			Assert.IsFalse(open);
		}
		
		[Test]
		public void CloseSelectMasterPageViewCommand_Executed_IsSelectMasterPageViewOpenPropertyChanged()
		{
			CreateViewModel();
			MonitorPropertyChangedEvents();
			viewModel.CloseSelectMasterPageViewCommand.Execute(null);
			
			bool contains = propertyChangedEvents.Contains("IsSelectMasterPageViewOpen");
			
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void MasterPages_ProjectContainsOneAspxMasterPage_ReturnsOneAspxMasterPage()
		{
			CreateViewModel();
			SelectAspxViewEngine();
			var masterPageFile = new MvcMasterPageFileName() {
				FullPath = @"d:\projects\MyProject\Views\Shared\Site.Master",
				FileName = "Site.Master",
				FolderRelativeToProject = @"Views\Shared"
			};
			fakeProject.AddMasterPageFile(masterPageFile);
			viewModel.OpenSelectMasterPageView();
			
			var expectedFileNames = new MvcMasterPageFileName[] {
				masterPageFile
			};
			
			ObservableCollection<MvcMasterPageFileName> fileNames = viewModel.MasterPages;
			
			Assert.AreEqual(expectedFileNames, fileNames);
		}
		
		[Test]
		public void MasterPages_CloseAndReOpenSelectMasterPageViewWhenProjectContainsOneAspxMasterPage_ReturnsOneAspxMasterPage()
		{
			CreateViewModel();
			SelectAspxViewEngine();
			var masterPageFile = new MvcMasterPageFileName();
			fakeProject.AddMasterPageFile(masterPageFile);
			viewModel.OpenSelectMasterPageView();
			viewModel.CloseSelectMasterPageView();
			viewModel.OpenSelectMasterPageView();
			
			var expectedFileNames = new MvcMasterPageFileName[] {
				masterPageFile
			};
			
			ObservableCollection<MvcMasterPageFileName> fileNames = viewModel.MasterPages;
			
			Assert.AreEqual(expectedFileNames, fileNames);
		}
		
		[Test]
		public void SelectMasterPageCommand_MasterPageNotSelected_CommandIsDisabled()
		{
			CreateViewModel();
			viewModel.SelectedMasterPage = null;
			
			bool canExecute = viewModel.SelectMasterPageCommand.CanExecute(null);
			
			Assert.IsFalse(canExecute);
		}
		
		[Test]
		public void SelectMasterPageCommand_MasterPageIsSelected_CommandIsEnabled()
		{
			CreateViewModel();
			viewModel.SelectedMasterPage = new MvcMasterPageFileName();
			
			bool canExecute = viewModel.SelectMasterPageCommand.CanExecute(null);
			
			Assert.IsTrue(canExecute);
		}
		
		[Test]
		public void SelectMasterPageCommand_Executed_SelectMasterPageViewModelIsClosed()
		{
			CreateViewModel();
			viewModel.OpenSelectMasterPageView();
			viewModel.SelectMasterPageCommand.Execute(null);
			
			bool open = viewModel.IsSelectMasterPageViewOpen;
			
			Assert.IsFalse(open);
		}
		
		[Test]
		public void SelectMasterPageCommand_Executed_IsSelectMasterPageViewOpenPropertyChanged()
		{
			CreateViewModel();
			viewModel.OpenSelectMasterPageView();
			MonitorPropertyChangedEvents();
			viewModel.SelectMasterPageCommand.Execute(null);
			
			bool contains = propertyChangedEvents.Contains("IsSelectMasterPageViewOpen");
			
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void SelectMasterPage_MasterPageSelected_MasterPageFileNameUpdated()
		{
			CreateViewModel();
			viewModel.MasterPageFile = "test.master";
			viewModel.OpenSelectMasterPageView();
			var masterPageFileName = new MvcMasterPageFileName() {
				FullPath = @"d:\projects\MyProject\Views\Shared\site.master",
				FileName = "site.master",
				FolderRelativeToProject = @"Views\Shared",
				VirtualPath = "~/Views/Shared/site.master"
			};
			viewModel.SelectedMasterPage = masterPageFileName;
			viewModel.SelectMasterPage();
			
			string updatedMasterPageFile = viewModel.MasterPageFile;
			
			Assert.AreEqual("~/Views/Shared/site.master", updatedMasterPageFile);
		}
		
		[Test]
		public void SelectMasterPage_MasterPageSelected_MasterPageFilePropertyChangedEventFired()
		{
			CreateViewModel();
			viewModel.OpenSelectMasterPageView();
			var masterPageFileName = new MvcMasterPageFileName();
			viewModel.SelectedMasterPage = masterPageFileName;
			
			MonitorPropertyChangedEvents();
			viewModel.SelectMasterPage();
			
			bool fired = propertyChangedEvents.Contains("MasterPageFile");
			
			Assert.IsTrue(fired);
		}
		
		[Test]
		public void MasterPages_ProjectContainsTwoAspxMasterPagesInIncorrectSortOrder_ReturnsTwoAspxMasterPagesSorted()
		{
			CreateViewModel();
			SelectAspxViewEngine();
			var masterPageFile1 = new MvcMasterPageFileName() {
				FullPath = @"d:\projects\MyProject\Views\Shared\ViewSite.Master",
				FileName = "ViewSite.Master",
				FolderRelativeToProject = @"Views\Shared"
			};
			fakeProject.AddMasterPageFile(masterPageFile1);
			var masterPageFile2 = new MvcMasterPageFileName() {
				FullPath = @"d:\projects\MyProject\Views\Shared\Site.Master",
				FileName = "Site.Master",
				FolderRelativeToProject = @"Views\Shared"
			};
			fakeProject.AddMasterPageFile(masterPageFile2);
			viewModel.OpenSelectMasterPageView();
			
			var expectedFileNames = new MvcMasterPageFileName[] {
				masterPageFile2,
				masterPageFile1
			};
			
			ObservableCollection<MvcMasterPageFileName> fileNames = viewModel.MasterPages;
			
			Assert.AreEqual(expectedFileNames, fileNames);
		}
	}
}
