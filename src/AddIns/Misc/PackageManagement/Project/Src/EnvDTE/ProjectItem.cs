// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectItem : global::EnvDTE.ProjectItemBase, global::EnvDTE.ProjectItem
	{
		SD.FileProjectItem projectItem;
		Project containingProject;
		
		public const string CopyToOutputDirectoryPropertyName = "CopyToOutputDirectory";
		public const string CustomToolPropertyName = "CustomTool";
		public const string FullPathPropertyName = "FullPath";
		
		public ProjectItem(Project project, FileProjectItem projectItem)
		{
			this.projectItem = projectItem;
			this.containingProject = project;
			this.ProjectItems = CreateProjectItems(projectItem);
			CreateProperties();
			Kind = GetKindFromFileProjectItemType();
		}
		
		global::EnvDTE.ProjectItems CreateProjectItems(FileProjectItem projectItem)
		{
			if (projectItem.ItemType == ItemType.Folder) {
				return new DirectoryProjectItems(this);
			}
			return new FileProjectItems(this);
		}
		
		internal ProjectItem(MSBuildBasedProject project, IClass c)
			: this(new Project(project), project.FindFile(c.CompilationUnit.FileName))
		{
		}
		
		internal ProjectItem(IProjectContent projectContent, IClass c)
			: this((MSBuildBasedProject)projectContent.Project, c)
		{
		}
		
		string GetKindFromFileProjectItemType()
		{
			if (IsDirectory) {
				return global::EnvDTE.Constants.vsProjectItemKindPhysicalFolder;
			}
			return global::EnvDTE.Constants.vsProjectItemKindPhysicalFile;
		}
		
		bool IsDirectory {
			get { return projectItem.ItemType == ItemType.Folder; }
		}
		
		public ProjectItem()
		{
		}
		
		void CreateProperties()
		{
			var propertyFactory = new ProjectItemPropertyFactory(this);
			Properties = new Properties(propertyFactory);
		}
		
		public virtual string Name {
			get { return Path.GetFileName(projectItem.Include); }
		}
		
		public virtual string Kind { get; set; }
		
		public global::EnvDTE.Project SubProject {
			get { return null; }
		}
		
		public virtual global::EnvDTE.Properties Properties { get; private set; }
		public virtual global::EnvDTE.Project ContainingProject {
			get { return this.containingProject; }
		}
		public virtual global::EnvDTE.ProjectItems ProjectItems { get; private set; }
		
		internal virtual object GetProperty(string name)
		{
			if (name == CopyToOutputDirectoryPropertyName) {
				return GetCopyToOutputDirectory();
			} else if (name == CustomToolPropertyName) {
				return projectItem.CustomTool;
			} else if (name == FullPathPropertyName) {
				return projectItem.FileName;
			}
			return String.Empty;
		}
		
		UInt32 GetCopyToOutputDirectory()
		{
			return (UInt32)projectItem.CopyToOutputDirectory;
		}
		
		internal virtual void SetProperty(string name, object value)
		{
			if (name == CopyToOutputDirectoryPropertyName) {
				SetCopyToOutputDirectory(value);
			} else if (name == CustomToolPropertyName) {
				projectItem.CustomTool = value as string;
			}
		}
		
		void SetCopyToOutputDirectory(object value)
		{
			CopyToOutputDirectory copyToOutputDirectory = ConvertToCopyToOutputDirectory(value);
			projectItem.CopyToOutputDirectory = copyToOutputDirectory;
		}
		
		CopyToOutputDirectory ConvertToCopyToOutputDirectory(object value)
		{
			string valueAsString = value.ToString();
			return (CopyToOutputDirectory)Enum.Parse(typeof(CopyToOutputDirectory), valueAsString);
		}
		
		internal virtual bool IsMatchByName(string name)
		{
			return String.Equals(this.Name, name, StringComparison.InvariantCultureIgnoreCase);
		}
		
		internal virtual bool IsChildItem(SD.ProjectItem msbuildProjectItem)
		{
			string directory = Path.GetDirectoryName(msbuildProjectItem.Include);
			return IsMatchByName(directory);
		}
		
		internal virtual ProjectItemRelationship GetRelationship(SD.ProjectItem msbuildProjectItem)
		{
			return new ProjectItemRelationship(this, msbuildProjectItem);
		}
		
		public void Delete()
		{
			containingProject.DeleteFile(projectItem.FileName);
			containingProject.Save();
		}
		
		public global::EnvDTE.FileCodeModel2 FileCodeModel {
			get {
				if (!IsDirectory) {
					return new FileCodeModel2(containingProject, projectItem);
				}
				return null;
			}
		}
		
		internal string GetIncludePath(string fileName)
		{
			string relativeDirectory = GetProjectItemRelativePathToProject();
			return Path.Combine(relativeDirectory, fileName);
		}
		
		string GetProjectItemRelativePathToProject()
		{
			return containingProject.GetRelativePath(projectItem.FileName);
		}
		
		internal string GetIncludePath()
		{
			return projectItem.Include;
		}
		
		public virtual void Remove()
		{
			containingProject.RemoveProjectItem(this);
			containingProject.Save();
		}
		
		internal FileProjectItem MSBuildProjectItem {
			get { return projectItem; }
		}
		
		protected override string GetFileNames(short index)
		{
			return FileName;
		}
		
		string FileName {
			get { return projectItem.FileName; }
		}
		
		public virtual global::EnvDTE.Document Document {
			get { return GetOpenDocument(); }
		}
		
		Document GetOpenDocument()
		{
			IViewContent view = containingProject.GetOpenFile(FileName);
			if (view != null) {
				return new Document(FileName, view);
			}
			return null;
		}
		
		public virtual global::EnvDTE.Window Open(string viewKind)
		{
			containingProject.OpenFile(FileName);
			return null;
		}
		
		public virtual short FileCount {
			get { return 1; }
		}
		
		public global::EnvDTE.ProjectItems Collection {
			get {
				string relativePath = GetProjectItemRelativeDirectoryToProject();
				if (String.IsNullOrEmpty(relativePath)) {
					return containingProject.ProjectItems;
				}
				var directoryProjectItem = new DirectoryProjectItem(containingProject, relativePath);
				return directoryProjectItem.ProjectItems;
			}
		}
		
		string GetProjectItemRelativeDirectoryToProject()
		{
			return Path.GetDirectoryName(GetProjectItemRelativePathToProject());
		}
	}
}
