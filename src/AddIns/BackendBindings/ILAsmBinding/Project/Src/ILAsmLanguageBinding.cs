// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.ILAsmBinding
{
	public class ILAsmLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "ILAsm";
		
		public string Language {
			get {
				return LanguageName;
			}
		}
		
		public IProject LoadProject(IMSBuildEngineProvider provider, string fileName, string projectName)
		{
			return new ILAsmProject(provider, fileName, projectName);
		}
		
		public IProject CreateProject(ProjectCreateInformation info)
		{
			return new ILAsmProject(info);
		}
	}
}
