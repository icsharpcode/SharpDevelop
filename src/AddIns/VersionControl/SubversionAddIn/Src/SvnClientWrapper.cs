// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Svn.Gui;
using SharpSvn;
using SharpSvn.UI;

namespace ICSharpCode.Svn
{
	/// <summary>
	/// A wrapper around the subversion library.
	/// </summary>
	public sealed class SvnClientWrapper : IDisposable
	{
		#region status->string conversion
		static string GetKindString(SvnNodeKind kind)
		{
			switch (kind) {
				case SvnNodeKind.Directory:
					return "directory ";
				case SvnNodeKind.File:
					return "file ";
				default:
					return null;
			}
		}
		
		public static string GetActionString(SvnChangeAction action)
		{
			switch (action) {
				case SvnChangeAction.Add:
					return GetActionString(SvnNotifyAction.CommitAdded);
				case SvnChangeAction.Delete:
					return GetActionString(SvnNotifyAction.CommitDeleted);
				case SvnChangeAction.Modify:
					return GetActionString(SvnNotifyAction.CommitModified);
				case SvnChangeAction.Replace:
					return GetActionString(SvnNotifyAction.CommitReplaced);
				default:
					return "unknown";
			}
		}
		
		static string GetActionString(SvnNotifyAction action)
		{
			switch (action) {
				case SvnNotifyAction.Add:
				case SvnNotifyAction.CommitAdded:
					return "added";
				case SvnNotifyAction.Copy:
					return "copied";
				case SvnNotifyAction.Delete:
				case SvnNotifyAction.UpdateDelete:
				case SvnNotifyAction.CommitDeleted:
					return "deleted";
				case SvnNotifyAction.Restore:
					return "restored";
				case SvnNotifyAction.Revert:
					return "reverted";
				case SvnNotifyAction.RevertFailed:
					return "revert failed";
				case SvnNotifyAction.Resolved:
					return "resolved";
				case SvnNotifyAction.Skip:
					return "skipped";
				case SvnNotifyAction.UpdateUpdate:
					return "updated";
				case SvnNotifyAction.UpdateExternal:
					return "updated external";
				case SvnNotifyAction.CommitModified:
					return "modified";
				case SvnNotifyAction.CommitReplaced:
					return "replaced";
				case SvnNotifyAction.LockFailedLock:
					return "lock failed";
				case SvnNotifyAction.LockFailedUnlock:
					return "unlock failed";
				case SvnNotifyAction.LockLocked:
					return "locked";
				case SvnNotifyAction.LockUnlocked:
					return "unlocked";
				default:
					return "unknown";
			}
		}
		#endregion
		
		#region Cancel support
		bool cancel;
		
		public void Cancel()
		{
			cancel = true;
		}
		
		void client_Cancel(object sender, SvnCancelEventArgs e)
		{
			e.Cancel = cancel;
		}
		#endregion
		
		SvnClient client;
		
		public SvnClientWrapper()
		{
			Debug("SVN: Create SvnClient instance");
			
			client = new SvnClient();
			client.Notify += client_Notify;
			client.Cancel += client_Cancel;
		}

		public void Dispose()
		{
			if (client != null)
				client.Dispose();
			client = null;
		}
		
		#region Authorization
		bool authorizationEnabled;
		bool allowInteractiveAuthorization;
		
		public void AllowInteractiveAuthorization()
		{
			CheckNotDisposed();
			if (!allowInteractiveAuthorization) {
				allowInteractiveAuthorization = true;
				SvnUI.Bind(client, SD.WinForms.MainWin32Window);
			}
		}
		
		void OpenAuth()
		{
			if (authorizationEnabled)
				return;
			authorizationEnabled = true;
		}
		#endregion
		
		#region Notifications
		public event EventHandler<SubversionOperationEventArgs> OperationStarted;
		public event EventHandler OperationFinished;
		public event EventHandler<NotificationEventArgs> Notify;
		
		void client_Notify(object sender, SvnNotifyEventArgs e)
		{
			if (Notify != null) {
				Notify(this, new NotificationEventArgs() {
				       	Action = GetActionString(e.Action),
				       	Kind = GetKindString(e.NodeKind),
				       	Path = e.Path
				       });
			}
		}
		#endregion
		
		[System.Diagnostics.ConditionalAttribute("DEBUG")]
		static void Debug(string text)
		{
			LoggingService.Debug(text);
		}
		
		void CheckNotDisposed()
		{
			if (client == null)
				throw new ObjectDisposedException("SvnClientWrapper");
		}
		
		void BeforeWriteOperation(string operationName)
		{
			BeforeReadOperation(operationName);
			ClearStatusCache();
		}
		
		void BeforeReadOperation(string operationName)
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
			if (OperationFinished != null)
				OperationFinished(this, EventArgs.Empty);
		}
		
		// We cache SingleStatus results because WPF asks our Condition several times
		// per menu entry; and it would be extremely slow to hit the hard disk every time (SD2-1672)
		Dictionary<string, Status> statusCache = new Dictionary<string, Status>(StringComparer.OrdinalIgnoreCase);
		
		public void ClearStatusCache()
		{
			CheckNotDisposed();
			statusCache.Clear();
		}
		
		public Status SingleStatus(string filename)
		{
			filename = FileUtility.NormalizePath(filename);
			Status result = null;
			if (statusCache.TryGetValue(filename, out result)) {
				Debug("SVN: SingleStatus(" + filename + ") = cached " + result.TextStatus);
				return result;
			}
			Debug("SVN: SingleStatus(" + filename + ")");
			BeforeReadOperation("stat");
			try {
				SvnStatusArgs args = new SvnStatusArgs {
					Revision = SvnRevision.Working,
					RetrieveAllEntries = true,
					RetrieveIgnoredEntries = true,
					Depth = SvnDepth.Empty
				};
				client.Status(
					filename, args,
					delegate (object sender, SvnStatusEventArgs e) {
						Debug("SVN: SingleStatus.callback(" + e.FullPath + "," + e.LocalContentStatus + ")");
						System.Diagnostics.Debug.Assert(filename.ToString().Equals(e.FullPath, StringComparison.OrdinalIgnoreCase));
						result = new Status {
							Copied = e.LocalCopied,
							TextStatus = ToStatusKind(e.LocalContentStatus)
						};
					}
				);
				if (result == null) {
					result = new Status {
						TextStatus = StatusKind.None
					};
				}
				statusCache.Add(filename, result);
				return result;
			} catch (SvnException ex) {
				switch (ex.SvnErrorCode) {
					case SvnErrorCode.SVN_ERR_WC_UPGRADE_REQUIRED:
						result = new Status { TextStatus = StatusKind.None };
						break;
					case SvnErrorCode.SVN_ERR_WC_NOT_WORKING_COPY:
						result = new Status { TextStatus = StatusKind.Unversioned };
						break;
					default:
						throw new SvnClientException(ex);
				}
				statusCache.Add(filename, result);
				return result;
			} finally {
				AfterOperation();
			}
		}
		
		static SvnDepth ConvertDepth(Recurse recurse)
		{
			if (recurse == Recurse.Full)
				return SvnDepth.Infinity;
			else
				return SvnDepth.Empty;
		}
		
		public void Add(string filename, Recurse recurse)
		{
			Debug("SVN: Add(" + filename + ", " + recurse + ")");
			BeforeWriteOperation("add");
			try {
				client.Add(filename, ConvertDepth(recurse));
			} catch (SvnException ex) {
				throw new SvnClientException(ex);
			} finally {
				AfterOperation();
			}
		}
		
		public string GetPropertyValue(string fileName, string propertyName)
		{
			Debug("SVN: GetPropertyValue(" + fileName + ", " + propertyName + ")");
			BeforeReadOperation("propget");
			try {
				string propertyValue;
				if (client.GetProperty(fileName, propertyName, out propertyValue))
					return propertyValue;
				else
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
			BeforeWriteOperation("propset");
			try {
				if (newPropertyValue != null)
					client.SetProperty(fileName, propertyName, newPropertyValue);
				else
					client.DeleteProperty(fileName, propertyName);
			} catch (SvnException ex) {
				throw new SvnClientException(ex);
			} finally {
				AfterOperation();
			}
		}
		
		public void Delete(string[] files, bool force)
		{
			Debug("SVN: Delete(" + string.Join(",", files) + ", " + force + ")");
			BeforeWriteOperation("delete");
			try {
				client.Delete(
					files,
					new SvnDeleteArgs {
						Force = force
					});
			} catch (SvnException ex) {
				throw new SvnClientException(ex);
			} finally {
				AfterOperation();
			}
		}
		
		public void Revert(string[] files, Recurse recurse)
		{
			Debug("SVN: Revert(" + string.Join(",", files) + ", " + recurse + ")");
			BeforeWriteOperation("revert");
			try {
				client.Revert(
					files,
					new SvnRevertArgs {
						Depth = ConvertDepth(recurse)
					});
			} catch (SvnException ex) {
				throw new SvnClientException(ex);
			} finally {
				AfterOperation();
			}
		}
		
		public void Move(string from, string to, bool force)
		{
			Debug("SVN: Move(" + from + ", " + to + ", " + force + ")");
			BeforeWriteOperation("move");
			try {
				client.Move(
					from, to,
					new SvnMoveArgs {
						Force = force
					});
			} catch (SvnException ex) {
				throw new SvnClientException(ex);
			} finally {
				AfterOperation();
			}
		}
		
		public void Copy(string from, string to)
		{
			Debug("SVN: Copy(" + from + ", " + to);
			BeforeWriteOperation("copy");
			try {
				client.Copy(from, to);
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
			BeforeReadOperation("log");
			try {
				client.Log(
					paths,
					new SvnLogArgs {
						Start = start,
						End = end,
						Limit = limit,
						RetrieveChangedPaths = discoverChangePaths,
						StrictNodeHistory = strictNodeHistory
					},
					delegate (object sender, SvnLogEventArgs e) {
						try {
							Debug("SVN: Log: Got revision " + e.Revision);
							LogMessage msg = new LogMessage() {
								Revision = e.Revision,
								Author = e.Author,
								Date = e.Time,
								Message = e.LogMessage
							};
							if (discoverChangePaths) {
								msg.ChangedPaths = new List<ChangedPath>();
								foreach (var entry in e.ChangedPaths) {
									msg.ChangedPaths.Add(new ChangedPath {
									                     	Path = entry.Path,
									                     	CopyFromPath = entry.CopyFromPath,
									                     	CopyFromRevision = entry.CopyFromRevision,
									                     	Action = entry.Action
									                     });
								}
							}
							logMessageReceiver(msg);
						} catch (Exception ex) {
							MessageService.ShowException(ex);
						}
					}
				);
				Debug("SVN: Log finished");
			} catch (SvnOperationCanceledException) {
				// allow cancel without exception
			} catch (SvnException ex) {
				throw new SvnClientException(ex);
			} finally {
				AfterOperation();
			}
		}
		
		public Stream OpenBaseVersion(string fileName)
		{
			MemoryStream stream = new MemoryStream();
			if (!this.client.Write(fileName, stream, new SvnWriteArgs() { Revision = SvnRevision.Base, ThrowOnError = false }))
				return null;
			stream.Seek(0, SeekOrigin.Begin);
			return stream;
		}
		
		public Stream OpenCurrentVersion(string fileName)
		{
			MemoryStream stream = new MemoryStream();
			if (!this.client.Write(fileName, stream, new SvnWriteArgs() { Revision = SvnRevision.Working }))
				return null;
			stream.Seek(0, SeekOrigin.Begin);
			return stream;
		}
		
		public static bool IsInSourceControl(string fileName)
		{
			if (Commands.RegisterEventsCommand.CanBeVersionControlledFile(fileName)) {
				StatusKind status = OverlayIconManager.GetStatus(fileName);
				return status != StatusKind.None && status != StatusKind.Unversioned && status != StatusKind.Ignored;
			} else {
				return false;
			}
		}
		
		static StatusKind ToStatusKind(SvnStatus kind)
		{
			switch (kind) {
				case SvnStatus.Added:
					return StatusKind.Added;
				case SvnStatus.Conflicted:
					return StatusKind.Conflicted;
				case SvnStatus.Deleted:
					return StatusKind.Deleted;
				case SvnStatus.External:
					return StatusKind.External;
				case SvnStatus.Ignored:
					return StatusKind.Ignored;
				case SvnStatus.Incomplete:
					return StatusKind.Incomplete;
				case SvnStatus.Merged:
					return StatusKind.Merged;
				case SvnStatus.Missing:
					return StatusKind.Missing;
				case SvnStatus.Modified:
					return StatusKind.Modified;
				case SvnStatus.Normal:
					return StatusKind.Normal;
				case SvnStatus.NotVersioned:
					return StatusKind.Unversioned;
				case SvnStatus.Obstructed:
					return StatusKind.Obstructed;
				case SvnStatus.Replaced:
					return StatusKind.Replaced;
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
		public long Revision;
		public string Author;
		public DateTime Date;
		public string Message;
		
		public List<ChangedPath> ChangedPaths;
	}
	
	public class ChangedPath
	{
		public string Path;
		public string CopyFromPath;
		public long CopyFromRevision;
		/// <summary>
		/// change action ('A','D','R' or 'M')
		/// </summary>
		public SvnChangeAction Action;
	}
	
	public class Revision
	{
		SvnRevision revision;
		
		public static readonly Revision Base = SvnRevision.Base;
		public static readonly Revision Committed = SvnRevision.Committed;
		public static readonly Revision Head = SvnRevision.Head;
		public static readonly Revision Working = SvnRevision.Working;
		public static readonly Revision Unspecified = SvnRevision.None;
		
		public static Revision FromNumber(long number)
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
			switch (revision.RevisionType) {
				case SvnRevisionType.Base:
					return "base";
				case SvnRevisionType.Committed:
					return "committed";
				case SvnRevisionType.Time:
					return revision.Time.ToString();
				case SvnRevisionType.Head:
					return "head";
				case SvnRevisionType.Number:
					return revision.Revision.ToString();
				case SvnRevisionType.Previous:
					return "previous";
				case SvnRevisionType.None:
					return "unspecified";
				case SvnRevisionType.Working:
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
		SvnErrorCode errorCode;
		
		internal SvnClientException(SvnException ex) : base(ex.Message, ex)
		{
			this.errorCode = ex.SvnErrorCode;
			LoggingService.Debug(ex);
		}
		
		/// <summary>
		/// Gets the inner exception of the exception being wrapped.
		/// </summary>
		public Exception GetInnerException()
		{
			return InnerException.InnerException;
		}
		
		public bool IsKnownError(KnownError knownError)
		{
			return (int)errorCode == (int)knownError;
		}
	}
	
	public enum KnownError
	{
		FileNotFound = SvnErrorCode.SVN_ERR_FS_NOT_FOUND,
		CannotDeleteFileWithLocalModifications = SvnErrorCode.SVN_ERR_CLIENT_MODIFIED,
		CannotDeleteFileNotUnderVersionControl = SvnErrorCode.SVN_ERR_UNVERSIONED_RESOURCE
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
