// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Commands
{
	/// <summary>
	/// Provides tag to string mapping for SharpDevelop. Tags are mapped to strings by several methods
	/// such as registry and resource files.
	/// </summary>
	public class SharpDevelopStringTagProvider : IStringTagProvider
	{
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
		
		public string ProvideString(string tag, StringTagPair[] customTags)
		{
			return ProvideString(tag);
		}
		
		public string ProvideString(string tag)
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
					return string.Empty;
				case "ITEMDIR":
					try {
						return Path.GetDirectoryName(GetCurrentItemPath()) ?? string.Empty;
					} catch (Exception) {}
					return string.Empty;
				case "ITEMFILENAME":
					try {
						return Path.GetFileName(GetCurrentItemPath()) ?? string.Empty;
					} catch (Exception) {}
					return string.Empty;
				case "ITEMEXT":
					try {
						return Path.GetExtension(GetCurrentItemPath()) ?? string.Empty;
					} catch (Exception) {}
					return string.Empty;
				case "ITEMNAMENOEXT":
					try {
						return Path.GetFileNameWithoutExtension(GetCurrentItemPath()) ?? string.Empty;
					} catch (Exception) {}
					return string.Empty;
					
				case "CURLINE":
					{
						IPositionable positionable = WorkbenchSingleton.Workbench.ActiveViewContent as IPositionable;
						if (positionable != null)
							return positionable.Line.ToString();
						return string.Empty;
					}
				case "CURCOL":
					{
						IPositionable positionable = WorkbenchSingleton.Workbench.ActiveViewContent as IPositionable;
						if (positionable != null)
							return positionable.Column.ToString();
						return string.Empty;
					}
				case "CURTEXT":
					{
						var tecp = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
						if (tecp != null) {
							return tecp.TextEditor.SelectedText;
						}
						return string.Empty;
					}
				case "TARGETPATH":
					try {
						return GetCurrentTargetPath() ?? string.Empty;
					} catch (Exception) {}
					return string.Empty;
				case "TARGETDIR":
					try {
						return Path.GetDirectoryName(GetCurrentTargetPath()) ?? string.Empty;
					} catch (Exception) {}
					return string.Empty;
				case "TARGETNAME":
					try {
						return Path.GetFileName(GetCurrentTargetPath()) ?? string.Empty;
					} catch (Exception) {}
					return string.Empty;
				case "TARGETEXT":
					try {
						return Path.GetExtension(GetCurrentTargetPath()) ?? string.Empty;
					} catch (Exception) {}
					return string.Empty;
				case "PROJECTDIR":
					if (ProjectService.CurrentProject != null) {
						return ProjectService.CurrentProject.Directory;
					}
					return string.Empty;
				case "PROJECTFILENAME":
					if (ProjectService.CurrentProject != null) {
						try {
							return Path.GetFileName(ProjectService.CurrentProject.FileName);
						} catch (Exception) {}
					}
					return string.Empty;
				case "COMBINEDIR":
				case "SOLUTIONDIR":
					return Path.GetDirectoryName(ProjectService.OpenSolution.FileName);
				case "SOLUTIONFILENAME":
				case "COMBINEFILENAME":
					try {
						return Path.GetFileName(ProjectService.OpenSolution.FileName);
					} catch (Exception) {}
					return string.Empty;
				case "SHARPDEVELOPBINPATH":
					return Path.GetDirectoryName(typeof(SharpDevelopStringTagProvider).Assembly.Location);
				case "STARTUPPATH":
					return Application.StartupPath;
				default:
					return null;
			}
		}
	}

}
