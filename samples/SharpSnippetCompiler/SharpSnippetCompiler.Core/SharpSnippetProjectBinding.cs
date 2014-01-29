// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpSnippetCompiler.Core
{
	public class SharpSnippetProjectBinding : IProjectBinding
	{
		public string Language{
			get { return "C#"; }
		}
		
		public IProject LoadProject(ProjectLoadInformation loadInformation)
		{
			return new SnippetCompilerProject(loadInformation);
		}
		
		public IProject CreateProject(ProjectCreateInformation info)
		{
			return null;
		}
		
		public bool HandlingMissingProject { get; private set; }
	}
}
