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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Converter;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for BuildOutput.xaml
	/// </summary>
	public partial class BuildOutput : UserControl, INotifyPropertyChanged, ProjectOptionPanel.ILoadSaveCallback
	{
		private ProjectOptionPanel projectOptions;
		private System.Windows.Input.ICommand updateProjectCommand;
		private System.Windows.Input.ICommand changeOutputPath;
		
		public BuildOutput()
		{
			InitializeComponent();
			this.UpdateProjectCommand  = new RelayCommand(UpdateProjectExecute);
			this.ChangeOutputPath = new RelayCommand(ChangeOutputPathExecute);
			DataContext = this;
		}
		
		
		#region Properties
		
		public ProjectOptionPanel.ProjectProperty<string> OutputPath {
			get {return projectOptions.GetProperty("OutputPath", "", TextBoxEditMode.EditRawProperty); }	
		}
		
		
		public ProjectOptionPanel.ProjectProperty<string> DocumentationFile {
			get {return projectOptions.GetProperty("DocumentationFile", "", TextBoxEditMode.EditRawProperty);}
		}
		
		
		public ProjectOptionPanel.ProjectProperty<DebugSymbolType> DebugType {
			get {return projectOptions.GetProperty("DebugType",ICSharpCode.SharpDevelop.Project.DebugSymbolType.Full ); }
		}
			
		#endregion
		
		#region IProjectUserControl
		
		
		public void Initialize(ProjectOptionPanel projectOptions)
		{
			if (projectOptions == null) {
				throw new ArgumentNullException("projectOptions");
			}
			this.projectOptions = projectOptions;
			projectOptions.RegisterLoadSaveCallback(this);
		}
		
		public void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			UpdateTargetFrameworkCombo();
			if (DocumentationFile.Value.Length > 0) {
				documentFileIsChecked = true;
			}
		}
		
		public bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			return true;
		}
			
		
		#endregion
		
		
		#region Documentation File
		
		private bool documentFileIsChecked;
		
		public bool DocumentFileIsChecked {
			get { return documentFileIsChecked; }
			set { documentFileIsChecked = value;
				XmlDocHelper();
				RaisePropertyChanged("DocumentFileIsChecked");
			}
		}
		
		private void XmlDocHelper()
		{
			if (DocumentFileIsChecked) {
				this.DocumentationFile.Value = MSBuildInternals.Escape(
					Path.ChangeExtension(ICSharpCode.Core.FileUtility.GetRelativePath(projectOptions.Project.Directory,projectOptions.
					                                                                  Project.OutputAssemblyFullPath),
					                     ".xml"));
			} else {
				this.DocumentationFile.Value = string.Empty;
			}
		}
		
		#endregion
		
		
		#region Command Update Project
		
		public System.Windows.Input.ICommand UpdateProjectCommand {
			get { return updateProjectCommand; }
			set { updateProjectCommand = value;
				RaisePropertyChanged("UpdateProjectCommand");
			}
		}
		
		
		private void UpdateProjectExecute ()
		{
			UpgradeViewContent.Show(projectOptions.Project.ParentSolution).Select(projectOptions.Project as IUpgradableProject);
			this.UpdateTargetFrameworkCombo();
		}
		
		
		private void UpdateTargetFrameworkCombo()
		{
			TargetFramework fx = ((IUpgradableProject)projectOptions.Project).CurrentTargetFramework;
			if (fx != null) {
				targetFrameworkComboBox.Items.Clear();
				targetFrameworkComboBox.Items.Add(fx.DisplayName);
				targetFrameworkComboBox.SelectedIndex = 0;
			}
		}
		
		#endregion
		
		#region ChangeOutputPathCommand
		
		public System.Windows.Input.ICommand ChangeOutputPath
		{
			get {return this.changeOutputPath;}
			set {this.changeOutputPath = value;
				RaisePropertyChanged("ChangeOutputPath");
			}
		}
		private void ChangeOutputPathExecute()
		{
			projectOptions.BrowseForFolder(OutputPath, "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}");
		}
		
		#endregion
		
		
		#region INotifyPropertyChanged
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		private void RaisePropertyChanged (string propertyName)
		{
			var handler = this.PropertyChanged;
			if (handler != null)
			{
				handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		
		#endregion
	}
}
