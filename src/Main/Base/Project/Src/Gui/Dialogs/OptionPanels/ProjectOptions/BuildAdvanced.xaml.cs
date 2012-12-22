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
using ICSharpCode.SharpDevelop.Project.Converter;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for BuildAdvenced.xaml
	/// </summary>
	public partial class BuildAdvanced : UserControl, INotifyPropertyChanged, ProjectOptionPanel.ILoadSaveCallback
	{

		private string dllBaseAddress;
		private System.Windows.Input.ICommand baseIntermediateOutputPathCommand;
		private System.Windows.Input.ICommand intermediateOutputPathCommand;
		private ProjectOptionPanel projectOptions;
		bool supports32BitPreferred;
		
		public BuildAdvanced()
		{
			InitializeComponent();
			this.BaseIntermediateOutputPathCommand = new RelayCommand(BaseIntermediateOutputPathExecute);
			this.IntermediateOutputPathCommand = new RelayCommand(IntermediateOutputPathExecute);
			this.DataContext = this;
		}

		public void Initialize (ProjectOptionPanel projectOptions)
		{
			if (projectOptions == null) {
				throw new ArgumentNullException("projectOptions");
			}
			this.projectOptions = projectOptions;
			projectOptions.RegisterLoadSaveCallback(this);
			
			
			this.SerializationInfo = new List<KeyItemPair>();
			this.SerializationInfo.Add(new KeyItemPair("Off", StringParser.Parse("${res:Dialog.ProjectOptions.Build.Off}")));
			this.SerializationInfo.Add(new KeyItemPair("On", StringParser.Parse("${res:Dialog.ProjectOptions.Build.On}")));
			this.SerializationInfo.Add(new KeyItemPair("Auto", StringParser.Parse("${res:Dialog.ProjectOptions.Build.Auto}")));
			
			this.TargetCPU = new List<KeyItemPair>();
			supports32BitPreferred = false;
			if (DotnetDetection.IsDotnet45Installed()) {
				supports32BitPreferred = projectOptions.Project.MinimumSolutionVersion >= Solution.SolutionVersionVS2010;
				// Show 32 vs. 64 options even for library projects;
				// it's relevant for web applications.
			}
			if (supports32BitPreferred) {
				this.TargetCPU.Add(new KeyItemPair("AnyCPU32", StringParser.Parse("${res:Dialog.ProjectOptions.Build.TargetCPU.Any32}")));
				this.TargetCPU.Add(new KeyItemPair("AnyCPU64", StringParser.Parse("${res:Dialog.ProjectOptions.Build.TargetCPU.Any64}")));
			} else {
				this.TargetCPU.Add(new KeyItemPair("AnyCPU", StringParser.Parse("${res:Dialog.ProjectOptions.Build.TargetCPU.Any}")));
			}
			this.TargetCPU.Add(new KeyItemPair("x86", StringParser.Parse("${res:Dialog.ProjectOptions.Build.TargetCPU.x86}")));
			this.TargetCPU.Add(new KeyItemPair("x64", StringParser.Parse("${res:Dialog.ProjectOptions.Build.TargetCPU.x64}")));
			this.TargetCPU.Add(new KeyItemPair("Itanium", StringParser.Parse("${res:Dialog.ProjectOptions.Build.TargetCPU.Itanium}")));
			
			this.FileAlign = new List<KeyItemPair>();
			this.FileAlign.Add(new KeyItemPair("512", "512"));
			this.FileAlign.Add(new KeyItemPair("1024", "1024"));
			this.FileAlign.Add(new KeyItemPair("2048", "2048"));
			this.FileAlign.Add(new KeyItemPair("4096", "4096"));
			this.FileAlign.Add(new KeyItemPair("8192", "8192"));
		}
		
		public void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			int val;
			if (!int.TryParse(BaseAddress.Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out val)) {
				val = 0x400000;
			}
			DllBaseAddress = "0x" + val.ToString("x", NumberFormatInfo.InvariantInfo);
			
			if (supports32BitPreferred && string.Equals(this.PlatformTarget.Value, "AnyCPU", StringComparison.OrdinalIgnoreCase)) {
				bool default32BitPreferred = false;
				var upgradableProject = projectOptions.Project as IUpgradableProject;
				if (upgradableProject != null && upgradableProject.CurrentTargetFramework.IsBasedOn(TargetFramework.Net45)) {
					default32BitPreferred = true;
				}
				if (Prefer32Bit.Value ?? default32BitPreferred)
					targetCpuComboBox.SelectedValue = "AnyCPU32";
				else
					targetCpuComboBox.SelectedValue = "AnyCPU64";
			} else {
				targetCpuComboBox.SelectedValue = this.PlatformTarget.Value;
			}
		}
		
		public bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			NumberStyles style = NumberStyles.Integer;
			string dllBaseAddressWithoutHexPrefix = dllBaseAddress;
			if (dllBaseAddress.StartsWith("0x")) {
				dllBaseAddressWithoutHexPrefix = dllBaseAddress.Substring(2);
				style = NumberStyles.HexNumber;
			}
			
			int val;
			if (int.TryParse(dllBaseAddressWithoutHexPrefix, style, NumberFormatInfo.InvariantInfo, out val)) {
				BaseAddress.Value = val.ToString(NumberFormatInfo.InvariantInfo);
			} else {
				MessageService.ShowMessage("${res:Dialog.ProjectOptions.PleaseEnterValidNumber}");
				return false;
			}
			// targetCPU is saved in targetCPUCombobox_SelectionChanged
			
			return true;
		}
		
		void TargetCpuComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (string.Equals((string)targetCpuComboBox.SelectedValue, "AnyCPU32", StringComparison.OrdinalIgnoreCase)) {
				this.PlatformTarget.Value = "AnyCPU";
				this.Prefer32Bit.Value = true;
			} else if (string.Equals((string)targetCpuComboBox.SelectedValue, "AnyCPU64", StringComparison.OrdinalIgnoreCase)) {
				this.PlatformTarget.Value = "AnyCPU";
				this.Prefer32Bit.Value = false;
			} else {
				this.PlatformTarget.Value = (string)targetCpuComboBox.SelectedValue;
				this.Prefer32Bit.Value = null;
			}
		}
		
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
		
		/// <summary>
		/// null means false when targeting .NET 4.0, but true if targeting .NET 4.5
		/// </summary>
		public ProjectOptionPanel.ProjectProperty<bool?> Prefer32Bit {
			get {return projectOptions.GetProperty<bool?>("Prefer32Bit", null, PropertyStorageLocations.PlatformSpecific ); }
		}
		
		public ProjectOptionPanel.ProjectProperty<string> FileAlignment {
			get {return projectOptions.GetProperty("FileAlignment","4096",
			                                       TextBoxEditMode.EditEvaluatedProperty, PropertyStorageLocations.PlatformSpecific ); }
		}
		
		public ProjectOptionPanel.ProjectProperty<string> BaseAddress {
			get {return projectOptions.GetProperty("BaseAddress","",
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
		
		
		public List<KeyItemPair> SerializationInfo {get;set;}
		
		public List<KeyItemPair> TargetCPU {get;set;}
		
		public List<KeyItemPair> FileAlign {get;set;}
		
		
		public string DllBaseAddress {
			get {
				return dllBaseAddress;
			}
			
			set {
				dllBaseAddress = value;
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