// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using ICSharpCode.SharpDevelop.Dom;
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
			LanguageProperties = BooLanguageProperties.Instance;
			SetupProject(fileName);
			IdGuid = BaseConfiguration["ProjectGuid"];
		}
		
		public BooProject(ProjectCreateInformation info)
		{
			Language = "Boo";
			LanguageProperties = BooLanguageProperties.Instance;
			Create(info);
			imports.Add("$(BooBinPath)\\MsBuild.Boo.Targets");
		}
		
		public override bool CanCompile(string fileName)
		{
			return new BooLanguageBinding().CanCompile(fileName);
		}
		
		public override ParseProjectContent CreateProjectContent()
		{
			ParseProjectContent pc = base.CreateProjectContent();
			ReferenceProjectItem systemItem = new ReferenceProjectItem(this, "System");
			pc.ReferencedContents.Add(ProjectContentRegistry.GetProjectContentForReference(systemItem));
			ReferenceProjectItem booLangItem = new ReferenceProjectItem(this, typeof(Boo.Lang.Builtins).Assembly.Location);
			pc.ReferencedContents.Add(ProjectContentRegistry.GetProjectContentForReference(booLangItem));
			pc.DefaultImports = new DefaultUsing(pc);
			pc.DefaultImports.Usings.Add("Boo.Lang.Builtins");
			return pc;
		}
		
		[Browsable(false)]
		public override IAmbience Ambience {
			get {
				return BooAmbience.Instance;
			}
		}
	}
}
