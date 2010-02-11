// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
	}
}
