// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using System.Diagnostics;
using System.ComponentModel;

namespace ICSharpCode.SharpDevelop.Project
{
	public enum BuildOnExecuteSetting
	{
		[Description("${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.WhenRunning.DoNotBuild}")]
		DoNotBuild,
		[Description("${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.WhenRunning.BuildOnlyModified}")]
		BuildOnlyModified,
		[Description("${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.WhenRunning.BuildModifiedAndDependent}")]
		BuildModifiedAndDependent,
		[Description("${res:Dialog.Options.IDEOptions.ProjectAndSolutionOptions.WhenRunning.RegularBuild}")]
		RegularBuild
	}
	
	/// <summary>
	/// Tracks changes to projects and causes only modified projects
	/// to be recompiled.
	/// </summary>
	static class BuildModifiedProjectsOnlyService
	{
		public static BuildOnExecuteSetting Setting {
			get { return PropertyService.Get("BuildOnExecute", BuildOnExecuteSetting.RegularBuild); }
			set { PropertyService.Set("BuildOnExecute", value); }
		}
		
		static readonly Dictionary<IProject, CompilationPass> unmodifiedProjects = new Dictionary<IProject, CompilationPass>();
		
		static BuildModifiedProjectsOnlyService()
		{
			// these actions cause a full recompilation:
			ProjectService.SolutionClosed += MarkAllForRecompilation;
			ProjectService.SolutionConfigurationChanged += MarkAllForRecompilation;
			ProjectService.SolutionSaved += MarkAllForRecompilation;
			BuildEngine.GuiBuildFinished += BuildEngine_GuiBuildFinished;
			
			FileUtility.FileSaved += OnFileSaved;
		}
		
		public static void Initialize()
		{
			// first call to init causes static ctor calls
		}
		
		static void BuildEngine_GuiBuildFinished(object sender, BuildEventArgs e)
		{
			// at the end of an successful build, mark all built projects as unmodified
			if (e.Results.Result == BuildResultCode.Success) {
				lock (unmodifiedProjects) {
					CompilationPass pass = new CompilationPass();
					foreach (IBuildable b in e.Results.BuiltProjects) {
						IProject p = GetProjectFromBuildable(b);
						if (p != null) {
							unmodifiedProjects[p] = pass;
						}
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
		
		static IProject GetProjectFromBuildable(IBuildable b)
		{
			while (b is Wrapper)
				b = ((Wrapper)b).wrapped;
			return b as IProject;
		}
		
		static void MarkAllForRecompilation(object sender, EventArgs e)
		{
			lock (unmodifiedProjects) {
				unmodifiedProjects.Clear();
			}
		}
		
		static void OnFileSaved(object sender, FileNameEventArgs e)
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
		
		public static IBuildable WrapBuildable(IBuildable buildable)
		{
			switch (Setting) {
				case BuildOnExecuteSetting.DoNotBuild:
					return new DummyBuildable(buildable);
				case BuildOnExecuteSetting.BuildModifiedAndDependent:
				case BuildOnExecuteSetting.BuildOnlyModified:
					lock (unmodifiedProjects) {
						foreach (var pair in unmodifiedProjects) {
							LoggingService.Debug(pair.Key.Name + ": " + pair.Value);
						}
					}
					return new WrapperFactory().GetWrapper(buildable);
				case BuildOnExecuteSetting.RegularBuild:
					return buildable;
				default:
					throw new NotSupportedException();
			}
		}
		
		sealed class DummyBuildable : IBuildable2
		{
			IBuildable wrappedBuildable;
			
			public DummyBuildable(IBuildable wrappedBuildable)
			{
				this.wrappedBuildable = wrappedBuildable;
			}
			
			public string Name {
				get { return wrappedBuildable.Name; }
			}
			
			public Solution ParentSolution {
				get { return wrappedBuildable.ParentSolution; }
			}
			
			public ProjectBuildOptions CreateProjectBuildOptions(BuildOptions options, bool isRootBuildable)
			{
				return null;
			}
			
			public ICollection<IBuildable> GetBuildDependencies(ProjectBuildOptions buildOptions)
			{
				return new IBuildable[0];
			}
			
			public void StartBuild(ProjectBuildOptions buildOptions, IBuildFeedbackSink feedbackSink)
			{
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
			public readonly CompilationPass CurrentPass = new CompilationPass();
			readonly Dictionary<IBuildable, IBuildable> dict = new Dictionary<IBuildable, IBuildable>();
			
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
		
		sealed class Wrapper : IBuildable2
		{
			internal readonly IBuildable wrapped;
			internal readonly WrapperFactory factory;
			
			public Wrapper(IBuildable wrapped, WrapperFactory factory)
			{
				this.wrapped = wrapped;
				this.factory = factory;
			}
			
			public string Name {
				get { return wrapped.Name; }
			}
			
			public Solution ParentSolution {
				get { return wrapped.ParentSolution; }
			}
			
			public ProjectBuildOptions CreateProjectBuildOptions(BuildOptions options, bool isRootBuildable)
			{
				return wrapped.CreateProjectBuildOptions(options, isRootBuildable);
			}
			
			Dictionary<ProjectBuildOptions, ICollection<IBuildable>> cachedBuildDependencies = new Dictionary<ProjectBuildOptions, ICollection<IBuildable>>();
			ICollection<IBuildable> cachedBuildDependenciesForNullOptions;
				
			public ICollection<IBuildable> GetBuildDependencies(ProjectBuildOptions buildOptions)
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
			
			public void StartBuild(ProjectBuildOptions buildOptions, IBuildFeedbackSink feedbackSink)
			{
				IProject p = wrapped as IProject;
				if (p == null) {
					wrapped.StartBuild(buildOptions, feedbackSink);
				} else {
					lock (unmodifiedProjects) {
						if (!unmodifiedProjects.TryGetValue(p, out lastCompilationPass)) {
							lastCompilationPass = null;
						}
					}
					if (lastCompilationPass != null && Setting == BuildOnExecuteSetting.BuildModifiedAndDependent) {
						lock (cachedBuildDependencies) {
							var dependencies = buildOptions != null ? cachedBuildDependencies[buildOptions] : cachedBuildDependenciesForNullOptions;
							if (dependencies.OfType<Wrapper>().Any(w=>w.WasRecompiledAfter(lastCompilationPass))) {
								lastCompilationPass = null;
							}
						}
					}
					if (lastCompilationPass != null) {
						feedbackSink.ReportMessage(
							StringParser.Parse("${res:MainWindow.CompilerMessages.SkipProjectNoChanges}",
							                   new string[,] {{ "Name", p.Name }})
						);
						feedbackSink.Done(true);
					} else {
						lastCompilationPass = factory.CurrentPass;
						wrapped.StartBuild(buildOptions, new BuildFeedbackSink(p, feedbackSink, factory.CurrentPass));
					}
				}
			}
			
			/// <summary>
			/// Wraps a build feedback sink and marks a project as requiring recompilation when
			/// compilation was not successful.
			/// </summary>
			sealed class BuildFeedbackSink : IBuildFeedbackSink
			{
				IProject project;
				IBuildFeedbackSink sink;
				CompilationPass currentPass;
				
				public BuildFeedbackSink(IProject p, IBuildFeedbackSink sink, CompilationPass currentPass)
				{
					Debug.Assert(p != null);
					Debug.Assert(sink != null);
					Debug.Assert(currentPass != null);
					this.project = p;
					this.sink = sink;
					this.currentPass = currentPass;
				}
				
				public void ReportError(BuildError error)
				{
					sink.ReportError(error);
				}
				
				public void ReportMessage(string message)
				{
					sink.ReportMessage(message);
				}
				
				public void Done(bool success)
				{
					if (success) {
						lock (unmodifiedProjects) {
							unmodifiedProjects[project] = currentPass;
						}
					}
					sink.Done(success);
				}
			}
		}
	}
}
