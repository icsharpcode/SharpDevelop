// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Project;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class ProjectReferencePanel : ListView, IReferencePanel
	{
		SelectReferenceDialog selectDialog;
		
		public ProjectReferencePanel(SelectReferenceDialog selectDialog)
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
			
			ItemActivate += new EventHandler(AddReference);
			PopulateListView();
		}
		
		public void AddReference(object sender, EventArgs e)
		{
			foreach (ListViewItem item in SelectedItems) {
				IProject project = (IProject)item.Tag;
				ILanguageBinding binding = LanguageBindingService.GetBindingPerLanguageName(project.Language);
				
				selectDialog.AddReference(ReferenceType.Project,
				                          project.Name,
				                          project.OutputAssemblyFullPath,
				                          project);
			}
		}
		
		void PopulateListView()
		{
			if (ProjectService.OpenSolution == null) {
				return;
			}
			
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				ListViewItem newItem = new ListViewItem(new string[] { project.Name, project.Directory });
				newItem.Tag = project;
				Items.Add(newItem);
			}
		}
	}
}
