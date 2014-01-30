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
using System.ComponentModel;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
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
		public const string LocalPathPropertyName = "LocalPath";
		
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
		
		internal static ProjectItem FindByEntity(IProject project, IEntityModel entity)
		{
			throw new NotImplementedException();
		}
		
//		internal ProjectItem(MSBuildBasedProject project, IClass c)
//			: this(new Project(project), project.FindFile(c.CompilationUnit.FileName))
//		{
//		}
//		
//		internal ProjectItem(IProjectContent projectContent, IClass c)
//			: this((MSBuildBasedProject)projectContent.Project, c)
//		{
//		}
		
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
			} else if ((name == FullPathPropertyName) || (name == LocalPathPropertyName)) {
				return projectItem.FileName.ToString();
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
//				if (!IsDirectory) {
//					return new FileCodeModel2(containingProject, projectItem);
//				}
//				return null;
				throw new NotImplementedException();
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
		
		public void Save(string fileName = null)
		{
			IViewContent view = containingProject.GetOpenFile(FileName);
			if (view != null) {
				containingProject.SaveFile(view);
			}
		}
	}
}
