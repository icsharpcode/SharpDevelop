// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// The <code>IProjectBinding</code> interface is the base interface
	/// of all project bindings avaiable.
	/// </summary>
	public interface IProjectBinding
	{
		/// <returns>
		/// The language for this project binding.
		/// </returns>
		string Language {
			get;
		}
		
		IProject LoadProject(ProjectLoadInformation info);
		
		/// <summary>
		/// Creates a IProject out of the given ProjetCreateInformation object.
		/// Each project binding must provide a representation of the project
		/// it 'controls'.
		/// </summary>
		IProject CreateProject(ProjectCreateInformation info);
		
		/// <summary>
		/// Determines whether this ProjectBinding handling missing project file 
		/// itself or it relies on the default logic of creating MissingProject project
		/// </summary>
		bool HandlingMissingProject { get; }
	}
}
