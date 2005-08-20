// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class Signing : AbstractProjectOptionPanel
	{
		ComboBox keyFile;
		
		const string KeyFileExtensions = "*.snk;*.pfx;*.key";
		
		public override void LoadPanelContents()
		{
			SetupFromXmlResource("ProjectOptions.Signing.xfrm");
			InitializeHelper();
			
			helper.BindBoolean("signAssemblyCheckBox", "SignAssembly", false);
			Get<CheckBox>("signAssembly").CheckedChanged += new EventHandler(UpdateEnabledStates);
			
			
			keyFile = Get<ComboBox>("keyFile");
			helper.BindString(keyFile, "AssemblyOriginatorKeyFile");
			if (keyFile.Text.Length > 0) {
				FindKeys(baseDirectory);
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
			
			helper.BindBoolean("delaySignOnlyCheckBox", "DelaySign", false);
			
			UpdateEnabledStates(this, EventArgs.Empty);
		}
		
		void FindKeys(string directory)
		{
			directory = Path.GetFullPath(directory);
			foreach (string fileName in Directory.GetFiles(directory, "*.snk")) {
				keyFile.Items.Add(FileUtility.GetRelativePath(baseDirectory, fileName));
			}
			foreach (string fileName in Directory.GetFiles(directory, "*.pfx")) {
				keyFile.Items.Add(FileUtility.GetRelativePath(baseDirectory, fileName));
			}
			foreach (string fileName in Directory.GetFiles(directory, "*.key")) {
				keyFile.Items.Add(FileUtility.GetRelativePath(baseDirectory, fileName));
			}
			if (directory.Length > 3) {
				FindKeys(Path.Combine(directory, ".."));
			}
		}
		
		void BrowseKeyFile()
		{
			keyFile.SelectedIndex = -1;
			new BrowseButtonEvent(this, "keyFileComboBox", "${res:SharpDevelop.FileFilter.KeyFiles} (" + KeyFileExtensions + ")|" + KeyFileExtensions + "|${res:SharpDevelop.FileFilter.AllFiles}|*.*").Event(this, EventArgs.Empty);
		}
		
		void CreateKeyFile()
		{
			if (File.Exists(CreateKeyForm.StrongNameTool)) {
				using (CreateKeyForm createKey = new CreateKeyForm(baseDirectory)) {
					createKey.KeyFile = project.Name;
					if (createKey.ShowDialog(WorkbenchSingleton.MainForm) == DialogResult.OK) {
						keyFile.Text = createKey.KeyFile;
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
		
		public override bool StorePanelContents()
		{
			if (IsDirty && Get<CheckBox>("signAssembly").Checked) {
				helper.SetProperty("AssemblyOriginatorKeyMode", "File");
			}
			return base.StorePanelContents();
		}
	}
}
