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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;
using ICSharpCode.SharpDevelop.Project.Dialogs;
using ICSharpCode.SharpDevelop.Templates;

namespace ICSharpCode.SharpDevelop
{
	class UIService : IUIService
	{
		public void ShowSolutionConfigurationEditorDialog(ISolution solution)
		{
			using (SolutionConfigurationEditor sce = new SolutionConfigurationEditor(solution)) {
				sce.ShowDialog(SD.WinForms.MainWin32Window);
				if (solution.IsDirty)
					solution.Save();
				foreach (IProject project in solution.Projects) {
					project.Save();
				}
			}
		}
		
		public FileTemplateResult ShowNewFileDialog(IProject project, DirectoryName directory, IEnumerable<TemplateCategory> templates)
		{
			#if DEBUG
			SD.Templates.UpdateTemplates();
			#endif
			using (NewFileDialog nfd = new NewFileDialog(project, directory, templates ?? SD.Templates.TemplateCategories)) {
				if (nfd.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK)
					return nfd.result;
				else
					return null;
			}
		}
		
		public ProjectTemplateResult ShowNewProjectDialog(ISolutionFolder solutionFolder, IEnumerable<TemplateCategory> templates)
		{
			#if DEBUG
			SD.Templates.UpdateTemplates();
			#endif
			using (NewProjectDialog npdlg = new NewProjectDialog(templates ?? SD.Templates.TemplateCategories, createNewSolution: solutionFolder == null)) {
				npdlg.SolutionFolder = solutionFolder;
				if (solutionFolder != null) {
					npdlg.InitialProjectLocationDirectory = AddNewProjectToSolution.GetInitialDirectorySuggestion(solutionFolder);
				}
				
				// show the dialog to request project type and name
				if (npdlg.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK) {
					return npdlg.result;
				} else {
					return null;
				}
			}
		}
	}
}
