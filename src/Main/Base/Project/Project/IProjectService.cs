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
using System.ComponentModel;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project.Converter;
using ICSharpCode.SharpDevelop.Project.PortableLibrary;

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
		/// This event is raised after a solution is opened.
		/// </summary>
		event EventHandler<SolutionEventArgs> SolutionOpened;
		
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
		bool OpenSolutionOrProject(FileName fileName);
		
		/// <summary>
		/// Opens a solution in the IDE.
		/// </summary>
		bool OpenSolution(FileName fileName);
		
		/// <summary>
		/// Opens a solution in the IDE that was created/loaded earlier.
		/// </summary>
		/// <remarks>
		/// The solution must be saved to disk before it can be opened.
		/// </remarks>
		bool OpenSolution(ISolution solution);
		
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
		/// Returns if the given file is considered a solution or project file.
		/// This method looks at the list of registered file extensions in /SharpDevelop/Workbench/ProjectBinding.
		/// </summary>
		/// <remarks>
		/// This method is thread-safe.
		/// </remarks>
		bool IsSolutionOrProjectFile(FileName fileName);
		
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
		
		/// <summary>
		/// Is raised when a new project is created.
		/// </summary>
		event EventHandler<ProjectEventArgs> ProjectCreated;
		
		event EventHandler<SolutionEventArgs> SolutionCreated;
		
		event EventHandler<ProjectItemEventArgs> ProjectItemAdded;
		event EventHandler<ProjectItemEventArgs> ProjectItemRemoved;
		
		/// <summary>
		/// Gets the list of registered .NET target frameworks.
		/// This is the list that is supposed to be displayed to the user when selecting a target framework.
		/// It corresponds to the AddInTree path '/SharpDevelop/TargetFrameworks'.
		/// 
		/// Note that in the project upgrade view, <see cref="IUpgradableProject.GetAvailableTargetFrameworks"/> is used instead.
		/// Some target framework types (such as 'portable library') might be added by project behaviors, and are not available in this list.
		/// </summary>
		IReadOnlyList<TargetFramework> TargetFrameworks { get; }
		
		/// <summary>
		/// Gets the list of registered project bindings.
		/// </summary>
		IReadOnlyList<ProjectBindingDescriptor> ProjectBindings { get; }
		
		/// <summary>
		/// Loads a project from disk without opening it in the IDE.
		/// </summary>
		/// <exception cref="ProjectLoadException">Invalid project file (or other error)</exception>
		/// <exception cref="System.IO.IOException">Error reading from the project file</exception>
		/// <remarks>
		/// The TypeGuid will be used to identity the project binding used for loading the project.
		/// If the TypeGuid provided with the ProjectLoadInformation is all zeroes, the project file extension
		/// will be used instead.
		/// </remarks>
		IProject LoadProject(ProjectLoadInformation info);
	}
	
	public interface IProjectServiceRaiseEvents
	{
		void RaiseProjectCreated(ProjectEventArgs e);
		void RaiseSolutionCreated(SolutionEventArgs e);
		
		void RaiseProjectItemAdded(ProjectItemEventArgs e);
		void RaiseProjectItemRemoved(ProjectItemEventArgs e);
	}
}
