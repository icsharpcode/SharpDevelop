/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05/02/2012
 * Time: 19:54
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Converter;
using ICSharpCode.SharpDevelop.Widgets;
using StringPair = System.Collections.Generic.KeyValuePair<string, string>;


namespace CSharpBinding.OptionPanels
{
	/// <summary>
	/// Interaction logic for BuildOptionsXaml.xaml
	/// </summary>
	/// 
	
	public partial class BuildOptionsXaml : ProjectOptionPanel
	{
//		private List<StringPair> fileAlignment;
		
		private ICommand updateProjectCommand;
		private ICommand changeOutputPath;
		private MSBuildBasedProject project;
	
		public BuildOptionsXaml()
		{
			InitializeComponent();
			
			
			/*
			fileAlignment = new List<StringPair>();
			fileAlignment.Add( new StringPair("512", "512"));
			fileAlignment.Add( new StringPair("1024", "1024"));
			fileAlignment.Add(new StringPair("2048", "2048"));
			fileAlignment.Add(new StringPair("4096", "4096"));
			fileAlignment.Add(new StringPair("8192", "8192"));*/
		}
		
		private void Initialize()
		{
			this.UpdateProjectCommand  = new RelayCommand(UpdateProjectExecute);
			this.ChangeOutputPath = new RelayCommand(ChangeOutputPathExecute);
			UpdateTargetFrameworkCombo();
		}
		#region properties
		
		public ProjectProperty<string> DefineConstants {
			get {return GetProperty("DefineConstants", "", TextBoxEditMode.EditRawProperty); }	
		}
		
		public ProjectProperty<string> Optimize {
			get {return GetProperty("Optimize", "", TextBoxEditMode.EditRawProperty); }	
		}
		
		
		public ProjectProperty<string> AllowUnsafeBlocks {
			get {return GetProperty("AllowUnsafeBlocks", "", TextBoxEditMode.EditRawProperty); }	
		}
		
		
		public ProjectProperty<string> CheckForOverflowUnderflow {
			get {return GetProperty("CheckForOverflowUnderflow", "", TextBoxEditMode.EditRawProperty); }	
		}
		
		
		public ProjectProperty<string> NoStdLib {
			get {return GetProperty("NoStdLib", "", TextBoxEditMode.EditRawProperty); }	
		}
		
		
		public ProjectProperty<string> OutputPath {
			get {return GetProperty("OutputPath", "", TextBoxEditMode.EditRawProperty); }	
		}
		
		// Documentfile missing and only partial implemented
		public ProjectProperty<string> DocumentationFile {
			get {return GetProperty("DocumentationFile", "", TextBoxEditMode.EditRawProperty);}	
		}
		
		
		//
		
		public ProjectProperty<DebugSymbolType> DebugType {
			get {return GetProperty("DebugType",ICSharpCode.SharpDevelop.Project.DebugSymbolType.Full ); }	
		}
		
		#endregion
		
		#region overrides
		
		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			base.Load(project, configuration, platform);
			this.project = project;
			this.Initialize();
		}
		
		#endregion
		
		
		#region Command Updateproject
		
		public ICommand UpdateProjectCommand {
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
		
		public ICommand ChangeOutputPath
		{
			get {return this.changeOutputPath;}
			set {this.changeOutputPath = value;
				base.RaisePropertyChanged(() => this.ChangeOutputPath);
			}
		}
		private void ChangeOutputPathExecute()
		{
			OutputPath.Value = base.BrowseForFolder("${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}",
			                                        base.BaseDirectory,outputPathTextBox.Text);		                               
			base.RaisePropertyChanged(()=> OutputPath);
		}
		
		#endregion
		
		
		//Property DebugType
		//void DebugSymbolsLoaded(object sender, EventArgs e)
		
		#region FileAlignment
		/*
		public List<KeyValuePair<string, string>> FileAlign {
			get { return fileAlignment; }
			set { fileAlignment = value; }
		}
		*/
		#endregion
	}
}