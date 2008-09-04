// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Text;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Svn.Gui;
using PumaCode.SvnDotNet.SubversionSharp;
using PumaCode.SvnDotNet.AprSharp;
using System.Runtime.Serialization;

namespace ICSharpCode.Svn
{
	using Svn = PumaCode.SvnDotNet.SubversionSharp.Svn;

	/// <summary>
	/// A wrapper around the subversion library.
	/// </summary>
	public sealed class SvnClientWrapper : IDisposable
	{
		#region status->string conversion
		static string GetKindString(Svn.NodeKind kind)
		{
			switch (kind) {
				case Svn.NodeKind.Dir:
					return "directory ";
				case Svn.NodeKind.File:
					return "file ";
			}
			return null;
		}
		
		public static string GetActionString(char action)
		{
			switch (action) {
				case 'A':
					return GetActionString(SvnWcNotify.Actions.CommitAdded);
				case 'D':
					return GetActionString(SvnWcNotify.Actions.CommitDeleted);
				case 'M':
					return GetActionString(SvnWcNotify.Actions.CommitModified);
				case 'R':
					return GetActionString(SvnWcNotify.Actions.CommitReplaced);
				default:
					return "unknown";
			}
		}
		
		static string GetActionString(SvnWcNotify.Actions action)
		{
			switch (action) {
				case SvnWcNotify.Actions.Add:
				case SvnWcNotify.Actions.UpdateAdd:
				case SvnWcNotify.Actions.CommitAdded:
					return "added";
				case SvnWcNotify.Actions.Copy:
					return "copied";
				case SvnWcNotify.Actions.Delete:
				case SvnWcNotify.Actions.UpdateDelete:
				case SvnWcNotify.Actions.CommitDeleted:
					return "deleted";
				case SvnWcNotify.Actions.Restore:
					return "restored";
				case SvnWcNotify.Actions.Revert:
					return "reverted";
				case SvnWcNotify.Actions.FailedRevert:
					return "revert failed";
				case SvnWcNotify.Actions.Resolved:
					return "resolved";
				case SvnWcNotify.Actions.Skip:
					return "skipped";
				case SvnWcNotify.Actions.UpdateUpdate:
					return "updated";
				case SvnWcNotify.Actions.UpdateExternal:
					return "updated external";
				case SvnWcNotify.Actions.CommitModified:
					return "modified";
				case SvnWcNotify.Actions.CommitReplaced:
					return "replaced";
				case SvnWcNotify.Actions.FailedLock:
					return "lock failed";
				case SvnWcNotify.Actions.FailedUnlock:
					return "unlock failed";
				case SvnWcNotify.Actions.Locked:
					return "locked";
				case SvnWcNotify.Actions.Unlocked:
					return "unlocked";
				default:
					return "unknown";
			}
		}
		#endregion
		
		#region AprPoolHandle
		sealed class AprPoolHandle : IDisposable
		{
			AprPool pool;
			
			public AprPool Pool {
				get { return pool; }
			}
			
			public AprPoolHandle()
			{
				pool = Svn.PoolCreate();
			}
			
			public void Dispose()
			{
				if (!pool.IsNull) {
					pool.Destroy();
				}
				GC.SuppressFinalize(this);
			}
			
			~AprPoolHandle()
			{
				if (!pool.IsNull) {
					pool.Destroy();
				}
			}
		}
		#endregion
		
		#region Cancel support
		bool cancel;
		
		public void Cancel()
		{
			cancel = true;
		}
		
		SvnError OnCancel(IntPtr baton)
		{
			// TODO: lookup correct error number
			if (cancel)
				return SvnError.Create(1, SvnError.NoError, "User cancelled.");
			else
				return SvnError.NoError;
		}
		#endregion
		
		AprPoolHandle memoryPool;
		SvnClient client;
		Dictionary<string, Status> statusCache = new Dictionary<string, Status>(StringComparer.InvariantCultureIgnoreCase);
		
		public SvnClientWrapper()
		{
			Debug("SVN: Create SvnClient instance");
			
			memoryPool = new AprPoolHandle();
			client = new SvnClient(memoryPool.Pool);
			client.Context.NotifyFunc2 = new SvnDelegate(new SvnWcNotify.Func2(OnNotify));
			client.Context.CancelFunc = new SvnDelegate(new SvnClient.CancelFunc(OnCancel));
		}
		
		public void Dispose()
		{
			if (memoryPool != null) {
				Debug("SVN: Dispose SvnClient");
				memoryPool.Dispose();
				memoryPool = null;
			}
			client = null;
			statusCache = null;
		}
		
		#region Authorization
		bool authorizationEnabled;
		bool allowInteractiveAuthorization;
		
		public bool AllowInteractiveAuthorization {
			get {
				return allowInteractiveAuthorization;
			}
			set {
				CheckNotDisposed();
				if (allowInteractiveAuthorization != value) {
					if (authorizationEnabled)
						throw new InvalidOperationException("Cannot change AllowInteractiveAuthorization after an operation was done.");
					allowInteractiveAuthorization = value;
				}
			}
		}
		
		void OpenAuth()
		{
			if (authorizationEnabled)
				return;
			authorizationEnabled = true;
			
			const int retryLimit = 3;
			client.AddUsernameProvider();
			client.AddSimpleProvider();
			if (allowInteractiveAuthorization) {
				client.AddPromptProvider(PasswordPrompt, IntPtr.Zero, retryLimit);
			}
			client.AddSslServerTrustFileProvider();
			if (allowInteractiveAuthorization) {
				client.AddPromptProvider(SslServerTrustPrompt, IntPtr.Zero);
			}
			client.AddSslClientCertPwFileProvider();
			if (allowInteractiveAuthorization) {
				client.AddPromptProvider(ClientCertificatePasswordPrompt, IntPtr.Zero, retryLimit);
			}
			client.AddSslClientCertFileProvider();
			if (allowInteractiveAuthorization) {
				client.AddPromptProvider(ClientCertificatePrompt, IntPtr.Zero, retryLimit);
			}
			client.OpenAuth();
		}
		
		SvnError PasswordPrompt(out SvnAuthCredSimple cred, IntPtr baton, AprString realm, AprString username, bool maySave, AprPool pool)
		{
			cred = IntPtr.Zero;
			LoggingService.Debug("PasswordPrompt");
			try {
				using (LoginDialog loginDialog = new LoginDialog(realm.Value, username.Value, maySave)) {
					if (WorkbenchSingleton.SafeThreadFunction<IWin32Window, DialogResult>(loginDialog.ShowDialog, WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
						cred = loginDialog.CreateCredential(pool);
					}
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
			return SvnError.NoError;
		}
		
		SvnError SslServerTrustPrompt(out SvnAuthCredSslServerTrust cred, IntPtr baton, AprString realm, SvnAuthCredSslServerTrust.CertFailures failures, SvnAuthSslServerCertInfo certInfo, bool maySave, IntPtr pool)
		{
			cred = IntPtr.Zero;
			LoggingService.Debug("SslServerTrustPrompt");
			try {
				using (SslServerTrustDialog sslServerTrustDialog = new SslServerTrustDialog(certInfo, failures, maySave)) {
					if (WorkbenchSingleton.SafeThreadFunction<IWin32Window, DialogResult>(sslServerTrustDialog.ShowDialog, WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
						cred = sslServerTrustDialog.CreateCredential(pool);
					}
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
			return SvnError.NoError;
		}
		
		SvnError ClientCertificatePasswordPrompt(out SvnAuthCredSslClientCertPw cred, IntPtr baton, AprString realm, bool maySave, IntPtr pool)
		{
			cred = IntPtr.Zero;
			LoggingService.Debug("SslServerTrustPrompt");
			try {
				using (ClientCertPassphraseDialog clientCertPassphraseDialog = new ClientCertPassphraseDialog(realm.Value, maySave)) {
					if (WorkbenchSingleton.SafeThreadFunction<IWin32Window, DialogResult>(clientCertPassphraseDialog.ShowDialog, WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
						cred = clientCertPassphraseDialog.CreateCredential(pool);
					}
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
			return SvnError.NoError;
		}
		
		SvnError ClientCertificatePrompt(out SvnAuthCredSslClientCert cred, IntPtr baton, AprString realm, bool maySave, IntPtr pool)
		{
			cred = IntPtr.Zero;
			LoggingService.Debug("SslServerTrustPrompt");
			try {
				using (ClientCertDialog clientCertDialog = new ClientCertDialog(realm.Value, maySave)) {
					if (WorkbenchSingleton.SafeThreadFunction<IWin32Window, DialogResult>(clientCertDialog.ShowDialog, WorkbenchSingleton.MainWin32Window) == DialogResult.OK) {
						cred = clientCertDialog.CreateCredential(pool);
					}
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
			return SvnError.NoError;
		}
		#endregion
		
		#region Notifications
		public event EventHandler<SubversionOperationEventArgs> OperationStarted;
		public event EventHandler OperationFinished;
		public event EventHandler<NotificationEventArgs> Notify;
		
		void OnNotify(IntPtr baton, SvnWcNotify notify, AprPool pool)
		{
			if (Notify != null) {
				Notify(this, new NotificationEventArgs() {
				       	Action = GetActionString(notify.Action),
				       	Kind = GetKindString(notify.Kind),
				       	Path = notify.Path.Value
				       });
			}
		}
		#endregion
		
		[System.Diagnostics.ConditionalAttribute("DEBUG")]
		void Debug(string text)
		{
			LoggingService.Debug(text);
		}
		
		void CheckNotDisposed()
		{
			if (client == null)
				throw new ObjectDisposedException("SvnClientWrapper");
		}
		
		void BeforeOperation(string operationName)
		{
			// before any subversion operation, ensure the object is not disposed
			// and register authorization if necessary
			CheckNotDisposed();
			OpenAuth();
			cancel = false;
			if (OperationStarted != null)
				OperationStarted(this, new SubversionOperationEventArgs { Operation = operationName });
		}
		
		void AfterOperation()
		{
			// after any subversion operation, clear the memory pool
			client.Clear();
			if (OperationFinished != null)
				OperationFinished(this, EventArgs.Empty);
		}
		
		public void ClearStatusCache()
		{
			CheckNotDisposed();
			statusCache.Clear();
		}
		
		public Status SingleStatus(string filename)
		{
			Debug("SVN: SingleStatus(" + filename + ")");
			BeforeOperation("stat");
			try {
				filename = FileUtility.NormalizePath(filename);
				Status result;
				if (statusCache.TryGetValue(filename, out result)) {
					Debug("SVN: SingleStatus retrieved from cache " + result.TextStatus);
					return result;
				}
				client.Status2(
					filename, Svn.Revision.Working,
					delegate (IntPtr baton, SvnPath path, SvnWcStatus2 status) {
						string dir = path.Value;
						Debug("SVN: SingleStatus.callback(" + dir + "," + status.TextStatus + ")");
						Status s = new Status {
							Copied = status.Copied,
							TextStatus = ToStatusKind(status.TextStatus)
						};
						statusCache[dir] = s;
						if (StringComparer.InvariantCultureIgnoreCase.Equals(filename, dir)) {
							result = s;
						}
					},
					IntPtr.Zero,
					false, true, false, false, false
				);
				if (result != null) {
					return result;
				} else {
					return new Status {
						TextStatus = StatusKind.None
					};
				}
			} catch (SvnException ex) {
				throw new SvnClientException(ex);
			} finally {
				AfterOperation();
			}
		}
		
		public void Add(string filename, Recurse recurse)
		{
			Debug("SVN: Add(" + filename + ", " + recurse + ")");
			BeforeOperation("add");
			try {
				client.Add3(filename, recurse == Recurse.Full, false, false);
			} catch (SvnException ex) {
				throw new SvnClientException(ex);
			} finally {
				AfterOperation();
			}
		}
		
		public string GetPropertyValue(string fileName, string propertyName)
		{
			Debug("SVN: GetPropertyValue(" + fileName + ", " + propertyName + ")");
			BeforeOperation("propget");
			try {
				AprHash hash = client.PropGet2(propertyName, fileName,
				                               Svn.Revision.Working, Svn.Revision.Working,
				                               false);
				foreach (AprHashEntry entry in hash) {
					// entry.ValueAsString treats the value as const char*, but it is a svn_string_t*
					SvnString value = entry.Value;
					Debug("SVN: GetPropertyValue hash entry: (" + entry.KeyAsString + ", " + value + ")");
					return value.Data.Value;
				}
				Debug("SVN: GetPropertyValue did not find any entries");
				return null;
			} catch (SvnException ex) {
				throw new SvnClientException(ex);
			} finally {
				AfterOperation();
			}
		}
		
		public void SetPropertyValue(string fileName, string propertyName, string newPropertyValue)
		{
			Debug("SVN: SetPropertyValue(" + fileName + ", " + propertyName + ", " + newPropertyValue + ")");
			BeforeOperation("propset");
			try {
				SvnString npv;
				if (newPropertyValue != null)
					npv = new SvnString(newPropertyValue, client.Pool);
				else
					npv = IntPtr.Zero;
				client.PropSet2(propertyName, npv, fileName, false, false);
			} catch (SvnException ex) {
				throw new SvnClientException(ex);
			} finally {
				AfterOperation();
			}
		}
		
		SvnPath[] ToSvnPaths(string[] files)
		{
			SvnPath[] paths = new SvnPath[files.Length];
			for (int i = 0; i < files.Length; i++) {
				paths[i] = new SvnPath(files[i], client.Pool);
			}
			return paths;
		}
		
		public void Delete(string[] files, bool force)
		{
			Debug("SVN: Delete(" + string.Join(",", files) + ", " + force + ")");
			BeforeOperation("delete");
			try {
				client.Delete2(ToSvnPaths(files), force);
			} catch (SvnException ex) {
				throw new SvnClientException(ex);
			} finally {
				AfterOperation();
			}
		}
		
		public void Revert(string[] files, Recurse recurse)
		{
			Debug("SVN: Revert(" + string.Join(",", files) + ", " + recurse + ")");
			BeforeOperation("revert");
			try {
				client.Revert(ToSvnPaths(files), recurse == Recurse.Full);
			} catch (SvnException ex) {
				throw new SvnClientException(ex);
			} finally {
				AfterOperation();
			}
		}
		
		public void Move(string from, string to, bool force)
		{
			Debug("SVN: Move(" + from + ", " + to + ", " + force + ")");
			BeforeOperation("move");
			try {
				client.Move3(from, to, force);
			} catch (SvnException ex) {
				throw new SvnClientException(ex);
			} finally {
				AfterOperation();
			}
		}
		
		public void Copy(string from, Revision revision, string to)
		{
			Debug("SVN: Copy(" + from + ", " + revision + ", " + to);
			BeforeOperation("copy");
			try {
				client.Copy2(from, revision, to);
			} catch (SvnException ex) {
				throw new SvnClientException(ex);
			} finally {
				AfterOperation();
			}
		}
		
		public void AddToIgnoreList(string directory, params string[] filesToIgnore)
		{
			Debug("SVN: AddToIgnoreList(" + directory + ", " + string.Join(",", filesToIgnore) + ")");
			string propertyValue = GetPropertyValue(directory, "svn:ignore");
			StringBuilder b = new StringBuilder();
			if (propertyValue != null) {
				using (StringReader r = new StringReader(propertyValue)) {
					string line;
					while ((line = r.ReadLine()) != null) {
						if (line.Length > 0) {
							b.AppendLine(line);
						}
					}
				}
			}
			foreach (string file in filesToIgnore)
				b.AppendLine(file);
			SetPropertyValue(directory, "svn:ignore", b.ToString());
		}
		
		public void Log(string[] paths, Revision start, Revision end,
		                int limit, bool discoverChangePaths, bool strictNodeHistory,
		                Action<LogMessage> logMessageReceiver)
		{
			Debug("SVN: Log({" + string.Join(",", paths) + "}, " + start + ", " + end +
			      ", " + limit + ", " + discoverChangePaths + ", " + strictNodeHistory + ")");
			BeforeOperation("log");
			try {
				client.Log2(
					ToSvnPaths(paths),
					start, end, limit, discoverChangePaths, strictNodeHistory,
					delegate (IntPtr baton, AprHash changed_paths, int revision, AprString author, AprString date, AprString message, AprPool pool) {
						Debug("SVN: Log: Got revision " + revision);
						DateTime dateTime = DateTime.MinValue;
						DateTime.TryParse(date.Value, out dateTime);
						LogMessage msg = new LogMessage() {
							Revision = revision,
							Author = author.Value,
							Date = dateTime,
							Message = message.Value
						};
						if (discoverChangePaths) {
							msg.ChangedPaths = new List<ChangedPath>();
							foreach (AprHashEntry entry in changed_paths) {
								SvnLogChangedPath changedPath = entry.Value;
								msg.ChangedPaths.Add(new ChangedPath {
								                     	Path = entry.KeyAsString,
								                     	CopyFromPath = changedPath.CopyFromPath.Value,
								                     	CopyFromRevision = changedPath.CopyFromRev,
								                     	Action = changedPath.Action
								                     });
							}
						}
						logMessageReceiver(msg);
						return SvnError.NoError;
					},
					IntPtr.Zero
				);
				Debug("SVN: Log finished");
			} catch (SvnException ex) {
				throw new SvnClientException(ex);
			} finally {
				AfterOperation();
			}
		}
		
		static StatusKind ToStatusKind(SvnWcStatus.Kind kind)
		{
			switch (kind) {
				case SvnWcStatus.Kind.Added:
					return StatusKind.Added;
				case SvnWcStatus.Kind.Conflicted:
					return StatusKind.Conflicted;
				case SvnWcStatus.Kind.Deleted:
					return StatusKind.Deleted;
				case SvnWcStatus.Kind.External:
					return StatusKind.External;
				case SvnWcStatus.Kind.Ignored:
					return StatusKind.Ignored;
				case SvnWcStatus.Kind.Incomplete:
					return StatusKind.Incomplete;
				case SvnWcStatus.Kind.Merged:
					return StatusKind.Merged;
				case SvnWcStatus.Kind.Missing:
					return StatusKind.Missing;
				case SvnWcStatus.Kind.Modified:
					return StatusKind.Modified;
				case SvnWcStatus.Kind.Normal:
					return StatusKind.Normal;
				case SvnWcStatus.Kind.Obstructed:
					return StatusKind.Obstructed;
				case SvnWcStatus.Kind.Replaced:
					return StatusKind.Replaced;
				case SvnWcStatus.Kind.Unversioned:
					return StatusKind.Unversioned;
				default:
					return StatusKind.None;
			}
		}
	}
	
	public class NotificationEventArgs : EventArgs
	{
		public string Action;
		public string Kind;
		public string Path;
	}
	
	public class SubversionOperationEventArgs : EventArgs
	{
		public string Operation;
	}
	
	public class LogMessage
	{
		public int Revision;
		public string Author;
		public DateTime Date;
		public string Message;
		
		public List<ChangedPath> ChangedPaths;
	}
	
	public class ChangedPath
	{
		public string Path;
		public string CopyFromPath;
		public int CopyFromRevision;
		/// <summary>
		/// change action ('A','D','R' or 'M')
		/// </summary>
		public char Action;
	}
	
	public class Revision
	{
		SvnRevision revision;
		
		public static readonly Revision Base = new SvnRevision(Svn.Revision.Base);
		public static readonly Revision Committed = new SvnRevision(Svn.Revision.Committed);
		public static readonly Revision Head = new SvnRevision(Svn.Revision.Head);
		public static readonly Revision Working = new SvnRevision(Svn.Revision.Working);
		public static readonly Revision Unspecified = new SvnRevision(Svn.Revision.Unspecified);
		
		public static Revision FromNumber(int number)
		{
			return new SvnRevision(number);
		}
		
		public static implicit operator SvnRevision(Revision r)
		{
			return r.revision;
		}
		
		public static implicit operator Revision(SvnRevision r)
		{
			return new Revision() { revision = r };
		}
		
		public override string ToString()
		{
			switch (revision.Kind) {
				case Svn.Revision.Base:
					return "base";
				case Svn.Revision.Committed:
					return "committed";
				case Svn.Revision.Date:
					return AprTime.ToDateTime(revision.Date).ToString();
				case Svn.Revision.Head:
					return "head";
				case Svn.Revision.Number:
					return revision.Number.ToString();
				case Svn.Revision.Previous:
					return "previous";
				case Svn.Revision.Unspecified:
					return "unspecified";
				case Svn.Revision.Working:
					return "working";
				default:
					return "unknown";
			}
		}
	}

	public class Status
	{
		public bool Copied { get; set; }
		public StatusKind TextStatus { get; set; }
	}

	public enum Recurse
	{
		None,
		Full
	}

	public class SvnClientException : Exception
	{
		public readonly int ErrorCode;
		
		internal SvnClientException(SvnException ex) : base(ex.Message, ex)
		{
			LoggingService.Debug(ex);
			ErrorCode = ex.AprErr;
		}
		
		/// <summary>
		/// Gets the inner exception of the exception being wrapped.
		/// </summary>
		public Exception GetInnerException()
		{
			return InnerException.InnerException;
		}
	}

	public enum StatusKind
	{
		None,
		Added,
		Conflicted,
		Deleted,
		Modified,
		Replaced,
		External,
		Ignored,
		Incomplete,
		Merged,
		Missing,
		Obstructed,
		Normal,
		Unversioned
	}
}
