// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Construction;
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Project : MarshalByRefObject, global::EnvDTE.Project
	{
		IPackageManagementProjectService projectService;
		IPackageManagementFileService fileService;
		DTE dte;
		
		public Project(MSBuildBasedProject project)
			: this(
				project,
				new PackageManagementProjectService(),
				new PackageManagementFileService())
		{
		}
		
		public Project(
			MSBuildBasedProject project,
			IPackageManagementProjectService projectService,
			IPackageManagementFileService fileService)
		{
			this.MSBuildProject = project;
			this.projectService = projectService;
			this.fileService = fileService;
			
			CreateProperties();
			Object = new ProjectObject(this);
			ProjectItems = new ProjectItems(this, this, fileService);
		}
		
		public Project()
		{
		}
		
		void CreateProperties()
		{
			var propertyFactory = new ProjectPropertyFactory(this);
			Properties = new Properties(propertyFactory);
		}
		
		public virtual string Name {
			get { return MSBuildProject.Name; }
		}
	
		public virtual string UniqueName {
			get { return GetUniqueName(); }
		}
		
		string GetUniqueName()
		{
			return FileUtility.GetRelativePath(MSBuildProject.ParentSolution.Directory, FileName);
		}
		
		public virtual string FileName {
			get { return MSBuildProject.FileName; }
		}
		
		public virtual string FullName {
			get { return FileName; }
		}
		
		public virtual object Object { get; private set; }
		public virtual global::EnvDTE.Properties Properties { get; private set; }
		public virtual global::EnvDTE.ProjectItems ProjectItems { get; private set; }
		
		public virtual global::EnvDTE.DTE DTE {
			get {
				if (dte == null) {
					dte = new DTE(projectService, fileService);
				}
				return dte;
			}
		}
		
		public virtual string Type {
			get { return GetProjectType(); }
		}
		
		string GetProjectType()
		{
			return new ProjectType(this).Type;
		}
		
		public virtual string Kind {
			get { return GetProjectKind(); }
		}
		
		string GetProjectKind()
		{
			return new ProjectKind(this).Kind;
		}
		
		internal MSBuildBasedProject MSBuildProject { get; private set; }
		
		public virtual void Save()
		{
			projectService.Save(MSBuildProject);
		}
		
		internal virtual void AddReference(string path)
		{
			if (!HasReference(path)) {
				var referenceItem = new ReferenceProjectItem(MSBuildProject, path);
				AddProjectItemToMSBuildProject(referenceItem);
			}
		}
		
		bool HasReference(string include)
		{
			foreach (ReferenceProjectItem reference in GetReferences()) {
				if (IsReferenceMatch(reference, include)) {
					return true;
				}
			}
			return false;
		}
		
		void AddProjectItemToMSBuildProject(SD.ProjectItem projectItem)
		{
			projectService.AddProjectItem(MSBuildProject, projectItem);
		}
		
		internal IEnumerable<SD.ProjectItem> GetReferences()
		{
			return MSBuildProject.GetItemsOfType(ItemType.Reference);
		}
		
		bool IsReferenceMatch(ReferenceProjectItem reference, string include)
		{
			return String.Equals(reference.Include, include, StringComparison.InvariantCultureIgnoreCase);
		}
		
		internal void RemoveReference(ReferenceProjectItem referenceItem)
		{
			projectService.RemoveProjectItem(MSBuildProject, referenceItem);
		}
		
		internal ProjectItem AddFileProjectItemUsingFullPath(string path)
		{
			string dependentUpon = GetDependentUpon(path);
			return AddFileProjectItemWithDependentUsingFullPath(path, dependentUpon);
		}
		
		string GetDependentUpon(string path)
		{
			var dependentFile = new DependentFile(MSBuildProject);
			FileProjectItem projectItem = dependentFile.GetParentFileProjectItem(path);
			if (projectItem != null) {
				return Path.GetFileName(projectItem.Include);
			}
			return null;
		}
		
		internal ProjectItem AddFileProjectItemWithDependentUsingFullPath(string path, string dependentUpon)
		{
			FileProjectItem fileProjectItem = CreateFileProjectItemUsingFullPath(path);
			fileProjectItem.FileName = path;
			fileProjectItem.DependentUpon = dependentUpon;
			AddProjectItemToMSBuildProject(fileProjectItem);
			return new ProjectItem(this, fileProjectItem);
		}
		
		FileProjectItem CreateFileProjectItemUsingPathRelativeToProject(string include)
		{
			ItemType itemType = GetDefaultItemType(include);
			return CreateFileProjectItemUsingPathRelativeToProject(itemType, include);
		}
		
		ItemType GetDefaultItemType(string include)
		{
			return MSBuildProject.GetDefaultItemType(Path.GetFileName(include));
		}
		
		FileProjectItem CreateFileProjectItemUsingPathRelativeToProject(ItemType itemType, string include)
		{
			var fileItem = new FileProjectItem(MSBuildProject, itemType) {
				Include = include
			};
			if (IsLink(include)) {
				fileItem.SetEvaluatedMetadata("Link", Path.GetFileName(include));
			}
			return fileItem;
		}
		
		bool IsLink(string include)
		{
			return include.StartsWith("..");
		}
		
		FileProjectItem CreateFileProjectItemUsingFullPath(string path)
		{
			string relativePath = GetRelativePath(path);
			return CreateFileProjectItemUsingPathRelativeToProject(relativePath);
		}
		
		internal IList<string> GetAllPropertyNames()
		{
			var names = new List<string>();
			lock (MSBuildProject.SyncRoot) {
				foreach (ProjectPropertyElement propertyElement in MSBuildProject.MSBuildProjectFile.Properties) {
					names.Add(propertyElement.Name);
				}
				names.Add("OutputFileName");
			}
			return names;
		}
		
		public virtual global::EnvDTE.CodeModel CodeModel {
			get { return new CodeModel(projectService.GetProjectContent(MSBuildProject) ); }
		}
		
		public virtual global::EnvDTE.ConfigurationManager ConfigurationManager {
			get { return new ConfigurationManager(this); }
		}
		
		internal virtual string GetLowercaseFileExtension()
		{
			return Path.GetExtension(FileName).ToLowerInvariant();
		}
		
		internal virtual void DeleteFile(string fileName)
		{
			fileService.RemoveFile(fileName);
		}
		
		internal ProjectItem AddDirectoryProjectItemUsingFullPath(string directory)
		{
			AddDirectoryProjectItemsRecursively(directory);
			return DirectoryProjectItem.CreateDirectoryProjectItemFromFullPath(this, directory);
		}
		
		void AddDirectoryProjectItemsRecursively(string directory)
		{
			string[] files = fileService.GetFiles(directory);
			string[] childDirectories = fileService.GetDirectories(directory);
			if (files.Any()) {
				foreach (string file in files) {
					AddFileProjectItemUsingFullPath(file);
				}
			} else if (!childDirectories.Any()) {
				AddDirectoryProjectItemToMSBuildProject(directory);
			}
			
			foreach (string childDirectory in childDirectories) {
				AddDirectoryProjectItemsRecursively(childDirectory);
			}
		}
		
		void AddDirectoryProjectItemToMSBuildProject(string directory)
		{
			FileProjectItem projectItem = CreateMSBuildProjectItemForDirectory(directory);
			AddProjectItemToMSBuildProject(projectItem);
		}
		
		FileProjectItem CreateMSBuildProjectItemForDirectory(string directory)
		{
			return new FileProjectItem(MSBuildProject, ItemType.Folder) {
				FileName = directory
			};
		}
		
		internal string GetRelativePath(string path)
		{
			return FileUtility.GetRelativePath(MSBuildProject.Directory, path);
		}
		
		internal IProjectBrowserUpdater CreateProjectBrowserUpdater()
		{
			return projectService.CreateProjectBrowserUpdater();
		}
		
		internal ICompilationUnit GetCompilationUnit(string fileName)
		{
			return fileService.GetCompilationUnit(fileName);
		}
		
		internal void RemoveProjectItem(ProjectItem projectItem)
		{
			projectService.RemoveProjectItem(MSBuildProject, projectItem.MSBuildProjectItem);
		}
		
		internal ProjectItem FindProjectItem(string fileName)
		{
			SD.FileProjectItem item = MSBuildProject.FindFile(fileName);
			if (item != null) {
				return new ProjectItem(this, item);
			}
			return null;
		}
		
		internal IViewContent GetOpenFile(string fileName)
		{
			return fileService.GetOpenFile(fileName);
		}
		
		internal void OpenFile(string fileName)
		{
			fileService.OpenFile(fileName);
		}
		
		internal bool IsFileFileInsideProjectFolder(string filePath)
		{
			return FileUtility.IsBaseDirectory(MSBuildProject.Directory, filePath);
		}
	}
}
