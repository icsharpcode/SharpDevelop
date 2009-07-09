// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class ProjectReferencePanel : ListView, IReferencePanel
	{
		ISelectReferenceDialog selectDialog;
		
		public ProjectReferencePanel(ISelectReferenceDialog selectDialog)
		{
			this.selectDialog = selectDialog;
			
			ColumnHeader nameHeader = new ColumnHeader();
			nameHeader.Text  = ResourceService.GetString("Dialog.SelectReferenceDialog.ProjectReferencePanel.NameHeader");
			nameHeader.Width = 170;
			Columns.Add(nameHeader);
			
			ColumnHeader directoryHeader = new ColumnHeader();
			directoryHeader.Text  = ResourceService.GetString("Dialog.SelectReferenceDialog.ProjectReferencePanel.DirectoryHeader");
			directoryHeader.Width = 290;
			Columns.Add(directoryHeader);
			
			View = View.Details;
			Dock = DockStyle.Fill;
			FullRowSelect = true;
			
			ItemActivate += delegate { AddReference(); };
			PopulateListView();
		}
		
		public void AddReference()
		{
			foreach (ListViewItem item in SelectedItems) {
				IProject project = (IProject)item.Tag;
				ILanguageBinding binding = LanguageBindingService.GetBindingPerLanguageName(project.Language);
				
				selectDialog.AddReference(
					project.Name, "Project", project.OutputAssemblyFullPath,
					new ProjectReferenceProjectItem(selectDialog.ConfigureProject, project)
				);
			}
		}
		
		void PopulateListView()
		{
			if (ProjectService.OpenSolution == null) {
				return;
			}
			
			foreach (IProject project in ProjectService.OpenSolution.Projects.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)) {
				ListViewItem newItem = new ListViewItem(new string[] { project.Name, project.Directory });
				newItem.Tag = project;
				Items.Add(newItem);
			}
		}
	}
}
