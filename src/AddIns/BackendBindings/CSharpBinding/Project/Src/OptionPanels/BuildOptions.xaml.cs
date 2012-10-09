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
	
	public partial class BuildOptions : ProjectOptionPanel
	{

//		private System.Windows.Input.ICommand updateProjectCommand;
//		private System.Windows.Input.ICommand changeOutputPath;
		
		public BuildOptions()
		{
			InitializeComponent();
			this.DataContext = this;
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
		
		
//		public ProjectProperty<string> OutputPath {
//			get {return GetProperty("OutputPath", "", TextBoxEditMode.EditRawProperty); }
//		}
//		
//		
//		public ProjectProperty<string> DocumentationFile {
//			get {return GetProperty("DocumentationFile", "", TextBoxEditMode.EditRawProperty);}
//		}
//		
//		
//		public ProjectProperty<DebugSymbolType> DebugType {
//			get {return GetProperty("DebugType",ICSharpCode.SharpDevelop.Project.DebugSymbolType.Full ); }
//		}
		
	
		#endregion
		
		#region overrides
		
		protected override void Initialize()
		{
			base.Initialize();
			buildOutput.SetProjectOptions(this);
			this.buildAdvanced.SetProjectOptions(this);
			this.errorsAndWarnings.SetProjectOptions(this);
			this.treatErrorsAndWarnings.SetProjectOptions(this);
		}
		
		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			base.Load(project, configuration, platform);
//			buildOutput.SetProjectOptions(this);
//			this.buildAdvanced.SetProjectOptions(this);
//			this.errorsAndWarnings.SetProjectOptions(this);
//			this.treatErrorsAndWarnings.SetProjectOptions(this);
			IsDirty = false;
		}
		
		
		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			if (buildAdvanced.SaveProjectOptions()) {
				treatErrorsAndWarnings.SaveProjectOptions();
				return base.Save(project, configuration, platform);
			}
			return false;
		}
		
		
		#endregion
		
		
		#region Command Update Project
		
//		public System.Windows.Input.ICommand UpdateProjectCommand {
//			get { return updateProjectCommand; }
//			set { updateProjectCommand = value;
//				base.RaisePropertyChanged(() =>this.UpdateProjectCommand);
//			}
//		}
		
		
//		private void UpdateProjectExecute ()
//		{
//			UpgradeViewContent.Show(base.Project.ParentSolution).Select(base.Project as IUpgradableProject);
////			this.UpdateTargetFrameworkCombo();
//		}
		
//		private void UpdateTargetFrameworkCombo()
//		{
//			/*
//			TargetFramework fx = ((IUpgradableProject)base.Project).CurrentTargetFramework;
//			if (fx != null) {
//				targetFrameworkComboBox.Items.Add(fx.DisplayName);
//				targetFrameworkComboBox.SelectedIndex = 0;
//			}
//			*/
//		}
		
		#endregion
		
		
		#region ChangeOutputPathCommand
		
//		public System.Windows.Input.ICommand ChangeOutputPath
//		{
//			get {return this.changeOutputPath;}
//			set {this.changeOutputPath = value;
//				base.RaisePropertyChanged(() => this.ChangeOutputPath);
//			}
//		}
//		private void ChangeOutputPathExecute()
//		{
////			BrowseForFolder(OutputPath, "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}");
//		}
//		
		#endregion
		
		
		#region SerializationInfo
		
//		public List<KeyItemPair> SerializationInfo {
//			get {return this.serializationInfo;}
//			set {this.serializationInfo = value;
//				base.RaisePropertyChanged(() => SerializationInfo);
//			}
//		}
		
		#endregion
		
		
		#region TargetCPU
		
//		public List<KeyItemPair> TargetCPU {
//			get { return targetCPU; }
//			set { targetCPU = value;
//				base.RaisePropertyChanged(() => TargetCPU);
//			}
//		}
		#endregion
		
		
		#region FileAlignment
		
//		public List<KeyItemPair> FileAlign {
//			get { return fileAlignment; }
//			set { fileAlignment = value;
//				base.RaisePropertyChanged(() => FileAlign);
//			}
//		}
		
		#endregion
	}
}