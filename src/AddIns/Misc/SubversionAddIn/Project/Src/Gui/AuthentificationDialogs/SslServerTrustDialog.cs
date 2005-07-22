// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
	public class SslServerTrustDialog : BaseSharpDevelopForm
	{
		SslServerCertificateInfo certificateInfo;
		SslFailures              failures;
		
		public SslServerCertificateInfo CertificateInfo {
			get {
				return certificateInfo;
			}
			set {
				certificateInfo = value;
				UpdateCertificateInfo();
			}
		}
		
		public SslFailures Failures {
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
		
		public SslServerTrustCredential Credential {
			get {
				SslServerTrustCredential cred = new SslServerTrustCredential();
				cred.AcceptedFailures = failures;
				cred.MaySave = MaySave;
				return cred;
			}
		}
		
		public SslServerTrustDialog(SslServerCertificateInfo certificateInfo, SslFailures failures, bool maySave)
		{
			SetupFromXmlStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("ICSharpCode.Svn.Resources.SslServerTrustDialog.xfrm"));
			this.CertificateInfo = certificateInfo;
			this.Failures        = failures;
			this.MaySave         = maySave;
		}
		
		void UpdateCertificateInfo()
		{
			if (certificateInfo != null) {
				ControlDictionary["hostNameLabel"].Text      = certificateInfo.HostName;
				ControlDictionary["fingerPrintlabel"].Text   = certificateInfo.FingerPrint;
				ControlDictionary["validLabel"].Text         = "From " + certificateInfo.ValidFrom + " to " + certificateInfo.ValidUntil;
				ControlDictionary["issuerLabel"].Text        = certificateInfo.Issuer;
				ControlDictionary["certificateTextBox"].Text = certificateInfo.AsciiCertificate;
			} else {
				ControlDictionary["hostNameLabel"].Text      = String.Empty;
				ControlDictionary["fingerPrintlabel"].Text   = String.Empty;
				ControlDictionary["validLabel"].Text         = String.Empty;
				ControlDictionary["issuerLabel"].Text        = String.Empty;
				ControlDictionary["certificateTextBox"].Text = String.Empty;
			}
		}
		
		bool HasFailures(SslFailures testFailures)
		{
			return (failures & testFailures) == testFailures;
		}
		
		void UpdateFailures()
		{
			if (HasFailures(SslFailures.CertificateAuthorityUnknown)) {
				ControlDictionary["certificateAuthorityStatusLabel"].Text      = "The issuing certificate authority(CA) is not trusted.";
				ControlDictionary["certificateAuthorityStatusLabel"].ForeColor = Color.Red;
			} else {
				ControlDictionary["certificateAuthorityStatusLabel"].Text      = "The issuing certificate authority(CA) is known and trusted.";
				ControlDictionary["certificateAuthorityStatusLabel"].ForeColor = Color.Green;
			}
			
			if (HasFailures(SslFailures.CertificateNameMismatch)) {
				ControlDictionary["certificateNameStatusLabel"].Text      = "The certificate's hostname does not match the hostname of the server.";
				ControlDictionary["certificateNameStatusLabel"].ForeColor = Color.Red;
			} else {
				ControlDictionary["certificateNameStatusLabel"].Text      = "The certificate's hostname matches the hostname of the server.";
				ControlDictionary["certificateNameStatusLabel"].ForeColor = Color.Green;
			}
			
			if (HasFailures(SslFailures.Expired)) {
				ControlDictionary["certificateDateStatusLabel"].Text      = "The server certificate has expired.";
				ControlDictionary["certificateDateStatusLabel"].ForeColor = Color.Red;
			} else if (HasFailures(SslFailures.NotYetValid)) {
				ControlDictionary["certificateDateStatusLabel"].Text      = "The server certificate is not yet valid.";
				ControlDictionary["certificateDateStatusLabel"].ForeColor = Color.Red;
			} else {
				ControlDictionary["certificateDateStatusLabel"].Text      = "The server certificate date is valid.";
				ControlDictionary["certificateDateStatusLabel"].ForeColor = Color.Green;
			}
			
		}
	}
}
