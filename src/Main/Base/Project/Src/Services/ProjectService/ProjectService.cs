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
			project.Items.Add(item);
		}
		
		/// <summary>
		/// Removes a project item from the project, raising the ProjectItemRemoved event.
		/// Make sure you call project.Save() after removing items!
		/// No action (not even raising the event) is taken when the item was already removed from the project.
		/// </summary>
		public static void RemoveProjectItem(IProject project, ProjectItem item)
		{
			if (project == null) throw new ArgumentNullException("project");
			if (item == null)    throw new ArgumentNullException("item");
			project.Items.Remove(item);
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
		
		/// <summary>
		/// Saves the current solution and all of its projects.
		/// </summary>
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
		/// Returns a File Dialog filter that can be used to filter on all registered file formats
		/// </summary>
		public static string GetAllFilesFilter()
		{
			IEnumerable<FileFilterDescriptor> filters = GetFileFilters();
			StringBuilder b = new StringBuilder(StringParser.Parse("${res:SharpDevelop.FileFilter.AllKnownFiles} (*.cs, *.vb, ...)|"));
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
		
		[Obsolete("Use SD.BuildService.IsBuilding instead")]
		public static bool IsBuilding {
			get {
				return SD.BuildService.IsBuilding;
			}
		}
		
		/// <summary>
		/// Is raised when a new project is created.
		/// </summary>
		[Obsolete("Use SD.ProjectService.ProjectCreated instead")]
		public static event EventHandler<ProjectEventArgs> ProjectCreated {
			add { SD.ProjectService.ProjectCreated += value; }
			remove { SD.ProjectService.ProjectCreated -= value; }
		}
		
		[Obsolete("Use SD.ProjectService.SolutionCreated instead")]
		public static event EventHandler<SolutionEventArgs> SolutionCreated {
			add { SD.ProjectService.SolutionCreated += value; }
			remove { SD.ProjectService.SolutionCreated -= value; }
		}
		
		[Obsolete("Use SD.BuildService.BuildStarted instead")]
		public static event EventHandler<BuildEventArgs> BuildStarted {
			add { SD.BuildService.BuildStarted += value; }
			remove { SD.BuildService.BuildStarted -= value; }
		}
		
		[Obsolete("Use SD.BuildService.BuildFinished instead")]
		public static event EventHandler<BuildEventArgs> BuildFinished {
			add { SD.BuildService.BuildFinished += value; }
			remove { SD.BuildService.BuildFinished -= value; }
		}
		
		[Obsolete("Use SD.ProjectService.SolutionOpened instead")]
		public static event EventHandler<SolutionEventArgs> SolutionLoaded {
			add { SD.ProjectService.SolutionOpened += value; }
			remove { SD.ProjectService.SolutionOpened -= value; }
		}
		
		[Obsolete("Use SD.ProjectService.SolutionClosed instead")]
		public static event EventHandler<SolutionEventArgs> SolutionClosed {
			add { SD.ProjectService.SolutionClosed += value; }
			remove { SD.ProjectService.SolutionClosed -= value; }
		}
		
		[Obsolete("Use SD.ProjectService.SolutionClosing instead")]
		public static event EventHandler<SolutionClosingEventArgs> SolutionClosing {
			add { SD.ProjectService.SolutionClosing += value; }
			remove { SD.ProjectService.SolutionClosing -= value; }
		}
		
		static EventAdapter<IProjectService, PropertyChangedEventHandler<IProject>, EventHandler<ProjectEventArgs>> currentProjectChangedAdapter =
			new EventAdapter<IProjectService, PropertyChangedEventHandler<IProject>, EventHandler<ProjectEventArgs>>(
				SD.GetService<IProjectService>(), (s, v) => s.CurrentProjectChanged += v, (s, v) => s.CurrentProjectChanged -= v,
				handler => (sender, e) => handler(null, new ProjectEventArgs(e.NewValue)));
		
		[Obsolete("Use SD.ProjectService.CurrentProjectChanged instead")]
		public static event EventHandler<ProjectEventArgs> CurrentProjectChanged {
			add { currentProjectChangedAdapter.Add(value); }
			remove { currentProjectChangedAdapter.Remove(value); }
		}
		
		[Obsolete("Use SD.ProjectService.ProjectItemAdded instead")]
		public static event EventHandler<ProjectItemEventArgs> ProjectItemAdded {
			add { SD.ProjectService.ProjectItemAdded += value; }
			remove { SD.ProjectService.ProjectItemAdded -= value; }
		}
		[Obsolete("Use SD.ProjectService.ProjectItemRemoved instead")]
		public static event EventHandler<ProjectItemEventArgs> ProjectItemRemoved {
			add { SD.ProjectService.ProjectItemRemoved += value; }
			remove { SD.ProjectService.ProjectItemRemoved -= value; }
		}
	}
}
