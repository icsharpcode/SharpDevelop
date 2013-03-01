// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

using ICSharpCode.AvalonEdit;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageManagementConsoleViewModel : ViewModelBase<PackageManagementConsoleViewModel>
	{
		RegisteredPackageSources registeredPackageSources;
		IPackageManagementProjectService projectService;
		IPackageManagementConsoleHost consoleHost;
		
		DelegateCommand clearConsoleCommand;
		
		ObservableCollection<PackageSourceViewModel> packageSources = new ObservableCollection<PackageSourceViewModel>();
		PackageSourceViewModel activePackageSource;
		
		PackageManagementConsole packageManagementConsole;
		
		public PackageManagementConsoleViewModel(
			RegisteredPackageSources registeredPackageSources,
			IPackageManagementProjectService projectService,
			IPackageManagementConsoleHost consoleHost)
		{
			this.registeredPackageSources = registeredPackageSources;
			this.projectService = projectService;
			this.consoleHost = consoleHost;
			
			Init();
		}
		
		void Init()
		{
			CreateCommands();
			UpdatePackageSourceViewModels();
			ReceiveNotificationsWhenPackageSourcesUpdated();
			UpdateDefaultProject();
			ReceiveNotificationsWhenSolutionIsUpdated();
			InitConsoleHost();
		}
		
		void InitConsoleHost()
		{
			packageManagementConsole = CreateConsole();
			consoleHost.ScriptingConsole = packageManagementConsole;
			consoleHost.Run();
		}
		
		protected virtual PackageManagementConsole CreateConsole()
		{
			return new PackageManagementConsole();
		}
		
		void CreateCommands()
		{
			clearConsoleCommand = new DelegateCommand(param => ClearConsole());
		}
		
		public ICommand ClearConsoleCommand {
			get { return clearConsoleCommand; }
		}
		
		public void ClearConsole()
		{
			consoleHost.Clear();
			consoleHost.WritePrompt();
		}
		
		void UpdatePackageSourceViewModels()
		{
			packageSources.Clear();
			AddEnabledPackageSourceViewModels();
			AddAggregatePackageSourceViewModelIfMoreThanOnePackageSourceViewModelAdded();
			SelectActivePackageSource();
		}

		void AddEnabledPackageSourceViewModels()
		{
			foreach (PackageSource packageSource in registeredPackageSources.GetEnabledPackageSources()) {
				AddPackageSourceViewModel(packageSource);
			}
		}
		
		void AddPackageSourceViewModel(PackageSource packageSource)
		{
			var viewModel = new PackageSourceViewModel(packageSource);
			packageSources.Add(viewModel);
		}
		
		void AddAggregatePackageSourceViewModelIfMoreThanOnePackageSourceViewModelAdded()
		{
			if (packageSources.Count > 1) {
				AddPackageSourceViewModel(RegisteredPackageSourceSettings.AggregatePackageSource);
			}
		}
		
		void SelectActivePackageSource()
		{
			PackageSource activePackageSource = consoleHost.ActivePackageSource;
			foreach (PackageSourceViewModel packageSourceViewModel in packageSources) {
				if (packageSourceViewModel.GetPackageSource().Equals(activePackageSource)) {
					ActivePackageSource = packageSourceViewModel;
					return;
				}
			}
			
			SelectFirstActivePackageSource();
		}
		
		void SelectFirstActivePackageSource()
		{
			if (packageSources.Count > 0) {
				ActivePackageSource = packageSources[0];
			}
		}
		
		void ReceiveNotificationsWhenPackageSourcesUpdated()
		{
			registeredPackageSources.CollectionChanged += PackageSourcesChanged;
		}

		void PackageSourcesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdatePackageSourceViewModels();
		}
		
		void UpdateDefaultProject()
		{
			DefaultProject = this.Projects.FirstOrDefault();
		}
		
		void ReceiveNotificationsWhenSolutionIsUpdated()
		{
			projectService.AllProjects.CollectionChanged += OnProjectCollectionChanged;
		}
		
		void OnProjectCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateDefaultProject();
		}
		
		public ObservableCollection<PackageSourceViewModel> PackageSources {
			get { return packageSources; }
		}
		
		public PackageSourceViewModel ActivePackageSource {
			get { return activePackageSource; }
			set {
				activePackageSource = value;
				consoleHost.ActivePackageSource = GetPackageSourceForViewModel(activePackageSource);
				OnPropertyChanged(viewModel => viewModel.ActivePackageSource);
			}
		}
		
		PackageSource GetPackageSourceForViewModel(PackageSourceViewModel activePackageSource)
		{
			if (activePackageSource != null) {
				return activePackageSource.GetPackageSource();
			}
			return null;
		}
		
		public IModelCollection<IProject> Projects {
			get { return projectService.AllProjects; }
		}
		
		public IProject DefaultProject {
			get { return consoleHost.DefaultProject; }
			set {
				consoleHost.DefaultProject = value;
				OnPropertyChanged(viewModel => viewModel.DefaultProject);
			}
		}
		
		public TextEditor TextEditor {
			get { return packageManagementConsole.TextEditor; }
		}
		
		public bool ShutdownConsole()
		{
			consoleHost.ShutdownConsole();
			consoleHost.Dispose();
			return !consoleHost.IsRunning;
		}
	}
}
