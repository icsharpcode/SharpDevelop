// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project.Converter
{
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
				if (".CMBX".Equals(Path.GetExtension(path), StringComparison.OrdinalIgnoreCase)) {
					ReadProjects(newSolution, path, projectFiles);
				} else {
					projectFiles.Add(path);
				}
				match = match.NextMatch();
			}
		}
		
		static bool IsVisualBasic(string prjx)
		{
			using (XmlTextReader reader = new XmlTextReader(prjx)) {
				reader.Read();
				return reader.GetAttribute("projecttype") == "VBNET";
			}
		}
		
		public static void ConvertSolution(Solution newSolution, string fileName)
		{
			List<string> projectFiles = new List<string>();
			ReadProjects(newSolution, fileName, projectFiles);
			Convert(newSolution, projectFiles);
		}
		
		public static void ConvertProject(Solution newSolution, string projectFileName)
		{
			List<string> projectFiles = new List<string>();
			projectFiles.Add(projectFileName);
			Convert(newSolution, projectFiles);
		}
		
		static void Convert(Solution newSolution, List<string> projectFiles)
		{
			PrjxToSolutionProject.Conversion conversion = new PrjxToSolutionProject.Conversion();
			
			foreach (string path in projectFiles) {
				string name = PrjxToSolutionProject.Conversion.GetProjectName(path);
				conversion.NameToGuid[name] = Guid.NewGuid();
				if (IsVisualBasic(path))
					conversion.NameToPath[name] = Path.ChangeExtension(path, ".vbproj");
				else
					conversion.NameToPath[name] = Path.ChangeExtension(path, ".csproj");
			}
			foreach (string path in projectFiles) {
				conversion.IsVisualBasic = IsVisualBasic(path);
				IProject newProject = PrjxToSolutionProject.ConvertOldProject(path, conversion, newSolution);
				newSolution.AddFolder(newProject);
			}
			if (conversion.Resources != null) {
				const string resourceWarning = "${res:SharpDevelop.Solution.ImportResourceWarning}";
				if (conversion.Resources.Count == 0) {
					MessageService.ShowMessage(resourceWarning);
				} else {
					StringBuilder txt = new StringBuilder(resourceWarning);
					txt.AppendLine();
					txt.AppendLine();
					txt.AppendLine("${res:SharpDevelop.Solution.ImportResourceWarningErrorText}");
					foreach (string r in conversion.Resources)
						txt.AppendLine(r);
					MessageService.ShowMessage(txt.ToString());
				}
			}
			newSolution.Save();
		}
	}
}
