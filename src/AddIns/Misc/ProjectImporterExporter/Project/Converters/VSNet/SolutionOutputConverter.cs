// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.ProjectImportExporter.Converters
{
	public class SolutionOutputConverter : AbstractOutputConverter
	{
		Hashtable GUIDHash     = new Hashtable();
		Hashtable typeGUIDHash = new Hashtable();
		Hashtable configHash   = new Hashtable();
		
		static Hashtable projectTypeGUIDHash = new Hashtable();
		static Hashtable languageNameHash    = new Hashtable();
		
		static SolutionOutputConverter()
		{
			projectTypeGUIDHash[".csproj"] = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
			projectTypeGUIDHash[".vbproj"] = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";
			languageNameHash[".csproj"] = "CSHARP";
			languageNameHash[".vbproj"] = "VisualBasic";
		}
		
		public override string FormatName {
			get {
				return "Visual Studio.NET 2003 Solutions";
			}
		}
		
		string CreateGUID(string projectFileName)
		{
			string name = Path.GetFileNameWithoutExtension(projectFileName);
			string result = (string)GUIDHash[name];
			if (result == null) {
				result = String.Concat('{', Guid.NewGuid().ToString().ToUpper(), '}');
				GUIDHash[name] = result;
			}
			return result;
		}
		
		
		string GetVSNetProjectFileExtension(string projectFileName)
		{
			XmlTextReader reader = new XmlTextReader(projectFileName);
			reader.MoveToContent();
			if (reader.MoveToAttribute("projecttype")) {
				switch (reader.Value) {
					case "C#":
						return ".csproj";
					case "VBNET":
						return ".vbproj";
					default:
						throw new System.NotSupportedException("Project type " + reader.Value + " is currently not supported.");
				}
			}
			return ".csproj";
		}
		
		public override void ConvertCombine(IProgressMonitor progressMonitor, string inputCombine, string outputPath)
		{
			string inputPath = Path.GetFullPath(Path.GetDirectoryName(inputCombine));
			
			Combine combine = new Combine();
			combine.LoadCombine(inputCombine);
			
			StreamWriter streamWriter = new StreamWriter(Path.Combine(outputPath, Path.ChangeExtension(Path.GetFileName(inputCombine), ".sln")));
			streamWriter.WriteLine("Microsoft Visual Studio Solution File, Format Version 8.00");
			
			ArrayList projects = Combine.GetAllProjects(combine);
			if (progressMonitor != null) {
				progressMonitor.BeginTask("Convert", projects.Count + 2);
			}
			foreach (ProjectCombineEntry project in projects) {
				string projectFileName  = Path.GetFullPath(Path.Combine(inputPath, project.Filename));
				string relativeFileName = projectFileName.Substring(inputPath.Length + 1);
				string newExtension = GetVSNetProjectFileExtension(projectFileName);
				//CreateGUID(projectFileName);
				streamWriter.WriteLine("Project(\"{3}\") = \"{1}\", \"{2}\", \"{0}\"",
				                       CreateGUID(projectFileName),
				                       project.Name,
				                       Path.ChangeExtension(relativeFileName, newExtension),
				                       projectTypeGUIDHash[newExtension]);
				typeGUIDHash[Path.GetFileNameWithoutExtension(projectFileName)] = projectTypeGUIDHash[newExtension];
				streamWriter.WriteLine("\tProjectSection(ProjectDependencies) = postProject");
				streamWriter.WriteLine("\tEndProjectSection");
			}
			if (progressMonitor != null) {
				progressMonitor.Worked(1);
			}
			foreach (ProjectCombineEntry project in projects) {
				string projectFileName  = Path.GetFullPath(Path.Combine(inputPath, project.Filename));
				string relativeFileName = projectFileName.Substring(inputPath.Length + 1);
				ConvertProject(null, projectFileName, Path.Combine(outputPath, Path.GetDirectoryName(relativeFileName)));
				if (progressMonitor != null) {
					progressMonitor.Worked(1);
				}
			}
			streamWriter.WriteLine("EndProject");
			streamWriter.WriteLine("Global");
			
			streamWriter.WriteLine("\tGlobalSection(SolutionConfiguration) = preSolution");
			SortedList allConfigs = new SortedList();
			foreach (ProjectCombineEntry project in projects) {
				string projectFileName  = Path.GetFullPath(Path.Combine(inputPath, project.Filename));
				foreach (string config in (ArrayList)configHash[projectFileName]) {
					allConfigs[config] = "1";
				}
			}
			foreach (DictionaryEntry entry in allConfigs) {
				streamWriter.WriteLine("\t\t{0} = {0}", entry.Key);
			}
			streamWriter.WriteLine("\tEndGlobalSection");
			streamWriter.WriteLine("\tGlobalSection(ProjectConfiguration) = postSolution");
			foreach (ProjectCombineEntry project in projects) {
				string projectFileName  = Path.GetFullPath(Path.Combine(inputPath, project.Filename));
				foreach (string config in (ArrayList)configHash[projectFileName]) {
					string name = Path.GetFileNameWithoutExtension(projectFileName);
					streamWriter.WriteLine("\t\t{0}.{1}.ActiveCfg = {1}|.NET", GUIDHash[name], config);
					streamWriter.WriteLine("\t\t{0}.{1}.Build.0 = {1}|.NET", GUIDHash[name], config);
				}
			}
			streamWriter.WriteLine("\tEndGlobalSection");
			streamWriter.WriteLine("\tGlobalSection(ExtensibilityGlobals) = postSolution");
			streamWriter.WriteLine("\tEndGlobalSection");
			streamWriter.WriteLine("\tGlobalSection(ExtensibilityAddIns) = postSolution");
			streamWriter.WriteLine("\tEndGlobalSection");
			streamWriter.WriteLine("EndGlobal");
			streamWriter.Close();
			if (progressMonitor != null) {
				progressMonitor.Done();
			}
		}
		
		public override void ConvertProject(IProgressMonitor progressMonitor, string inputProject, string outputPath)
		{
			if (progressMonitor != null) {
				progressMonitor.BeginTask("Convert", 3);
			}
			
			string newExtension = GetVSNetProjectFileExtension(inputProject);
			string frameworkPath = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
			if (!frameworkPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
				frameworkPath += Path.DirectorySeparatorChar;
			
			XsltArgumentList xsltArgumentList = new XsltArgumentList();
			xsltArgumentList.AddParam("LanguageName", "", languageNameHash[newExtension]);
			xsltArgumentList.AddParam("FrameworkPath", "", frameworkPath);
			SolutionOutputConverterTool solutionConversionTool = new SolutionOutputConverterTool();
			solutionConversionTool.projectGUIDHash = GUIDHash;
			solutionConversionTool.projectTypeGUIDHash = projectTypeGUIDHash;
			xsltArgumentList.AddExtensionObject("urn:convtool", solutionConversionTool);
			
			string outputFile = Path.Combine(outputPath, Path.ChangeExtension(Path.GetFileName(inputProject), newExtension));
			if (!Directory.Exists(Path.GetDirectoryName(outputFile))) {
				Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
			}
			if (progressMonitor != null) {
				progressMonitor.Worked(1);
			}

			ConvertXml.Convert(inputProject,
			                   new XmlTextReader(Assembly.GetCallingAssembly().GetManifestResourceStream("ProjectToVSNetProjectConversion.xsl")),
			                   outputFile,
			                   xsltArgumentList);
			if (progressMonitor != null) {
				progressMonitor.Worked(1);
			}

			foreach (string fileName in solutionConversionTool.copiedFiles) {
				string srcFile = Path.Combine(Path.GetDirectoryName(inputProject), fileName);
				string dstFile = Path.Combine(outputPath, fileName);
				if (srcFile.ToLower() == dstFile.ToLower()) continue;
				if (File.Exists(srcFile)) {
					if (!Directory.Exists(Path.GetDirectoryName(dstFile))) {
						Directory.CreateDirectory(Path.GetDirectoryName(dstFile));
					}
					File.Copy(srcFile, dstFile, true);
					File.SetAttributes(dstFile, FileAttributes.Normal);
				}
			}
			configHash[inputProject] = solutionConversionTool.configurations;
			if (progressMonitor != null) {
				progressMonitor.Done();
			}
		}
	}
}
