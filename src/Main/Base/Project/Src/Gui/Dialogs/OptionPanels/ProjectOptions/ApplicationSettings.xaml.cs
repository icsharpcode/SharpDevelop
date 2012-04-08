/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 01.04.2012
 * Time: 17:16
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Win32;

//using System.Windows.Forms;





namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for ApplicationSettingsXaml.xaml
	/// </summary>
	public partial class ApplicationSettings : ProjectOptionPanel
	{
		private const string iconsfilter = "${res:SharpDevelop.FileFilter.Icons}|*.ico|${res:SharpDevelop.FileFilter.AllFiles}|*.*";
		private const string manifestFilter = "${res:Dialog.ProjectOptions.ApplicationSettings.Manifest.ManifestFiles}|*.manifest|${res:SharpDevelop.FileFilter.AllFiles}|*.*";
		private const string win32filter = "Win32 Resource files|*.res|${res:SharpDevelop.FileFilter.AllFiles}|*.*";
		MSBuildBasedProject project;
		
		public ApplicationSettings()
		{
			InitializeComponent();
		}
		
		
		private void Initialize()
		{
			
//			<ComboBox  MinWidth="85" .. gui:EnumBinding.EnumType="{x:Type nr everity}" SelectedValue="{Binding Severity}"/>
//			http://www.beacosta.com/blog/?p=52
			foreach (IClass c in GetPossibleStartupObjects(project)) {
				startupObjectComboBox.Items.Add(c.FullyQualifiedName);
			}

			this.outputTypeComboBox.SelectedValue = OutputType.Value.ToString();
			
			FillManifestCombo();
			
			// embedding manifests requires the project to target MSBuild 3.5 or higher
			project_MinimumSolutionVersionChanged(null, null);
			// re-evaluate if the project has the minimum version whenever this options page gets visible
			// because the "convert project" button on the compiling tab page might have updated the MSBuild version.
			project.MinimumSolutionVersionChanged += project_MinimumSolutionVersionChanged;
			
			projectFolderTextBox.Text = project.Directory;
			projectFileTextBox.Text = Path.GetFileName(project.FileName);
			
			//OptionBinding
			RefreshStartupObjectEnabled(this, EventArgs.Empty);
			RefreshOutputNameTextBox(this, null);
			
			ApplicationIconTextBox_TextChanged(this,null);
			this.startupObjectComboBox.SelectionChanged += (s,e) => {IsDirty = true;};
		}

		
		void FillManifestCombo()
		{
			applicationManifestComboBox.Items.Add(StringParser.Parse("${res:Dialog.ProjectOptions.ApplicationSettings.Manifest.EmbedDefault}"));
			applicationManifestComboBox.Items.Add(StringParser.Parse("${res:Dialog.ProjectOptions.ApplicationSettings.Manifest.DoNotEmbedManifest}"));
			foreach (string fileName in Directory.GetFiles(project.Directory, "*.manifest")) {
				applicationManifestComboBox.Items.Add(Path.GetFileName(fileName));
			}
			applicationManifestComboBox.Items.Add(StringParser.Parse("<${res:Global.CreateButtonText}...>"));
			applicationManifestComboBox.Items.Add(StringParser.Parse("<${res:Global.BrowseText}...>"));
			applicationManifestComboBox.SelectedIndex = 0;
		}
		
		
		public ProjectProperty<string> AssemblyName {
			get { return GetProperty("AssemblyName", "", TextBoxEditMode.EditRawProperty); }
		}
		
		public ProjectProperty<string> RootNamespace {
			get { return GetProperty("RootNamespace", "", TextBoxEditMode.EditRawProperty); }
		}
		
		
		public ProjectProperty<OutputType> OutputType {
			get {return GetProperty("OutputType", ICSharpCode.SharpDevelop.Project.OutputType.Exe); }	
		}
		
		
		public ProjectProperty<string> ApplicationIcon {
			get { return GetProperty("ApplicationIcon", "", TextBoxEditMode.EditRawProperty); }
		}
		
		
		public ProjectProperty<string> ApplicationManifest {
			get { return GetProperty("ApplicationManifest", "", TextBoxEditMode.EditRawProperty); }
		}
		
		
		public ProjectProperty<string> Win32Resource {
			get { return GetProperty("Win32Resource", "", TextBoxEditMode.EditRawProperty); }
		}
		
		
		#region overrides
		
		
		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			base.Load(project, configuration, platform);
			this.project = project;
			Initialize();
		}
		
		
		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			return base.Save(project, configuration, platform);
		}
		
		#endregion
		
		
		public static IList<IClass> GetPossibleStartupObjects(IProject project)
		{
			List<IClass> results = new List<IClass>();
			IProjectContent pc = ParserService.GetProjectContent(project);
			if (pc != null) {
				foreach(IClass c in pc.Classes) {
					foreach (IMethod m in c.Methods) {
						if (m.IsStatic && m.Name == "Main") {
							results.Add(c);
						}
					}
				}
			}
			return results;
		}
		
		void project_MinimumSolutionVersionChanged(object sender, EventArgs e)
		{
			// embedding manifests requires the project to target MSBuild 3.5 or higher
			applicationManifestComboBox.IsEnabled = project.MinimumSolutionVersion >= Solution.SolutionVersionVS2008;
		}
			
		#region refresh Outputpath + StartupOptions
		
		void RefreshOutputNameTextBox (object sender, TextChangedEventArgs e)
		{
			if (this.outputTypeComboBox.SelectedValue != null) {
				var enmType = (OutputType) Enum.Parse(typeof(OutputType),this.outputTypeComboBox.SelectedValue.ToString());
				this.outputNameTextBox.Text = this.assemblyNameTextBox.Text + CompilableProject.GetExtension(enmType);
			}
		}
		
		
		void RefreshStartupObjectEnabled(object sender, EventArgs e)
		{
			if (this.outputTypeComboBox.SelectedValue != null) {
				var enmType = (OutputType) Enum.Parse(typeof(OutputType),this.outputTypeComboBox.SelectedValue.ToString());
				bool isLibrary = ICSharpCode.SharpDevelop.Project.OutputType.Library == enmType;
				startupObjectComboBox.IsEnabled = !isLibrary;
			}
		}
		
		
		void OutputTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			RefreshOutputNameTextBox(this,null);
			RefreshStartupObjectEnabled(this,null);
		}
		
		
		#endregion
	
		#region ApplicationIcon
		
		void ApplicationIconButton_Click(object sender, RoutedEventArgs e)
		{
			var filter  = StringParser.Parse(iconsfilter);
			string fileName = BrowseForFile(filter);
			if (!String.IsNullOrEmpty(fileName))
			{
				this.applicationIconTextBox.Text = fileName;
			}
		}
		
		
		void ApplicationIconTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (project != null) {
				if(FileUtility.IsValidPath(this.applicationIconTextBox.Text))
				{
					string appIconPath = Path.Combine(project.Directory, this.applicationIconTextBox.Text);
					Console.WriteLine(appIconPath);
					var b = File.Exists(appIconPath);
					if (File.Exists(appIconPath)) {
						
						try {
		
							FileStream stream = new FileStream(appIconPath, FileMode.Open, FileAccess.Read);
							Image image = new Image();
							BitmapImage src = new BitmapImage();
							src.BeginInit();
							src.StreamSource = stream;
							src.EndInit();
							
							image.Source = src;
							image.Stretch = Stretch.Uniform;

							this.applicationIconImage.Source = image.Source;
							this.applicationIconImage.Stretch = Stretch.Fill;

						} catch (OutOfMemoryException) {
							this.applicationIconImage.Source = null;
							MessageService.ShowErrorFormatted("${res:Dialog.ProjectOptions.ApplicationSettings.InvalidIconFile}",
							                                  FileUtility.NormalizePath(appIconPath));
						}
					} else {
						this.applicationIconImage.Source = null;
					}
				}
			}
		}
		
		#endregion
		
		#region manifest
		
		void ApplicationManifestComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (applicationManifestComboBox.SelectedIndex == applicationManifestComboBox.Items.Count - 2) {
				CreateManifest();
			} else if (applicationManifestComboBox.SelectedIndex == applicationManifestComboBox.Items.Count - 1) {
				BrowseForManifest();
			}
		}
		
		
		void BrowseForManifest()
		{
			applicationManifestComboBox.SelectedIndex = -1;
			var fileName = BrowseForFile(manifestFilter);
			if (!String.IsNullOrEmpty(fileName)) {
				this.applicationManifestComboBox.Items.Insert(0,fileName);
				this.applicationManifestComboBox.SelectedIndex = 0;
			}
		}
		
		void CreateManifest()
		{
			string manifestFile = Path.Combine(project.Directory, "app.manifest");
			if (!File.Exists(manifestFile)) {
				string defaultManifest;
				using (Stream stream = typeof(ApplicationSettings).Assembly.GetManifestResourceStream("Resources.DefaultManifest.manifest")) {
					if (stream == null)
						throw new ResourceNotFoundException("DefaultManifest.manifest");
					using (StreamReader r = new StreamReader(stream)) {
						defaultManifest = r.ReadToEnd();
					}
				}
				defaultManifest = defaultManifest.Replace("\t", EditorControlService.GlobalOptions.IndentationString);
				File.WriteAllText(manifestFile, defaultManifest, System.Text.Encoding.UTF8);
				FileService.FireFileCreated(manifestFile, false);
			}
			
			if (!project.IsFileInProject(manifestFile)) {
				FileProjectItem newItem = new FileProjectItem(project, ItemType.None);
				newItem.Include = "app.manifest";
				ProjectService.AddProjectItem(project, newItem);
				ProjectBrowserPad.RefreshViewAsync();
			}
		
			FileService.OpenFile(manifestFile);
			
			this.applicationManifestComboBox.Items.Insert(0,"app.manifest");
			this.applicationManifestComboBox.SelectedIndex = 0;
		}
		
		#endregion
		
		#region openFile
		
		string BrowseForFile (string filter)
		{
			OpenFileDialog fileDialog = new OpenFileDialog {
				Filter = filter,
				Multiselect = false
			};

			if (fileDialog.ShowDialog() != true || fileDialog.FileNames.Length == 0)
				return String.Empty;
			return fileDialog.FileName;
		}
		
		#endregion
		
		
		#region Win32ResourceFile
		
		void Win32ResourceComboButton_Click(object sender, RoutedEventArgs e)
		{
			var filter  = StringParser.Parse(win32filter);
			string fileName = BrowseForFile(filter);
			if (!String.IsNullOrEmpty(fileName))
			{
				this.win32ResourceFileTextBox.Text = fileName;
			}
		}
		
		#endregion
		
			
	}
}
