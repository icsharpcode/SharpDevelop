// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using ICSharpCode.Core;
using Microsoft.Build.Construction;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using MSBuild = Microsoft.Build;
using ProjectCollection = Microsoft.Build.Evaluation.ProjectCollection;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Messing with MSBuild's internals.
	/// </summary>
	public static class MSBuildInternals
	{
		/// <summary>
		/// Note: due to MSBuild limitations, all projects being built must be from the same ProjectCollection.
		/// SharpDevelop simply uses the predefined ProjectCollection.GlobalProjectCollection.
		/// Code accessing that collection (even if indirectly through MSBuild) should lock on
		/// MSBuildInternals.GlobalProjectCollectionLock.
		/// </summary>
		public readonly static object GlobalProjectCollectionLock = new object();
		
		internal static void UnloadProject(MSBuild.Evaluation.Project project)
		{
			lock (GlobalProjectCollectionLock) {
				ProjectCollection.GlobalProjectCollection.UnloadProject(project);
			}
		}
		
		internal static MSBuild.Evaluation.Project LoadProject(ProjectRootElement rootElement, IDictionary<string, string> globalProps)
		{
			lock (GlobalProjectCollectionLock) {
				string toolsVersion = rootElement.ToolsVersion;
				if (string.IsNullOrEmpty(toolsVersion))
					toolsVersion = ProjectCollection.GlobalProjectCollection.DefaultToolsVersion;
				return new MSBuild.Evaluation.Project(rootElement, globalProps, toolsVersion, ProjectCollection.GlobalProjectCollection);
			}
		}
		
		internal static ProjectInstance LoadProjectInstance(ProjectRootElement rootElement, IDictionary<string, string> globalProps)
		{
			lock (GlobalProjectCollectionLock) {
				string toolsVersion = rootElement.ToolsVersion;
				if (string.IsNullOrEmpty(toolsVersion))
					toolsVersion = ProjectCollection.GlobalProjectCollection.DefaultToolsVersion;
				return new ProjectInstance(rootElement, globalProps, toolsVersion, ProjectCollection.GlobalProjectCollection);
			}
		}
		
		const string MSBuildXmlNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";
		
		#region Escaping
		/// <summary>
		/// Escapes special MSBuild characters ( '%', '*', '?', '@', '$', '(', ')', ';', "'" ).
		/// </summary>
		public static string Escape(string text)
		{
			return MSBuild.Evaluation.ProjectCollection.Escape(text);
		}
		
		/// <summary>
		/// Unescapes escaped MSBuild characters.
		/// </summary>
		public static string Unescape(string text)
		{
			return MSBuild.Evaluation.ProjectCollection.Unescape(text);
		}
		#endregion
		
		internal static PropertyStorageLocations GetLocationFromCondition(MSBuild.Construction.ProjectElement element)
		{
			while (element != null) {
				if (!string.IsNullOrEmpty(element.Condition))
					return GetLocationFromCondition(element.Condition);
				element = element.Parent;
			}
			return PropertyStorageLocations.Base;
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
		
		internal static void ResolveAssemblyReferences(MSBuildBasedProject baseProject, ReferenceProjectItem[] referenceReplacements)
		{
			ProjectInstance project = baseProject.CreateProjectInstance();
			project.SetProperty("BuildingProject", "false");
			
			List<ProjectItemInstance> references = (
				from item in project.Items
				where ItemType.ReferenceItemTypes.Contains(new ItemType(item.ItemType))
				select item
			).ToList();
			
			ReferenceProjectItem[] referenceProjectItems;
			
			if (referenceReplacements == null) {
				// Remove the "Private" meta data.
				// This is necessary to detect the default value for "Private"
				foreach (ProjectItemInstance reference in references) {
					reference.RemoveMetadata("Private");
				}
				
				referenceProjectItems = baseProject.Items.OfType<ReferenceProjectItem>().ToArray();
			} else {
				foreach (ProjectItemInstance reference in references) {
					project.RemoveItem(reference);
				}
				foreach (ReferenceProjectItem item in referenceReplacements) {
					project.AddItem("Reference", item.Include);
				}
				referenceProjectItems = referenceReplacements;
			}
			
			string[] targets = { "ResolveAssemblyReferences" };
			BuildRequestData requestData = new BuildRequestData(project, targets, new HostServices());
			ILogger[] loggers = { new SimpleErrorLogger() };
			BuildSubmission submission = ParallelMSBuildManager.StartBuild(requestData, loggers, null);
			LoggingService.Debug("Started build for ResolveAssemblyReferences");
			submission.WaitHandle.WaitOne();
			BuildResult result = submission.BuildResult;
			if (result == null)
				throw new InvalidOperationException("BuildResult is null");
			LoggingService.Debug("Build for ResolveAssemblyReferences finished: " + result.OverallResult);
			
			
			var referenceDict = new Dictionary<string, ReferenceProjectItem>();
			foreach (ReferenceProjectItem item in referenceProjectItems) {
				// references could be duplicate, so we cannot use referenceDict.Add or reference.ToDictionary
				referenceDict[item.Include] = item;
			}
			
			
			foreach (ProjectItemInstance item in project.GetItems("_ResolveAssemblyReferenceResolvedFiles")) {
				string originalInclude = item.GetMetadataValue("OriginalItemSpec");
				ReferenceProjectItem reference;
				if (referenceDict.TryGetValue(originalInclude, out reference)) {
					reference.AssemblyName = new Dom.DomAssemblyName(item.GetMetadataValue("FusionName"));
					//string fullPath = item.GetEvaluatedMetadata("FullPath"); is incorrect for relative paths
					string fullPath = FileUtility.GetAbsolutePath(baseProject.Directory, item.GetMetadataValue("Identity"));
					reference.FileName = fullPath;
					reference.Redist = item.GetMetadataValue("Redist");
					LoggingService.Debug("Got information about " + originalInclude + "; fullpath=" + fullPath);
					reference.DefaultCopyLocalValue = bool.Parse(item.GetMetadataValue("CopyLocal"));
				} else {
					LoggingService.Warn("Unknown item " + originalInclude);
				}
			}
		}
		
		sealed class SimpleErrorLogger : ILogger
		{
			#region ILogger interface implementation
			public LoggerVerbosity Verbosity { get; set; }
			public string Parameters { get; set; }
			
			public void Initialize(IEventSource eventSource)
			{
				eventSource.ErrorRaised += OnError;
				eventSource.WarningRaised += OnWarning;
			}
			
			public void Shutdown()
			{
			}
			#endregion
			
			void OnError(object sender, BuildErrorEventArgs e)
			{
				TaskService.BuildMessageViewCategory.AppendLine(e.Message);
			}
			
			void OnWarning(object sender, BuildWarningEventArgs e)
			{
				TaskService.BuildMessageViewCategory.AppendLine(e.Message);
			}
		}
	}
}
