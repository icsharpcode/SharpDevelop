// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Input;

using ICSharpCode.AspNet.Mvc;

namespace ICSharpCode.AspNet.Mvc
{
	public class AddMvcControllerToProjectViewModel : ViewModelBase<AddMvcControllerToProjectViewModel>
	{
		IMvcControllerFileGenerator controllerGenerator;
		ISelectedMvcFolder selectedControllerFolder;
		MvcControllerFileName controllerFileName = new MvcControllerFileName();
		bool closed;
		List<MvcControllerTemplateViewModel> controllerTemplates;
		
		public AddMvcControllerToProjectViewModel(ISelectedMvcFolder selectedControllerFolder)
			: this(
				selectedControllerFolder,
				new MvcControllerFileGenerator())
		{
		}
		
		public AddMvcControllerToProjectViewModel(
			ISelectedMvcFolder selectedControllerFolder,
			IMvcControllerFileGenerator controllerGenerator)
		{
			this.selectedControllerFolder = selectedControllerFolder;
			this.controllerGenerator = controllerGenerator;
			
			this.controllerFileName.Folder = selectedControllerFolder.Path;
			
			SetLanguageForFileGeneration();
			CreateCommands();
			AddControllerTemplates();
		}
		
		void SetLanguageForFileGeneration()
		{
			MvcTextTemplateLanguage language = GetTemplateLanguage();
			controllerFileName.Language = language;
			controllerGenerator.TemplateLanguage = language;
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
		
		public IEnumerable<MvcControllerTemplateViewModel> ControllerTemplates {
			get { return controllerTemplates; }
		}
		
		public MvcControllerTemplateViewModel SelectedControllerTemplate { get; set; }
		
		void AddControllerTemplates()
		{
			controllerTemplates = new List<MvcControllerTemplateViewModel>();
			AddEmptyControllerTemplate();
			AddEmptyReadWriteControllerTemplate();
			SelectEmptyControllerTemplate();
		}
		
		void AddEmptyReadWriteControllerTemplate()
		{
			var template = new MvcControllerTemplateViewModel() {
				Name = "EmptyReadWrite",
				Description = "Controller with create, read, update and delete actions",
				AddActionMethods = true
			};
			AddControllerTemplate(template);
		}
		
		void AddEmptyControllerTemplate()
		{
			var template = new MvcControllerTemplateViewModel() {
				Name = "Empty", 
				Description = "Empty controller",
				AddActionMethods = false
			};
			AddControllerTemplate(template);
		}
		
		void AddControllerTemplate(MvcControllerTemplateViewModel template)
		{
			controllerTemplates.Add(template);
		}
		
		void SelectEmptyControllerTemplate()
		{
			SelectedControllerTemplate = controllerTemplates[0];
		}
		
		public bool CanAddMvcController()
		{
			return controllerFileName.HasValidControllerName();
		}
		
		public void AddMvcController()
		{
			GenerateMvcControllerFile();
			AddMvcControllerFileToProject();
			IsClosed = true;
		}
		
		void GenerateMvcControllerFile()
		{
			ConfigureMvcControllerGenerator();
			controllerGenerator.GenerateFile(controllerFileName);
		}
		
		void ConfigureMvcControllerGenerator()
		{
			controllerGenerator.AddActionMethods = SelectedControllerTemplate.AddActionMethods;
			controllerGenerator.Project = selectedControllerFolder.Project;
		}
		
		void AddMvcControllerFileToProject()
		{
			string fileName = controllerFileName.GetFileName();
			selectedControllerFolder.AddFileToProject(fileName);
		}
	}
}
