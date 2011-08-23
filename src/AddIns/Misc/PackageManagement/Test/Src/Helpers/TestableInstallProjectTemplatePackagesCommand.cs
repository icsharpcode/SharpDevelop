// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.Helpers
{
	public class TestableInstallProjectTemplatePackagesCommand : InstallProjectTemplatePackagesCommand
	{
		public FakeMessageService FakeMessageService;
		public FakeLoggingService FakeLoggingService;
		public FakePackageRepositoryFactory FakePackageRepositoryCache;
		public FakePackageManagementProjectService FakeProjectService;
		
		public TestableInstallProjectTemplatePackagesCommand()
			: this(
				new FakePackageRepositoryFactory(),
				new FakePackageManagementProjectService(),
				new FakeMessageService(),
				new FakeLoggingService())
		{
		}
		
		public TestableInstallProjectTemplatePackagesCommand(
			FakePackageRepositoryFactory fakePackageRepositoryCache,
			FakePackageManagementProjectService projectService,
			FakeMessageService messageService,
			FakeLoggingService loggingService)
			: base(fakePackageRepositoryCache, projectService, messageService, loggingService)
		{
			this.FakePackageRepositoryCache = fakePackageRepositoryCache;
			this.FakeProjectService = projectService;
			this.FakeMessageService = messageService;
			this.FakeLoggingService = loggingService;
		}
		
		public MSBuildBasedProject ProjectPassedToCreatePackageReferencesForProject;
		public List<MSBuildBasedProject> ProjectsPassedToCreatePackageReferencesForProject = 
			new List<MSBuildBasedProject>();
		
		public IPackageRepositoryCache PackageRepositoryCachePassedToCreatePackageReferencesForProject;
		public FakePackageReferencesForProject FakePackageReferencesForProject = new FakePackageReferencesForProject();
		
		protected override IPackageReferencesForProject CreatePackageReferencesForProject(
			MSBuildBasedProject project,
			IPackageRepositoryCache packageRepositoryCache)
		{
			ProjectPassedToCreatePackageReferencesForProject = project;
			ProjectsPassedToCreatePackageReferencesForProject.Add(project);
			PackageRepositoryCachePassedToCreatePackageReferencesForProject = packageRepositoryCache;
			return FakePackageReferencesForProject;
		}
	}
}
