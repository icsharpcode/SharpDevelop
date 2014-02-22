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
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Templates;

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
				PackageManagementServices.PackageRepositoryCache,
				PackageManagementServices.ProjectService,
				SD.MessageService,
				SD.Log)
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
			var createInfo = Owner as ProjectTemplateResult;
			if (createInfo == null) {
				return Enumerable.Empty<MSBuildBasedProject>();
			}
				
			return createInfo.NewProjects.OfType<MSBuildBasedProject>();
		}
		
		IPackageReferencesForProject CreatePackageReferencesForProject(MSBuildBasedProject project)
		{
			return CreatePackageReferencesForProject(project, packageRepositoryCache);
		}
		
		protected virtual IPackageReferencesForProject CreatePackageReferencesForProject(
			MSBuildBasedProject project,
			IPackageRepositoryCache packageRepositoryCache)
		{
			return new PackageReferencesForProject(project, packageRepositoryCache);
		}
	}
}
