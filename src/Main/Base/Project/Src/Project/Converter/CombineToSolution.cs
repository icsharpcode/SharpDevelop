// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project.Converter
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public static class CombineToSolution
	{
		static string ReadContent(string fileName)
		{
			using (StreamReader sr = File.OpenText(fileName)) {
				return sr.ReadToEnd();
			}
		}
		static Regex combineLinePattern  = new Regex("<Entry filename=\"(?<FileName>.*)\"", RegexOptions.Compiled);
		
		static void ReadProjects(Solution newSolution, string fileName, List<string> projectFiles)
		{
			string solutionDirectory = Path.GetDirectoryName(fileName);
			string content           = ReadContent(fileName);
			
			Match match = combineLinePattern.Match(content);
			while (match.Success) {
				string path = Path.Combine(solutionDirectory, match.Result("${FileName}"));
				if (Path.GetExtension(path).ToUpper() == ".CMBX") {
					ReadProjects(newSolution, path, projectFiles);
				} else {
					projectFiles.Add(path);
				}
				match = match.NextMatch();
			}
		}
		
		public static void ConvertSolution(Solution newSolution, string fileName)
		{
			List<string> projectFiles = new List<string>();
			ReadProjects(newSolution, fileName, projectFiles);
			
			PrjxToSolutionProject.Conversion conversion = new PrjxToSolutionProject.Conversion();
			
			foreach (string path in projectFiles) {
				string name = PrjxToSolutionProject.Conversion.GetProjectName(path);
				conversion.NameToGuid[name] = Guid.NewGuid();
				conversion.NameToPath[name] = Path.ChangeExtension(path, ".csproj");
			}
			foreach (string path in projectFiles) {
				IProject newProject = PrjxToSolutionProject.ConvertOldProject(path, conversion);
				newSolution.AddFolder(newProject);
			}
			newSolution.Save();
		}
	}
}
