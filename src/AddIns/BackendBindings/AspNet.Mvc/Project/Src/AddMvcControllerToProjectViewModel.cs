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
using System.Windows.Input;

using ICSharpCode.SharpDevelop.Widgets;
using ICSharpCode.AspNet.Mvc;

namespace ICSharpCode.AspNet.Mvc
{
	public class AddMvcControllerToProjectViewModel : ViewModelBase<AddMvcControllerToProjectViewModel>
	{
		IMvcControllerFileGenerator controllerGenerator;
		ISelectedMvcFolder selectedControllerFolder;
		IMvcTextTemplateRepository textTemplateRepository;
		MvcControllerFileName controllerFileName = new MvcControllerFileName();
		bool closed;
		List<MvcControllerTextTemplate> controllerTemplates;
		
		public AddMvcControllerToProjectViewModel(ISelectedMvcFolder selectedControllerFolder)
			: this(
				selectedControllerFolder,
				new MvcControllerFileGenerator(),
				new MvcTextTemplateRepository())
		{
		}
		
		public AddMvcControllerToProjectViewModel(
			ISelectedMvcFolder selectedControllerFolder,
			IMvcControllerFileGenerator controllerGenerator,
			IMvcTextTemplateRepository textTemplateRepository)
		{
			this.selectedControllerFolder = selectedControllerFolder;
			this.controllerGenerator = controllerGenerator;
			this.textTemplateRepository = textTemplateRepository;
			
			this.controllerFileName.Folder = selectedControllerFolder.Path;
			
			SetLanguageForFileGeneration();
			CreateCommands();
			AddControllerTemplates();
		}
		
		void SetLanguageForFileGeneration()
		{
			MvcTextTemplateLanguage language = GetTemplateLanguage();
			controllerFileName.Language = language;
		}
		
		MvcTextTemplateLanguage GetTemplateLanguage()
		{
			return selectedControllerFolder.GetTemplateLanguage();
		}
		
		void CreateCommands()
		{
			AddMvcControllerCommand = new RelayCommand(AddMvcController, CanAddMvcController);
		}
		
		public ICommand AddMvcControllerCommand { get; private set; }
		
		public string ControllerName {
			get { return controllerFileName.ControllerName; }
			set { controllerFileName.ControllerName = value; }
		}
		
		public bool IsClosed {
			get { return closed; }
			set {
				closed = value;
				OnPropertyChanged(viewModel => viewModel.IsClosed);
			}
		}
		
		public IEnumerable<MvcControllerTextTemplate> ControllerTemplates {
			get { return controllerTemplates; }
		}
		
		public MvcControllerTextTemplate SelectedControllerTemplate { get; set; }
		
		void AddControllerTemplates()
		{
			controllerTemplates = GetControllerTemplatesFromRepository();
			SelectFirstControllerTemplate();
		}
		
		List<MvcControllerTextTemplate> GetControllerTemplatesFromRepository()
		{
			var criteria = new MvcTextTemplateCriteria() {
				TemplateLanguage = GetTemplateLanguage()
			};
			return textTemplateRepository.GetMvcControllerTextTemplates(criteria).ToList();
		}
		
		void SelectFirstControllerTemplate()
		{
			SelectedControllerTemplate = controllerTemplates[0];
		}
		
		public bool CanAddMvcController()
		{
			return controllerFileName.HasValidControllerName();
		}
		
		public void AddMvcController()
		{
			if (GenerateMvcControllerFile()) {
				AddMvcControllerFileToProject();
			}
			IsClosed = true;
		}
		
		bool GenerateMvcControllerFile()
		{
			ConfigureMvcControllerGenerator();
			controllerGenerator.GenerateFile(controllerFileName);
			return !controllerGenerator.HasErrors;
		}
		
		void ConfigureMvcControllerGenerator()
		{
			controllerGenerator.Template = SelectedControllerTemplate;
			controllerGenerator.Project = selectedControllerFolder.Project;
		}
		
		void AddMvcControllerFileToProject()
		{
			string fileName = controllerFileName.GetFileName();
			selectedControllerFolder.AddFileToProject(fileName);
		}
	}
}
