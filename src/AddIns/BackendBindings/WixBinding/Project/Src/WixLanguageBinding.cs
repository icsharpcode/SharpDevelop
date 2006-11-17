// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	public class WixLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "Wix";
		
		public string Language {
			get {
				return LanguageName;
			}
		}
		
		public IProject LoadProject(IMSBuildEngineProvider provider, string fileName, string projectName)
		{
			return new WixProject(provider, fileName, projectName);
		}
		
		public IProject CreateProject(ProjectCreateInformation info)
		{
			return new WixProject(info);
		}
	}
}
