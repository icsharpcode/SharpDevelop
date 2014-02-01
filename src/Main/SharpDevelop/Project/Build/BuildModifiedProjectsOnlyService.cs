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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Tracks changes to projects and causes only modified projects
	/// to be recompiled.
	/// </summary>
	class BuildModifiedProjectsOnlyService
	{
		readonly Dictionary<IProject, CompilationPass> unmodifiedProjects = new Dictionary<IProject, CompilationPass>();
		
		public BuildModifiedProjectsOnlyService(IBuildService buildService, IProjectService projectService)
		{
			projectService.CurrentSolutionChanged += OnSolutionChanged;
			buildService.BuildFinished += BuildService_BuildFinished;
			
			FileUtility.FileSaved += OnFileSaved;
		}
		
		void OnSolutionChanged(object sender, PropertyChangedEventArgs<ISolution> e)
		{
			lock (unmodifiedProjects) {
				unmodifiedProjects.Clear();
			}
			if (e.OldValue != null) {
				e.OldValue.ActiveConfigurationChanged -= MarkAllForRecompilation;
				e.OldValue.IsDirtyChanged -= MarkAllForRecompilation;
			}
			if (e.NewValue != null) {
				e.NewValue.ActiveConfigurationChanged += MarkAllForRecompilation;
				e.NewValue.IsDirtyChanged += MarkAllForRecompilation;
			}
		}
		
		void MarkAllForRecompilation(object sender, EventArgs e)
		{
			lock (unmodifiedProjects) {
				unmodifiedProjects.Clear();
			}
		}
		
		void BuildService_BuildFinished(object sender, BuildEventArgs e)
		{
			// at the end of an successful build, mark all built projects as unmodified
			if (e.Results.Result == BuildResultCode.Success) {
				lock (unmodifiedProjects) {
					CompilationPass pass = new CompilationPass();
					foreach (IProject p in e.Projects) {
						unmodifiedProjects[p] = pass;
					}
				}
			}
			// at the end of a cleaning build, mark all projects as requiring a rebuild
			if (e.Options.ProjectTarget == BuildTarget.Clean || e.Options.TargetForDependencies == BuildTarget.Clean) {
				lock (unmodifiedProjects) {
					unmodifiedProjects.Clear();
				}
			}
		}
		
		void OnFileSaved(object sender, FileNameEventArgs e)
		{
			if (ProjectService.OpenSolution != null) {
				foreach (IProject p in ProjectService.OpenSolution.Projects) {
					if (p.FindFile(e.FileName) != null || FileUtility.IsEqualFileName(p.FileName, e.FileName)) {
						lock (unmodifiedProjects) {
							unmodifiedProjects.Remove(p);
						}
					}
				}
			}
		}
		
		public IBuildable WrapBuildable(IBuildable buildable, BuildDetection setting)
		{
			switch (setting) {
				case BuildDetection.DoNotBuild:
					return new DummyBuildable(buildable);
				case BuildDetection.BuildModifiedAndDependent:
				case BuildDetection.BuildOnlyModified:
					lock (unmodifiedProjects) {
						foreach (var pair in unmodifiedProjects) {
							LoggingService.Debug(pair.Key.Name + ": " + pair.Value);
						}
					}
					return new WrapperFactory(this, setting).GetWrapper(buildable);
				case BuildDetection.RegularBuild:
					return buildable;
				default:
					throw new NotSupportedException();
			}
		}
		
		sealed class DummyBuildable : IBuildable
		{
			IBuildable wrappedBuildable;
			
			public DummyBuildable(IBuildable wrappedBuildable)
			{
				this.wrappedBuildable = wrappedBuildable;
			}
			
			public string Name {
				get { return wrappedBuildable.Name; }
			}
			
			public ProjectBuildOptions CreateProjectBuildOptions(BuildOptions options, bool isRootBuildable)
			{
				return null;
			}
			
			public IEnumerable<IBuildable> GetBuildDependencies(ProjectBuildOptions buildOptions)
			{
				return Enumerable.Empty<IBuildable>();
			}
			
			public Task<bool> BuildAsync(ProjectBuildOptions options, IBuildFeedbackSink feedbackSink, IProgressMonitor progressMonitor)
			{
				return Task.FromResult(true);
			}
		}
		
		sealed class CompilationPass
		{
			public readonly int Index;
			
			static int nextIndex;
			
			public CompilationPass()
			{
				Index = System.Threading.Interlocked.Increment(ref nextIndex);
			}
			
			public override string ToString()
			{
				return "[CompilationPass " + Index + "]";
			}
		}
		
		sealed class WrapperFactory
		{
			public readonly BuildDetection Setting;
			public readonly CompilationPass CurrentPass = new CompilationPass();
			internal readonly BuildModifiedProjectsOnlyService Service;
			readonly Dictionary<IBuildable, IBuildable> dict = new Dictionary<IBuildable, IBuildable>();
			
			public WrapperFactory(BuildModifiedProjectsOnlyService service, BuildDetection setting)
			{
				this.Service = service;
				this.Setting = setting;
			}
			
			public IBuildable GetWrapper(IBuildable wrapped)
			{
				IBuildable b;
				lock (dict) {
					if (!dict.TryGetValue(wrapped, out b))
						b = dict[wrapped] = new Wrapper(wrapped, this);
				}
				return b;
			}
		}
		
		sealed class Wrapper : IBuildable
		{
			internal readonly IBuildable wrapped;
			internal readonly WrapperFactory factory;
			internal readonly BuildModifiedProjectsOnlyService service;
			
			public Wrapper(IBuildable wrapped, WrapperFactory factory)
			{
				this.wrapped = wrapped;
				this.factory = factory;
				this.service = factory.Service;
			}
			
			public string Name {
				get { return wrapped.Name; }
			}
			
			public ProjectBuildOptions CreateProjectBuildOptions(BuildOptions options, bool isRootBuildable)
			{
				return wrapped.CreateProjectBuildOptions(options, isRootBuildable);
			}
			
			Dictionary<ProjectBuildOptions, ICollection<IBuildable>> cachedBuildDependencies = new Dictionary<ProjectBuildOptions, ICollection<IBuildable>>();
			ICollection<IBuildable> cachedBuildDependenciesForNullOptions;
			
			public IEnumerable<IBuildable> GetBuildDependencies(ProjectBuildOptions buildOptions)
			{
				List<IBuildable> result = new List<IBuildable>();
				foreach (IBuildable b in wrapped.GetBuildDependencies(buildOptions)) {
					result.Add(factory.GetWrapper(b));
				}
				lock (cachedBuildDependencies) {
					if (buildOptions != null)
						cachedBuildDependencies[buildOptions] = result;
					else
						cachedBuildDependenciesForNullOptions = result;
				}
				return result;
			}
			
			CompilationPass lastCompilationPass;
			
			/// <summary>
			/// Returns true if "this" was recompiled after "comparisonPass".
			/// </summary>
			internal bool WasRecompiledAfter(CompilationPass comparisonPass)
			{
				Debug.Assert(comparisonPass != null);
				
				if (lastCompilationPass == null)
					return true;
				return lastCompilationPass.Index > comparisonPass.Index;
			}
			
			public async Task<bool> BuildAsync(ProjectBuildOptions options, IBuildFeedbackSink feedbackSink, IProgressMonitor progressMonitor)
			{
				IProject p = wrapped as IProject;
				if (p == null) {
					return await wrapped.BuildAsync(options, feedbackSink, progressMonitor);
				} else {
					lock (service.unmodifiedProjects) {
						if (!service.unmodifiedProjects.TryGetValue(p, out lastCompilationPass)) {
							lastCompilationPass = null;
						}
					}
					if (lastCompilationPass != null && factory.Setting == BuildDetection.BuildModifiedAndDependent) {
						lock (cachedBuildDependencies) {
							var dependencies = options != null ? cachedBuildDependencies[options] : cachedBuildDependenciesForNullOptions;
							if (dependencies.OfType<Wrapper>().Any(w=>w.WasRecompiledAfter(lastCompilationPass))) {
								lastCompilationPass = null;
							}
						}
					}
					if (lastCompilationPass != null) {
						feedbackSink.ReportMessage(
							StringParser.Parse("${res:MainWindow.CompilerMessages.SkipProjectNoChanges}",
							                   new StringTagPair("Name", p.Name))
						);
						return true;
					} else {
						lastCompilationPass = factory.CurrentPass;
						var success = await wrapped.BuildAsync(options, feedbackSink, progressMonitor);
						if (success) {
							lock (service.unmodifiedProjects) {
								service.unmodifiedProjects[p] = factory.CurrentPass;
							}
						}
						return success;
					}
				}
			}
		}
	}
}
