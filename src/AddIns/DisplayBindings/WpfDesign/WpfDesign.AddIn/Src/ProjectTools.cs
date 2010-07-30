// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Kumar Devvrat"/>
//     <version>$Revision: $</version>
// </file>
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
		internal static void AddFiles(string []fileNames)
		{
			IProject project=ProjectService.CurrentProject;	
			Debug.Assert(project!=null);				
			
			foreach(var file in fileNames){
				string relFileName=FileUtility.GetRelativePath(project.Directory,file);
				FileProjectItem fileProjectItem=new FileProjectItem(project,project.GetDefaultItemType(file),relFileName);
				FileNode fileNode=new FileNode(file,FileNodeStatus.InProject);
				fileNode.ProjectItem=fileProjectItem;				
				ProjectService.AddProjectItem(project,fileProjectItem);
			}
			project.Save();
			ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
		}
		
		/// <summary>
		/// Get all files with given <param name="Extension"/> in the project and returns directory and the filename
		/// </summary>
		/// <param name="Extension"></param>
		/// <returns></returns>
		internal static List<KeyValuePair<string, string>> RetrieveFiles(string []Extension)
		{
			List<KeyValuePair<string, string>> files=new List<KeyValuePair<string, string>>();
			IProject project=ProjectService.CurrentProject;
			Debug.Assert(project!=null);
			
			foreach(var item in project.Items){
				FileProjectItem fileProjectItem=item as FileProjectItem;
				if(fileProjectItem!=null){
					string dirName=Path.GetDirectoryName(fileProjectItem.VirtualName);
					if(Extension.Contains(Path.GetExtension(fileProjectItem.VirtualName)))
						files.Add(new KeyValuePair<string, string>(dirName, fileProjectItem.FileName));
				}
			}
			
			return files;
		}
		
	}
}
