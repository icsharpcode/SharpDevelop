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
