using Microsoft.Build.Construction;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CppBinding.Project
{
	public class CppProject : CompilableProject
	{
		public CppProject(ProjectCreateInformation info) : base(info)
		{
			AddImport(DefaultPropsFile, null);
			AddImport(PropsFile, null);
			AddImport(DefaultTargetsFile, null);
			AddProjectConfigurationsItemGroup();
		}
		
		public CppProject(ProjectLoadInformation info) : base(info)
		{
		}
		
		public override ProjectItem CreateProjectItem(IProjectItemBackendStore item)
		{
			if ("ProjectConfiguration" == item.ItemType.ItemName)
	    			return new ProjectConfigurationProjectItem(this, item);
			return base.CreateProjectItem(item);
		}
		
		public override string Language
		{
			get { return CppLanguageBinding.LanguageName; }
		}
		
		public override LanguageProperties LanguageProperties
		{
			get { return LanguageProperties.None; }
		}
		
		public override string OutputAssemblyFullPath
		{
			/// <summary>
			/// For vcxprojs the output assembly location is stored in OutDir property.
			/// The path is relative to [Solution] folder, not [Solution]/[Project]
			/// </summary>
			get
			{
				string outputPath = GetEvaluatedProperty("OutDir") ?? "";
				if (!Path.IsPathRooted(outputPath)) 
					return FileUtility.NormalizePath(Path.Combine(Path.Combine(Path.Combine(Directory, ".."), outputPath), 
					                                              	AssemblyName + GetExtension(OutputType)));
				else {
					// this will be valid if there is an explicit OutDir property in vcxproj file.
					if (GetUnevalatedProperty("OutDir").StartsWith("$(SolutionDir)")) {
						// in #D every project is compiled by msbuild separately, this mean that SolutionDir will
						// be equal to ProjectDir, so it has to be replaced with actual solution directory
						string evaluatedSolutionDir = GetEvaluatedProperty("SolutionDir");
						outputPath = Path.Combine(ParentSolution.Directory, outputPath.Substring(evaluatedSolutionDir.Length));
					}
					return FileUtility.NormalizePath(Path.Combine(outputPath, AssemblyName + GetExtension(OutputType)));
				}
			}
		}
		
		public override ItemType GetDefaultItemType(string fileName)
		{
			string extension = Path.GetExtension(fileName).ToLower();
			switch (extension) {
				case ".cpp": return ItemType.ClCompile;
				case ".c": return ItemType.ClCompile;
				case ".hpp": return ItemType.ClInclude;
				case ".h": return ItemType.ClInclude;
			}
			return base.GetDefaultItemType(fileName);
		}
		
		public const string DefaultTargetsFile = @"$(VCTargetsPath)\Microsoft.Cpp.Targets";
		public const string DefaultPropsFile = @"$(VCTargetsPath)\Microsoft.Cpp.Default.props";
		public const string PropsFile = @"$(VCTargetsPath)\Microsoft.Cpp.props";
		
		/// <summary>
		/// Adds the item group containting the ProjectConfiguration items to a new project.
		/// </summary>
		private void AddProjectConfigurationsItemGroup() {
			ProjectRootElement file = MSBuildProjectFile;
			ProjectItemGroupElement configItemGroup = file.AddItemGroup();
			configItemGroup.Label = "ProjectConfigurations";	
			foreach (string target in new string[] {"Debug|Win32", "Release|Win32"}) {
				ProjectItemElement prjConfiguration = configItemGroup.AddItem("ProjectConfiguration", target);
				prjConfiguration.AddMetadata("Configuration", GetConfigurationNameFromKey(target));
				prjConfiguration.AddMetadata("Platform", GetPlatformNameFromKey(target));
			}
		}
	}
}
