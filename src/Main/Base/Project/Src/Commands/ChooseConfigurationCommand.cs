//// <file>
////     <copyright see="prj:///doc/copyright.txt"/>
////     <license see="prj:///doc/license.txt"/>
////     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
////     <version value="$version"/>
//// </file>
//
//using System;
//using System.Windows.Forms;
//
//using ICSharpCode.Core;
//using ICSharpCode.SharpDevelop.Gui;
//using ICSharpCode.SharpDevelop.Project;
//
//namespace ICSharpCode.SharpDevelop.Commands
//{
//	class ChooseConfigurationCommand : AbstractComboBoxCommand
//	{
//		public ChooseConfigurationCommand()
//		{
//			ProjectService.CurrentProjectChanged        += new ICSharpCode.Core.ProjectEventHandler(CurrentProjectChanged);
//			ProjectService.SolutionConfigurationChanged += new SolutionConfigurationEventArgs(ActiveConfigurationChanged);
//			ProjectService.ConfigurationAdded           += new EventHandler(ConfigurationAdded);
//			ProjectService.ConfigurationRemoved         += new EventHandler(ConfigurationRemoved);
//		}
//		
//		void ConfigurationAdded(object sender, EventArgs e)
//		{
//			Refresh();
//		}
//		
//		void ConfigurationRemoved(object sender, EventArgs e)
//		{
//			Refresh();
//		}
//		
//		void ActiveConfigurationChanged(object sender, SolutionConfigurationEventArgs e)
//		{
//			Refresh();
//		}
//		
//		void CurrentProjectChanged(object sender, ProjectEventArgs e)
//		{
//			Refresh();
//		}
//		
//		void Refresh()
//		{
//			SdMenuComboBox toolbarItem = (SdMenuComboBox)Owner;
//			ComboBox combo = toolbarItem.ComboBox;
//			
//			
//			IProject project = ProjectService.CurrentProject;
//			
//			combo.Items.Clear();
//			
//			if(project != null) {
//				foreach(IConfiguration config in project.Configurations) {
//					int index = combo.Items.Add(config.Name);
//					if(config.Equals(project.ActiveConfiguration)) {
//						combo.SelectedIndex = index;
//					}
//				}
//			}
//		}
//		
//		public override void Run()
//		{
//			SdMenuComboBox toolbarItem = (SdMenuComboBox)Owner;
//			ComboBox combo = toolbarItem.ComboBox;
//			
//			
//			IProject project = ProjectService.CurrentProject;
//			project.ActiveConfiguration = project.Configurations[combo.SelectedIndex];
//		}
//	}
//}
