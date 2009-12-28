// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.RubyBinding
{
	public class RubyLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "Ruby";
		
		public RubyLanguageBinding()
		{
		}
		
		public string Language {
			get { return LanguageName; }
		}
		
		public IProject LoadProject(IMSBuildEngineProvider engineProvider, string fileName, string projectName)
		{
			return new RubyProject(engineProvider, fileName, projectName);
		}
		
		public IProject CreateProject(ProjectCreateInformation info)
		{
			return new RubyProject(info);
		}		
	}
}
