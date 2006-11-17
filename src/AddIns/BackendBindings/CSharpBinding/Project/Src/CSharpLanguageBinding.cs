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

namespace CSharpBinding
{
	public class CSharpLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "C#";
		
		public string Language {
			get {
				return LanguageName;
			}
		}
		
		public IProject LoadProject(IMSBuildEngineProvider engineProvider, string fileName, string projectName)
		{
			return new CSharpProject(engineProvider, fileName, projectName);
		}
		
		public IProject CreateProject(ProjectCreateInformation info)
		{
			return new CSharpProject(info);
		}
	}
}
