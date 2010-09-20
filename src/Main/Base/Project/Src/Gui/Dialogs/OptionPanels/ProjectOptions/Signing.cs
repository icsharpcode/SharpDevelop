// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class Signing : AbstractXmlFormsProjectOptionPanel
	{
		ComboBox keyFile;
		ConfigurationGuiBinding signAssemblyBinding;
		
		const string KeyFileExtensions = "*.snk;*.pfx;*.key";
		
		public override void LoadPanelContents()
		{
			SetupFromXmlResource("ProjectOptions.Signing.xfrm");
			InitializeHelper();
			
			ConfigurationGuiBinding b;
			ChooseStorageLocationButton locationButton;
			
			signAssemblyBinding = helper.BindBoolean("signAssemblyCheckBox", "SignAssembly", false);
			locationButton = signAssemblyBinding.CreateLocationButtonInPanel("signingGroupBox");
			Get<CheckBox>("signAssembly").CheckedChanged += new EventHandler(UpdateEnabledStates);
			
			
			keyFile = Get<ComboBox>("keyFile");
			b = helper.BindString(keyFile, "AssemblyOriginatorKeyFile", TextBoxEditMode.EditRawProperty);
			b.RegisterLocationButton(locationButton);
			FindKeys(baseDirectory);
			if (keyFile.Text.Length > 0) {
				if (!keyFile.Items.Contains(keyFile.Text)) {
					keyFile.Items.Add(keyFile.Text);
				}
			}
			keyFile.Items.Add(StringParser.Parse("<${res:Global.CreateButtonText}...>"));
			keyFile.Items.Add(StringParser.Parse("<${res:Global.BrowseText}...>"));
			keyFile.SelectedIndexChanged += delegate {
				if (keyFile.SelectedIndex == keyFile.Items.Count - 1) {
					BeginInvoke(new MethodInvoker(BrowseKeyFile));
				}
				if (keyFile.SelectedIndex == keyFile.Items.Count - 2) {
					BeginInvoke(new MethodInvoker(CreateKeyFile));
				}
			};
			
			b = helper.BindBoolean("delaySignOnlyCheckBox", "DelaySign", false);
			b.RegisterLocationButton(locationButton);
			
			UpdateEnabledStates(this, EventArgs.Empty);
			
			helper.AddConfigurationSelector(this);
			
			helper.Saved += delegate {
				if (Get<CheckBox>("signAssembly").Checked) {
					helper.SetProperty("AssemblyOriginatorKeyMode", "File", true, signAssemblyBinding.Location);
				}
			};
		}
		
		void FindKeys(string directory)
		{
			directory = FileUtility.NormalizePath(directory);
			while (true) {
				try {
					foreach (string fileName in Directory.GetFiles(directory, "*.snk")) {
						keyFile.Items.Add(MSBuildInternals.Escape(FileUtility.GetRelativePath(baseDirectory, fileName)));
					}
					foreach (string fileName in Directory.GetFiles(directory, "*.pfx")) {
						keyFile.Items.Add(MSBuildInternals.Escape(FileUtility.GetRelativePath(baseDirectory, fileName)));
					}
					foreach (string fileName in Directory.GetFiles(directory, "*.key")) {
						keyFile.Items.Add(MSBuildInternals.Escape(FileUtility.GetRelativePath(baseDirectory, fileName)));
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
		
		void BrowseKeyFile()
		{
			keyFile.SelectedIndex = -1;
			BrowseForFile(ControlDictionary["keyFileComboBox"], "${res:SharpDevelop.FileFilter.KeyFiles} (" + KeyFileExtensions + ")|" + KeyFileExtensions + "|${res:SharpDevelop.FileFilter.AllFiles}|*.*", TextBoxEditMode.EditRawProperty);
		}
		
		void CreateKeyFile()
		{
			if (File.Exists(CreateKeyForm.StrongNameTool)) {
				using (CreateKeyForm createKey = new CreateKeyForm(baseDirectory)) {
					createKey.KeyFile = project.Name;
					if (createKey.ShowDialog(WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
						keyFile.Text = MSBuildInternals.Escape(createKey.KeyFile);
						return;
					}
				}
			} else {
				MessageService.ShowMessage("${res:Dialog.ProjectOptions.Signing.SNnotFound}");
			}
			keyFile.Text = "";
		}
		
		void UpdateEnabledStates(object sender, EventArgs e)
		{
			ControlDictionary["strongNameSignPanel"].Enabled = Get<CheckBox>("signAssembly").Checked;
			
			Get<Button>("changePassword").Enabled = false;
		}
	}
}
