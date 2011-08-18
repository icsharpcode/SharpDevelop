// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class SelectedMvcFolder : ISelectedMvcFolder
	{
		IMvcFileService fileService;
		ISelectedFolderNodeInProjectsView selectedFolderNode;
		
		public SelectedMvcFolder(
			ISelectedFolderNodeInProjectsView selectedFolderNode,
			IMvcFileService fileService)
		{
			this.selectedFolderNode = selectedFolderNode;
			this.fileService = fileService;
		}
		
		public SelectedMvcFolder(DirectoryNode directoryNode)
			: this(
				new SelectedFolderNodeInProjectsView(directoryNode),
				new MvcFileService())
		{
		}
		
		public SelectedMvcFolder()
			: this(ProjectBrowserPad.Instance.SelectedNode as DirectoryNode)
		{
		}
		
		public string Path {
			get { return selectedFolderNode.Folder; }
		}
		
		public IMvcProject Project {
			get { return selectedFolderNode.Project; }
		}
		
		public void AddFileToProject(string fileName)
		{
			string fullPath = GetFullPathToFile(fileName);
			AddNewFileToFolderNode(fullPath);
			SaveProject();
			OpenFile(fullPath);
		}
		
		string GetFullPathToFile(string fileName)
		{
			return System.IO.Path.Combine(Path, fileName);
		}
		
		void AddNewFileToFolderNode(string path)
		{
			selectedFolderNode.AddNewFile(path);
		}
		
		void SaveProject()
		{
			selectedFolderNode.Project.Save();
		}
		
		void OpenFile(string fullPath)
		{
			fileService.OpenFile(fullPath);
		}
		
		public MvcTextTemplateLanguage GetTemplateLanguage()
		{
			return Project.GetTemplateLanguage();
		}
	}
}
