// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class SelectedMvcFolder : ISelectedMvcFolder
	{
		DirectoryNode directoryNode;
		
		public SelectedMvcFolder(DirectoryNode directoryNode)
		{
			this.directoryNode = directoryNode;
		}
		
		public SelectedMvcFolder()
			: this(ProjectBrowserPad.Instance.SelectedNode as DirectoryNode)
		{
		}
		
		public string Path {
			get { return directoryNode.Directory; }
		}
		
		public IProject Project {
			get { return directoryNode.Project; }
		}
		
		public void AddFileToProject(string fileName)
		{
			string fullPath = System.IO.Path.Combine(Path, fileName);
			AddNewFileToDirectoryNode(fullPath);
			SaveProject();
		}
		
		protected virtual void AddNewFileToDirectoryNode(string path)
		{
			directoryNode.AddNewFile(path);
		}
		
		void SaveProject()
		{
			directoryNode.Project.Save();
		}
		
		public MvcTextTemplateLanguage GetTemplateLanguage()
		{
			string language = Project.Language;
			return MvcTextTemplateLanguageConverter.Convert(language);
		}
	}
}
