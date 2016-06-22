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
