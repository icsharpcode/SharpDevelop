// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.IO;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.VBNet;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace VBNetBinding
{
	public class VBNetProject : CompilableProject
	{
		protected override void OnPropertyChanged(ProjectPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.PropertyName == "OutputType") {
				switch (this.OutputType) {
					case OutputType.WinExe:
						SetProperty(e.Configuration, e.Platform,
						            "MyType", "WindowsForms", e.Location, true);
						break;
					case OutputType.Exe:
						SetProperty(e.Configuration, e.Platform,
						            "MyType", "Console", e.Location, true);
						break;
					default:
						SetProperty(e.Configuration, e.Platform,
						            "MyType", "Windows", e.Location, true);
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
		
		public VBNetProject(IMSBuildEngineProvider provider, string fileName, string projectName)
			: base(provider)
		{
			this.Name = projectName;
			InitVB();
			LoadProject(fileName);
		}
		
		public const string DefaultTargetsFile = @"$(MSBuildBinPath)\Microsoft.VisualBasic.Targets";
		
		public VBNetProject(ProjectCreateInformation info)
			: base(info.Solution)
		{
			InitVB();
			Create(info);
			
			SetProperty("Debug", null, "DefineConstants", "DEBUG=1,TRACE=1",
			            PropertyStorageLocations.ConfigurationSpecific, true);
			SetProperty("Release", null, "DefineConstants", "TRACE=1",
			            PropertyStorageLocations.ConfigurationSpecific, true);
			
			this.MSBuildProject.AddNewImport(DefaultTargetsFile, null);
		}
		
		protected override ParseProjectContent CreateProjectContent()
		{
			ParseProjectContent pc = base.CreateProjectContent();
			ReferenceProjectItem vbRef = new ReferenceProjectItem(this, "Microsoft.VisualBasic");
			if (vbRef != null) {
				pc.AddReferencedContent(ParserService.GetProjectContentForReference(vbRef));
			}
			MyNamespaceBuilder.BuildNamespace(this, pc);
			return pc;
		}
		
		void InitVB()
		{
			reparseReferencesSensitiveProperties.Add("TargetFrameworkVersion");
			reparseCodeSensitiveProperties.Add("DefineConstants");
		}
		
		public override string Language {
			get { return VBNetLanguageBinding.LanguageName; }
		}
		
		public override LanguageProperties LanguageProperties {
			get { return LanguageProperties.VBNet; }
		}
		
		public override ItemType GetDefaultItemType(string fileName)
		{
			if (string.Equals(Path.GetExtension(fileName), ".vb", StringComparison.OrdinalIgnoreCase))
				return ItemType.Compile;
			else
				return base.GetDefaultItemType(fileName);
		}
	}
}
