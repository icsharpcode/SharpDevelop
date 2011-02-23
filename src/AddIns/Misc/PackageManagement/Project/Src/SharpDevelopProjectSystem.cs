// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class SharpDevelopProjectSystem : PhysicalFileSystem, IProjectSystem
	{
		MSBuildBasedProject project;
		ProjectTargetFramework targetFramework;
		IPackageManagementFileService fileService;
		
		public SharpDevelopProjectSystem(MSBuildBasedProject project)
			: this(project, new PackageManagementFileService())
		{
		}
		
		public SharpDevelopProjectSystem(MSBuildBasedProject project, IPackageManagementFileService fileService)
			: base(AppendTrailingSlashToDirectory(project.Directory))
		{
			this.project = project;
			this.fileService = fileService;
		}
		
		static string AppendTrailingSlashToDirectory(string directory)
		{
			return directory + @"\";
		}
		
		public FrameworkName TargetFramework {
			get { return GetTargetFramework(); }
		}
		
		FrameworkName GetTargetFramework()
		{
			if (targetFramework == null) {
				targetFramework = new ProjectTargetFramework(project);
			}
			return targetFramework.TargetFrameworkName;
		}
		
		public string ProjectName {
			get { return project.Name; }
		}
		
		public dynamic GetPropertyValue(string propertyName)
		{
			return project.GetEvaluatedProperty(propertyName);
		}
		
		public void AddReference(string referencePath, Stream stream)
		{
			ReferenceProjectItem assemblyReference = CreateReference(referencePath);
			ProjectService.AddProjectItem(project, assemblyReference);
			project.Save();
		}
		
		ReferenceProjectItem CreateReference(string referencePath)
		{
			var assemblyReference = new ReferenceProjectItem(project);
			assemblyReference.Include = Path.GetFileNameWithoutExtension(referencePath);
			assemblyReference.HintPath = referencePath;
			return assemblyReference;
		}
		
		public bool ReferenceExists(string name)
		{
			ReferenceProjectItem referenceProjectItem = FindReference(name);
			if (referenceProjectItem != null) {
				return true;
			}
			return false;
		}
		
		ReferenceProjectItem FindReference(string name)
		{
			string referenceName = Path.GetFileNameWithoutExtension(name);
			foreach (ReferenceProjectItem referenceProjectItem in project.GetItemsOfType(ItemType.Reference)) {
				if (IsMatchIgnoringCase(referenceProjectItem.Include, referenceName)) {
					return referenceProjectItem;
				}
			}
			return null;
		}
		
		bool IsMatchIgnoringCase(string lhs, string rhs)
		{
			return String.Equals(lhs, rhs, StringComparison.InvariantCultureIgnoreCase);
		}
		
		public void RemoveReference(string name)
		{
			ReferenceProjectItem referenceProjectItem = FindReference(name);
			if (referenceProjectItem != null) {
				ProjectService.RemoveProjectItem(project, referenceProjectItem);
				project.Save();
			}
		}
		
		public bool IsSupportedFile(string path)
		{
			return !IsAppConfigFile(path);
		}
		
		bool IsAppConfigFile(string path)
		{
			string fileName = Path.GetFileName(path);
			return IsMatchIgnoringCase("app.config", fileName);
		}
		
		public override void AddFile(string path, Stream stream)
		{
			PhysicalFileSystemAddFile(path, stream);
			if (ShouldAddFileToProject(path)) {
				AddFileToProject(path);
			}
		}
		
		protected virtual void PhysicalFileSystemAddFile(string path, Stream stream)
		{
			base.AddFile(path, stream);
		}
		
		void AddFileToProject(string path)
		{
			FileProjectItem fileItem = CreateFileProjectItem(path);
			ProjectService.AddProjectItem(project, fileItem);
			project.Save();
		}
		
		FileProjectItem CreateFileProjectItem(string path)
		{
			ItemType itemType = project.GetDefaultItemType(path);
			FileProjectItem fileItem = new FileProjectItem(project, itemType);
			fileItem.FileName = path;
			return fileItem;
		}
		
		bool ShouldAddFileToProject(string path)
		{
			return !IsBinDirectory(path) && !FileExistsInProject(path);
		}
		
		bool IsBinDirectory(string path)
		{
			string directoryName = Path.GetDirectoryName(path);
			return IsMatchIgnoringCase(directoryName, "bin");
		}
		
		bool FileExistsInProject(string path)
		{
			string fullPath = GetFullPath(path);
			return project.IsFileInProject(fullPath);
		}
		
		public override void DeleteDirectory(string path, bool recursive)
		{
			string directory = GetFullPath(path);
			fileService.RemoveDirectory(directory);
			project.Save();
		}
		
		public override void DeleteFile(string path)
		{
			string fileName = GetFullPath(path);
			fileService.RemoveFile(fileName);
			project.Save();
		}
	}
}
