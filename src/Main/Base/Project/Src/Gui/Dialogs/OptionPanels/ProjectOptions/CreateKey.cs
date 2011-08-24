// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    /// <summary>
    /// A form that creates keys for assembly signing.
    /// </summary>
	public class CreateKeyForm : BaseSharpDevelopForm
	{
		string baseDirectory;
		
		/// <summary>
		/// Initializes the CreateKeyFrom() dialog setting the b ase directory for adding keys to the
		/// location specified.
		/// </summary>
		/// <param name="baseDirectory">The folder for placing the key.</param>
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
		
        /// <summary>
        /// Creates a key with the sn.exe utility.
        /// </summary>
        /// <param name="keyPath">The path of the key to create.</param>
        /// <returns>True if the key was created correctly.</returns>
		public static bool CreateKey(string keyPath)
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
		
        /// <summary>
        /// Gets the path of the "strong named" executable. This is used to create keys for strongly signing
        /// .NET assemblies.
        /// </summary>
		public static string StrongNameTool {
			get {
        		return FileUtility.GetSdkPath("sn.exe");
			}
		}
	}
}
