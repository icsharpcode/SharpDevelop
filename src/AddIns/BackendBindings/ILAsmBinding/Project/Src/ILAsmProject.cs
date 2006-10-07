// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
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
			this.Imports.Add(new MSBuildImport(@"$(SharpDevelopBinPath)\SharpDevelop.Build.MSIL.Targets"));
		}
		
		public override bool CanCompile(string fileName)
		{
			return string.Equals(".il", Path.GetExtension(fileName), StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
