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
		ISelectedMvcViewFolder selectedViewFolder;
		MvcViewFileName viewFileName = new MvcViewFileName();
		bool closed;
		
		public AddMvcViewToProjectViewModel(
			ISelectedMvcViewFolder selectedViewFolder,
			IMvcViewFileGenerator viewGenerator)
		{
			this.selectedViewFolder = selectedViewFolder;
			this.viewGenerator = viewGenerator;
			this.viewFileName.ViewFolder = selectedViewFolder.Path;
			
			CreateCommands();
		}
		
		void CreateCommands()
		{
			AddMvcViewCommand = new DelegateCommand(param => AddMvcView());
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
		
		public void AddMvcView()
		{
			GenerateMvcViewFile();
			AddMvcViewFileToProject();
			IsClosed = true;
		}
		
		void GenerateMvcViewFile()
		{
			ConfigureMvcViewGenerator();
			viewGenerator.GenerateView(viewFileName);
		}
		
		void ConfigureMvcViewGenerator()
		{
			viewGenerator.Language = GetTemplateLanguage();
			viewGenerator.Project = selectedViewFolder.Project;
		}
		
		MvcTextTemplateLanguage GetTemplateLanguage()
		{
			string language = selectedViewFolder.Project.Language;
			return MvcTextTemplateLanguageConverter.Convert(language);
		}
		
		void AddMvcViewFileToProject()
		{
			string fileName = viewFileName.GetFileName();
			selectedViewFolder.AddFileToProject(fileName);
		}
	}
}

