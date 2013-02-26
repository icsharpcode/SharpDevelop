// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

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
		
		public bool ReadOnly {
			get { return false; }
		}
		
		public string Directory {
			get { return directory; }
			set { directory = value; }
		}
		
		public string RootNamespace {
			get { return rootNamespace; }
			set { rootNamespace = value; }
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
		
		public event EventHandler ActiveConfigurationChanged { add {} remove {} }
		public event EventHandler ActivePlatformChanged { add {} remove {} }
		
		public object SyncRoot {
			get { return syncRoot; }
		}
		
		public ISolution ParentSolution {
			get { return new Solution(new MockProjectChangeWatcher()); }
		}
		
		public event EventHandler<ICSharpCode.SharpDevelop.Parser.ParseInformationEventArgs> ParseInformationUpdated { add {} remove {} }
		
		public IReadOnlyCollection<ProjectItem> Items {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IReadOnlyCollection<ItemType> AvailableFileItemTypes {
			get {
				throw new NotImplementedException();
			}
		}
		
		public List<ProjectSection> ProjectSections {
			get {
				throw new NotImplementedException();
			}
		}
		
		public FileName FileName {
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
		
		public string AssemblyName {
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
		
		public IReadOnlyCollection<string> ConfigurationNames {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IReadOnlyCollection<string> PlatformNames {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsStartable {
			get {
				throw new NotImplementedException();
			}
		}
		
		public Properties ProjectSpecificProperties {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int MinimumSolutionVersion {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IProjectContent ProjectContent {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICSharpCode.SharpDevelop.Refactoring.ICodeGenerator CodeGenerator {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICSharpCode.SharpDevelop.Dom.ITypeDefinitionModelCollection TypeDefinitionModels {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ISolutionFolder Parent {
			get {
				throw new NotImplementedException();
			}
			set {
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
		
		public ProjectItem CreateProjectItem(IProjectItemBackendStore item)
		{
			throw new NotImplementedException();
		}
		
		public IEnumerable<ReferenceProjectItem> ResolveAssemblyReferences(System.Threading.CancellationToken cancellationToken)
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
		
		public bool HasProjectType(Guid projectTypeGuid)
		{
			throw new NotImplementedException();
		}
		
		public string GetDefaultNamespace(string fileName)
		{
			throw new NotImplementedException();
		}
		
		public System.CodeDom.Compiler.CodeDomProvider CreateCodeDomProvider()
		{
			throw new NotImplementedException();
		}
		
		public void GenerateCodeFromCodeDom(System.CodeDom.CodeCompileUnit compileUnit, System.IO.TextWriter writer)
		{
			throw new NotImplementedException();
		}
		
		public IAmbience GetAmbience()
		{
			throw new NotImplementedException();
		}
		
		public ICSharpCode.SharpDevelop.Refactoring.ISymbolSearch PrepareSymbolSearch(IEntity entity)
		{
			throw new NotImplementedException();
		}
		
		public void OnParseInformationUpdated(ICSharpCode.SharpDevelop.Parser.ParseInformationEventArgs args)
		{
			throw new NotImplementedException();
		}
		
		public IEnumerable<IBuildable> GetBuildDependencies(ProjectBuildOptions buildOptions)
		{
			throw new NotImplementedException();
		}
		
		public System.Threading.Tasks.Task<bool> BuildAsync(ProjectBuildOptions options, IBuildFeedbackSink feedbackSink, IProgressMonitor progressMonitor)
		{
			throw new NotImplementedException();
		}
		
		public ProjectBuildOptions CreateProjectBuildOptions(BuildOptions options, bool isRootBuildable)
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
		
		public void SetMemento(Properties memento)
		{
			throw new NotImplementedException();
		}
	}
}
