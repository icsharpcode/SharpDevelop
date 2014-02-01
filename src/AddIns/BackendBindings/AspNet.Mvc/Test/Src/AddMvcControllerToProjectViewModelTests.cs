// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;

using AspNet.Mvc.Tests.Helpers;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class AddMvcControllerToProjectControllerModelTests : MvcTestsBase
	{
		AddMvcControllerToProjectViewModel viewModel;
		FakeMvcControllerFileGenerator fakeControllerGenerator;
		FakeSelectedMvcFolder fakeSelectedMvcControllerFolder;
		List<string> propertyChangedEvents;
		
		void CreateViewModelWithCSharpProject()
		{
			string path = @"d:\projects\MyAspMvcProject\Controllers";
			CreateViewModelWithControllerFolderPath(path, MvcTextTemplateLanguage.CSharp);
		}
		
		void CreateViewModelWithVisualBasicProject()
		{
			string path = @"d:\projects\MyAspMvcProject\Controllers";
			CreateViewModelWithControllerFolderPath(path, MvcTextTemplateLanguage.VisualBasic);			
		}
		
		void CreateViewModelWithControllerFolderPath(
			string path,
			MvcTextTemplateLanguage language)
		{
			var textTemplateRepository = new MvcTextTemplateRepository(@"d:\SD\ItemTemplates");
			CreateViewModelWithControllerFolderPath(path, language, textTemplateRepository);
		}
		
		void CreateViewModelWithControllerFolderPath(
			string path,
			MvcTextTemplateLanguage language,
			MvcTextTemplateRepository textTemplateRepository)
		{
			fakeSelectedMvcControllerFolder = new FakeSelectedMvcFolder();
			fakeSelectedMvcControllerFolder.TemplateLanguage = language;
			fakeSelectedMvcControllerFolder.Path = path;
			fakeControllerGenerator = new FakeMvcControllerFileGenerator();
			viewModel = new AddMvcControllerToProjectViewModel(
				fakeSelectedMvcControllerFolder,
				fakeControllerGenerator,
				textTemplateRepository);
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
		
		MvcControllerTextTemplate GetControllerTemplateFromViewModel(string templateName)
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
		
		void AddCompilerErrorToControllerGenerator()
		{
			fakeControllerGenerator.AddCompilerError();
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
				MvcTextTemplateLanguage.CSharp);
			viewModel.ControllerName = "Home";
			viewModel.AddMvcController();
			
			MvcControllerFileName controllerFileName = fakeControllerGenerator.FileNamePassedToGenerateController;
			string fileName = controllerFileName.GetPath();
			string expectedFileName = @"d:\projects\MyProject\Controllers\Home.cs";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void AddMvcController_SelectedControllerFolderIsInVisualBasicProject_VisualBasicProjectIsPassedToMvcControllerGenerator()
		{
			CreateViewModelWithCSharpProject();
			fakeSelectedMvcControllerFolder.SetVisualBasicAsTemplateLanguage();
			viewModel.AddMvcController();
			
			IMvcProject project = fakeControllerGenerator.Project;
			FakeMvcProject expectedProject = fakeSelectedMvcControllerFolder.FakeMvcProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void AddMvcController_FileIsGenerated_FileIsAddedToProject()
		{
			CreateViewModelWithCSharpProject();
			viewModel.ControllerName = "Home";
			fakeSelectedMvcControllerFolder.SetCSharpAsTemplateLanguage();
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
			
			string fileNameGenerated = fakeControllerGenerator.FileNamePassedToGenerateController.GetPath();
			string expectedFileNameGenerated = @"d:\projects\MyAspMvcProject\Controllers\HomeController.vb";
			
			Assert.AreEqual(expectedFileNameGenerated, fileNameGenerated);
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
			
			MvcControllerTextTemplate controllerTemplate = viewModel.SelectedControllerTemplate;
			
			Assert.AreEqual("Empty", controllerTemplate.Name);
		}
		
		[Test]
		public void AddMvcController_EmptyTemplateSelected_TemplatePassedToFileGenerator()
		{
			CreateViewModelWithCSharpProject();
			SelectEmptyControllerTemplate();
			MvcControllerTextTemplate expectedTemplate = viewModel.SelectedControllerTemplate;
			viewModel.AddMvcController();
			
			MvcControllerTextTemplate template = fakeControllerGenerator.Template;
			
			Assert.AreEqual(expectedTemplate, template);
		}
		
		[Test]
		public void ControllerTemplates_CSharpProjectDefaultTemplates_EmptyControllerTemplateFileNameIsCSharpTemplateFileName()
		{
			var templateRepository = new MvcTextTemplateRepository(@"d:\sd\AspNetMvcAddIn");
			CreateViewModelWithControllerFolderPath(
				@"d:\projects\MyAspMvcProject\Controllers",
				MvcTextTemplateLanguage.CSharp,
				templateRepository);
			
			MvcControllerTextTemplate template = GetControllerTemplateFromViewModel("Empty");
			string fileName = template.FileName;
			string expectedFileName = @"d:\sd\AspNetMvcAddIn\ItemTemplates\CSharp\CodeTemplates\AddController\Controller.tt";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void ControllerTemplates_VisualBasicProjectDefaultTemplates_EmptyControllerTemplateFileNameIsVisualBasicTemplateFileName()
		{
			var templateRepository = new MvcTextTemplateRepository(@"d:\sd\AspNetMvcAddIn");
			CreateViewModelWithControllerFolderPath(
				@"d:\projects\MyAspMvcProject\Controllers",
				MvcTextTemplateLanguage.VisualBasic,
				templateRepository);
			
			MvcControllerTextTemplate template = GetControllerTemplateFromViewModel("Empty");
			string fileName = template.FileName;
			string expectedFileName = @"d:\sd\AspNetMvcAddIn\ItemTemplates\VisualBasic\CodeTemplates\AddController\Controller.tt";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void AddMvcController_TemplateGenerationHasError_FileIsNotAddedToProject()
		{
			CreateViewModelWithCSharpProject();
			viewModel.ControllerName = "Home";
			AddCompilerErrorToControllerGenerator();
			
			viewModel.AddMvcController();
			
			string fileAddedToProject = fakeSelectedMvcControllerFolder.FileNamePassedToAddFile;
			Assert.IsNull(fileAddedToProject);
		}
	}
}
