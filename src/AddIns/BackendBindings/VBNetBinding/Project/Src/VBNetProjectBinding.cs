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

namespace ICSharpCode.VBNetBinding
{
	public class VBNetProjectBinding : IProjectBinding
	{
		public const string LanguageName = "VBNet";
		
		public string Language
		{
			get {
				return LanguageName;
			}
		}
		
		public IProject LoadProject(ProjectLoadInformation info)
		{
			return new VBNetProject(info);
		}
		
		public IProject CreateProject(ProjectCreateInformation info)
		{
			return new VBNetProject(info);
		}
	}
}
