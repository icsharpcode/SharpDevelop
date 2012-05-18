// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.VBNetBinding.OptionPanels
{
	public class ProjectImports : AbstractXmlFormsProjectOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlResource("ProjectImports.xfrm");
			InitializeHelper();
			
			Get<Button>("addImport").Click += new EventHandler(addImportButton_Click);
			Get<Button>("removeImport").Click += new EventHandler(removeImportButton_Click);
			Get<ComboBox>("namespaces").TextChanged += new EventHandler(namespacesComboBox_TextCanged);
			Get<ListBox>("imports").SelectedIndexChanged += new EventHandler(importsListBox_SelectedIndexChanged);
			
			Get<ComboBox>("namespaces").Items.Clear();
			Get<ComboBox>("namespaces").AutoCompleteSource = AutoCompleteSource.ListItems;
			Get<ComboBox>("namespaces").AutoCompleteMode = AutoCompleteMode.Suggest;
			foreach(ProjectItem item in project.Items)
			{
				if(item.ItemType == ItemType.Import) {
					Get<ListBox>("imports").Items.Add(item.Include);
				}
			}
			
			IProjectContent projectContent = ParserService.GetProjectContent(project);
			foreach(IProjectContent refProjectContent in projectContent.ThreadSafeGetReferencedContents()) {
				AddNamespaces(refProjectContent);
				
			}
			AddNamespaces(projectContent);
			
			namespacesComboBox_TextCanged(null, EventArgs.Empty);
			importsListBox_SelectedIndexChanged(null, EventArgs.Empty);
		}
		
		private void AddNamespaces(IProjectContent projectContent)
		{
			foreach(string projectNamespace in projectContent.NamespaceNames) {
				if (!string.IsNullOrEmpty(projectNamespace)) {
					if (!Get<ComboBox>("namespaces").Items.Contains(projectNamespace)) {
						Get<ComboBox>("namespaces").Items.Add(projectNamespace);
					}
				}
			}
		}
		
		private void namespacesComboBox_TextCanged(object sender, EventArgs e)
		{
			Get<Button>("addImport").Enabled = !string.IsNullOrEmpty(Get<ComboBox>("namespaces").Text) &&
				! Get<ListBox>("imports").Items.Contains(Get<ComboBox>("namespaces").Text);
		}
		
		private void importsListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			Get<Button>("removeImport").Enabled = Get<ListBox>("imports").SelectedIndex != -1;
		}
		
		private void removeImportButton_Click(object sender, EventArgs e)
		{
			Get<ListBox>("imports").Items.RemoveAt(Get<ListBox>("imports").SelectedIndex);
			IsDirty = true;
		}
		
		private void addImportButton_Click(object sender, EventArgs e)
		{
			Get<ListBox>("imports").Items.Add(Get<ComboBox>("namespaces").Text);
			Get<ComboBox>("namespaces").Text = "";
			IsDirty = true;
		}
		
		public override bool StorePanelContents()
		{
			List<ProjectItem> imports = new List<ProjectItem>();
			foreach(ProjectItem item in project.Items)
			{
				if(item.ItemType == ItemType.Import)
				{
					imports.Add(item);
				}
			}
			
			foreach(ImportProjectItem item in imports)
			{
				ProjectService.RemoveProjectItem(project, item);
			}
			
			foreach(string importedNamespace in Get<ListBox>("imports").Items)
			{
				ProjectService.AddProjectItem(project, new ImportProjectItem(project, importedNamespace));
			}
			return base.StorePanelContents();
		}
	}
}
