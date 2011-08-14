// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
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
		List<MvcViewEngineViewModel> viewEngines;
		MvcViewEngineViewModel selectedViewEngine;
		
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
			AddViewEngines();
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
		
		void AddViewEngines()
		{
			viewEngines = new List<MvcViewEngineViewModel>();
			AddAspxViewEngine();
			AddRazorViewEngine();
			SelectAspxViewEngine();
		}
		
		void AddAspxViewEngine()
		{
			var viewType = new MvcViewEngineViewModel("ASPX", MvcTextTemplateType.Aspx);
			AddViewEngine(viewType);
		}
		
		void AddRazorViewEngine()
		{
			var viewType = new MvcViewEngineViewModel("Razor", MvcTextTemplateType.Razor);
			AddViewEngine(viewType);
		}
		
		void AddViewEngine(MvcViewEngineViewModel viewEngine)
		{
			viewEngines.Add(viewEngine);			
		}
		
		void SelectAspxViewEngine()
		{
			SelectedViewEngine = viewEngines[0];
		}
		
		public IEnumerable<MvcViewEngineViewModel> ViewEngines {
			get { return viewEngines; }
		}
		
		public MvcViewEngineViewModel SelectedViewEngine {
			get { return selectedViewEngine; }
			set {
				selectedViewEngine = value;
				viewFileName.TemplateType = selectedViewEngine.TemplateType;
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
			viewFileName.TemplateLanguage = GetTemplateLanguage();
			viewGenerator.GenerateFile(viewFileName);
		}
		
		void ConfigureMvcViewGenerator()
		{
			viewGenerator.Project = selectedViewFolder.Project;
			viewGenerator.TemplateLanguage = GetTemplateLanguage();
			viewGenerator.TemplateType = selectedViewEngine.TemplateType;
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
		
		public bool IsPartialView {
			get { return viewFileName.IsPartialView; }
			set { viewFileName.IsPartialView = value; }
		}
	}
}

