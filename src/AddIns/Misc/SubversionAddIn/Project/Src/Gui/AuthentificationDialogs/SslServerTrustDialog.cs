// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
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
	public class SslServerTrustDialog : BaseSharpDevelopForm
	{
		SvnAuthSslServerCertInfo certificateInfo;
		SvnAuthCredSslServerTrust.CertFailures failures;
		
		public SvnAuthSslServerCertInfo CertificateInfo {
			get {
				return certificateInfo;
			}
			set {
				certificateInfo = value;
				UpdateCertificateInfo();
			}
		}
		
		public SvnAuthCredSslServerTrust.CertFailures Failures {
			get {
				return failures;
			}
			set {
				failures = value;
				UpdateFailures();
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
		
		public SvnAuthCredSslServerTrust CreateCredential(AprPool pool)
		{
			SvnAuthCredSslServerTrust cred = SvnAuthCredSslServerTrust.Alloc(pool);
			cred.AcceptedFailures = failures;
			cred.MaySave = MaySave;
			return cred;
		}
		
		public SslServerTrustDialog(SvnAuthSslServerCertInfo certificateInfo, SvnAuthCredSslServerTrust.CertFailures failures, bool maySave)
		{
			SetupFromXmlStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("ICSharpCode.Svn.Resources.SslServerTrustDialog.xfrm"));
			this.CertificateInfo = certificateInfo;
			this.Failures        = failures;
			this.MaySave         = maySave;
		}
		
		void UpdateCertificateInfo()
		{
			if (!certificateInfo.IsNull) {
				ControlDictionary["hostNameLabel"].Text      = certificateInfo.Hostname.Value;
				ControlDictionary["fingerPrintlabel"].Text   = certificateInfo.Fingerprint.Value;
				ControlDictionary["validLabel"].Text         = "From " + certificateInfo.ValidFrom + " to " + certificateInfo.ValidUntil;
				ControlDictionary["issuerLabel"].Text        = certificateInfo.IssuerDName.Value;
				ControlDictionary["certificateTextBox"].Text = certificateInfo.AsciiCert.Value;
			} else {
				ControlDictionary["hostNameLabel"].Text      = String.Empty;
				ControlDictionary["fingerPrintlabel"].Text   = String.Empty;
				ControlDictionary["validLabel"].Text         = String.Empty;
				ControlDictionary["issuerLabel"].Text        = String.Empty;
				ControlDictionary["certificateTextBox"].Text = String.Empty;
			}
		}
		
		bool HasFailures(SvnAuthCredSslServerTrust.CertFailures testFailures)
		{
			return (failures & testFailures) == testFailures;
		}
		
		void UpdateFailures()
		{
			if (HasFailures(SvnAuthCredSslServerTrust.CertFailures.UnknownCA)) {
				ControlDictionary["certificateAuthorityStatusLabel"].Text      = "The issuing certificate authority(CA) is not trusted.";
				ControlDictionary["certificateAuthorityStatusLabel"].ForeColor = Color.Red;
			} else {
				ControlDictionary["certificateAuthorityStatusLabel"].Text      = "The issuing certificate authority(CA) is known and trusted.";
				ControlDictionary["certificateAuthorityStatusLabel"].ForeColor = Color.Green;
			}
			
			if (HasFailures(SvnAuthCredSslServerTrust.CertFailures.CNMismatch)) {
				ControlDictionary["certificateNameStatusLabel"].Text      = "The certificate's hostname does not match the hostname of the server.";
				ControlDictionary["certificateNameStatusLabel"].ForeColor = Color.Red;
			} else {
				ControlDictionary["certificateNameStatusLabel"].Text      = "The certificate's hostname matches the hostname of the server.";
				ControlDictionary["certificateNameStatusLabel"].ForeColor = Color.Green;
			}
			
			if (HasFailures(SvnAuthCredSslServerTrust.CertFailures.Expired)) {
				ControlDictionary["certificateDateStatusLabel"].Text      = "The server certificate has expired.";
				ControlDictionary["certificateDateStatusLabel"].ForeColor = Color.Red;
			} else if (HasFailures(SvnAuthCredSslServerTrust.CertFailures.NotYetValid)) {
				ControlDictionary["certificateDateStatusLabel"].Text      = "The server certificate is not yet valid.";
				ControlDictionary["certificateDateStatusLabel"].ForeColor = Color.Red;
			} else {
				ControlDictionary["certificateDateStatusLabel"].Text      = "The server certificate date is valid.";
				ControlDictionary["certificateDateStatusLabel"].ForeColor = Color.Green;
			}
		}
	}
}
