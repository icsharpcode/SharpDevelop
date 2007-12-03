// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.BuildEngine;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// MockProject.
	/// </summary>
	public class MockProject : IProject
	{
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
		
		public ICSharpCode.SharpDevelop.Dom.IAmbience Ambience {
			get {
				throw new NotImplementedException();
			}
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
			get {
				throw new NotImplementedException();
			}
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
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
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
			get {
				throw new NotImplementedException();
			}
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
			get {
				throw new NotImplementedException();
			}
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
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
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
		
		public void StartBuild(BuildOptions options)
		{
			throw new NotImplementedException();
		}
		
		public ProjectItem CreateProjectItem(BuildItem item)
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
		#endregion		
	}
}
