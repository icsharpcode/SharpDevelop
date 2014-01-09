// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// The <c>IProjectBinding</c> interface is implemented by language binding AddIns
	/// in order to support project creation/loading.
	/// </summary>
	/// <seealso cref="ProjectBindingDoozer"/>
	public interface IProjectBinding
	{
		/// <summary>
		/// Loads a project from disk.
		/// </summary>
		/// <exception cref="ProjectLoadException">Invalid project file (or other error)</exception>
		/// <exception cref="System.IO.IOException">Error reading from the project file</exception>
		IProject LoadProject(ProjectLoadInformation info);
		
		/// <summary>
		/// Creates a IProject out of the given ProjetCreateInformation object.
		/// Each project binding must provide a representation of the project
		/// it 'controls'.
		/// </summary>
		/// <exception cref="ProjectLoadException">Invalid project file (or other error)</exception>
		/// <exception cref="System.IO.IOException">Error writing new project to disk</exception>
		IProject CreateProject(ProjectCreateInformation info);
		
		/// <summary>
		/// Determines whether this ProjectBinding handling missing project file
		/// itself or it relies on the default logic of creating MissingProject project
		/// </summary>
		bool HandlingMissingProject { get; }
	}
}
