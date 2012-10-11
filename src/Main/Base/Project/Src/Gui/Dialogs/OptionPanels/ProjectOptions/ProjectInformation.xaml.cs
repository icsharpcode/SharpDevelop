/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 10.10.2012
 * Time: 20:20
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for ProjectInformation.xaml
	/// </summary>
	public partial class ProjectInformation : UserControl,IProjectUserControl, INotifyPropertyChanged
	{
		private ProjectOptionPanel projectOptions;
		
		public ProjectInformation()
		{
			InitializeComponent();
			this.DataContext = this;
		}
		
		public void SetProjectOptions(ProjectOptionPanel projectOptions)
		{
			if (projectOptions == null) {
				throw new ArgumentNullException("projectOptions");
			}
			this.projectOptions = projectOptions;
		}
		
		public bool SaveProjectOptions()
		{
			throw new NotImplementedException();
		}
		
		private string projectFolder;
		
		public string ProjectFolder {
			get { return projectFolder; }
			set { projectFolder = value;
			RaisePropertyChanged("ProjectFolder");}
		}
		
		
		private string projectFile;
		
		public string ProjectFile {
			get { return projectFile; }
			set { projectFile = value;
				RaisePropertyChanged("ProjectFile");}
		}
		
		private string outputTypeName;
		
		public string OutputTypeName {
			get { return outputTypeName; }
			set { outputTypeName = value;
				RaisePropertyChanged("OutputTypeName");}
		}
		
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