// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

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
			AddMvcControllerCommand = new DelegateCommand(param => AddMvcController(), param => CanAddMvcController());
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
