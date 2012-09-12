// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Shell;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// This class handles the recent open files and the recent open project files of SharpDevelop
	/// </summary>
	sealed class RecentOpen : IRecentOpen
	{
		/// <summary>
		/// This variable is the maximal length of lastfile/lastopen entries
		/// must be > 0
		/// </summary>
		int MAX_LENGTH = 10;
		
		ObservableCollection<string> recentFiles    = new ObservableCollection<string>();
		ObservableCollection<string> recentProjects = new ObservableCollection<string>();
		Properties properties;
		
		public IReadOnlyList<string> RecentFiles {
			get { return recentFiles; }
		}

		public IReadOnlyList<string> RecentProjects {
			get { return recentProjects; }
		}
		
		public RecentOpen(Properties p)
		{
			// don't check whether files exist because that might be slow (e.g. if file is on network
			// drive that's unavailable)
			this.properties = p;
			recentFiles.AddRange(p.GetList<string>("Files"));
			recentProjects.AddRange(p.GetList<string>("Projects"));
		}
		
		public void AddRecentFile(string name)
		{
			for (int i = 0; i < recentFiles.Count; ++i) {
				if (recentFiles[i].Equals(name, StringComparison.OrdinalIgnoreCase)) {
					recentFiles.RemoveAt(i);
				}
			}
			
			while (recentFiles.Count >= MAX_LENGTH) {
				recentFiles.RemoveAt(recentFiles.Count - 1);
			}
			
			recentFiles.Insert(0, name);
			properties.SetList("Files", recentFiles);
		}
		
		public void ClearRecentFiles()
		{
			recentFiles.Clear();
			properties.SetList("Files", recentFiles);
		}
		
		public void ClearRecentProjects()
		{
			recentProjects.Clear();
			properties.SetList("Projects", recentProjects);
		}
		
		public void AddRecentProject(string name)
		{
			for (int i = 0; i < recentProjects.Count; ++i) {
				if (recentProjects[i].ToString().Equals(name, StringComparison.OrdinalIgnoreCase)) {
					recentProjects.RemoveAt(i);
				}
			}
			
			while (recentProjects.Count >= MAX_LENGTH) {
				recentProjects.RemoveAt(recentProjects.Count - 1);
			}
			
			recentProjects.Insert(0, name);
			JumpList.AddToRecentCategory(name);
			properties.SetList("Projects", recentProjects);
		}
		
		internal void FileRemoved(object sender, FileEventArgs e)
		{
			for (int i = 0; i < recentFiles.Count; ++i) {
				string file = recentFiles[i].ToString();
				if (e.FileName == file) {
					recentFiles.RemoveAt(i);
					break;
				}
			}
		}
		
		internal void FileRenamed(object sender, FileRenameEventArgs e)
		{
			for (int i = 0; i < recentFiles.Count; ++i) {
				string file = recentFiles[i].ToString();
				if (e.SourceFile == file) {
					recentFiles.RemoveAt(i);
					recentFiles.Insert(i, e.TargetFile);
					break;
				}
			}
		}
	}
}
