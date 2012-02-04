// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public interface ISelectedMvcFolder
	{
		string Path { get; }
		IMvcProject Project { get; }
		
		/// <summary>
		/// Adds the file to the folder.
		/// </summary>
		/// <param name="fileName">The filename should not have any path information</param>
		void AddFileToProject(string fileName);
		
		MvcTextTemplateLanguage GetTemplateLanguage();
	}
}
