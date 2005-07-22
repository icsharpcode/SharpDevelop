// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;
using System.Diagnostics;
using System.Collections;
using System.IO;


using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Core
{
	/// <summary>
	/// This class handles the recent open files and the recent open project files of SharpDevelop
	/// it checks, if the files exists at every creation, and if not it doesn't list them in the 
	/// recent files, and they'll not be saved during the next option save.
	/// </summary>
	public class RecentOpen
	{
		/// <summary>
		/// This variable is the maximal length of lastfile/lastopen entries
		/// must be > 0
		/// </summary>
		int MAX_LENGTH = 10;
		
		ArrayList lastfile    = new ArrayList();
		ArrayList lastproject = new ArrayList();
		
		public event EventHandler RecentFileChanged;
		public event EventHandler RecentProjectChanged;
		
		public ArrayList RecentFile {
			get {
				System.Diagnostics.Debug.Assert(lastfile != null, "RecentOpen : set string[] LastFile (value == null)");
				return lastfile;
			}
		}

		public ArrayList RecentProject {
			get {
				System.Diagnostics.Debug.Assert(lastproject != null, "RecentOpen : set string[] LastProject (value == null)");
				return lastproject;
			}
		}
		
		void OnRecentFileChange()
		{
			if (RecentFileChanged != null) {
				RecentFileChanged(this, null);
			}
		}
		
		void OnRecentProjectChange()
		{
			if (RecentProjectChanged != null) {
				RecentProjectChanged(this, null);
			}
		}

		public RecentOpen()
		{
		}
		
		public RecentOpen(Properties p)
		{
			if (p.Contains("Files")) {
				string[] files    = p["Files"].Split(',');
				foreach (string file in files) {
					if (File.Exists(file)) {
						lastfile.Add(file);
					}
				}
			}
			
			if (p.Contains("Projects")) {
				string[] projects = p["Projects"].Split(',');
				foreach (string file in projects) {
					if (File.Exists(file)) {
						lastproject.Add(file);
					}
				}
			}
		}
		
		public void AddLastFile(string name)
		{
			for (int i = 0; i < lastfile.Count; ++i) {
				if (lastfile[i].ToString().ToLower() == name.ToLower()) {
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
			
			OnRecentFileChange();
		}
		
		public void ClearRecentFiles()
		{
			lastfile.Clear();
			
			OnRecentFileChange();
		}
		
		public void ClearRecentProjects()
		{
			lastproject.Clear();
			
			OnRecentProjectChange();
		}
		
		public void AddLastProject(string name)
		{
			for (int i = 0; i < lastproject.Count; ++i) {
				if (lastproject[i].ToString().ToLower() == name.ToLower()) {
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
			OnRecentProjectChange();
		}
		
		public static RecentOpen FromXmlElement(Properties properties)
		{
			return new RecentOpen(properties);
		}
		
		public Properties ToProperties()
		{
			Properties p = new Properties();
			p["Files"]    = String.Join(",", (string[])lastfile.ToArray(typeof(string)));
			p["Projects"] = String.Join(",", (string[])lastproject.ToArray(typeof(string)));
			return p;
		}
		
		public void FileRemoved(object sender, FileEventArgs e)
		{
			for (int i = 0; i < lastfile.Count; ++i) {
				string file = lastfile[i].ToString();
				if (e.FileName == file) {
					lastfile.RemoveAt(i);
					OnRecentFileChange();
					break;
				}
			}
		}
		
		public void FileRenamed(object sender, FileRenameEventArgs e)
		{
			for (int i = 0; i < lastfile.Count; ++i) {
				string file = lastfile[i].ToString();
				if (e.SourceFile == file) {
					lastfile.RemoveAt(i);
					lastfile.Insert(i, e.TargetFile);
					OnRecentFileChange();
					break;
				}
			}
		}
	}
}
