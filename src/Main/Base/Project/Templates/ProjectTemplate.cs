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
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Templates
{
	public abstract class ProjectTemplate : TemplateBase
	{
		/// <summary>
		/// Gets whether this template is available within the specified solution.
		/// </summary>
		/// <param name="solution">The solution to which the new project should be added.
		/// Can be <c>null</c> when creating a new solution.</param>
		public virtual bool IsVisible(ISolution solution)
		{
			return true;
		}
		
		public virtual IEnumerable<TargetFramework> SupportedTargetFrameworks {
			get { return Enumerable.Empty<TargetFramework>(); }
		}
		
		/// <summary>
		/// Creates projects from the template; and adds them to the solution specified in the parameter object.
		/// </summary>
		/// <param name="options">Parameter object used to pass options for the template creation.</param>
		/// <returns>
		/// Returns a result object that describes the projects that were created;
		/// or null if the operation was aborted.
		/// </returns>
		/// <exception cref="IOException">Error writing the projects to disk</exception>
		/// <exception cref="ProjectLoadException">Error creating the projects (e.g. a separate download is required for projects of this type [like the F# compiler])</exception>
		public abstract ProjectTemplateResult CreateProjects(ProjectTemplateOptions options);
		
		internal ProjectTemplateResult CreateAndOpenSolution(ProjectTemplateOptions options, string solutionDirectory, string solutionName)
		{
			FileName solutionFileName = FileName.Create(Path.Combine(solutionDirectory, solutionName + ".sln"));
			bool solutionOpened = false;
			ISolution createdSolution = SD.ProjectService.CreateEmptySolutionFile(solutionFileName);
			try {
				options.Solution = createdSolution;
				options.SolutionFolder = createdSolution;
				var result = CreateProjects(options);
				if (result == null) {
					return null;
				}
				createdSolution.Save(); // solution must be saved before it can be opened
				if (SD.ProjectService.OpenSolution(createdSolution)) {
					solutionOpened = true;
					SD.GetRequiredService<IProjectServiceRaiseEvents>().RaiseSolutionCreated(new SolutionEventArgs(createdSolution));
					return result;
				} else {
					return null;
				}
			} finally {
				if (!solutionOpened)
					createdSolution.Dispose();
			}
		}
		
		/// <summary>
		/// Runs the actions after the newly created solution is opened in the IDE.
		/// </summary>
		public virtual void RunOpenActions(ProjectTemplateResult result)
		{
		}
	}
}
