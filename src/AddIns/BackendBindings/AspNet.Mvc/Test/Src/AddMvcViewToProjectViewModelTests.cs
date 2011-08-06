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
		FakeSelectedMvcViewFolder fakeSelectedMvcViewFolder;
		List<string> propertyChangedEvents;
		
		void CreateViewModel()
		{
			string path = @"d:\projects\MyAspMvcProject\Views\Home";
			CreateViewModelWithViewFolderPath(path);
		}
		
		void CreateViewModelWithViewFolderPath(string path)
		{
			fakeSelectedMvcViewFolder = new FakeSelectedMvcViewFolder();
			fakeSelectedMvcViewFolder.Path = path;
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
		
		[Test]
		public void AddMvcViewCommand_ExecutedWhenViewNameSpecified_MvcViewIsGenerated()
		{
			CreateViewModel();
			viewModel.ViewName = "MyViewPage";
			viewModel.AddMvcViewCommand.Execute(null);
			
			bool generated = fakeViewGenerator.IsGenerateViewCalled;
			
			Assert.IsTrue(generated);
		}
		
		[Test]
		public void AddMvcView_ViewNameAndViewFolderSpecified_ViewFullPathUsedToGenerateFile()
		{
			CreateViewModelWithViewFolderPath(@"d:\projects\MyProject\Views\Home");
			viewModel.ViewName = "Index";
			viewModel.AddMvcView();
			
			MvcViewFileName viewFileName = fakeViewGenerator.FileNamePassedToGenerateView;
			string fileName = viewFileName.GetPath();
			string expectedFileName = @"d:\projects\MyProject\Views\Home\Index.aspx";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void AddMvcView_SelectedViewFolderIsInVisualBasicProject_VisualBasicMvcViewFileGenerated()
		{
			CreateViewModel();
			fakeSelectedMvcViewFolder.ProjectLanguage = "VBNet";
			viewModel.AddMvcView();
			
			MvcTextTemplateLanguage templateLanguage = fakeViewGenerator.Language;
			
			Assert.AreEqual(MvcTextTemplateLanguage.VisualBasic, templateLanguage);
		}
		
		[Test]
		public void AddMvcView_SelectedViewFolderIsInCSharpProject_CSharpMvcViewFileGenerated()
		{
			CreateViewModel();
			fakeSelectedMvcViewFolder.ProjectLanguage = "C#";
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
	}
}
