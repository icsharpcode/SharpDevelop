// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 434 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;

using VBNetBinding;

namespace VBNetBinding.OptionPanels
{
	public class ProjectImports : AbstractProjectOptionPanel
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
			foreach(ProjectItem item in project.Items)
			{
				if(item.ItemType == ItemType.Import) {
					Get<ListBox>("imports").Items.Add(item.Include);
				}
			}
		
			DefaultProjectContent projectContent = (DefaultProjectContent)ParserService.GetProjectContent(project);
			foreach(DefaultProjectContent refProjectContent in projectContent.ReferencedContents)
			{
				addNamespaces(refProjectContent);
				
			}
			addNamespaces(projectContent);
			
			namespacesComboBox_TextCanged(null, EventArgs.Empty);
			importsListBox_SelectedIndexChanged(null, EventArgs.Empty);
		}
		
		private void addNamespaces(DefaultProjectContent projectContent)
		{
			Dictionary<string, DefaultProjectContent.NamespaceStruct>.KeyCollection namespaces = projectContent.NamespaceNames;
			foreach(string projectNamespace in namespaces)
			{
				if(projectNamespace != "") {
					Get<ComboBox>("namespaces").Items.Add(projectNamespace);
				}
			}
		}
		
		private void namespacesComboBox_TextCanged(object sender, EventArgs e)
		{
			Get<Button>("addImport").Enabled = Get<ComboBox>("namespaces").Text != "" &&
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
				project.Items.Remove(item);
			}
			
			foreach(string importedNamespace in Get<ListBox>("imports").Items)
			{
				ImportProjectItem importItem = new ImportProjectItem(project);
				importItem.Include = importedNamespace;
				project.Items.Add(importItem);
				
			}
			return base.StorePanelContents();
		}
	}
}
