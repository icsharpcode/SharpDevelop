// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
		
		protected List<ProjectItem>                 items          = new List<ProjectItem>();
		protected List<string>                      imports        = new List<string>();
		
		protected string fileName;
		protected string language;
		
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
		
		[Browsable(false)]
		public PropertyGroup BaseConfiguration {
			get {
				if (!configurations.ContainsKey("")) {
					configurations[""] = new PropertyGroup();
				}
				return configurations[""];
			}
		}
		
		[Browsable(false)]
		public PropertyGroup ActiveConfiguration {
			get {
				if (Platform != null && Platform.Length > 0) {
					return configurations[Configuration + "|" + Platform];
				}
				return configurations[Configuration];
			}
		}
		
		[Browsable(false)]
		public List<ProjectItem> Items {
			get {
				return items;
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
				return Path.GetFullPath(fileName);
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
					directoryName = Path.GetFullPath(Path.GetDirectoryName(fileName));
				}
				return directoryName;
			}
		}
		
		[Browsable(false)]
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
		
		[Browsable(false)]
		public string Configuration {
			get {
				// is always stored in BaseConfiguration
				return BaseConfiguration["Configuration"];
			}
			set {
				BaseConfiguration["Configuration"] = value;
			}
		}
		
		[Browsable(false)]
		public string Platform {
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
		
		public virtual bool IsStartable {
			get {
				return false;
			}
		}
		
		/*
		public string GetOutputPath(string configurationName, string platform)
		{
			return GetConfiguration(configurationName, platform)["OutputPath"];
		}
		
		public void SetOutputPath(string configurationName, string platform, string val)
		{
			GetConfiguration(configurationName, platform)["OutputPath"] = val;
		}
		 */
		
		public string GetProperty(string property)
		{
			return GetProperty(property, "");
		}
		
		public T GetProperty<T>(string property, T defaultValue)
		{
			return GetProperty(this.Configuration, this.Platform, property, defaultValue);
		}
		
		public T GetProperty<T>(string configurationName, string platform, string property, T defaultValue)
		{
			string configurationKey = platform != null ? configurationName + "|" + platform : configurationName;
			PropertyGroup pg;
			if (userConfigurations.TryGetValue(configurationKey, out pg)) {
				if (pg.IsSet(property)) {
					return pg.Get(property, defaultValue);
				}
			}
			if (configurations.TryGetValue(configurationKey, out pg)) {
				if (pg.IsSet(property)) {
					return pg.Get(property, defaultValue);
				}
			}
			if (BaseConfiguration.IsSet(property)) {
				return BaseConfiguration.Get(property, defaultValue);
			}
			return defaultValue;
		}
		
		public void SetProperty<T>(string property, T value)
		{
			SetProperty(this.Configuration, this.Platform, property, value, PropertyStorageLocation.Unchanged);
		}
		
		public void SetProperty<T>(string property, T value, PropertyStorageLocation location)
		{
			SetProperty(this.Configuration, this.Platform, property, value, location);
		}
		
		public void SetProperty<T>(string configurationName, string platform, string property, T value, PropertyStorageLocation location)
		{
			string configurationKey = platform != null ? configurationName + "|" + platform : configurationName;
			PropertyGroup pg;
			T defaultValue = (typeof(T) == typeof(string)) ? (T)(object)string.Empty : default(T);
			switch (location) {
				case PropertyStorageLocation.Unchanged:
					if (userConfigurations.TryGetValue(configurationKey, out pg)) {
						if (pg.IsSet(property)) {
							pg.Set(property, defaultValue, value);
							return;
						}
					}
					if (configurations.TryGetValue(configurationKey, out pg)) {
						if (pg.IsSet(property)) {
							pg.Set(property, defaultValue, value);
							return;
						}
					}
					BaseConfiguration.Set(property, defaultValue, value);
					return;
				case PropertyStorageLocation.BaseConfiguration:
					if (!BaseConfiguration.IsSet(property)) {
						RemoveProperty(configurations, property);
						RemoveProperty(userConfigurations, property);
					}
					BaseConfiguration.Set(property, defaultValue, value);
					return;
				case PropertyStorageLocation.SpecificConfiguration:
					if (BaseConfiguration.IsSet(property)) {
						BaseConfiguration.Remove(property);
					}
					RemoveProperty(userConfigurations, property);
					if (!configurations.TryGetValue(configurationKey, out pg)) {
						configurations[configurationKey] = pg = new PropertyGroup();
					}
					pg.Set(property, defaultValue, value);
					return;
				case PropertyStorageLocation.UserSpecificConfiguration:
					if (BaseConfiguration.IsSet(property)) {
						BaseConfiguration.Remove(property);
					}
					RemoveProperty(configurations, property);
					if (!userConfigurations.TryGetValue(configurationKey, out pg)) {
						userConfigurations[configurationKey] = pg = new PropertyGroup();
					}
					pg.Set(property, defaultValue, value);
					return;
				default:
					throw new InvalidEnumArgumentException("location", (int)location, typeof(PropertyStorageLocation));
			}
		}
		
		static void RemoveProperty(Dictionary<string, PropertyGroup> dict, string property)
		{
			foreach (PropertyGroup pg in dict.Values) {
				if (pg.IsSet(property)) {
					pg.Remove(property);
				}
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
		
		
		public virtual CompilerResults Build()
		{
			return null;
		}
		
		public virtual CompilerResults Rebuild()
		{
			return null;
		}
		
		public virtual CompilerResults Clean()
		{
			return null;
		}
		
		public virtual CompilerResults Publish()
		{
			return null;
		}
		
		/// <summary>
		/// Creates a new project content for this project.
		/// This method should only be called by ParserService.LoadSolutionProjectsInternal()!
		/// </summary>
		public virtual ParseProjectContent CreateProjectContent()
		{
			return ParseProjectContent.CreateUninitalized(this);
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
		
		/// <summary>
		/// Loads project preferences (currently opened files, bookmarks etc.).
		/// </summary>
		public virtual void SetMemento(Properties properties)
		{
			foreach (ICSharpCode.SharpDevelop.Bookmarks.SDBookmark mark in properties.Get("bookmarks", new ICSharpCode.SharpDevelop.Bookmarks.SDBookmark[0])) {
				ICSharpCode.SharpDevelop.Bookmarks.BookmarkManager.AddMark(mark);
			}
			foreach (string fileName in properties.Get("files", new string[0])) {
				if (File.Exists(fileName)) {
					FileService.OpenFile(fileName);
				}
			}
		}
	}
}
