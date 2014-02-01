// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Default implementation of the IProject interface.
	/// </summary>
	public abstract class AbstractProject : LocalizedObject, IProject
	{
		// Member documentation: see IProject members.
		
		readonly ISolution parentSolution;
		readonly ConfigurationMapping configurationMapping;
		
		protected AbstractProject(ProjectInformation information)
		{
			if (information == null)
				throw new ArgumentNullException("information");
			this.parentSolution = information.Solution;
			this.activeConfiguration = information.ActiveProjectConfiguration;
			this.configurationMapping = information.ConfigurationMapping ?? new ConfigurationMapping();
			this.Name = information.ProjectName;
			this.FileName = information.FileName;
			this.idGuid = (information.IdGuid != Guid.Empty ? information.IdGuid : Guid.NewGuid());
			this.TypeGuid = information.TypeGuid;
			if (information.ProjectSections != null)
				this.projectSections.AddRange(information.ProjectSections);
		}
		
		#region IDisposable implementation
		bool isDisposed;
		
		[Browsable(false)]
		public bool IsDisposed {
			get { return isDisposed; }
		}
		
		public event EventHandler Disposed;
		
		public virtual void Dispose()
		{
			SD.MainThread.VerifyAccess();
			lock (SyncRoot) {
				if (isDisposed)
					return;
				isDisposed = true;
				if (watcher != null)
					watcher.Dispose();
			}
			if (Disposed != null)
				Disposed(this, EventArgs.Empty);
		}
		#endregion
		
		#region Preferences
		Properties preferences;
		
		public Properties Preferences {
			get {
				lock (syncRoot) {
					if (preferences == null) {
						preferences = new Properties(); // in case of errors, use empty properties container
						FileName preferencesFile = GetPreferenceFileName(fileName);
						if (FileUtility.IsValidPath(preferencesFile) && File.Exists(preferencesFile)) {
							try {
								preferences = Properties.Load(preferencesFile);
							} catch (IOException) {
							} catch (UnauthorizedAccessException) {
							} catch (XmlException) {
								// ignore errors about inaccessible or malformed files
							}
						}
					}
					return preferences;
				}
			}
		}
		
		static FileName GetPreferenceFileName(string projectFileName)
		{
			string directory = Path.Combine(PropertyService.ConfigDirectory, "preferences");
			return FileName.Create(Path.Combine(directory,
			                                    Path.GetFileName(projectFileName)
			                                    + "." + projectFileName.ToUpperInvariant().GetStableHashCode().ToString("x")
			                                    + ".xml"));
		}
		
		public void SavePreferences()
		{
			var p = this.Preferences;
			GetOrCreateBehavior().SavePreferences(p);
			try {
				FileName preferencesFile = GetPreferenceFileName(fileName);
				System.IO.Directory.CreateDirectory(preferencesFile.GetParentDirectory());
				p.Save(preferencesFile);
			} catch (IOException) {
			} catch (UnauthorizedAccessException) {
			}
		}
		#endregion
		
		#region Filename / Directory
		volatile FileName fileName;
		volatile DirectoryName directoryName;
		protected IProjectChangeWatcher watcher;
		
		/// <summary>
		/// Gets the name of the project file.
		/// (Full file name, example: @"D:\Serralongue\SharpDevelop\samples\CustomPad\CustomPad.csproj")
		/// 
		/// Only the getter is thread-safe.
		/// </summary>
		[ReadOnly(true)]
		public FileName FileName {
			get {
				return fileName;
			}
			set {
				if (value == null)
					throw new ArgumentNullException();
				SD.MainThread.VerifyAccess();
				Debug.Assert(FileUtility.IsUrl(value) || Path.IsPathRooted(value));
				
				lock (SyncRoot) {
					if (watcher == null) {
						if (SD.Services.GetService(typeof(IWorkbench)) == null) {
							watcher = new MockProjectChangeWatcher();
						} else {
							watcher = new ProjectChangeWatcher(value);
							watcher.Enable();
						}
					} else {
						watcher.Disable();
						watcher.Rename(value);
						watcher.Enable();
					}
					
					fileName = value;
					directoryName = value.GetParentDirectory();
				}
			}
		}
		
		/// <summary>
		/// True if the file that contains the project is readonly.
		/// </summary>
		[ReadOnly(true)]
		public virtual bool IsReadOnly {
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
		public DirectoryName Directory {
			get { return directoryName; }
		}
		#endregion
		
		#region ProjectSections
		SimpleModelCollection<SolutionSection> projectSections = new NullSafeSimpleModelCollection<SolutionSection>();
		
		[Browsable(false)]
		public IMutableModelCollection<SolutionSection> ProjectSections {
			get {
				SD.MainThread.VerifyAccess();
				return projectSections;
			}
		}
		#endregion
		
		#region Configuration / Platform management
		ConfigurationAndPlatform activeConfiguration = new ConfigurationAndPlatform("Debug", "AnyCPU");
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:Dialog.Options.CombineOptions.Configurations.ConfigurationColumnHeader}")]
		public ConfigurationAndPlatform ActiveConfiguration {
			get { return activeConfiguration; }
			set {
				SD.MainThread.VerifyAccess();
				if (value.Configuration == null || value.Platform == null)
					throw new ArgumentNullException();
				
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
		
		sealed class ReadOnlyConfigurationOrPlatformNameCollection : ImmutableModelCollection<string>, IConfigurationOrPlatformNameCollection
		{
			public ReadOnlyConfigurationOrPlatformNameCollection(IEnumerable<string> items)
				: base(items)
			{
			}
			
			public string ValidateName(string name)
			{
				return Contains(name) ? name : null;
			}
			
			public void Add(string newName, string copyFrom)
			{
				throw new NotSupportedException();
			}
			
			public void Remove(string name)
			{
				throw new NotSupportedException();
			}
			
			public void Rename(string oldName, string newName)
			{
				throw new NotSupportedException();
			}
		}
		
		public virtual IConfigurationOrPlatformNameCollection ConfigurationNames {
			get {
				return new ReadOnlyConfigurationOrPlatformNameCollection(new[] { "Debug", "Release" });
			}
		}
		
		public virtual IConfigurationOrPlatformNameCollection PlatformNames {
			get {
				return new ReadOnlyConfigurationOrPlatformNameCollection(new[] { "AnyCPU" });
			}
		}
		
		public ConfigurationMapping ConfigurationMapping {
			get { return configurationMapping; }
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
		public virtual IReadOnlyCollection<ItemType> AvailableFileItemTypes {
			get {
				return ItemType.DefaultFileItems;
			}
		}
		
		[Browsable(false)]
		public virtual IMutableModelCollection<ProjectItem> Items {
			get {
				return new ImmutableModelCollectionImplementsMutableInterface<ProjectItem>(Enumerable.Empty<ProjectItem>());
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
		
		[Browsable(false)]
		public virtual ILanguageBinding LanguageBinding {
			get {
				return DefaultLanguageBinding.DefaultInstance;
			}
		}
		
		/// <summary>
		/// Gets the full path of the output assembly.
		/// Returns null when the project does not output any assembly.
		/// </summary>
		[Browsable(false)]
		public virtual FileName OutputAssemblyFullPath {
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
		public bool IsFileInProject(FileName fileName)
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
		public FileProjectItem FindFile(FileName fileName)
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
				FileProjectItem outputItem;
				findFileCache.TryGetValue(fileName, out outputItem);
				return outputItem;
			}
		}
		
		public virtual IProjectContent ProjectContent {
			get {
				return null;
			}
		}
		
		public virtual event EventHandler<ParseInformationEventArgs> ParseInformationUpdated {
			add {}
			remove {}
		}
		
		public virtual void OnParseInformationUpdated(ParseInformationEventArgs args)
		{
			throw new NotSupportedException();
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
		public virtual SolutionFormatVersion MinimumSolutionVersion {
			get { return SolutionFormatVersion.VS2005; }
		}
		
		/// <summary>
		/// Resolves assembly references for this project.
		/// The resulting list of resolved references will include project references.
		/// </summary>
		public virtual IEnumerable<ReferenceProjectItem> ResolveAssemblyReferences(CancellationToken cancellationToken)
		{
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
				referenceItems.Add(new ReferenceProjectItem(this, "mscorlib") { FileName = FileName.Create(typeof(object).Module.FullyQualifiedName) });
			}
			return referenceItems;
		}
		
		public virtual Task<bool> BuildAsync(ProjectBuildOptions options, IBuildFeedbackSink feedbackSink, IProgressMonitor progressMonitor)
		{
			feedbackSink.ReportError(new BuildError { ErrorText = "Building project " + Name + " is not supported.", IsWarning = true });
			// we don't know how to build anything, report that we're done.
			return Task.FromResult(true);
		}
		
		public virtual IEnumerable<IBuildable> GetBuildDependencies(ProjectBuildOptions buildOptions)
		{
			lock (SyncRoot) {
				List<IBuildable> result = new List<IBuildable>();
				foreach (SolutionSection section in this.ProjectSections) {
					if (section.SectionName == "ProjectDependencies") {
						foreach (var entry in section) {
							Guid guid;
							if (Guid.TryParse(entry.Key, out guid)) {
								foreach (IProject p in ParentSolution.Projects) {
									if (p.IdGuid == guid) {
										result.Add(p);
									}
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
			string solutionConfiguration = options.SolutionConfiguration ?? ParentSolution.ActiveConfiguration.Configuration;
			string solutionPlatform = options.SolutionPlatform ?? ParentSolution.ActiveConfiguration.Platform;
			
			// start of default implementation
			var projectConfig = this.ConfigurationMapping.GetProjectConfiguration(new ConfigurationAndPlatform(solutionConfiguration, solutionPlatform));
			
			ProjectBuildOptions projectOptions = new ProjectBuildOptions(isRootBuildable ? options.ProjectTarget : options.TargetForDependencies);
			projectOptions.BuildOutputVerbosity = options.BuildOutputVerbosity;
			projectOptions.Configuration = projectConfig.Configuration;
			projectOptions.Platform = projectConfig.Platform;
			
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
		
		public virtual void ProjectLoaded()
		{
			GetOrCreateBehavior().ProjectLoaded();
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
		
		public virtual string GetDefaultNamespace(string fileName)
		{
			string relPath = FileUtility.GetRelativePath(this.Directory, Path.GetDirectoryName(fileName));
			string[] subdirs = relPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			StringBuilder standardNameSpace = new StringBuilder(this.RootNamespace);
			foreach(string subdir in subdirs) {
				if (subdir == "." || subdir == ".." || subdir.Length == 0)
					continue;
				if (subdir.Equals("src", StringComparison.OrdinalIgnoreCase))
					continue;
				if (subdir.Equals("source", StringComparison.OrdinalIgnoreCase))
					continue;
				if (standardNameSpace.Length > 0)
					standardNameSpace.Append('.');
				standardNameSpace.Append(NewFileDialog.GenerateValidClassOrNamespaceName(subdir, true));
			}
			return standardNameSpace.ToString();
		}
		
		public virtual System.CodeDom.Compiler.CodeDomProvider CreateCodeDomProvider()
		{
			return null;
		}
		
		public virtual void GenerateCodeFromCodeDom(System.CodeDom.CodeCompileUnit compileUnit, TextWriter writer)
		{
			var provider = this.CreateCodeDomProvider();
			if (provider != null) {
				var options = new System.CodeDom.Compiler.CodeGeneratorOptions();
				options.BlankLinesBetweenMembers = AmbienceService.CodeGenerationProperties.Get("BlankLinesBetweenMembers", true);
				options.BracingStyle             = AmbienceService.CodeGenerationProperties.Get("StartBlockOnSameLine", true) ? "Block" : "C";
				options.ElseOnClosing            = AmbienceService.CodeGenerationProperties.Get("ElseOnClosing", true);
				options.IndentString = SD.EditorControlService.GlobalOptions.IndentationString;
				provider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
			} else {
				writer.WriteLine("No CodeDom provider was found for this language.");
			}
		}
		
		public virtual IAmbience GetAmbience()
		{
			return new DefaultAmbience();
		}
		
		public virtual Refactoring.ISymbolSearch PrepareSymbolSearch(ISymbol entity)
		{
			return GetOrCreateBehavior().PrepareSymbolSearch(entity);
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
			return projectTypeGuid == this.TypeGuid;
		}
		
		[Browsable(false)]
		public virtual IAssemblyModel AssemblyModel {
			get {
				return EmptyAssemblyModel.Instance;
			}
		}
		
		[Browsable(false)]
		public Guid TypeGuid { get; private set; }
		
		Guid idGuid;
		
		[Browsable(false)]
		public virtual Guid IdGuid {
			get {
				return idGuid;
			}
			set {
				idGuid = value;
			}
		}
		
		[Browsable(false)]
		public string Name { get; set; }
		
		[Browsable(false)]
		public ISolutionFolder ParentFolder { get; set; }
		
		[Browsable(false)]
		public ISolution ParentSolution {
			get { return parentSolution; }
		}
		
		readonly object syncRoot = new object();
		
		public object SyncRoot {
			get { return syncRoot; }
		}
	}
}
