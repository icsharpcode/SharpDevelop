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
	public class ClientCertPassphraseDialog : BaseSharpDevelopForm
	{
		public string Realm {
			get {
				return ControlDictionary["realmLabel"].Text;
			}
			set {
				ControlDictionary["realmLabel"].Text = value;
			}
		}
		
		public string Passphrase {
			get {
				return ControlDictionary["passPhraseTextBox"].Text;
			}
			set {
				ControlDictionary["passPhraseTextBox"].Text = value;
			}
		}
		
		public bool MaySave {
			get {
				return ((CheckBox)ControlDictionary["saveCredentialsCheckBox"]).Checked;
			}
			set {
				((CheckBox)ControlDictionary["saveCredentialsCheckBox"]).Checked = value;
			}
		}
		
		public SvnAuthCredSslClientCertPw CreateCredential(AprPool pool)
		{
			SvnAuthCredSslClientCertPw cred = SvnAuthCredSslClientCertPw.Alloc(pool);
			cred.CertFile = new SvnPath(Passphrase, pool); // this should be cred.Password, the property is named incorrectly in Svn.Net
			cred.MaySave  = MaySave;
			return cred;
		}
		
		public ClientCertPassphraseDialog(string realm, bool maySave)
		{
			SetupFromXmlStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("ICSharpCode.Svn.Resources.ClientCertPassphraseDialog.xfrm"));
			this.Realm   = realm;
			this.MaySave = maySave;
		}
	}
}
