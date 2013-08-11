// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for DebugOptions.xaml
	/// </summary>
	public partial class DebugOptions :  ProjectOptionPanel
	{
		
		public DebugOptions()
		{
			InitializeComponent();
		}
		
		public ProjectProperty<StartAction> StartAction
		{
			get{ return GetProperty<StartAction>("StartAction",
			                                     SharpDevelop.Project.StartAction.Project, PropertyStorageLocations.ConfigurationSpecific);}
		}
		
		public ProjectProperty<string> StartProgram
		{
			get { return GetProperty("StartProgram", "", TextBoxEditMode.EditRawProperty, PropertyStorageLocations.ConfigurationSpecific);}
		}
		
		
		public ProjectProperty<string> StartURL
		{
			get{ return GetProperty("StartURL", "", TextBoxEditMode.EditRawProperty, PropertyStorageLocations.ConfigurationSpecific);}
		}
		
		
		public ProjectProperty<string> StartArguments
		{
			get{ return GetProperty("StartArguments", "", TextBoxEditMode.EditRawProperty, PropertyStorageLocations.ConfigurationSpecific);}
		}
		
		
		public ProjectProperty<string> StartWorkingDirectory
		{
			get { return GetProperty("StartWorkingDirectory", "", TextBoxEditMode.EditRawProperty, PropertyStorageLocations.ConfigurationSpecific);}
		}

		void ExternalProgramButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			string fileFilter =  "${res:SharpDevelop.FileFilter.ExecutableFiles}|*.exe;*.com;*.pif;*.bat;*.cmd";
			BrowseForFile(this.StartProgram, fileFilter);
		}
		
		void BrowseForFolder_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			BrowseForFolder(StartWorkingDirectory, "${res:Dialog.ProjectOptions.SelectFolderTitle}");
		}
	}
}
