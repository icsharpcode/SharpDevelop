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
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
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
		
		public ApplicationOptions()
		{
			InitializeComponent();
			this.DataContext = this;
		}
		
	
		void SetOutputTypeCombo()
		{
			MSBuildItemDefinitionGroup group = new MSBuildItemDefinitionGroup(base.Project, base.Project.ActiveConfiguration);
			string subsystem = group.GetElementMetadata("Link", "SubSystem");
			string configurationType = base.Project.GetEvaluatedProperty("ConfigurationType");
			OutputType validOutputType = ConfigurationTypeToOutputType(configurationType, subsystem);
			this.outputTypeComboBox.SelectedIndex = Array.IndexOf((OutputType[])Enum.GetValues(typeof(OutputType)), validOutputType);
		}
		
		
		void FillManifestCombo()
		{
			applicationManifestComboBox.Items.Add(StringParser.Parse("${res:Dialog.ProjectOptions.ApplicationSettings.Manifest.EmbedDefault}"));
			applicationManifestComboBox.Items.Add(StringParser.Parse("${res:Dialog.ProjectOptions.ApplicationSettings.Manifest.DoNotEmbedManifest}"));
			foreach (string fileName in Directory.GetFiles(base.Project.Directory, "*.manifest")) {
				applicationManifestComboBox.Items.Add(Path.GetFileName(fileName));
			}
			applicationManifestComboBox.Items.Add(StringParser.Parse("<${res:Global.CreateButtonText}...>"));
			applicationManifestComboBox.Items.Add(StringParser.Parse("<${res:Global.BrowseText}...>"));
			applicationManifestComboBox.SelectedIndex = 0;
		}
		
		
		void project_MinimumSolutionVersionChanged(object sender, EventArgs e)
		{
			// embedding manifests requires the project to target MSBuild 3.5 or higher
			applicationManifestComboBox.IsEnabled = base.Project.MinimumSolutionVersion >= SolutionFormatVersion.VS2008;
		}
		
		
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
		
		protected override void Initialize()
		{
			base.Initialize();
			
			foreach (var c in GetPossibleStartupObjects(base.Project)) {
				startupObjectComboBox.Items.Add(c.FullName);
			}
			
			SetOutputTypeCombo();
			
			FillManifestCombo();
			
			// embedding manifests requires the project to target MSBuild 3.5 or higher
			project_MinimumSolutionVersionChanged(null, null);
			// re-evluate if the project has the minimum version whenever this options page gets visible
			// because the "convert project" button on the compiling tab page might have updated the MSBuild version.
			base.Project.MinimumSolutionVersionChanged += project_MinimumSolutionVersionChanged;
			
			this.projectInformation.ProjectFolder = base.BaseDirectory;
			this.projectInformation.ProjectFile = Path.GetFileName(base.Project.FileName);
			
			//OptionBinding
			RefreshStartupObjectEnabled(this, EventArgs.Empty);
			RefreshOutputNameTextBox(this, null);
			
			//SetApplicationIcon();
			this.applicationIconTextBox.Text = GetApplicationIconPathFromResourceScripts();
			ApplicationIconTextBox_TextChanged(this,null);
			
			this.applicationIconTextBox.TextChanged += ApplicationIconTextBox_TextChanged;
			
			this.startupObjectComboBox.SelectionChanged += (s,e) => {IsDirty = true;};
			this.outputTypeComboBox.SelectionChanged += OutputTypeComboBox_SelectionChanged;
			IsDirty = false;
		}
		
		public override void Dispose()
		{
			base.Project.MinimumSolutionVersionChanged -= project_MinimumSolutionVersionChanged;
			base.Dispose();
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
			MSBuildItemDefinitionGroup group = new MSBuildItemDefinitionGroup(base.Project, base.Project.ActiveConfiguration);
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
		
		public static IList<IUnresolvedTypeDefinition> GetPossibleStartupObjects(IProject project)
		{
			List<IUnresolvedTypeDefinition> results = new List<IUnresolvedTypeDefinition>();
			IProjectContent pc = project.ProjectContent;
			if (pc != null) {
				foreach (var c in pc.TopLevelTypeDefinitions) {
					foreach (var m in c.Methods) {
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
				this.projectInformation.OutputTypeName = this.assemblyNameTextBox.Text + CompilableProject.GetExtension(enmType);
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
			IEnumerable <ProjectItem> resourceScripts =base.Project .Items.Where(
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
				iconResourceScriptPath = AddResourceScriptToProject(base.Project, DEFAULT_RC_NAME);
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
			ProjectService.AddProjectItem(project, rcFileItem);
			return fileName;
		}
		
		
		void ApplicationIconButton_Click(object sender, RoutedEventArgs e)
		{
			BrowseForFile(ApplicationIcon, iconsfilter);
		}
		
		
		void ApplicationIconTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (base.Project != null) {
				if(FileUtility.IsValidPath(this.applicationIconTextBox.Text))
				{
					string appIconPath = Path.Combine(base.Project.Directory, this.applicationIconTextBox.Text);
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
			IsDirty = true;
		}
		
		
		void BrowseForManifest()
		{
			BrowseForFile(ApplicationManifest, manifestFilter);
		}
		
		void CreateManifest()
		{
			string manifestFile = Path.Combine(base.Project.Directory, "app.manifest");
			if (!File.Exists(manifestFile)) {
				string defaultManifest;
				using (Stream stream = typeof(ApplicationSettings).Assembly.GetManifestResourceStream("Resources.DefaultManifest.manifest")) {
					if (stream == null)
						throw new ResourceNotFoundException("DefaultManifest.manifest");
					using (StreamReader r = new StreamReader(stream)) {
						defaultManifest = r.ReadToEnd();
					}
				}
				defaultManifest = defaultManifest.Replace("\t", SD.EditorControlService.GlobalOptions.IndentationString);
				File.WriteAllText(manifestFile, defaultManifest, System.Text.Encoding.UTF8);
				FileService.FireFileCreated(manifestFile, false);
			}
			
			if (!base.Project.IsFileInProject(FileName.Create(manifestFile))) {
				FileProjectItem newItem = new FileProjectItem(base.Project, ItemType.None);
				newItem.Include = "app.manifest";
				ProjectService.AddProjectItem(base.Project, newItem);
				ProjectBrowserPad.RefreshViewAsync();
			}
			
			FileService.OpenFile(manifestFile);
			
			this.applicationManifestComboBox.Items.Insert(0,"app.manifest");
			this.applicationManifestComboBox.SelectedIndex = 0;
		}
		
		#endregion
	}
}
