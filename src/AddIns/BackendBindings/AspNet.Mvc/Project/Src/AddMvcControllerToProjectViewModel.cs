// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Input;
using ICSharpCode.AspNet.Mvc;

namespace ICSharpCode.AspNet.Mvc
{
	public class AddMvcControllerToProjectViewModel : ViewModelBase<AddMvcControllerToProjectViewModel>
	{
		IMvcControllerFileGenerator controllerGenerator;
		ISelectedMvcFolder selectedControllerFolder;
		IMvcFileService fileService;
		MvcControllerFileName controllerFileName = new MvcControllerFileName();
		bool closed;
		
		public AddMvcControllerToProjectViewModel(ISelectedMvcFolder selectedControllerFolder)
			: this(
				selectedControllerFolder,
				new MvcControllerFileGenerator(),
				new MvcFileService())
		{
		}
		
		public AddMvcControllerToProjectViewModel(
			ISelectedMvcFolder selectedControllerFolder,
			IMvcControllerFileGenerator controllerGenerator,
			IMvcFileService fileService)
		{
			this.selectedControllerFolder = selectedControllerFolder;
			this.controllerGenerator = controllerGenerator;
			this.fileService = fileService;
			this.controllerFileName.Folder = selectedControllerFolder.Path;
		
			CreateCommands();
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
		
		public bool CanAddMvcController()
		{
			return controllerFileName.HasValidControllerName();
		}
		
		public void AddMvcController()
		{
			GenerateMvcControllerFile();
			AddMvcControllerFileToProject();
			OpenMvcControllerFileCreated();
			IsClosed = true;
		}
		
		void GenerateMvcControllerFile()
		{
			ConfigureMvcControllerGenerator();
			controllerGenerator.GenerateFile(controllerFileName);
		}
		
		void ConfigureMvcControllerGenerator()
		{
			controllerGenerator.Language = GetTemplateLanguage();
			controllerGenerator.Project = selectedControllerFolder.Project;
		}
		
		MvcTextTemplateLanguage GetTemplateLanguage()
		{
			return selectedControllerFolder.GetTemplateLanguage();
		}
		
		void AddMvcControllerFileToProject()
		{
			string fileName = controllerFileName.GetFileName();
			selectedControllerFolder.AddFileToProject(fileName);
		}
		
		void OpenMvcControllerFileCreated()
		{
			string path = controllerFileName.GetPath();
			fileService.OpenFile(path);
		}
	}
}
