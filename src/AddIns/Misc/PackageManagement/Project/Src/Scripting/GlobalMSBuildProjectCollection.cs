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

using ICSharpCode.SharpDevelop;
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
				SD.MainThread.InvokeIfRequired(() => UpdateProject(msbuildProjects));
				GetGlobalProjectCollection().UnloadProject(msbuildProjects.GlobalMSBuildProject);
			}
		}
		
		void UpdateProject(GlobalAndInternalProject msbuildProjects)
		{
			UpdateImports(msbuildProjects);
			UpdateProperties(msbuildProjects);
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
