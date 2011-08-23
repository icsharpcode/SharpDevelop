// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Project;
using System;

namespace FSharpBinding
{
	public class FSharpProjectBinding : IProjectBinding
	{
		public string Language {
			get { return "F#"; }
		}
		
		public IProject LoadProject(ProjectLoadInformation info)
		{
			return new FSharpProject(info);
		}
		
		public IProject CreateProject(ICSharpCode.SharpDevelop.Internal.Templates.ProjectCreateInformation info)
		{
			return new FSharpProject(info);
		}
		
		public bool HandlingMissingProject {
			get { 
				return false; 
			}
		}
	}
}
