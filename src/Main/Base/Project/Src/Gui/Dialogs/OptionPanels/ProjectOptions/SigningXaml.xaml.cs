/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 26.04.2012
 * Time: 19:56
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for SigningXaml.xaml
	/// </summary>
	public partial class SigningXaml : ProjectOptionPanel
	{
		private const string KeyFileExtensions = "*.snk;*.pfx;*.key";
		private List<string> keyFile = new List<string>();
		private MSBuildBasedProject project;
		
		
		public SigningXaml()
		{
			InitializeComponent();
		}
		
		
		private void Initialize()
		{
			FindKeys(base.BaseDirectory);
			SelectedKey = AssemblyOriginatorKeyFile.Value;
			if (SelectedKey != null) {
				if (!KeyFile.Contains(SelectedKey)) {
					keyFile.Add(SelectedKey);
				}
			}
			
			keyFile.Add(StringParser.Parse("<${res:Global.CreateButtonText}...>"));
			keyFile.Add(StringParser.Parse("<${res:Global.BrowseText}...>"));
		}
		
		public ProjectProperty<String> SignAssembly {
			get { return GetProperty("SignAssembly","false", TextBoxEditMode.EditEvaluatedProperty); }
		}
		
		
		public ProjectProperty<string> AssemblyOriginatorKeyFile {
			get { return GetProperty("AssemblyOriginatorKeyFile","", TextBoxEditMode.EditEvaluatedProperty); }
		}
		
		
		public ProjectProperty<string> DelaySign {
			get { return GetProperty("DelaySign","", TextBoxEditMode.EditEvaluatedProperty); }
		}
		
		
		public ProjectProperty<string> AssemblyOriginatorKeyMode {
			get { return GetProperty("AssemblyOriginatorKeyMode","", TextBoxEditMode.EditEvaluatedProperty); }
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
			if (signAssemblyCheckBox.IsChecked) {
				this.AssemblyOriginatorKeyFile.Value = "File";
			}
			return base.Save(project, configuration, platform);
		}
		#endregion
		
		
		#region keyFile
		
		public List<string> KeyFile {
			get { return keyFile; }
			set {
				keyFile = value;
				base.RaisePropertyChanged(() => KeyFile);
			}
		}
		
		public string SelectedKey {get;set;}
		
		void FindKeys(string directory)
		{
			directory = FileUtility.NormalizePath(directory);
			while (true) {
				try {
//					var files = from file in new DirectoryInfo(@"C:\").GetFiles()
//            where file..Name.StartsWith("_")
//            select file;

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
				BrowseKeyFile();
			}
			if (cbo.SelectedIndex == keyFile.Count - 2) {
				CreateKeyFile();
			}
		}
		
		
		void BrowseKeyFile()
		{
			string str = OptionsHelper.OpenFile("${res:SharpDevelop.FileFilter.KeyFiles} (" + KeyFileExtensions + ")|" + KeyFileExtensions + "|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
			if (!String.IsNullOrEmpty(str)) {
				this.AssemblyOriginatorKeyFile.Value = str;
			}                                                        
		}
		
		
		private void CreateKeyFile()
		{
			if (File.Exists(CreateKeyXaml.StrongNameTool)) {
				CreateKeyXaml createKey = new CreateKeyXaml(base.BaseDirectory);
				createKey.KeyFile = project.Name;
				createKey.ShowDialog();
				if (createKey.DialogResult.HasValue && createKey.DialogResult.Value)
				{
					SelectedKey = MSBuildInternals.Escape(createKey.KeyFile);
				} else{
					SelectedKey = String.Empty;
				}
			} else {
				MessageService.ShowMessage("${res:Dialog.ProjectOptions.Signing.SNnotFound}");
			}
		}
		
		#endregion	
	}
}