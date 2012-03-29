/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 14.11.2011
 * Time: 19:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
	public partial class DebugOptions :  ProjectOptionPanel,INotifyPropertyChanged
	{
		
		public DebugOptions()
		{
			InitializeComponent();
		}
		
		public ProjectProperty<StartAction> StartAction
		{
			get
			{
				return GetProperty<StartAction>("StartAction", Project.StartAction.Project, PropertyStorageLocations.ConfigurationSpecific);
			}
		}
		
		public ProjectProperty<string> StartProgram
		{
			get
			{
				return GetProperty("StartProgram", "", TextBoxEditMode.EditRawProperty, PropertyStorageLocations.ConfigurationSpecific);
			}
		}
		
	
		public ProjectProperty<string> StartURL
		{
			get
			{
				return GetProperty("StartURL", "", TextBoxEditMode.EditRawProperty, PropertyStorageLocations.ConfigurationSpecific);
			}
		}
	
		
		public ProjectProperty<string> StartArguments
		{
			get
			{
				return GetProperty("StartArguments", "", TextBoxEditMode.EditRawProperty, PropertyStorageLocations.ConfigurationSpecific);
			}
		}
	
		
		public ProjectProperty<string> StartWorkingDirectory
		{
			get {
				return GetProperty("StartWorkingDirectory", "", TextBoxEditMode.EditRawProperty, PropertyStorageLocations.ConfigurationSpecific);
			}
		}

		void ExternalProgramButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			string fileFilter =  "${res:SharpDevelop.FileFilter.ExecutableFiles}|*.exe;*.com;*.pif;*.bat;*.cmd";
			this.StartProgram.Value = this.FileDialog(fileFilter);
		}
		
		
		void BrwoseForFolder_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			StartWorkingDirectory.Value = BrowseForFolder ("${res:Dialog.ProjectOptions.SelectFolderTitle}",TextBoxEditMode.EditRawProperty);
		}
		
		
		private string FileDialog (string filter)
		{
			string fileName = String.Empty;
			
			using (OpenFileDialog fdiag = new OpenFileDialog())
			{
					fdiag.Filter      = StringParser.Parse(filter);
					fdiag.Multiselect = false;
					try {
						string initialDir = System.IO.Path.GetDirectoryName(System.IO.Path.Combine(base.BaseDirectory,string.Empty));
						if (FileUtility.IsValidPath(initialDir) && System.IO.Directory.Exists(initialDir)) {
							fdiag.InitialDirectory = initialDir;
						}
					} 
					catch {}
					
					if(fdiag.ShowDialog() == DialogResult.OK) {
						
						fileName = fdiag.FileName;
						
						if (base.BaseDirectory != null) {
							fileName = FileUtility.GetRelativePath(base.BaseDirectory,fileName);
						}
						/*
						if (textBoxEditMode == TextBoxEditMode.EditEvaluatedProperty) {
							target.Text = file;
						} else {
							target.Text = MSBuildInternals.Escape(file);
						}
						*/
					}
//					return fileName;
			}
			return fileName;
		}
		
		private string BrowseForFolder(string description,TextBoxEditMode textBoxEditMode)
		{
			
			string startLocation = base.BaseDirectory;
				if (startLocation != null) {
					string text = StartWorkingDirectory.Value;
					if (textBoxEditMode == TextBoxEditMode.EditRawProperty)
						text = MSBuildInternals.Unescape(text);
					
				startLocation = FileUtility.GetAbsolutePath(startLocation, text);
			
				}
				
				using (FolderBrowserDialog fdiag = FileService.CreateFolderBrowserDialog(description, startLocation))
				{
					if (fdiag.ShowDialog() == DialogResult.OK) {
						string path = fdiag.SelectedPath;
						if (base.BaseDirectory != null) {
							path = FileUtility.GetRelativePath(base.BaseDirectory, path);
						}
						if (!path.EndsWith("\\") && !path.EndsWith("/"))
							path += "\\";
						
//						if (textBoxEditMode == TextBoxEditMode.EditEvaluatedProperty) {
////							panel.ControlDictionary[target].Text = path;
//						} else {
//							panel.ControlDictionary[target].Text = MSBuildInternals.Escape(path);
////						}
						return path;
					}
					
				}
			return startLocation;
		}
		
		private void OnPropertyChanged (string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged (this,new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
	}
	
}