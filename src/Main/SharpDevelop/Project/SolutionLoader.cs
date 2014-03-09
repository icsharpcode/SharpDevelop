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
using System.Text.RegularExpressions;
using System.Windows.Media.Effects;
using ICSharpCode.Core;
using Microsoft.Build.Exceptions;

namespace ICSharpCode.SharpDevelop.Project
{
	sealed class SolutionLoader : IDisposable
	{
		readonly FileName fileName;
		readonly TextReader textReader;
		string currentLine;
		int lineNumber;
		
		public SolutionLoader(TextReader textReader)
		{
			this.textReader = textReader;
			NextLine();
		}
		
		public SolutionLoader(FileName fileName)
		{
			this.fileName = fileName;
			// read solution files using system encoding, but detect UTF8 if BOM is present
			this.textReader = new StreamReader(fileName, Encoding.Default, true);
			NextLine();
		}
		
		public void Dispose()
		{
			textReader.Dispose();
		}
		
		void NextLine()
		{
			do {
				currentLine = textReader.ReadLine();
				lineNumber++;
			} while (currentLine != null && (currentLine.Length == 0 || currentLine[0] == '#'));
		}
		
		ProjectLoadException Error(string message, params object[] formatItems)
		{
			if (formatItems.Length > 0)
				message = StringParser.Format(message, formatItems);
			else
				message = StringParser.Parse(message);
			return new ProjectLoadException("Error reading from " + fileName + " at line " + lineNumber + ":" + Environment.NewLine + message);
		}
		
		#region ReadSolution
		public void ReadSolution(Solution solution, IProgressMonitor progress)
		{
			ReadFormatHeader();
			ReadVersionProperties(solution);
			
			// Read solution folder and project entries:
			var solutionEntries = new List<ProjectLoadInformation>();
			var projectInfoDict = new Dictionary<Guid, ProjectLoadInformation>();
			var solutionFolderDict = new Dictionary<Guid, SolutionFolder>();
			int projectCount = 0;
			bool fixedGuidConflicts = false;
			
			ProjectLoadInformation information;
			while ((information = ReadProjectEntry(solution)) != null) {
				solutionEntries.Add(information);
				if (projectInfoDict.ContainsKey(information.IdGuid)) {
					// resolve GUID conflicts
					SD.Log.WarnFormatted("Detected duplicate GUID in .sln file: {0} is used for {1} and {2}", information.IdGuid, information.ProjectName, projectInfoDict[information.IdGuid].ProjectName);
					information.IdGuid = Guid.NewGuid();
					fixedGuidConflicts = true;
				}
				projectInfoDict.Add(information.IdGuid, information);
				
				if (information.TypeGuid == ProjectTypeGuids.SolutionFolder) {
					solutionFolderDict.Add(information.IdGuid, CreateSolutionFolder(solution, information));
				} else {
					projectCount++;
				}
			}
			
			progress.CancellationToken.ThrowIfCancellationRequested();
			
			// Read global sections:
			if (currentLine != "Global") {
				if (currentLine == null)
					throw Error("Unexpected end of file");
				else
					throw Error("Unknown line: " + currentLine);
			}
			NextLine();
			
			Dictionary<Guid, SolutionFolder> guidToParentFolderDict = null;
			
			SolutionSection section;
			while ((section = ReadSection(isGlobal: true)) != null) {
				switch (section.SectionName) {
					case "SolutionConfigurationPlatforms":
						var configurations = LoadSolutionConfigurations(section);
						foreach (var config in configurations.Select(c => c.Configuration).Distinct(ConfigurationAndPlatform.ConfigurationNameComparer))
							solution.ConfigurationNames.Add(config, null);
						foreach (var platform in configurations.Select(c => c.Platform).Distinct(ConfigurationAndPlatform.ConfigurationNameComparer))
							solution.PlatformNames.Add(platform, null);
						break;
					case "ProjectConfigurationPlatforms":
						LoadProjectConfigurations(section, projectInfoDict);
						break;
					case "NestedProjects":
						guidToParentFolderDict = LoadNesting(section, solutionFolderDict);
						break;
					default:
						solution.GlobalSections.Add(section);
						break;
				}
			}
			if (currentLine != "EndGlobal")
				throw Error("Expected 'EndGlobal'");
			NextLine();
			if (currentLine != null)
				throw Error("Expected end of file");
			
			solution.LoadPreferences();
			
			// Now that the project configurations have been set, we can actually load the projects:
			int projectsLoaded = 0;
			foreach (var projectInfo in solutionEntries) {
				// Make copy of IdGuid just in case the project binding writes to projectInfo.IdGuid
				Guid idGuid = projectInfo.IdGuid;
				ISolutionItem solutionItem;
				if (projectInfo.TypeGuid == ProjectTypeGuids.SolutionFolder) {
					solutionItem = solutionFolderDict[idGuid];
				} else {
					// Load project:
					projectInfo.ActiveProjectConfiguration = projectInfo.ConfigurationMapping.GetProjectConfiguration(solution.ActiveConfiguration);
					progress.TaskName = "Loading " + projectInfo.ProjectName;
					using (projectInfo.ProgressMonitor = progress.CreateSubTask(1.0 / projectCount)) {
						solutionItem = LoadProjectWithErrorHandling(projectInfo);
					}
					if (solutionItem.IdGuid != idGuid) {
						Guid projectFileGuid = solutionItem.IdGuid;
						if (!projectInfoDict.ContainsKey(projectFileGuid)) {
							// We'll use the GUID from the project file.
							// Register that GUID in the dictionary to avoid its use by multiple projects.
							projectInfoDict.Add(projectFileGuid, projectInfo);
						} else {
							// Cannot use GUID from project file due to conflict.
							// To fix the problem without potentially introducing new conflicts in other .sln files that contain the project,
							// we generate a brand new GUID:
							solutionItem.IdGuid = Guid.NewGuid();
						}
						SD.Log.WarnFormatted("<ProjectGuid> in project '{0}' is '{1}' and does not match the GUID stored in the solution ({2}). "
						                     + "The conflict was resolved using the GUID {3}",
						                     projectInfo.ProjectName, projectFileGuid, idGuid, solutionItem.IdGuid);
						fixedGuidConflicts = true;
					}
					projectsLoaded++;
					progress.Progress = (double)projectsLoaded / projectCount;
				}
				// Add solutionItem to solution:
				SolutionFolder folder;
				if (guidToParentFolderDict != null && guidToParentFolderDict.TryGetValue(idGuid, out folder)) {
					folder.Items.Add(solutionItem);
				} else {
					solution.Items.Add(solutionItem);
				}
			}
			
			solution.IsDirty = fixedGuidConflicts; // reset IsDirty=false unless we've fixed GUID conflicts
		}

		static IProject LoadProjectWithErrorHandling(ProjectLoadInformation projectInfo)
		{
			Exception exception;
			try {
				return SD.ProjectService.LoadProject(projectInfo);
			} catch (FileNotFoundException) {
				return new MissingProject(projectInfo);
			} catch (ProjectLoadException ex) {
				exception = ex;
			} catch (IOException ex) {
				exception = ex;
			} catch (UnauthorizedAccessException ex) {
				exception = ex;
			}
			LoggingService.Warn("Project load error", exception);
			return new ErrorProject(projectInfo, exception);
		}
		#endregion
		
		#region ReadFormatHeader
		static Regex versionPattern = new Regex(@"^Microsoft Visual Studio Solution File, Format Version\s+(?<Version>[\d\.]+)\s*$");
		static Regex ideVersionPattern = new Regex(@"((Minimum)?VisualStudioVersion)\s+=\s(?<Version>\d+\.\d+\.\d+\.\d+)");
		
		public SolutionFormatVersion ReadFormatHeader()
		{
			if (currentLine == null) // can happen if the .sln is an empty file
				throw Error("Solution file is empty");
			Match match = versionPattern.Match(currentLine);
			if (!match.Success)
				throw Error("The file is not a valid solution file");
			
			SolutionFormatVersion version;
			switch (match.Result("${Version}")) {
				case "7.00":
				case "8.00":
					throw Error("${res:SharpDevelop.Solution.CannotLoadOldSolution}");
				case "9.00":
					version = SolutionFormatVersion.VS2005;
					break;
				case "10.00":
					version = SolutionFormatVersion.VS2008;
					break;
				case "11.00":
					version = SolutionFormatVersion.VS2010;
					break;
				case "12.00":
					version = SolutionFormatVersion.VS2012;
					break;
				default:
					throw Error("${res:SharpDevelop.Solution.UnknownSolutionVersion}", match.Result("${Version}"));
			}
			NextLine();
			return version;
		}
		
		void ReadVersionProperties(Solution solution)
		{
			Match match = ideVersionPattern.Match(currentLine);
			while (match.Success) {
				Version ideVersion = new Version(match.Result("${Version}"));
				switch (match.Groups[1].Value) {
					case "VisualStudioVersion":
						solution.currVSVersion = ideVersion;
						break;
					case "MinimumVisualStudioVersion":
						solution.minVSVersion = ideVersion;
						break;
				}
				NextLine();
				match = ideVersionPattern.Match(currentLine);
			}
		}
		#endregion
		
		#region ReadSection
		void ReadSectionEntries(SolutionSection section)
		{
			while (currentLine != null) {
				int pos = currentLine.IndexOf('=');
				if (pos < 0)
					break; // end of section
				string key = currentLine.Substring(0, pos).Trim();
				string value = currentLine.Substring(pos + 1).Trim();
				section.Add(key, value);
				NextLine();
			}
		}
		
		static readonly Regex sectionHeaderPattern = new Regex("^\\s*(Global|Project)Section\\((?<Name>.*)\\)\\s*=\\s*(?<Type>.*)\\s*$");
		
		public SolutionSection ReadSection(bool isGlobal)
		{
			if (currentLine == null)
				return null;
			Match match = sectionHeaderPattern.Match(currentLine);
			if (!match.Success)
				return null;
			NextLine();
			
			SolutionSection section = new SolutionSection(match.Groups["Name"].Value, match.Groups["Type"].Value);
			ReadSectionEntries(section);
			string expectedLine = isGlobal ? "EndGlobalSection" : "EndProjectSection";
			if ((currentLine ?? string.Empty).Trim() != expectedLine)
				throw Error("Expected " + expectedLine);
			NextLine();
			return section;
		}
		#endregion
		
		#region ReadProjectEntry
		static readonly Regex projectLinePattern = new Regex("^\\s*Project\\(\"(?<TypeGuid>.*)\"\\)\\s+=\\s+\"(?<Title>.*)\",\\s*\"(?<Location>.*)\",\\s*\"(?<IdGuid>.*)\"\\s*$");
		
		public ProjectLoadInformation ReadProjectEntry(ISolution parentSolution)
		{
			if (currentLine == null)
				return null;
			Match match = projectLinePattern.Match(currentLine);
			if (!match.Success)
				return null;
			NextLine();
			string title = match.Groups["Title"].Value;
			string location = match.Groups["Location"].Value;
			FileName projectFileName = FileName.Create(Path.Combine(parentSolution.Directory, location));
			var loadInformation = new ProjectLoadInformation(parentSolution, projectFileName, title);
			loadInformation.TypeGuid = ParseGuidDefaultEmpty(match.Groups["TypeGuid"].Value);
			loadInformation.IdGuid = ParseGuidDefaultEmpty(match.Groups["IdGuid"].Value);
			SolutionSection section;
			while ((section = ReadSection(isGlobal: false)) != null) {
				loadInformation.ProjectSections.Add(section);
			}
			if (currentLine != "EndProject")
				throw Error("Expected 'EndProject'");
			NextLine();
			return loadInformation;
		}
		
		static Guid ParseGuidDefaultEmpty(string value)
		{
			Guid guid;
			if (Guid.TryParse(value, out guid))
				return guid;
			else
				return Guid.Empty;
		}
		#endregion
		
		#region Load Configurations
		internal IEnumerable<ConfigurationAndPlatform> LoadSolutionConfigurations(IEnumerable<KeyValuePair<string, string>> section)
		{
			// Entries in the section look like this: 'Debug|Any CPU = Debug|Any CPU'
			return section.Select(e => ConfigurationAndPlatform.FromKey(e.Key))
				.Where(e => ConfigurationAndPlatform.IsValidName(e.Configuration) && ConfigurationAndPlatform.IsValidName(e.Platform));
		}
		
		void LoadProjectConfigurations(SolutionSection section, Dictionary<Guid, ProjectLoadInformation> projectInfoDict)
		{
			foreach (var pair in section) {
				// pair is an entry like this: '{35CEF10F-2D4C-45F2-9DD1-161E0FEC583C}.Debug|Any CPU.ActiveCfg = Debug|Any CPU'
				if (pair.Key.EndsWith(".ActiveCfg", StringComparison.OrdinalIgnoreCase)) {
					Guid guid;
					ConfigurationAndPlatform solutionConfig;
					if (!TryParseProjectConfigurationKey(pair.Key, out guid, out solutionConfig))
						continue;
					ProjectLoadInformation projectInfo;
					if (!projectInfoDict.TryGetValue(guid, out projectInfo))
						continue;
					var projectConfig = ConfigurationAndPlatform.FromKey(pair.Value);
					if (projectConfig == default(ConfigurationAndPlatform))
						continue;
					projectInfo.ConfigurationMapping.SetProjectConfiguration(solutionConfig, projectConfig);
					// Disable build if we see a '.ActiveCfg' entry.
					projectInfo.ConfigurationMapping.SetBuildEnabled(solutionConfig, false);
				}
			}
			// Enable build/deploy if we see the corresponding entries:
			foreach (var pair in section) {
				// pair is an entry like this: '{35CEF10F-2D4C-45F2-9DD1-161E0FEC583C}.Debug|Any CPU.Build.0 = Debug|Any CPU'
				Guid guid;
				ConfigurationAndPlatform solutionConfig;
				if (!TryParseProjectConfigurationKey(pair.Key, out guid, out solutionConfig))
					continue;
				ProjectLoadInformation projectInfo;
				if (!projectInfoDict.TryGetValue(guid, out projectInfo))
					continue;
				if (pair.Key.EndsWith(".Build.0", StringComparison.OrdinalIgnoreCase)) {
					projectInfo.ConfigurationMapping.SetBuildEnabled(solutionConfig, true);
				} else if (pair.Key.EndsWith(".Deploy.0", StringComparison.OrdinalIgnoreCase)) {
					projectInfo.ConfigurationMapping.SetDeployEnabled(solutionConfig, true);
				}
			}
		}
		
		bool TryParseProjectConfigurationKey(string key, out Guid guid, out ConfigurationAndPlatform config)
		{
			guid = default(Guid);
			config = default(ConfigurationAndPlatform);
			
			int firstDot = key.IndexOf('.');
			int secondDot = key.IndexOf('.', firstDot + 1);
			if (firstDot < 0 || secondDot < 0)
				return false;
			
			string guidText = key.Substring(0, firstDot);
			if (!Guid.TryParse(guidText, out guid))
				return false;
			
			string configKey = key.Substring(firstDot + 1, secondDot - (firstDot + 1));
			config = ConfigurationAndPlatform.FromKey(configKey);
			return config != default(ConfigurationAndPlatform);
		}
		#endregion
		
		#region Load Nesting
		SolutionFolder CreateSolutionFolder(Solution solution, ProjectLoadInformation information)
		{
			var folder = new SolutionFolder(solution, information.IdGuid);
			folder.Name = information.ProjectName;
			// Add solution items:
			var solutionItemsSection = information.ProjectSections.FirstOrDefault(s => s.SectionName == "SolutionItems");
			if (solutionItemsSection != null) {
				foreach (string location in solutionItemsSection.Values) {
					var fileItem = new SolutionFileItem(solution);
					fileItem.FileName = FileName.Create(Path.Combine(information.Solution.Directory, location));
					folder.Items.Add(fileItem);
				}
			}
			return folder;
		}
		
		/// <summary>
		/// Converts the 'NestedProjects' section into a dictionary from project GUID to parent solution folder.
		/// </summary>
		Dictionary<Guid, SolutionFolder> LoadNesting(SolutionSection section, IReadOnlyDictionary<Guid, SolutionFolder> solutionFolderDict)
		{
			var result = new Dictionary<Guid, SolutionFolder>();
			foreach (var entry in section) {
				Guid idGuid;
				Guid parentGuid;
				if (Guid.TryParse(entry.Key, out idGuid) && Guid.TryParse(entry.Value, out parentGuid)) {
					SolutionFolder parentFolder;
					if (solutionFolderDict.TryGetValue(parentGuid, out parentFolder))
						result[idGuid] = parentFolder;
				}
			}
			return result;
		}
		#endregion
	}
}
