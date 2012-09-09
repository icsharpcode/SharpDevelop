/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05/02/2012
 * Time: 19:54
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

using Gui.Dialogs.OptionPanels.ProjectOptions;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Converter;
using ICSharpCode.SharpDevelop.Widgets;

namespace CSharpBinding.OptionPanels
{
	/// <summary>
	/// Interaction logic for BuildOptionsXaml.xaml
	/// </summary>
	/// SYST
	
	public partial class BuildOptions : ProjectOptionPanel
	{

		private List<KeyItemPair> serializationInfo;
		private List<KeyItemPair> targetCPU;
		private List<KeyItemPair> fileAlignment;
		private List<KeyItemPair> warnLevel;
		
		private System.Windows.Input.ICommand updateProjectCommand;
		private System.Windows.Input.ICommand changeOutputPath;
		private System.Windows.Input.ICommand baseIntermediateOutputPathCommand;
		private System.Windows.Input.ICommand intermediateOutputPathCommand;
		
		private MSBuildBasedProject project;
		
		public BuildOptions()
		{
			InitializeComponent();
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
			
			this.warnLevel = new List<KeyItemPair>();
			this.warnLevel.Add(new KeyItemPair("0","0"));
			this.warnLevel.Add(new KeyItemPair("1","1"));
			this.warnLevel.Add(new KeyItemPair("2","2"));
			this.warnLevel.Add(new KeyItemPair("3","3"));
			this.warnLevel.Add(new KeyItemPair("4","4"));
			this.WarnLevel = warnLevel;
		}
		
		private void Initialize()
		{
			this.UpdateProjectCommand  = new RelayCommand(UpdateProjectExecute);
			this.ChangeOutputPath = new RelayCommand(ChangeOutputPathExecute);
			UpdateTargetFrameworkCombo();
			if (DocumentationFile.Value.Length > 0) {
				documentFileIsChecked = true;
			}
			XmlDocHelper();
			this.BaseIntermediateOutputPathCommand = new RelayCommand(BaseIntermediateOutputPathExecute);
			this.IntermediateOutputPathCommand = new RelayCommand(IntermediateOutputPathExecute);
			SetTreatWarningAsErrorRadioButtons();
			IsDirty = false;
		}
		
		#region properties
		
		public ProjectProperty<string> DefineConstants {
			get {return GetProperty("DefineConstants", "",
			                        TextBoxEditMode.EditRawProperty,PropertyStorageLocations.ConfigurationSpecific); }
		}
		
		public ProjectProperty<bool> Optimize {
			get { return GetProperty("Optimize", false, PropertyStorageLocations.ConfigurationSpecific); }
		}
		
		public ProjectProperty<bool> AllowUnsafeBlocks {
			get { return GetProperty("AllowUnsafeBlocks", false); }
		}
		
		public ProjectProperty<bool> CheckForOverflowUnderflow {
			get { return GetProperty("CheckForOverflowUnderflow", false, PropertyStorageLocations.ConfigurationSpecific); }
		}
		
		public ProjectProperty<bool> NoStdLib {
			get { return GetProperty("NoStdLib", false); }
		}
		
		
		public ProjectProperty<string> OutputPath {
			get {return GetProperty("OutputPath", "", TextBoxEditMode.EditRawProperty); }
		}
		
		public ProjectProperty<string> DocumentationFile {
			get {return GetProperty("DocumentationFile", "", TextBoxEditMode.EditRawProperty);}
		}
		
		
		public ProjectProperty<DebugSymbolType> DebugType {
			get {return GetProperty("DebugType",ICSharpCode.SharpDevelop.Project.DebugSymbolType.Full ); }
		}
		
		// used in multibinding
		public ProjectProperty<bool> RegisterForComInterop {
			get {return GetProperty("RegisterForComInterop", false, PropertyStorageLocations.PlatformSpecific ); }
		}
		
		
		public ProjectProperty<string> GenerateSerializationAssemblies {
			get {return GetProperty("GenerateSerializationAssemblies","Auto",
			                        TextBoxEditMode.EditEvaluatedProperty, PropertyStorageLocations.PlatformSpecific ); }
		}
		
		public ProjectProperty<string> PlatformTarget {
			get {return GetProperty("PlatformTarget","AnyCPU",
			                        TextBoxEditMode.EditEvaluatedProperty, PropertyStorageLocations.PlatformSpecific ); }
		}
		
		public ProjectProperty<string> FileAlignment {
			get {return GetProperty("FileAlignment","4096",
			                        TextBoxEditMode.EditEvaluatedProperty, PropertyStorageLocations.PlatformSpecific ); }
		}
		
		public ProjectProperty<string> BaseAddress {
			get {return GetProperty("BaseAddress","1000",
			                        TextBoxEditMode.EditEvaluatedProperty,PropertyStorageLocations.PlatformSpecific ); }
		}
		
		
		public ProjectProperty<string> BaseIntermediateOutputPath {
			get {return GetProperty("BaseIntermediateOutputPath",@"obj\",
			                        TextBoxEditMode.EditRawProperty,PropertyStorageLocations.ConfigurationSpecific ); }
		}
		
		
		public ProjectProperty<string> IntermediateOutputPath {
			get {return GetProperty("IntermediateOutputPath",@"obj\$(Configuration)\",TextBoxEditMode.EditRawProperty ); }
		}
		
		
		public ProjectProperty<string> WarningLevel {
			get {return GetProperty("WarningLevel","4",TextBoxEditMode.EditEvaluatedProperty ); }
		}
		
		
		public ProjectProperty<string> NoWarn {
			get {return GetProperty("NoWarn","",TextBoxEditMode.EditRawProperty ); }
		}
		
		
		public ProjectProperty<string> WarningsAsErrors {
			get {return GetProperty("WarningsAsErrors","",TextBoxEditMode.EditRawProperty ); }
		}
		
		
		public ProjectProperty<bool> TreatWarningsAsErrors {
			get {return GetProperty("TreatWarningsAsErrors", false); }
		}
		
		
		#endregion
		
		#region overrides
		
		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			base.Load(project, configuration, platform);
			this.project = project;
			this.Initialize();
		}
		
		
		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			SaveTreatWarningAsErrorRadioButtons();
			
			NumberStyles style = NumberStyles.Integer;
			if (dllBaseAdress.StartsWith("0x")) {
				dllBaseAdress = dllBaseAdress.Substring(2);
				style = NumberStyles.HexNumber;
			}
			int val;
			if (int.TryParse(dllBaseAdress, style, NumberFormatInfo.InvariantInfo, out val)) {
				BaseAddress.Value = val.ToString(NumberFormatInfo.InvariantInfo);
			}
			
			return base.Save(project, configuration, platform);
		}
		#endregion
		
		#region Documentation File
		
		private bool documentFileIsChecked;
		
		public bool DocumentFileIsChecked {
			get { return documentFileIsChecked; }
			set { documentFileIsChecked = value;
				XmlDocHelper();
				base.RaisePropertyChanged(() => DocumentFileIsChecked);
			}
		}
		
		private void XmlDocHelper()
		{
			if (DocumentFileIsChecked) {
				this.xmlDocumentationTextBox.Text = MSBuildInternals.Escape(
					Path.ChangeExtension(ICSharpCode.Core.FileUtility.GetRelativePath(project.Directory, project.OutputAssemblyFullPath),
					                     ".xml"));
			} else {
				this.xmlDocumentationTextBox.Text = string.Empty;
			}
		}
		
		
		#endregion
		
		#region Command Update Project
		
		public System.Windows.Input.ICommand UpdateProjectCommand {
			get { return updateProjectCommand; }
			set { updateProjectCommand = value;
				base.RaisePropertyChanged(() =>this.UpdateProjectCommand);
			}
		}
		
		
		private void UpdateProjectExecute ()
		{
			UpgradeViewContent.Show(project.ParentSolution).Select(project as IUpgradableProject);
			this.UpdateTargetFrameworkCombo();
		}
		
		private void UpdateTargetFrameworkCombo()
		{
			TargetFramework fx = ((IUpgradableProject)project).CurrentTargetFramework;
			if (fx != null) {
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
				base.RaisePropertyChanged(() => this.ChangeOutputPath);
			}
		}
		private void ChangeOutputPathExecute()
		{
			OutputPath.Value = OptionsHelper.BrowseForFolder("${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}",
			                                                 base.BaseDirectory,base.BaseDirectory,
			                                                 outputPathTextBox.Text,TextBoxEditMode.EditRawProperty);
			base.RaisePropertyChanged(()=> OutputPath);
		}
		
		#endregion
		
		#region SerializationInfo
		
		public List<KeyItemPair> SerializationInfo {
			get {return this.serializationInfo;}
			set {this.serializationInfo = value;
				base.RaisePropertyChanged(() => SerializationInfo);
			}
		}
		
		#endregion
		
		
		#region TargetCPU
		
		public List<KeyItemPair> TargetCPU {
			get { return targetCPU; }
			set { targetCPU = value;
				base.RaisePropertyChanged(() => TargetCPU);
			}
		}
		#endregion
		
		#region FileAlignment
		
		public List<KeyItemPair> FileAlign {
			get { return fileAlignment; }
			set { fileAlignment = value;
				base.RaisePropertyChanged(() => FileAlign);
			}
		}
		
		#endregion
		
		#region DLL BaseAdress
		
		private string dllBaseAdress;
		
		public string DllBaseAdress {
			get {
				int val;
				if (!int.TryParse(BaseAddress.Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out val)) {
					val = 1000;
				}
				return  "0x" + val.ToString("x", NumberFormatInfo.InvariantInfo);
			}
			
			set {
				dllBaseAdress = value;
				
				if (CheckBaseAdress(value)) {
					IsDirty = true;
					base.RaisePropertyChanged(() => DllBaseAdress);
				} else {
					MessageService.ShowMessage("${res:Dialog.ProjectOptions.PleaseEnterValidNumber}");
				}
			}
		}

		
		private bool CheckBaseAdress(string toCheck)
		{
			NumberStyles style = NumberStyles.Integer;
			if (toCheck.StartsWith("0x")) {
				toCheck = toCheck.Substring(2);
				style = NumberStyles.HexNumber;
			}
			if (!String.IsNullOrEmpty(toCheck)) {
				int val;
				if (int.TryParse(toCheck, style, NumberFormatInfo.InvariantInfo, out val)) {
					return true;
				} else {
					MessageService.ShowMessage("${res:Dialog.ProjectOptions.PleaseEnterValidNumber}");
					return false;
				}
			}
			return false;
		}
		
		
		#endregion
		
		#region BaseIntermediateOutputPath
		
		public System.Windows.Input.ICommand BaseIntermediateOutputPathCommand {
			get{return this.baseIntermediateOutputPathCommand;}
			set {this.baseIntermediateOutputPathCommand = value;
				base.RaisePropertyChanged(() => BaseIntermediateOutputPathCommand);}
		}
		
		
		private void BaseIntermediateOutputPathExecute ()
		{
			BaseIntermediateOutputPath.Value = OptionsHelper.BrowseForFolder("${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}",
			                                                                 base.BaseDirectory,base.BaseDirectory,
			                                                                 this.baseIntermediateOutputPathTextBox.Text,TextBoxEditMode.EditRawProperty);
			base.RaisePropertyChanged(()=> BaseIntermediateOutputPath);
		}
		
		#endregion
		
		#region IntermediateOutputPath
		
		public System.Windows.Input.ICommand IntermediateOutputPathCommand {
			get{return this.intermediateOutputPathCommand;}
			set {this.intermediateOutputPathCommand = value;
				base.RaisePropertyChanged(() => IntermediateOutputPathCommand);}
		}
		
		
		private void IntermediateOutputPathExecute ()
		{
			IntermediateOutputPath.Value = OptionsHelper.BrowseForFolder("${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}",
			                                                             base.BaseDirectory,base.BaseDirectory,
			                                                             this.intermediateOutputPathTextBox.Text,TextBoxEditMode.EditRawProperty);
			base.RaisePropertyChanged(()=> IntermediateOutputPath);
		}
		
		#endregion
		
		#region WarningLevel
		
		public List<KeyItemPair> WarnLevel {
			get { return warnLevel; }
			set { warnLevel = value;
				base.RaisePropertyChanged(() => WarnLevel);
			}
		}

		#endregion
		
		#region SpecificWarnings TreatWarningsAsErrors
		
		private void SetTreatWarningAsErrorRadioButtons()
		{
			if (this.TreatWarningsAsErrors.Value) {
				this.allRadioButton.IsChecked  = true;
			} else {
				if (WarningsAsErrors.Value.Length > 0) {
					this.specificWarningsRadioButton.IsChecked = true;
				} else {
					this.noneRadioButton.IsChecked = true;
				}
			}
			this.noneRadioButton.Checked += ErrorButton_Checked;
			this.allRadioButton.Checked += ErrorButton_Checked;
			this.specificWarningsRadioButton.Checked += ErrorButton_Checked;
		}
		
		
		
		
		private void SaveTreatWarningAsErrorRadioButtons()
		{

			if ((bool)this.noneRadioButton.IsChecked){
				this.specificWarningsTextBox.Text = string.Empty;
			}
			
			if ((bool)this.allRadioButton.IsChecked) {
				this.TreatWarningsAsErrors.Value = true;
			}	else {
				this.TreatWarningsAsErrors.Value = false;
			}
			this.noneRadioButton.Checked -= ErrorButton_Checked;
			this.allRadioButton.Checked -= ErrorButton_Checked;
			this.specificWarningsRadioButton.Checked -= ErrorButton_Checked;
		}
		
		void ErrorButton_Checked(object sender, System.Windows.RoutedEventArgs e)
		{
			IsDirty = true;
		}
		
		
		#endregion
	}
	
	
}