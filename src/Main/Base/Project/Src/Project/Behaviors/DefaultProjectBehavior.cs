// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project.Converter;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.SharpDevelop.Project
{
	public sealed class DefaultProjectBehavior : ProjectBehavior
	{
		public DefaultProjectBehavior(AbstractProject project)
			: base(project)
		{
		}
		
		new AbstractProject Project {
			get {
				return (AbstractProject)base.Project;
			}
		}
		
		public override bool IsStartable {
			get { return false; }
		}
		
		public override void Start(bool withDebugging)
		{
			ProcessStartInfo psi;
			try {
				// we have to call CreateStartInfo through IProject, because otherwise the
				// project behavior chain would not be processed!
				psi = Project.CreateStartInfo();
			} catch (ProjectStartException ex) {
				MessageService.ShowError(ex.Message);
				return;
			}
			if (withDebugging) {
				DebuggerService.CurrentDebugger.Start(psi);
			} else {
				DebuggerService.CurrentDebugger.StartWithoutDebugging(psi);
			}
		}
		
		public override ProcessStartInfo CreateStartInfo()
		{
			throw new NotSupportedException();
		}
		
		public override ItemType GetDefaultItemType(string fileName)
		{
			return ItemType.None;
		}
		
		public override ProjectItem CreateProjectItem(IProjectItemBackendStore item)
		{
			return new UnknownProjectItem(Project, item);
		}
		
		public override void ProjectCreationComplete()
		{
			
		}
		
		public override IEnumerable<CompilerVersion> GetAvailableCompilerVersions()
		{
			return Enumerable.Empty<CompilerVersion>();
		}
		
		public override void UpgradeProject(CompilerVersion newVersion, TargetFramework newFramework)
		{
			throw new NotSupportedException();
		}
		
		
		/// <summary>
		/// Saves project preferences (currently opened files, bookmarks etc.) to the
		/// a property container.
		/// </summary>
		public override Properties CreateMemento()
		{
			WorkbenchSingleton.AssertMainThread();
			
			// breakpoints and files
			Properties properties = new Properties();
			properties.Set("bookmarks", ICSharpCode.SharpDevelop.Bookmarks.BookmarkManager.GetProjectBookmarks(Project).ToArray());
			List<string> files = new List<string>();
			foreach (string fileName in FileService.GetOpenFiles()) {
				if (fileName != null && Project.IsFileInProject(fileName)) {
					files.Add(fileName);
				}
			}
			properties.Set("files", files.ToArray());
			
			// other project data
			properties.Set("projectSavedData", Project.ProjectSpecificProperties ?? new Properties());
			
			return properties;
		}
		
		public override void SetMemento(Properties memento)
		{
			WorkbenchSingleton.AssertMainThread();
			
			foreach (ICSharpCode.SharpDevelop.Bookmarks.SDBookmark mark in memento.Get("bookmarks", new ICSharpCode.SharpDevelop.Bookmarks.SDBookmark[0])) {
				ICSharpCode.SharpDevelop.Bookmarks.BookmarkManager.AddMark(mark);
			}
			foreach (string fileName in memento.Get("files", new string[0])) {
				AbstractProject.filesToOpenAfterSolutionLoad.Add(fileName);
			}
			
			// other project data
			Project.ProjectSpecificProperties = memento.Get("projectSavedData", new Properties());
			base.SetMemento(memento);
		}
	}
}
