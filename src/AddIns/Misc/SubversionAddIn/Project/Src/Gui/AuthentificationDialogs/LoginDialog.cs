// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui.XmlForms;
using PumaCode.SvnDotNet.AprSharp;
using PumaCode.SvnDotNet.SubversionSharp;

namespace ICSharpCode.Svn.Gui
{
	/// <summary>
	/// Description of LoginDialog.
	/// </summary>
	public class LoginDialog : BaseSharpDevelopForm
	{
		public bool MaySave {
			get {
				return ((CheckBox)ControlDictionary["saveCredentialsCheckBox"]).Checked;
			}
			set {
				((CheckBox)ControlDictionary["saveCredentialsCheckBox"]).Checked = value;
			}
		}
		
		public string Realm {
			get {
				return ControlDictionary["realmLabel"].Text;
			}
			set {
				ControlDictionary["realmLabel"].Text = value;
			}
		}
		
		public string UserName {
			get {
				return ControlDictionary["userNameTextBox"].Text;
			}
			set {
				ControlDictionary["userNameTextBox"].Text = value;
			}
		}
		
		public SvnAuthCredSimple CreateCredential(AprPool pool)
		{
			SvnAuthCredSimple cred = SvnAuthCredSimple.Alloc(pool);
			cred.Username = new AprString(UserName, pool);
			cred.Password = new AprString(Password, pool);
			cred.MaySave = MaySave;
			return cred;
		}
		
		string Password {
			get {
				return ControlDictionary["pwd1TextBox"].Text;
			}
		}
		
		string ReTypedPassword {
			get {
				return ControlDictionary["pwd2TextBox"].Text;
			}
		}
		
		bool ShowPasswords {
			get {
				return ((CheckBox)ControlDictionary["showPasswordCheckBox"]).Checked;
			}
		}
		
		public LoginDialog(string realm, string userName, bool maySave)
		{
			SetupFromXmlStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("ICSharpCode.Svn.Resources.LoginDialog.xfrm"));
			this.UserName = userName;
			this.Realm    = realm;
			this.MaySave  = maySave;
			((CheckBox)ControlDictionary["showPasswordCheckBox"]).CheckedChanged += new EventHandler(ShowPasswordCheckBoxCheckedChanged);
			((TextBox)ControlDictionary["pwd1TextBox"]).PasswordChar = '*';
			((TextBox)ControlDictionary["pwd2TextBox"]).PasswordChar = '*';
			
			((TextBox)ControlDictionary["pwd1TextBox"]).TextChanged += new EventHandler(PasswordTextChanged);
			((TextBox)ControlDictionary["pwd2TextBox"]).TextChanged += new EventHandler(PasswordTextChanged);
			
			ControlDictionary["okButton"].Click += new EventHandler(OkButtonClicked);
		}
		
		void ShowPasswordCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			if (ShowPasswords) {
				((TextBox)ControlDictionary["pwd1TextBox"]).PasswordChar = '\0';
				((TextBox)ControlDictionary["pwd2TextBox"]).Enabled = false;
			} else {
				((TextBox)ControlDictionary["pwd1TextBox"]).PasswordChar = '*';
				((TextBox)ControlDictionary["pwd2TextBox"]).Enabled = true;
			}
		}
		
		void PasswordTextChanged(object sender, EventArgs e)
		{
			ControlDictionary["okButton"].Enabled = ShowPasswords || Password == ReTypedPassword;
		}
		
		void OkButtonClicked(object sender, EventArgs e)
		{
			bool done = false;
			if (ShowPasswords) {
				done = UserName.Length > 0 && Password.Length > 0;
			} else {
				done = Password == ReTypedPassword;
			}
			
			if (done) {
				DialogResult = DialogResult.OK;
			}
		}
		
	}
}
