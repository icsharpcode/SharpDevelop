// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
				FileProjectItem item = project.FindFile(file);
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
