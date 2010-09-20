// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
