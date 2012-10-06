// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Util;
using Microsoft.Build.Construction;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
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
		/// SharpDevelop uses one project collection per solution.
		/// Code accessing one of those collection (even if indirectly through MSBuild) should lock on
		/// MSBuildInternals.SolutionProjectCollectionLock.
		/// </summary>
		public readonly static object SolutionProjectCollectionLock = new object();
		
		// TODO: I think MSBuild actually uses OrdinalIgnoreCase. SharpDevelop 3.x just used string.operator ==, so I'm keeping
		// that setting until all code is ported to use PropertyNameComparer and we've verified what MSBuild is actually using.
		public readonly static StringComparer PropertyNameComparer = StringComparer.Ordinal;
		
		internal static void UnloadProject(MSBuild.Evaluation.ProjectCollection projectCollection, MSBuild.Evaluation.Project project)
		{
			lock (SolutionProjectCollectionLock) {
				projectCollection.UnloadProject(project);
			}
		}
		
		internal static MSBuild.Evaluation.Project LoadProject(MSBuild.Evaluation.ProjectCollection projectCollection, ProjectRootElement rootElement, IDictionary<string, string> globalProps)
		{
			lock (SolutionProjectCollectionLock) {
				string toolsVersion = rootElement.ToolsVersion;
				if (string.IsNullOrEmpty(toolsVersion))
					toolsVersion = projectCollection.DefaultToolsVersion;
				return new MSBuild.Evaluation.Project(rootElement, globalProps, toolsVersion, projectCollection);
			}
		}
		
		internal static ProjectInstance LoadProjectInstance(MSBuild.Evaluation.ProjectCollection projectCollection, ProjectRootElement rootElement, IDictionary<string, string> globalProps)
		{
			lock (SolutionProjectCollectionLock) {
				string toolsVersion = rootElement.ToolsVersion;
				if (string.IsNullOrEmpty(toolsVersion))
					toolsVersion = projectCollection.DefaultToolsVersion;
				return new ProjectInstance(rootElement, globalProps, toolsVersion, projectCollection);
			}
		}
		
		public const string MSBuildXmlNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";
		
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
		
		/// <summary>
		/// Resolves the location of the reference files.
		/// </summary>
		/// <param name="baseProject">The base project.</param>
		/// <param name="referenceReplacements">A different set of references to use instead of those in the project.
		/// Used by the GacReferencePanel.</param>
		public static IList<ReferenceProjectItem> ResolveAssemblyReferences(
			MSBuildBasedProject baseProject,
			ReferenceProjectItem[] additionalReferences = null, bool resolveOnlyAdditionalReferences = false,
			bool logErrorsToOutputPad = true)
		{
			ProjectInstance project = baseProject.CreateProjectInstance();
			project.SetProperty("BuildingProject", "false");
			project.SetProperty("DesignTimeBuild", "true");
			
			List<ProjectItemInstance> references = (
				from item in project.Items
				where ItemType.ReferenceItemTypes.Contains(new ItemType(item.ItemType))
				select item
			).ToList();
			
			List<ReferenceProjectItem> referenceProjectItems;
			
			if (resolveOnlyAdditionalReferences) {
				// Remove existing references from project
				foreach (ProjectItemInstance reference in references) {
					project.RemoveItem(reference);
				}
				references.Clear();
				referenceProjectItems = new List<ReferenceProjectItem>();
			} else {
				// Remove the "Private" meta data.
				// This is necessary to detect the default value for "Private"
				foreach (ProjectItemInstance reference in references) {
					reference.RemoveMetadata("Private");
				}
				referenceProjectItems = baseProject.Items.OfType<ReferenceProjectItem>().ToList();
			}
			
			if (additionalReferences != null) {
				referenceProjectItems.AddRange(additionalReferences);
				foreach (ReferenceProjectItem item in additionalReferences) {
					references.Add(project.AddItem("Reference", item.Include));
				}
			}
			
			List<string> targets = new List<string>();
			if (baseProject.HasProjectType(ProjectTypeGuids.PortableLibrary)) {
				targets.Add("ResolveReferences");
				targets.Add("DesignTimeResolveAssemblyReferences");
			} else {
				targets.Add("ResolveAssemblyReferences");
			}
			BuildRequestData requestData = new BuildRequestData(project, targets.ToArray(), new HostServices());
			List<ILogger> loggers = new List<ILogger>();
			//loggers.Add(new ConsoleLogger(LoggerVerbosity.Diagnostic));
			if (logErrorsToOutputPad)
				loggers.Add(new SimpleErrorLogger());
			lock (SolutionProjectCollectionLock) {
				BuildParameters parameters = new BuildParameters(baseProject.MSBuildProjectCollection);
				parameters.Loggers = loggers;
				
				LoggingService.Debug("Started build for ResolveAssemblyReferences");
				BuildResult result = BuildManager.DefaultBuildManager.Build(parameters, requestData);
				if (result == null)
					throw new InvalidOperationException("BuildResult is null");
				LoggingService.Debug("Build for ResolveAssemblyReferences finished: " + result.OverallResult);
			}
			
			IEnumerable<ProjectItemInstance> resolvedAssemblyProjectItems = project.GetItems("_ResolveAssemblyReferenceResolvedFiles");
			
			var query =
				from msbuildItem in resolvedAssemblyProjectItems
				where msbuildItem.GetMetadataValue("ReferenceSourceTarget") != "ProjectReference"
				let originalInclude = msbuildItem.GetMetadataValue("OriginalItemSpec")
				join item in referenceProjectItems.Where(p => p.ItemType != ItemType.ProjectReference) on originalInclude equals item.Include into referenceItems
				select new {
				OriginalInclude = originalInclude,
				AssemblyName = new DomAssemblyName(msbuildItem.GetMetadataValue("FusionName")),
				FullPath = FileUtility.GetAbsolutePath(baseProject.Directory, msbuildItem.GetMetadataValue("Identity")),
				Redist = msbuildItem.GetMetadataValue("Redist"),
				CopyLocal = bool.Parse(msbuildItem.GetMetadataValue("CopyLocal")),
				ReferenceItems = referenceItems
			};
			// HACK: mscorlib is reported twice for portable library projects (even if we don't specify it as additionalReference)
			query = query.DistinctBy(asm => asm.FullPath);
			List<ReferenceProjectItem> resolvedAssemblies = new List<ReferenceProjectItem>();
			List<ReferenceProjectItem> handledReferenceItems = new List<ReferenceProjectItem>();
			foreach (var assembly in query) {
				LoggingService.Debug("Got information about " + assembly.OriginalInclude + "; fullpath=" + assembly.FullPath);
				foreach (var referenceItem in assembly.ReferenceItems) {
					referenceItem.AssemblyName = assembly.AssemblyName;
					referenceItem.FileName = assembly.FullPath;
					referenceItem.Redist = assembly.Redist;
					referenceItem.DefaultCopyLocalValue = assembly.CopyLocal;
					handledReferenceItems.Add(referenceItem);
				}
				ReferenceProjectItem firstItem = assembly.ReferenceItems.FirstOrDefault();
				if (firstItem != null) {
					resolvedAssemblies.Add(firstItem);
				} else {
					resolvedAssemblies.Add(new ReferenceProjectItem(baseProject, assembly.OriginalInclude) { FileName = assembly.FullPath });
				}
			}
			// Add any assemblies that weren't resolved yet. This is important - for example, this adds back project references.
			foreach (var referenceItem in referenceProjectItems.Except(handledReferenceItems)) {
				resolvedAssemblies.Add(referenceItem);
			}
			return resolvedAssemblies;
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
