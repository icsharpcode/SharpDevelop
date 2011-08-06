// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public interface ISelectedMvcViewFolder
	{
		string Path { get; }
		IProject Project { get; }
		
		/// <summary>
		/// Adds the file to the view folder.
		/// </summary>
		/// <param name="fileName">The filename should not have any path information</param>
		void AddFileToProject(string fileName);
	}
}
