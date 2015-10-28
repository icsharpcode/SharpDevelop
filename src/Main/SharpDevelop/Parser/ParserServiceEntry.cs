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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Parser
{
	sealed class ParserServiceEntry
	{
		struct ProjectEntry
		{
			public readonly IProject Project;
			public readonly IUnresolvedFile UnresolvedFile;
			public readonly ParseInformation CachedParseInformation;
			
			public ProjectEntry(IProject project, IUnresolvedFile unresolvedFile, ParseInformation cachedParseInformation)
			{
				this.Project = project;
				this.UnresolvedFile = unresolvedFile;
				this.CachedParseInformation = cachedParseInformation;
			}
		}
		
		readonly ParserService parserService;
		internal readonly FileName fileName;
		internal readonly IParser parser;
		List<ProjectEntry> entries = new List<ProjectEntry> { default(ProjectEntry) };
		ITextSourceVersion currentVersion;
		
		// Lock ordering: runningAsyncParseLock, rwLock, lock(parserService.fileEntryDict)
		// (to avoid deadlocks, the locks may only be acquired in this order)
		internal readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
		
		public ParserServiceEntry(ParserService parserService, FileName fileName)
		{
			this.parserService = parserService;
			this.fileName = fileName;
			this.parser = parserService.CreateParser(fileName);
		}
		
		#region Owner Projects
		IProject PrimaryProject {
			get { return entries[0].Project; }
		}
		
		int FindIndexForProject(IProject parentProject)
		{
			Debug.Assert(rwLock.IsReadLockHeld || rwLock.IsUpgradeableReadLockHeld || rwLock.IsWriteLockHeld);
			if (parentProject == null)
				return 0;
			for (int i = 0; i < entries.Count; i++) {
				if (entries[i].Project == parentProject)
					return i;
			}
			// project not found
			return -1;
		}
		
		public void AddOwnerProject(IProject project, bool isLinkedFile)
		{
			Debug.Assert(project != null);
			rwLock.EnterWriteLock();
			try {
				if (FindIndexForProject(project) >= 0)
					throw new InvalidOperationException("The project alreadys owns the file");
				ProjectEntry newEntry = new ProjectEntry(project, null, null);
				if (entries[0].Project == null) {
					entries[0] = newEntry;
				} else if (isLinkedFile) {
					entries.Add(newEntry);
				} else {
					entries.Insert(0, newEntry);
				}
			} finally {
				rwLock.ExitWriteLock();
			}
		}
		
		public void RemoveOwnerProject(IProject project)
		{
			Debug.Assert(project != null);
			ProjectEntry oldEntry;
			bool removedLastOwner = false;
			rwLock.EnterWriteLock();
			try {
				int index = FindIndexForProject(project);
				if (index < 0)
					throw new InvalidOperationException("The project does not own the file");
				oldEntry = entries[index];
				if (entries.Count == 1) {
					entries[0] = default(ProjectEntry);
					removedLastOwner = true;
				} else {
					entries.RemoveAt(index);
				}
			} finally {
				rwLock.ExitWriteLock();
			}
			if (oldEntry.UnresolvedFile != null) {
				project.OnParseInformationUpdated(new ParseInformationEventArgs(project, oldEntry.UnresolvedFile, null));
			}
			if (removedLastOwner) {
				// allow the parser service to forget this entry
				parserService.RegisterForCacheExpiry(this);
			}
		}
		#endregion
		
		/// <summary>
		/// Compares currentVersion with version.
		/// -1 = currentVersion is older; 0 = same version; 1 = newVersion is older
		/// </summary>
		int CompareVersions(ITextSourceVersion newVersion)
		{
			Debug.Assert(rwLock.IsReadLockHeld || rwLock.IsUpgradeableReadLockHeld || rwLock.IsWriteLockHeld);
			if (currentVersion != null && newVersion != null && currentVersion.BelongsToSameDocumentAs(newVersion))
				return currentVersion.CompareAge(newVersion);
			else
				return -1;
		}
		
		#region Expire Cache + GetExistingUnresolvedFile + GetCachedParseInformation
		public void ExpireCache()
		{
			rwLock.EnterWriteLock();
			try {
				if (PrimaryProject == null) {
					parserService.RemoveEntry(this);
				} else {
					for (int i = 0; i < entries.Count; i++) {
						var oldEntry = entries[i];
						entries[i] = new ProjectEntry(oldEntry.Project, oldEntry.UnresolvedFile, null);
					}
				}
				// force re-parse on next ParseFile() call even if unchanged
				this.currentVersion = null;
			} finally {
				rwLock.ExitWriteLock();
			}
		}
		
		public IUnresolvedFile GetExistingUnresolvedFile(ITextSourceVersion version, IProject parentProject)
		{
			rwLock.EnterReadLock();
			try {
				if (version != null && CompareVersions(version) != 0) {
					return null;
				}
				int index = FindIndexForProject(parentProject);
				if (index < 0)
					return null;
				return entries[index].UnresolvedFile;
			} finally {
				rwLock.ExitReadLock();
			}
		}
		
		public ParseInformation GetCachedParseInformation(ITextSourceVersion version, IProject parentProject)
		{
			rwLock.EnterReadLock();
			try {
				if (version != null && CompareVersions(version) != 0) {
					return null;
				}
				int index = FindIndexForProject(parentProject);
				if (index < 0)
					return null;
				return entries[index].CachedParseInformation;
			} finally {
				rwLock.ExitReadLock();
			}
		}
		#endregion
		
		#region Parse
		public ParseInformation Parse(ITextSource fileContent, IProject parentProject, CancellationToken cancellationToken)
		{
			if (fileContent == null && parser != null) {
				fileContent = parser.GetFileContent(fileName);
			}
			
			return DoParse(fileContent, parentProject, true, cancellationToken).CachedParseInformation;
		}
		
		public IUnresolvedFile ParseFile(ITextSource fileContent, IProject parentProject, CancellationToken cancellationToken)
		{
			if (fileContent == null && parser != null) {
				fileContent = parser.GetFileContent(fileName);
			}
			
			return DoParse(fileContent, parentProject, false, cancellationToken).UnresolvedFile;
		}
		
		ProjectEntry DoParse(ITextSource fileContent, IProject parentProject, bool fullParseInformationRequested,
		                     CancellationToken cancellationToken)
		{
			if (parser == null)
				return default(ProjectEntry);
			
			if (fileContent == null) {
				// No file content was specified. Because the callers of this method already check for currently open files,
				// we can assume that the file isn't open and simply read it from disk.
				try {
					fileContent = SD.FileService.GetFileContentFromDisk(fileName, cancellationToken);
				} catch (IOException) {
					// It is possible that the file gets deleted/becomes inaccessible while a background parse
					// operation is enqueued, so we have to handle IO exceptions.
					return default(ProjectEntry);
				} catch (UnauthorizedAccessException) {
					return default(ProjectEntry);
				}
			}
			
			ProjectEntry result;
			rwLock.EnterUpgradeableReadLock();
			try {
				int index = FindIndexForProject(parentProject);
				int versionComparison = CompareVersions(fileContent.Version);
				if (versionComparison > 0 || index < 0) {
					// We're going backwards in time, or are requesting a project that is not an owner
					// for this entry.
					var parseInfo = ParseWithExceptionHandling(fileContent, fullParseInformationRequested, parentProject, cancellationToken);
					FreezableHelper.Freeze(parseInfo.UnresolvedFile);
					return new ProjectEntry(parentProject, parseInfo.UnresolvedFile, parseInfo);
				} else {
					if (versionComparison == 0 && index >= 0) {
						// Ensure we have parse info for the specified project (entry.UnresolvedFile is null for newly registered projects)
						// If full parse info is requested, ensure we have full parse info.
						if (entries[index].UnresolvedFile != null && !(fullParseInformationRequested && entries[index].CachedParseInformation == null)) {
							// We already have the requested version parsed, just return it:
							return entries[index];
						}
					}
				}
				
				ParseInformationEventArgs[] results = new ParseInformationEventArgs[entries.Count];
				for (int i = 0; i < entries.Count; i++) {
					var parseInfo = ParseWithExceptionHandling(fileContent, fullParseInformationRequested, entries[i].Project, cancellationToken);
					if (parseInfo == null)
						throw new NullReferenceException(parser.GetType().Name + ".Parse() returned null");
					if (fullParseInformationRequested && !parseInfo.IsFullParseInformation)
						throw new InvalidOperationException(parser.GetType().Name + ".Parse() did not return full parse info as requested.");
					OnDiskTextSourceVersion onDiskVersion = fileContent.Version as OnDiskTextSourceVersion;
					if (onDiskVersion != null)
						parseInfo.UnresolvedFile.LastWriteTime = onDiskVersion.LastWriteTime;
					FreezableHelper.Freeze(parseInfo.UnresolvedFile);
					results[i] = new ParseInformationEventArgs(entries[i].Project, entries[i].UnresolvedFile, parseInfo);
				}
				
				// Only if all parse runs succeeded, register the parse information.
				rwLock.EnterWriteLock();
				try {
					currentVersion = fileContent.Version;
					for (int i = 0; i < entries.Count; i++) {
						if (fullParseInformationRequested || (entries[i].CachedParseInformation != null && results[i].NewParseInformation.IsFullParseInformation))
							entries[i] = new ProjectEntry(entries[i].Project, results[i].NewUnresolvedFile, results[i].NewParseInformation);
						else
							entries[i] = new ProjectEntry(entries[i].Project, results[i].NewUnresolvedFile, null);
						if (entries[i].Project != null)
							entries[i].Project.OnParseInformationUpdated(results[i]);
						parserService.RaiseParseInformationUpdated(results[i]);
					}
					result = entries[index];
				} finally {
					rwLock.ExitWriteLock();
				}
			} finally {
				rwLock.ExitUpgradeableReadLock();
			}
			parserService.RegisterForCacheExpiry(this);
			return result;
		}

		ParseInformation ParseWithExceptionHandling(ITextSource fileContent, bool fullParseInformationRequested, IProject project, CancellationToken cancellationToken)
		{
			Debug.Assert(rwLock.IsUpgradeableReadLockHeld && !rwLock.IsWriteLockHeld);
			#if DEBUG
			if (Debugger.IsAttached)
				return parser.Parse(fileName, fileContent, fullParseInformationRequested, project, cancellationToken);
			#endif
			try {
				return parser.Parse(fileName, fileContent, fullParseInformationRequested, project, cancellationToken);
			} catch (Exception ex) {
				SD.Log.Error("Got " + ex.GetType().Name + " while parsing " + fileName);
				throw;
			}
		}
		#endregion
		
		#region ParseAsync
		/// <summary>lock object for protecting the runningAsyncParse* fields</summary>
		object runningAsyncParseLock = new object();
		Task<ProjectEntry> runningAsyncParseTask;
		ITextSourceVersion runningAsyncParseFileContentVersion;
		IProject runningAsyncParseProject;
		bool runningAsyncParseFullInfoRequested;
		
		public async Task<ParseInformation> ParseAsync(ITextSource fileContent, IProject parentProject, CancellationToken cancellationToken)
		{
			return (await DoParseAsync(fileContent, parentProject, true, cancellationToken)).CachedParseInformation;
		}
		
		public async Task<IUnresolvedFile> ParseFileAsync(ITextSource fileContent, IProject parentProject, CancellationToken cancellationToken)
		{
			return (await DoParseAsync(fileContent, parentProject, false, cancellationToken)).UnresolvedFile;
		}
		
		Task<ProjectEntry> DoParseAsync(ITextSource fileContent, IProject parentProject, bool requestFullParseInformation, CancellationToken cancellationToken)
		{
			// Create snapshot of file content, if required
			bool lookupOpenFileOnTargetThread;
			if (fileContent != null) {
				lookupOpenFileOnTargetThread = false;
				// File content was explicitly specified:
				// Let's make a snapshot in case the text source is mutable.
				fileContent = fileContent.CreateSnapshot();
			} else if (SD.MainThread.InvokeRequired) {
				// fileContent == null && not on the main thread:
				// Don't fetch the file content right now; if we need to SafeThreadCall() anyways,
				// it's better to do so from the background task.
				lookupOpenFileOnTargetThread = true;
			} else {
				// fileContent == null && we are on the main thread:
				// Let's look up the file in the list of open files right now
				// so that we don't need to SafeThreadCall() later on.
				lookupOpenFileOnTargetThread = false;
				if (parser != null) {
					fileContent = parser.GetFileContent(fileName);
				}
			}
			Task<ProjectEntry> task;
			lock (runningAsyncParseLock) {
				if (fileContent != null) {
					// Optimization:
					// don't start a background task if fileContent was specified and up-to-date parse info is available
					rwLock.EnterReadLock();
					try {
						int index = FindIndexForProject(parentProject);
						int versionComparison = CompareVersions(fileContent.Version);
						if (versionComparison == 0 && index >= 0) {
							// Ensure we have parse info for the specified project (entry.UnresolvedFile is null for newly registered projects)
							// If full parse info is requested, ensure we have full parse info.
							if (entries[index].UnresolvedFile != null && !(requestFullParseInformation && entries[index].CachedParseInformation == null)) {
								// We already have the requested version parsed, just return it:
								return Task.FromResult(entries[index]);
							}
						}
					} finally {
						rwLock.ExitReadLock();
					}
					// Optimization:
					// if an equivalent task is already running, return that one instead
					if (runningAsyncParseTask != null && (!requestFullParseInformation || runningAsyncParseFullInfoRequested)
					    && runningAsyncParseProject == parentProject
					    && runningAsyncParseFileContentVersion.BelongsToSameDocumentAs(fileContent.Version)
					    && runningAsyncParseFileContentVersion.CompareAge(fileContent.Version) == 0)
					{
						return runningAsyncParseTask;
					}
				}
				task = new Task<ProjectEntry>(
					delegate {
						try {
							if (lookupOpenFileOnTargetThread) {
								fileContent = SD.FileService.GetFileContentForOpenFile(fileName);
							}
							return DoParse(fileContent, parentProject, requestFullParseInformation, cancellationToken);
						} finally {
							lock (runningAsyncParseLock) {
								runningAsyncParseTask = null;
								runningAsyncParseFileContentVersion = null;
								runningAsyncParseProject = null;
							}
						}
					}, cancellationToken);
				if (fileContent != null && fileContent.Version != null && !cancellationToken.CanBeCanceled) {
					runningAsyncParseTask = task;
					runningAsyncParseFileContentVersion = fileContent.Version;
					runningAsyncParseProject = parentProject;
					runningAsyncParseFullInfoRequested = requestFullParseInformation;
				}
			}
			task.Start();
			return task;
		}
		#endregion
		
		public void RegisterUnresolvedFile(IProject project, IUnresolvedFile unresolvedFile)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (unresolvedFile == null)
				throw new ArgumentNullException("unresolvedFile");
			FreezableHelper.Freeze(unresolvedFile);
			var newParseInfo = new ParseInformation(unresolvedFile, null, false);
			rwLock.EnterWriteLock();
			try {
				int index = FindIndexForProject(project);
				if (index >= 0) {
					currentVersion = null;
					var args = new ParseInformationEventArgs(project, entries[index].UnresolvedFile, newParseInfo);
					entries[index] = new ProjectEntry(project, unresolvedFile, null);
					project.OnParseInformationUpdated(args);
					parserService.RaiseParseInformationUpdated(args);
				}
			} finally {
				rwLock.ExitWriteLock();
			}
		}
	}
}
