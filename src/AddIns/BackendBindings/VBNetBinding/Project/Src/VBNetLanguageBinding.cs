// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace VBNetBinding
{
	public class VBNetLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "VBNet";
		
		public string Language
		{
			get {
				return LanguageName;
			}
		}
		
		public IProject LoadProject(string fileName, string projectName)
		{
			return new VBNetProject(fileName, projectName);
		}
		
		public IProject CreateProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			VBNetProject p = new VBNetProject(info);
			if (projectOptions != null) {
				p.ImportOptions(projectOptions.Attributes);
			}
			return p;
		}
	}
}
