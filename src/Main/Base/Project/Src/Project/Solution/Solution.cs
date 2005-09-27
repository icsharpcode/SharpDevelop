// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of Solution.
	/// </summary>
	public class Solution : SolutionFolder, IDisposable
	{
		// contains <guid>, (IProject/ISolutionFolder) pairs.
		Dictionary<string, ISolutionFolder> guidDictionary = new Dictionary<string, ISolutionFolder>();
		
		string fileName = String.Empty;
		
		public class ProjectEnumerator {
			Solution solution;
			public ProjectEnumerator(Solution solution)
			{
				this.solution = solution;
			}
			public IEnumerator<IProject> GetEnumerator()
			{
				Stack<ISolutionFolder> stack = new Stack<ISolutionFolder>();
				
				foreach (ISolutionFolder solutionFolder in solution.Folders) {
					stack.Push(solutionFolder);
				}
				
				while (stack.Count > 0) {
					ISolutionFolder currentFolder = stack.Pop();
					
					if (currentFolder is IProject) {
						yield return ((IProject)currentFolder);
					}
					
					if (currentFolder is ISolutionFolderContainer) {
						ISolutionFolderContainer currentContainer = (ISolutionFolderContainer)currentFolder;
						foreach (ISolutionFolder subFolder in currentContainer.Folders) {
							stack.Push(subFolder);
						}
					}
				}
			}
		}
		ProjectEnumerator projectEnumerator;
		[Browsable(false)]
		public ProjectEnumerator Projects {
			get {
				return projectEnumerator;
			}
		}
		
		
		public class SolutionFolderContainerEnumerator {
			Solution solution;
			public SolutionFolderContainerEnumerator(Solution solution)
			{
				this.solution = solution;
			}
			public IEnumerator<ISolutionFolderContainer> GetEnumerator()
			{
				Stack<ISolutionFolder> stack = new Stack<ISolutionFolder>();
				
				foreach (ISolutionFolder solutionFolder in solution.Folders) {
					stack.Push(solutionFolder);
				}
				
				while (stack.Count > 0) {
					ISolutionFolder currentFolder = stack.Pop();
					
					if (currentFolder is ISolutionFolderContainer) {
						ISolutionFolderContainer currentContainer = (ISolutionFolderContainer)currentFolder;
						yield return currentContainer;
						foreach (ISolutionFolder subFolder in currentContainer.Folders) {
							stack.Push(subFolder);
						}
					}
				}
			}
		}
		SolutionFolderContainerEnumerator solutionFolderContainerEnumerator;
		[Browsable(false)]
		public SolutionFolderContainerEnumerator SolutionFolderContainers {
			get {
				return solutionFolderContainerEnumerator;
			}
		}
		
		public class SolutionFolderEnumerator {
			Solution solution;
			public SolutionFolderEnumerator(Solution solution)
			{
				this.solution = solution;
			}
			public IEnumerator<ISolutionFolder> GetEnumerator()
			{
				Stack<ISolutionFolder> stack = new Stack<ISolutionFolder>();
				
				foreach (ISolutionFolder solutionFolder in solution.Folders) {
					stack.Push(solutionFolder);
				}
				
				while (stack.Count > 0) {
					ISolutionFolder currentFolder = stack.Pop();
					
					yield return currentFolder;
					
					if (currentFolder is ISolutionFolderContainer) {
						ISolutionFolderContainer currentContainer = (ISolutionFolderContainer)currentFolder;
						foreach (ISolutionFolder subFolder in currentContainer.Folders) {
							stack.Push(subFolder);
						}
					}
				}
			}
		}
		SolutionFolderEnumerator solutionFolderEnumerator;
		[Browsable(false)]
		public SolutionFolderEnumerator SolutionFolders {
			get {
				return solutionFolderEnumerator;
			}
		}
		
		[Browsable(false)]
		public IProject StartupProject {
			get {
				if (!HasProjects) {
					return null;
				}
				IProject startupProject = preferences.StartupProject;
				if (startupProject != null)
					return startupProject;
				foreach (IProject project in Projects) {
					if (project.IsStartable) {
						return project;
					}
				}
				return null;
			}
		}
		
		[Browsable(false)]
		public bool HasProjects {
			get {
				return Projects.GetEnumerator().MoveNext();
			}
		}
		
		public string FileName {
			get {
				return fileName;
			}
		}
		
		public string Directory {
			get {
				return Path.GetDirectoryName(fileName);
			}
		}
		
		[Browsable(false)]
		public bool IsDirty {
			get {
				foreach (IProject project in Projects) {
					if (project.IsDirty) {
						return true;
					}
				}
				return false;
			}
		}
		
		#region ISolutionFolderContainer implementations
		public override ProjectSection SolutionItems {
			get {
				foreach (SolutionFolder folder in Folders) {
					if (folder.Name == "Solution Items") {
						return folder.SolutionItems;
					}
				}
				
				SolutionFolder newFolder = CreateFolder("Solution Items");
				return newFolder.SolutionItems;
			}
		}
		
		public override void AddFolder(ISolutionFolder folder)
		{
			base.AddFolder(folder);
			guidDictionary[folder.IdGuid] = folder;
		}
		
		#endregion
		
		SolutionPreferences preferences;
		
		public SolutionPreferences Preferences {
			get {
				return preferences;
			}
		}
		
		public Solution()
		{
			preferences = new SolutionPreferences(this);
			solutionFolderEnumerator          = new SolutionFolderEnumerator(this);
			solutionFolderContainerEnumerator = new SolutionFolderContainerEnumerator(this);
			projectEnumerator                 = new ProjectEnumerator(this);
		}
		
		public ISolutionFolder GetSolutionFolder(string guid)
		{
			foreach (ISolutionFolder solutionFolder in SolutionFolders) {
				if (solutionFolder.IdGuid == guid) {
					return solutionFolder;
				}
			}
			return null;
		}
		
		
		public void Save()
		{
			Save(fileName);
		}
		
		public SolutionFolder CreateFolder(string folderName)
		{
			return new SolutionFolder(folderName, folderName, "{" + Guid.NewGuid().ToString().ToUpper() + "}");
		}
		
		
		public void Save(string fileName)
		{
			this.fileName = fileName;
			string outputDirectory = Path.GetDirectoryName(fileName);
			if (!System.IO.Directory.Exists(outputDirectory)) {
				System.IO.Directory.CreateDirectory(outputDirectory);
			}
			
			StringBuilder projectSection        = new StringBuilder();
			StringBuilder nestedProjectsSection = new StringBuilder();
			
			List<ISolutionFolder> folderList = Folders;
			Stack<ISolutionFolder> stack = new Stack<ISolutionFolder>(folderList.Count);
			// push folders in reverse order because it's a stack
			for (int i = folderList.Count - 1; i >= 0; i--) {
				stack.Push(folderList[i]);
			}
			
			while (stack.Count > 0) {
				ISolutionFolder currentFolder = stack.Pop();
				
				projectSection.Append("Project(\"");
				projectSection.Append(currentFolder.TypeGuid);
				projectSection.Append("\")");
				projectSection.Append(" = ");
				projectSection.Append('"');
				projectSection.Append(currentFolder.Name);
				projectSection.Append("\", ");
				string relativeLocation;
				if (currentFolder is IProject) {
					currentFolder.Location = ((IProject)currentFolder).FileName;
				}
				if (Path.IsPathRooted(currentFolder.Location)) {
					relativeLocation = FileUtility.GetRelativePath(Path.GetDirectoryName(FileName), currentFolder.Location);
				} else {
					relativeLocation = currentFolder.Location;
				}
				projectSection.Append('"');
				projectSection.Append(relativeLocation);
				projectSection.Append("\", ");
				projectSection.Append('"');
				projectSection.Append(currentFolder.IdGuid);
				projectSection.Append("\"");
				projectSection.AppendLine();
				
				if (currentFolder is IProject) {
					// IProject project = (IProject)currentFolder;
					// currently nothing to do. (I don't know if projects may have sections).
				} else if (currentFolder is SolutionFolder) {
					SolutionFolder folder = (SolutionFolder)currentFolder;
					
					foreach (ProjectSection section in folder.Sections) {
						projectSection.Append("\tProjectSection(");
						projectSection.Append(section.Name);
						projectSection.Append(") = ");
						projectSection.Append(section.SectionType);
						projectSection.Append(Environment.NewLine);
						
						section.AppendSection(projectSection, "\t\t");
						
						projectSection.Append("\tEndProjectSection");
						projectSection.Append(Environment.NewLine);
					}
					
					foreach (ISolutionFolder subFolder in folder.Folders) {
						stack.Push(subFolder);
						nestedProjectsSection.Append("\t\t");
						nestedProjectsSection.Append(subFolder.IdGuid);
						nestedProjectsSection.Append(" = ");
						nestedProjectsSection.Append(folder.IdGuid);
						nestedProjectsSection.Append(Environment.NewLine);
					}
				} else {
					LoggingService.Warn("Solution.Load(): unknown folder : " + currentFolder);
				}
				projectSection.Append("EndProject");
				projectSection.Append(Environment.NewLine);
			}
			
			StringBuilder globalSection = new StringBuilder();
			globalSection.Append("Global");
			globalSection.Append(Environment.NewLine);
			foreach (ProjectSection section in Sections) {
				globalSection.Append("\tGlobalSection(");
				globalSection.Append(section.Name);
				globalSection.Append(") = ");
				globalSection.Append(section.SectionType);
				globalSection.Append(Environment.NewLine);
				
				section.AppendSection(globalSection, "\t\t");
				
				globalSection.Append("\tEndGlobalSection");
				globalSection.Append(Environment.NewLine);
			}
			
			// we need to specify UTF8 because MsBuild needs the BOM
			using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.UTF8)) {
				sw.WriteLine("Microsoft Visual Studio Solution File, Format Version 9.00");
				Version v = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
				sw.WriteLine("# SharpDevelop " + v.Major + "." + v.Minor + "." + v.Build + "." + v.Revision);
				sw.Write(projectSection.ToString());
				
				sw.Write(globalSection.ToString());
				
				if (nestedProjectsSection.Length > 0) {
					sw.WriteLine("\tGlobalSection(NestedProjects) = preSolution");
					sw.Write(nestedProjectsSection.ToString());
					sw.WriteLine("\tEndGlobalSection");
				}
				
				sw.WriteLine("EndGlobal");
			}
		}
		
		static Regex versionPattern       = new Regex("Microsoft Visual Studio Solution File, Format Version\\s+(?<Version>.*)", RegexOptions.Compiled);
		
		static Regex projectLinePattern   = new Regex("Project\\(\"(?<ProjectGuid>.*)\"\\)\\s+=\\s+\"(?<Title>.*)\",\\s*\"(?<Location>.*)\",\\s*\"(?<Guid>.*)\"", RegexOptions.Compiled);
		static Regex globalSectionPattern = new Regex("\\s*GlobalSection\\((?<Name>.*)\\)\\s*=\\s*(?<Type>.*)", RegexOptions.Compiled);
		
		static string GetFirstNonCommentLine(TextReader sr)
		{
			string line = "";
			while ((line = sr.ReadLine()) != null) {
				line = line.Trim();
				if (line.Length > 0 && line[0] != '#')
					return line;
			}
			return "";
		}
		
		/// <summary>
		/// Reads the specified solution file. The project-location-guid information is written into the conversion class.
		/// </summary>
		/// <returns>The version number of the solution.</returns>
		public static string ReadSolutionInformation(string solutionFileName, Converter.PrjxToSolutionProject.Conversion conversion)
		{
			string solutionDirectory = Path.GetDirectoryName(solutionFileName);
			using (StreamReader sr = File.OpenText(solutionFileName)) {
				string line = GetFirstNonCommentLine(sr);
				Match match = versionPattern.Match(line);
				if (!match.Success) {
					return null;
				}
				string version = match.Result("${Version}");
				while ((line = sr.ReadLine()) != null) {
					match = projectLinePattern.Match(line);
					if (match.Success) {
						string projectGuid  = match.Result("${ProjectGuid}");
						string title        = match.Result("${Title}");
						string location     = Path.Combine(solutionDirectory, match.Result("${Location}"));
						string guid         = match.Result("${Guid}");
						conversion.NameToGuid.Add(title, new Guid(guid));
						conversion.NameToPath.Add(title, location);
					}
				}
				return version;
			}
		}
		
		static bool SetupSolution(Solution newSolution, string fileName)
		{
			string         solutionDirectory     = Path.GetDirectoryName(fileName);
			ProjectSection nestedProjectsSection = null;
			
			bool needsConversion = false;
			
			using (StreamReader sr = File.OpenText(fileName)) {
				string line = GetFirstNonCommentLine(sr);
				Match match = versionPattern.Match(line);
				if (!match.Success) {
					MessageService.ShowErrorFormatted("${res:SharpDevelop.Solution.InvalidSolutionFile}", fileName);
					return false;
				}
				
				switch (match.Result("${Version}")) {
					case "7.00":
						needsConversion = true;
						if (!MessageService.AskQuestion("${res:SharpDevelop.Solution.ConvertSolutionVersion7}")) {
							return false;
						}
						break;
					case "8.00":
						needsConversion = true;
						if (!MessageService.AskQuestion("${res:SharpDevelop.Solution.ConvertSolutionVersion8}")) {
							return false;
						}
						break;
					case "9.00":
						break;
					default:
						MessageService.ShowErrorFormatted("${res:SharpDevelop.Solution.UnknownSolutionVersion}", match.Result("${Version}"));
						return false;
				}
				
				while (true) {
					line = sr.ReadLine();
					
					if (line == null) {
						break;
					}
					match = projectLinePattern.Match(line);
					if (match.Success) {
						string projectGuid  = match.Result("${ProjectGuid}");
						string title        = match.Result("${Title}");
						string location     = Path.Combine(solutionDirectory, match.Result("${Location}"));
						string guid         = match.Result("${Guid}");
						
						if (projectGuid == FolderGuid) {
							SolutionFolder newFolder = SolutionFolder.ReadFolder(sr, title, location, guid);
							newSolution.AddFolder(newFolder);
						} else {
							IProject newProject;
							if (!File.Exists(location)) {
								newProject = new MissingProject(location);
								newProject.TypeGuid = projectGuid;
							} else {
								ILanguageBinding binding = LanguageBindingService.GetBindingPerProjectFile(location);
								if (binding != null) {
									newProject = binding.LoadProject(location, title);
								} else {
									newProject = new UnknownProject(location);
									newProject.TypeGuid = projectGuid;
								}
							}
							newProject.IdGuid = guid;
							newSolution.AddFolder(newProject);
						}
						match = match.NextMatch();
					} else {
						match = globalSectionPattern.Match(line);
						if (match.Success) {
							ProjectSection newSection = ProjectSection.ReadGlobalSection(sr, match.Result("${Name}"), match.Result("${Type}"));
							// Don't put the NestedProjects section into the global sections list
							// because it's transformed to a tree representation and the tree representation
							// is transformed back to the NestedProjects section during save.
							if (newSection.Name == "NestedProjects") {
								nestedProjectsSection = newSection;
							} else {
								newSolution.Sections.Add(newSection);
							}
						}
					}
				}
			}
			// Create solution folder 'tree'.
			if (nestedProjectsSection != null) {
				foreach (SolutionItem item in nestedProjectsSection.Items) {
					string from = item.Name;
					string to   = item.Location;
					ISolutionFolderContainer folder = newSolution.guidDictionary[to] as ISolutionFolderContainer;
					folder.AddFolder(newSolution.guidDictionary[from]);
				}
			}
			if (needsConversion) {
				// save in new format
				newSolution.Save();
			}
			return true;
		}
		
		static Solution solutionBeingLoaded;
		
		public static Solution SolutionBeingLoaded {
			get {
				return solutionBeingLoaded;
			}
		}
		
		public static Solution Load(string fileName)
		{
			Solution newSolution = new Solution();
			solutionBeingLoaded = newSolution;
			newSolution.Name     = Path.GetFileNameWithoutExtension(fileName);
			
			string extension = Path.GetExtension(fileName).ToUpper();
			if (extension == ".CMBX") {
				if (!MessageService.AskQuestion("${res:SharpDevelop.Solution.ImportCmbx}")) {
					return null;
				}
				newSolution.fileName = Path.ChangeExtension(fileName, ".sln");
				ICSharpCode.SharpDevelop.Project.Converter.CombineToSolution.ConvertSolution(newSolution, fileName);
			} else if (extension == ".PRJX") {
				if (!MessageService.AskQuestion("${res:SharpDevelop.Solution.ImportPrjx}")) {
					return null;
				}
				newSolution.fileName = Path.ChangeExtension(fileName, ".sln");
				ICSharpCode.SharpDevelop.Project.Converter.CombineToSolution.ConvertProject(newSolution, fileName);
			} else {
				newSolution.fileName = fileName;
				if (!SetupSolution(newSolution, fileName)) {
					return null;
				}
			}
			
			solutionBeingLoaded = null;
			return newSolution;
		}
		
		#region System.IDisposable interface implementation
		public void Dispose()
		{
			foreach (IProject project in Projects) {
				project.Dispose();
			}
		}
		#endregion
		
		public CompilerResults Build()
		{
			return MSBuildProject.RunMSBuild(FileName, null);
		}
		
		public CompilerResults Rebuild()
		{
			return MSBuildProject.RunMSBuild(FileName, "Rebuild");
		}
		
		public CompilerResults Clean()
		{
			return MSBuildProject.RunMSBuild(FileName, "Clean");
		}
		
		public CompilerResults Publish()
		{
			return MSBuildProject.RunMSBuild(FileName, "Publish");
		}
		
	}
}
