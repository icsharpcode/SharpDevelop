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
