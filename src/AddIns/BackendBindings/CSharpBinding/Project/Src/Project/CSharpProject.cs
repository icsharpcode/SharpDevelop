// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Converter;

namespace CSharpBinding
{
	/// <summary>
	/// IProject implementation for .csproj files.
	/// </summary>
	public class CSharpProject : CompilableProject
	{
		public override IAmbience GetAmbience()
		{
			return new CSharpAmbience();
		}
		
		public override string Language {
			get { return CSharpProjectBinding.LanguageName; }
		}
		
		public override LanguageProperties LanguageProperties {
			get { return LanguageProperties.CSharp; }
		}
		
		void Init()
		{
			reparseReferencesSensitiveProperties.Add("TargetFrameworkVersion");
			reparseCodeSensitiveProperties.Add("DefineConstants");
		}
		
		public CSharpProject(ProjectLoadInformation loadInformation)
			: base(loadInformation)
		{
			Init();
		}
		
		public const string DefaultTargetsFile = @"$(MSBuildBinPath)\Microsoft.CSharp.Targets";
		public const string ExtendedTargetsFile = @"$(SharpDevelopBinPath)\SharpDevelop.Build.CSharp.targets";
		
		public CSharpProject(ProjectCreateInformation info)
			: base(info)
		{
			Init();
			
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
		
		public override void StartBuild(ProjectBuildOptions options, IBuildFeedbackSink feedbackSink)
		{
			if (this.MinimumSolutionVersion == Solution.SolutionVersionVS2005) {
				MSBuildEngine.StartBuild(this,
				                         options,
				                         feedbackSink,
				                         MSBuildEngine.AdditionalTargetFiles.Concat(
				                         	new [] { Path.Combine(MSBuildEngine.SharpDevelopBinPath, "SharpDevelop.CheckMSBuild35Features.targets") }));
			} else {
				base.StartBuild(options, feedbackSink);
			}
		}
		
		static readonly CompilerVersion msbuild20 = new CompilerVersion(new Version(2, 0), "C# 2.0");
		static readonly CompilerVersion msbuild35 = new CompilerVersion(new Version(3, 5), "C# 3.0");
		static readonly CompilerVersion msbuild40 = new CompilerVersion(new Version(4, 0), DotnetDetection.IsDotnet45Installed() ? "C# 5.0" : "C# 4.0");
		
		public override CompilerVersion CurrentCompilerVersion {
			get {
				switch (MinimumSolutionVersion) {
					case Solution.SolutionVersionVS2005:
						return msbuild20;
					case Solution.SolutionVersionVS2008:
						return msbuild35;
					case Solution.SolutionVersionVS2010:
					case Solution.SolutionVersionVS11:
						return msbuild40;
					default:
						throw new NotSupportedException();
				}
			}
		}
		
		public override IEnumerable<CompilerVersion> GetAvailableCompilerVersions()
		{
			List<CompilerVersion> versions = new List<CompilerVersion>();
			if (DotnetDetection.IsDotnet35SP1Installed()) {
				versions.Add(msbuild20);
				versions.Add(msbuild35);
			}
			versions.Add(msbuild40);
			return versions;
		}
		
		/*
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
		 */
		
		protected override ProjectBehavior CreateDefaultBehavior()
		{
			return new CSharpProjectBehavior(this, base.CreateDefaultBehavior());
		}
	}
	
	public class CSharpProjectBehavior : ProjectBehavior
	{
		public CSharpProjectBehavior(CSharpProject project, ProjectBehavior next = null)
			: base(project, next)
		{
			
		}
		
		public override ItemType GetDefaultItemType(string fileName)
		{
			if (string.Equals(Path.GetExtension(fileName), ".cs", StringComparison.OrdinalIgnoreCase))
				return ItemType.Compile;
			else
				return base.GetDefaultItemType(fileName);
		}
	}
}
