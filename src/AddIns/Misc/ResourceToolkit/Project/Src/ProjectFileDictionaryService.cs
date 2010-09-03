// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace Hornung.ResourceToolkit
{
	/// <summary>
	/// Provides a way to find out which project a certain file belongs to
	/// and caches the results to improve performance.
	/// </summary>
	public static class ProjectFileDictionaryService
	{
		static Dictionary<string, IProject> files = new Dictionary<string, IProject>();
		
		static ProjectFileDictionaryService()
		{
			// Remove file from dictionary when file is removed from project
			ProjectService.ProjectItemRemoved += delegate(object sender, ProjectItemEventArgs e) {
				if (e.ProjectItem != null && e.ProjectItem.FileName != null) {
					files.Remove(e.ProjectItem.FileName);
				}
			};
			// Clear cache when solution is closed
			ProjectService.SolutionClosed += delegate { files.Clear(); };
		}
		
		/// <summary>
		/// Gets the project that contains the specified file.
		/// </summary>
		/// <param name="fileName">The file to find the project for.</param>
		/// <returns>The project that contains the specified file. If the file is not found in any project or there is no open project, the current project is returned, which may be <c>null</c>.</returns>
		public static IProject GetProjectForFile(string fileName)
		{
			if (!String.IsNullOrEmpty(fileName)) {
				
				IProject p;
				
				if (files.TryGetValue(fileName, out p)) {
					return p;
				}
				
				if ((p = GetProjectForFileInternal(fileName)) != null) {
					files[fileName] = p;
					return p;
				}
				
			}
			
			#if DEBUG
			LoggingService.Debug("ResourceToolkit: ProjectFileDictionary: Could not determine project for file '"+(fileName ?? "<null>")+"'.");
			#endif
			
			return ProjectService.CurrentProject;
		}
		
		/// <summary>
		/// Gets the project that contains the specified file.
		/// </summary>
		/// <param name="fileName">The file to find the project for.</param>
		/// <returns>The project that contains the specified file. If the file is not found in any project or there is no open project, <c>null</c> is returned.</returns>
		static IProject GetProjectForFileInternal(string fileName)
		{
			if (ProjectService.OpenSolution != null) {
				IProject p;
				if ((p = ProjectService.OpenSolution.FindProjectContainingFile(fileName)) != null) {
					return p;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Adds a file name to the project dictionary or sets the file's project if
		/// it is already listed.
		/// </summary>
		/// <param name="file">The file to add.</param>
		/// <param name="project">The project the specified file belongs to.</param>
		public static void AddFile(string file, IProject project)
		{
			files[file] = project;
		}
	}
}
