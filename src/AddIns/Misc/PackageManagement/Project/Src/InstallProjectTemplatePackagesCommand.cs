// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement
{
	public class InstallProjectTemplatePackagesCommand : AbstractCommand
	{
		IPackageRepositoryCache packageRepositoryCache;
		IPackageManagementProjectService projectService;
		IMessageService messageService;
		ILoggingService loggingService;
		
		public InstallProjectTemplatePackagesCommand()
			: this(
				PackageManagementServices.ProjectTemplatePackageRepositoryCache,
				PackageManagementServices.ProjectService,
				ServiceManager.Instance.MessageService,
				ServiceManager.Instance.LoggingService)
		{
		}
		
		public InstallProjectTemplatePackagesCommand(
			IPackageRepositoryCache packageRepositoryCache,
			IPackageManagementProjectService projectService,	
			IMessageService messageService,
			ILoggingService loggingService)
		{
			this.packageRepositoryCache = packageRepositoryCache;
			this.projectService = projectService;
			this.messageService = messageService;
			this.loggingService = loggingService;
		}
		
		public override void Run()
		{
			try {
				InstallPackages();
			} catch (Exception ex) {
				DisplayError(ex);
			}
		}
		
		void DisplayError(Exception ex)
		{
			loggingService.Error(null, ex);
			messageService.ShowError(ex.Message);
		}
		
		void InstallPackages()
		{
			foreach (MSBuildBasedProject project in GetCreatedProjects()) {
				IPackageReferencesForProject packageReferences = CreatePackageReferencesForProject(project);
				packageReferences.RemovePackageReferences();
				packageReferences.InstallPackages();
			}
		}
		
		IEnumerable<MSBuildBasedProject> GetCreatedProjects()
		{
			var createInfo = Owner as ProjectCreateInformation;
			var newCreatedProjects = new NewProjectsCreated(createInfo, projectService);
			return newCreatedProjects.GetProjects();
		}
		
		IPackageReferencesForProject CreatePackageReferencesForProject(MSBuildBasedProject project)
		{
			return CreatePackageReferencesForProject(project, packageRepositoryCache);
		}
		
		protected virtual IPackageReferencesForProject 
			CreatePackageReferencesForProject(MSBuildBasedProject project, IPackageRepositoryCache packageRepositoryCache)
		{
			return new PackageReferencesForProject(project, packageRepositoryCache);
		}
	}
}
