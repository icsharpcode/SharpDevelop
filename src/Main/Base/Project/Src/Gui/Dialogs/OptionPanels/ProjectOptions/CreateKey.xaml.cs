/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 01.06.2012
 * Time: 20:20
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for CreateKeyXaml.xaml
	/// </summary>
	public partial class CreateKey : Window,INotifyPropertyChanged
	{
		
		private bool checkBoxChecked;
		private string keyFile;
		private string baseDirectory;
		
		public CreateKey()
		{
			InitializeComponent();
			DataContext = this;
		}
		
		public CreateKey (string baseDirectory):this()
		{
			this.baseDirectory = baseDirectory;
		}
		
		
		public bool CheckBoxChecked {
			get { return checkBoxChecked; }
			set { checkBoxChecked = value;
				OnPropertyChange("CheckBoxChecked");
			}
		}
		
		
		public string KeyFile {
			get { return keyFile; }
			set { keyFile = value;
				OnPropertyChange("KeyFile");			}
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
        
        
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		private void OnPropertyChange (string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
		
		void okButtonClick(object sender, RoutedEventArgs e)
		{
			KeyFile = KeyFile.Trim();
			if (KeyFile.Length == 0) {
				MessageService.ShowMessage("${res:Dialog.ProjectOptions.Signing.EnterKeyName}");
				return;
			}
			
			if (CheckBoxChecked) {
				if (!CheckPassword(this.passwordTextBox.Text.Trim(),this.confirmPasswordTextBox.Text.Trim()))
				{
					return;
				}
				MessageService.ShowMessage("Creating a key file with a password is currently not supported.");
				return;
			}
			if (!KeyFile.EndsWith(".snk") && !KeyFile.EndsWith(".pfx"))
				KeyFile += ".snk";
			if (CreateKeyInternal(Path.Combine(baseDirectory, KeyFile))) {
				this.DialogResult = true;
				Close();
			}
		}
		
		 /// <summary>
        /// Creates a key with the sn.exe utility.
        /// </summary>
        /// <param name="keyPath">The path of the key to create.</param>
        /// <returns>True if the key was created correctly.</returns>
		private static bool CreateKeyInternal(string keyPath)
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
		
		
		public static bool CheckPassword(string password, string confirm)
		{
			
//			if (password.Text.Length < 6) {
//				MessageService.ShowMessage("${res:Dialog.ProjectOptions.Signing.PasswordTooShort}");
//				password.Focus();
//				return false;
//			}
//			if (password.Text != confirm.Text) {
//				MessageService.ShowMessage("${res:Dialog.ProjectOptions.Signing.PasswordsDontMatch}");
//				return false;
//			}
			return true;
		}
		
		
//		private static bool CheckPassword(Control password, Control confirm)
//		{
//			password.Text = password.Text.Trim();
//			confirm.Text = confirm.Text.Trim();
//			if (password.Text.Length < 6) {
//				MessageService.ShowMessage("${res:Dialog.ProjectOptions.Signing.PasswordTooShort}");
//				password.Focus();
//				return false;
//			}
//			if (password.Text != confirm.Text) {
//				MessageService.ShowMessage("${res:Dialog.ProjectOptions.Signing.PasswordsDontMatch}");
//				return false;
//			}
//			return true;
//		}
		
		
		void cancelButtonClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			Close();
		}
		
		
	}
}