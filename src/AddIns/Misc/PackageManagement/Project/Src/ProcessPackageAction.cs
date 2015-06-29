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
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public abstract class ProcessPackageAction : IPackageAction
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
		public SemanticVersion PackageVersion { get; set; }
		public string PackageId { get; set; }
		public IPackageScriptRunner PackageScriptRunner { get; set; }
		public bool AllowPrereleaseVersions { get; set; }
		
		public virtual bool HasPackageScriptsToRun()
		{
			return false;
		}
		
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
			RunWithExceptionReporting(() => {
				BeforeExecute();
				if (PackageScriptRunner != null) {
					ExecuteWithScriptRunner();
				} else {
					ExecuteCore();
				}
			});
		}
		
		void RunWithExceptionReporting(Action action)
		{
			try {
				action();
			} catch (Exception ex) {
				packageManagementEvents.OnPackageOperationError(ex);
				throw;
			}
		}
		
		protected virtual void BeforeExecute()
		{
			GetLoggerIfMissing();
			ConfigureProjectLogger();
			GetPackageIfMissing();
		}
		
		void ExecuteWithScriptRunner()
		{
			using (RunPackageScriptsAction runScriptsAction = CreateRunPackageScriptsAction()) {
				ExecuteCore();
			}
		}
		
		RunPackageScriptsAction CreateRunPackageScriptsAction()
		{
			return CreateRunPackageScriptsAction(PackageScriptRunner, Project);
		}
		
		protected virtual RunPackageScriptsAction CreateRunPackageScriptsAction(
			IPackageScriptRunner scriptRunner,
			IPackageManagementProject project)
		{
			return new RunPackageScriptsAction(scriptRunner, project);
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
				Package = FindPackage();
			}
			if (Package == null) {
				ThrowPackageNotFoundError(PackageId);
			}
		}
		
		protected virtual IPackage FindPackage()
		{
			return Project.SourceRepository.FindPackage(
				PackageId,
				PackageVersion,
				Project.ConstraintProvider,
				AllowPrereleaseVersions,
				allowUnlisted: false);
		}
		
		void ThrowPackageNotFoundError(string packageId)
		{
			string message = String.Format("Unable to find package '{0}'.", packageId);
			throw new ApplicationException(message);
		}
		
		protected bool PackageIdExistsInProject()
		{
			string id = GetPackageId();
			return Project.IsPackageInstalled(id);
		}
		
		string GetPackageId()
		{
			if (Package != null) {
				return Package.Id;
			}
			return PackageId;
		}
		
		protected virtual IOpenPackageReadMeMonitor CreateOpenPackageReadMeMonitor(string packageId)
		{
			return new OpenPackageReadMeMonitor(packageId, Project);
		}
	}
}
