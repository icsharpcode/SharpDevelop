// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using MSBuild = Microsoft.Build.BuildEngine;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Messing with MSBuild's internals.
	/// </summary>
	public static class MSBuildInternals
	{
		/// <summary>
		/// MSBuild does not support multi-threading, so every invocation of MSBuild that
		/// runs inside the SharpDevelop process must lock on this object to prevent conflicts.
		/// </summary>
		public readonly static object InProcessMSBuildLock = new object();
		
		const string MSBuildXmlNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";
		
		#region Escaping
		/// <summary>
		/// Escapes special MSBuild characters ( '%', '*', '?', '@', '$', '(', ')', ';', "'" ).
		/// </summary>
		public static string Escape(string text)
		{
			return MSBuild.Utilities.Escape(text);
		}
		
		/// <summary>
		/// Unescapes escaped MSBuild characters.
		/// </summary>
		public static string Unescape(string text)
		{
			if (text == null)
				throw new ArgumentNullException("text");
			StringBuilder b = null;
			for (int i = 0; i < text.Length; i++) {
				char c = text[i];
				if (c == '%' && i + 2 < text.Length) {
					if (b == null) b = new StringBuilder(text, 0, i, text.Length);
					string a = text[i + 1].ToString() + text[i + 2].ToString();
					int num;
					if (int.TryParse(a, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num)) {
						b.Append((char)num);
						i += 2;
					} else {
						b.Append('%');
					}
				} else {
					if (b != null) {
						b.Append(c);
					}
				}
			}
			if (b != null)
				return b.ToString();
			else
				return text;
		}
		#endregion
		
		/// <summary>
		/// This is a special case in MSBuild we need to take care of.
		/// </summary>
		public static string FixPlatformNameForProject(string platformName)
		{
			if (platformName == "Any CPU") {
				return "AnyCPU";
			} else {
				return platformName;
			}
		}
		
		/// <summary>
		/// This is a special case in MSBuild we need to take care of.
		/// Opposite of FixPlatformNameForProject
		/// </summary>
		public static string FixPlatformNameForSolution(string platformName)
		{
			if (platformName == "AnyCPU") {
				return "Any CPU";
			} else {
				return platformName;
			}
		}
		
		internal static void AddItemToGroup(MSBuild.BuildItemGroup group, ProjectItem item)
		{
			if (group == null)
				throw new ArgumentNullException("group");
			if (item == null)
				throw new ArgumentNullException("item");
			if (item.IsAddedToProject)
				throw new ArgumentException("item is already added to project", "item");
			MSBuild.BuildItem newItem = group.AddNewItem(item.ItemType.ToString(), item.Include, item.TreatIncludeAsLiteral);
			foreach (string name in item.MetadataNames) {
				newItem.SetMetadata(name, item.GetMetadata(name));
			}
			item.BuildItem = newItem;
			Debug.Assert(item.IsAddedToProject);
		}
		
		internal static void EnsureCorrectTempProject(MSBuild.Project baseProject,
		                                              string configuration, string platform,
		                                              ref MSBuild.Project tempProject)
		{
			if (configuration == null && platform == null) {
				// unload temp project
				if (tempProject != null && tempProject != baseProject) {
					tempProject.ParentEngine.UnloadAllProjects();
				}
				tempProject = null;
				return;
			}
			if (configuration == null)
				configuration = baseProject.GetEvaluatedProperty("Configuration");
			if (platform == null)
				platform = baseProject.GetEvaluatedProperty("Platform");
			
			if (tempProject != null
			    && tempProject.GetEvaluatedProperty("Configuration") == configuration
			    && tempProject.GetEvaluatedProperty("Platform") == platform)
			{
				// already correct
				return;
			}
			if (baseProject.GetEvaluatedProperty("Configuration") == configuration
			    && baseProject.GetEvaluatedProperty("Platform") == platform)
			{
				tempProject = baseProject;
				return;
			}
			// create new project
			
			// unload old temp project
			if (tempProject != null && tempProject != baseProject) {
				tempProject.ParentEngine.UnloadAllProjects();
			}
			try {
				MSBuild.Engine engine = CreateEngine();
				tempProject = engine.CreateNewProject();
				// tell MSBuild the path so that projects containing <Import Project="relativePath" />
				// can be loaded
				tempProject.FullFileName = baseProject.FullFileName;
				MSBuildBasedProject.InitializeMSBuildProject(tempProject);
				tempProject.LoadXml(baseProject.Xml);
				tempProject.SetProperty("Configuration", configuration);
				tempProject.SetProperty("Platform", platform);
			} catch (Exception ex) {
				ICSharpCode.Core.MessageService.ShowWarning(ex.ToString());
				tempProject = baseProject;
			}
		}
		
		internal static PropertyStorageLocations GetLocationFromCondition(string condition)
		{
			if (string.IsNullOrEmpty(condition)) {
				return PropertyStorageLocations.Base;
			}
			PropertyStorageLocations location = 0; // 0 is unknown
			if (condition.Contains("$(Configuration)"))
				location |= PropertyStorageLocations.ConfigurationSpecific;
			if (condition.Contains("$(Platform)"))
				location |= PropertyStorageLocations.PlatformSpecific;
			return location;
		}
		
		readonly static Regex configurationRegEx = new Regex(@"'(?<property>[^']*)'\s*==\s*'(?<value>[^']*)'", RegexOptions.Compiled);
		
		internal static void GetConfigurationAndPlatformFromCondition(string condition,
		                                                              out string configuration,
		                                                              out string platform)
		{
			Match match = configurationRegEx.Match(condition);
			if (match.Success) {
				string conditionProperty = match.Result("${property}");
				string conditionValue = match.Result("${value}");
				if (conditionProperty == "$(Configuration)|$(Platform)") {
					// configuration is ok
					configuration = MSBuildBasedProject.GetConfigurationNameFromKey(conditionValue);
					platform = MSBuildBasedProject.GetPlatformNameFromKey(conditionValue);
				} else if (conditionProperty == "$(Configuration)") {
					configuration = conditionValue;
					platform = null;
				} else if (conditionProperty == "$(Platform)") {
					configuration = null;
					platform = conditionValue;
				} else {
					configuration = null;
					platform = null;
				}
			} else {
				configuration = null;
				platform = null;
			}
		}
		
		/// <summary>
		/// Evaluates the specified condition in the project and specified configuration/platform.
		/// WARNING: EvaluateCondition might add a temporary property group and remove it again,
		/// which invalidates enumerators over the list of property groups!
		/// </summary>
		internal static bool EvaluateCondition(MSBuild.Project project,
		                                       string configuration, string platform,
		                                       string condition,
		                                       ref MSBuild.Project tempProject)
		{
			if (string.IsNullOrEmpty(condition)) {
				return true;
			}
			EnsureCorrectTempProject(project, configuration, platform, ref tempProject);
			return EvaluateCondition(tempProject, condition);
		}
		
		/// <summary>
		/// Evaluates the specified condition in the project.
		/// WARNING: EvaluateCondition might add a temporary property group and remove it again,
		/// which invalidates enumerators over the list of property groups!
		/// </summary>
		internal static bool EvaluateCondition(MSBuild.Project project,
		                                       string condition)
		{
			const string propertyName = "MSBuildInternalsEvaluateConditionDummyPropertyName";
			MSBuild.BuildPropertyGroup pGroup = project.AddNewPropertyGroup(true);
			pGroup.AddNewProperty(propertyName, "ConditionFalse");
			pGroup.AddNewProperty(propertyName, "ConditionTrue").Condition = condition;
			bool result = project.GetEvaluatedProperty(propertyName) == "ConditionTrue";
			project.RemovePropertyGroup(pGroup);
			return result;
		}
		
		public static MSBuild.BuildProperty GetProperty(MSBuild.BuildPropertyGroup pg, string name)
		{
			return pg.Cast<MSBuild.BuildProperty>().FirstOrDefault(p => p.Name == name);
		}
		
		public static MSBuild.Engine CreateEngine()
		{
			return new MSBuild.Engine(MSBuild.ToolsetDefinitionLocations.Registry
			                          | MSBuild.ToolsetDefinitionLocations.ConfigurationFile);
		}
		
		/// <summary>
		/// Removes all &lt;Import&gt; nodes from a project.
		/// </summary>
		public static void ClearImports(MSBuild.Project project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			
			XmlElement xmlProject = BeginXmlManipulation(project);
			List<XmlNode> nodesToRemove = new List<XmlNode>();
			foreach (XmlNode node in xmlProject.ChildNodes) {
				if (node.NodeType == XmlNodeType.Element && node.Name == "Import") {
					nodesToRemove.Add(node);
				}
			}
			foreach (XmlNode node in nodesToRemove) {
				xmlProject.RemoveChild(node);
			}
			EndXmlManipulation(project);
		}
		
		/// <summary>
		/// Changes the value of the ProjectPath property on an existing import.
		/// Note: this methods causes the project to recreate all imports, so existing import
		/// instances might not be affected.
		/// </summary>
		public static void SetImportProjectPath(MSBuildBasedProject project, MSBuild.Import import,
		                                        string newRawPath)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (import == null)
				throw new ArgumentNullException("import");
			if (newRawPath == null)
				throw new ArgumentNullException("newRawPath");
			
			lock (project.SyncRoot) {
				XmlAttribute a = (XmlAttribute)typeof(MSBuild.Import).InvokeMember(
					"ProjectPathAttribute",
					BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
					null, import, null
				);
				a.Value = newRawPath;
				EndXmlManipulation(project.MSBuildProject);
			}
			project.CreateItemsListFromMSBuild();
		}
		
		/// <summary>
		/// Gets all custom metadata names defined directly on the item, ignoring defaulted metadata entries.
		/// </summary>
		public static IList<string> GetCustomMetadataNames(MSBuild.BuildItem item)
		{
			PropertyInfo prop = typeof(MSBuild.BuildItem).GetProperty("ItemDefinitionLibrary", BindingFlags.Instance | BindingFlags.NonPublic);
			object oldValue = prop.GetValue(item, null);
			prop.SetValue(item, null, null);
			IList<string> result = (IList<string>)item.CustomMetadataNames;
			prop.SetValue(item, oldValue, null);
			return result;
		}
		
		static XmlElement CreateElement(XmlDocument document, string name)
		{
			return document.CreateElement(name, MSBuildXmlNamespace);
		}
		
		static XmlElement BeginXmlManipulation(MSBuild.Project project)
		{
			return (XmlElement)typeof(MSBuild.Project).InvokeMember(
				"ProjectElement",
				BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
				null, project, null
			);
		}
		
		static void EndXmlManipulation(MSBuild.Project project)
		{
			MarkProjectAsDirtyForReprocessXml(project);
		}
		
		internal static void MarkProjectAsDirtyForReprocessXml(MSBuild.Project project)
		{
			typeof(MSBuild.Project).InvokeMember(
				"MarkProjectAsDirtyForReprocessXml",
				BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
				null, project, null
			);
		}
		
		internal static void ResolveAssemblyReferences(MSBuildBasedProject baseProject, ReferenceProjectItem[] referenceReplacements)
		{
			MSBuild.Engine tempEngine;
			MSBuild.Project tempProject;
			IEnumerable<ReferenceProjectItem> references;
			
			lock (baseProject.SyncRoot) {
				// create a copy of the project
				tempEngine = CreateEngine();
				tempProject = tempEngine.CreateNewProject();
				// tell MSBuild the path so that projects containing <Import Project="relativePath" />
				// can be loaded
				tempProject.FullFileName = baseProject.MSBuildProject.FullFileName;
				MSBuildBasedProject.InitializeMSBuildProject(tempProject);
				tempProject.LoadXml(baseProject.MSBuildProject.Xml);
				tempProject.SetProperty("Configuration", baseProject.ActiveConfiguration);
				tempProject.SetProperty("Platform", baseProject.ActivePlatform);
				tempProject.SetProperty("BuildingProject", "false");
				
				if (referenceReplacements == null) {
					references = baseProject.GetItemsOfType(ItemType.Reference).OfType<ReferenceProjectItem>();
					
					// remove the "Private" meta data
					foreach (MSBuild.BuildItemGroup itemGroup in tempProject.ItemGroups) {
						// skip item groups from imported projects
						if (itemGroup.IsImported)
							continue;
						foreach (MSBuild.BuildItem item in itemGroup) {
							if (item.Name == ItemType.Reference.ItemName) {
								item.RemoveMetadata("Private");
							}
						}
					}
				} else {
					references = referenceReplacements;
					
					// replace all references in the project with the referenceReplacements
					foreach (MSBuild.BuildItemGroup itemGroup in tempProject.ItemGroups) {
						// skip item groups from imported projects
						if (itemGroup.IsImported)
							continue;
						foreach (MSBuild.BuildItem item in itemGroup.ToArray()) {
							if (item.Name == ItemType.Reference.ItemName) {
								itemGroup.RemoveItem(item);
							}
						}
					}
					foreach (ReferenceProjectItem item in referenceReplacements) {
						tempProject.AddNewItem("Reference", item.Include, true);
					}
				}
			}
			var referenceDict = new Dictionary<string, ReferenceProjectItem>();
			foreach (ReferenceProjectItem item in references) {
				// references could be duplicate, so we cannot use referenceDict.Add or reference.ToDictionary
				referenceDict[item.Include] = item;
			}
			
			
			#if DEBUG
			//engine.RegisterLogger(new MSBuild.ConsoleLogger(Microsoft.Build.Framework.LoggerVerbosity.Detailed));
			#endif
			
			//Environment.CurrentDirectory = Path.GetDirectoryName(tempProject.FullFileName);
			lock (MSBuildInternals.InProcessMSBuildLock) {
				if (!tempProject.Build("ResolveAssemblyReferences")) {
					LoggingService.Warn("ResolveAssemblyReferences exited with error");
					return;
				}
			}
			
			foreach (MSBuild.BuildItem item in tempProject.GetEvaluatedItemsByName("_ResolveAssemblyReferenceResolvedFiles")) {
				string originalInclude = item.GetEvaluatedMetadata("OriginalItemSpec");
				ReferenceProjectItem reference;
				if (referenceDict.TryGetValue(originalInclude, out reference)) {
					reference.AssemblyName = new Dom.DomAssemblyName(item.GetEvaluatedMetadata("FusionName"));
					//string fullPath = item.GetEvaluatedMetadata("FullPath"); is incorrect for relative paths
					string fullPath = FileUtility.GetAbsolutePath(baseProject.Directory, item.GetEvaluatedMetadata("Identity"));
					reference.FileName = fullPath;
					reference.Redist = item.GetEvaluatedMetadata("Redist");
					//LoggingService.Debug("Got information about " + originalInclude + "; fullpath=" + fullPath);
					reference.DefaultCopyLocalValue = bool.Parse(item.GetEvaluatedMetadata("CopyLocal"));
				} else {
					LoggingService.Warn("Unknown item " + originalInclude);
				}
			}
			
			tempEngine.UnloadAllProjects(); // unload temp project
		}
	}
}
