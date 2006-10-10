// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Project
{
	public class MSBuildProject : AbstractProject
	{
		List<string> unknownXmlSections     = new List<string>();
		List<string> userUnknownXmlSections = new List<string>();
		List<MSBuildImport> imports         = new List<MSBuildImport>();
		
		protected char BuildConstantSeparator = ';';
		
		/// <summary>
		/// A list of project properties whose change causes reparsing of references and
		/// files.
		/// </summary>
		protected readonly List<string> reparseSensitiveProperties = new List<string>();
		
		/// <summary>
		/// A list of project properties that are saved after the normal properties.
		/// Use this for properties that could reference other properties, e.g.
		/// PostBuildEvent references OutputPath.
		/// </summary>
		protected readonly List<string> lastSavedProperties = new List<string>(new string[]
		                                                                       {"PostBuildEvent",
		                                                                       	"PreBuildEvent"});
		
		/// <summary>
		/// Gets the list of MSBuild Imports.
		/// </summary>
		/// <returns>
		/// List of Import filenames, <example>$(MSBuildBinPath)\Microsoft.VisualBasic.targets</example>
		/// </returns>
		[Browsable(false)]
		public List<MSBuildImport> Imports {
			get {
				return imports;
			}
		}
		
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
			if (information.CreateProjectWithDefaultOutputPath) {
				configurations["Debug|*"]["OutputPath"] = @"bin\Debug\";
			}
			configurations["Debug|*"]["Optimize"] = "False";
			configurations["Debug|*"]["DefineConstants"] = "DEBUG" + BuildConstantSeparator + "TRACE";
			configurations["Debug|*"]["DebugSymbols"] = "True";
			configurations["Debug|*"]["DebugType"] = "Full";
			
			configurations["Release|*"] = new PropertyGroup();
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
			using (XmlTextReader reader = new XmlTextReader(projectFileName)) {
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
								string project = reader.GetAttribute("Project");
								MSBuildImport import = new MSBuildImport(ProjectItem.MSBuildUnescape(project));
								if (reader.GetAttribute("Condition") != null) {
									import.Condition = reader.GetAttribute("Condition");
								}
								Imports.Add(import);
								break;
							default:
								unknownXmlSections.Add(reader.ReadOuterXml());
								break;
						}
					}
				}
			}
			ExpandWildcards();
			
			string userSettingsFileName = projectFileName + ".user";
			if (File.Exists(userSettingsFileName)) {
				using (XmlTextReader reader = new XmlTextReader(userSettingsFileName)) {
					reader.WhitespaceHandling = WhitespaceHandling.Significant;
					reader.Namespaces = false;
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
		
		void ExpandWildcards()
		{
			for (int i = 0; i < Items.Count; i++) {
				ProjectItem item = Items[i];
				if (item.Include.IndexOf('*') >= 0 && item is FileProjectItem) {
					Items.RemoveAt(i--);
					try {
						string path = Path.Combine(this.Directory, Path.GetDirectoryName(item.Include));
						foreach (string file in System.IO.Directory.GetFiles(path, Path.GetFileName(item.Include))) {
							ProjectItem n = item.Clone();
							n.Include = FileUtility.GetRelativePath(this.Directory, file);
							Items.Insert(++i, n);
						}
					} catch (Exception ex) {
						MessageService.ShowError(ex, "Error expanding wildcards in " + item.Include);
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
				string configuration = ProjectItem.MSBuildUnescape(match.Result("${value}"));
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
			using (XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8)) {
				writer.Formatting = Formatting.Indented;
				writer.Namespaces = false;
				
				writer.WriteStartElement("Project");
				// 				writer.WriteAttributeString("MSBuildVersion", "2.0");
				writer.WriteAttributeString("DefaultTargets", "Build");
				writer.WriteAttributeString("xmlns", "http://schemas.microsoft.com/developer/msbuild/2003");
				BaseConfiguration["ProjectGuid"] = IdGuid;
				
				SaveProperties(writer, BaseConfiguration, configurations, 1);
				
				List<ProjectItem> references   = new List<ProjectItem>();
				List<ProjectItem> imports      = new List<ProjectItem>();
				List<ProjectItem> projectFiles = new List<ProjectItem>();
				List<ProjectItem> other        = new List<ProjectItem>();
				
				foreach (ProjectItem item in this.Items) {
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
				
				foreach (MSBuildImport import in Imports) {
					writer.WriteStartElement("Import");
					writer.WriteAttributeString("Project", ProjectItem.MSBuildEscape(import.Project));
					if (import.Condition != null) {
						writer.WriteAttributeString("Condition", import.Condition);
					}
					writer.WriteEndElement();
				}
				
				SaveProperties(writer, BaseConfiguration, configurations, 2);
				
				writer.WriteEndElement();
			}
			
			string userSettingsFileName = fileName + ".user";
			if (userConfigurations.Count > 0 || UserBaseConfiguration.PropertyCount > 0 || File.Exists(userSettingsFileName)) {
				using (XmlTextWriter writer = new XmlTextWriter(userSettingsFileName, Encoding.UTF8)) {
					writer.Formatting = Formatting.Indented;
					writer.Namespaces = false;
					writer.WriteStartElement("Project");
					writer.WriteAttributeString("xmlns", "http://schemas.microsoft.com/developer/msbuild/2003");
					
					SaveProperties(writer, UserBaseConfiguration, userConfigurations, 1);
					SaveUnknownXmlSections(writer, userUnknownXmlSections);
					SaveProperties(writer, UserBaseConfiguration, userConfigurations, 2);
					
					
					writer.WriteEndElement();
				}
			}
		}
		
		void SaveProperties(XmlWriter writer, PropertyGroup baseConfiguration, Dictionary<string, PropertyGroup> configurations, int runNumber)
		{
			Predicate<KeyValuePair<string, string>> filterPredicate;
			if (runNumber == 1) {
				filterPredicate = delegate(KeyValuePair<string, string> property) {
					return !lastSavedProperties.Contains(property.Key);
				};
			} else {
				filterPredicate = delegate(KeyValuePair<string, string> property) {
					return lastSavedProperties.Contains(property.Key);
				};
			}
			
			if (baseConfiguration.PropertyCount > 0) {
				PropertyGroup.WriteProperties(writer,
				                              string.Empty,
				                              Linq.Where(baseConfiguration, filterPredicate),
				                              baseConfiguration.IsGuardedProperty);
			}
			foreach (KeyValuePair<string, PropertyGroup> entry in configurations) {
				// Skip empty groups
				if (entry.Value.PropertyCount == 0) {
					continue;
				}
				string condition;
				if (entry.Key.StartsWith("*|")) {
					condition = " '$(Platform)' == '" + ProjectItem.MSBuildEscape(entry.Key.Substring(2)) + "' ";
				} else if (entry.Key.EndsWith("|*")) {
					condition = " '$(Configuration)' == '" + ProjectItem.MSBuildEscape(entry.Key.Substring(0, entry.Key.Length - 2)) + "' ";
				} else {
					condition = " '$(Configuration)|$(Platform)' == '" + ProjectItem.MSBuildEscape(entry.Key) + "' ";
				}
				PropertyGroup.WriteProperties(writer,
				                              condition,
				                              Linq.Where(entry.Value, filterPredicate),
				                              entry.Value.IsGuardedProperty);
			}
		}
		
		static void SaveUnknownXmlSections(XmlWriter writer, List<string> unknownElements)
		{
			foreach (string element in unknownElements) {
				// round-trip xml text again for better formatting
				XmlTextReader reader = new XmlTextReader(new StringReader(element));
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
		
		protected void Start(string program, bool withDebugging)
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
			if (!System.IO.Directory.Exists(psi.WorkingDirectory)) {
				MessageService.ShowError("Working directory " + psi.WorkingDirectory + " does not exist; the process cannot be started. You can specify the working directory in the project options.");
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
			this.IsDirty = true;
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
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:Dialog.ProjectOptions.Platform}")]
		public override string Platform {
			get {
				return base.Platform;
			}
			set {
				if (base.Platform != value) {
					SetPlatformOrConfiguration(true, value);
				}
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:Dialog.ProjectOptions.Configuration}")]
		public override string Configuration {
			get {
				return base.Configuration;
			}
			set {
				if (base.Configuration != value) {
					SetPlatformOrConfiguration(false, value);
				}
			}
		}
		
		void SetPlatformOrConfiguration(bool platform, string newValue)
		{
			Dictionary<string, string> reparseSensitiveValues = new Dictionary<string, string>();
			foreach (string p in reparseSensitiveProperties) {
				reparseSensitiveValues[p] = GetProperty(p);
			}
			
			if (platform)
				base.Platform = newValue;
			else
				base.Configuration = newValue;
			
			foreach (string p in reparseSensitiveProperties) {
				if (reparseSensitiveValues[p] != GetProperty(p)) {
					ParserService.Reparse(this, true, true);
					break;
				}
			}
		}
		
		public override void SetProperty<T>(string configurationName, string platform, string property, T value, PropertyStorageLocations location)
		{
			if (reparseSensitiveProperties.Contains(property)) {
				string oldValue = GetProperty(property);
				base.SetProperty(configurationName, platform, property, value, location);
				if (oldValue != GetProperty(property)) {
					// change had an effect on current configuration
					ParserService.Reparse(this, true, true);
				}
			} else {
				base.SetProperty(configurationName, platform, property, value, location);
			}
		}
	}
}
