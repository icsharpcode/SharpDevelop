﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		/// <remarks>
		/// This property is thread-safe.
		/// </remarks>
		ISolution CurrentSolution { get; }
		
		event PropertyChangedEventHandler<ISolution> CurrentSolutionChanged;
		
		/// <summary>
		/// This event is raised before a solution is closed.
		/// </summary>
		event EventHandler<SolutionClosingEventArgs> SolutionClosing;
		
		/// <summary>
		/// This event is raised after a solution is closed.
		/// </summary>
		event EventHandler<SolutionEventArgs> SolutionClosed;
		
		/// <summary>
		/// Gets/Sets the project that is currently considered 'active' within the IDE.
		/// </summary>
		/// <remarks>
		/// The getter is thread-safe; the setter may only be called on the main thread.
		/// </remarks>
		IProject CurrentProject { get; set; }
		
		event PropertyChangedEventHandler<IProject> CurrentProjectChanged;
		
		/// <summary>
		/// A collection that contains all projects in the currently open solution.
		/// 
		/// The collection instance is reused when the solution is closed and another is opened.
		/// </summary>
		IModelCollection<IProject> AllProjects { get; }
		
		/// <summary>
		/// Finds the project that contains the specified file.
		/// Returns null if none of the open projects contains the file.
		/// </summary>
		/// <remarks>
		/// If multiple projects contain the file, any one of them is returned.
		/// This method is thread-safe.
		/// </remarks>
		IProject FindProjectContainingFile(FileName fileName);
		
		/// <summary>
		/// If the given filename is a solution file (.sln), it is loaded and opened in the IDE.
		/// Otherwise, SharpDevelop looks for a .sln file corresponding to the project file, and opens that instead.
		/// If no such .sln file is found, SharpDevelop will create one.
		/// </summary>
		/// <remarks>
		/// This method may only be called on the main thread.
		/// If any errors occur, this method may display an error dialog.
		/// </remarks>
		void OpenSolutionOrProject(FileName fileName);
		
		/// <summary>
		/// Closes the solution: cancels build, clears solution data, fires the SolutionClosing and SolutionClosed events.
		/// </summary>
		/// <param name="allowCancel">Whether to allow the user to cancel closing the solution.</param>
		/// <returns>
		/// True if the solution was closed successfully; false if the operation was aborted.
		/// </returns>
		/// <remarks>
		/// This method may only be called on the main thread.
		/// </remarks>
		bool CloseSolution(bool allowCancel = true);
		
		/// <summary>
		/// Returns if the given file is considered a project or solution file.
		/// This method looks at the list of registered file extensions in /SharpDevelop/Workbench/ProjectBinding.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		bool IsProjectOrSolutionFile(FileName fileName);
		
		/// <summary>
		/// Loads a solution file without opening it in the IDE.
		/// </summary>
		/// <exception cref="ProjectLoadException">
		/// The .sln file is malformed or an unsupported version, and cannot be loaded.
		/// This exception does not occur if only individual projects within the solution are invalid.
		/// </exception>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		ISolution LoadSolutionFile(FileName fileName, IProgressMonitor progress);
		
		/// <summary>
		/// Creates a new, empty solution without opening it in the IDE.
		/// The file is not saved to disk until <see cref="ISolution.Save"/> is called.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		ISolution CreateEmptySolutionFile(FileName fileName);
	}
}
