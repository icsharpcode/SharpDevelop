// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Parser;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Class responsible for building a project using MSBuild.
	/// Is called by MSBuildProject.
	/// </summary>
	sealed class MSBuildEngine : IMSBuildEngine
	{
		const string CompileTaskNamesPath = "/SharpDevelop/MSBuildEngine/CompileTaskNames";
		const string AdditionalTargetFilesPath = "/SharpDevelop/MSBuildEngine/AdditionalTargetFiles";
		const string AdditionalLoggersPath = "/SharpDevelop/MSBuildEngine/AdditionalLoggers";
		const string LoggerFiltersPath = "/SharpDevelop/MSBuildEngine/LoggerFilters";
		const string AdditionalPropertiesPath = "/SharpDevelop/MSBuildEngine/AdditionalProperties";
		
		public ISet<string> CompileTaskNames { get; private set; }
		public IList<FileName> AdditionalTargetFiles { get; private set; }
		public IList<IMSBuildAdditionalLogger> AdditionalMSBuildLoggers { get; private set; }
		public IList<IMSBuildLoggerFilter> MSBuildLoggerFilters { get;  private set; }
		
		public MSBuildEngine()
		{
			CompileTaskNames = new SortedSet<string>(
				AddInTree.BuildItems<string>(CompileTaskNamesPath, null, false),
				StringComparer.OrdinalIgnoreCase
			);
			AdditionalTargetFiles = SD.AddInTree.BuildItems<string>(AdditionalTargetFilesPath, null, false).Select(FileName.Create).ToList();
			AdditionalMSBuildLoggers = SD.AddInTree.BuildItems<IMSBuildAdditionalLogger>(AdditionalLoggersPath, null, false).ToList();
			MSBuildLoggerFilters = SD.AddInTree.BuildItems<IMSBuildLoggerFilter>(LoggerFiltersPath, null, false).ToList();
		}
		
		public IEnumerable<KeyValuePair<string, string>> GlobalBuildProperties {
			get {
				yield return new KeyValuePair<string, string>("SharpDevelopBinPath", Path.GetDirectoryName(typeof(MSBuildEngine).Assembly.Location));
				// 'BuildingSolutionFile' tells MSBuild that we took care of building a project's dependencies
				// before trying to build the project itself. This speeds up compilation because it prevents MSBuild from
				// repeatedly looking if a project needs to be rebuilt.
				yield return new KeyValuePair<string, string>("BuildingSolutionFile", "true");
				// BuildingSolutionFile does not work in MSBuild 4.0 anymore, but BuildingInsideVisualStudio
				// can be used to get the same effect.
				yield return new KeyValuePair<string, string>("BuildingInsideVisualStudio", "true");
				
				// Re-load these properties from AddInTree every time because "text" might contain
				// SharpDevelop properties resolved by the StringParser (e.g. ${property:FxCopPath}).
				// (this is also why this is an enumerable implemented with yield return)
				AddInTreeNode node = AddInTree.GetTreeNode(MSBuildEngine.AdditionalPropertiesPath, false);
				if (node != null) {
					foreach (Codon codon in node.Codons) {
						object item = node.BuildChildItem(codon, null);
						if (item != null) {
							string text = item.ToString();
							yield return new KeyValuePair<string, string>(codon.Id, text);
						}
					}
				}
			}
		}
		
		public Task<bool> BuildAsync(IProject project, ProjectBuildOptions options, IBuildFeedbackSink feedbackSink, CancellationToken cancellationToken, IEnumerable<string> additionalTargetFiles)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (options == null)
				throw new ArgumentNullException("options");
			if (feedbackSink == null)
				throw new ArgumentNullException("feedbackSink");
			
			var additionalTargetFileList = additionalTargetFiles != null ? additionalTargetFiles.ToList() : new List<string>();
			additionalTargetFileList.AddRange(AdditionalTargetFiles.Select(fileName => fileName.ToString()));
			if (project.MinimumSolutionVersion >= SolutionFormatVersion.VS2010) {
				additionalTargetFileList.Add(Path.Combine(Path.GetDirectoryName(typeof(MSBuildEngine).Assembly.Location), "SharpDevelop.TargetingPack.targets"));
			}
			var engine = new MSBuildEngineWorker(this, project, options, feedbackSink, additionalTargetFileList);
			return engine.RunBuildAsync(cancellationToken);
		}
		
		public IList<ReferenceProjectItem> ResolveAssemblyReferences(
			MSBuildBasedProject baseProject,
			ReferenceProjectItem[] additionalReferences, bool resolveOnlyAdditionalReferences,
			bool logErrorsToOutputPad)
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
			if (baseProject.MinimumSolutionVersion >= SolutionFormatVersion.VS2010) {
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
			lock (MSBuildInternals.SolutionProjectCollectionLock) {
				BuildParameters parameters = new BuildParameters(baseProject.MSBuildProjectCollection);
				parameters.Loggers = loggers;
				
				//LoggingService.Debug("Started build for ResolveAssemblyReferences");
				BuildResult result = BuildManager.DefaultBuildManager.Build(parameters, requestData);
				if (result == null)
					throw new InvalidOperationException("BuildResult is null");
				//LoggingService.Debug("Build for ResolveAssemblyReferences finished: " + result.OverallResult);
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
				//LoggingService.Debug("Got information about " + assembly.OriginalInclude + "; fullpath=" + assembly.FullPath);
				foreach (var referenceItem in assembly.ReferenceItems) {
					referenceItem.AssemblyName = assembly.AssemblyName;
					referenceItem.FileName = FileName.Create(assembly.FullPath);
					referenceItem.Redist = assembly.Redist;
					referenceItem.DefaultCopyLocalValue = assembly.CopyLocal;
					handledReferenceItems.Add(referenceItem);
				}
				ReferenceProjectItem firstItem = assembly.ReferenceItems.FirstOrDefault();
				if (firstItem != null) {
					resolvedAssemblies.Add(firstItem);
				} else {
					resolvedAssemblies.Add(new ReferenceProjectItem(baseProject, assembly.OriginalInclude) { FileName = FileName.Create(assembly.FullPath) });
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
