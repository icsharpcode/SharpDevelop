// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Deals with loading projects and solutions.
	/// </summary>
	[SDService("SD.ProjectService")]
	public interface IProjectService
	{
		/// <summary>
		/// Gets the solution that is currently opened within the IDE.
		/// </summary>
		ISolution OpenSolution { get; }
		
		event EventHandler OpenSolutionChanged;
		
		/// <summary>
		/// Gets/Sets the project that is currently considered 'active' within the IDE.
		/// </summary>
		IProject CurrentProject { get; set; }
		
		event EventHandler CurrentProjectChanged;
		
		/// <summary>
		/// Gets the list of projects that are currently opened within the IDE.
		/// </summary>
		IModelCollection<IProject> Projects { get; }
		
		/// <summary>
		/// Finds the project that contains the specified file.
		/// Returns null if none of the open projects contains the file.
		/// </summary>
		/// <remarks>
		/// If multiple projects contain the file, any one of them is returned.
		/// </remarks>
		IProject FindProjectContainingFile(FileName fileName);
		
		/// <summary>
		/// If the given filename is a solution file (.sln), it is loaded and opened in the IDE.
		/// Otherwise, SharpDevelop looks for a .sln file corresponding to the project file, and opens that instead.
		/// If no such .sln file is found, SharpDevelop will create one.
		/// </summary>
		/// <remarks>
		/// If any errors occur, this method may display an error dialog.
		/// </remarks>
		void OpenSolutionOrProject(FileName fileName);
		
		/// <summary>
		/// Closes the currently open solution.
		/// </summary>
		void CloseSolution();
		
		/// <summary>
		/// Returns if the given file is considered a project or solution file.
		/// This method looks at the list of registered file extensions in /SharpDevelop/Workbench/ProjectBinding.
		/// </summary>
		bool IsProjectOrSolutionFile(FileName fileName);
		
		/// <summary>
		/// Loads a solution file without opening it in the IDE.
		/// </summary>
		/// <exception cref="ProjectLoadException">
		/// The .sln file is malformed or an unsupported version, and cannot be loaded.
		/// This exception does not occur if only individual projects within the solution are invalid.
		/// </exception>
		ISolution LoadSolutionFile(FileName fileName);
		
		/// <summary>
		/// Creates a new, empty solution and loads it without opening it in the IDE.
		/// The file is not saved to disk until <see cref="ISolution.Save"/> is called.
		/// </summary>
		ISolution CreateEmptySolutionFile(FileName fileName);
	}
}
