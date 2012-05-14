// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Construction;
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Project : MarshalByRefObject
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
			ProjectItems = new ProjectItems(this, fileService);
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
			get { throw new NotImplementedException(); }
		}
		
		public virtual string FileName {
			get { return MSBuildProject.FileName; }
		}
		
		public virtual string FullName {
			get { return FileName; }
		}
		
		public virtual object Object { get; private set; }
		public virtual Properties Properties { get; private set; }
		public virtual ProjectItems ProjectItems { get; private set; }
		
		public virtual DTE DTE {
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
				projectService.AddProjectItem(MSBuildProject, referenceItem);
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
		
		internal void AddFile(string include)
		{
			var fileProjectItem = CreateFileProjectItem(include);
			projectService.AddProjectItem(MSBuildProject, fileProjectItem);
		}
		
		FileProjectItem CreateFileProjectItem(string include)
		{
			ItemType itemType = GetDefaultItemType(include);
			return CreateFileProjectItem(itemType, include);
		}
		
		ItemType GetDefaultItemType(string include)
		{
			return MSBuildProject.GetDefaultItemType(include);
		}
		
		FileProjectItem CreateFileProjectItem(ItemType itemType, string include)
		{
			var fileProjectItem = new FileProjectItem(MSBuildProject, itemType);
			fileProjectItem.Include = include;
			return fileProjectItem;
		}
		
		internal IList<string> GetAllPropertyNames()
		{
			var names = new List<string>();
			lock (MSBuildProject.SyncRoot) {
				foreach (ProjectPropertyElement propertyElement in MSBuildProject.MSBuildProjectFile.Properties) {
					names.Add(propertyElement.Name);
				}
			}
			return names;
		}
		
		public virtual CodeModel CodeModel {
			get { throw new NotImplementedException(); }
		}
		
		public virtual ConfigurationManager ConfigurationManager {
			get { throw new NotImplementedException(); }
		}
		
		internal virtual string GetLowercaseFileExtension()
		{
			return Path.GetExtension(FileName).ToLowerInvariant();
		}
	}
}
