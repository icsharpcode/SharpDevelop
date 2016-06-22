// 
// TypeScriptProjectOptionsPanel.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2014 Matthew Ward
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TypeScriptBinding
{
	public partial class TypeScriptProjectOptionsPanel : ProjectOptionPanel
	{
		bool compileOnSave;
		bool compileOnBuild;
		bool includeComments;
		bool generateSourceMap;
		bool allowImplicitAnyTypes;
		bool useOutputFileName;
		bool useOutputDirectory;
		DisplayValue selectedEcmaScriptTargetVersion;
		List<DisplayValue> ecmaScriptTargetVersions = new List<DisplayValue>();
		DisplayValue selectedModuleKind;
		List<DisplayValue> moduleKinds = new List<DisplayValue>();
		string outputFileName = String.Empty;
		string outputDirectory = String.Empty;
		
		public TypeScriptProjectOptionsPanel()
		{
			InitializeComponent();
			
			ecmaScriptTargetVersions.Add(new DisplayValue("ES3", "ECMAScript 3"));
			ecmaScriptTargetVersions.Add(new DisplayValue("ES5", "ECMAScript 5"));
			ecmaScriptTargetVersions.Add(new DisplayValue("ES6", "ECMAScript 6"));
			
			moduleKinds.Add(new DisplayValue("none", "None"));
			moduleKinds.Add(new DisplayValue("amd", "AMD"));
			moduleKinds.Add(new DisplayValue("commonjs", "CommonJS"));
			moduleKinds.Add(new DisplayValue("system", "System"));
			moduleKinds.Add(new DisplayValue("umd", "UMD"));
			
			DataContext = this;
		}
		
		void UpdateDirtyFlag<T>(T oldValue, T newValue)
		{
			if (!Object.Equals(oldValue, newValue)) {
				IsDirty = true;
			}
		}
		
		public bool CompileOnSave {
			get { return compileOnSave; }
			set {
				UpdateDirtyFlag(compileOnSave, value);
				compileOnSave = value;
				RaisePropertyChanged(() => CompileOnSave);
			}
		}
		
		public bool CompileOnBuild {
			get { return compileOnBuild; }
			set {
				UpdateDirtyFlag(compileOnBuild, value);
				compileOnBuild = value;
				RaisePropertyChanged(() => CompileOnBuild);
			}
		}
		
		public bool IncludeComments {
			get { return includeComments; }
			set {
				UpdateDirtyFlag(includeComments, value);
				includeComments = value;
				RaisePropertyChanged(() => IncludeComments);
			}
		}
		
		public bool GenerateSourceMap {
			get { return generateSourceMap; }
			set {
				UpdateDirtyFlag(generateSourceMap, value);
				generateSourceMap = value;
				RaisePropertyChanged(() => GenerateSourceMap);
			}
		}
		
		public List<DisplayValue> EcmaScriptTargetVersions {
			get { return ecmaScriptTargetVersions; }
		}
		
		public DisplayValue SelectedEcmaScriptTargetVersion {
			get { return selectedEcmaScriptTargetVersion; }
			set {
				UpdateDirtyFlag(selectedEcmaScriptTargetVersion, value);
				selectedEcmaScriptTargetVersion = value;
				RaisePropertyChanged(() => SelectedEcmaScriptTargetVersion);
			}
		}
		
		public List<DisplayValue> ModuleKinds {
			get { return moduleKinds; }
		}
		
		public DisplayValue SelectedModuleKind {
			get { return selectedModuleKind; }
			set {
				UpdateDirtyFlag(selectedModuleKind, value);
				selectedModuleKind = value;
				RaisePropertyChanged(() => SelectedModuleKind);
			}
		}
		
		public bool AllowImplicitAnyTypes {
			get { return allowImplicitAnyTypes; }
			set {
				UpdateDirtyFlag(allowImplicitAnyTypes, value);
				allowImplicitAnyTypes = value;
				RaisePropertyChanged(() => AllowImplicitAnyTypes);
			}
		}
		
		public string OutputFileName {
			get { return outputFileName; }
			set {
				UpdateDirtyFlag(outputFileName, value);
				outputFileName = value;
				RaisePropertyChanged(() => OutputFileName);
			}
		}
		
		public bool UseOutputFileName {
			get { return useOutputFileName; }
			set {
				UpdateDirtyFlag(useOutputFileName, value);
				useOutputFileName = value;
				RaisePropertyChanged(() => UseOutputFileName);
			}
		}
		
		public string OutputDirectory {
			get { return outputDirectory; }
			set {
				UpdateDirtyFlag(outputDirectory, value);
				outputDirectory = value;
				RaisePropertyChanged(() => OutputDirectory);
			}
		}
		
		public bool UseOutputDirectory {
			get { return useOutputDirectory; }
			set {
				UpdateDirtyFlag(useOutputDirectory, value);
				useOutputDirectory = value;
				RaisePropertyChanged(() => UseOutputDirectory);
			}
		}
		
		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			base.Load(project, configuration, platform);
			
			var buildConfig = new BuildConfiguration(configuration, platform);
			
			var typeScriptProject = new TypeScriptProject(project);
			CompileOnSave = typeScriptProject.GetCompileOnSave(buildConfig);
			CompileOnBuild = typeScriptProject.GetCompileOnBuild(buildConfig);
			IncludeComments = !typeScriptProject.GetRemoveComments(buildConfig);
			GenerateSourceMap = typeScriptProject.GetGenerateSourceMap(buildConfig);
			AllowImplicitAnyTypes = !typeScriptProject.GetNoImplicitAny(buildConfig);
			SelectedEcmaScriptTargetVersion = GetEcmaScriptTargetVersion(typeScriptProject, buildConfig);
			SelectedModuleKind = GetModuleKind(typeScriptProject, buildConfig);
			OutputFileName = typeScriptProject.GetOutputFileName(buildConfig);
			OutputDirectory = typeScriptProject.GetOutputDirectory(buildConfig);
			
			if (!String.IsNullOrEmpty(outputFileName)) {
				UseOutputFileName = true;
			}
			if (!String.IsNullOrEmpty(outputDirectory)) {
				UseOutputDirectory = true;
			}
			
			IsDirty = false;
		}
		
		DisplayValue GetEcmaScriptTargetVersion(TypeScriptProject project, BuildConfiguration buildConfig)
		{
			string value = project.GetEcmaScriptVersion(buildConfig);
			
			DisplayValue displayValue = ecmaScriptTargetVersions.FirstOrDefault(version => version.Id.Equals(value, StringComparison.OrdinalIgnoreCase));
			if (displayValue != null) {
				return displayValue;
			}
			
			return ecmaScriptTargetVersions[0];
		}
		
		DisplayValue GetModuleKind(TypeScriptProject project, BuildConfiguration buildConfig)
		{
			string value = project.GetModuleKind(buildConfig);
			
			DisplayValue displayValue = moduleKinds.FirstOrDefault(moduleKind => moduleKind.Id.Equals(value, StringComparison.OrdinalIgnoreCase));
			if (displayValue != null) {
				return displayValue;
			}
			
			return moduleKinds[0];
		}
		
		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			var buildConfig = new BuildConfiguration(configuration, platform);
			
			var typeScriptProject = new TypeScriptProject(project);
			typeScriptProject.SetCompileOnSave(buildConfig, CompileOnSave);
			typeScriptProject.SetCompileOnBuild(buildConfig, CompileOnBuild);
			typeScriptProject.SetRemoveComments(buildConfig, !IncludeComments);
			typeScriptProject.SetGenerateSourceMap(buildConfig, GenerateSourceMap);
			typeScriptProject.SetNoImplicitAny(buildConfig, !AllowImplicitAnyTypes);
			typeScriptProject.SetEcmaScriptVersion(buildConfig, SelectedEcmaScriptTargetVersion.Id);
			typeScriptProject.SetModuleKind(buildConfig, SelectedModuleKind.Id);
			
			typeScriptProject.SetOutputFileName(buildConfig, GetOutputFileName());
			typeScriptProject.SetOutputDirectory(buildConfig, GetOutputDirectory());
			
			return base.Save(project, configuration, platform);
		}
		
		string GetOutputFileName()
		{
			if (UseOutputFileName) {
				return OutputFileName;
			}
			return String.Empty;
		}
		
		string GetOutputDirectory()
		{
			if (UseOutputDirectory) {
				return OutputDirectory;
			}
			return String.Empty;
		}
		
		void BrowseForOutputDirectoryClick(object sender, RoutedEventArgs e)
		{
			BrowseForOutputDirectory();
		}
		
		void BrowseForOutputDirectory()
		{
			using (var dialog = new FolderBrowserDialog()) {
				dialog.ShowNewFolderButton = true;
				if (DialogResult.OK == dialog.ShowDialog()) {
					OutputDirectory = dialog.SelectedPath;
				}
			}
		}
	}
}