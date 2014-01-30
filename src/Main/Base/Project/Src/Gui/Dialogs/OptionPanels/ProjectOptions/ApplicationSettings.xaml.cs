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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;

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
		
		public ApplicationSettings()
		{
			InitializeComponent();
			this.DataContext = this;
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			startupObjectComboBox.Items.Clear();
			foreach (var c in GetPossibleStartupObjects(base.Project)) {
				startupObjectComboBox.Items.Add(c.FullName);
			}
			
			FillManifestCombo();
			
			// embedding manifests requires the project to target MSBuild 3.5 or higher
			project_MinimumSolutionVersionChanged(null, null);
			// re-evaluate if the project has the minimum version whenever this options page gets visible
			// because the "convert project" button on the compiling tab page might have updated the MSBuild version.
			base.Project.MinimumSolutionVersionChanged += project_MinimumSolutionVersionChanged;
			
			this.projectInformation.ProjectFolder = base.BaseDirectory;
			this.projectInformation.ProjectFile = Path.GetFileName(base.Project.FileName);
			
			//OptionBinding
			RefreshStartupObjectEnabled(this, EventArgs.Empty);
			RefreshOutputNameTextBox(this, null);
			
			ApplicationIconTextBox_TextChanged(this,null);
			this.outputTypeComboBox.SelectionChanged += RefreshOutputNameTextBox;
			this.outputTypeComboBox.SelectionChanged += RefreshStartupObjectEnabled;
		}

		public override void Dispose()
		{
			base.Project.MinimumSolutionVersionChanged -= project_MinimumSolutionVersionChanged;
			base.Dispose();
		}
		
		// must be observable because we insert an entry in the <Create> action
		readonly ObservableCollection<string> manifestItems = new ObservableCollection<string>();
		
		public ObservableCollection<string> ManifestItems {
			get { return manifestItems; }
		}
		
		public ProjectProperty<string> AssemblyName {
			get { return GetProperty("AssemblyName", "", TextBoxEditMode.EditEvaluatedProperty); }
		}
		
		
		public ProjectProperty<string> RootNamespace {
			get { return GetProperty("RootNamespace", "", TextBoxEditMode.EditEvaluatedProperty); }
		}
		
		
		public ProjectProperty<OutputType> OutputType {
			get { return GetProperty("OutputType", ICSharpCode.SharpDevelop.Project.OutputType.Exe); }
		}
		
		
		public ProjectProperty<string> StartupObject {
			get { return GetProperty("StartupObject", "", TextBoxEditMode.EditEvaluatedProperty); }
		}
		
		
		public ProjectProperty<string> ApplicationIcon {
			get { return GetProperty("ApplicationIcon", "", TextBoxEditMode.EditRawProperty); }
		}
		
		
		public ProjectProperty<string> ApplicationManifest {
			get { return GetProperty("ApplicationManifest", "", TextBoxEditMode.EditRawProperty); }
		}
		
		
		public ProjectProperty<bool> NoWin32Manifest {
			get { return GetProperty("NoWin32Manifest", false); }
		}
		
		
		public ProjectProperty<string> Win32Resource {
			get { return GetProperty("Win32Resource", "", TextBoxEditMode.EditRawProperty); }
		}
		
		
		#region Load/Save
		
		
		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			base.Load(project, configuration, platform);
			// Ensure that the template is applied before we assign ComboBox.Text, this ensures
			// the TextBoxBase.TextChanged event fires immediately and doesn't cause us to
			// set the IsDirty flag later.
			applicationManifestComboBox.ApplyTemplate();
			if (string.IsNullOrEmpty(this.ApplicationManifest.Value)) {
				if (this.NoWin32Manifest.Value) {
					applicationManifestComboBox.SelectedIndex = 1;
				} else {
					applicationManifestComboBox.SelectedIndex = 0;
				}
			} else {
				applicationManifestComboBox.Text = this.ApplicationManifest.Value;
			}
			this.projectInformation.OutputTypeName = AssemblyName.Value + CompilableProject.GetExtension(OutputType.Value);
			IsDirty = false;
		}
		
		
		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			if (applicationManifestComboBox.SelectedIndex == 0) {
				// Embed default manifest
				this.NoWin32Manifest.Value = false;
				this.ApplicationManifest.Value = "";
				this.NoWin32Manifest.Location = ApplicationManifest.Location;
			} else if (applicationManifestComboBox.SelectedIndex == 1) {
				// No manifest
				this.NoWin32Manifest.Value = true;
				this.ApplicationManifest.Value = "";
				this.NoWin32Manifest.Location = ApplicationManifest.Location;
			} else {
				ApplicationManifest.Value = applicationManifestComboBox.Text;
				this.NoWin32Manifest.Value = false;
				this.NoWin32Manifest.Location = ApplicationManifest.Location;
			}
			return base.Save(project, configuration, platform);
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
		
		
		void project_MinimumSolutionVersionChanged(object sender, EventArgs e)
		{
			// embedding manifests requires the project to target MSBuild 3.5 or higher
			applicationManifestComboBox.IsEnabled = base.Project.MinimumSolutionVersion >= SolutionFormatVersion.VS2008;
		}
		
		
		#region refresh Outputpath + StartupOptions
		
		void RefreshOutputNameTextBox (object sender, EventArgs e)
		{
			if (this.outputTypeComboBox.SelectedValue != null) {
				var outputType = (OutputType)this.outputTypeComboBox.SelectedValue;
				this.projectInformation.OutputTypeName = this.assemblyNameTextBox.Text + CompilableProject.GetExtension(outputType);
			}
		}
		
		void RefreshStartupObjectEnabled(object sender, EventArgs e)
		{
			if (this.outputTypeComboBox.SelectedValue != null) {
				var outputType = (OutputType)this.outputTypeComboBox.SelectedValue;
				startupObjectComboBox.IsEnabled = outputType != SharpDevelop.Project.OutputType.Library;
			}
		}
		#endregion
		
		#region ApplicationIcon
		
		void ApplicationIconButton_Click(object sender, RoutedEventArgs e)
		{
			BrowseForFile(ApplicationIcon, iconsfilter);
		}

		void ApplicationIconTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (base.Project != null) {
				if(FileUtility.IsValidPath(this.applicationIconTextBox.Text))
				{
					string appIconPath = Path.Combine(base.BaseDirectory, this.applicationIconTextBox.Text);
					if (File.Exists(appIconPath)) {
						try {
							MemoryStream memoryStream = new MemoryStream();
							using (var stream = new FileStream(appIconPath, FileMode.Open, FileAccess.Read))
							{
								stream.CopyTo(memoryStream);
							}
							memoryStream.Position = 0;
							
							BitmapImage src = new BitmapImage();
							src.BeginInit();
							src.StreamSource = memoryStream;
							src.EndInit();
							Image = src;
							
						} catch (OutOfMemoryException) {
							Image = null;
							MessageService.ShowErrorFormatted("${res:Dialog.ProjectOptions.ApplicationSettings.InvalidIconFile}",
							                                  FileUtility.NormalizePath(appIconPath));
						} catch (NotSupportedException) {
							Image = null;
							MessageService.ShowErrorFormatted("${res:Dialog.ProjectOptions.ApplicationSettings.InvalidIconFile}",
							                                  FileUtility.NormalizePath(appIconPath));
						}
					} else {
						Image = null;
					}
				}
			}
		}
		
		private ImageSource image;
		
		public ImageSource Image {
			get { return image; }
			set { image = value;
				base.RaisePropertyChanged(() => Image);
			}
		}
		#endregion
		
		#region manifest
		
		const int newManifestInsertPosition = 2;
		
		void FillManifestCombo()
		{
			manifestItems.Add(StringParser.Parse("${res:Dialog.ProjectOptions.ApplicationSettings.Manifest.EmbedDefault}"));
			manifestItems.Add(StringParser.Parse("${res:Dialog.ProjectOptions.ApplicationSettings.Manifest.DoNotEmbedManifest}"));
			// When a new manifest is created, it'll be inserted at this position in the list
			Debug.Assert(newManifestInsertPosition == manifestItems.Count);
			foreach (string fileName in Directory.GetFiles(base.BaseDirectory, "*.manifest")) {
				manifestItems.Add(Path.GetFileName(fileName));
			}
			manifestItems.Add(StringParser.Parse("<${res:Global.CreateButtonText}...>"));
			manifestItems.Add(StringParser.Parse("<${res:Global.BrowseText}...>"));
		}
		
		void ApplicationManifestComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Because this event is raised while the combobox is still switching to the "<create>" or "<browse>" value,
			// we cannot set comboBox.Text within this event handler.
			// To avoid this problem, we invoke the operation after the combobox has finished switching to the new value.
			Dispatcher.BeginInvoke(new Action(
				delegate {
					if (applicationManifestComboBox.SelectedIndex == applicationManifestComboBox.Items.Count - 2) {
						CreateManifest();
					} else if (applicationManifestComboBox.SelectedIndex == applicationManifestComboBox.Items.Count - 1) {
						BrowseForFile(ApplicationManifest, manifestFilter);
						// Because we're not using a binding but loading the manifest combobox manually,
						// we need to store re-load the changed property value.
						applicationManifestComboBox.Text = this.ApplicationManifest.Value;
					}
				}));
		}
		
		void CreateManifest()
		{
			string manifestFile = Path.Combine(base.BaseDirectory, "app.manifest");
			if (!File.Exists(manifestFile)) {
				string defaultManifest;
				using (Stream stream = typeof(ApplicationSettings).Assembly.GetManifestResourceStream("ICSharpCode.SharpDevelop.Resources.DefaultManifest.manifest")) {
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
			
			if (!manifestItems.Contains("app.manifest"))
				manifestItems.Insert(newManifestInsertPosition, "app.manifest");
			this.applicationManifestComboBox.SelectedIndex = manifestItems.IndexOf("app.manifest");
		}
		
		void ApplicationManifestComboBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			// We don't use binding with the application manifest combo box,
			// so we need to manually set IsDirty to true.
			this.IsDirty = true;
		}
		
		#endregion
		
		#region Win32ResourceFile
		
		void Win32ResourceComboButton_Click(object sender, RoutedEventArgs e)
		{
			BrowseForFile(Win32Resource, win32filter);
		}
		
		#endregion
	}
}
