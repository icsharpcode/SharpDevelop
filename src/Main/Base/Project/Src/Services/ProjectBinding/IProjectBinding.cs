// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
	}
}
