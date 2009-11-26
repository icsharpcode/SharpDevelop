// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Commands
{
	/// <summary>
	/// Provides tag to string mapping for SharpDevelop. Tags are mapped to strings by several methods
	/// such as registry and resource files.
	/// </summary>
	public class SharpDevelopStringTagProvider :  IStringTagProvider
	{
		readonly static string[] tags = new string[] {
			"ItemPath", "ItemDir", "ItemFilename", "ItemExt", "ItemNameNoExt",
			"CurLine", "CurCol", "CurText",
			"TargetPath", "TargetDir", "TargetName", "TargetExt",
			"CurrentProjectName",
			"ProjectDir", "ProjectFilename",
			"CombineDir", "CombineFilename",
			"SolutionDir", "SolutionFilename",
			"Startuppath", "ConfigDirectory",
			"TaskService.Warnings", "TaskService.Errors", "TaskService.Messages"
		};
		
		public string[] Tags {
			get {
				return tags;
			}
		}
		
		string GetCurrentItemPath()
		{
			return WorkbenchSingleton.Workbench.ActiveViewContent.PrimaryFileName;
		}
		
		string GetCurrentTargetPath()
		{
			if (ProjectService.CurrentProject != null) {
				return ProjectService.CurrentProject.OutputAssemblyFullPath;
			}
			/*if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				string fileName = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName;
				return ProjectService.GetOutputAssemblyName(fileName);
			}*/
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
				case "CurrentProjectName":
					if (ProjectService.CurrentProject == null)
						return "<no current project>";
					else
						return ProjectService.CurrentProject.Name;
					
			}
			switch (tag.ToUpperInvariant()) {
				case "ITEMPATH":
					try {
						return GetCurrentItemPath() ?? string.Empty;
					} catch (Exception) {}
					break;
				case "ITEMDIR":
					try {
						return Path.GetDirectoryName(GetCurrentItemPath()) ?? string.Empty;
					} catch (Exception) {}
					break;
				case "ITEMFILENAME":
					try {
						return Path.GetFileName(GetCurrentItemPath()) ?? string.Empty;
					} catch (Exception) {}
					break;
				case "ITEMEXT":
					try {
						return Path.GetExtension(GetCurrentItemPath()) ?? string.Empty;
					} catch (Exception) {}
					break;
				case "ITEMNAMENOEXT":
					try {
						return Path.GetFileNameWithoutExtension(GetCurrentItemPath()) ?? string.Empty;
					} catch (Exception) {}
					break;
					
				case "CURLINE":
					{
						IPositionable positionable = WorkbenchSingleton.Workbench.ActiveViewContent as IPositionable;
						if (positionable != null)
							return (positionable.Line + 1).ToString();
						break;
					}
				case "CURCOL":
					{
						IPositionable positionable = WorkbenchSingleton.Workbench.ActiveViewContent as IPositionable;
						if (positionable != null)
							return (positionable.Column + 1).ToString();
						break;
					}
				case "CURTEXT":
					{
						var tecp = WorkbenchSingleton.Workbench.ActiveViewContent as DefaultEditor.Gui.Editor.ITextEditorControlProvider;
						if (tecp != null) {
							return tecp.TextEditorControl.ActiveTextAreaControl.SelectionManager.SelectedText;
						}
						break;
					}
				case "TARGETPATH":
					try {
						return GetCurrentTargetPath() ?? string.Empty;
					} catch (Exception) {}
					break;
				case "TARGETDIR":
					try {
						return Path.GetDirectoryName(GetCurrentTargetPath()) ?? string.Empty;
					} catch (Exception) {}
					break;
				case "TARGETNAME":
					try {
						return Path.GetFileName(GetCurrentTargetPath()) ?? string.Empty;
					} catch (Exception) {}
					break;
				case "TARGETEXT":
					try {
						return Path.GetExtension(GetCurrentTargetPath()) ?? string.Empty;
					} catch (Exception) {}
					break;
					
				case "PROJECTDIR":
					if (ProjectService.CurrentProject != null) {
						return ProjectService.CurrentProject.Directory;
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
				case "SOLUTIONDIR":
					return Path.GetDirectoryName(ProjectService.OpenSolution.FileName);
				case "SOLUTIONFILENAME":
				case "COMBINEFILENAME":
					try {
						return Path.GetFileName(ProjectService.OpenSolution.FileName);
					} catch (Exception) {}
					break;
				case "STARTUPPATH":
					return Application.StartupPath;
				case "CONFIGDIRECTORY":
					return PropertyService.ConfigDirectory;
			}
			return String.Empty;
		}
	}

}
