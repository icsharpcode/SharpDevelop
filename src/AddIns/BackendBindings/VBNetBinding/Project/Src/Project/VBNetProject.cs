// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace VBNetBinding
{
	public class VBNetProject : MSBuildProject
	{
		public override void SetProperty<T>(string configurationName, string platform, string property, T value, PropertyStorageLocations location)
		{
			base.SetProperty(configurationName, platform, property, value, location);
			if (property == "OutputType") {
				switch (this.OutputType) {
					case OutputType.WinExe:
						base.SetProperty(configurationName, platform, "MyType", "WindowsForms", location);
						break;
					case OutputType.Exe:
						base.SetProperty(configurationName, platform, "MyType", "Console", location);
						break;
					default:
						base.SetProperty(configurationName, platform, "MyType", "Windows", location);
						break;
				}
			}
		}
		
		[Browsable(false)]
		public override IAmbience Ambience {
			get {
				return VBNetAmbience.Instance;
			}
		}
		
		public VBNetProject(string fileName, string projectName)
		{
			this.Name = projectName;
			InitVB();
			SetupProject(fileName);
			IdGuid = BaseConfiguration["ProjectGuid"];
		}
		
		public const string DefaultTargetsFile = @"$(MSBuildBinPath)\Microsoft.VisualBasic.Targets";
		
		public VBNetProject(ProjectCreateInformation info)
		{
			InitVB();
			Create(info);
			imports.Add(DefaultTargetsFile);
		}
		
		public override ParseProjectContent CreateProjectContent()
		{
			ParseProjectContent pc = base.CreateProjectContent();
			ReferenceProjectItem vbRef = new ReferenceProjectItem(this, "Microsoft.VisualBasic");
			if (vbRef != null) {
				pc.ReferencedContents.Add(ParserService.GetProjectContentForReference(vbRef));
			}
			MyNamespaceBuilder.BuildNamespace(this, pc);
			return pc;
		}
		
		void InitVB()
		{
			Language = "VBNet";
			LanguageProperties = ICSharpCode.SharpDevelop.Dom.LanguageProperties.VBNet;
			BuildConstantSeparator = ',';
		}
		
		public override bool CanCompile(string fileName)
		{
			return string.Equals(Path.GetExtension(fileName), ".vb", StringComparison.OrdinalIgnoreCase);
		}
	}
}
