using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Construction;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using System.Diagnostics;

namespace ICSharpCode.CppBinding.Project
{
	public class CppProject : CompilableProject
	{
		public CppProject(ProjectCreateInformation info)
			: base(info)
		{
			AddImport(DefaultPropsFile, null);
			AddImport(PropsFile, null);
			AddImport(DefaultTargetsFile, null);
			AddProjectConfigurationsItemGroup();
		}

		public CppProject(ProjectLoadInformation info)
			: base(info)
		{
		}

		public override ProjectItem CreateProjectItem(IProjectItemBackendStore item)
		{
			if ("ProjectConfiguration" == item.ItemType.ItemName)
				return new ProjectConfigurationProjectItem(this, item);
			return base.CreateProjectItem(item);
		}

		public override IAmbience GetAmbience()
		{
			return new CppAmbience();
		}

		public override string Language
		{
			get { return CppLanguageBinding.LanguageName; }
		}

		public override LanguageProperties LanguageProperties
		{
			get { return CppLanguageBinding.LanguageProperties; }
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
				else
				{
					// this will be valid if there is an explicit OutDir property in vcxproj file.
					if (GetUnevalatedProperty("OutDir").StartsWith("$(SolutionDir)"))
					{
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
			const string RESOURCE_COMPILE = "ResourceCompile";

			string extension = Path.GetExtension(fileName).ToLower();
			switch (extension)
			{
				case ".cpp": return ItemType.ClCompile;
				case ".c": return ItemType.ClCompile;
				case ".hpp": return ItemType.ClInclude;
				case ".h": return ItemType.ClInclude;
				case ".rc": return new ItemType(RESOURCE_COMPILE);
			}
			return base.GetDefaultItemType(fileName);
		}

		public const string DefaultTargetsFile = @"$(VCTargetsPath)\Microsoft.Cpp.Targets";
		public const string DefaultPropsFile = @"$(VCTargetsPath)\Microsoft.Cpp.Default.props";
		public const string PropsFile = @"$(VCTargetsPath)\Microsoft.Cpp.props";

		/// <summary>
		/// Relation of file inclusion in the project. Contains file names. If there
		/// is a pair ("a", "b") in the relation then in file "a" there is an include
		/// directive that includes file "b".
		/// </summary>
		public DependencyRelation<string, string> FileInclusionRelation
		{
			get
			{
				lock (this)
				{
					if (fileInclusionRelation == null)
						fileInclusionRelation = new DependencyRelation<string, string>();
					return fileInclusionRelation;
				}
			}
		}
		DependencyRelation<string, string> fileInclusionRelation;

		/// <summary>
		/// Returns all directories where included files are being searched.
		/// </summary>
		/// <param name="fileName">name of the file for which the check is being performed.
		/// If this is null, then only project default include location will be returned.</param>
		public IList<string> GetIncludeDirectories(string fileName)
		{
			IList<string> result = GetEvaluatedProperty("IncludePath")
				.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

			if (fileName != null)
			{
				FileProjectItem fileItem = FindFile(fileName);
				if (fileItem != null)
				{
					string additionalIncludes = fileItem.GetEvaluatedMetadata("AdditionalIncludeDirectories");
					result = result.Concat(
						additionalIncludes.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)).ToList();
				}
			}

			for(int i=0; i<result.Count; i++)
				if (!Path.IsPathRooted(result[i]))
					result[i] = FileUtility.NormalizePath(Path.Combine(Directory, result[i]));
			return result;
		}

		/// <summary>
		/// Gets the define symbols that are set for the specified file.
		/// </summary>
		/// <param name="fileName">name of the file</param>
		/// <returns>a list of symbols defined in project file, or an empty list if file doesn't exist in project</returns>
		public IList<string> GetProjectDefines(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			FileProjectItem fileItem = FindFile(fileName);
			IList<string> result;
			if (fileItem != null) {
				string definedSymbols = fileItem.GetEvaluatedMetadata("PreprocessorDefinitions");
				result = definedSymbols.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			}
			else
				result = new string[0];
			return result;
		}
		
		public IList<string> GetProjectUndefines(string fileName) 
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			FileProjectItem fileItem = FindFile(fileName);
			IList<string> result;
			if (fileItem != null) {
				string undefinedSymbols = fileItem.GetEvaluatedMetadata("UndefinePreprocessorDefinitions");
				result = undefinedSymbols.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			}
			else
				result = new string[0];
			return result;
		}

		/// <summary>
		/// Adds the item group containting the ProjectConfiguration items to a new project.
		/// </summary>
		void AddProjectConfigurationsItemGroup()
		{
			ProjectRootElement file = MSBuildProjectFile;
			ProjectItemGroupElement configItemGroup = file.AddItemGroup();
			configItemGroup.Label = "ProjectConfigurations";
			foreach (string target in new string[] { "Debug|Win32", "Release|Win32" })
			{
				ProjectItemElement prjConfiguration = configItemGroup.AddItem("ProjectConfiguration", target);
				prjConfiguration.AddMetadata("Configuration", GetConfigurationNameFromKey(target));
				prjConfiguration.AddMetadata("Platform", GetPlatformNameFromKey(target));
			}
		}
	}
}