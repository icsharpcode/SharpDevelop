// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace Grunwald.BooBinding
{
	public class BooLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "Boo";
		
		public string Language {
			get {
				return LanguageName;
			}
		}
		
		public IProject LoadProject(IMSBuildEngineProvider provider, string fileName, string projectName)
		{
			return new BooProject(provider, fileName, projectName);
		}
		
		public IProject CreateProject(ProjectCreateInformation info)
		{
			return new BooProject(info);
		}
		
		public LanguageProperties LanguageProperties {
			get {
				return BooLanguageProperties.Instance;
			}
		}
	}
}
