// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Shell;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// This class handles the recent open files and the recent open project files of SharpDevelop
	/// it checks, if the files exists at every creation, and if not it doesn't list them in the
	/// recent files, and they'll not be saved during the next option save.
	/// </summary>
	public sealed class RecentOpen
	{
		/// <summary>
		/// This variable is the maximal length of lastfile/lastopen entries
		/// must be > 0
		/// </summary>
		int MAX_LENGTH = 10;
		
		readonly ObservableCollection<string> lastfile    = new ObservableCollection<string>();
		readonly ObservableCollection<string> lastproject = new ObservableCollection<string>();
		
		public IList<string> RecentFile {
			get {
				return lastfile;
			}
		}

		public IList<string> RecentProject {
			get {
				return lastproject;
			}
		}
		
		public RecentOpen()
		{
		}
		
		public RecentOpen(Properties p)
		{
			// don't check whether files exist because that might be slow (e.g. if file is on network
			// drive that's unavailable)
			if (p.Contains("Files")) {
				lastfile.AddRange(p["Files"].Split(','));
			}
			
			if (p.Contains("Projects")) {
				lastproject.AddRange(p["Projects"].Split(','));
			}
		}
		
		public void AddLastFile(string name)
		{
			for (int i = 0; i < lastfile.Count; ++i) {
				if (lastfile[i].ToString().Equals(name, StringComparison.OrdinalIgnoreCase)) {
					lastfile.RemoveAt(i);
				}
			}
			
			while (lastfile.Count >= MAX_LENGTH) {
				lastfile.RemoveAt(lastfile.Count - 1);
			}
			
			if (lastfile.Count > 0) {
				lastfile.Insert(0, name);
			} else {
				lastfile.Add(name);
			}
		}
		
		public void ClearRecentFiles()
		{
			lastfile.Clear();
		}
		
		public void ClearRecentProjects()
		{
			lastproject.Clear();
		}
		
		public void AddLastProject(string name)
		{
			for (int i = 0; i < lastproject.Count; ++i) {
				if (lastproject[i].ToString().Equals(name, StringComparison.OrdinalIgnoreCase)) {
					lastproject.RemoveAt(i);
				}
			}
			
			while (lastproject.Count >= MAX_LENGTH) {
				lastproject.RemoveAt(lastproject.Count - 1);
			}
			
			if (lastproject.Count > 0) {
				lastproject.Insert(0, name);
			} else {
				lastproject.Add(name);
			}
			JumpList.AddToRecentCategory(name);
		}
		
		public static RecentOpen FromXmlElement(Properties properties)
		{
			return new RecentOpen(properties);
		}
		
		public Properties ToProperties()
		{
			Properties p = new Properties();
			p["Files"]    = String.Join(",", lastfile.ToArray());
			p["Projects"] = String.Join(",", lastproject.ToArray());
			return p;
		}
		
		internal void FileRemoved(object sender, FileEventArgs e)
		{
			for (int i = 0; i < lastfile.Count; ++i) {
				string file = lastfile[i].ToString();
				if (e.FileName == file) {
					lastfile.RemoveAt(i);
					break;
				}
			}
		}
		
		internal void FileRenamed(object sender, FileRenameEventArgs e)
		{
			for (int i = 0; i < lastfile.Count; ++i) {
				string file = lastfile[i].ToString();
				if (e.SourceFile == file) {
					lastfile.RemoveAt(i);
					lastfile.Insert(i, e.TargetFile);
					break;
				}
			}
		}
	}
}
