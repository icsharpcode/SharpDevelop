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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CppBinding.Project
{
	/// <summary>
	/// Interaction logic for ApplicationSettingsXaml.xaml
	/// </summary>
	public partial class ApplicationOptions : ProjectOptionPanel
	{
		private const string iconsfilter = "${res:SharpDevelop.FileFilter.Icons}|*.ico|${res:SharpDevelop.FileFilter.AllFiles}|*.*";
		private const string manifestFilter = "${res:Dialog.ProjectOptions.ApplicationSettings.Manifest.ManifestFiles}|*.manifest|${res:SharpDevelop.FileFilter.AllFiles}|*.*";
		private const string win32filter = "Win32 Resource files|*.res|${res:SharpDevelop.FileFilter.AllFiles}|*.*";
		MSBuildBasedProject project;
		
		public ApplicationOptions()
		{
			InitializeComponent();
		}
		
		#region Initialize
		
		private void Initialize()
		{
			
			foreach (IClass c in GetPossibleStartupObjects(project)) {
				startupObjectComboBox.Items.Add(c.FullyQualifiedName);
			}

			//this.outputTypeComboBox.SelectedValue = OutputType.Value.ToString();
			
			SetOutputTypeCombo();
			
			FillManifestCombo();
			
			// embedding manifests requires the project to target MSBuild 3.5 or higher
			project_MinimumSolutionVersionChanged(null, null);
			// re-evluate if the project has the minimum version whenever this options page gets visible
			// because the "convert project" button on the compiling tab page might have updated the MSBuild version.
			project.MinimumSolutionVersionChanged += project_MinimumSolutionVersionChanged;
			
			projectFolderTextBox.Text = project.Directory;
			projectFileTextBox.Text = Path.GetFileName(project.FileName);
			
			//OptionBinding
			RefreshStartupObjectEnabled(this, EventArgs.Empty);
			RefreshOutputNameTextBox(this, null);
			
			//SetApplicationIcon();
			this.applicationIconTextBox.Text = GetApplicationIconPathFromResourceScripts();
			ApplicationIconTextBox_TextChanged(this,null);
			IsDirty = false;
			this.applicationIconTextBox.TextChanged += ApplicationIconTextBox_TextChanged;
			
			this.startupObjectComboBox.SelectionChanged += (s,e) => {IsDirty = true;};
			this.outputTypeComboBox.SelectionChanged += OutputTypeComboBox_SelectionChanged;
		}

		
		void SetOutputTypeCombo()
		{
			MSBuildItemDefinitionGroup group = new MSBuildItemDefinitionGroup(project, project.ActiveConfiguration, project.ActivePlatform);
			string subsystem = group.GetElementMetadata("Link", "SubSystem");
			string configurationType = project.GetEvaluatedProperty("ConfigurationType");
			OutputType validOutputType = ConfigurationTypeToOutputType(configurationType, subsystem);
			this.outputTypeComboBox.SelectedIndex = Array.IndexOf((OutputType[])Enum.GetValues(typeof(OutputType)), validOutputType);
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
		
		
		void project_MinimumSolutionVersionChanged(object sender, EventArgs e)
		{
			// embedding manifests requires the project to target MSBuild 3.5 or higher
			applicationManifestComboBox.IsEnabled = project.MinimumSolutionVersion >= Solution.SolutionVersionVS2008;
		}
		
		
		#endregion
		
		#region Properties
		
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
		
		
		public ProjectProperty<string> ConfigurationType {
			get { return GetProperty("ConfigurationType", "", TextBoxEditMode.EditRawProperty); }
		}
		
		
//		public ProjectProperty<string> Win32Resource {
//			get { return GetProperty("Win32Resource", "", TextBoxEditMode.EditRawProperty); }
//		}
		
		#endregion
		
		
		#region overrides
		
		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			base.Load(project, configuration, platform);
			this.project = project;
			Initialize();
		}
		
		
		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			ConfigurationType.Value = ConvertOutputType();
			SetApplicationIcon();
			return base.Save(project, configuration, platform);
		}
		
		#endregion
		
		
		#region OutputType <-> ConfigurationType property mapping
		
		/// <summary>
		/// Applies the OutputType property value from combo box control to the vcxproj project.
		/// <para>The OutputType property is translated to ConfigurationType and Subsystem properties</para>
		/// </summary>
		/// <returns>the ConfigurationType associated to OutputType</returns>
		string ConvertOutputType()
		{
			OutputType[] values = (OutputType[])Enum.GetValues(typeof(OutputType));
			OutputType outputType = values[this.outputTypeComboBox.SelectedIndex];
			
			string subsystem = OutputTypeToSubsystem(outputType);
			MSBuildItemDefinitionGroup group = new MSBuildItemDefinitionGroup(project,
			                                                                  project.ActiveConfiguration, project.ActivePlatform);
			group.SetElementMetadata("Link", "SubSystem", subsystem);
			
			return OutputTypeToConfigurationType(outputType);
		}
		
		
		static string OutputTypeToConfigurationType(OutputType outputType)
		{
			switch (outputType)
			{
				case ICSharpCode.SharpDevelop.Project.OutputType.Exe:
					return "Application";
				case ICSharpCode.SharpDevelop.Project.OutputType.Library:
					return "DynamicLibrary";
				case ICSharpCode.SharpDevelop.Project.OutputType.Module:
					//TODO: get an apropriate way to handle netmodule creation
					//see: http://msdn.microsoft.com/en-us/library/k669k83h(VS.80).aspx
					LoggingService.Info(".netmodule output not supported, will produce a class library");
					return "DynamicLibrary";
				case ICSharpCode.SharpDevelop.Project.OutputType.WinExe:
					return "Application";
			}
			throw new ArgumentException("Unknown OutputType value " + outputType);
		}
		
		static string OutputTypeToSubsystem(OutputType outputType)
		{
			if (ICSharpCode.SharpDevelop.Project.OutputType.WinExe == outputType)
				return "Windows";
			return "Console";
		}
		
		static OutputType ConfigurationTypeToOutputType(string configurationType, string subsystem)
		{
			if ("Application" == configurationType && "Windows" != subsystem)
				return ICSharpCode.SharpDevelop.Project.OutputType.Exe;
			else if ("Application" == configurationType && "Windows" == subsystem)
				return ICSharpCode.SharpDevelop.Project.OutputType.WinExe;
			else if ("DynamicLibrary" == configurationType)
				return ICSharpCode.SharpDevelop.Project.OutputType.Library;
			LoggingService.Info("ConfigurationType " +configurationType + " is not supported, will use Library output type");
			return ICSharpCode.SharpDevelop.Project.OutputType.Library;
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
			IsDirty = true;
		}
		
		
		#endregion
		
		#region ApplicationIcon
		
		const string DEFAULT_ICON_ID = "ICON0";
		const string DEFAULT_RC_NAME = "app.rc";
		string iconResourceScriptPath;	//path to the resource script where application icon is defined
		ResourceEntry foundIconEntry;
		
		/// <summary>
		/// Gets the icon file location from the rc files added to project. 
		/// Searches all project items of type "ResourceCompile" and returns the resource of type ICON with the lowest ID.
		/// </summary>
		/// <returns>path to the icon file or null if the icon wasn't specified</returns>
		string GetApplicationIconPathFromResourceScripts() {
			foundIconEntry = null;
			iconResourceScriptPath = null;
			IEnumerable <ProjectItem> resourceScripts = project.Items.Where(
						item => item is FileProjectItem && ((FileProjectItem)item).BuildAction == "ResourceCompile");			
			
			// search in all resource scripts, but due to limitation in resource compiler, only one of them can contain icons
			foreach (ProjectItem item in resourceScripts) {
				ResourceScript rc = new ResourceScript(item.FileName);
				if (rc.Icons.Count == 0) continue;
				if (foundIconEntry == null || rc.Icons.First().ResourceID.CompareTo(foundIconEntry.ResourceID)<0) {
					foundIconEntry = rc.Icons.First();
					iconResourceScriptPath = item.FileName;
				}
			}
			
			//when no icon was found, then select the resource script where icon definition may be created
			if (iconResourceScriptPath == null && resourceScripts.Any())
				iconResourceScriptPath = resourceScripts.First().FileName;
				
			return foundIconEntry != null ? foundIconEntry.Data : null;
		}
		
		
		void SetApplicationIcon() {            
			string iconPath = this.applicationIconTextBox.Text;
			string newIconId;
			ResourceScript rc;
			if (iconPath.Trim() == "") return;
			if (iconResourceScriptPath != null)
			{
				rc = new ResourceScript(iconResourceScriptPath);
				newIconId = foundIconEntry != null ? foundIconEntry.ResourceID : DEFAULT_ICON_ID;
				rc.Save(iconResourceScriptPath);
			}
			else
			{
				iconResourceScriptPath = AddResourceScriptToProject(project, DEFAULT_RC_NAME);
				rc = new ResourceScript();
				newIconId = DEFAULT_ICON_ID;
			}
			rc.SetIcon(newIconId, iconPath);
			rc.Save(iconResourceScriptPath);
		}		
		
		
		static string AddResourceScriptToProject(IProject project, string rcFileName) {
			string fileName = Path.Combine(project.Directory, rcFileName);
			FileProjectItem rcFileItem = new FileProjectItem(project, project.GetDefaultItemType(fileName));
			rcFileItem.Include = FileUtility.GetRelativePath(project.Directory, fileName);
			((IProjectItemListProvider)project).AddProjectItem(rcFileItem);
			return fileName;
		}
		
		
		void ApplicationIconButton_Click(object sender, RoutedEventArgs e)
		{
			string fileName = OptionsHelper.OpenFile(iconsfilter);
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
							IsDirty = true;
							
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
			var fileName = OptionsHelper.OpenFile(manifestFilter);
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
	}
}
