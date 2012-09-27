/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 24.09.2012
 * Time: 19:54
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for BuildAdvenced.xaml
	/// </summary>
	public partial class BuildAdvanced : UserControl,IProjectUserControl, INotifyPropertyChanged
	{
		private List<KeyItemPair> serializationInfo;
		private List<KeyItemPair> targetCPU;
		private string dllBaseAdress;
		private List<KeyItemPair> fileAlignment;
		private System.Windows.Input.ICommand baseIntermediateOutputPathCommand;
		private System.Windows.Input.ICommand intermediateOutputPathCommand;
		private ProjectOptionPanel projectOptions;
		
		public BuildAdvanced()
		{
			InitializeComponent();
			this.DataContext = this;
			this.serializationInfo = new List<KeyItemPair>();
			
			this.serializationInfo.Add (new KeyItemPair("Off",StringParser.Parse("${res:Dialog.ProjectOptions.Build.Off}")));
			this.serializationInfo.Add (new KeyItemPair("On",StringParser.Parse("${res:Dialog.ProjectOptions.Build.On}")));
			this.serializationInfo.Add (new KeyItemPair("Auto",StringParser.Parse( "${res:Dialog.ProjectOptions.Build.Auto}")));
			this.SerializationInfo = this.serializationInfo;
			
			this.targetCPU = new List<KeyItemPair>();
			this.targetCPU.Add(new KeyItemPair( "AnyCPU",StringParser.Parse("${res:Dialog.ProjectOptions.Build.TargetCPU.Any}")));
			this.targetCPU.Add(new KeyItemPair( "x86",StringParser.Parse("${res:Dialog.ProjectOptions.Build.TargetCPU.x86}")));
			this.targetCPU.Add(new KeyItemPair( "x64",StringParser.Parse("${res:Dialog.ProjectOptions.Build.TargetCPU.x64}")));
			this.targetCPU.Add(new KeyItemPair( "Itanium",StringParser.Parse("${res:Dialog.ProjectOptions.Build.TargetCPU.Itanium}")));
			this.TargetCPU = targetCPU;
			
			fileAlignment = new List<KeyItemPair>();
			fileAlignment.Add( new KeyItemPair("512", "512"));
			fileAlignment.Add( new KeyItemPair("1024", "1024"));
			fileAlignment.Add(new KeyItemPair("2048", "2048"));
			fileAlignment.Add(new KeyItemPair("4096", "4096"));
			fileAlignment.Add(new KeyItemPair("8192", "8192"));
			FileAlign = fileAlignment;
		}
		
		#region IProjectUserControl
		
		
		public void SetProjectOptions (ProjectOptionPanel projectOptions)
		{
			if (projectOptions == null) {
				throw new ArgumentNullException("projectOptions");
			}
			this.projectOptions = projectOptions;
			
			this.BaseIntermediateOutputPathCommand = new RelayCommand(BaseIntermediateOutputPathExecute);
			this.IntermediateOutputPathCommand = new RelayCommand(IntermediateOutputPathExecute);
			
			int val;
			if (!int.TryParse(BaseAddress.Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out val)) {
				val = 0x400000;
			}
			DllBaseAdress =  "0x" + val.ToString("x", NumberFormatInfo.InvariantInfo);
		}
		
		
		public bool SaveProjectOptions()
		{
			NumberStyles style = NumberStyles.Integer;
			if (dllBaseAdress.StartsWith("0x")) {
				dllBaseAdress = dllBaseAdress.Substring(2);
				style = NumberStyles.HexNumber;
			}
			
			int val;
			if (int.TryParse(dllBaseAdress, style, NumberFormatInfo.InvariantInfo, out val)) {
				BaseAddress.Value = val.ToString(NumberFormatInfo.InvariantInfo);
				return true;
			} else {
				MessageService.ShowMessage("${res:Dialog.ProjectOptions.PleaseEnterValidNumber}");
				return false;
			}
		}
		
		#endregion
		
		
		#region Properies
		
		public ProjectOptionPanel.ProjectProperty<bool> RegisterForComInterop {
			get {return projectOptions.GetProperty("RegisterForComInterop", false, PropertyStorageLocations.PlatformSpecific ); }
		}
		
		
		public ProjectOptionPanel.ProjectProperty<string> GenerateSerializationAssemblies {
			get {return projectOptions.GetProperty("GenerateSerializationAssemblies","Auto",
			                        TextBoxEditMode.EditEvaluatedProperty, PropertyStorageLocations.PlatformSpecific ); }
		}
		
		public ProjectOptionPanel.ProjectProperty<string> PlatformTarget {
			get {return projectOptions.GetProperty("PlatformTarget","AnyCPU",
			                        TextBoxEditMode.EditEvaluatedProperty, PropertyStorageLocations.PlatformSpecific ); }
		}
		
		public ProjectOptionPanel.ProjectProperty<string> FileAlignment {
			get {return projectOptions.GetProperty("FileAlignment","4096",
			                        TextBoxEditMode.EditEvaluatedProperty, PropertyStorageLocations.PlatformSpecific ); }
		}
		
		public ProjectOptionPanel.ProjectProperty<string> BaseAddress {
			get {return projectOptions.GetProperty("BaseAddress","1000",
			                        TextBoxEditMode.EditEvaluatedProperty,PropertyStorageLocations.PlatformSpecific ); }
		}
		
		
		public ProjectOptionPanel.ProjectProperty<string> BaseIntermediateOutputPath {
			get {return projectOptions.GetProperty("BaseIntermediateOutputPath",@"obj\",
			                        TextBoxEditMode.EditRawProperty,PropertyStorageLocations.ConfigurationSpecific ); }
		}
		
		
		public ProjectOptionPanel.ProjectProperty<string> IntermediateOutputPath {
			get {return projectOptions.GetProperty("IntermediateOutputPath",@"obj\$(Configuration)\",TextBoxEditMode.EditRawProperty ); }
		}
		
		
		#endregion
		
		public List<KeyItemPair> SerializationInfo {
			get {return this.serializationInfo;}
			set {this.serializationInfo = value;
				RaisePropertyChanged("SerializationInfo");
			}
		}
		
		
		public List<KeyItemPair> TargetCPU {
			get { return targetCPU; }
			set { targetCPU = value;
				RaisePropertyChanged("TargetCPU");
			}
		}
		
		
		public List<KeyItemPair> FileAlign {
			get { return fileAlignment; }
			set { fileAlignment = value;
				RaisePropertyChanged("FileAlign");
			}
		}
		
		
		public string DllBaseAdress {
			get {
				return dllBaseAdress;
			}
			
			set {
				dllBaseAdress = value;
				projectOptions.IsDirty = true;
				RaisePropertyChanged("DllBaseAdress");
			}
		}
		
		
		#region BaseIntermediateOutputPath
		
		public System.Windows.Input.ICommand BaseIntermediateOutputPathCommand {
			get{return this.baseIntermediateOutputPathCommand;}
			set {this.baseIntermediateOutputPathCommand = value;
				RaisePropertyChanged("BaseIntermediateOutputPathCommand");
			}
		}
		
		
		private void BaseIntermediateOutputPathExecute ()
		{
			projectOptions.BrowseForFolder(BaseIntermediateOutputPath,
			                               ResourceService.GetString("Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription"));
		}
		
		#endregion
		
		#region IntermediateOutputPath
		
		public System.Windows.Input.ICommand IntermediateOutputPathCommand {
			get{return this.intermediateOutputPathCommand;}
			set {this.intermediateOutputPathCommand = value;
				RaisePropertyChanged("IntermediateOutputPathCommand");
			}
		}
		
		
		private void IntermediateOutputPathExecute ()
		{
			projectOptions.BrowseForFolder(IntermediateOutputPath,
			                               ResourceService.GetString("Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription"));
		}
		
		#endregion
		
		#region INotifyPropertyChanged
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
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