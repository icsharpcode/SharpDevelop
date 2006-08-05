// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Threading;
using System.Windows.Forms;

using NSvn.Common;
using NSvn.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Svn.Gui;
using ICSharpCode.Core;

namespace ICSharpCode.Svn
{
	/// <summary>
	/// Description of SvnClient.
	/// </summary>
	public class SvnClient
	{
		public static SvnClient Instance = new SvnClient();
		
		Client client;
		string logMessage = String.Empty;
		
		
		MessageViewCategory svnCategory;
		
		public MessageViewCategory SvnCategory {
			get {
				if (svnCategory == null) {
					svnCategory = new MessageViewCategory("Subversion", "Subversion");
					CompilerMessageView compilerMessageView = (CompilerMessageView)WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).PadContent;
					compilerMessageView.AddCategory(svnCategory);
				}
				return svnCategory;
			}
		}
		
		public NSvn.Core.Client Client {
			get {
				return client;
			}
		}
		
		public string LogMessage {
			get {
				return logMessage;
			}
			set {
				logMessage = value;
			}
		}
		
		string GetKindString(NodeKind kind)
		{
			switch (kind) {
				case NodeKind.Directory:
					return "directory ";
				case NodeKind.File:
					return "file ";
			}
			return null;
		}
		
		public static string GetActionString(ChangedPathAction action)
		{
			switch (action) {
				case ChangedPathAction.Add:
					return GetActionString(NotifyAction.CommitAdded);
				case ChangedPathAction.Delete:
					return GetActionString(NotifyAction.CommitDeleted);
				case ChangedPathAction.Modify:
					return GetActionString(NotifyAction.CommitModified);
				case ChangedPathAction.Replace:
					return GetActionString(NotifyAction.CommitReplaced);
				default:
					return "unknown";
			}
		}
		
		public static string GetActionString(NotifyAction action)
		{
			switch (action) {
				case NotifyAction.Add:
				case NotifyAction.UpdateAdd:
				case NotifyAction.CommitAdded:
					return "added";
				case NotifyAction.Copy:
					return "copied";
				case NotifyAction.Delete:
				case NotifyAction.UpdateDelete:
				case NotifyAction.CommitDeleted:
					return "deleted";
				case NotifyAction.Restore:
					return "restored";
				case NotifyAction.Revert:
					return "reverted";
				case NotifyAction.FailedRevert:
					return "revert failed";
				case NotifyAction.Resolved:
					return "resolved";
				case NotifyAction.Skip:
					return "skipped";
				case NotifyAction.UpdateUpdate:
					return "updated";
				case NotifyAction.CommitPostfixTxDelta:
				case NotifyAction.UpdateCompleted:
					return "";
				case NotifyAction.UpdateExternal:
					return "updated external";
				case NotifyAction.CommitModified:
					return "modified";
				case NotifyAction.CommitReplaced:
					return "replaced";
				default:
					return "unknown";
			}
		}
		
		void ReceiveNotification(object sender, NotificationEventArgs e)
		{
			if (e.Action == NotifyAction.UpdateCompleted) {
				SvnCategory.AppendText(Environment.NewLine + "Updated " + e.Path + " to revision " + e.RevisionNumber + ".");
				return;
			}
			if (e.Action == NotifyAction.CommitPostfixTxDelta) {
				SvnCategory.AppendText(".");
				return;
			}
			
			string kind   = GetKindString(e.NodeKind);
			string action = GetActionString(e.Action);
			SvnCategory.AppendText(Environment.NewLine + kind + action + " : " + e.Path);
		}
		
		void SetLogMessage(object sender, LogMessageEventArgs e)
		{
			if (e.Message == null) {
				e.Message = logMessage;
			}
		}
		
		void WriteMid(string str)
		{
			const int max = 40;
			string filler = new String('-', max - str.Length / 2);
			SvnCategory.AppendText(Environment.NewLine + filler + " " + str + " " + filler);
			if (str.Length % 2 == 0) {
				SvnCategory.AppendText("-");
			}
		}
		
		class ThreadStartWrapper
		{
			ThreadStart innerDelegate;
			
			public ThreadStartWrapper(ThreadStart innerDelegate)
			{
				this.innerDelegate = innerDelegate;
			}
			
			public void Start()
			{
				try {
					innerDelegate();
				} catch (Exception e) {
					SvnClient.Instance.OperationDone();
					
					MessageService.ShowError(e);
				} finally {
					SvnClient.Instance.OperationDone();
				}
			}
		}
		
		InOperationDialog inOperationForm;
		bool done = false;
		public void OperationStart(string operationName, ThreadStart threadStart)
		{
			done = false;
			WriteMid(operationName);
			
			Thread thread = new Thread(new ThreadStart(new ThreadStartWrapper(threadStart).Start));
			thread.Name = "SvnOperation";
			thread.IsBackground = true;
			inOperationForm = new InOperationDialog(operationName, thread);
			inOperationForm.Owner = (Form)WorkbenchSingleton.Workbench;
			inOperationForm.Show();
			thread.Start();
		}
		
		void OperationDone()
		{
			if (done) {
				return;
			}
			WorkbenchSingleton.SafeThreadCall(WriteMid, "Done");
			try {
				if (inOperationForm != null) {
					inOperationForm.Operation = null;
					WorkbenchSingleton.SafeThreadCall(inOperationForm.Close);
					inOperationForm = null;
				}
			} catch (Exception e) {
				MessageService.ShowError(e);
			} finally {
				done = true;
			}
		}
		
		public void WaitForOperationEnd()
		{
			while (!done) {
				Application.DoEvents();
			}
		}
		
		SvnClient()
		{
			LoggingService.Info("SVN: SvnClient initialized");
			client = new Client();
			client.LogMessage   += new LogMessageDelegate(SetLogMessage);
			client.Notification += new NotificationDelegate(ReceiveNotification);
			
			client.AuthBaton.Add(AuthenticationProvider.GetUsernameProvider());
			client.AuthBaton.Add(AuthenticationProvider.GetSimpleProvider());
			client.AuthBaton.Add(AuthenticationProvider.GetSimplePromptProvider(new SimplePromptDelegate(this.PasswordPrompt), 3));
			client.AuthBaton.Add(AuthenticationProvider.GetSslServerTrustFileProvider());
			client.AuthBaton.Add(AuthenticationProvider.GetSslServerTrustPromptProvider(new SslServerTrustPromptDelegate(this.SslServerTrustPrompt)));
			client.AuthBaton.Add(AuthenticationProvider.GetSslClientCertPasswordFileProvider());
			client.AuthBaton.Add(AuthenticationProvider.GetSslClientCertPasswordPromptProvider(new SslClientCertPasswordPromptDelegate(this.ClientCertificatePasswordPrompt), 3));
			client.AuthBaton.Add(AuthenticationProvider.GetSslClientCertFileProvider());
			client.AuthBaton.Add(AuthenticationProvider.GetSslClientCertPromptProvider(new SslClientCertPromptDelegate(this.ClientCertificatePrompt), 3));
		}
		
		SimpleCredential PasswordPrompt(string realm, string userName, bool maySave)
		{
			using (LoginDialog loginDialog = new LoginDialog(realm, userName, maySave)) {
				if (loginDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					return loginDialog.Credential;
				}
			}
			return null;
		}
		
		SslServerTrustCredential SslServerTrustPrompt(string realm, SslFailures failures, SslServerCertificateInfo info, bool maySave)
		{
			using (SslServerTrustDialog sslServerTrustDialog = new SslServerTrustDialog(info, failures, maySave)) {
				if (sslServerTrustDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					return sslServerTrustDialog.Credential;
				}
			}
			return null;
		}
		
		SslClientCertificatePasswordCredential ClientCertificatePasswordPrompt(string realm, bool maySave)
		{
			using (ClientCertPassphraseDialog clientCertPassphraseDialog = new ClientCertPassphraseDialog(realm, maySave)) {
				if (clientCertPassphraseDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					return clientCertPassphraseDialog.Credential;
				}
			}
			return null;
		}
		
		SslClientCertificateCredential ClientCertificatePrompt(string realm, bool maySave)
		{
			using (ClientCertDialog clientCertDialog = new ClientCertDialog(realm, maySave)) {
				if (clientCertDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK) {
					return clientCertDialog.Credential;
				}
			}
			return null;
		}
	}
}
