// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Interface called to load (convert) project and solution files
	/// </summary>
	public interface IProjectLoader
	{
		/// <summary>
		/// Load/Convert the project solution
		/// </summary>
		void Load(string fileName);
	}
	
	/// <summary>
	/// Loader for MSBuild project files
	/// </summary>
	public class LoadProject : IProjectLoader
	{
		public void Load(string fileName)
		{
			ProjectService.LoadProject(fileName);
		}
	}

	/// <summary>
	/// Loader for sln files as well as Sharpdevelop cmbx and prjx files
	/// </summary>
	public class LoadSolution : IProjectLoader
	{
		public void Load(string fileName)
		{
			ProjectService.LoadSolution(fileName);
		}
	}
}
