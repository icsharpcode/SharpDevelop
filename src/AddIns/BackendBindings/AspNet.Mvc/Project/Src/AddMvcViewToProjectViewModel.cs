// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace ICSharpCode.AspNet.Mvc
{
	public class AddMvcViewToProjectViewModel : ViewModelBase<AddMvcViewToProjectViewModel>
	{
		IMvcViewFileGenerator viewGenerator;
		ISelectedMvcFolder selectedViewFolder;
		IMvcTextTemplateRepository textTemplateRepository;
		MvcViewFileName viewFileName = new MvcViewFileName();
		bool closed;
		List<MvcViewEngineViewModel> viewEngines;
		MvcViewEngineViewModel selectedViewEngine;
		bool isStronglyTypedView;
		bool isContentPage;
		bool isSelectMasterPageViewOpen;
		MvcProjectFile selectedMasterPage;
		string masterPageFile = String.Empty;
		List<MvcViewTextTemplate> viewTemplates = new List<MvcViewTextTemplate>();
		MvcViewTextTemplate selectedViewTemplate;
		MvcModelClassViewModelsForSelectedFolder modelClassesForSelectedFolder;
		MvcModelClassViewModel selectedModelClass;
		
		public AddMvcViewToProjectViewModel(ISelectedMvcFolder selectedViewFolder)
			: this(
				selectedViewFolder,
				new MvcViewFileGenerator(),
				new MvcTextTemplateRepository())
		{
		}
		
		public AddMvcViewToProjectViewModel(
			ISelectedMvcFolder selectedViewFolder,
			IMvcViewFileGenerator viewGenerator,
			IMvcTextTemplateRepository textTemplateRepository)
		{
			this.selectedViewFolder = selectedViewFolder;
			this.viewGenerator = viewGenerator;
			this.textTemplateRepository = textTemplateRepository;
			this.viewFileName.Folder = selectedViewFolder.Path;
			this.ModelClassName = String.Empty;
			this.PrimaryContentPlaceHolderId = "Main";
			this.MasterPages = new ObservableCollection<MvcProjectFile>();
			
			CreateModelClassesForSelectedFolder();
			CreateCommands();
			AddViewEngines();
			SetDefaultMasterPage();
		}
		
		void CreateModelClassesForSelectedFolder()
		{
			modelClassesForSelectedFolder = new MvcModelClassViewModelsForSelectedFolder(selectedViewFolder);
		}
		
		void CreateCommands()
		{
			AddMvcViewCommand = new DelegateCommand(param => AddMvcView(), param => CanAddMvcView());
			OpenSelectMasterPageViewCommand = new DelegateCommand(param => OpenSelectMasterPageView());
			CloseSelectMasterPageViewCommand = new DelegateCommand(param => CloseSelectMasterPageView());
			SelectMasterPageCommand = new DelegateCommand(param => SelectMasterPage(), param => CanSelectMasterPage());
		}
		
		public ICommand AddMvcViewCommand { get; private set; }
		public ICommand OpenSelectMasterPageViewCommand { get; private set; }
		public ICommand CloseSelectMasterPageViewCommand { get; private set; }
		public ICommand SelectMasterPageCommand { get; private set; }
		
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
		
		void SetDefaultMasterPage()
		{
			if (IsAspxViewEngineSelected) {
				MasterPageFile = "~/Views/Shared/Site.Master";
			} else {
				MasterPageFile = String.Empty;
			}
		}
		
		public IEnumerable<MvcViewEngineViewModel> ViewEngines {
			get { return viewEngines; }
		}
		
		public MvcViewEngineViewModel SelectedViewEngine {
			get { return selectedViewEngine; }
			set {
				selectedViewEngine = value;
				viewFileName.TemplateType = selectedViewEngine.TemplateType;
				OnSelectedViewEngineChanged();
			}
		}
		
		void OnSelectedViewEngineChanged()
		{
			IsAspxViewEngineSelected = selectedViewEngine.TemplateType.IsAspx();
			IsRazorViewEngineSelected = selectedViewEngine.TemplateType.IsRazor();
			
			SetDefaultMasterPage();
			AddViewTemplates();
			
			OnPropertyChanged(viewModel => viewModel.IsAspxViewEngineSelected);
			OnPropertyChanged(viewModel => viewModel.IsRazorViewEngineSelected);
			OnPropertyChanged(viewModel => viewModel.MasterPageFile);
		}
		
		void AddViewTemplates()
		{
			viewTemplates = GetViewTemplates();
			OnPropertyChanged(viewModel => viewModel.ViewTemplates);
			SelectDefaultViewTemplate();
		}
		
		List<MvcViewTextTemplate> GetViewTemplates()
		{
			var templateCriteria = new MvcTextTemplateCriteria() {
				TemplateLanguage = GetTemplateLanguage(),
				TemplateType = selectedViewEngine.TemplateType
			};
			return textTemplateRepository
				.GetMvcViewTextTemplates(templateCriteria)
				.OrderBy(t => t.Name)
				.ToList();
		}
		
		void SelectDefaultViewTemplate()
		{
			SelectedViewTemplate = GetEmptyViewTemplate();
			if (SelectedViewTemplate == null) {
				SelectedViewTemplate = viewTemplates.FirstOrDefault();
			}
		}
		
		MvcViewTextTemplate GetEmptyViewTemplate()
		{
			return viewTemplates.Find(t => t.IsEmptyTemplate());
		}
		
		public IEnumerable<MvcViewTextTemplate> ViewTemplates {
			get { return viewTemplates; }
		}
		
		public MvcViewTextTemplate SelectedViewTemplate {
			get { return selectedViewTemplate; }
			set {
				selectedViewTemplate = value;
				OnPropertyChanged(viewModel => viewModel.SelectedViewTemplate);
			}
		}
		
		public bool IsRazorViewEngineSelected { get; private set; }
		public bool IsAspxViewEngineSelected { get; private set; }
		
		public bool IsContentPage {
			get { return isContentPage; }
			set {
				isContentPage = value;
				OnPropertyChanged(viewModel => viewModel.IsContentPage);
				OnPropertyChanged(viewModel => viewModel.IsContentPagePropertiesEnabled);
			}
		}
		
		public bool IsContentPageEnabled {
			get { return !IsPartialView; }
		}
		
		public bool IsContentPagePropertiesEnabled {
			get { return IsContentPage && IsContentPageEnabled; }
		}
		
		public bool CanAddMvcView()
		{
			return viewFileName.HasValidViewName();
		}
		
		public void AddMvcView()
		{
			if (GenerateMvcViewFile()) {
				AddMvcViewFileToProject();
			}
			IsClosed = true;
		}
		
		bool GenerateMvcViewFile()
		{
			ConfigureMvcViewGenerator();
			viewFileName.TemplateLanguage = GetTemplateLanguage();
			viewGenerator.GenerateFile(viewFileName);
			return !viewGenerator.HasErrors;
		}
		
		void ConfigureMvcViewGenerator()
		{
			viewGenerator.IsContentPage	= IsContentPage;
			viewGenerator.MasterPageFile = GetMasterPageFile();
			viewGenerator.ModelClassName = GetModelClassName();
			viewGenerator.ModelClassAssemblyLocation = GetModelClassAssemblyLocation();
			viewGenerator.PrimaryContentPlaceHolderId = GetPrimaryContentPlaceHolderId();
			viewGenerator.Project = selectedViewFolder.Project;
			viewGenerator.Template = selectedViewTemplate;
		}
		
		string GetMasterPageFile()
		{
			if (IsContentPage) {
				return MasterPageFile;
			}
			return String.Empty;
		}
		
		string GetModelClassName()
		{
			if (IsStronglyTypedView) {
				if (SelectedModelClass != null) {
					return SelectedModelClass.FullName;
				}
				return ModelClassName.Trim();
			}
			return String.Empty;
		}
		
		string GetModelClassAssemblyLocation()
		{
			if (IsStronglyTypedView) {
				if (SelectedModelClass != null) {
					return SelectedModelClass.AssemblyLocation;
				}
			}
			return String.Empty;
		}
		
		string GetPrimaryContentPlaceHolderId()
		{
			if (IsContentPage) {
				return PrimaryContentPlaceHolderId;
			}
			return String.Empty;
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
		
		public bool IsPartialView {
			get { return viewFileName.IsPartialView; }
			set {
				viewFileName.IsPartialView = value;
				OnPropertyChanged(viewModel => viewModel.IsContentPageEnabled);
				OnPropertyChanged(viewModel => viewModel.IsContentPagePropertiesEnabled);
			}
		}
		
		public bool IsStronglyTypedView {
			get { return isStronglyTypedView; }
			set {
				isStronglyTypedView = value;
				if (isStronglyTypedView) {
					modelClassesForSelectedFolder.GetModelClasses();
					OnPropertyChanged(viewModel => viewModel.ModelClasses);
				}
				OnPropertyChanged(viewModel => viewModel.IsStronglyTypedView);
				OnPropertyChanged(viewModel => viewModel.IsViewTemplateEnabled);
			}
		}
		
		public bool IsViewTemplateEnabled {
			get { return IsStronglyTypedView && (SelectedModelClass != null); }
		}
		
		public IEnumerable<MvcModelClassViewModel> ModelClasses {
			get { return modelClassesForSelectedFolder.ModelClasses; }
		}
		
		public MvcModelClassViewModel SelectedModelClass {
			get { return selectedModelClass; }
			set {
				selectedModelClass = value;
				if (selectedModelClass == null) {
					SelectDefaultViewTemplate();
				}
				OnPropertyChanged(viewModel => viewModel.SelectedModelClass);
				OnPropertyChanged(viewModel => viewModel.IsViewTemplateEnabled);
			}
		}
		
		string modelClassName;
		
		public string ModelClassName {
			get { return modelClassName; }
			set {
				modelClassName = value;
				if (!ModelClassNameMatchesSelectedModelClassName()) {
					SelectedModelClass = null;
				}
			}
		}
		
		bool ModelClassNameMatchesSelectedModelClassName()
		{
			if (selectedModelClass != null) {
				return selectedModelClass.Name == modelClassName;
			}
			return false;
		}
		
		public string PrimaryContentPlaceHolderId { get; set; }
		
		public string MasterPageFile {
			get { return masterPageFile; }
			set {
				masterPageFile = value;
				OnPropertyChanged(viewModel => viewModel.MasterPageFile);
			}
		}
		
		public bool IsSelectMasterPageViewOpen {
			get { return isSelectMasterPageViewOpen; }
			set {
				isSelectMasterPageViewOpen = value;
				OnPropertyChanged(viewModel => viewModel.IsSelectMasterPageViewOpen);
			}
		}
		
		public void OpenSelectMasterPageView()
		{
			UpdateMasterPages();
			IsSelectMasterPageViewOpen = true;
		}
		
		void UpdateMasterPages()
		{
			MasterPages.Clear();
			foreach (MvcProjectFile fileName in GetMasterPageFileNames()) {
				MasterPages.Add(fileName);
			}
		}
		
		IEnumerable<MvcProjectFile> GetMasterPageFileNames()
		{
			IEnumerable<MvcProjectFile> unsortedMasterPages = GetMasterPageFileNamesForSelectedViewEngine();
			var masterPages = new List<MvcProjectFile>(unsortedMasterPages);
			masterPages.Sort();
			return masterPages;
		}
		
		IEnumerable<MvcProjectFile> GetMasterPageFileNamesForSelectedViewEngine()
		{
			IMvcProject project = selectedViewFolder.Project;
			if (IsAspxViewEngineSelected) {
				return project.GetAspxMasterPageFiles();
			}
			return project.GetRazorFiles();
		}
		
		public void CloseSelectMasterPageView()
		{
			IsSelectMasterPageViewOpen = false;
		}
		
		public ObservableCollection<MvcProjectFile> MasterPages { get; private set; }
		
		public MvcProjectFile SelectedMasterPage {
			get { return selectedMasterPage; }
			set { selectedMasterPage = value; }
		}
		
		public void SelectMasterPage()
		{
			if (selectedMasterPage != null) {
				MasterPageFile = selectedMasterPage.VirtualPath;
			}
			IsSelectMasterPageViewOpen = false;
		}
		
		public bool CanSelectMasterPage()
		{
			return selectedMasterPage != null;
		}
	}
}

