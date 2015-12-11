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
using System.Diagnostics;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Commands.TabStrip
{
	public class CloseFileTab : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = Owner as IWorkbenchWindow;
			if (window != null) {
				window.CloseWindow(false);
			}
		}
	}
	
	public class CloseAllButThisFileTab : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow thisWindow = Owner as IWorkbenchWindow;
			foreach (IWorkbenchWindow window in SD.Workbench.WorkbenchWindowCollection.ToArray()) {
				if (window != thisWindow) {
					if (!window.CloseWindow(false))
						break;
				}
			}
		}
	}
	
	public class SaveFileTab : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = Owner as IWorkbenchWindow;
			if (window != null) {
				SaveFile.Save(window);
			}
		}
	}
	
	public class SaveFileAsTab : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = Owner as IWorkbenchWindow;
			
			if (window != null) {
				SaveFileAs.Save(window);
			}
		}
	}
	
	/// <summary>
	/// Copies the path to the clipboard.
	/// </summary>
	public class CopyPathName : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = Owner as IWorkbenchWindow;
			SD.Clipboard.SetText(window.ActiveViewContent.PrimaryFileName ?? "");
		}
	}
	
	/// <summary>
	/// Expands and scrolls to this file in the project browser.
	/// </summary>
	public class NavigateToFileInProjectBrowser : AbstractMenuCommand
	{
		public override void Run()
		{
			var fileName = GetActiveFileName();
			var projectBrowser = GetProjectBrowser();
			if (fileName != null && projectBrowser != null) {
				projectBrowser.SelectFileAndExpand(fileName);
				projectBrowser.Focus();
			}
		}
		
		string GetActiveFileName()
		{
			var workbenchWindow = Owner as IWorkbenchWindow;
			if (workbenchWindow == null)
				workbenchWindow = SD.Workbench.ActiveWorkbenchWindow;

			if (workbenchWindow != null && workbenchWindow.ActiveViewContent != null) {
				return workbenchWindow.ActiveViewContent.PrimaryFileName;
			}

			return null;
		}
		
		ProjectBrowserControl GetProjectBrowser()
		{
			if ((ProjectBrowserPad.Instance != null) && (ProjectBrowserPad.Instance.Control is ProjectBrowserPanel)) {
				return (ProjectBrowserPad.Instance.Control as ProjectBrowserPanel).ProjectBrowserControl;
			}
			return null;
		}
	}
	
	/// <summary>
	/// Opens the folder containing this file in the windows explorer.
	/// </summary>
	public class OpenFolderContainingFile : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = Owner as IWorkbenchWindow;
			ICSharpCode.SharpDevelop.Project.Commands.OpenFolderContainingFile.OpenContainingFolderInExplorer(
				window.ActiveViewContent.PrimaryFileName);
		}
	}
	
	/// <summary>
	/// Opens a command prompt at the file's location.
	/// </summary>
	public class OpenCommandPromptHere : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = Owner as IWorkbenchWindow;
			ICSharpCode.SharpDevelop.Project.Commands.OpenCommandPromptHere.OpenCommandPrompt(
				window.ActiveViewContent.PrimaryFileName);
		}
	}
}
