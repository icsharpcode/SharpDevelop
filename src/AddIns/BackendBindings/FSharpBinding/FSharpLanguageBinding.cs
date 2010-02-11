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
	public class FSharpLanguageBinding : ILanguageBinding
	{
		public string Language {
			get { return "F#"; }
		}
		
		public IProject LoadProject(IMSBuildEngineProvider engineProvider, string fileName, string projectName)
		{
			return new FSharpProject(engineProvider, fileName, projectName);
		}
		
		public IProject CreateProject(ICSharpCode.SharpDevelop.Internal.Templates.ProjectCreateInformation info)
		{
			return new FSharpProject(info);
		}
	}
}
