// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.RubyBinding
{
	public class RubyProjectBinding : IProjectBinding
	{
		public const string LanguageName = "Ruby";
		
		public RubyProjectBinding()
		{
		}
		
		public string Language {
			get { return LanguageName; }
		}
		
		public IProject LoadProject(ProjectLoadInformation loadInformation)
		{
			return new RubyProject(loadInformation);
		}
		
		public IProject CreateProject(ProjectCreateInformation info)
		{
			return new RubyProject(info);
		}
		
		public bool HandlingMissingProject {
			get { 
				return false; 
			}
		}
	}
}
