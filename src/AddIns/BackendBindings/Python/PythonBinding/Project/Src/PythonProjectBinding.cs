// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Language binding for Python.
	/// </summary>
	public class PythonProjectBinding : IProjectBinding
	{
		public const string LanguageName = "Python";
		
		public PythonProjectBinding()
		{
		}
		
		public string Language {
			get { return LanguageName; }
		}
		
		public IProject LoadProject(ProjectLoadInformation loadInformation)
		{
			return new PythonProject(loadInformation);
		}
		
		public IProject CreateProject(ProjectCreateInformation info)
		{
			return new PythonProject(info);
		}
		
		public bool HandlingMissingProject {
			get { 
				return false; 
			}
		}
	}
}
