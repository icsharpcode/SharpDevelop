// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Project
{
	public abstract class AbstractProject : AbstractSolutionFolder, IProject
	{
		protected Dictionary<string, PropertyGroup> configurations     = new Dictionary<string, PropertyGroup>();
		protected Dictionary<string, PropertyGroup> userConfigurations = new Dictionary<string, PropertyGroup>();
		
		protected List<ProjectItem> items   = new List<ProjectItem>();
		protected List<string>      imports = new List<string>();
		readonly List<ProjectSection> projectSections = new List<ProjectSection>();
		
		string fileName;
		string language;
		
		[Browsable(false)]
		public Dictionary<string, PropertyGroup> Configurations {
			get {
				return configurations;
			}
		}
		
		[Browsable(false)]
		public Dictionary<string, PropertyGroup> UserConfigurations {
			get {
				return userConfigurations;
			}
		}
		
		public static string GetConfigurationNameFromKey(string key)
		{
			return key.Substring(0, key.IndexOf('|'));
		}
		
		public static string GetPlatformNameFromKey(string key)
		{
			return key.Substring(key.IndexOf('|') + 1);
		}
		
		public List<string> GetConfigurationNames()
		{
			List<string> configurationNames = new List<string>();
			foreach (string key in configurations.Keys) {
				string configuration = GetConfigurationNameFromKey(key);
				if (configuration != "*" && !configurationNames.Contains(configuration)) {
					configurationNames.Add(configuration);
				}
			}
			foreach (string key in userConfigurations.Keys) {
				string configuration = GetConfigurationNameFromKey(key);
				if (configuration != "*" && !configurationNames.Contains(configuration)) {
					configurationNames.Add(configuration);
				}
			}
			if (!configurationNames.Contains(this.Configuration)) {
				configurationNames.Add(this.Configuration);
			}
			return configurationNames;
		}
		
		public List<string> GetPlatformNames()
		{
			List<string> platformNames = new List<string>();
			foreach (string key in configurations.Keys) {
				string platform = GetPlatformNameFromKey(key);
				if (platform != "*" && !platformNames.Contains(platform)) {
					platformNames.Add(platform);
				}
			}
			foreach (string key in userConfigurations.Keys) {
				string platform = GetPlatformNameFromKey(key);
				if (platform != "*" && !platformNames.Contains(platform)) {
					platformNames.Add(platform);
				}
			}
			if (!platformNames.Contains(this.Platform)) {
				platformNames.Add(this.Platform);
			}
			return platformNames;
		}
		
		/// <summary>
		/// Import options from an attribute collection. This is used to read the template options.
		/// </summary>
		public void ImportOptions(XmlAttributeCollection attributes)
		{
			Type t = GetType();
			foreach (XmlAttribute attr in attributes) {
				PropertyInfo prop = t.GetProperty(attr.Name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
				if (prop == null) {
					MessageService.ShowError("Property '" + attr.Name + "' does not exist!");
				} else {
					TypeConverter desc = TypeDescriptor.GetConverter(prop.PropertyType);
					prop.SetValue(this, desc.ConvertFromInvariantString(attr.Value), null);
				}
			}
		}
		
		protected bool isDirty = false;
		
		[Browsable(false)]
		public bool IsDirty {
			get {
				return isDirty;
			}
			set {
				isDirty = value;
			}
		}
		
		PropertyGroup baseConfiguration = new PropertyGroup();
		
		[Browsable(false)]
		public PropertyGroup BaseConfiguration {
			get {
				return baseConfiguration;
			}
		}
		
		PropertyGroup userBaseConfiguration = new PropertyGroup();
		
		[Browsable(false)]
		public PropertyGroup UserBaseConfiguration {
			get {
				return userBaseConfiguration;
			}
		}
		
		[Browsable(false)]
		public List<ProjectItem> Items {
			get {
				return items;
			}
		}
		
		/// <summary>
		/// Gets a list of property sections stored in the solution file.
		/// </summary>
		[Browsable(false)]
		public List<ProjectSection> ProjectSections {
			get {
				return projectSections;
			}
		}
		
		/// <summary>
		/// Gets the list of MSBuild Imports.
		/// </summary>
		/// <returns>
		/// List of Import filenames, <example>$(MSBuildBinPath)\Microsoft.VisualBasic.targets</example>
		/// </returns>
		[Browsable(false)]
		public List<string> Imports {
			get {
				return imports;
			}
		}
		
		[Browsable(false)]
		public string FileName {
			get {
				if (fileName == null) {
					return String.Empty;
				}
				return fileName;
			}
			set {
				fileName = value;
			}
		}
		
		string directoryName;
		
		[Browsable(false)]
		public string Directory {
			get {
				if (directoryName == null) {
					if (fileName == null) {
						return String.Empty;
					}
					directoryName = Path.GetDirectoryName(fileName);
				}
				return directoryName;
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:MainWindow.Windows.Debug.CallStack.Language}")]
		public string Language {
			get {
				return language;
			}
			set {
				language = value;
			}
		}
		
		ICSharpCode.SharpDevelop.Dom.LanguageProperties languageProperties = ICSharpCode.SharpDevelop.Dom.LanguageProperties.CSharp;
		
		[Browsable(false)]
		public ICSharpCode.SharpDevelop.Dom.LanguageProperties LanguageProperties {
			get {
				return languageProperties;
			}
			set {
				languageProperties = value;
			}
		}
		
		[Browsable(false)]
		public virtual IAmbience Ambience {
			get {
				return null;
			}
		}
		
		string activeConfiguration = "Debug";
		string activePlatform = "AnyCPU";
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:Dialog.ProjectOptions.Platform}")]
		public string Platform {
			get {
				return activePlatform;
			}
			set {
				if (value == "Any CPU") {
					// This seems to be a special case in MSBuild
					List<string> platformNames = GetPlatformNames();
					if (!platformNames.Contains("Any CPU") && platformNames.Contains("AnyCPU")) {
						value = "AnyCPU";
					}
				}
				activePlatform = value;
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:Dialog.ProjectOptions.Configuration}")]
		public string Configuration {
			get {
				return activeConfiguration;
			}
			set {
				activeConfiguration = value;
			}
		}
		
		[Browsable(false)]
		public string DefaultConfiguration {
			get {
				// is always stored in BaseConfiguration
				return BaseConfiguration["Configuration"];
			}
			set {
				BaseConfiguration["Configuration"] = value;
			}
		}
		
		[Browsable(false)]
		public string DefaultPlatform {
			get {
				// is always stored in BaseConfiguration
				return BaseConfiguration["Platform"];
			}
			set {
				BaseConfiguration["Platform"] = value;
			}
		}
		
		[Browsable(false)]
		public virtual OutputType OutputType {
			get {
				return GetProperty("OutputType", OutputType.Exe);
			}
			set {
				SetProperty("OutputType", value);
			}
		}
		
		[Browsable(false)]
		public string DocumentationFileName {
			get {
				string file = GetProperty("DocumentationFile");
				if (file == null || file.Length == 0)
					return null;
				return Path.Combine(Directory, file);
			}
		}
		
		[Browsable(false)]
		public string OutputAssemblyFullPath {
			get {
				string outputPath = GetProperty("OutputPath");
				return Path.Combine(Path.Combine(Directory, outputPath), AssemblyName + GetExtension(OutputType));
			}
		}
		
		public static string GetExtension(OutputType outputType)
		{
			switch (outputType) {
				case OutputType.WinExe:
				case OutputType.Exe:
					return ".exe";
				case OutputType.Module:
					return ".netmodule";
				default:
					return ".dll";
			}
		}
		
		/// <summary>
		/// Legacy, only for VS.NET compatiblity reasons.
		/// </summary>
		[Browsable(false)]
		public Guid Guid {
			get {
				// is always in base config
				return new Guid(BaseConfiguration["ProjectGuid"]);
			}
		}
		
		[Browsable(false)]
		public string RootNamespace {
			get {
				return GetProperty("RootNamespace");
			}
			set {
				SetProperty("RootNamespace", value);
			}
		}
		
		[Browsable(false)]
		public string AssemblyName {
			get {
				return GetProperty("AssemblyName");
			}
			set {
				SetProperty("AssemblyName", value);
			}
		}
		
		[Browsable(false)]
		public string AppDesignerFolder {
			get {
				return GetProperty("AppDesignerFolder");
			}
			set {
				SetProperty("AppDesignerFolder", value);
			}
		}
		
		[Browsable(false)]
		public override string TypeGuid {
			get {
				return LanguageBindingService.GetCodonPerLanguageName(Language).Guid;
			}
			set {
				throw new System.NotSupportedException();
			}
		}
		
		
		public virtual bool CanCompile(string fileName)
		{
			return false;
		}
		
		public virtual void Save()
		{
			Save(FileName);
		}

		public virtual void Save(string fileName)
		{
		}
		
		public virtual void Start(bool withDebugging)
		{
		}
		
		[Browsable(false)]
		public virtual bool IsStartable {
			get {
				return false;
			}
		}
		
		public string GetProperty(string property)
		{
			return GetProperty(property, "");
		}
		
		public T GetProperty<T>(string property, T defaultValue)
		{
			PropertyStorageLocations tmp;
			return GetProperty(this.Configuration, this.Platform, property, defaultValue, out tmp);
		}
		
		public T GetProperty<T>(string configurationName, string platform, string property, T defaultValue, out PropertyStorageLocations location)
		{
			location = PropertyStorageLocations.UserFile;
			PropertyGroup pg = FindProperty(configurationName, platform, property,
			                                UserBaseConfiguration, userConfigurations, ref location);
			if (pg != null) {
				return pg.Get(property, defaultValue);
			}
			location = PropertyStorageLocations.Base;
			pg = FindProperty(configurationName, platform, property,
			                  BaseConfiguration, configurations, ref location);
			if (pg != null) {
				return pg.Get(property, defaultValue);
			} else {
				// we need to look for the property in other configurations.
				// For example, "XML doc" might only exist in the release build, so we have
				// to set the correct location even though the property is not found in debug conf.
				location = FindProperty(configurationName, platform, property);
				return defaultValue;
			}
		}
		
		public PropertyStorageLocations FindProperty(string configurationName, string platform, string property)
		{
			PropertyStorageLocations location = PropertyStorageLocations.Unknown;
			FindProperty(configurationName, platform, property,
			             UserBaseConfiguration, userConfigurations, ref location);
			if (location != PropertyStorageLocations.Unknown) {
				return location | PropertyStorageLocations.UserFile;
			}
			FindProperty(configurationName, platform, property,
			             BaseConfiguration, configurations, ref location);
			if (location == PropertyStorageLocations.Unknown) {
				// we need to look for the property in other configurations.
				// For example, "XML doc" might only exist in the release build, so we have
				// to set the correct location even though the property is not found in debug conf.
				foreach (KeyValuePair<string, PropertyGroup> pair in userConfigurations) {
					if (pair.Value.IsSet(property)) {
						return GetLocationFromConfigKey(pair.Key) | PropertyStorageLocations.UserFile;
					}
				}
				foreach (KeyValuePair<string, PropertyGroup> pair in configurations) {
					if (pair.Value.IsSet(property)) {
						return GetLocationFromConfigKey(pair.Key);
					}
				}
			}
			return location;
		}
		
		/// <summary>
		/// Searches a property in a set of configurations (either project file OR user file).
		/// Returns the property group that contains the property, or null, if it is not found.
		/// The value of <paramref name="location"/> is OR'ed with PropertyStorageLocations.Base,
		/// ConfigurationSpecific, PlatformSpecific, or not changed depending on if/where the property
		/// was found.
		/// </summary>
		static PropertyGroup FindProperty(string configurationName, string platform,
		                                  string property,
		                                  PropertyGroup baseConfig, Dictionary<string, PropertyGroup> configurations,
		                                  ref PropertyStorageLocations location)
		{
			PropertyGroup pg;
			if (configurationName != null && platform != null) {
				if (configurations.TryGetValue(configurationName + "|" + platform, out pg)) {
					if (pg.IsSet(property)) {
						location |= PropertyStorageLocations.ConfigurationAndPlatformSpecific;
						return pg;
					}
				}
			}
			if (configurationName != null) {
				if (configurations.TryGetValue(configurationName + "|*", out pg)) {
					if (pg.IsSet(property)) {
						location |= PropertyStorageLocations.ConfigurationSpecific;
						return pg;
					}
				}
			}
			if (platform != null) {
				if (configurations.TryGetValue("*|" + platform, out pg)) {
					if (pg.IsSet(property)) {
						location |= PropertyStorageLocations.PlatformSpecific;
						return pg;
					}
				}
			}
			if (baseConfig.IsSet(property)) {
				location |= PropertyStorageLocations.Base;
				return baseConfig;
			}
			return null;
		}
		
		public void SetProperty<T>(string property, T value)
		{
			SetProperty(this.Configuration, this.Platform, property, value, PropertyStorageLocations.Unchanged);
		}
		
		public void SetProperty<T>(string property, T value, PropertyStorageLocations location)
		{
			SetProperty(this.Configuration, this.Platform, property, value, location);
		}
		
		public virtual void SetProperty<T>(string configurationName, string platform, string property, T value, PropertyStorageLocations location)
		{
			if (location == PropertyStorageLocations.Unchanged) {
				location = FindProperty(configurationName, platform, property);
				if (location == PropertyStorageLocations.Unknown) {
					location = PropertyStorageLocations.Base;
				}
			}
			
			// default value is default(T) except for strings, where it is string.Empty
			T defaultValue = (typeof(T) == typeof(string)) ? (T)(object)string.Empty : default(T);
			
			PropertyGroup baseConfig = this.BaseConfiguration;
			Dictionary<string, PropertyGroup> configs = this.configurations;
			if ((location & PropertyStorageLocations.UserFile) == PropertyStorageLocations.UserFile) {
				baseConfig = this.UserBaseConfiguration;
				configs    = this.userConfigurations;
			}
			PropertyGroup targetGroup;
			switch (location & PropertyStorageLocations.ConfigurationAndPlatformSpecific) {
				case PropertyStorageLocations.ConfigurationAndPlatformSpecific:
					targetGroup = GetOrCreateGroup(configs, configurationName + "|" + platform);
					break;
				case PropertyStorageLocations.ConfigurationSpecific:
					targetGroup = GetOrCreateGroup(configs, configurationName + "|*");
					break;
				case PropertyStorageLocations.PlatformSpecific:
					targetGroup = GetOrCreateGroup(configs, "*|" + platform);
					break;
				default:
					targetGroup = baseConfig;
					break;
			}
			if (!targetGroup.IsSet(property)) {
				string oldBaseValue = null;
				// clear property from other groups
				if (location != PropertyStorageLocations.Base) {
					if (BaseConfiguration.IsSet(property)) {
						oldBaseValue = BaseConfiguration[property];
						BaseConfiguration.Remove(property);
					}
				}
				if (location != (PropertyStorageLocations.Base | PropertyStorageLocations.UserFile)) {
					if (UserBaseConfiguration.IsSet(property)) {
						oldBaseValue = UserBaseConfiguration[property];
						UserBaseConfiguration.Remove(property);
					}
				}
				if (oldBaseValue != null) {
					// copy old base value to all configurations
					IEnumerable<string> configurationNames = GetConfigurationNames();
					IEnumerable<string> platformNames      = GetPlatformNames();
					switch (location & PropertyStorageLocations.ConfigurationAndPlatformSpecific) {
						case PropertyStorageLocations.ConfigurationAndPlatformSpecific:
							foreach (string cN in configurationNames) {
								foreach (string pN in platformNames) {
									GetOrCreateGroup(configs, cN + "|" + pN)[property] = oldBaseValue;
								}
							}
							break;
						case PropertyStorageLocations.ConfigurationSpecific:
							foreach (string cN in configurationNames) {
								GetOrCreateGroup(configs, cN + "|*")[property] = oldBaseValue;
							}
							break;
						case PropertyStorageLocations.PlatformSpecific:
							foreach (string pN in platformNames) {
								GetOrCreateGroup(configs, "*|" + pN)[property] = oldBaseValue;
							}
							break;
					}
				}
				
				RemoveProperty(property, location, configurations, configs);
				RemoveProperty(property, location, userConfigurations, configs);
			}
			targetGroup.Set(property, defaultValue, value);
		}
		
		static PropertyGroup GetOrCreateGroup(Dictionary<string, PropertyGroup> configs, string groupName)
		{
			PropertyGroup pg;
			if (!configs.TryGetValue(groupName, out pg)) {
				pg = new PropertyGroup();
				configs.Add(groupName, pg);
			}
			return pg;
		}
		
		void RemoveProperty(string property, PropertyStorageLocations newLocation, Dictionary<string, PropertyGroup> configs, Dictionary<string, PropertyGroup> newConfigs)
		{
			newLocation &= PropertyStorageLocations.ConfigurationAndPlatformSpecific;
			// we could modify the collection inside the loop, so we need to copy it first
			// to an array to prevent exceptions
			KeyValuePair<string, PropertyGroup>[] cachedPairs = new KeyValuePair<string, PropertyGroup>[configs.Count];
			int i = 0;
			foreach (KeyValuePair<string, PropertyGroup> pair in configs) {
				cachedPairs[i++] = pair;
			}
			foreach (KeyValuePair<string, PropertyGroup> pair in cachedPairs) {
				if (!pair.Value.IsSet(property)) {
					continue;
				}
				PropertyStorageLocations thisLocation = GetLocationFromConfigKey(pair.Key);
				if (thisLocation == (newLocation & ~PropertyStorageLocations.UserFile)) {
					if (configs == newConfigs) {
						// skip if this property group is in "newLocation"
						continue;
					} else {
						// move from project to user (in same location)
						GetOrCreateGroup(newConfigs, pair.Key)[property] = pair.Value[property];
					}
				} else {
					// in some cases, we want to move settings when the location is changed
					switch (newLocation & PropertyStorageLocations.ConfigurationAndPlatformSpecific) {
						case PropertyStorageLocations.ConfigurationAndPlatformSpecific:
							// copy from this to all new (more specific) configurations
							if (thisLocation == PropertyStorageLocations.ConfigurationSpecific) {
								string configName = GetConfigurationNameFromKey(pair.Key);
								foreach (string pN in GetPlatformNames()) {
									GetOrCreateGroup(newConfigs, configName + "|" + pN)[property] = pair.Value[property];
								}
							} else if (thisLocation == PropertyStorageLocations.PlatformSpecific) {
								string platformName = GetPlatformNameFromKey(pair.Key);
								foreach (string cN in GetConfigurationNames()) {
									GetOrCreateGroup(newConfigs, cN + "|" + platformName)[property] = pair.Value[property];
								}
							}
							break;
						case PropertyStorageLocations.ConfigurationSpecific:
							// copy from this configuration|platform to a configuration
							if (thisLocation == PropertyStorageLocations.ConfigurationAndPlatformSpecific) {
								string configName = GetConfigurationNameFromKey(pair.Key);
								PropertyGroup pg = GetOrCreateGroup(newConfigs, configName + "|*");
								if (!pg.IsSet(property))
									pg[property] = pair.Value[property];
							}
							break;
						case PropertyStorageLocations.PlatformSpecific:
							// copy from this configuration|platform to a platform
							if (thisLocation == PropertyStorageLocations.ConfigurationAndPlatformSpecific) {
								string platformName = GetPlatformNameFromKey(pair.Key);
								PropertyGroup pg = GetOrCreateGroup(newConfigs, "*|" + platformName);
								if (!pg.IsSet(property))
									pg[property] = pair.Value[property];
							}
							break;
					}
				}
				pair.Value.Remove(property);
			}
		}
		
		static PropertyStorageLocations GetLocationFromConfigKey(string key)
		{
			if (key.StartsWith("*|")) {
				return PropertyStorageLocations.PlatformSpecific;
			} else if (key.EndsWith("|*")) {
				return PropertyStorageLocations.ConfigurationSpecific;
			} else {
				return PropertyStorageLocations.ConfigurationAndPlatformSpecific;
			}
		}
		
		/// <summary>
		/// Returns true, if a specific file (given by it's name)
		/// is inside this project.
		/// </summary>
		public bool IsFileInProject(string fileName)
		{
			for (int i = 0; i < items.Count; ++i) {
				FileProjectItem fileItem = items[i] as FileProjectItem;
				if (fileItem != null) {
					if (FileUtility.IsEqualFileName(fileItem.FileName, fileName)) {
						return true;
					}
				}
			}
			return false;
		}
		
		/// <summary>
		/// Returns the file content as a string which can be parsed by the parser.
		/// The fileName must be a file name in the project. This is used for files
		/// 'behind' other files or zipped file contents etc.
		/// </summary>
		public virtual string GetParseableFileContent(string fileName)
		{
			return null;
		}
		
		#region System.IDisposable interface implementation
		public virtual void Dispose()
		{
			configurations.Clear();
			
			foreach (ProjectItem item in items) {
				item.Dispose();
			}
			items.Clear();
			
			imports.Clear();
		}
		#endregion
		
		
		public virtual void Build(MSBuildEngineCallback callback)
		{
		}
		
		public virtual void Rebuild(MSBuildEngineCallback callback)
		{
		}
		
		public virtual void Clean(MSBuildEngineCallback callback)
		{
		}
		
		public virtual void Publish(MSBuildEngineCallback callback)
		{
		}
		
		/// <summary>
		/// Creates a new project content for this project.
		/// This method should only be called by ParserService.LoadSolutionProjectsInternal()!
		/// When overriding this method, you should call the base implementation first
		/// to get an uninitialized ParseProjectContent.
		/// </summary>
		public virtual ParseProjectContent CreateProjectContent()
		{
			return ParseProjectContent.CreateUninitalized(this);
		}

		public virtual ProjectItem CreateProjectItem(string itemType)
		{
			return new UnknownProjectItem(this, itemType);
		}
		
		/// <summary>
		/// Saves project preferences (currently opened files, bookmarks etc.) to the
		/// a property container.
		/// </summary>
		public virtual Properties CreateMemento()
		{
			Properties properties = new Properties();
			properties.Set("bookmarks", ICSharpCode.SharpDevelop.Bookmarks.BookmarkManager.GetProjectBookmarks(this).ToArray());
			List<string> files = new List<string>();
			foreach (IViewContent vc in WorkbenchSingleton.Workbench.ViewContentCollection) {
				string fileName = vc.FileName;
				if (fileName != null && IsFileInProject(fileName)) {
					files.Add(fileName);
				}
			}
			properties.Set("files", files.ToArray());
			return properties;
		}
		
		internal static List<string> filesToOpenAfterSolutionLoad = new List<string>();
		
		/// <summary>
		/// Loads project preferences (currently opened files, bookmarks etc.).
		/// </summary>
		public virtual void SetMemento(Properties properties)
		{
			foreach (ICSharpCode.SharpDevelop.Bookmarks.SDBookmark mark in properties.Get("bookmarks", new ICSharpCode.SharpDevelop.Bookmarks.SDBookmark[0])) {
				ICSharpCode.SharpDevelop.Bookmarks.BookmarkManager.AddMark(mark);
			}
			foreach (string fileName in properties.Get("files", new string[0])) {
				filesToOpenAfterSolutionLoad.Add(fileName);
			}
		}
	}
}
