using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public class MSBuildProject : AbstractProject
	{
		public MSBuildProject()
		{
		}
		
		public void Create(ProjectCreateInformation information)
		{
			Name = information.ProjectName;
			configurations[""] = new PropertyGroup();
			IdGuid = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
			BaseConfiguration["OutputType"]    = "Exe";
			BaseConfiguration["RootNamespace"] = "RootNameSpace";
			BaseConfiguration["AssemblyName"]  = "a";
			BaseConfiguration["Configuration"] = "Debug";
			BaseConfiguration.SetIsGuarded("Configuration", true);
			BaseConfiguration["Platform"]      = "AnyCPU";
			BaseConfiguration.SetIsGuarded("Platform", true);
			
			configurations["Debug|AnyCPU"] = new PropertyGroup();
			configurations["Debug|AnyCPU"]["OutputPath"] = @"bin\Debug\";
			configurations["Debug|AnyCPU"]["Optimize"] = @"false";
			
			configurations["Release|AnyCPU"] = new PropertyGroup();
			configurations["Release|AnyCPU"]["OutputPath"] = @"bin\Release\";
			configurations["Release|AnyCPU"]["Optimize"] = @"true";
			
			fileName = information.OutputProjectFileName;
			
			imports.Add(@"$(MSBuildBinPath)\Microsoft.CSHARP.Targets");
		}
		
		public override bool CanCompile(string fileName)
		{
			return true;
		}
		
		#region Xml reading routines
		static PropertyGroup ReadPropertyGroup(XmlTextReader reader)
		{
			PropertyGroup properties = new PropertyGroup();
			PropertyGroup.ReadProperties(reader, properties, "PropertyGroup");
			return properties;
		}
		
		readonly static Regex configurationRegEx = new Regex(@"\s*'.*'\s*==\s*'(?<configuration>.*)'", RegexOptions.Compiled);
		
		protected void SetupProject(string projectFileName)
		{	
			this.fileName = projectFileName;
			using (XmlTextReader reader = new XmlTextReader(projectFileName)) {
				while (reader.Read()){
					if (reader.IsStartElement()) {
						switch (reader.LocalName) {
							case "PropertyGroup":
								string condition = reader.GetAttribute("Condition");
								
								PropertyGroup propertyGroup = ReadPropertyGroup(reader);
								if (condition == null) {
									condition = String.Empty;
								}
								
								string configuration;
								Match match = configurationRegEx.Match(condition);
								if (match.Success) {
									configuration = match.Result("${configuration}");
								} else {
									configuration = condition;
								}
								if (!configurations.ContainsKey(configuration)) {
									configurations[configuration] = propertyGroup;
								} else {
									configurations[configuration].Merge(propertyGroup);
								}
								break;
							case "ItemGroup":
								ProjectItem.ReadItemGroup(reader, this, Items);
								break;
							case "Import":
								string import = reader.GetAttribute("Project");
								Imports.Add(import);
								break;
						}
					}
				}
			}
			
			string userSettingsFileName = projectFileName + ".user";
			if (File.Exists(userSettingsFileName)) {
				using (XmlTextReader reader = new XmlTextReader(userSettingsFileName)) {
					while (reader.Read()){
						if (reader.IsStartElement()) {
							switch (reader.LocalName) {
								case "PropertyGroup":
									string condition = reader.GetAttribute("Condition");
									PropertyGroup propertyGroup = ReadPropertyGroup(reader);
									if (condition == null) {
										condition = String.Empty;
									}
									
									string configuration;
									Match match = configurationRegEx.Match(condition);
									if (match.Success) {
										configuration = match.Result("${configuration}");
									} else {
										configuration = condition;
									}
									if (!userConfigurations.ContainsKey(configuration)) {
										userConfigurations[configuration] = propertyGroup;
									} else {
										userConfigurations[configuration].Merge(propertyGroup);
									}
									break;
							}
						}
					}
				}
			}
		}
		
		public override void Save(string fileName)
		{
			string outputDirectory = Path.GetDirectoryName(fileName);
			if (!System.IO.Directory.Exists(outputDirectory)) {
				System.IO.Directory.CreateDirectory(outputDirectory);
			}
			using (XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.Default)) {
				writer.Formatting = Formatting.Indented;
				
				writer.WriteStartElement("Project");
				// 				writer.WriteAttributeString("MSBuildVersion", "2.0");
				writer.WriteAttributeString("DefaultTargets", "Build");
				writer.WriteAttributeString("xmlns", "http://schemas.microsoft.com/developer/msbuild/2003");
				BaseConfiguration["ProjectGuid"] = IdGuid;
				
				foreach (KeyValuePair<string, PropertyGroup> entry in configurations) {
					writer.WriteStartElement("PropertyGroup");
					if (entry.Key != null && entry.Key.Length > 0) {
						if (entry.Key.Contains("|")) {
							writer.WriteAttributeString("Condition", " '$(Configuration)|$(Platform)' == '" + entry.Key + "' ");
						} else {
							writer.WriteAttributeString("Condition", " '$(Configuration)' == '" + entry.Key + "' ");
						}
					}
					entry.Value.WriteProperties(writer);
					writer.WriteEndElement();
				}
				
				List<ProjectItem> references   = new List<ProjectItem>();
				List<ProjectItem> projectFiles = new List<ProjectItem>();
				List<ProjectItem> other        = new List<ProjectItem>();
				
				foreach (ProjectItem item in this.items) {
					switch (item.ItemType) {
						case ItemType.Reference:
							references.Add(item);
							break;
						case ItemType.Compile:
						case ItemType.EmbeddedResource:
						case ItemType.None:
							projectFiles.Add(item);
							break;
						default:
							other.Add(item);
							break;
					}
				}
				
				if (references.Count > 0) {
					ProjectItem.WriteItemGroup(writer, references);
				}
				
				if (projectFiles.Count > 0) {
					ProjectItem.WriteItemGroup(writer, projectFiles);
				}
				
				if (other.Count > 0) {
					ProjectItem.WriteItemGroup(writer, other);
				}
				
				foreach (string import in Imports) {
					writer.WriteStartElement("Import");
					writer.WriteAttributeString("Project", import);
					writer.WriteEndElement();
				}
				
				writer.WriteEndElement();
			}
			
			if (userConfigurations.Count > 0) {
				string userSettingsFileName = fileName + ".user";
				using (XmlTextWriter writer = new XmlTextWriter(userSettingsFileName, Encoding.Default)) {
					writer.Formatting = Formatting.Indented;
					writer.WriteStartElement("Project");
					writer.WriteAttributeString("xmlns", "http://schemas.microsoft.com/developer/msbuild/2003");
					
					foreach (KeyValuePair<string, PropertyGroup> entry in userConfigurations) {
						writer.WriteStartElement("PropertyGroup");
						if (entry.Key != null && entry.Key.Length > 0) {
							if (entry.Key.Contains("|")) {
								writer.WriteAttributeString("Condition", " '$(Configuration)|$(Platform)' == '" + entry.Key + "' ");
							} else {
								writer.WriteAttributeString("Condition", " '$(Configuration)' == '" + entry.Key + "' ");
							}
						}
						entry.Value.WriteProperties(writer);
						writer.WriteEndElement();
					}
					
					writer.WriteEndElement();
				}
			}
		}
		#endregion
		
		public override void Start(bool withDebugging)
		{
			DebuggerService.StartWithoutDebugging(OutputAssemblyFullPath,
			                                      Path.GetDirectoryName(OutputAssemblyFullPath),
			                                      "");
			
		}
		
		
//		static void BeforeBuild()
//		{
//			TaskService.NotifyTaskChange();
//			
//			StatusBarService.SetMessage("${res:MainWindow.StatusBar.CompilingMessage}");
//			
//			StringParser.Properties["Project"] = this.Name;
//			
//			TaskService.BuildMessageViewCategory.AppendText(StringParser.Parse("${res:MainWindow.CompilerMessages.BuildStartedOutput}", new string[,] {
//			                                                                 	{"PROJECT", this.Name},
//			                                                                 	{"CONFIG", this.Configuration + "|" + this.Platform}
//			                                                                   }) + Environment.NewLine);
//			TaskService.BuildMessageViewCategory.AppendText(StringParser.Parse("${res:MainWindow.CompilerMessages.PerformingMainCompilationOutput}") + Environment.NewLine);
//			// TODO :BEFORE COMPILE ACTION.
//			//TaskService.CompilerOutput += StringParser.Parse("${res:MainWindow.CompilerMessages.ExecuteScript}", new string[,] { {"SCRIPT", conf.ExecuteBeforeBuild} }) + "\n";
//		}
//		
//		static void AfterBuild()
//		{
//			// TODO: After COMPILE ACTION.
//			//TaskService.CompilerOutput += StringParser.Parse("${res:MainWindow.CompilerMessages.ExecuteScript}", new string[,] { {"SCRIPT", conf.ExecuteAfterBuild} }) + "\n";
//			
//			TaskService.BuildMessageViewCategory.AppendText(StringParser.Parse("${res:MainWindow.CompilerMessages.ProjectStatsOutput}", new string[,] { {"ERRORS", TaskService.Errors.ToString()}, {"WARNINGS", TaskService.Warnings.ToString()} }) + Environment.NewLine + Environment.NewLine);
//			isDirty = TaskService.Errors != 0;
//		}
//		
		readonly static Regex normalError   = new Regex(@"^(?<file>\S.*)\((?<line>\d+),(?<column>\d+)\):\s+(?<error>\w+)\s+(?<number>[\d\w]+):\s+(?<message>.*)$", RegexOptions.Compiled);
		readonly static Regex compilerError = new Regex(@"^(?<who>[^:]*):\s+(?<error>\w+)\s+(?<number>[\d\w]+):\s+(?<message>.*)$", RegexOptions.Compiled);
		readonly static Regex generalError  = new Regex(@"^(?<error>\S.+)\s+(?<number>[\d\w]+):\s+(?<message>.*)$", RegexOptions.Compiled);
		
		readonly static Regex projectName   = new Regex(@"^Project\s+\""[^""]*\""[^""]*\""(?<name>[^""]*)\""", RegexOptions.Compiled);
		
		
		static CompilerError GetCompilerError(string line, string workingPath)
		{
			Match match = normalError.Match(line);
			if (match.Success) {
				CompilerError error = new CompilerError();
				error.Column      = Int32.Parse(match.Result("${column}"));
				error.Line        = Int32.Parse(match.Result("${line}"));
				error.FileName    = Path.Combine(workingPath, match.Result("${file}"));
				error.IsWarning   = match.Result("${error}") == "warning";
				error.ErrorNumber = match.Result("${number}");
				error.ErrorText   = match.Result("${message}");
				return error;
			}
			match = compilerError.Match(line);
			if (match.Success) {
				CompilerError error = new CompilerError();
				error.IsWarning   = match.Result("${error}") == "warning";
				error.ErrorNumber = match.Result("${number}");
				error.ErrorText   = match.Result("${who}") + ":" + match.Result("${message}");
				return error;
			}
			
			match = generalError.Match(line);
			if (match.Success) {
				CompilerError error = new CompilerError();
				error.IsWarning   = match.Result("${error}") == "warning";
				error.ErrorNumber = match.Result("${number}");
				error.ErrorText   = match.Result("${message}");
				return error;
			}
			return null;
		}
		
		public static CompilerResults RunMSBuild(string fileName, string target)
		{
			WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();
			CompilerResults results = new CompilerResults(null);
//			BeforeBuild();
			string runtimeDirectory    = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
			ProcessStartInfo startInfo = new ProcessStartInfo("\"" + Path.Combine(runtimeDirectory, "msbuild.exe") + "\"");
			if (target != null) {
				startInfo.Arguments = "/nologo /verbosity:m \"" + fileName + "\" \"/t:" + target + "\"";
			} else {
				startInfo.Arguments = "/nologo /verbosity:m \"" + fileName;
			}
			string workingDirectory = Path.GetDirectoryName(fileName);
			startInfo.WorkingDirectory = workingDirectory;
			
			startInfo.UseShellExecute         = false;
			startInfo.RedirectStandardOutput  = true;
			Process p = Process.Start(startInfo);
			
			StreamReader reader = p.StandardOutput;
			while (!p.HasExited) {
				string line = reader.ReadLine();
				if (line != null) {
					TaskService.BuildMessageViewCategory.AppendText(line + Environment.NewLine);
					Match match = projectName.Match(line);
					if (match.Success) {
						string name = match.Result("${name}");
						if (name != null) {
							workingDirectory = Path.GetDirectoryName(name);
						}
					} else {
						CompilerError error = GetCompilerError(line, workingDirectory);
						if (error != null) {
							results.Errors.Add(error);
						}
					} 
					results.Output.Add(line);
				}
				System.Windows.Forms.Application.DoEvents();
			}
			while (true) {
				string line = reader.ReadLine();
				if (line == null) {
					break;
				}
				TaskService.BuildMessageViewCategory.AppendText(line + Environment.NewLine);
				Match match = projectName.Match(line);
				if (match.Success) {
					string name = match.Result("${name}");
					if (name != null) {
						workingDirectory = Path.GetDirectoryName(name);
					}
				} else {
					CompilerError error = GetCompilerError(line, workingDirectory);
					if (error != null) {
						results.Errors.Add(error);
					}
				} 
				results.Output.Add(line);
			}
//			TaskService.BuildMessageViewCategory.AppendText(reader.ReadToEnd() + Environment.NewLine);
			p.WaitForExit();
//			AfterBuild();
			return results;
		}
		
		public override CompilerResults Build()
		{
			return RunMSBuild(FileName, "Build");
		}
		
		public override CompilerResults Rebuild()
		{
			return RunMSBuild(FileName, "Rebuild");
		}
		
		public override CompilerResults Clean()
		{
			CompilerResults result = RunMSBuild(FileName, "Clean");
			isDirty = true;
			return result;
		}
		
		public override CompilerResults Publish()
		{
			return RunMSBuild(FileName, "Publish");
		}
		
		public override string ToString()
		{
			return String.Format("[MSBuildProject: FileName={0}, Name={1}, Items={2}]",
			                     FileName,
			                     Name,
			                     Items.Count);
		}
	}
}
