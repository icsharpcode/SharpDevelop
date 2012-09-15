/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 26.04.2012
 * Time: 19:56
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for SigningXaml.xaml
	/// </summary>
	public partial class Signing : ProjectOptionPanel
	{
		private const string KeyFileExtensions = "*.snk;*.pfx;*.key";
		private string selectedKey;
		private ObservableCollection<string> keyFile = new ObservableCollection<string>();
		
		public Signing()
		{
			InitializeComponent();
		}
		
		
		private void Initialize()
		{
			FindKeys(base.BaseDirectory);
			SelectedKey = AssemblyOriginatorKeyFile.Value.Trim();
			if (SelectedKey.Length >  0) {
				if (!KeyFile.Contains(SelectedKey)) {
					keyFile.Add(SelectedKey);
				}
			}
			keyFile.Add(StringParser.Parse("<${res:Global.CreateButtonText}...>"));
			keyFile.Add(StringParser.Parse("<${res:Global.BrowseText}...>"));
			keyFileComboBox.SelectedIndex = 0;
			keyFileComboBox.SelectionChanged += KeyFileComboBox_SelectionChanged;
			IsDirty = false;
		}
		
		public ProjectProperty<bool> SignAssembly {
			get { return GetProperty("SignAssembly", false); }
		}
		
		
		public ProjectProperty<string> AssemblyOriginatorKeyFile {
			get { return GetProperty("AssemblyOriginatorKeyFile","", TextBoxEditMode.EditRawProperty); }
		}
		
		
		public ProjectProperty<bool> DelaySign {
			get { return GetProperty("DelaySign", false); }
		}
		
		
		public ProjectProperty<string> AssemblyOriginatorKeyMode {
			get { return GetProperty("AssemblyOriginatorKeyMode","", TextBoxEditMode.EditEvaluatedProperty); }
		}
		
		#region overrides
		
		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			base.Load(project, configuration, platform);
			Initialize();
		}
		
		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			/*
			helper.Saved += delegate {
				if (Get<CheckBox>("signAssembly").Checked) {
					helper.SetProperty("AssemblyOriginatorKeyMode", "File", true, signAssemblyBinding.Location);
				}
			};
			 */
			if (signAssemblyCheckBox.IsChecked == true) {
				this.AssemblyOriginatorKeyFile.Value = "File";
			}
			keyFileComboBox.SelectionChanged -= KeyFileComboBox_SelectionChanged;
			return base.Save(project, configuration, platform);
		}
		#endregion
		
		
		#region KeyFile
		
		public ObservableCollection<string> KeyFile {
			get { return keyFile; }
			set {
				keyFile = value;
				base.RaisePropertyChanged(() => KeyFile);
			}
		}
		
		
		public string SelectedKey {
			get { return selectedKey; }
			set { selectedKey = value;
				base.RaisePropertyChanged(() => SelectedKey);
			}
		}
		
		
		void FindKeys(string directory)
		{
			directory = FileUtility.NormalizePath(directory);
			while (true) {
				try {
					foreach (string fileName in Directory.GetFiles(directory, "*.snk")) {
						keyFile.Add(MSBuildInternals.Escape(FileUtility.GetRelativePath(base.BaseDirectory, fileName)));
					}
					foreach (string fileName in Directory.GetFiles(directory, "*.pfx")) {
						keyFile.Add(MSBuildInternals.Escape(FileUtility.GetRelativePath(base.BaseDirectory, fileName)));
					}
					foreach (string fileName in Directory.GetFiles(directory, "*.key")) {
						keyFile.Add(MSBuildInternals.Escape(FileUtility.GetRelativePath(base.BaseDirectory, fileName)));
					}
				} catch {
					// can happen for networked drives / network locations
					break;
				}
				int pos = directory.LastIndexOf(Path.DirectorySeparatorChar);
				if (pos < 0)
					break;
				directory = directory.Substring(0, pos);
			}
		}
		
		
		void KeyFileComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var cbo = (ComboBox) sender;
			
			if (cbo.SelectedIndex == keyFile.Count - 1) {
				BrowseForFile(this.AssemblyOriginatorKeyFile, "${res:SharpDevelop.FileFilter.KeyFiles} (" + KeyFileExtensions + ")|" + KeyFileExtensions + "|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			}
			if (cbo.SelectedIndex == keyFile.Count - 2) {
				CreateKeyFile();
			}
		}
		
		
		private void CreateKeyFile()
		{
			if (File.Exists(StrongNameTool)) {
				string title = StringParser.Parse("${res:Dialog.ProjectOptions.Signing.CreateKey.Title}");
				string str = StringParser.Parse("${res:Dialog.ProjectOptions.Signing.CreateKey.KeyName}");
				var keyName = str.Remove(str.IndexOf("&"),1);
				
				string key = MessageService.ShowInputBox(title,keyName,base.Project.Name);
				if(!String.IsNullOrEmpty(key)) {
					if (CreateKey(key.Trim())) {
						this.keyFileComboBox.SelectionChanged -= KeyFileComboBox_SelectionChanged;
						var generated = MSBuildInternals.Escape(key);
						KeyFile.Add(generated);
						SelectedKey = generated;
						this.keyFileComboBox.SelectionChanged += KeyFileComboBox_SelectionChanged;
					}
				} else {
					MessageService.ShowMessage("${res:Dialog.ProjectOptions.Signing.SNnotFound}");
				}
			}
		}
		
		#endregion
		
		/// <summary>
		/// Gets the path of the "strong named" executable. This is used to create keys for strongly signing
		/// .NET assemblies.
		/// </summary>
		private static string StrongNameTool {
			get {
				return FileUtility.GetSdkPath("sn.exe");
			}
		}
		
		
		/// <summary>
		/// Creates a key with the sn.exe utility.
		/// </summary>
		/// <param name="keyPath">The path of the key to create.</param>
		/// <returns>True if the key was created correctly.</returns>
		private static bool CreateKey(string keyPath)
		{
			if (File.Exists(keyPath)) {
				string question = "${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteQuestion}";
				question = StringParser.Parse(question, new StringTagPair("fileNames", keyPath));
				if (!MessageService.AskQuestion(question, "${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteQuestion.InfoName}")) {
					return false;
				}
			}
			Process p = Process.Start(StrongNameTool, "-k \"" + keyPath + "\"");
			p.WaitForExit();
			if (p.ExitCode != 0) {
				MessageService.ShowMessage("${res:Dialog.ProjectOptions.Signing.ErrorCreatingKey}");
				return false;
			}
			return true;
		}
	}
}