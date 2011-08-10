// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using UnitTesting.Tests.Utils;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class MockProject : IProject
	{
		readonly object syncRoot = new object();
		string directory = String.Empty;
		string rootNamespace = String.Empty;
		
		public MockProject()
		{
		}
		
		#region IProject
		public event EventHandler DirtyChanged;
		
		protected virtual void OnDirtyChanged(EventArgs e)
		{
			if (DirtyChanged != null) {
				DirtyChanged(this, e);
			}
		}
		
		public ReadOnlyCollection<ProjectItem> Items {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool ReadOnly {
			get { return false; }
		}
		
		public ICollection<ItemType> AvailableFileItemTypes {
			get {
				throw new NotImplementedException();
			}
		}
		
		public System.Collections.Generic.List<ProjectSection> ProjectSections {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICSharpCode.SharpDevelop.Dom.LanguageProperties LanguageProperties {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICSharpCode.SharpDevelop.Dom.IAmbience GetAmbience()
		{
			throw new NotImplementedException();
		}
		
		public string FileName {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string Directory {
			get { return directory; }
			set { directory = value; }
		}
		
		public string AssemblyName {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string RootNamespace {
			get { return rootNamespace; }
			set { rootNamespace = value; }
		}
		
		public string OutputAssemblyFullPath {
			get {
				throw new NotImplementedException();
			}
		}
		
		string language = String.Empty;
		
		public string Language {
			get {
				return language;
			}
			set {
				language = value;
			}
		}
		
		public string AppDesignerFolder {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string ActiveConfiguration {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string ActivePlatform {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public event EventHandler ActiveConfigurationChanged { add {} remove {} }
		
		public event EventHandler ActivePlatformChanged { add {} remove {} }
		
		public ICollection<string> ConfigurationNames {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICollection<string> PlatformNames {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsStartable {
			get {
				throw new NotImplementedException();
			}
		}
		
		public object SyncRoot {
			get { return syncRoot; }
		}
		
		public ISolutionFolderContainer Parent {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public Solution ParentSolution {
			get { return new Solution(new MockProjectChangeWatcher()); }
		}
		
		public string TypeGuid {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string IdGuid {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string Location {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string Name {
			get { return String.Empty; }
			set {
			}
		}
		
		public bool IsDirty {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public Properties ProjectSpecificProperties {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IEnumerable<ProjectItem> GetItemsOfType(ItemType type)
		{
			throw new NotImplementedException();
		}
		
		public ItemType GetDefaultItemType(string fileName)
		{
			throw new NotImplementedException();
		}
		
		public void Save()
		{
			throw new NotImplementedException();
		}
		
		public bool IsFileInProject(string fileName)
		{
			throw new NotImplementedException();
		}
		
		public FileProjectItem FindFile(string fileName)
		{
			throw new NotImplementedException();
		}
		
		public void Start(bool withDebugging)
		{
			throw new NotImplementedException();
		}
		
		public ParseProjectContent CreateProjectContent()
		{
			throw new NotImplementedException();
		}
		
		public ProjectItem CreateProjectItem(IProjectItemBackendStore item)
		{
			throw new NotImplementedException();
		}
		
		public void Dispose()
		{
			throw new NotImplementedException();
		}
		
		public Properties CreateMemento()
		{
			throw new NotImplementedException();
		}
		
		public void SetMemento(ICSharpCode.Core.Properties memento)
		{
			throw new NotImplementedException();
		}
		
		public int MinimumSolutionVersion {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void ResolveAssemblyReferences()
		{
			throw new NotImplementedException();
		}
		
		public ICollection<IBuildable> GetBuildDependencies(ProjectBuildOptions buildOptions)
		{
			throw new NotImplementedException();
		}
		
		public void StartBuild(ProjectBuildOptions buildOptions, IBuildFeedbackSink feedbackSink)
		{
			throw new NotImplementedException();
		}
		
		public ProjectBuildOptions CreateProjectBuildOptions(BuildOptions options, bool isRootBuildable)
		{
			throw new NotImplementedException();
		}
		
		public void ProjectCreationComplete()
		{
			throw new NotImplementedException();
		}
		
		public System.Xml.Linq.XElement LoadProjectExtensions(string name)
		{
			throw new NotImplementedException();
		}
		
		public void SaveProjectExtensions(string name, System.Xml.Linq.XElement element)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
