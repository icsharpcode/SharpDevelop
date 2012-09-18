// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.VBNetBinding.OptionPanels
{
	/// <summary>
	/// Interaction logic for ProjectImportsOptions.xaml
	/// </summary>
	public partial class ProjectImports : ProjectOptionPanel	
	{
		
		public ProjectImports()
		{
			InitializeComponent();
		}
		
		#region override
		
		protected override void Initialize()
		{
			ProjectItems = new ObservableCollection<string>();
			NameSpaceItems = new ObservableCollection<string> ();
				
			foreach(ProjectItem item in base.Project.Items)
			{
				if(item.ItemType == ItemType.Import) {
					ProjectItems.Add(item.Include);
				}
			}
			
			
			IProjectContent projectContent = ParserService.GetProjectContent(base.Project);
			foreach(IProjectContent refProjectContent in projectContent.ThreadSafeGetReferencedContents()) {
				AddNamespaces(refProjectContent);
			}
			AddNamespaces(projectContent);
		}
			
		
		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
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
			
			foreach(string importedNamespace in ProjectItems)
			{
				ProjectService.AddProjectItem(project, new ImportProjectItem(project, importedNamespace));
			}
			
			return base.Save(project, configuration, platform);
		}
		
		#endregion
		
		
		private ObservableCollection<string> projectItems;
		
		public ObservableCollection<string> ProjectItems {
			get { return projectItems; }
			set { projectItems = value;
				base.RaisePropertyChanged(() => ProjectItems);
			}
		}
		
		private string selectedProjectItem;
		
		public string SelectedProjectItem {
			get { return selectedProjectItem; }
			set { selectedProjectItem = value;
				base.RaisePropertyChanged(() => SelectedProjectItem);
				RemoveButtonEnable = true;
				AddButtonEnable = false;
			}
		}
		
		private ObservableCollection <string> nameSpaceItems;
		
		public ObservableCollection<string> NameSpaceItems {
			get { return nameSpaceItems; }
			set { nameSpaceItems = value;
				base.RaisePropertyChanged(() => NameSpaceItems);
			}
		}
		
		
		private string selectedNameSpace;
		
		public string SelectedNameSpace {
			get { return selectedNameSpace; }
			set { selectedNameSpace = value;
				base.RaisePropertyChanged(()=>SelectedNameSpace);
				AddButtonEnable = true;
			}
		}
		
		
		private bool addButtonEnable;
		
		public bool AddButtonEnable {
			get { return addButtonEnable; }
			set { addButtonEnable = value;
				base.RaisePropertyChanged(() => AddButtonEnable);
			}
		}
		
		private bool removeButtonEnable;
		
		public bool RemoveButtonEnable {
			get { return removeButtonEnable; }
			set { removeButtonEnable = value;
				base.RaisePropertyChanged(() => RemoveButtonEnable);
			}
		}
		
	
		
		private void AddNamespaces(IProjectContent projectContent)
		{
			foreach(string projectNamespace in projectContent.NamespaceNames) {
				if (!string.IsNullOrEmpty(projectNamespace)) {
					
					if (!NameSpaceItems.Contains(projectNamespace)) {
						NameSpaceItems.Add(projectNamespace);
					}
				}
			}
		}
		
		void AddButton_Click(object sender, RoutedEventArgs e)
		{
			ProjectItems.Add(SelectedNameSpace);
			IsDirty = true;
		}
		
		void RemoveButton_Click(object sender, RoutedEventArgs e)
		{
			ProjectItems.Remove(SelectedProjectItem);
			SelectedProjectItem = null;
			RemoveButtonEnable = false;
			AddButtonEnable = false;
			IsDirty = true;
		}
	}
}
