// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Project
	{
		IPackageManagementProjectService projectService;
		
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
			
			Object = new ProjectObject(this);
			Properties = new Properties(this);
			ProjectItems = new ProjectItems(this, fileService);
		}
		
		public string Name {
			get { return MSBuildProject.Name; }
		}
		
		public ProjectObject Object { get; private set; }
		public Properties Properties { get; private set; }
		public ProjectItems ProjectItems { get; private set; }
		
		internal MSBuildBasedProject MSBuildProject { get; private set; }
		
		internal void Save()
		{
			projectService.Save(MSBuildProject);
		}
		
		internal void AddReference(string path)
		{
			var referenceItem = new ReferenceProjectItem(MSBuildProject, path);
			projectService.AddProjectItem(MSBuildProject, referenceItem);
		}
		
		internal IEnumerable<ProjectItem> GetReferences()
		{
			return MSBuildProject.GetItemsOfType(ItemType.Reference);
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
			var fileProjectItem = new FileProjectItem(MSBuildProject, itemType);
			fileProjectItem.Include = include;
			return fileProjectItem;
		}
		
		ItemType GetDefaultItemType(string include)
		{
			return MSBuildProject.GetDefaultItemType(include);
		}
	}
}
