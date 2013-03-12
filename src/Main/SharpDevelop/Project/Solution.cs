// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Project
{
	class Solution : SolutionFolder, ISolution
	{
		FileName fileName;
		DirectoryName directory;
		readonly IProjectChangeWatcher changeWatcher;
		readonly IFileService fileService;
		
		public Solution(FileName fileName, IProjectChangeWatcher changeWatcher, IFileService fileService)
		{
			this.changeWatcher = changeWatcher;
			this.fileService = fileService;
			this.ConfigurationNames = new SolutionConfigurationOrPlatformNameCollection(this, false);
			this.PlatformNames = new SolutionConfigurationOrPlatformNameCollection(this, true);
			this.FileName = fileName;
			base.Name = fileName.GetFileNameWithoutExtension();
			
			this.globalSections = new SynchronizedModelCollection<SolutionSection>(new NullSafeSimpleModelCollection<SolutionSection>());
			globalSections.CollectionChanged += OnSolutionSectionCollectionChanged;
			
			fileService.FileRenamed += FileServiceFileRenamed;
			fileService.FileRemoved += FileServiceFileRemoved;
		}
		
		public void Dispose()
		{
			fileService.FileRenamed -= FileServiceFileRenamed;
			fileService.FileRemoved -= FileServiceFileRemoved;
			
			changeWatcher.Dispose();
			foreach (var project in this.Projects) {
				project.Dispose();
			}
		}
		
		#region FileName
		public event EventHandler FileNameChanged = delegate { };
		
		public FileName FileName {
			get { return fileName; }
			private set {
				if (fileName != value) {
					this.fileName = value;
					this.directory = value.GetParentDirectory();
					UpdateMSBuildProperties();
					FileNameChanged(this, EventArgs.Empty);
				}
			}
		}
		
		public DirectoryName Directory {
			get { return directory; }
		}
		
		public override string Name {
			get {
				return base.Name;
			}
			set {
				throw new NotImplementedException();
			}
		}
		#endregion
		
		#region StartupProject
		public event EventHandler StartupProjectChanged = delegate { };
		
		IProject startupProject;
		
		public IProject StartupProject {
			get {
				if (startupProject == null) {
					startupProject = AutoDetectStartupProject();
				}
				return startupProject;
			}
			set {
				if (value == null || value.ParentSolution != this)
					throw new ArgumentException();
				if (startupProject != value) {
					startupProject = value;
					preferences.Set("StartupProject", value.IdGuid.ToString());
					StartupProjectChanged(this, EventArgs.Empty);
				}
			}
		}
		
		IProject AutoDetectStartupProject()
		{
			string startupProjectGuidText = preferences.Get("StartupProject", string.Empty);
			Guid startupProjectGuid;
			if (Guid.TryParse(startupProjectGuidText, out startupProjectGuid)) {
				var project = projects.FirstOrDefault(p => p.IdGuid == startupProjectGuid);
				if (project != null)
					return project;
			}
			return projects.FirstOrDefault(p => p.IsStartable);
		}
		#endregion
		
		#region Project list
		IMutableModelCollection<IProject> projects = new SynchronizedModelCollection<IProject>(new SimpleModelCollection<IProject>());
		
		public IModelCollection<IProject> Projects {
			get { return projects; }
		}
		
		void OnProjectAdded(IProject project)
		{
			projects.Add(project);
			project.ProjectSections.CollectionChanged += OnSolutionSectionCollectionChanged;
			OnSolutionSectionCollectionChanged(EmptyList<SolutionSection>.Instance, project.ProjectSections);
			if (startupProject == null && AutoDetectStartupProject() == project)
				this.StartupProject = project; // when there's no startable project in the solution and one is added, we mark that it as the startup project
		}
		
		void OnProjectRemoved(IProject project)
		{
			project.ProjectSections.CollectionChanged -= OnSolutionSectionCollectionChanged;
			OnSolutionSectionCollectionChanged(project.ProjectSections, EmptyList<SolutionSection>.Instance);
			bool wasStartupProject = (startupProject == project);
			if (wasStartupProject)
				startupProject = null; // this will force auto-detection on the next property access
			projects.Remove(project);
			if (wasStartupProject)
				StartupProjectChanged(this, EventArgs.Empty);
			
			// HACK: ensure the project gets disposed
			SD.MainThread.InvokeAsyncAndForget(
				delegate {
					project.Dispose();
				}, DispatcherPriority.Background);
		}
		
		internal IDisposable ReportBatch()
		{
			return projects.BatchUpdate();
		}
		
		internal void ReportRemovedItem(ISolutionItem oldItem)
		{
			if (oldItem is ISolutionFolder) {
				// recurse into removed folders
				foreach (var childItem in ((ISolutionFolder)oldItem).Items) {
					ReportRemovedItem(childItem);
				}
			} else if (oldItem is IProject) {
				OnProjectRemoved((IProject)oldItem);
			}
		}
		
		internal void ReportAddedItem(ISolutionItem newItem)
		{
			if (newItem is ISolutionFolder) {
				// recurse into added folders
				foreach (var childItem in ((ISolutionFolder)newItem).Items) {
					ReportAddedItem(childItem);
				}
			} else if (newItem is IProject) {
				OnProjectAdded((IProject)newItem);
			}
		}
		#endregion
		
		public IEnumerable<ISolutionItem> AllItems {
			get {
				return this.Items.Flatten(i => i is ISolutionFolder ? ((ISolutionFolder)i).Items : null);
			}
		}
		
		readonly IMutableModelCollection<SolutionSection> globalSections;
		
		public IMutableModelCollection<SolutionSection> GlobalSections {
			get { return globalSections; }
		}
		
		void OnSolutionSectionCollectionChanged(IReadOnlyCollection<SolutionSection> oldItems, IReadOnlyCollection<SolutionSection> newItems)
		{
			this.IsDirty = true;
			foreach (var section in oldItems) {
				section.Changed -= OnSolutionSectionChanged;
			}
			foreach (var section in newItems) {
				section.Changed += OnSolutionSectionChanged;
			}
		}
		
		void OnSolutionSectionChanged(object sender, EventArgs e)
		{
			this.IsDirty = true;
		}
		
		public ISolutionItem GetItemByGuid(Guid guid)
		{
			// Maybe we should maintain a dictionary to make these lookups faster?
			// But I don't think lookups by GUID are commonly used...
			return this.AllItems.FirstOrDefault(i => i.IdGuid == guid);
		}
		
		#region Preferences
		Properties preferences;
		
		public Properties Preferences {
			get { return preferences; }
		}
		
		internal void LoadPreferences()
		{
			try {
				preferences = new Properties();
			} catch (IOException) {
				
			}
			// Load active configuration from preferences
			CreateDefaultConfigurationsIfMissing();
			this.ActiveConfiguration = ConfigurationAndPlatform.FromKey(preferences.Get("ActiveConfiguration", "Debug|Any CPU"));
			ValidateConfiguration();
			// We can't set the startup project property yet; LoadPreferences() is called before
			// the projects are loaded into the solution.
			// This is necessary so that the projects can be loaded in the correct configuration
			// - we avoid an expensive configuration switch during solution load.
		}
		
		/*
		
		static Properties LoadSolutionPreferences(string solutionFileName)
		{
			try {
				string file = GetPreferenceFileName(solutionFileName);
				if (FileUtility.IsValidPath(file) && File.Exists(file)) {
					try {
						return Properties.Load(file);
					} catch (IOException) {
					} catch (UnauthorizedAccessException) {
					} catch (XmlException) {
						// ignore errors about inaccessible or malformed files
					}
				}
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
			return new Properties();
		}
		
		static void ApplyConfigurationAndReadProjectPreferences()
		{
			openSolution.ApplySolutionConfigurationAndPlatformToProjects();
			foreach (IProject project in openSolution.Projects) {
				string file = GetPreferenceFileName(project.FileName);
				if (FileUtility.IsValidPath(file) && File.Exists(file)) {
					Properties properties = null;
					try {
						properties = Properties.Load(file);
					} catch (IOException) {
					} catch (UnauthorizedAccessException) {
					} catch (XmlException) {
						// ignore errors about inaccessible or malformed files
					}
					if (properties != null)
						project.SetMemento(properties);
				}
			}
		}
		 */
		
		
		public void SavePreferences()
		{
			preferences.Set("ActiveConfiguration.Configuration", activeConfiguration.Configuration);
			preferences.Set("ActiveConfiguration.Platform", activeConfiguration.Platform);
			// TODO: save to disk
			/*
			string directory = Path.Combine(PropertyService.ConfigDirectory, "preferences");
			if (!Directory.Exists(directory)) {
				Directory.CreateDirectory(directory);
			}
			
			if (SolutionPreferencesSaving != null)
				SolutionPreferencesSaving(null, new SolutionEventArgs(openSolution));
			Properties memento = openSolution.Preferences.Clone();
			
			string fullFileName = GetPreferenceFileName(openSolution.FileName);
			if (FileUtility.IsValidPath(fullFileName)) {
				#if DEBUG
				memento.Save(fullFileName);
				#else
				FileUtility.ObservedSave(new NamedFileOperationDelegate(memento.Save), fullFileName, FileErrorPolicy.Inform);
				#endif
			}
			
			foreach (IProject project in OpenSolution.Projects) {
				memento = project.CreateMemento();
				if (memento == null) continue;
				
				fullFileName = GetPreferenceFileName(project.FileName);
				if (FileUtility.IsValidPath(fullFileName)) {
					#if DEBUG
					memento.Save(fullFileName);
					#else
					FileUtility.ObservedSave(new NamedFileOperationDelegate(memento.Save), fullFileName, FileErrorPolicy.Inform);
					#endif
				}
			}*/
		}
		#endregion
		
		#region Save
		public void Save()
		{
			try {
				changeWatcher.Disable();
				using (var solutionWriter = new SolutionWriter(fileName)) {
					solutionWriter.WriteFormatHeader(ComputeSolutionVersion());
					solutionWriter.WriteSolutionItems(this);
					solutionWriter.WriteGlobalSections(this);
				}
				changeWatcher.Enable();
			} catch (IOException ex) {
				MessageService.ShowErrorFormatted("${res:SharpDevelop.Solution.CannotSave.IOException}", fileName, ex.Message);
			} catch (UnauthorizedAccessException ex) {
				FileAttributes attributes = File.GetAttributes(fileName);
				if ((FileAttributes.ReadOnly & attributes) == FileAttributes.ReadOnly) {
					MessageService.ShowErrorFormatted("${res:SharpDevelop.Solution.CannotSave.ReadOnly}", fileName);
				}
				else
				{
					MessageService.ShowErrorFormatted
						("${res:SharpDevelop.Solution.CannotSave.UnauthorizedAccessException}", fileName, ex.Message);
				}
			}
		}
		
		SolutionFormatVersion ComputeSolutionVersion()
		{
			SolutionFormatVersion version = SolutionFormatVersion.VS2005;
			foreach (var project in this.Projects) {
				if (project.MinimumSolutionVersion > version)
					version = project.MinimumSolutionVersion;
			}
			return version;
		}
		#endregion
		
		#region MSBuildProjectCollection
		readonly Microsoft.Build.Evaluation.ProjectCollection msBuildProjectCollection = new Microsoft.Build.Evaluation.ProjectCollection();
		
		public Microsoft.Build.Evaluation.ProjectCollection MSBuildProjectCollection {
			get { return msBuildProjectCollection; }
		}
		
		void UpdateMSBuildProperties()
		{
			var dict = new Dictionary<string, string>();
			MSBuildInternals.AddMSBuildSolutionProperties(this, dict);
			foreach (var pair in dict) {
				msBuildProjectCollection.SetGlobalProperty(pair.Key, pair.Value);
			}
		}
		#endregion
		
		#region Handle FileService.FileRenamed / FileRemoved
		void FileServiceFileRenamed(object sender, FileRenameEventArgs e)
		{
			string oldName = e.SourceFile;
			string newName = e.TargetFile;
			foreach (ISolutionFileItem fileItem in this.AllItems.OfType<ISolutionFileItem>()) {
				if (FileUtility.IsBaseDirectory(oldName, fileItem.FileName)) {
					string newFullName = FileUtility.RenameBaseDirectory(fileItem.FileName, oldName, newName);
					fileItem.FileName = FileName.Create(newFullName);
				}
			}
			
			foreach (IProject project in this.Projects) {
				if (FileUtility.IsBaseDirectory(project.Directory, oldName)) {
					foreach (ProjectItem item in project.Items) {
						if (FileUtility.IsBaseDirectory(oldName, item.FileName)) {
							ProjectService.OnProjectItemRemoved(new ProjectItemEventArgs(project, item));
							item.FileName = FileUtility.RenameBaseDirectory(item.FileName, oldName, newName);
							ProjectService.OnProjectItemAdded(new ProjectItemEventArgs(project, item));
						}
					}
				}
			}
		}
		
		void FileServiceFileRemoved(object sender, FileEventArgs e)
		{
			string fileName = e.FileName;
			
			foreach (ISolutionFileItem fileItem in this.AllItems.OfType<ISolutionFileItem>().ToArray()) {
				if (FileUtility.IsBaseDirectory(fileName, fileItem.FileName)) {
					fileItem.ParentFolder.Items.Remove(fileItem);
				}
			}
			
			foreach (IProject project in this.Projects) {
				if (FileUtility.IsBaseDirectory(project.Directory, fileName)) {
					IProjectItemListProvider provider = project as IProjectItemListProvider;
					if (provider != null) {
						foreach (ProjectItem item in provider.Items.ToArray()) {
							if (FileUtility.IsBaseDirectory(fileName, item.FileName)) {
								provider.RemoveProjectItem(item);
								ProjectService.OnProjectItemRemoved(new ProjectItemEventArgs(project, item));
							}
						}
					}
				}
			}
		}
		#endregion
		
		public bool IsReadOnly {
			get {
				try {
					FileAttributes attributes = File.GetAttributes(fileName);
					return ((FileAttributes.ReadOnly & attributes) == FileAttributes.ReadOnly);
				} catch (FileNotFoundException) {
					return false;
				} catch (DirectoryNotFoundException) {
					return true;
				}
			}
		}
		
		#region IConfigurable implementation
		ConfigurationAndPlatform activeConfiguration = new ConfigurationAndPlatform("Debug", "AnyCPU");
		
		public ConfigurationAndPlatform ActiveConfiguration {
			get { return activeConfiguration; }
			set {
				if (value.Configuration == null || value.Platform == null)
					throw new ArgumentNullException();
				
				if (activeConfiguration != value) {
					activeConfiguration = value;
					
					// Apply changed configuration to all projects:
					foreach (var project in this.Projects) {
						project.ActiveConfiguration = project.ConfigurationMapping.GetProjectConfiguration(value);
					}
					
					ActiveConfigurationChanged(this, EventArgs.Empty);
				}
			}
		}
		
		public event EventHandler ActiveConfigurationChanged = delegate { };
		
		public IConfigurationOrPlatformNameCollection ConfigurationNames { get; private set; }
		public IConfigurationOrPlatformNameCollection PlatformNames { get; private set; }
		
		void CreateDefaultConfigurationsIfMissing()
		{
			if (this.ConfigurationNames.Count == 0) {
				this.ConfigurationNames.Add("Debug");
				this.ConfigurationNames.Add("Release");
			}
			if (this.PlatformNames.Count == 0) {
				this.PlatformNames.Add("Any CPU");
			}
		}
		
		void ValidateConfiguration()
		{
			string config = this.ActiveConfiguration.Configuration;
			string platform = this.ActiveConfiguration.Platform;
			if (!this.ConfigurationNames.Contains(config))
				config = this.ConfigurationNames.First();
			if (!this.PlatformNames.Contains(platform))
				platform = this.PlatformNames.First();
			this.ActiveConfiguration = new ConfigurationAndPlatform(config, platform);
		}
		#endregion
		
		#region ICanBeDirty implementation
		public event EventHandler IsDirtyChanged;
		
		bool isDirty;
		
		public bool IsDirty {
			get { return isDirty; }
			set {
				if (isDirty != value) {
					isDirty = value;
					
					if (IsDirtyChanged != null) {
						IsDirtyChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		#endregion
		
		public override string ToString()
		{
			return "[Solution " + fileName + " with " + projects.Count + " projects]";
		}
	}
}
