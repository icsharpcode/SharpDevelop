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
			return SD.Workbench.ActiveViewContent.PrimaryFileName;
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
						IPositionable positionable = SD.GetActiveViewContentService<IPositionable>();
						if (positionable != null)
							return positionable.Line.ToString();
						return string.Empty;
					}
				case "CURCOL":
					{
						IPositionable positionable = SD.GetActiveViewContentService<IPositionable>();
						if (positionable != null)
							return positionable.Column.ToString();
						return string.Empty;
					}
				case "CURTEXT":
					{
						ITextEditor editor = SD.GetActiveViewContentService<ITextEditor>();
						if (editor != null) {
							return editor.SelectedText;
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
