// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using MSBuild = Microsoft.Build.BuildEngine;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Messing with MSBuild's internals.
	/// </summary>
	public static class MSBuildInternals
	{
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
		
		internal static void AddItemToGroup(MSBuild.BuildItemGroup group, ProjectItem item)
		{
			if (group == null)
				throw new ArgumentNullException("group");
			if (item == null)
				throw new ArgumentNullException("item");
			if (item.IsAddedToProject)
				throw new ArgumentException("item is already added to project", "item");
			MSBuild.BuildItem newItem = group.AddNewItem(item.ItemType.ToString(), item.Include);
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
			MSBuild.Engine engine = CreateEngine();
			tempProject = engine.CreateNewProject();
			tempProject.LoadXml(baseProject.Xml);
			tempProject.SetProperty("Configuration", configuration);
			tempProject.SetProperty("Platform", platform);
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
			pGroup.Condition = condition;
			pGroup.AddNewProperty(propertyName, "ConditionTrue");
			bool result = project.GetEvaluatedProperty(propertyName) == "ConditionTrue";
			project.RemovePropertyGroup(pGroup);
			return result;
		}
		
		public static MSBuild.BuildProperty GetProperty(MSBuild.BuildPropertyGroup pg, string name)
		{
			return Linq.Find(Linq.CastTo<MSBuild.BuildProperty>(pg),
			                 delegate(MSBuild.BuildProperty p) { return p.Name == name; });
		}
		
		public static MSBuild.Engine CreateEngine()
		{
			return new MSBuild.Engine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory());
		}
		
		/// <summary>
		/// Removes all &lt;Import&gt; nodes from a project.
		/// </summary>
		public static void ClearImports(MSBuild.Project project)
		{
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
			
			XmlAttribute a = (XmlAttribute)typeof(MSBuild.Import).InvokeMember(
				"ProjectPathAttribute",
				BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
				null, import, null
			);
			a.Value = newRawPath;
			EndXmlManipulation(project.MSBuildProject);
			project.CreateItemsListFromMSBuild();
		}
		
		public static IEnumerable<string> GetCustomMetadataNames(MSBuild.BuildItem item)
		{
			ArrayList a = (ArrayList)typeof(MSBuild.BuildItem).InvokeMember(
				"GetAllCustomMetadataNames",
				BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
				null, item, null
			);
			return (string[])a.ToArray(typeof(string));
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
			typeof(MSBuild.Project).InvokeMember(
				"MarkProjectAsDirtyForReprocessXml",
				BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
				null, project, null
			);
		}
	}
}
