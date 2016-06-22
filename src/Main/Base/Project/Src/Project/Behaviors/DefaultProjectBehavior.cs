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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project.Converter;

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
				SD.Debugger.Start(psi);
			} else {
				SD.Debugger.StartWithoutDebugging(psi);
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
		
		public override IEnumerable<CompilerVersion> GetAvailableCompilerVersions()
		{
			return Enumerable.Empty<CompilerVersion>();
		}
		
		public override void UpgradeProject(CompilerVersion newVersion, TargetFramework newFramework)
		{
			throw new NotSupportedException();
		}
		
		public override void SavePreferences(Properties preferences)
		{
			SD.MainThread.VerifyAccess();
			
			// breakpoints and files
			preferences.SetList("bookmarks", SD.BookmarkManager.GetProjectBookmarks(Project));
			List<string> files = new List<string>();
			foreach (var fileName in FileService.GetOpenFiles()) {
				if (fileName != null && Project.IsFileInProject(fileName)) {
					files.Add(fileName);
				}
			}
			preferences.SetList("openFiles", files);
		}
		
		public override void ProjectLoaded()
		{
			SD.MainThread.VerifyAccess();
			
			var memento = Project.Preferences;
			foreach (var mark in memento.GetList<ICSharpCode.SharpDevelop.Editor.Bookmarks.SDBookmark>("bookmarks")) {
				SD.BookmarkManager.AddMark(mark);
			}
			List<string> filesToOpen = new List<string>();
			foreach (string fileName in memento.GetList<string>("openFiles")) {
				if (File.Exists(fileName)) {
					filesToOpen.Add(fileName);
				}
			}
			System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(
				System.Windows.Threading.DispatcherPriority.Loaded,
				new Action(
					delegate {
						NavigationService.SuspendLogging();
						foreach (string file in filesToOpen)
							FileService.OpenFile(file);
						NavigationService.ResumeLogging();
					}));
		}
	}
}
