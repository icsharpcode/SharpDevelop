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
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Project
{
	sealed class SolutionWriter : IDisposable
	{
		readonly TextWriter writer;
		readonly DirectoryName basePath;
		
		public SolutionWriter(TextWriter writer, DirectoryName basePath = null)
		{
			this.writer = writer;
			this.basePath = basePath;
		}
		
		public SolutionWriter(FileName fileName)
		{
			// we need to specify UTF8 because MSBuild needs the BOM
			this.writer = new StreamWriter(fileName, false, Encoding.UTF8);
			this.basePath = fileName.GetParentDirectory();
		}
		
		public void Dispose()
		{
			writer.Dispose();
		}
		
		#region Format Header
		public void WriteFormatHeader(SolutionFormatVersion version)
		{
			writer.WriteLine();
			writer.WriteLine("Microsoft Visual Studio Solution File, Format Version " + (int)version + ".00");
			switch (version) {
				case SolutionFormatVersion.VS2005:
					writer.WriteLine("# Visual Studio 2005");
					break;
				case SolutionFormatVersion.VS2008:
					writer.WriteLine("# Visual Studio 2008");
					break;
				case SolutionFormatVersion.VS2010:
					writer.WriteLine("# Visual Studio 2010");
					break;
				case SolutionFormatVersion.VS2012:
					writer.WriteLine("# Visual Studio 2012");
					break;
			}
			writer.WriteLine("# SharpDevelop " + RevisionClass.Major + "." + RevisionClass.Minor);
		}
		
		public void WriteSolutionVersionProperties(SolutionFormatVersion version, Version currVSVersion, Version minVSVersion)
		{
			if (version >= SolutionFormatVersion.VS2012) {
				writer.WriteLine("VisualStudioVersion = {0}", currVSVersion);
				writer.WriteLine("MinimumVisualStudioVersion = {0}", minVSVersion);
			}
		}
		#endregion
		
		#region Sections
		public void WriteSectionEntries(SolutionSection section)
		{
			foreach (var entry in section) {
				writer.Write("\t\t");
				writer.Write(entry.Key);
				writer.Write(" = ");
				writer.Write(entry.Value);
				writer.WriteLine();
			}
		}
		
		public void WriteProjectSection(SolutionSection section)
		{
			if (section.Count != 0) {
				writer.WriteLine("\tProjectSection({0}) = {1}", section.SectionName, section.SectionType);
				WriteSectionEntries(section);
				writer.WriteLine("\tEndProjectSection");
			}
		}
		
		public void WriteGlobalSection(SolutionSection section)
		{
			if (section.Count != 0) {
				writer.WriteLine("\tGlobalSection({0}) = {1}", section.SectionName, section.SectionType);
				WriteSectionEntries(section);
				writer.WriteLine("\tEndGlobalSection");
			}
		}
		#endregion
		
		#region Projects
		public void WriteSolutionItems(ISolution solution)
		{
			foreach (var item in solution.AllItems)
				WriteSolutionItem(item);
		}
		
		public void WriteSolutionItem(ISolutionItem item)
		{
			ISolutionFolder folder = item as ISolutionFolder;
			IProject project = item as IProject;
			if (folder != null) {
				WriteProjectHeader(ProjectTypeGuids.SolutionFolder, folder.Name, folder.Name, folder.IdGuid);
				// File items are represented as nodes in the tree, but they're actually
				// saved as properties on their containing folder.
				SolutionSection section = new SolutionSection("SolutionItems", "preProject");
				foreach (var file in folder.Items.OfType<ISolutionFileItem>()) {
					string location = FileUtility.GetRelativePath(basePath, file.FileName);
					section.Add(location, location);
				}
				WriteProjectSection(section);
				writer.WriteLine("EndProject");
			} else if (project != null) {
				string location = FileUtility.GetRelativePath(basePath, project.FileName);
				WriteProjectHeader(project.TypeGuid, project.Name, location, project.IdGuid);
				foreach (var section in project.ProjectSections) {
					WriteProjectSection(section);
				}
				writer.WriteLine("EndProject");
			}
		}
		
		void WriteProjectHeader(Guid typeGuid, string name, string location, Guid idGuid)
		{
			writer.WriteLine("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"",
			                 GuidToString(typeGuid), name, location, GuidToString(idGuid));
		}
		
		static string GuidToString(Guid guid)
		{
			return guid.ToString("B").ToUpperInvariant();
		}
		#endregion
		
		#region Global Section
		public void WriteGlobalSections(ISolution solution)
		{
			writer.WriteLine("Global");
			
			WriteGlobalSection(GenerateSolutionConfigurationSection(solution));
			WriteGlobalSection(GenerateProjectConfigurationSection(solution));
			
			foreach (var section in solution.GlobalSections) {
				WriteGlobalSection(section);
			}
			
			WriteGlobalSection(GenerateNestingSection(solution));
			
			writer.WriteLine("EndGlobal");
		}
		
		SolutionSection GenerateSolutionConfigurationSection(IConfigurable solution)
		{
			SolutionSection section = new SolutionSection("SolutionConfigurationPlatforms", "preSolution");
			foreach (var config in solution.ConfigurationNames) {
				foreach (var platform in solution.PlatformNames) {
					string key = config + "|" + platform;
					section.Add(key, key);
				}
			}
			return section;
		}
		
		SolutionSection GenerateProjectConfigurationSection(ISolution solution)
		{
			SolutionSection section = new SolutionSection("ProjectConfigurationPlatforms", "postSolution");
			foreach (var project in solution.Projects) {
				foreach (var configuration in solution.ConfigurationNames) {
					foreach (var platform in solution.PlatformNames) {
						var solutionConfig = new ConfigurationAndPlatform(configuration, platform);
						var projectConfig = project.ConfigurationMapping.GetProjectConfiguration(solutionConfig);
						string key = GuidToString(project.IdGuid) + "." + solutionConfig;
						string value = projectConfig.Configuration + "|" + MSBuildInternals.FixPlatformNameForSolution(projectConfig.Platform);
						section.Add(key + ".ActiveCfg", value);
						if (project.ConfigurationMapping.IsBuildEnabled(solutionConfig))
							section.Add(key + ".Build.0", value);
						if (project.ConfigurationMapping.IsDeployEnabled(solutionConfig))
							section.Add(key + ".Deploy.0", value);
					}
				}
			}
			return section;
		}
		
		SolutionSection GenerateNestingSection(ISolution solution)
		{
			SolutionSection section = new SolutionSection("NestedProjects", "preSolution");
			foreach (var item in solution.AllItems) {
				if (item is ISolutionFileItem)
					continue; // file items are a special case as they're saved in their own section
				if (item.ParentFolder != solution) {
					section.Add(GuidToString(item.IdGuid), GuidToString(item.ParentFolder.IdGuid));
				}
			}
			return section;
		}
		#endregion
	}
}
