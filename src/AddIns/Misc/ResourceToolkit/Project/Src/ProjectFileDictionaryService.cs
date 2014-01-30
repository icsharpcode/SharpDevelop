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
