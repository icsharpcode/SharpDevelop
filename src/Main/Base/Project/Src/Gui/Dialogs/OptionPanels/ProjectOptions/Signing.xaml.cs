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
	
		public Signing()
		{
			InitializeComponent();
		}
		
		
		public ProjectProperty<bool> SignAssembly {
			get { return GetProperty("SignAssembly", false); }
		}
		
		
		public ProjectProperty<bool> DelaySign {
			get { return GetProperty("DelaySign", false); }
		}
		
		
		public ProjectProperty<string> AssemblyOriginatorKeyFile {
			get { return GetProperty("AssemblyOriginatorKeyFile","", TextBoxEditMode.EditRawProperty); }
		}
		
		
		public ProjectProperty<string> AssemblyOriginatorKeyMode {
			get { return GetProperty("AssemblyOriginatorKeyMode","", TextBoxEditMode.EditEvaluatedProperty); }
		}
		
		#region overrides
		
	
		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			base.Load(project, configuration, platform);
			// Ensure that the template is applied before we assign ComboBox.Text, this ensures
			// the TextBoxBase.TextChanged event fires immediately and doesn't cause us to
			// set the IsDirty flag later.
			keyFileComboBox.ApplyTemplate();
			FillKeyFileComboBox();
			if (KeyFile.Count > 2) {
				if (!String.IsNullOrEmpty(this.AssemblyOriginatorKeyFile.Value)) {
					keyFileComboBox.Text = this.AssemblyOriginatorKeyFile.Value;
				}
			}
		}
		
		
		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			if (signAssemblyCheckBox.IsChecked == true) {
				this.AssemblyOriginatorKeyMode.Value = "File";
			}
			return base.Save(project, configuration, platform);
		}
		
		#endregion
		
		
		#region KeyFile
		
		private ObservableCollection<string> keyFile = new ObservableCollection<string>();
		
		public ObservableCollection<string> KeyFile {
			get { return keyFile; }
			set {
				keyFile = value;
				base.RaisePropertyChanged(() => KeyFile);
			}
		}
		
		
		private void FillKeyFileComboBox()
		{
			FindKeys(base.BaseDirectory);
			keyFile.Add(StringParser.Parse("<${res:Global.CreateButtonText}...>"));
			keyFile.Add(StringParser.Parse("<${res:Global.BrowseText}...>"));
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
			// Because this event is raised while the combobox is still switching to the "<create>" or "<browse>" value,
			// we cannot set comboBox.Text within this event handler.
			// To avoid this problem, we invoke the operation after the combobox has finished switching to the new value.
			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					if (this.keyFileComboBox.SelectedIndex == keyFile.Count - 1) {
						keyFileComboBox.Text = String.Empty;
						BrowseForFile(this.AssemblyOriginatorKeyFile, "${res:SharpDevelop.FileFilter.KeyFiles} (" + KeyFileExtensions + ")|" + KeyFileExtensions + "|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
						keyFileComboBox.Text = this.AssemblyOriginatorKeyFile.Value;
					}
					if (this.keyFileComboBox.SelectedIndex == keyFile.Count - 2) {
						keyFileComboBox.Text = String.Empty;
						CreateKeyFile();
					}
				});
		}
		
		private void CreateKeyFile()
		{
			if (File.Exists(StrongNameTool)) {
				string title = StringParser.Parse("${res:Dialog.ProjectOptions.Signing.CreateKey.Title}");
				string str = StringParser.Parse("${res:Dialog.ProjectOptions.Signing.CreateKey.KeyName}");
				var keyName = str.Remove(str.IndexOf("&"),1);
				
				string key = MessageService.ShowInputBox(title,keyName,base.Project.Name);
				if(!String.IsNullOrEmpty(key)) {
					if (!key.EndsWith(".snk") && !key.EndsWith(".pfx"))
						key += ".snk";
					if (CreateKey(Path.Combine(base.Project.Directory, key))) {
						AssemblyOriginatorKeyFile.Value = MSBuildInternals.Escape(key);
						keyFileComboBox.Text = AssemblyOriginatorKeyFile.Value;
					}
				}
			} else {
				MessageService.ShowMessage("${res:Dialog.ProjectOptions.Signing.SNnotFound}");
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