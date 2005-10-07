// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Core;

namespace Grunwald.BooBinding
{
	public class BooProject : MSBuildProject
	{
		public BooProject(string fileName, string projectName)
		{
			this.Name = projectName;
			Language = "Boo";
			SetupProject(fileName);
			IdGuid = BaseConfiguration["ProjectGuid"];
		}
		
		public BooProject(ProjectCreateInformation info)
		{
			Language = "Boo";
			Create(info);
			imports.Add("$(BooBinPath)\\MsBuild.Boo.Targets");
		}
		
		public override bool CanCompile(string fileName)
		{
			return new BooLanguageBinding().CanCompile(fileName);
		}
		
		[Browsable(false)]
		public override IAmbience Ambience {
			get {
				return BooAmbience.Instance;
			}
		}
	}
}
