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
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Core;

namespace ICSharpCode.WpfDesign.AddIn
{
	/// <summary>
	/// Static helper methods to interact with the project
	/// </summary>
	internal class ProjectTools
	{
		/// <summary>
		/// Add files to the current project at the project node.
		/// </summary>
		/// <param name="fileNames">list of files that have to be added.</param>
		internal static IEnumerable<FileProjectItem> AddFiles(string[] fileNames, ItemType itemType)
		{
			IProject project=ProjectService.CurrentProject;	
			Debug.Assert(project!=null);				
			
			List<FileProjectItem> resultItems = new List<FileProjectItem>();
			foreach (string file in fileNames) {
				FileProjectItem item = project.FindFile(FileName.Create(file));
				if (item != null)
					continue; // file already belongs to the project
				string relFileName = FileUtility.GetRelativePath(project.Directory,file);
				item = new FileProjectItem(project, itemType, relFileName);
				ProjectService.AddProjectItem(project, item);
				resultItems.Add(item);
			}
			project.Save();
			ProjectBrowserPad.RefreshViewAsync();
			return resultItems;
		}
		
		/// <summary>
		/// Get all files with given <param name="Extension"/> in the project and returns directory and the filename
		/// </summary>
		/// <param name="Extension"></param>
		/// <returns></returns>
		internal static List<FileProjectItem> RetrieveFiles(string []Extension)
		{
			List<FileProjectItem> files=new List<FileProjectItem>();
			IProject project=ProjectService.CurrentProject;
			Debug.Assert(project!=null);
			
			foreach(var item in project.Items){
				FileProjectItem fileProjectItem=item as FileProjectItem;
				if(fileProjectItem!=null){
					if(Extension.Contains(Path.GetExtension(fileProjectItem.VirtualName).ToLowerInvariant()))
						files.Add(fileProjectItem);
				}
			}
			
			return files;
		}
		
	}
}
