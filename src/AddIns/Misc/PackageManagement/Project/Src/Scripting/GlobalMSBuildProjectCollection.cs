// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Evaluation;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class GlobalMSBuildProjectCollection : IGlobalMSBuildProjectCollection
	{
		class  GlobalAndInternalProject
		{
			public Project GlobalMSBuildProject;
			public MSBuildBasedProject SharpDevelopMSBuildProject;
			public int GlobalMSBuildProjectImportsCount;
			
			public bool HasGlobalMSBuildProjectImportsChanged()
			{
				return GlobalMSBuildProjectImportsCount != GlobalMSBuildProject.Xml.Imports.Count;
			}
		}
		
		List<GlobalAndInternalProject> projects = new List<GlobalAndInternalProject>();
		
		PackageManagementLogger logger = new PackageManagementLogger(
			new ThreadSafePackageManagementEvents(PackageManagementServices.PackageManagementEvents));
		
		public void AddProject(IPackageManagementProject packageManagementProject)
		{
			AddProject(packageManagementProject.ConvertToDTEProject().MSBuildProject);
		}
		
		void AddProject(MSBuildBasedProject sharpDevelopProject)
		{
			Project globalProject = GetGlobalProjectCollection().LoadProject(sharpDevelopProject.FileName);
			
			projects.Add(new GlobalAndInternalProject {
				GlobalMSBuildProject = globalProject,
				SharpDevelopMSBuildProject = sharpDevelopProject,
				GlobalMSBuildProjectImportsCount = globalProject.Xml.Imports.Count
			});
		}
		
		ProjectCollection GetGlobalProjectCollection()
		{
			return ProjectCollection.GlobalProjectCollection;
		}
		
		public void Dispose()
		{
			foreach (GlobalAndInternalProject msbuildProjects in projects) {
				UpdateImports(msbuildProjects);
				UpdateProperties(msbuildProjects);
				GetGlobalProjectCollection().UnloadProject(msbuildProjects.GlobalMSBuildProject);
			}
		}
		
		void UpdateImports(GlobalAndInternalProject msbuildProjects)
		{
			if (!msbuildProjects.HasGlobalMSBuildProjectImportsChanged()) {
				return;
			}
			
			LogProjectImportsChanged(msbuildProjects.SharpDevelopMSBuildProject);
			
			var importsMerger = new MSBuildProjectImportsMerger(
				msbuildProjects.GlobalMSBuildProject,
				msbuildProjects.SharpDevelopMSBuildProject);
			
			importsMerger.Merge();
			
			LogProjectImportMergeResult(msbuildProjects.SharpDevelopMSBuildProject, importsMerger.Result);
		}
		
		void LogProjectImportsChanged(MSBuildBasedProject project)
		{
			logger.Log(
				MessageLevel.Info,
				"Project imports have been modified outside SharpDevelop for project '{0}'.",
				project.Name);
		}
		
		void LogProjectImportMergeResult(MSBuildBasedProject project, MSBuildProjectImportsMergeResult result)
		{
			logger.Log(
				MessageLevel.Info,
				"Project import merge result for project '{0}':\r\n{1}",
				project.Name,
				result);
		}
		
		void UpdateProperties(GlobalAndInternalProject msbuildProjects)
		{
			var propertiesMerger = new MSBuildProjectPropertiesMerger(
				msbuildProjects.GlobalMSBuildProject,
				msbuildProjects.SharpDevelopMSBuildProject);
			
			propertiesMerger.Merge();
			
			if (propertiesMerger.Result.AnyPropertiesChanged())
			{
				LogProjectPropertiesMergeResult(msbuildProjects.SharpDevelopMSBuildProject, propertiesMerger.Result);
			}
		}
		
		void LogProjectPropertiesMergeResult(MSBuildBasedProject project, MSBuildProjectPropertiesMergeResult result)
		{
			logger.Log(
				MessageLevel.Info,
				"Project properties merge result for project '{0}':\r\n{1}",
				project.Name,
				result);
		}
	}
}
