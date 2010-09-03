// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

namespace ICSharpCode.SharpDevelop.Project
{
	public class SolutionFolder : AbstractSolutionFolder, ISolutionFolderContainer
	{
		public const string FolderGuid = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";
		
		[Browsable(false)]
		public override string TypeGuid {
			get {
				return FolderGuid;
			}
			set {
				throw new System.NotSupportedException();
			}
		}
		
		[Browsable(false)]
		public bool IsEmpty {
			get {
				return Folders.Count == 0 && SolutionItems.Items.Count == 0;
			}
		}
		
		protected SolutionFolder()
		{
		}
		
		public SolutionFolder(string name, string location, string idGuid)
		{
			this.Location = location;
			this.Name     = name;
			this.IdGuid   = idGuid;
		}
		
		#region ISolutionFolderContainer implementation
		List<ISolutionFolder> folders  = new List<ISolutionFolder>();
		List<ProjectSection>  sections = new List<ProjectSection>();
		
		[Browsable(false)]
		public List<ProjectSection> Sections {
			get {
				return sections;
			}
		}
		
		[Browsable(false)]
		public List<ISolutionFolder> Folders {
			get {
				return folders;
			}
		}
		
		[Browsable(false)]
		public virtual ProjectSection SolutionItems {
			get {
				foreach (ProjectSection section in sections) {
					if (section.Name == "SolutionItems") {
						return section;
					}
				}
				ProjectSection solutionItems = new ProjectSection("SolutionItems", "postProject");
				sections.Add(solutionItems);
				return solutionItems;
			}
		}
		
		public virtual void AddFolder(ISolutionFolder folder)
		{
			if (string.IsNullOrEmpty(folder.IdGuid)) {
				folder.IdGuid = Guid.NewGuid().ToString().ToUpperInvariant();
			}
			
			bool isNew = false;
			if (folder.Parent != null) {
				folder.Parent.RemoveFolder(folder);
			} else {
				// this is a new project/solution folder
				isNew = true;
			}
			if (isNew) {
				this.ParentSolution.BeforeAddFolderToSolution(folder);
			}
			folder.Parent = this;
			Folders.Add(folder);
			if (isNew) {
				this.ParentSolution.AfterAddFolderToSolution(folder);
			}
		}
		
		public virtual void RemoveFolder(ISolutionFolder folder)
		{
			for (int i = 0; i < Folders.Count; ++i) {
				if (folder.IdGuid == Folders[i].IdGuid) {
					Folders.RemoveAt(i);
					break;
				}
			}
		}
		
		public bool IsAncestorOf(ISolutionFolder folder)
		{
			object curParent = folder;
			while (curParent != null && curParent is ISolutionFolder) {
				ISolutionFolder curFolder = (ISolutionFolder)curParent;
				if (curFolder == this) {
					return true;
				}
				curParent = curFolder.Parent;
			}
			return false;
		}
		#endregion
		
		static Regex sectionHeaderPattern = new Regex("\\s*ProjectSection\\((?<Name>.*)\\)\\s*=\\s*(?<Type>.*)", RegexOptions.Compiled);
		
		public static SolutionFolder ReadFolder(TextReader sr, string title, string location, string guid)
		{
			SolutionFolder newFolder = new SolutionFolder(title, location, guid);
			ReadProjectSections(sr, newFolder.Sections);
			return newFolder;
		}
		
		/// <summary>
		/// Reads project sections from the TextReader until the line "EndProject" is found and saves
		/// them into the specified sectionList.
		/// </summary>
		public static void ReadProjectSections(TextReader sr, ICollection<ProjectSection> sectionList)
		{
			while (true) {
				string line = sr.ReadLine();
				if (line == null || line.Trim() == "EndProject") {
					break;
				}
				Match match = sectionHeaderPattern.Match(line);
				if (match.Success) {
					sectionList.Add(ProjectSection.ReadProjectSection(sr, match.Result("${Name}"), match.Result("${Type}")));
				}
			}
		}
	}
}
