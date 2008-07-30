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
		// TODO: translate
		[Description("Do not build")]
		DoNotBuild,
		[Description("Build modified projects only")]
		BuildOnlyModified,
		[Description("Build modified projects and projects depending on them")]
		BuildModifiedAndDependent,
		[Description("Build all projects")]
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
		
		static readonly HashSet<IProject> unmodifiedProjects = new HashSet<IProject>();
		
		static BuildModifiedProjectsOnlyService()
		{
			// these actions cause a full recompilation:
			ProjectService.SolutionClosed += MarkAllForRecompilation;
			ProjectService.SolutionConfigurationChanged += MarkAllForRecompilation;
			ProjectService.SolutionSaved += MarkAllForRecompilation;
			ProjectService.EndBuild += ProjectService_EndBuild;
			
			FileUtility.FileSaved += OnFileSaved;
		}
		
		public static void Initialize()
		{
			// first call to init causes static ctor calls
		}
		
		static void ProjectService_EndBuild(object sender, BuildEventArgs e)
		{
			// at the end of an successful build, mark all projects as unmodified
			if (e.Results.Result == BuildResultCode.Success) {
				if (ProjectService.OpenSolution != null) {
					lock (unmodifiedProjects) {
						unmodifiedProjects.AddRange(ProjectService.OpenSolution.Projects);
					}
				}
			}
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
					if (p.FindFile(e.FileName) != null) {
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
					return new WrapperFactory().GetWrapper(buildable);
				case BuildOnExecuteSetting.RegularBuild:
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
			
			public Solution ParentSolution {
				get { return wrappedBuildable.ParentSolution; }
			}
			
			public ICollection<IBuildable> GetBuildDependencies(ProjectBuildOptions buildOptions)
			{
				return new IBuildable[0];
			}
			
			public void StartBuild(ProjectBuildOptions buildOptions, IBuildFeedbackSink feedbackSink)
			{
			}
		}
		
		sealed class WrapperFactory
		{
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
		
		sealed class Wrapper : IBuildable
		{
			IBuildable wrapped;
			WrapperFactory factory;
			
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
			
			Dictionary<ProjectBuildOptions, ICollection<IBuildable>> cachedBuildDependencies = new Dictionary<ProjectBuildOptions, ICollection<IBuildable>>();
			
			public ICollection<IBuildable> GetBuildDependencies(ProjectBuildOptions buildOptions)
			{
				List<IBuildable> result = new List<IBuildable>();
				foreach (IBuildable b in wrapped.GetBuildDependencies(buildOptions)) {
					result.Add(factory.GetWrapper(b));
				}
				lock (cachedBuildDependencies) {
					cachedBuildDependencies[buildOptions] = result;
				}
				return result;
			}
			
			internal bool wasRecompiled;
			
			public void StartBuild(ProjectBuildOptions buildOptions, IBuildFeedbackSink feedbackSink)
			{
				IProject p = wrapped as IProject;
				if (p == null) {
					wrapped.StartBuild(buildOptions, feedbackSink);
				} else {
					bool isUnmodified;
					lock (unmodifiedProjects) {
						isUnmodified = unmodifiedProjects.Contains(p);
						// mark project as unmodified
						unmodifiedProjects.Add(p);
					}
					if (isUnmodified && Setting == BuildOnExecuteSetting.BuildModifiedAndDependent) {
						lock (cachedBuildDependencies) {
							if (cachedBuildDependencies[buildOptions].OfType<Wrapper>().Any(w=>w.wasRecompiled)) {
								isUnmodified = false;
							}
						}
					}
					if (isUnmodified) {
						feedbackSink.ReportMessage("Skipped " + p.Name + " (no changes inside SharpDevelop)");
						feedbackSink.Done(true);
					} else {
						wasRecompiled = true;
						wrapped.StartBuild(buildOptions, new BuildFeedbackSink(p, feedbackSink));
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
				
				public BuildFeedbackSink(IProject p, IBuildFeedbackSink sink)
				{
					Debug.Assert(p != null);
					Debug.Assert(sink != null);
					this.project = p;
					this.sink = sink;
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
					if (!success) {
						// force recompilation if there was a build error
						lock (unmodifiedProjects) {
							unmodifiedProjects.Remove(project);
						}
					}
					sink.Done(success);
				}
			}
		}
	}
}
