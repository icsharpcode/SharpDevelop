// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class SharpDevelopStringTagProvider :  IStringTagProvider 
	{
		readonly static string[] tags = new string[] { "ItemPath", "ItemDir", "ItemFilename", "ItemExt",
				                      "CurLine", "CurCol", "CurText",
				                      "TargetPath", "TargetDir", "TargetName", "TargetExt",
				                      "ProjectDir", "ProjectFilename",
				                      "CombineDir", "CombineFilename",
				                      "Startuppath",
				                      "TaskService.Warnings",
				                      "TaskService.Errors",
				                      "TaskService.Messages"
		};

		
		public string[] Tags {
			get {
				return tags;
			}
		}
		
		string GetCurrentItemPath()
		{
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null && !WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.IsViewOnly && !WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.IsUntitled) {
				return WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName;
			}
			return String.Empty;
		}
		
		string GetCurrentTargetPath()
		{
//	TODO:		
//			if (ProjectService.CurrentProject != null) {
//				return ProjectService.GetOutputAssemblyName(ProjectService.CurrentProject);
//			}
//			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
//				string fileName = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName;
//				return ProjectService.GetOutputAssemblyName(fileName);
//			}
			return String.Empty;
		}
		
		public string Convert(string tag)
		{
			switch (tag) {
				case "TaskService.Warnings":
					return TaskService.GetCount(TaskType.Warning).ToString();
				case "TaskService.Errors":
					return TaskService.GetCount(TaskType.Error).ToString();
				case "TaskService.Messages":
					return TaskService.GetCount(TaskType.Message).ToString();
					
			}
			switch (tag.ToUpper()) {
					
				case "ITEMPATH":
					try {
						return GetCurrentItemPath();
					} catch (Exception) {}
					break;
				case "ITEMDIR":
					try {
						return Path.GetDirectoryName(GetCurrentItemPath());
					} catch (Exception) {}
					break;
				case "ITEMFILENAME":
					try {
						return Path.GetFileName(GetCurrentItemPath());
					} catch (Exception) {}
					break;
				case "ITEMEXT":
					try {
						return Path.GetExtension(GetCurrentItemPath());
					} catch (Exception) {}
					break;
				
				// TODO:
				case "CURLINE":
					return String.Empty;
				case "CURCOL":
					return String.Empty;
				case "CURTEXT":
					return String.Empty;
				
				case "TARGETPATH":
					try {
						return GetCurrentTargetPath();
					} catch (Exception) {}
					break;
				case "TARGETDIR":
					try {
						return Path.GetDirectoryName(GetCurrentTargetPath());
					} catch (Exception) {}
					break;
				case "TARGETNAME":
					try {
						return Path.GetFileName(GetCurrentTargetPath());
					} catch (Exception) {}
					break;
				case "TARGETEXT":
					try {
						return Path.GetExtension(GetCurrentTargetPath());
					} catch (Exception) {}
					break;
				
				case "PROJECTDIR":
					if (ProjectService.CurrentProject != null) {
						return ProjectService.CurrentProject.FileName;
					}
					break;
				case "PROJECTFILENAME":
					if (ProjectService.CurrentProject != null) {
						try {
							return Path.GetFileName(ProjectService.CurrentProject.FileName);
						} catch (Exception) {}
					}
					break;
				
				case "COMBINEDIR":
					return Path.GetDirectoryName(ProjectService.OpenSolution.FileName);
				case "COMBINEFILENAME":
					try {
						return Path.GetFileName(ProjectService.OpenSolution.FileName);
					} catch (Exception) {}
					break;
				case "STARTUPPATH":
					return Application.StartupPath;
			}
			return String.Empty;
		}
	}

}
