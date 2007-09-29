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
		[Browsable(false)]
		public override IAmbience Ambience {
			get {
				return CSharpAmbience.Instance;
			}
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
		
		protected override void Create(ProjectCreateInformation information)
		{
			base.Create(information);
			this.AddImport(DefaultTargetsFile, null);
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
			if (this.MinimumSolutionVersion == 9) {
				MSBuildEngine.StartBuild(this,
				                         options,
				                         feedbackSink,
				                         MSBuildEngine.AdditionalTargetFiles.Concat(
				                         	new [] { "$(SharpDevelopBinPath)/SharpDevelop.CheckMSBuild35Features.targets" }));
			} else {
				base.StartBuild(options, feedbackSink);
			}
		}
	}
}
