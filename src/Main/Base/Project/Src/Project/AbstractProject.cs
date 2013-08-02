// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Default implementation of the IProject interface.
	/// </summary>
	public abstract class AbstractProject : AbstractSolutionFolder, IProject
	{
		// Member documentation: see IProject members.
		
		#region static methods - DO NOT BELONG HERE; PLEASE MOVE
		public static string GetConfigurationNameFromKey(string key)
		{
			int pos = key.IndexOf('|');
			if (pos < 0)
				return key;
			else
				return key.Substring(0, pos);
		}
		
		public static string GetPlatformNameFromKey(string key)
		{
			return key.Substring(key.IndexOf('|') + 1);
		}
		#endregion
		
		#region IDisposable implementation
		bool isDisposed;
		
		[Browsable(false)]
		public bool IsDisposed {
			get { return isDisposed; }
		}
		
		public event EventHandler Disposed;
		
		public virtual void Dispose()
		{
			WorkbenchSingleton.AssertMainThread();
			if (watcher != null)
				watcher.Dispose();
			isDisposed = true;
			if (Disposed != null) {
				Disposed(this, EventArgs.Empty);
			}
		}
		#endregion
		
		#region IMementoCapable implementation
		internal static List<string> filesToOpenAfterSolutionLoad = new List<string>();
		
		/// <summary>
		/// Saves project preferences (currently opened files, bookmarks etc.) to the
		/// a property container.
		/// </summary>
		public virtual Properties CreateMemento()
		{
			return GetOrCreateBehavior().CreateMemento();
		}
		
		public virtual void SetMemento(Properties memento)
		{
			GetOrCreateBehavior().SetMemento(memento);
		}
		#endregion
		
		#region Filename / Directory
		volatile string fileName;
		string cachedDirectoryName;
		protected IProjectChangeWatcher watcher;
		
		/// <summary>
		/// Gets the name of the project file.
		/// (Full file name, example: @"D:\Serralongue\SharpDevelop\samples\CustomPad\CustomPad.csproj")
		/// 
		/// Only the getter is thread-safe.
		/// </summary>
		[ReadOnly(true)]
		public string FileName {
			get {
				return fileName ?? "";
			}
			set {
				if (value == null)
					throw new ArgumentNullException();
				WorkbenchSingleton.AssertMainThread();
				Debug.Assert(FileUtility.IsUrl(value) || Path.IsPathRooted(value));
				
				if (WorkbenchSingleton.Workbench == null)
					watcher = new MockProjectChangeWatcher();
				
				if (watcher == null) {
					watcher = new ProjectChangeWatcher(value);
					watcher.Enable();
				} else {
					watcher.Disable();
					watcher.Rename(value);
					watcher.Enable();
				}
				
				lock (SyncRoot) { // locking still required for Directory
					fileName = value;
					cachedDirectoryName = null;
				}
			}
		}
		
		/// <summary>
		/// True if the file that contains the project is readonly.
		/// </summary>
		[ReadOnly(true)]
		public virtual bool ReadOnly {
			get {
				try {
					FileAttributes attributes = File.GetAttributes(FileName);
					return ((FileAttributes.ReadOnly & attributes) == FileAttributes.ReadOnly);
				} catch (FileNotFoundException) {
					return false;
				} catch (IOException) {
					// directory not found, network path not available, etc.
					return true;
				}
			}
		}
		
		/// <summary>
		/// Gets the directory of the project file.
		/// This is equivalent to Path.GetDirectoryName(project.FileName);
		/// (Example: @"D:\Serralongue\SharpDevelop\samples\CustomPad")
		/// 
		/// This member is thread-safe.
		/// </summary>
		[Browsable(false)]
		public string Directory {
			get {
				lock (SyncRoot) {
					if (cachedDirectoryName == null) {
						try {
							cachedDirectoryName = Path.GetDirectoryName(this.FileName);
						} catch (Exception) {
							cachedDirectoryName = "";
						}
					}
					return cachedDirectoryName;
				}
			}
		}
		#endregion
		
		#region ProjectSections
		List<ProjectSection> projectSections = new List<ProjectSection>();
		
		[Browsable(false)]
		public List<ProjectSection> ProjectSections {
			get {
				WorkbenchSingleton.AssertMainThread();
				return projectSections;
			}
		}
		#endregion
		
		#region Language Properties / GetAmbience
		[Browsable(false)]
		public virtual ICSharpCode.SharpDevelop.Dom.LanguageProperties LanguageProperties {
			get {
				return ICSharpCode.SharpDevelop.Dom.LanguageProperties.None;
			}
		}
		
		public virtual ICSharpCode.SharpDevelop.Dom.IAmbience GetAmbience()
		{
			return null;
		}
		#endregion
		
		#region Configuration / Platform management
		string activeConfiguration = "Debug";
		string activePlatform = "AnyCPU";
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:Dialog.Options.CombineOptions.Configurations.ConfigurationColumnHeader}")]
		public string ActiveConfiguration {
			get { return activeConfiguration; }
			set {
				WorkbenchSingleton.AssertMainThread();
				
				if (activeConfiguration != value) {
					activeConfiguration = value;
					
					OnActiveConfigurationChanged(EventArgs.Empty);
				}
			}
		}
		
		public event EventHandler ActiveConfigurationChanged;
		
		protected virtual void OnActiveConfigurationChanged(EventArgs e)
		{
			if (ActiveConfigurationChanged != null) {
				ActiveConfigurationChanged(this, e);
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:Dialog.ProjectOptions.Platform}")]
		public string ActivePlatform {
			get { return activePlatform; }
			set {
				WorkbenchSingleton.AssertMainThread();
				
				if (activePlatform != value) {
					activePlatform = value;
					
					OnActivePlatformChanged(EventArgs.Empty);
				}
			}
		}
		
		public event EventHandler ActivePlatformChanged;
		
		protected virtual void OnActivePlatformChanged(EventArgs e)
		{
			if (ActivePlatformChanged != null) {
				ActivePlatformChanged(this, e);
			}
		}
		
		[Browsable(false)]
		public virtual ICollection<string> ConfigurationNames {
			get {
				return new string[] { "Debug", "Release" };
			}
		}
		
		[Browsable(false)]
		public virtual ICollection<string> PlatformNames {
			get {
				return new string[] { "AnyCPU" };
			}
		}
		#endregion
		
		#region Save
		public void Save()
		{
			Save(this.FileName);
		}
		
		public virtual void Save(string fileName)
		{
		}
		#endregion
		
		/// <summary>
		/// Gets the list of available file item types. This member is thread-safe.
		/// </summary>
		[Browsable(false)]
		public virtual ICollection<ItemType> AvailableFileItemTypes {
			get {
				return ItemType.DefaultFileItems;
			}
		}
		
		/// <summary>
		/// Gets the list of items in the project. This member is thread-safe.
		/// The returned collection is guaranteed not to change - adding new items or removing existing items
		/// will create a new collection.
		/// </summary>
		[Browsable(false)]
		public virtual ReadOnlyCollection<ProjectItem> Items {
			get {
				return new ReadOnlyCollection<ProjectItem>(new ProjectItem[0]);
			}
		}
		
		/// <summary>
		/// Gets all items in the project that have the specified item type.
		/// This member is thread-safe.
		/// </summary>
		public virtual IEnumerable<ProjectItem> GetItemsOfType(ItemType itemType)
		{
			foreach (ProjectItem item in this.Items) {
				if (item.ItemType == itemType) {
					yield return item;
				}
			}
		}
		
		[ReadOnly(true)]
		public virtual string AssemblyName {
			get {
				return this.Name;
			}
			set {
			}
		}
		
		[Browsable(false)]
		public virtual string RootNamespace {
			get {
				return this.Name;
			}
			set {
			}
		}
		
		/// <summary>
		/// Gets the full path of the output assembly.
		/// Returns null when the project does not output any assembly.
		/// </summary>
		[Browsable(false)]
		public virtual string OutputAssemblyFullPath {
			get {
				return null;
			}
		}
		
		[Browsable(false)]
		public virtual string AppDesignerFolder {
			get {
				return "";
			}
			set {
				throw new NotSupportedException();
			}
		}
		
		/// <summary>
		/// Gets the name of the language binding used for the project.
		/// </summary>
		[ReadOnly(true)]
		public virtual string Language {
			get {
				return "";
			}
		}
		
		[Browsable(false)]
		public virtual bool IsStartable {
			get {
				return GetOrCreateBehavior().IsStartable;
			}
		}
		
		public virtual void Start(bool withDebugging)
		{
			GetOrCreateBehavior().Start(withDebugging);
		}
		
		/// <summary>
		/// Creates the start info used to start the project.
		/// </summary>
		/// <exception cref="ProjectStartException">Occurs when the project cannot be started.</exception>
		/// <returns>ProcessStartInfo used to start the project.
		/// Note: this can be a ProcessStartInfo with a URL as filename!</returns>
		public virtual ProcessStartInfo CreateStartInfo()
		{
			return GetOrCreateBehavior().CreateStartInfo();
		}
		
		/// <summary>
		/// Returns true, if a specific file is inside this project.
		/// This member is thread-safe.
		/// </summary>
		/// <param name="fileName">The <b>fully qualified</b> file name of the file</param>
		public bool IsFileInProject(string fileName)
		{
			return FindFile(fileName) != null;
		}
		
		Dictionary<string, FileProjectItem> findFileCache;
		
		internal protected void ClearFindFileCache()
		{
			lock (SyncRoot) {
				findFileCache = null;
			}
		}
		
		/// <summary>
		/// Returns the project item for a specific file; or null if the file is not found in the project.
		/// This member is thread-safe.
		/// </summary>
		/// <param name="fileName">The <b>fully qualified</b> file name of the file</param>
		public FileProjectItem FindFile(string fileName)
		{
			lock (SyncRoot) {
				if (findFileCache == null) {
					findFileCache = new Dictionary<string, FileProjectItem>(StringComparer.OrdinalIgnoreCase);
					foreach (ProjectItem item in this.Items) {
						FileProjectItem fileItem = item as FileProjectItem;
						if (fileItem != null) {
							findFileCache[item.FileName] = fileItem;
						}
					}
				}
				fileName = FileUtility.NormalizePath(fileName);
				FileProjectItem outputItem;
				findFileCache.TryGetValue(fileName, out outputItem);
				return outputItem;
			}
		}
		
		ParseProjectContent IProject.CreateProjectContent()
		{
			return this.CreateProjectContent();
		}
		protected virtual ParseProjectContent CreateProjectContent()
		{
			return null;
		}
		
		/// <summary>
		/// Creates a new projectItem for the passed itemType
		/// </summary>
		public virtual ProjectItem CreateProjectItem(IProjectItemBackendStore item)
		{
			return GetOrCreateBehavior().CreateProjectItem(item);
		}
		
		#region Dirty
		bool isDirty;
		
		public event EventHandler DirtyChanged;
		
		[Browsable(false)]
		public bool IsDirty {
			get { return isDirty; }
			set {
				isDirty = value;
				if (DirtyChanged != null) {
					DirtyChanged(this, EventArgs.Empty);
				}
			}
		}
		#endregion
		
		public override string ToString()
		{
			return string.Format("[{0}: {1}]", GetType().Name, this.Name);
		}
		
		/// <summary>
		/// Gets the default item type the specified file should have.
		/// Returns ItemType.None for unknown items.
		/// Every override should call base.GetDefaultItemType for unknown file extensions.
		/// </summary>
		public virtual ItemType GetDefaultItemType(string fileName)
		{
			return GetOrCreateBehavior().GetDefaultItemType(fileName);
		}
		
		[Browsable(false)]
		public virtual int MinimumSolutionVersion {
			get { return Solution.SolutionVersionVS2005; }
		}
		
		public virtual void ResolveAssemblyReferences()
		{
		}
		
		/// <summary>
		/// Resolves assembly references for this project.
		/// The resulting list of resolved references will include project references.
		/// </summary>
		public virtual IEnumerable<ReferenceProjectItem> ResolveAssemblyReferences(CancellationToken cancellationToken)
		{
			ResolveAssemblyReferences();
			List<ReferenceProjectItem> referenceItems = new List<ReferenceProjectItem>();
			bool mscorlib = false;
			foreach (ProjectItem item in this.Items) {
				cancellationToken.ThrowIfCancellationRequested();
				if (ItemType.ReferenceItemTypes.Contains(item.ItemType)) {
					ReferenceProjectItem reference = item as ReferenceProjectItem;
					if (reference != null) {
						referenceItems.Add(reference);
						mscorlib |= "mscorlib".Equals(reference.Include, StringComparison.OrdinalIgnoreCase);
					}
				}
			}
			if (!mscorlib) {
				referenceItems.Add(new ReferenceProjectItem(this, "mscorlib") { FileName = typeof(object).Module.FullyQualifiedName });
			}
			return referenceItems;
		}
		
		public virtual void StartBuild(ProjectBuildOptions options, IBuildFeedbackSink feedbackSink)
		{
			feedbackSink.ReportError(new BuildError { ErrorText = "Building project " + Name + " is not supported.", IsWarning = true });
			// we don't know how to build anything, report that we're done.
			feedbackSink.Done(true);
		}
		
		public virtual ICollection<IBuildable> GetBuildDependencies(ProjectBuildOptions buildOptions)
		{
			lock (SyncRoot) {
				List<IBuildable> result = new List<IBuildable>();
				foreach (ProjectSection section in this.ProjectSections) {
					if (section.Name == "ProjectDependencies") {
						foreach (SolutionItem item in section.Items) {
							foreach (IProject p in ParentSolution.Projects) {
								if (p.IdGuid == item.Name) {
									result.Add(p);
								}
							}
						}
					}
				}
				return result;
			}
		}
		
		public virtual ProjectBuildOptions CreateProjectBuildOptions(BuildOptions options, bool isRootBuildable)
		{
			if (options == null)
				throw new ArgumentNullException("options");
			// start of default implementation
			var configMatchings = this.ParentSolution.GetActiveConfigurationsAndPlatformsForProjects(options.SolutionConfiguration, options.SolutionPlatform);
			ProjectBuildOptions projectOptions = new ProjectBuildOptions(isRootBuildable ? options.ProjectTarget : options.TargetForDependencies);
			projectOptions.BuildOutputVerbosity = options.BuildOutputVerbosity;
			// find the project configuration
			foreach (var matching in configMatchings) {
				if (matching.Project == this) {
					projectOptions.Configuration = matching.Configuration;
					projectOptions.Platform = matching.Platform;
				}
			}
			// fall back to solution config if we don't find any entries for the project
			if (string.IsNullOrEmpty(projectOptions.Configuration))
				projectOptions.Configuration = options.SolutionConfiguration;
			if (string.IsNullOrEmpty(projectOptions.Platform))
				projectOptions.Platform = options.SolutionPlatform;
			
			// copy global properties to project options
			foreach (var pair in options.GlobalAdditionalProperties)
				projectOptions.Properties[pair.Key] = pair.Value;
			if (isRootBuildable) {
				// copy properties for root project to project options
				foreach (var pair in options.ProjectAdditionalProperties) {
					projectOptions.Properties[pair.Key] = pair.Value;
				}
			}
			return projectOptions;
		}
		
		public virtual void ProjectCreationComplete()
		{
			GetOrCreateBehavior().ProjectCreationComplete();
		}
		
		public virtual XElement LoadProjectExtensions(string name)
		{
			return new XElement(name);
		}
		
		public virtual void SaveProjectExtensions(string name, XElement element)
		{
		}
		
		public virtual bool ContainsProjectExtension(string name)
		{
			return false;
		}
		
		Properties projectSpecificProperties;
		
		[Browsable(false)]
		public Properties ProjectSpecificProperties {
			get {
				if (projectSpecificProperties == null) {
					projectSpecificProperties = new Properties();
				}
				return projectSpecificProperties;
			}
			internal set { projectSpecificProperties = value; }
		}
		
		protected virtual ProjectBehavior CreateDefaultBehavior()
		{
			return new DefaultProjectBehavior(this);
		}
		
		protected ProjectBehavior projectBehavior;
		
		protected virtual ProjectBehavior GetOrCreateBehavior()
		{
			lock (SyncRoot) {
				if (projectBehavior == null)
					projectBehavior = ProjectBehaviorService.LoadBehaviorsForProject(this, CreateDefaultBehavior());
				return projectBehavior;
			}
		}
		
		protected virtual void InvalidateBehavior()
		{
			lock (SyncRoot) {
				projectBehavior = null;
			}
		}
		
		public virtual bool HasProjectType(Guid projectTypeGuid)
		{
			Guid myGuid;
			if (Guid.TryParse(this.TypeGuid, out myGuid)) {
				return myGuid == projectTypeGuid;
			} else {
				return false;
			}
		}
	}
}
