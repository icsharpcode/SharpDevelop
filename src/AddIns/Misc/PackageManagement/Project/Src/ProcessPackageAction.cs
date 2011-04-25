// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public abstract class ProcessPackageAction
	{
		IPackageManagementEvents packageManagementEvents;
		
		public ProcessPackageAction(
			IPackageManagementProject project,
			IPackageManagementEvents packageManagementEvents)
		{
			this.Project = project;
			this.packageManagementEvents = packageManagementEvents;
		}
		
		public IPackageManagementProject Project { get; set; }
		public ILogger Logger { get; set; }
		public IPackage Package { get; set; }
		public Version PackageVersion { get; set; }
		public string PackageId { get; set; }
		public IPackageScriptSession PackageScriptSession { get; set; }
		
		protected void OnParentPackageInstalled()
		{
			packageManagementEvents.OnParentPackageInstalled(Package);
		}
		
		protected void OnParentPackageUninstalled()
		{
			packageManagementEvents.OnParentPackageUninstalled(Package);
		}
		
		public void Execute()
		{
			BeforeExecute();
			using (RunPackageScriptsAction runScriptsAction = CreateRunPackageScriptsAction()) {
				ExecuteCore();
			}
		}
		
		protected virtual void BeforeExecute()
		{
			GetLoggerIfMissing();
			ConfigureProjectLogger();
			GetPackageIfMissing();
		}
		
		RunPackageScriptsAction CreateRunPackageScriptsAction()
		{
			return CreateRunPackageScriptsAction(PackageScriptSession, Project);
		}
		
		protected virtual RunPackageScriptsAction CreateRunPackageScriptsAction(
			IPackageScriptSession session,
			IPackageManagementProject project)
		{
			return new RunPackageScriptsAction(session, project);
		}
		
		protected virtual void ExecuteCore()
		{
		}
		
		void GetLoggerIfMissing()
		{
			if (Logger == null) {
				Logger = new PackageManagementLogger(packageManagementEvents);
			}
		}
		
		void ConfigureProjectLogger()
		{
			Project.Logger = Logger;
		}
		
		void GetPackageIfMissing()
		{
			if (Package == null) {
				Package = Project.SourceRepository.FindPackage(PackageId, PackageVersion);
			}
		}
	}
}
