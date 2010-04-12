// SharpDevelop samples
// Copyright (c) 2010, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpSnippetCompiler.Core;

namespace ICSharpCode.SharpSnippetCompiler
{
	public sealed class Program
	{		
		MainForm mainForm;
		
		[STAThread]
		static void Main(string[] args)
		{
			Program program = new Program();
			program.Run(args);
		}
		
		void Run(string[] args)
		{						
			SharpSnippetCompilerManager.Init();
		
			mainForm = new MainForm();
			
			// Force creation of the debugger before workbench is created.
			IDebugger debugger = DebuggerService.CurrentDebugger;
			
			Workbench workbench = new Workbench(mainForm);
			WorkbenchSingleton.InitializeWorkbench(workbench, new WorkbenchLayout());
			PadDescriptor errorList = workbench.GetPad(typeof(ErrorListPad));
			errorList.CreatePad();
			mainForm.ErrorList = errorList.PadContent.Control;			
			
			PadDescriptor outputList = workbench.GetPad(typeof(CompilerMessageView));
			outputList.CreatePad();
			mainForm.OutputList = outputList.PadContent.Control;
			
			mainForm.Visible = true;
			
			SnippetCompilerProject.Load();
			IProject project = GetCurrentProject();
			ProjectService.CurrentProject = project;
			LoadFiles(project);
			
			ParserService.StartParserThread();
			
			//WorkbenchSingleton.SafeThreadAsyncCall(new Action<Program>(LoadFile));
							
			try {
				Application.Run(mainForm);
			} finally {
				try {
					// Save properties
					//PropertyService.Save();
				} catch (Exception ex) {
					MessageService.ShowError(ex, "Properties could not be saved.");
				}
			}
		}

		void LoadFiles(IProject project)
		{
			foreach (ProjectItem item in project.Items) {
				FileProjectItem fileItem = item as FileProjectItem;
				if (fileItem != null && File.Exists(item.FileName)) {
					mainForm.LoadFile(item.FileName);
				}
			}
		}
				
		IProject GetCurrentProject()
		{
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				return project;
			}
			return null;
		}
	}
}
