using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public class SolutionFolder : AbstractSolutionFolder, ISolutionFolderContainer
	{
		public const string FolderGuid = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";
		
		public override string TypeGuid {
			get {
				return FolderGuid;
			}
			set {
				throw new System.NotSupportedException();
			}
		}
		
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
		
		public List<ProjectSection> Sections {
			get {
				return sections;
			}
		}
		
		public List<ISolutionFolder> Folders {
			get {
				return folders;
			}
		}
		
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
			if (folder.Parent != null) {
				folder.Parent.RemoveFolder(folder);
			}
			folder.Parent = this;
			Folders.Add(folder);
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
		public static SolutionFolder ReadFolder(StreamReader sr, string title, string location, string guid)
		{
			SolutionFolder newFolder = new SolutionFolder(title, location, guid);
			while (true) {
				string line = sr.ReadLine();
				if (line == null || line.Trim() == "EndProject") {
					break;
				}
				Match match = sectionHeaderPattern.Match(line);
				if (match.Success) {
					newFolder.Sections.Add(ProjectSection.ReadProjectSection(sr, match.Result("${Name}"), match.Result("${Type}")));
				}
			}
			return newFolder;
		}
		
	}
}
