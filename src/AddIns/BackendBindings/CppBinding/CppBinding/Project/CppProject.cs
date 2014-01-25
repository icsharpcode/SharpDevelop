// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Construction;
using Microsoft.Build.Exceptions;

namespace ICSharpCode.CppBinding.Project
{
	public class CppProject : CompilableProject
	{
		public CppProject(ProjectCreateInformation info)
			: base(info)
		{
			try {
				AddImport(DefaultPropsFile, null);
				AddImport(PropsFile, null);
				AddImport(DefaultTargetsFile, null);
				AddProjectConfigurationsItemGroup();
				base.ReevaluateIfNecessary(); // provoke exception if import is invalid
			} catch (InvalidProjectFileException ex) {
				Dispose();
				throw new ProjectLoadException("Please ensure that the Windows SDK is installed on your computer.\n\n" + ex.Message, ex);
			}
		}

		public CppProject(ProjectLoadInformation info)
			: base(info)
		{
		}

		public override IAmbience GetAmbience()
		{
			return new CSharpAmbience();
		}

		public override string Language
		{
			get { return CppProjectBinding.LanguageName; }
		}

//		public override LanguageProperties LanguageProperties
//		{
//			get { return LanguageProperties.; }
//		}

		public override FileName OutputAssemblyFullPath
		{
			/// <summary>
			/// For vcxprojs the output assembly location is stored in OutDir property.
			/// The path is relative to [Solution] folder, not [Solution]/[Project]
			/// </summary>
			get
			{
				string outputPath = GetEvaluatedProperty("OutDir") ?? "";
				if (!Path.IsPathRooted(outputPath))
					return FileName.Create(Path.Combine(ParentSolution.Directory, outputPath,
					                                    AssemblyName + GetExtension(OutputType)));
				else
				{
					// this will be valid if there is an explicit OutDir property in vcxproj file.
					if ((GetUnevalatedProperty("OutDir") ?? "").StartsWith("$(SolutionDir)", StringComparison.OrdinalIgnoreCase)) {
						// in #D every project is compiled by msbuild separately, this mean that SolutionDir will
						// be equal to ProjectDir, so it has to be replaced with actual solution directory
						string evaluatedSolutionDir = GetEvaluatedProperty("SolutionDir") ?? "";
						outputPath = Path.Combine(ParentSolution.Directory, outputPath.Substring(evaluatedSolutionDir.Length));
					}
					return FileName.Create(Path.Combine(outputPath, AssemblyName + GetExtension(OutputType)));
				}
			}
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
		public IList<string> GetIncludeDirectories(FileName fileName)
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
		public IList<string> GetProjectDefines(FileName fileName)
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
		
		public IList<string> GetProjectUndefines(FileName fileName)
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
			foreach (var target in new [] { new ConfigurationAndPlatform("Debug", "Win32"), new ConfigurationAndPlatform("Release", "Win32") })
			{
				ProjectItemElement prjConfiguration = configItemGroup.AddItem("ProjectConfiguration", target.ToString());
				prjConfiguration.AddMetadata("Configuration", target.Configuration);
				prjConfiguration.AddMetadata("Platform", target.Platform);
			}
		}
		
		protected override ProjectBehavior CreateDefaultBehavior()
		{
			return new CppProjectBehavior(this, base.CreateDefaultBehavior());
		}
	}
	
	public class CppProjectBehavior : ProjectBehavior
	{
		public CppProjectBehavior(CppProject project, ProjectBehavior next = null)
			: base(project, next)
		{
		}
		
		public override ProjectItem CreateProjectItem(IProjectItemBackendStore item)
		{
			if ("ProjectConfiguration" == item.ItemType.ItemName)
				return new ProjectConfigurationProjectItem(Project, item);
			return base.CreateProjectItem(item);
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
	}
}
