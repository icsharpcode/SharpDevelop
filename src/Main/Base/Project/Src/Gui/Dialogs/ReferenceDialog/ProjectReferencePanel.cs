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
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class ProjectReferencePanel : UserControl, IReferencePanel
	{
		ISelectReferenceDialog selectDialog;
		TextBox filterTextBox;
		ListView listView;
		
		public ProjectReferencePanel(ISelectReferenceDialog selectDialog)
		{
			this.selectDialog = selectDialog;
			listView = new ListView();
			
			ColumnHeader nameHeader = new ColumnHeader();
			nameHeader.Text = ResourceService.GetString("Dialog.SelectReferenceDialog.ProjectReferencePanel.NameHeader");
			nameHeader.Width = 170;
			listView.Columns.Add(nameHeader);
			
			ColumnHeader directoryHeader = new ColumnHeader();
			directoryHeader.Text = ResourceService.GetString("Dialog.SelectReferenceDialog.ProjectReferencePanel.DirectoryHeader");
			directoryHeader.Width = 290;
			listView.Columns.Add(directoryHeader);

			listView.View = View.Details;
			listView.Dock = DockStyle.Fill;
			listView.FullRowSelect = true;

			this.Dock = DockStyle.Fill;
			this.Controls.Add(listView);

			listView.ItemActivate += delegate {
				AddReference();
			};
			PopulateListView();
			
			
			Panel upperPanel = new Panel { Dock = DockStyle.Top, Height = 20 };
			filterTextBox = new TextBox { Width = 150, Dock = DockStyle.Right };
			filterTextBox.TextChanged += delegate {
				Search();
			};
				
			upperPanel.Controls.Add(filterTextBox);
			
			this.Controls.Add(upperPanel);
		}
		
		public void AddReference()
		{
			foreach (ListViewItem item in listView.SelectedItems) {
				IProject project = (IProject)item.Tag;
				
				selectDialog.AddReference(
					project.Name, "Project", project.OutputAssemblyFullPath,
					new ProjectReferenceProjectItem(selectDialog.ConfigureProject, project)
				);
			}
			filterTextBox.Text = "";
		}
		
		void PopulateListView()
		{
			if (ProjectService.OpenSolution == null) {
				return;
			}
			
			foreach (IProject project in ProjectService.OpenSolution.Projects.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)) {
				ListViewItem newItem = new ListViewItem(new string[] { project.Name, project.Directory });
				newItem.Tag = project;
				listView.Items.Add(newItem);
			}
		}
		
		static bool ContainsAnyOfTokens(string bigText, string[] tokens)
		{
			if (tokens.Length == 0)
				return true;
			foreach (var token in tokens) {
				if (bigText.IndexOf(token, StringComparison.OrdinalIgnoreCase) < 0)
					return false;
			}
			return true;
		}
		
		void Search()
		{
			listView.Items.Clear();
			var tokens = filterTextBox.Text.Split(new []{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
			
			foreach (IProject project in ProjectService.OpenSolution.Projects.
			         Where(pr=>ContainsAnyOfTokens(pr.Name, tokens))
			         .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)) {
				ListViewItem newItem = new ListViewItem(new string[] { project.Name, project.Directory });
				newItem.Tag = project;
				listView.Items.Add(newItem);
			}
		}
	}
}
