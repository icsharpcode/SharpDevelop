// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Project
{
	public enum SolutionFormatVersion // TODO: change IProject.MinimumSolutionFormatVersion to this enum type
	{
		VS2005 = 9,
		VS2008 = 10,
		VS2010 = 11,
		VS2012 = 12
	}
	
	/// <summary>
	/// Represents a solution.
	/// </summary>
	public interface ISolution : ISolutionFolder, ICanBeDirty, IConfigurable, IDisposable
	{
		Microsoft.Build.Evaluation.ProjectCollection MSBuildProjectCollection { get; }
		
		/// <summary>
		/// Gets the full path of the .sln file.
		/// </summary>
		FileName FileName { get; }
		
		event EventHandler FileNameChanged;
		
		/// <summary>
		/// Gets the full path of the directory containing the .sln file.
		/// </summary>
		string Directory { get; }
		
		/// <summary>
		/// Gets/Sets the startup project.
		/// </summary>
		IProject StartupProject { get; set; }
		
		event EventHandler StartupProjectChanged;
		
		/// <summary>
		/// Gets all projects in the solution.
		/// </summary>
		IModelCollection<IProject> Projects { get; }
		
		/// <summary>
		/// Loads an existing project from disk and adds it to this solution.
		/// </summary>
		/// <param name="fileName">Path to the project file</param>
		/// <param name="parentFolder">
		/// Optional: The parent folder to which the new project should be added.
		/// If this parameter is not specified, the project is added to the root folder of the solution.
		/// </param>
		void AddExistingProject(FileName fileName, ISolutionFolder parentFolder = null);
		
		/// <summary>
		/// Gets all file items in the solution.
		/// </summary>
		IModelCollection<ISolutionFileItem> FileItems { get; }
		
		/// <summary>
		/// Gets a container that can be used to store data about the solution.
		/// This data is stored in SharpDevelop's config directory, not directly with the .sln file.
		/// </summary>
		Properties Preferences { get; }
		
		/// <summary>
		/// Saves the preferences for this solution; and also for any projects within this solution.
		/// </summary>
		void SavePreferences();
		
		/// <summary>
		/// Gets whether the solution is read-only.
		/// </summary>
		bool IsReadOnly { get; }
		
		/// <summary>
		/// Saves the solution.
		/// This will also save all modified projects within this solution.
		/// </summary>
		void Save();
		
		
	}
}
