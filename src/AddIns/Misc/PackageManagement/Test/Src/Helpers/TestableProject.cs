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
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Construction;

namespace PackageManagement.Tests.Helpers
{
	public class TestableProject : MSBuildBasedProject
	{
		public bool IsSaved;
		string assemblyName;
		string rootNamespace;
		bool isStartable = true;
		IAssemblyModel assemblyModel;
		IProjectContent projectContent;
		ILanguageBinding languageBinding;
		
		public ItemType ItemTypeToReturnFromGetDefaultItemType {
			get { return TestableProjectBehaviour.ItemTypeToReturnFromGetDefaultItemType; }
			set { TestableProjectBehaviour.ItemTypeToReturnFromGetDefaultItemType = value; }
		}
		
		public IReadOnlyCollection<ProjectItem> ItemsWhenSaved;
		
		public TestableProject(ProjectCreateInformation createInfo)
			: base(createInfo)
		{
		}
		
		public override string Language {
			get {
				// HACK: unit tests only provide the file extension, but we now need the language
				switch (this.FileName.GetExtension().ToLowerInvariant()) {
					case ".csproj":
						return "C#";
					case ".vbproj":
						return "VB";
					default:
						return string.Empty;
				}
			}
		}
		
		public override bool IsStartable {
			get { return isStartable; }
		}
		
		public override void Save(string fileName)
		{
			IsSaved = true;
			ItemsWhenSaved = Items;
		}
		
		public string FileNamePassedToGetDefaultItemType {
			get { return TestableProjectBehaviour.FileNamePassedToGetDefaultItemType; }
		}
		
		public TestableProjectBehaviour TestableProjectBehaviour = new TestableProjectBehaviour();
		
		protected override ProjectBehavior GetOrCreateBehavior()
		{
			return TestableProjectBehaviour;
		}
		
		public ReferenceProjectItem AddReference(string include)
		{
			var referenceProjectItem = new ReferenceProjectItem(this, include);
			ProjectService.AddProjectItem(this, referenceProjectItem);
			return referenceProjectItem;
		}
		
		public ProjectReferenceProjectItem AddProjectReference(IProject referencedProject)
		{
			var referenceProjectItem = new ProjectReferenceProjectItem(this, referencedProject);
			referenceProjectItem.FileName = referencedProject.FileName;
			ProjectService.AddProjectItem(this, referenceProjectItem);
			return referenceProjectItem;
		}
		
		public FileProjectItem AddFile(string include)
		{
			var fileProjectItem = new FileProjectItem(this, ItemType.Compile, include);
			ProjectService.AddProjectItem(this, fileProjectItem);
			return fileProjectItem;
		}
		
		public FileProjectItem AddDirectory(string include)
		{
			var fileProjectItem = new FileProjectItem(this, ItemType.Folder, include);
			ProjectService.AddProjectItem(this, fileProjectItem);
			return fileProjectItem;
		}
		
		public override string AssemblyName {
			get { return assemblyName; }
			set { assemblyName = value; }
		}
		
		public override string RootNamespace {
			get { return rootNamespace; }
			set { rootNamespace = value; }
		}
		
		public FileProjectItem AddDependentFile(string include, string dependentUpon)
		{
			FileProjectItem dependentFile = AddFile(include);
			dependentFile.DependentUpon = dependentUpon;
			return dependentFile;
		}
		
		public ProjectImportElement GetLastMSBuildChildElement()
		{
			lock (SyncRoot) {
				return MSBuildProjectFile.LastChild as ProjectImportElement;
			}
		}
		
		public ProjectImportElement GetFirstMSBuildChildElement()
		{
			lock (SyncRoot) {
				return MSBuildProjectFile.FirstChild as ProjectImportElement;
			}
		}
		
		public ICollection<ProjectImportElement> GetImports()
		{
			lock (SyncRoot) {
				return MSBuildProjectFile.Imports;
			}
		}
		
		public override IAssemblyModel AssemblyModel {
			get { return assemblyModel; }
		}
		
		public void SetAssemblyModel(IAssemblyModel assemblyModel)
		{
			this.assemblyModel = assemblyModel;
		}
		
		public override IProjectContent ProjectContent {
			get { return projectContent; }
		}
		
		public void SetProjectContent(IProjectContent projectContent)
		{
			this.projectContent = projectContent;
		}
		
		public void SetLanguageBinding(ILanguageBinding languageBinding)
		{
			this.languageBinding = languageBinding;
		}
		
		public override ILanguageBinding LanguageBinding {
			get { return languageBinding; }
		}
	}
}
