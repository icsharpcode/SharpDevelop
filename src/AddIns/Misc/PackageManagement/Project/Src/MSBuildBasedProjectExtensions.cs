// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Construction;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public static class MSBuildBasedProjectExtensions
	{
		public static readonly Guid WebApplication = Guid.Parse(ProjectTypeGuids.WebApplication);
		public static readonly Guid WebSite = Guid.Parse(ProjectTypeGuids.WebSite);
		
		public static bool IsWebProject(this MSBuildBasedProject project)
		{
			return project.HasProjectType(WebApplication) || project.HasProjectType(WebSite);
		}
		
		public static void AddImportIfMissing(
			this MSBuildBasedProject project,
			string importedProjectFile,
			ProjectImportLocation importLocation)
		{
			lock (project.SyncRoot) {
				if (project.ImportExists(importedProjectFile))
					return;
				
				ProjectImportElement import = AddImport(project.MSBuildProjectFile, importedProjectFile, importLocation);
				import.Condition = GetCondition(importedProjectFile);
			}
		}
		
		static ProjectImportElement AddImport(
			ProjectRootElement projectRoot,
			string importedProjectFile,
			ProjectImportLocation importLocation)
		{
			if (importLocation == ProjectImportLocation.Top) {
				return AddImportAtTop(projectRoot, importedProjectFile);
			}
			return projectRoot.AddImport(importedProjectFile);
		}
		
		static ProjectImportElement AddImportAtTop(ProjectRootElement projectRoot, string importedProjectFile)
		{
			ProjectImportElement import = projectRoot.CreateImportElement(importedProjectFile);
			projectRoot.InsertBeforeChild(import, projectRoot.FirstChild);
			return import;
		}
		
		static string GetCondition(string importedProjectFile)
		{
			return String.Format("Exists('{0}')", importedProjectFile);
		}
		
		public static bool ImportExists(this MSBuildBasedProject project, string importedProjectFile)
		{
			lock (project.SyncRoot) {
				return project.FindImport(importedProjectFile) != null;
			}
		}
		
		public static void RemoveImport(this MSBuildBasedProject project, string importedProjectFile)
		{
			lock (project.SyncRoot) {
				ProjectImportElement import = project.FindImport(importedProjectFile);
				if (import != null) {
					import.Parent.RemoveChild(import);
				}
			}
		}
		
		static ProjectImportElement FindImport(this MSBuildBasedProject project, string importedProjectFile)
		{
			return project.MSBuildProjectFile.FindImport(importedProjectFile);
		}
	}
}
