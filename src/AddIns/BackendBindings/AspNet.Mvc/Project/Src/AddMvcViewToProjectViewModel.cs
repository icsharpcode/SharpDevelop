// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Windows.Input;

namespace ICSharpCode.AspNet.Mvc
{
	public class AddMvcViewToProjectViewModel : ViewModelBase<AddMvcViewToProjectViewModel>
	{
		IMvcViewFileGenerator viewGenerator;
		ISelectedMvcFolder selectedViewFolder;
		IMvcFileService fileService;
		MvcViewFileName viewFileName = new MvcViewFileName();
		bool closed;
		
		public AddMvcViewToProjectViewModel(ISelectedMvcFolder selectedViewFolder)
			: this(
				selectedViewFolder,
				new MvcViewFileGenerator(),
				new MvcFileService())
		{
		}
		
		public AddMvcViewToProjectViewModel(
			ISelectedMvcFolder selectedViewFolder,
			IMvcViewFileGenerator viewGenerator,
			IMvcFileService fileService)
		{
			this.selectedViewFolder = selectedViewFolder;
			this.viewGenerator = viewGenerator;
			this.fileService = fileService;
			this.viewFileName.Folder = selectedViewFolder.Path;
			
			CreateCommands();
		}
		
		void CreateCommands()
		{
			AddMvcViewCommand = new DelegateCommand(param => AddMvcView(), param => CanAddMvcView());
		}
		
		public ICommand AddMvcViewCommand { get; private set; }
		
		public string ViewName {
			get { return viewFileName.ViewName; }
			set { viewFileName.ViewName = value; }
		}
		
		public bool IsClosed {
			get { return closed; }
			set {
				closed = value;
				OnPropertyChanged(viewModel => viewModel.IsClosed);
			}
		}
		
		public bool CanAddMvcView()
		{
			return viewFileName.HasValidViewName();
		}
		
		public void AddMvcView()
		{
			GenerateMvcViewFile();
			AddMvcViewFileToProject();
			OpenMvcViewFileCreated();
			IsClosed = true;
		}
		
		void GenerateMvcViewFile()
		{
			ConfigureMvcViewGenerator();
			viewGenerator.GenerateFile(viewFileName);
		}
		
		void ConfigureMvcViewGenerator()
		{
			viewGenerator.Language = GetTemplateLanguage();
			viewGenerator.Project = selectedViewFolder.Project;
		}
		
		MvcTextTemplateLanguage GetTemplateLanguage()
		{
			return selectedViewFolder.GetTemplateLanguage();
		}
		
		void AddMvcViewFileToProject()
		{
			string fileName = viewFileName.GetFileName();
			selectedViewFolder.AddFileToProject(fileName);
		}
		
		void OpenMvcViewFileCreated()
		{
			string path = viewFileName.GetPath();
			fileService.OpenFile(path);
		}
	}
}

