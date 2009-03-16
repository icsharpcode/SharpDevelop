// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Linq;
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
						            "MyType", "WindowsForms", e.NewLocation, true);
						break;
					case OutputType.Exe:
						SetProperty(e.Configuration, e.Platform,
						            "MyType", "Console", e.NewLocation, true);
						break;
					default:
						SetProperty(e.Configuration, e.Platform,
						            "MyType", "Windows", e.NewLocation, true);
						break;
				}
			}
		}
		
		public override IAmbience GetAmbience()
		{
			return new VBNetAmbience();
		}
		
		public VBNetProject(IMSBuildEngineProvider provider, string fileName, string projectName)
			: base(provider)
		{
			this.Name = projectName;
			InitVB();
			LoadProject(fileName);
		}
		
		public const string DefaultTargetsFile = @"$(MSBuildBinPath)\Microsoft.VisualBasic.Targets";
		public const string ExtendedTargetsFile = @"$(SharpDevelopBinPath)\SharpDevelop.Build.VisualBasic.targets";
		
		public VBNetProject(ProjectCreateInformation info)
			: base(info.Solution)
		{
			InitVB();
			
			this.AddImport(DefaultTargetsFile, null);
			
			// Add import before Create call - base.Create will call AddOrRemoveExtensions, which
			// needs to change the import when the compact framework is targeted.
			Create(info);
			
			SetProperty("Debug", null, "DefineConstants", "DEBUG=1,TRACE=1",
			            PropertyStorageLocations.ConfigurationSpecific, true);
			SetProperty("Release", null, "DefineConstants", "TRACE=1",
			            PropertyStorageLocations.ConfigurationSpecific, true);
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
		
		public override void StartBuild(ProjectBuildOptions options, IBuildFeedbackSink feedbackSink)
		{
			if (this.MinimumSolutionVersion == Solution.SolutionVersionVS2005) {
				MSBuildEngine.StartBuild(this,
				                         options,
				                         feedbackSink,
				                         MSBuildEngine.AdditionalTargetFiles.Concat(
				                         	new [] { "$(SharpDevelopBinPath)/SharpDevelop.CheckMSBuild35Features.targets" }));
			} else {
				base.StartBuild(options, feedbackSink);
			}
		}
		
		protected override void AddOrRemoveExtensions()
		{
			// Test if SharpDevelop-Build extensions are required
			bool needExtensions = false;
			
			foreach (var p in GetAllProperties("TargetFrameworkVersion")) {
				if (p.IsImported == false) {
					if (p.Value.StartsWith("CF")) {
						needExtensions = true;
					}
				}
			}
			
			foreach (Microsoft.Build.BuildEngine.Import import in MSBuildProject.Imports) {
				if (needExtensions) {
					if (DefaultTargetsFile.Equals(import.ProjectPath, StringComparison.OrdinalIgnoreCase)) {
						//import.ProjectPath = extendedTargets;
						MSBuildInternals.SetImportProjectPath(this, import, ExtendedTargetsFile);
						// Workaround for SD2-1490. It would be better if the project browser could refresh itself
						// when necessary.
						ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
						break;
					}
				} else {
					if (ExtendedTargetsFile.Equals(import.ProjectPath, StringComparison.OrdinalIgnoreCase)) {
						//import.ProjectPath = defaultTargets;
						MSBuildInternals.SetImportProjectPath(this, import, DefaultTargetsFile);
						// Workaround for SD2-1490. It would be better if the project browser could refresh itself
						// when necessary.
						ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
						break;
					}
				}
			}
		}
	}
}
