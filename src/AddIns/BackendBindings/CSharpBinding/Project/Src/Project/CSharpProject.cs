// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Linq;
using System.IO;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace CSharpBinding
{
	/// <summary>
	/// Description of CSharpProject.
	/// </summary>
	public class CSharpProject : CompilableProject
	{
		public override IAmbience GetAmbience()
		{
			return new CSharpAmbience();
		}
		
		public override string Language {
			get { return CSharpLanguageBinding.LanguageName; }
		}
		
		public override LanguageProperties LanguageProperties {
			get { return LanguageProperties.CSharp; }
		}
		
		void Init()
		{
			reparseReferencesSensitiveProperties.Add("TargetFrameworkVersion");
			reparseCodeSensitiveProperties.Add("DefineConstants");
		}
		
		public CSharpProject(IMSBuildEngineProvider engineProvider, string fileName, string projectName)
			: base(engineProvider)
		{
			this.Name = projectName;
			Init();
			LoadProject(fileName);
		}
		
		public CSharpProject(ProjectCreateInformation info)
			: base(info.Solution)
		{
			Init();
			Create(info);
		}
		
		public const string DefaultTargetsFile = @"$(MSBuildBinPath)\Microsoft.CSharp.Targets";
		public const string ExtendedTargetsFile = @"$(SharpDevelopBinPath)\SharpDevelop.Build.CSharp.targets";
		
		protected override void Create(ProjectCreateInformation information)
		{
			this.AddImport(DefaultTargetsFile, null);
			
			// Add import before base.Create call - base.Create will call AddOrRemoveExtensions, which
			// needs to change the import when the compact framework is targeted.
			base.Create(information);
			
			SetProperty("Debug", null, "CheckForOverflowUnderflow", "True",
			            PropertyStorageLocations.ConfigurationSpecific, true);
			SetProperty("Release", null, "CheckForOverflowUnderflow", "False",
			            PropertyStorageLocations.ConfigurationSpecific, true);
			
			SetProperty("Debug", null, "DefineConstants", "DEBUG;TRACE",
			            PropertyStorageLocations.ConfigurationSpecific, false);
			SetProperty("Release", null, "DefineConstants", "TRACE",
			            PropertyStorageLocations.ConfigurationSpecific, false);
		}
		
		public override ItemType GetDefaultItemType(string fileName)
		{
			if (string.Equals(Path.GetExtension(fileName), ".cs", StringComparison.OrdinalIgnoreCase))
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
