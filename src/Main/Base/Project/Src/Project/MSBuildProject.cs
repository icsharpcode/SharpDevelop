// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
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
		List<string> unknownXmlSections     = new List<string>();
		List<string> userUnknownXmlSections = new List<string>();
		protected char BuildConstantSeparator = ';';
		
		public MSBuildProject()
		{
		}
		
		protected virtual void Create(ProjectCreateInformation information)
		{
			Name = information.ProjectName;
			IdGuid = "{" + Guid.NewGuid().ToString().ToUpperInvariant() + "}";
			BaseConfiguration["OutputType"]    = "Exe";
			BaseConfiguration["RootNamespace"] = information.ProjectName;
			BaseConfiguration["AssemblyName"]  = information.ProjectName;
			BaseConfiguration["Configuration"] = "Debug";
			BaseConfiguration.SetIsGuarded("Configuration", true);
			BaseConfiguration["Platform"]      = "AnyCPU";
			BaseConfiguration.SetIsGuarded("Platform", true);
			
			configurations["Debug|*"] = new PropertyGroup();
			configurations["Debug|*"]["BaseIntermediateOutputPath"] = @"obj\";
			configurations["Debug|*"]["IntermediateOutputPath"] = @"obj\Debug\";
			if (information.CreateProjectWithDefaultOutputPath) {
				configurations["Debug|*"]["OutputPath"] = @"bin\Debug\";
			}
			configurations["Debug|*"]["Optimize"] = "False";
			configurations["Debug|*"]["DefineConstants"] = "DEBUG" + BuildConstantSeparator + "TRACE";
			configurations["Debug|*"]["DebugSymbols"] = "True";
			configurations["Debug|*"]["DebugType"] = "Full";
			
			configurations["Release|*"] = new PropertyGroup();
			configurations["Release|*"]["BaseIntermediateOutputPath"] = @"obj\";
			configurations["Release|*"]["IntermediateOutputPath"] = @"obj\Release\";
			if (information.CreateProjectWithDefaultOutputPath) {
				configurations["Release|*"]["OutputPath"] = @"bin\Release\";
			}
			configurations["Release|*"]["Optimize"] = "True";
			configurations["Release|*"]["DefineConstants"] = "TRACE";
			configurations["Release|*"]["DebugSymbols"] = "False";
			configurations["Release|*"]["DebugType"] = "None";
			
			this.FileName = Path.GetFullPath(information.OutputProjectFileName);
		}
		
		public override bool CanCompile(string fileName)
		{
			return true;
		}
		
		#region Xml reading routines
		public override ProjectItem CreateProjectItem(string itemType)
		{
			return ProjectItemFactory.CreateProjectItem(this, itemType);
		}

		static PropertyGroup ReadPropertyGroup(XmlReader reader)
		{
			PropertyGroup properties = new PropertyGroup();
			PropertyGroup.ReadProperties(reader, properties, "PropertyGroup");
			return properties;
		}
		
		readonly static Regex configurationRegEx = new Regex(@"\s*'(?<property>[^']*)'\s*==\s*'(?<value>[^']*)'", RegexOptions.Compiled);
		
		protected void SetupProject(string projectFileName)
		{
			this.FileName = Path.GetFullPath(projectFileName);
			using (MSBuildFileReader reader = new MSBuildFileReader(projectFileName)) {
				reader.WhitespaceHandling = WhitespaceHandling.Significant;
				reader.Namespaces = false;
				reader.MoveToContent(); // we have to skip over the XmlDeclaration (if it exists)
				if (reader.Name == "VisualStudioProject") {
					reader.Close();
					Converter.PrjxToSolutionProject.ConvertVSNetProject(projectFileName);
					SetupProject(projectFileName);
					return;
				}
				while (reader.Read()) {
					if (reader.IsStartElement()) {
						switch (reader.LocalName) {
							case "PropertyGroup":
								LoadPropertyGroup(reader, false);
								break;
							case "ItemGroup":
								ProjectItem.ReadItemGroup(reader, this, Items);
								break;
							case "Import":
								string import = reader.GetAttribute("Project");
								Imports.Add(import);
								break;
							default:
								unknownXmlSections.Add(reader.ReadOuterXml());
								break;
						}
					}
				}
			}
			
			string userSettingsFileName = projectFileName + ".user";
			if (File.Exists(userSettingsFileName)) {
				using (MSBuildFileReader reader = new MSBuildFileReader(userSettingsFileName)) {
					reader.WhitespaceHandling = WhitespaceHandling.Significant;
					reader.MoveToContent(); // we have to skip over the XmlDeclaration (if it exists)
					while (reader.Read()){
						if (reader.IsStartElement()) {
							switch (reader.LocalName) {
								case "PropertyGroup":
									LoadPropertyGroup(reader, true);
									break;
								default:
									userUnknownXmlSections.Add(reader.ReadOuterXml());
									break;
							}
						}
					}
				}
			}
		}
		
		void LoadPropertyGroup(XmlReader reader, bool isUserFile)
		{
			string condition = reader.GetAttribute("Condition");
			if (condition == null) {
				if (isUserFile)
					UserBaseConfiguration.Merge(ReadPropertyGroup(reader));
				else
					BaseConfiguration.Merge(ReadPropertyGroup(reader));
				return;
			}
			Match match = configurationRegEx.Match(condition);
			if (match.Success) {
				Dictionary<string, PropertyGroup> configurations = isUserFile ? this.userConfigurations : this.configurations;
				
				string conditionProperty = match.Result("${property}");
				string configuration = match.Result("${value}");
				if (conditionProperty == "$(Configuration)|$(Platform)") {
					// configuration is ok
				} else if (conditionProperty == "$(Configuration)") {
					configuration += "|*";
				} else if (conditionProperty == "$(Platform)") {
					configuration = "*|" + configuration;
				} else {
					configuration = null;
				}
				if (configuration != null) {
					PropertyGroup propertyGroup = ReadPropertyGroup(reader);
					if (!configurations.ContainsKey(configuration)) {
						configurations[configuration] = propertyGroup;
					} else {
						configurations[configuration].Merge(propertyGroup);
					}
					return;
				}
			}
			if (isUserFile)
				userUnknownXmlSections.Add(reader.ReadOuterXml());
			else
				unknownXmlSections.Add(reader.ReadOuterXml());
		}
		
		public override void Save(string fileName)
		{
			string outputDirectory = Path.GetDirectoryName(fileName);
			if (!System.IO.Directory.Exists(outputDirectory)) {
				System.IO.Directory.CreateDirectory(outputDirectory);
			}
			using (MSBuildFileWriter writer = new MSBuildFileWriter(fileName, Encoding.UTF8)) {
				writer.Formatting = Formatting.Indented;
				writer.Namespaces = false;
				
				writer.WriteStartElement("Project");
				// 				writer.WriteAttributeString("MSBuildVersion", "2.0");
				writer.WriteAttributeString("DefaultTargets", "Build");
				writer.WriteAttributeString("xmlns", "http://schemas.microsoft.com/developer/msbuild/2003");
				BaseConfiguration["ProjectGuid"] = IdGuid;
				
				SaveProperties(writer, BaseConfiguration, configurations);
				
				List<ProjectItem> references   = new List<ProjectItem>();
				List<ProjectItem> imports      = new List<ProjectItem>();
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
						case ItemType.Import:
							imports.Add(item);
							break;
						default:
							other.Add(item);
							break;
					}
				}
				
				if (references.Count > 0) {
					ProjectItem.WriteItemGroup(writer, references);
				}
				
				if (imports.Count > 0) {
					ProjectItem.WriteItemGroup(writer, imports);
				}
				
				if (projectFiles.Count > 0) {
					ProjectItem.WriteItemGroup(writer, projectFiles);
				}
				
				if (other.Count > 0) {
					ProjectItem.WriteItemGroup(writer, other);
				}
				
				SaveUnknownXmlSections(writer, unknownXmlSections);
				
				foreach (string import in Imports) {
					writer.WriteStartElement("Import");
					writer.WriteAttributeString("Project", import);
					writer.WriteEndElement();
				}
				
				writer.WriteEndElement();
			}
			
			string userSettingsFileName = fileName + ".user";
			if (userConfigurations.Count > 0 || UserBaseConfiguration.PropertyCount > 0 || File.Exists(userSettingsFileName)) {
				using (MSBuildFileWriter writer = new MSBuildFileWriter(userSettingsFileName, Encoding.UTF8)) {
					writer.Formatting = Formatting.Indented;
					writer.Namespaces = false;
					writer.WriteStartElement("Project");
					writer.WriteAttributeString("xmlns", "http://schemas.microsoft.com/developer/msbuild/2003");
					
					SaveProperties(writer, UserBaseConfiguration, userConfigurations);
					SaveUnknownXmlSections(writer, userUnknownXmlSections);
					
					writer.WriteEndElement();
				}
			}
		}
		
		static void SaveProperties(MSBuildFileWriter writer, PropertyGroup baseConfiguration, Dictionary<string, PropertyGroup> configurations)
		{
			if (baseConfiguration.PropertyCount > 0) {
				writer.WriteStartElement("PropertyGroup");
				baseConfiguration.WriteProperties(writer);
				writer.WriteEndElement();
			}
			foreach (KeyValuePair<string, PropertyGroup> entry in configurations) {
				// Skip empty groups
				if (entry.Value.PropertyCount == 0) {
					continue;
				}
				writer.WriteStartElement("PropertyGroup");
				if (entry.Key.StartsWith("*|")) {
					writer.WriteAttributeString("Condition", " '$(Platform)' == '" + entry.Key.Substring(2) + "' ");
				} else if (entry.Key.EndsWith("|*")) {
					writer.WriteAttributeString("Condition", " '$(Configuration)' == '" + entry.Key.Substring(0, entry.Key.Length - 2) + "' ");
				} else {
					writer.WriteAttributeString("Condition", " '$(Configuration)|$(Platform)' == '" + entry.Key + "' ");
				}
				entry.Value.WriteProperties(writer);
				writer.WriteEndElement();
			}
		}
		
		static void SaveUnknownXmlSections(MSBuildFileWriter writer, List<string> unknownElements)
		{
			foreach (string element in unknownElements) {
				// round-trip xml text again for better formatting
				MSBuildFileReader reader = new MSBuildFileReader(new StringReader(element));
				writer.WriteNode(reader, false);
				reader.Close();
			}
		}
		#endregion
		
		#region Start / Run
		public override bool IsStartable {
			get {
				switch (this.StartAction) {
					case StartAction.Project:
						return OutputType == OutputType.Exe || OutputType == OutputType.WinExe;
					case StartAction.Program:
						return this.StartProgram.Length > 0;
					case StartAction.StartURL:
						return this.StartUrl.Length > 0;
				}
				return false;
			}
		}
		
		void Start(string program, bool withDebugging)
		{
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = Path.Combine(Directory, program);
			string workingDir = StringParser.Parse(this.StartWorkingDirectory);
			if (workingDir.Length == 0) {
				psi.WorkingDirectory = Path.GetDirectoryName(psi.FileName);
			} else {
				psi.WorkingDirectory = Path.Combine(Directory, workingDir);
			}
			psi.Arguments = StringParser.Parse(this.StartArguments);
			
			if (!File.Exists(psi.FileName)) {
				MessageService.ShowError(psi.FileName + " does not exist and cannot be started.");
				return;
			}
			
			if (withDebugging) {
				DebuggerService.CurrentDebugger.Start(psi);
			} else {
				DebuggerService.CurrentDebugger.StartWithoutDebugging(psi);
			}
		}
		
		public override void Start(bool withDebugging)
		{
			switch (this.StartAction) {
				case StartAction.Project:
					Start(this.OutputAssemblyFullPath, withDebugging);
					break;
				case StartAction.Program:
					Start(this.StartProgram, withDebugging);
					break;
				case StartAction.StartURL:
					FileService.OpenFile("browser://" + this.StartUrl);
					break;
				default:
					throw new System.ComponentModel.InvalidEnumArgumentException("StartAction", (int)this.StartAction, typeof(StartAction));
			}
		}
		
		[Browsable(false)]
		public string StartProgram {
			get {
				return GetProperty("StartProgram");
			}
			set {
				SetProperty("StartProgram", value);
			}
		}
		
		[Browsable(false)]
		public string StartUrl {
			get {
				return GetProperty("StartURL");
			}
			set {
				SetProperty("StartURL", value);
			}
		}
		
		[Browsable(false)]
		public StartAction StartAction {
			get {
				return GetProperty("StartAction", StartAction.Project);
			}
			set {
				SetProperty("StartAction", value);
			}
		}
		
		[Browsable(false)]
		public string StartArguments {
			get {
				return GetProperty("StartArguments");
			}
			set {
				SetProperty("StartArguments", value);
			}
		}
		
		[Browsable(false)]
		public string StartWorkingDirectory {
			get {
				return GetProperty("StartWorkingDirectory");
			}
			set {
				SetProperty("StartWorkingDirectory", value);
			}
		}
		#endregion
		
		public static void RunMSBuild(string fileName, string target, string configuration, string platform, bool isSingleProject, MSBuildEngineCallback callback, IDictionary<string, string> additionalProperties)
		{
			WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();
			MSBuildEngine engine = new MSBuildEngine();
			if (isSingleProject) {
				string dir = ProjectService.OpenSolution.Directory;
				if (!dir.EndsWith("/") && !dir.EndsWith("\\"))
					dir += Path.DirectorySeparatorChar;
				engine.AdditionalProperties.Add("SolutionDir", dir);
			}
			if (additionalProperties != null) {
				foreach (KeyValuePair<string, string> pair in additionalProperties) {
					engine.AdditionalProperties.Add(pair.Key, pair.Value);
				}
			}
			engine.Configuration = configuration;
			engine.Platform = platform;
			engine.MessageView = TaskService.BuildMessageViewCategory;
			if (target == null) {
				engine.Run(fileName, callback);
			} else {
				engine.Run(fileName, new string[] { target }, callback);
			}
		}
		
		public void RunMSBuild(string target, MSBuildEngineCallback callback, IDictionary<string, string> additionalProperties)
		{
			RunMSBuild(this.FileName, target, this.Configuration, this.Platform, true, callback, additionalProperties);
		}
		
		public override void Build(MSBuildEngineCallback callback, IDictionary<string, string> additionalProperties)
		{
			RunMSBuild("Build", callback, additionalProperties);
		}
		
		public override void Rebuild(MSBuildEngineCallback callback, IDictionary<string, string> additionalProperties)
		{
			RunMSBuild("Rebuild", callback, additionalProperties);
		}
		
		public override void Clean(MSBuildEngineCallback callback, IDictionary<string, string> additionalProperties)
		{
			RunMSBuild("Clean", callback, additionalProperties);
			isDirty = true;
		}
		
		public override void Publish(MSBuildEngineCallback callback, IDictionary<string, string> additionalProperties)
		{
			RunMSBuild("Publish", callback, additionalProperties);
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
