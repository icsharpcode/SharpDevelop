// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Drawing;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using NSvn.Common;
using NSvn.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.Svn.Gui
{
	/// <summary>
	/// Description of LoginDialog.
	/// </summary>
	public class ClientCertDialog : BaseSharpDevelopForm
	{
		public string Realm {
			get {
				return ControlDictionary["realmLabel"].Text;
			}
			set {
				ControlDictionary["realmLabel"].Text = value;
			}
		}
		
		public string FileName {
			get {
				return ControlDictionary["pathTextBox"].Text;
			}
			set {
				ControlDictionary["pathTextBox"].Text = value;
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
		
		public SslClientCertificateCredential Credential {
			get {
				SslClientCertificateCredential cred = new SslClientCertificateCredential();
				cred.CertificateFile = FileName;
				cred.MaySave         = MaySave;
				return cred;
			}
		}
			
		public ClientCertDialog(string realm, bool maySave)
		{
			SetupFromXmlStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("ICSharpCode.Svn.Resources.ClientCertDialog.xfrm"));
			this.Realm   = realm;
			this.MaySave = maySave;
		}
	}
}
