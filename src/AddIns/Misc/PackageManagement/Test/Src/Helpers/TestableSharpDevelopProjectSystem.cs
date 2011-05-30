// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.Helpers
{
	public class TestableSharpDevelopProjectSystem : SharpDevelopProjectSystem
	{
		public string PathPassedToPhysicalFileSystemAddFile;
		public Stream StreamPassedToPhysicalFileSystemAddFile;
		public FakeFileService FakeFileService;
		public FakePackageManagementProjectService FakeProjectService;
		public FakeLogger FakeLogger;
		public string FileNamePassedToLogDeletedFile;
		public FileNameAndDirectory FileNameAndDirectoryPassedToLogDeletedFileFromDirectory;
		public string DirectoryPassedToLogDeletedDirectory;
		public ReferenceAndProjectName ReferenceAndProjectNamePassedToLogAddedReferenceToProject;
		public ReferenceAndProjectName ReferenceAndProjectNamePassedToLogRemovedReferenceFromProject;
		public FileNameAndProjectName FileNameAndProjectNamePassedToLogAddedFileToProject;
		
		public TestableSharpDevelopProjectSystem(MSBuildBasedProject project)
			: this(
				project,
				new FakeFileService(project),
				new FakePackageManagementProjectService(),
				new FakeLogger())
		{
		}
		
		TestableSharpDevelopProjectSystem(
			MSBuildBasedProject project,
			IPackageManagementFileService fileService,
			IPackageManagementProjectService projectService,
			FakeLogger logger)
			: base(project, fileService, projectService)
		{
			FakeFileService = (FakeFileService)fileService;
			FakeProjectService = (FakePackageManagementProjectService)projectService;
			Logger = logger;
		}
		
		protected override void PhysicalFileSystemAddFile(string path, Stream stream)
		{
			PathPassedToPhysicalFileSystemAddFile = path;
			StreamPassedToPhysicalFileSystemAddFile = stream;
		}
		
		protected override void LogDeletedFile(string fileName)
		{
			FileNamePassedToLogDeletedFile = fileName;
		}
		
		protected override void LogDeletedFileFromDirectory(string fileName, string directory)
		{
			FileNameAndDirectoryPassedToLogDeletedFileFromDirectory = new FileNameAndDirectory(fileName, directory);
		}
		
		protected override void LogDeletedDirectory(string directory)
		{
			DirectoryPassedToLogDeletedDirectory = directory;
		}
		
		protected override void LogAddedReferenceToProject(string referenceName, string projectName)
		{
			ReferenceAndProjectNamePassedToLogAddedReferenceToProject = 
				new ReferenceAndProjectName(referenceName, projectName);
		}
		
		protected override void LogRemovedReferenceFromProject(string referenceName, string projectName)
		{
			ReferenceAndProjectNamePassedToLogRemovedReferenceFromProject = 
				new ReferenceAndProjectName(referenceName, projectName);
		}
		
		protected override void LogAddedFileToProject(string fileName, string projectName)
		{
			FileNameAndProjectNamePassedToLogAddedFileToProject =
				new FileNameAndProjectName(fileName, projectName);
		}
	}
}
