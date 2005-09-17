// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.ILAsmBinding
{
	public class ILAsmProject : MSBuildProject
	{
		public ILAsmProject(string fileName, string projectName)
		{
			this.Name = projectName;
			Language = "ILAsm";
			SetupProject(fileName);
			IdGuid = BaseConfiguration["ProjectGuid"];
		}
		
		public ILAsmProject(ProjectCreateInformation info)
		{
			Language = "ILAsm";
			Create(info);
			imports.Add(@"$(SharpDevelopBinPath)\SharpDevelop.Build.MSIL.Targets");
		}
		
		public override bool CanCompile(string fileName)
		{
			return new ILAsmLanguageBinding().CanCompile(fileName);
		}
	}
}
