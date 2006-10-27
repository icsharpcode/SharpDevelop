// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class CreateKeyForm : BaseSharpDevelopForm
	{
		string baseDirectory;
		
		public CreateKeyForm(string baseDirectory)
		{
			this.baseDirectory = baseDirectory;
			SetupFromXmlResource("ProjectOptions.CreateKey.xfrm");
			Get<CheckBox>("usePassword").CheckedChanged += delegate {
				ControlDictionary["passwordPanel"].Enabled = Get<CheckBox>("usePassword").Checked;
			};
			ControlDictionary["okButton"].Click += OkButtonClick;
		}
		
		void OkButtonClick(object sender, EventArgs e)
		{
			KeyFile = KeyFile.Trim();
			if (KeyFile.Length == 0) {
				MessageService.ShowMessage("${res:Dialog.ProjectOptions.Signing.EnterKeyName}");
				return;
			}
			bool usePassword = Get<CheckBox>("usePassword").Checked;
			if (usePassword) {
				if (!CheckPassword(ControlDictionary["passwordTextBox"],
				                   ControlDictionary["confirmPasswordTextBox"]))
				{
					return;
				}
				MessageService.ShowMessage("Creating a key file with a password is currently not supported.");
				return;
			}
			if (!KeyFile.EndsWith(".snk") && !KeyFile.EndsWith(".pfx"))
				KeyFile += ".snk";
			if (CreateKey(Path.Combine(baseDirectory, KeyFile))) {
				this.DialogResult = DialogResult.OK;
				Close();
			}
		}
		
		public static bool CreateKey(string keyPath)
		{
			if (File.Exists(keyPath)) {
				string question = "${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteQuestion}";
				question = StringParser.Parse(question, new string[,] {{"fileName", keyPath}});
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
		
		public string KeyFile {
			get {
				return ControlDictionary["keyFileTextBox"].Text;
			}
			set {
				ControlDictionary["keyFileTextBox"].Text = value;
			}
		}
		
		public static bool CheckPassword(Control password, Control confirm)
		{
			password.Text = password.Text.Trim();
			confirm.Text = confirm.Text.Trim();
			if (password.Text.Length < 6) {
				MessageService.ShowMessage("${res:Dialog.ProjectOptions.Signing.PasswordTooShort}");
				password.Focus();
				return false;
			}
			if (password.Text != confirm.Text) {
				MessageService.ShowMessage("${res:Dialog.ProjectOptions.Signing.PasswordsDontMatch}");
				return false;
			}
			return true;
		}
		
		public static string StrongNameTool {
			get {
				return FileUtility.NetSdkInstallRoot + "bin\\sn.exe";
			}
		}
	}
}
