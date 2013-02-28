// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Project
{
	class Solution : SolutionFolder, ISolution
	{
		#warning reimplement ProjectChangeWatcher for solution file
		FileName fileName;
		DirectoryName directory;
		
		public Solution(FileName fileName)
		{
			this.FileName = fileName;
			this.MSBuildProjectCollection = new Microsoft.Build.Evaluation.ProjectCollection();
			this.ConfigurationNames = new SolutionConfigurationOrPlatformNameCollection(this, false);
			this.PlatformNames = new SolutionConfigurationOrPlatformNameCollection(this, true);
		}
		
		public void Dispose()
		{
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
					FileNameChanged(this, EventArgs.Empty);
				}
			}
		}
		
		public DirectoryName Directory {
			get { return directory; }
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
					StartupProjectChanged(this, EventArgs.Empty);
				}
			}
		}
		
		IProject AutoDetectStartupProject()
		{
			return projects.FirstOrDefault(p => p.IsStartable);
		}
		#endregion
		
		#region Project list
		SimpleModelCollection<IProject> projects = new SimpleModelCollection<IProject>();
		// TODO: thread-safety?
		
		public IModelCollection<IProject> Projects {
			get { return projects; }
		}
		
		void OnProjectAdded(IProject project)
		{
			projects.Add(project);
			if (startupProject == null && AutoDetectStartupProject() == project)
				this.StartupProject = project; // when there's no startable project in the solution and one is added, we mark that it as the startup project
		}
		
		void OnProjectRemoved(IProject project)
		{
			bool wasStartupProject = (startupProject == project);
			if (wasStartupProject)
				startupProject = null; // this will force auto-detection on the next property access
			projects.Remove(project);
			if (wasStartupProject)
				StartupProjectChanged(this, EventArgs.Empty);
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
				OnProjectRemoved((IProject)newItem);
			}
		}
		#endregion
		
		public IEnumerable<ISolutionItem> AllItems {
			get {
				return this.Items.Flatten(i => i is ISolutionFolder ? ((ISolutionFolder)i).Items : null);
			}
		}
		
		List<SolutionSection> globalSections = new List<SolutionSection>();
		
		public IList<SolutionSection> GlobalSections {
			get { return globalSections; }
		}
		
		public ISolutionItem GetItemByGuid(Guid guid)
		{
			return this.AllItems.FirstOrDefault(i => i.IdGuid == guid);
		}
		
		#region Preferences
		Properties preferences;
		
		public Properties Preferences {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void LoadPreferences()
		{
			
		}
		
		public void SavePreferences()
		{
			throw new NotImplementedException();
		}
		#endregion
		
		#region Save
		public void Save()
		{
			foreach (var project in this.Projects) {
				project.Save();
			}
			try {
				//changeWatcher.Disable();
				using (var solutionWriter = new SolutionWriter(fileName)) {
					solutionWriter.WriteFormatHeader(ComputeSolutionVersion());
					solutionWriter.WriteSolutionItems(this);
					solutionWriter.WriteGlobalSections(this);
				}
				//changeWatcher.Enable();
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
		
		public Microsoft.Build.Evaluation.ProjectCollection MSBuildProjectCollection { get; private set; }
		
		public bool IsReadOnly {
			get {
				throw new NotImplementedException();
			}
		}
		
		#region IConfigurable implementation
		ConfigurationAndPlatform activeConfiguration = new ConfigurationAndPlatform("Debug", "AnyCPU");
		
		public ConfigurationAndPlatform ActiveConfiguration {
			get { return activeConfiguration; }
			set {
				SD.MainThread.VerifyAccess();
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
		
		internal static void CreateDefaultConfigurationsIfMissing(IConfigurable solution)
		{
			if (solution.ConfigurationNames.Count == 0) {
				solution.ConfigurationNames.Add("Debug");
				solution.ConfigurationNames.Add("Release");
			}
			if (solution.PlatformNames.Count == 0) {
				solution.PlatformNames.Add("Any CPU");
			}
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
	}
}
