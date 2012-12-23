// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using ICSharpCode.Core;
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
		IPackageManagementProjectService projectService;
		
		public SharpDevelopProjectSystem(MSBuildBasedProject project)
			: this(project, new PackageManagementFileService(), new PackageManagementProjectService())
		{
		}
		
		public SharpDevelopProjectSystem(
			MSBuildBasedProject project,
			IPackageManagementFileService fileService,
			IPackageManagementProjectService projectService)
			: base(AppendTrailingSlashToDirectory(project.Directory))
		{
			this.project = project;
			this.fileService = fileService;
			this.projectService = projectService;
		}
		
		static string AppendTrailingSlashToDirectory(string directory)
		{
			return directory + @"\";
		}
		
		public bool IsBindingRedirectSupported { get; set; }
		
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
			AddReferenceToProject(assemblyReference);
		}
		
		ReferenceProjectItem CreateReference(string referencePath)
		{
			var assemblyReference = new ReferenceProjectItem(project);
			assemblyReference.Include = Path.GetFileNameWithoutExtension(referencePath);
			assemblyReference.HintPath = referencePath;
			return assemblyReference;
		}
		
		void AddReferenceToProject(ReferenceProjectItem assemblyReference)
		{
			projectService.AddProjectItem(project, assemblyReference);
			projectService.Save(project);
			LogAddedReferenceToProject(assemblyReference);
		}
		
		void LogAddedReferenceToProject(ReferenceProjectItem referenceProjectItem)
		{
			LogAddedReferenceToProject(referenceProjectItem.Include, ProjectName);
		}
		
		protected virtual void LogAddedReferenceToProject(string referenceName, string projectName)
		{
			DebugLogFormat("Added reference '{0}' to project '{1}'.", referenceName, projectName);
		}
		
		void DebugLogFormat(string format, params object[] args)
		{
			Logger.Log(MessageLevel.Debug, format, args);
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
			string referenceName = GetReferenceName(name);
			foreach (ReferenceProjectItem referenceProjectItem in project.GetItemsOfType(ItemType.Reference)) {
				if (IsMatchIgnoringCase(referenceProjectItem.Include, referenceName)) {
					return referenceProjectItem;
				}
			}
			return null;
		}
		
		string GetReferenceName(string name)
		{
			if (HasDllOrExeFileExtension(name)) {
				return Path.GetFileNameWithoutExtension(name);
			}
			return name;
		}
		
		bool HasDllOrExeFileExtension(string name)
		{
			string extension = Path.GetExtension(name);
			return
				IsMatchIgnoringCase(extension, ".dll") ||
				IsMatchIgnoringCase(extension, ".exe");
		}
		
		bool IsMatchIgnoringCase(string lhs, string rhs)
		{
			return String.Equals(lhs, rhs, StringComparison.InvariantCultureIgnoreCase);
		}
		
		public void RemoveReference(string name)
		{
			ReferenceProjectItem referenceProjectItem = FindReference(name);
			if (referenceProjectItem != null) {
				projectService.RemoveProjectItem(project, referenceProjectItem);
				projectService.Save(project);
				LogRemovedReferenceFromProject(referenceProjectItem);
			}
		}
		
		void LogRemovedReferenceFromProject(ReferenceProjectItem referenceProjectItem)
		{
			LogRemovedReferenceFromProject(referenceProjectItem.Include, ProjectName);
		}
		
		protected virtual void LogRemovedReferenceFromProject(string referenceName, string projectName)
		{
			DebugLogFormat("Removed reference '{0}' from project '{1}'.", referenceName, projectName);
		}
		
		public bool IsSupportedFile(string path)
		{
			if (project.IsWebProject()) {
				return !IsAppConfigFile(path);
			}
			return !IsWebConfigFile(path);
		}
		
		bool IsWebConfigFile(string path)
		{
			return IsFileNameMatchIgnoringPath("web.config", path);
		}
		
		bool IsAppConfigFile(string path)
		{
			return IsFileNameMatchIgnoringPath("app.config", path);
		}
		
		bool IsFileNameMatchIgnoringPath(string fileName1, string path)
		{
			string fileName2 = Path.GetFileName(path);
			return IsMatchIgnoringCase(fileName1, fileName2);
		}
		
		public override void AddFile(string path, Stream stream)
		{
			PhysicalFileSystemAddFile(path, stream);
			if (ShouldAddFileToProject(path)) {
				AddFileToProject(path);
			}
			LogAddedFileToProject(path);
		}
		
		protected virtual void PhysicalFileSystemAddFile(string path, Stream stream)
		{
			base.AddFile(path, stream);
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
		
		void AddFileToProject(string path)
		{
			FileProjectItem fileItem = CreateFileProjectItem(path);
			projectService.AddProjectItem(project, fileItem);
			projectService.Save(project);
		}
		
		FileProjectItem CreateFileProjectItem(string path)
		{
			ItemType itemType = project.GetDefaultItemType(path);
			var fileItem = new FileProjectItem(project, itemType);
			fileItem.FileName = path;
			fileItem.CustomTool = projectService.GetDefaultCustomToolForFileName(fileItem);
			return fileItem;
		}
		
		void LogAddedFileToProject(string fileName)
		{
			LogAddedFileToProject(fileName, ProjectName);
		}
		
		protected virtual void LogAddedFileToProject(string fileName, string projectName)
		{
			DebugLogFormat("Added file '{0}' to project '{1}'.", fileName, projectName);
		}
		
		public override void DeleteDirectory(string path, bool recursive)
		{
			string directory = GetFullPath(path);
			fileService.RemoveDirectory(directory);
			projectService.Save(project);
			LogDeletedDirectory(path);
		}
		
		public override void DeleteFile(string path)
		{
			string fileName = GetFullPath(path);
			fileService.RemoveFile(fileName);
			projectService.Save(project);
			LogDeletedFileInfo(path);
		}
		
		protected virtual void LogDeletedDirectory(string folder)
		{
			DebugLogFormat("Removed folder '{0}'.", folder);
		}
		
		void LogDeletedFileInfo(string path)
		{
			string fileName = Path.GetFileName(path);
			string directory = Path.GetDirectoryName(path);
			if (String.IsNullOrEmpty(directory)) {
				LogDeletedFile(fileName);
			} else {
				LogDeletedFileFromDirectory(fileName, directory);
			}
		}
		
		protected virtual void LogDeletedFile(string fileName)
		{
			DebugLogFormat("Removed file '{0}'.", fileName);
		}
		
		protected virtual void LogDeletedFileFromDirectory(string fileName, string directory)
		{
			DebugLogFormat("Removed file '{0}' from folder '{1}'.", fileName, directory);
		}
		
		public void AddFrameworkReference(string name)
		{
			ReferenceProjectItem assemblyReference = CreateGacReference(name);
			AddReferenceToProject(assemblyReference);
		}
		
		ReferenceProjectItem CreateGacReference(string name)
		{
			var assemblyReference = new ReferenceProjectItem(project);
			assemblyReference.Include = name;
			return assemblyReference;
		}
		
		public string ResolvePath(string path)
		{
			return path;
		}
	}
}
