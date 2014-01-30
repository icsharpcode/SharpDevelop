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
using System.Linq;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Construction;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public static class MSBuildBasedProjectExtensions
	{
		public static bool IsWebProject(this MSBuildBasedProject project)
		{
			return project.HasProjectType(ProjectTypeGuids.WebApplication) || project.HasProjectType(ProjectTypeGuids.WebSite);
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
