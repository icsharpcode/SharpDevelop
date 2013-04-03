// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Templates;
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
		
		public FileTemplateResult ShowNewFileDialog(IProject project, DirectoryName directory, IEnumerable<ICSharpCode.SharpDevelop.Templates.FileTemplate> templates)
		{
			using (NewFileDialog nfd = new NewFileDialog(project, directory)) {
				if (nfd.ShowDialog(SD.WinForms.MainWin32Window) != DialogResult.OK)
					return null;
				if (project != null) {
					foreach (KeyValuePair<string, FileDescriptionTemplate> createdFile in nfd.CreatedFiles) {
						FileName fileName = FileName.Create(createdFile.Key);
						ItemType type = project.GetDefaultItemType(fileName);
						FileProjectItem newItem = new FileProjectItem(project, type);
						newItem.FileName = fileName;
						createdFile.Value.SetProjectItemProperties(newItem);
						project.Items.Add(newItem);
					}
					project.Save();
				}
				var result = new FileTemplateResult(nfd.options);
				result.NewFiles.AddRange(nfd.CreatedFiles.Select(p => FileName.Create(p.Key)));
				return result;
			}
		}
		
		public ProjectTemplateResult ShowNewProjectDialog(ISolutionFolder solutionFolder, IEnumerable<ICSharpCode.SharpDevelop.Templates.ProjectTemplate> templates)
		{
			using (NewProjectDialog npdlg = new NewProjectDialog(createNewSolution: solutionFolder == null)) {
				npdlg.SolutionFolder = solutionFolder;
				if (solutionFolder != null) {
					npdlg.InitialProjectLocationDirectory = AddNewProjectToSolution.GetInitialDirectorySuggestion(solutionFolder);
				}
				
				// show the dialog to request project type and name
				if (npdlg.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK) {
					return new ProjectTemplateResult();
				} else {
					return null;
				}
			}
		}
	}
}
