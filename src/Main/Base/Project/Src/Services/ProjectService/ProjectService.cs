// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.SharpDevelop.Project
{
	public static class ProjectService
	{
		public static ISolution OpenSolution {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				return SD.ProjectService.CurrentSolution;
			}
		}
		
		/// <summary>
		/// Gets/Sets the active project.
		/// Returns null if no project is active.
		/// The getter is thread-safe; the setter may only be called on the main thread.
		/// </summary>
		public static IProject CurrentProject {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				return SD.ProjectService.CurrentProject;
			}
			set {
				SD.ProjectService.CurrentProject = value;
			}
		}
		
		/// <summary>
		/// Returns if a project loader exists for the given file. This method works even in early
		/// startup (before service initialization)
		/// </summary>
		[Obsolete("Use SD.ProjectService.IsProjectOrSolutionFile instead")]
		public static bool HasProjectLoader(string fileName)
		{
			return SD.ProjectService.IsSolutionOrProjectFile(FileName.Create(fileName));
		}
		
		[Obsolete("Use SD.ProjectService.OpenSolutionOrProject instead")]
		public static void LoadSolutionOrProject(string fileName)
		{
			SD.ProjectService.OpenSolutionOrProject(FileName.Create(fileName));
		}
		
		/// <summary>
		/// Adds a project item to the project, raising the ProjectItemAdded event.
		/// Make sure you call project.Save() after adding new items!
		/// </summary>
		public static void AddProjectItem(IProject project, ProjectItem item)
		{
			if (project == null) throw new ArgumentNullException("project");
			if (item == null)    throw new ArgumentNullException("item");
			IProjectItemListProvider provider = project as IProjectItemListProvider;
			if (provider != null) {
				provider.AddProjectItem(item);
				OnProjectItemAdded(new ProjectItemEventArgs(project, item));
			}
		}
		
		/// <summary>
		/// Removes a project item from the project, raising the ProjectItemRemoved event.
		/// Make sure you call project.Save() after removing items!
		/// No action (not even raising the event) is taken when the item was already removed form the project.
		/// </summary>
		public static void RemoveProjectItem(IProject project, ProjectItem item)
		{
			if (project == null) throw new ArgumentNullException("project");
			if (item == null)    throw new ArgumentNullException("item");
			IProjectItemListProvider provider = project as IProjectItemListProvider;
			if (provider != null) {
				if (provider.RemoveProjectItem(item)) {
					OnProjectItemRemoved(new ProjectItemEventArgs(project, item));
				}
			}
		}
		
		[Obsolete("Use SD.ProjectService.OpenSolutionOrProject() instead")]
		public static void LoadSolution(string fileName)
		{
			SD.ProjectService.OpenSolutionOrProject(FileName.Create(fileName));
			//FileUtility.ObservedLoad(LoadSolutionInternal, fileName);
		}
		
		/// <summary>
		/// Load a single project as solution.
		/// </summary>
		[Obsolete("Use SD.ProjectService.OpenSolutionOrProject() instead")]
		public static void LoadProject(string fileName)
		{
			SD.ProjectService.OpenSolutionOrProject(FileName.Create(fileName));
			//FileUtility.ObservedLoad(LoadProjectInternal, fileName);
		}
		
		public static void SaveSolution()
		{
			var openSolution = SD.ProjectService.CurrentSolution;
			if (openSolution != null) {
				openSolution.Save();
				foreach (IProject project in openSolution.Projects) {
					project.Save();
				}
			}
		}
		
		/// <summary>
		/// Gets the list of file filters.
		/// </summary>
		public static IReadOnlyList<FileFilterDescriptor> GetFileFilters()
		{
			return AddInTree.BuildItems<FileFilterDescriptor>("/SharpDevelop/Workbench/FileFilter", null);
		}
		
		/// <summary>
		/// Returns a File Dialog filter that can be used to filter on all registered project formats
		/// </summary>
		public static string GetAllProjectsFilter(object caller, bool includeSolutions)
		{
			IEnumerable<FileFilterDescriptor> filters = AddInTree.BuildItems<FileFilterDescriptor>("/SharpDevelop/Workbench/Combine/FileFilter", null);
			if (!includeSolutions)
				filters = filters.Where(f => !f.ContainsExtension(".sln"));
			StringBuilder b = new StringBuilder(StringParser.Parse("${res:SharpDevelop.Solution.AllKnownProjectFormats}|"));
			bool first = true;
			foreach (var filter in filters) {
				string ext = filter.Extensions;
				if (ext != "*.*" && ext.Length > 0) {
					if (!first) {
						b.Append(';');
					} else {
						first = false;
					}
					b.Append(ext);
				}
			}
			foreach (var filter in filters) {
				b.Append('|');
				b.Append(filter.ToString());
			}
			return b.ToString();
		}
		
		[Obsolete]
		public static bool IsBuilding {
			get {
				return SD.BuildService.IsBuilding;
			}
		}
		
		internal static void OnProjectItemAdded(ProjectItemEventArgs e)
		{
			if (ProjectItemAdded != null) {
				ProjectItemAdded(null, e);
			}
		}
		internal static void OnProjectItemRemoved(ProjectItemEventArgs e)
		{
			if (ProjectItemRemoved != null) {
				ProjectItemRemoved(null, e);
			}
		}
		
		internal static void OnProjectCreated(ProjectEventArgs e)
		{
			if (ProjectCreated != null) {
				ProjectCreated(null, e);
			}
		}
		internal static void OnSolutionCreated(SolutionEventArgs e)
		{
			if (SolutionCreated != null) {
				SolutionCreated(null, e);
			}
		}
		
		/// <summary>
		/// Is raised when a new project is created.
		/// </summary>
		public static event EventHandler<ProjectEventArgs> ProjectCreated;
		public static event EventHandler<SolutionEventArgs> SolutionCreated;
		
		public static event EventHandler<BuildEventArgs> BuildStarted {
			add { SD.BuildService.BuildStarted += value; }
			remove { SD.BuildService.BuildStarted -= value; }
		}
		
		public static event EventHandler<BuildEventArgs> BuildFinished {
			add { SD.BuildService.BuildFinished += value; }
			remove { SD.BuildService.BuildFinished -= value; }
		}
		
		public static event EventHandler<SolutionEventArgs> SolutionLoaded {
			add { SD.ProjectService.SolutionOpened += value; }
			remove { SD.ProjectService.SolutionOpened -= value; }
		}
		
		public static event EventHandler<SolutionEventArgs> SolutionClosed {
			add { SD.ProjectService.SolutionClosed += value; }
			remove { SD.ProjectService.SolutionClosed -= value; }
		}
		
		public static event EventHandler<SolutionClosingEventArgs> SolutionClosing {
			add { SD.ProjectService.SolutionClosing += value; }
			remove { SD.ProjectService.SolutionClosing -= value; }
		}
		
		/// <summary>
		/// Raised before the solution preferences are being saved. Allows you to save
		/// your additional properties in the solution preferences.
		/// </summary>
		public static event EventHandler<SolutionEventArgs> SolutionPreferencesSaving;
		
		static EventAdapter<IProjectService, PropertyChangedEventHandler<IProject>, EventHandler<ProjectEventArgs>> currentProjectChangedAdapter =
			new EventAdapter<IProjectService, PropertyChangedEventHandler<IProject>, EventHandler<ProjectEventArgs>>(
				SD.GetService<IProjectService>(), (s, v) => s.CurrentProjectChanged += v, (s, v) => s.CurrentProjectChanged -= v,
				handler => (sender, e) => handler(null, new ProjectEventArgs(e.NewValue)));
		
		[Obsolete("Use SD.ProjectService.CurrentProjectChanged instead")]
		public static event EventHandler<ProjectEventArgs> CurrentProjectChanged {
			add { currentProjectChangedAdapter.Add(value); }
			remove { currentProjectChangedAdapter.Remove(value); }
		}
		
		public static event EventHandler<ProjectItemEventArgs> ProjectItemAdded;
		public static event EventHandler<ProjectItemEventArgs> ProjectItemRemoved;
	}
}
