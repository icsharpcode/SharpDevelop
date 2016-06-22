// 
// TypeScriptService.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2013 Matthew Ward
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.ComponentModel;
using System.Linq;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.TypeScriptBinding.Hosting;

namespace ICSharpCode.TypeScriptBinding
{
	public static class TypeScriptService
	{
		static readonly TypeScriptParserService parserService = new TypeScriptParserService();
		static readonly TypeScriptContextProvider contextProvider = new TypeScriptContextProvider();
		static readonly TypeScriptTaskService taskService = new TypeScriptTaskService();
		static TypeScriptWorkbenchMonitor workbenchMonitor;
		static TypeScriptProjectMonitor projectMonitor;
		
		public static TypeScriptContextProvider ContextProvider {
			get { return contextProvider; }
		}
		
		public static TypeScriptTaskService TaskService {
			get { return taskService; }
		}
		
		public static void Initialize()
		{
			WorkbenchSingleton.WorkbenchCreated += WorkbenchCreated;
		}
		
		static void WorkbenchCreated(object sender, EventArgs e)
		{
			IWorkbench workbench = SD.Workbench;
			workbench.MainWindow.Closing += MainWindowClosing;
			workbenchMonitor = new TypeScriptWorkbenchMonitor(workbench, contextProvider);
			projectMonitor = new TypeScriptProjectMonitor(contextProvider);
			parserService.Start();
		}
		
		static void MainWindowClosing(object sender, CancelEventArgs e)
		{
			parserService.Stop();
		}
		
		public static TypeScriptProject GetProjectForFile(FileName fileName)
		{
			if (ProjectService.OpenSolution == null)
				return null;
			
			return ProjectService
				.OpenSolution
				.Projects
				.Where(project => project.IsFileInProject(fileName))
				.Where(project => project is MSBuildBasedProject)
				.Select(project => new TypeScriptProject(project))
				.FirstOrDefault();
		}
		
		public static string GetFileContents(FileName fileName)
		{
			ITextSource fileContent = SD.FileService.GetFileContent(fileName);
			return fileContent.Text;
		}
	}
}
