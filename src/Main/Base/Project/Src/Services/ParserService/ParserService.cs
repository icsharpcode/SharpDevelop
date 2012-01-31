// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Parser
{
	/// <summary>
	/// Stores the compilation units for files.
	/// </summary>
	public static class ParserService
	{
		static readonly object syncLock = new object();
		static IList<ParserDescriptor> parserDescriptors;
		//static Dictionary<IProjectContent, IProject> projectContents = new Dictionary<IProjectContent, IProject>();
		static Dictionary<FileName, FileEntry> fileEntryDict = new Dictionary<FileName, FileEntry>();
		
		#region Manage Project Contents
		/// <summary>
		/// Gets or creates a compilation for the specified project.
		/// </summary>
		public static ICompilation GetCompilation(IProject project)
		{
			return GetCurrentSolutionSnapshot().GetCompilation(project);
		}
		
		/// <summary>
		/// Gets or creates a compilation for the project that contains the specified file.
		/// Returns a dummy compilation if there is no project for that file.
		/// </summary>
		public static ICompilation GetCompilationForFile(FileName fileName)
		{
			return GetCurrentSolutionSnapshot().GetCompilation(ProjectService.OpenSolution.FindProjectContainingFile(fileName));
		}
		
		/// <summary>
		/// Gets a snapshot of the current compilations. This method is useful when a consistent snapshot
		/// across multiple compilations is needed.
		/// </summary>
		public static SharpDevelopSolutionSnapshot GetCurrentSolutionSnapshot()
		{
			return new SharpDevelopSolutionSnapshot();
		}
		
		[Obsolete("Use project.ProjectContent instead")]
		public static IProjectContent GetProjectContent(IProject project)
		{
			return project.ProjectContent;
		}
		
		/// <summary>
		/// Gets the project that owns the specified project content.
		/// Returns null for referenced assemblies.
		/// </summary>
		public static IProject GetProject(IProjectContent projectContent)
		{
			if (projectContent == null)
				return null;
			if (ProjectService.OpenSolution != null) {
				foreach (var project in ProjectService.OpenSolution.Projects) {
					if (project.ProjectContent == projectContent)
						return project;
				}
			}
			return null;
		}
		#endregion
		
		#region Initialization + ParserThread
		internal static void InitializeParserService()
		{
			if (parserDescriptors == null) {
				parserDescriptors = AddInTree.BuildItems<ParserDescriptor>("/Workspace/Parser", null, false);
			}
		}
		
		static DispatcherTimer timer;
		
		internal static void StartParserThread()
		{
			WorkbenchSingleton.DebugAssertMainThread();
			timer = new DispatcherTimer(DispatcherPriority.Background);
			timer.Interval = TimeSpan.FromSeconds(1.5);
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();
		}

		internal static void StopParserThread()
		{
			timer.Stop();
		}
		
		static System.Threading.Tasks.Task lastParseRun;
		
		static void timer_Tick(object sender, EventArgs e)
		{
			if (lastParseRun != null) {
				// don't start another parse run if the last one is still running
				if (!lastParseRun.IsCompleted)
					return;
				lastParseRun = null;
			}
			
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (viewContent == null)
				return;
			FileName fileName = viewContent.PrimaryFileName;
			if (fileName == null)
				return;
			if (!HasParser(fileName))
				return;
			
			ITextSource snapshot;
			IEditable editable = viewContent as IEditable;
			if (editable != null)
				snapshot = editable.CreateSnapshot();
			else
				snapshot = GetParseableFileContent(viewContent.PrimaryFileName);
			
			lastParseRun = ParseAsync(fileName, snapshot).ContinueWith(
				delegate(Task<ParseInformation> backgroundTask) {
					ParseInformation parseInfo = backgroundTask.Result;
					RaiseParserUpdateStepFinished(new ParserUpdateStepEventArgs(fileName, snapshot, parseInfo));
				});
		}
		#endregion
		
		#region CreateParser / TaskListTokens
		static readonly string[] DefaultTaskListTokens = {"HACK", "TODO", "UNDONE", "FIXME"};
		
		/// <summary>
		/// Gets/Sets the task list tokens.
		/// This property is thread-safe.
		/// </summary>
		public static string[] TaskListTokens {
			get { return PropertyService.Get("SharpDevelop.TaskListTokens", DefaultTaskListTokens); }
			set { PropertyService.Set("SharpDevelop.TaskListTokens", value); }
		}
		
		/// <summary>
		/// Creates a new IParser instance that can parse the specified file.
		/// This method is thread-safe.
		/// </summary>
		public static IParser CreateParser(FileName fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (parserDescriptors == null)
				return null;
			foreach (ParserDescriptor descriptor in parserDescriptors) {
				if (descriptor.CanParse(fileName)) {
					IParser p = descriptor.CreateParser();
					if (p != null) {
						p.LexerTags = TaskListTokens;
						return p;
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Gets whether a parser is registered for the specified file name.
		/// </summary>
		/// <remarks>GetParser()/CreateParser() can still return null when HasParser() returns true
		/// if there is an error during parser creation (e.g. incorrectly registered AddIn)</remarks>
		public static bool HasParser(FileName fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (parserDescriptors == null)
				return false;
			foreach (ParserDescriptor descriptor in parserDescriptors) {
				if (descriptor.CanParse(fileName)) {
					return true;
				}
			}
			return false;
		}
		#endregion
		
		#region GetParseableFileContent
		/// <summary>
		/// Gets the default file encoding.
		/// This property is thread-safe.
		/// </summary>
		public static Encoding DefaultFileEncoding {
			get {
				return Encoding.GetEncoding(FileService.DefaultFileEncodingCodePage);
			}
		}
		
		/// <summary>
		/// Gets the content of the specified file.
		/// This method is thread-safe. This method involves waiting for the main thread, so using it while
		/// holding a lock can lead to deadlocks.
		/// </summary>
		public static ITextSource GetParseableFileContent(FileName fileName)
		{
			ITextSource fileContent = Gui.WorkbenchSingleton.SafeThreadFunction(GetParseableFileContentForOpenFile, fileName);
			if (fileContent != null)
				return fileContent;
			else
				return new StringTextSource(ICSharpCode.AvalonEdit.Utils.FileReader.ReadFileContent(fileName, DefaultFileEncoding));
		}
		
		public static ITextSource GetParseableFileContent(string fileName)
		{
			return GetParseableFileContent(FileName.Create(fileName));
		}
		
		/// <summary>
		/// Gets the file content for a file that is currently open.
		/// Returns null if the file is not open.
		/// </summary>
		static ITextSource GetParseableFileContentForOpenFile(FileName fileName)
		{
			OpenedFile file = FileService.GetOpenedFile(fileName);
			if (file != null) {
				IFileDocumentProvider p = file.CurrentView as IFileDocumentProvider;
				if (p != null) {
					IDocument document = p.GetDocumentForFile(file);
					if (document != null) {
						return document.CreateSnapshot();
					}
				}
				
				using(Stream s = file.OpenRead()) {
					// load file
					return new StringTextSource(ICSharpCode.AvalonEdit.Utils.FileReader.ReadFileContent(s, DefaultFileEncoding));
				}
			}
			return null;
		}
		#endregion
		
		#region Parse Information Management
		sealed class FileEntry
		{
			readonly FileName fileName;
			internal readonly IParser parser;
			volatile IParsedFile mainParsedFile;
			volatile ParseInformation cachedParseInformation;
			ITextSourceVersion bufferVersion;
			bool disposed;
			
			public FileEntry(FileName fileName)
			{
				this.fileName = fileName;
				this.parser = CreateParser(fileName);
			}
			
			/// <summary>
			/// Intended for unit tests only
			/// </summary>
			public void RegisterParseInformation(ParseInformation parseInfo)
			{
				lock (this) {
					this.mainParsedFile = (parseInfo != null) ? parseInfo.ParsedFile : null;
					if (parseInfo != null && parseInfo.IsFullParseInformation)
						this.cachedParseInformation = parseInfo;
					else
						this.cachedParseInformation = null;
				}
			}
			
			public ParseInformation GetCachedParseInformation()
			{
				return cachedParseInformation; // read volatile
			}
			
			public ParseInformation GetCachedParseInformation(ITextSourceVersion version)
			{
				if (version == null)
					return GetCachedParseInformation();
				lock (this) {
					if (bufferVersion != null && bufferVersion.BelongsToSameDocumentAs(version)) {
						if (bufferVersion.CompareAge(version) >= 0)
							return cachedParseInformation;
					}
				}
				return null;
			}
			
			public IParsedFile GetExistingParsedFile()
			{
				return this.mainParsedFile; // read volatile
			}
			
			public ParseInformation Parse(ITextSource fileContent)
			{
				if (fileContent == null) {
					fileContent = WorkbenchSingleton.SafeThreadFunction(GetParseableFileContentForOpenFile, fileName);
				}
				
				ParseInformation parseInfo;
				IParsedFile parsedFile;
				DoParse(fileContent, true, out parseInfo, out parsedFile);
				return parseInfo;
			}
			
			public IParsedFile ParseFile(ITextSource fileContent)
			{
				if (fileContent == null) {
					fileContent = WorkbenchSingleton.SafeThreadFunction(GetParseableFileContentForOpenFile, fileName);
				}
				
				ParseInformation parseInfo;
				IParsedFile parsedFile;
				DoParse(fileContent, false, out parseInfo, out parsedFile);
				return parsedFile;
			}
			
			void DoParse(ITextSource fileContent,
			             bool fullParseInformationRequested, out ParseInformation resultParseInfo, out IParsedFile resultUnit)
			{
				resultParseInfo = null;
				resultUnit = null;
				
				if (parser == null)
					return;
				
				if (fileContent == null) {
					// No file content was specified. Because the callers of this method already check for currently open files,
					// we can assume that the file isn't open and simply read it from disk.
					lock (this) {
						// Don't bother reading the file if this FileEntry was already disposed.
						if (this.disposed)
							return;
					}
					string fileAsString;
					try {
						fileAsString = ICSharpCode.AvalonEdit.Utils.FileReader.ReadFileContent(fileName, DefaultFileEncoding);
					} catch (IOException) {
						// It is possible that the file gets deleted/becomes inaccessible while a background parse
						// operation is enqueued, so we have to handle IO exceptions.
						return;
					} catch (UnauthorizedAccessException) {
						return;
					}
					fileContent = new StringTextSource(fileAsString);
				}
				
				ITextSourceVersion fileContentVersion = fileContent.Version;
				lock (this) {
					if (this.disposed)
						return;
					
					if (fileContentVersion != null && this.bufferVersion != null && this.bufferVersion.BelongsToSameDocumentAs(fileContentVersion)) {
						if (this.bufferVersion.CompareAge(fileContentVersion) >= 0) {
							// If full parse info is requested, ensure we have full parse info.
							if (!(fullParseInformationRequested && this.cachedParseInformation == null)) {
								resultParseInfo = this.cachedParseInformation;
								resultUnit = this.mainParsedFile;
								return;
							}
						}
					}
				} // exit lock
				
				// We now leave the lock to do the actual parsing.
				// This is done to allow IParser implementations to invoke methods on the main thread without
				// risking deadlocks.
				
				try {
					resultParseInfo = parser.Parse(fileName, fileContent, fullParseInformationRequested);
				} catch (Exception ex) {
					throw new ApplicationException("Error parsing " + fileName, ex);
				}
				if (resultParseInfo == null)
					throw new NullReferenceException(parser.GetType().Name + ".Parse() returned null");
				if (fullParseInformationRequested && !resultParseInfo.IsFullParseInformation)
					throw new InvalidOperationException(parser.GetType().Name + ".Parse() did not return full parse info as requested.");
				
				resultUnit = resultParseInfo.ParsedFile;
				// ensure the new unit is frozen before we make it visible to the outside world
				FreezableHelper.Freeze(resultUnit);
				
				lock (this) {
					if (this.disposed) {
						resultParseInfo = null;
						resultUnit = null;
						return;
					}
					
					// ensure we never go backwards in time (we need to repeat this check after we've reacquired the lock)
					if (fileContentVersion != null && this.bufferVersion != null && this.bufferVersion.BelongsToSameDocumentAs(fileContentVersion)) {
						if (this.bufferVersion.CompareAge(fileContentVersion) >= 0) {
							if (fullParseInformationRequested && this.cachedParseInformation == null) {
								// We must not go backwards in time, but the newer version that we have parsed
								// does not have full parse information.
								// Thus, we return the outdated (but "new enough") parse information that we found above,
								// but we won't register it.
								return;
							} else {
								resultParseInfo = this.cachedParseInformation;
								resultUnit = this.mainParsedFile;
								return;
							}
						}
					}
					
					#warning How to update the PCs?
					/*for (int i = 0; i < newUnits.Length; i++) {
						IProjectContent pc = projectContents[i];
						// update the compilation unit
						IParsedFile oldUnit = oldUnits.FirstOrDefault(o => o.ProjectContent == pc);
						// ensure the new unit is frozen beforewe make it visible to the outside world
						newUnits[i].Freeze();
						pc.UpdateProjectContent(oldUnit, newUnits[i]);
					}*/
					RaiseParseInformationUpdated(new ParseInformationEventArgs(mainParsedFile, resultParseInfo));
					
					/*
					// remove all old units that don't exist anymore
					foreach (IParsedFile oldUnit in oldUnits) {
						if (!newUnits.Any(n => n.ProjectContent == oldUnit.ProjectContent)) {
							oldUnit.ProjectContent.UpdateProjectContent(oldUnit, null);
							RaiseParseInformationUpdated(new ParseInformationEventArgs(oldUnit, null, false));
						}
					}*/
					
					this.bufferVersion = fileContentVersion;
					this.mainParsedFile = resultUnit;
					// Cached the new parse information around if it was requested, or if we had already cached parse information previously.
					if (resultParseInfo.IsFullParseInformation && (fullParseInformationRequested || this.cachedParseInformation != null)) {
						this.cachedParseInformation = resultParseInfo;
					} else {
						this.cachedParseInformation = null;
					}
				} // exit lock
			}
			
			public void Clear()
			{
				IParsedFile parseInfo;
				lock (this) {
					// by setting the disposed flag, we'll cause all running ParseFile() calls to return null and not
					// call into the parser anymore, so we can do the remainder of the clean-up work outside the lock
					this.disposed = true;
					parseInfo = this.mainParsedFile;
					this.bufferVersion = null;
					this.mainParsedFile = null;
				}
				/*foreach (IParsedFile oldUnit in oldUnits) {
					oldUnit.ProjectContent.UpdateProjectContent(oldUnit, null);
					bool isPrimary = parseInfo == oldUnit;
				}*/
				RaiseParseInformationUpdated(new ParseInformationEventArgs(parseInfo, null));
			}
			
			void SnapshotFileContentForAsyncOperation(ref ITextSource fileContent, out bool lookupOpenFileOnTargetThread)
			{
				if (fileContent != null) {
					lookupOpenFileOnTargetThread = false;
					// File content was explicitly specified:
					// Let's make a snapshot in case the text source is mutable.
					fileContent = fileContent.CreateSnapshot();
				} else if (WorkbenchSingleton.InvokeRequired) {
					// fileContent == null && not on the main thread:
					// Don't fetch the file content right now; if we need to SafeThreadCall() anyways,
					// it's better to do so from the background task.
					lookupOpenFileOnTargetThread = true;
				} else {
					// fileContent == null && we are on the main thread:
					// Let's look up the file in the list of open files right now
					// so that we don't need to SafeThreadCall() later on.
					lookupOpenFileOnTargetThread = false;
					fileContent = GetParseableFileContentForOpenFile(fileName);
				}
			}
			
			Task<ParseInformation> runningAsyncParseTask;
			ITextSourceVersion runningAsyncParseFileContentVersion;
			
			public Task<ParseInformation> ParseAsync(ITextSource fileContent)
			{
				bool lookupOpenFileOnTargetThread;
				SnapshotFileContentForAsyncOperation(ref fileContent, out lookupOpenFileOnTargetThread);
				
				ITextSourceVersion fileContentVersion = fileContent != null ? fileContent.Version : null;
				if (fileContentVersion != null) {
					// Optimization:
					// don't start a background task if fileContent was specified and up-to-date parse info is available
					lock (this) {
						if (cachedParseInformation != null && bufferVersion != null && bufferVersion.BelongsToSameDocumentAs(fileContentVersion)) {
							if (bufferVersion.CompareAge(fileContentVersion) >= 0) {
								return TaskFromResult(cachedParseInformation);
							}
						}
					}
				}
				
				var task = new Task<ParseInformation>(
					delegate {
						try {
							if (lookupOpenFileOnTargetThread) {
								fileContent = WorkbenchSingleton.SafeThreadFunction(GetParseableFileContentForOpenFile, fileName);
							}
							ParseInformation parseInfo;
							IParsedFile parsedFile;
							DoParse(fileContent, true, out parseInfo, out parsedFile);
							return parseInfo;
						} catch (Exception ex) {
							MessageService.ShowException(ex, "Error during async parse");
							return null;
						} finally {
							lock (this) {
								this.runningAsyncParseTask = null;
								this.runningAsyncParseFileContentVersion = null;
							}
						}
					}
				);
				if (fileContentVersion != null) {
					// Optimization: when additional async parse runs are requested while the parser is already
					// running for that file content, return the task that's already running
					// instead of starting additional copies.
					lock (this) {
						if (runningAsyncParseTask != null && runningAsyncParseFileContentVersion.BelongsToSameDocumentAs(fileContentVersion)) {
							if (runningAsyncParseFileContentVersion.CompareAge(fileContentVersion) >= 0)
								return runningAsyncParseTask;
						}
						this.runningAsyncParseTask = task;
						this.runningAsyncParseFileContentVersion = fileContentVersion;
					}
				}
				task.Start();
				return task;
			}
			
			public Task<IParsedFile> ParseFileAsync(ITextSource fileContent)
			{
				bool lookupOpenFileOnTargetThread;
				SnapshotFileContentForAsyncOperation(ref fileContent, out lookupOpenFileOnTargetThread);
				
				// TODO: don't use background task if fileContent was specified and up-to-date parse info is available
				return System.Threading.Tasks.Task.Factory.StartNew(
					delegate {
						try {
							if (lookupOpenFileOnTargetThread) {
								fileContent = WorkbenchSingleton.SafeThreadFunction(GetParseableFileContentForOpenFile, fileName);
							}
							ParseInformation parseInfo;
							IParsedFile parsedFile;
							DoParse(fileContent, false, out parseInfo, out parsedFile);
							return parsedFile;
						} catch (Exception ex) {
							MessageService.ShowException(ex, "Error during async parse");
							return null;
						}
					}
				);
			}
		}
		
		static FileEntry GetFileEntry(FileName fileName, bool createOnDemand)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			
			FileEntry entry;
			lock (syncLock) {
				if (!fileEntryDict.TryGetValue(fileName, out entry)) {
					if (!createOnDemand)
						return null;
					entry = new FileEntry(fileName);
					fileEntryDict.Add(fileName, entry);
				}
			}
			return entry;
		}
		
		/// <summary>
		/// Removes all parse information (both IParsedFile and ParseInformation) for the specified file.
		/// This method is thread-safe.
		/// </summary>
		public static void ClearParseInformation(FileName fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			
			LoggingService.Info("ClearParseInformation: " + fileName);
			
			FileEntry entry;
			lock (syncLock) {
				if (fileEntryDict.TryGetValue(fileName, out entry)) {
					fileEntryDict.Remove(fileName);
				}
			}
			if (entry != null)
				entry.Clear();
		}
		
		/// <summary>
		/// This is the old method returning potentially-stale parse information.
		/// Use Parse()/ParseFile() instead if you need fresh parse info; otherwise use GetExistingParsedFile().
		/// </summary>
		[Obsolete("Use Parse()/ParseFile() instead if you need fresh parse info; otherwise use GetExistingParsedFile().")]
		public static ParseInformation GetParseInformation(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return null;
			return Parse(FileName.Create(fileName));
		}
		
		/// <summary>
		/// Gets full parse information for the specified file, if it is available.
		/// </summary>
		/// <returns>
		/// Returns the ParseInformation for the specified file,
		/// or null if it is not in the parse information cache.
		/// 
		/// If only the IParsedFile is available (non-full parse information), this method
		/// returns null.
		/// </returns>
		/// <remarks>
		/// This method is thread-safe.
		/// 
		/// The ParserService may drop elements from the cache at any moment,
		/// only IParsedFile will be stored for a longer time.
		/// </remarks>
		public static ParseInformation GetCachedParseInformation(FileName fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return null;
			FileEntry entry = GetFileEntry(fileName, false);
			if (entry != null)
				return entry.GetCachedParseInformation();
			else
				return null;
		}
		
		/// <summary>
		/// Gets full parse information for the specified file, if it is available and at least as recent as the specified version.
		/// </summary>
		/// <returns>
		/// If only the IParsedFile is available (non-full parse information), this method
		/// returns null.
		/// If parse information is avaiable but potentially outdated (older than <paramref name="version"/>,
		/// or belonging to a different document), this method returns null.
		/// </returns>
		public static ParseInformation GetCachedParseInformation(FileName fileName, ITextSourceVersion version)
		{
			if (string.IsNullOrEmpty(fileName))
				return null;
			FileEntry entry = GetFileEntry(fileName, false);
			if (entry != null)
				return entry.GetCachedParseInformation(version);
			else
				return null;
		}
		
		/// <summary>
		/// Gets parse information for the specified file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns>
		/// Returns the IParsedFile for the specified file,
		/// or null if the file has not been parsed yet.
		/// </returns>
		/// <remarks>This method is thread-safe.</remarks>
		public static IParsedFile GetExistingParsedFile(FileName fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return null;
			FileEntry entry = GetFileEntry(fileName, false);
			if (entry != null)
				return entry.GetExistingParsedFile();
			else
				return null;
		}
		
		/// <summary>
		/// Parses the specified file.
		/// Produces full parse information.
		/// </summary>
		/// <param name="fileName">Name of the file to parse</param>
		/// <param name="fileContent">Optional: Content of the file to parse.
		/// The fileContent is taken as a hint - if a newer version than it is already available, that will be used instead.
		/// </param>
		/// <returns>
		/// Returns the ParseInformation for the specified file, or null if the file cannot be parsed.
		/// For files currently open in an editor, this method does not necessary reparse, but may return
		/// an existing cached parse information (but only if it's still up-to-date).
		/// </returns>
		/// <remarks>
		/// This method is thread-safe. This parser being used may involve locking or waiting for the main thread,
		/// so using this method while holding a lock can lead to deadlocks.
		/// </remarks>
		public static ParseInformation Parse(FileName fileName, ITextSource fileContent = null)
		{
			return GetFileEntry(fileName, true).Parse(fileContent);
		}
		
		/// <summary>
		/// Asynchronous version of <see cref="Parse(FileName, ITextSource)"/>.
		/// </summary>
		/// <param name="fileName">Name of the file to parse</param>
		/// <param name="fileContent">Optional: Content of the file to parse.
		/// The fileContent is taken as a hint - if a newer version than it is already available, that will be used instead.
		/// </param>
		/// <returns>
		/// Returns the ParseInformation for the specified file, or null if the file cannot be parsed.
		/// For files currently open in an editor, this method does not necessary reparse, but may return
		/// an existing cached parse information (but only if it's still up-to-date).
		/// </returns>
		/// <remarks>
		/// This method is thread-safe. This parser being used may involve locking or waiting for the main thread,
		/// so waiting for the task can cause deadlocks.
		/// </remarks>
		public static Task<ParseInformation> ParseAsync(FileName fileName, ITextSource fileContent = null)
		{
			return GetFileEntry(fileName, true).ParseAsync(fileContent);
		}
		
		/// <summary>
		/// Parses the specified file.
		/// This method does not request full parse information
		/// </summary>
		/// <param name="fileName">Name of the file to parse</param>
		/// <param name="fileContent">Optional: Content of the file to parse.
		/// The fileContent is taken as a hint - if a newer version than it is already available, that will be used instead.
		/// </param>
		/// <returns>
		/// Returns the IParsedFile for the specified file, or null if the file cannot be parsed.
		/// For files currently open in an editor, this method does not necessary reparse, but may return
		/// the existing IParsedFile (but only if it's still up-to-date).
		/// </returns>
		/// <remarks>
		/// This method is thread-safe. This parser being used may involve locking or waiting for the main thread,
		/// so using this method while holding a lock can lead to deadlocks.
		/// </remarks>
		public static IParsedFile ParseFile(FileName fileName, ITextSource fileContent = null)
		{
			return GetFileEntry(fileName, true).ParseFile(fileContent);
		}
		
		/// <summary>
		/// Async version of ParseFile().
		/// </summary>
		public static Task<IParsedFile> ParseFileAsync(FileName fileName, ITextSource fileContent = null)
		{
			return GetFileEntry(fileName, true).ParseFileAsync(fileContent);
		}
		
		/// <summary>
		/// Parses the current view content.
		/// This method can only be called from the main thread.
		/// </summary>
		public static ParseInformation ParseCurrentViewContent()
		{
			WorkbenchSingleton.AssertMainThread();
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (viewContent != null)
				return ParseViewContent(viewContent);
			else
				return null;
		}
		
		/// <summary>
		/// Parses the specified view content.
		/// This method can only be called from the main thread.
		/// </summary>
		public static ParseInformation ParseViewContent(IViewContent viewContent)
		{
			if (viewContent == null)
				throw new ArgumentNullException("viewContent");
			WorkbenchSingleton.AssertMainThread();
			if (string.IsNullOrEmpty(viewContent.PrimaryFileName))
				return null;
			IEditable editable = viewContent as IEditable;
			if (editable != null)
				return Parse(viewContent.PrimaryFileName, editable.CreateSnapshot());
			else
				return Parse(viewContent.PrimaryFileName);
		}
		
		/// <summary>
		/// Gets the parser instance that is responsible for the specified file.
		/// Will create a new IParser instance on demand.
		/// This method is thread-safe.
		/// </summary>
		public static IParser GetParser(FileName fileName)
		{
			return GetFileEntry(fileName, true).parser;
		}
		
		/// <summary>
		/// Registers a compilation unit in the parser service.
		/// Does not fire the OnParseInformationUpdated event, please use this for unit tests only!
		/// </summary>
		public static void RegisterParseInformation(ParseInformation parseInfo)
		{
			FileEntry entry = GetFileEntry(parseInfo.FileName, true);
			entry.RegisterParseInformation(parseInfo);
		}
		
		/// <summary>
		/// Replaces the list of available parsers.
		/// Please use this for unit tests only!
		/// </summary>
		public static void RegisterAvailableParsers(params ParserDescriptor[] descriptors)
		{
			lock (syncLock) {
				parserDescriptors = new List<ParserDescriptor>();
				parserDescriptors.AddRange(descriptors);
			}
		}
		
		#endregion
		
		#region ParseInformationUpdated / ParserUpdateStepFinished events
		/// <summary>
		/// Occurs whenever parse information was updated. This event is raised on the main thread.
		/// </summary>
		public static event EventHandler<ParseInformationEventArgs> ParseInformationUpdated = delegate {};
		
		static void RaiseParseInformationUpdated(ParseInformationEventArgs e)
		{
			// RaiseParseInformationUpdated is called inside a lock, but we don't want to raise the event inside that lock.
			// To ensure events are raised in the same order, we always invoke on the main thread.
			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					string addition;
					if (e.OldParsedFile == null)
						addition = " (new)";
					else if (e.NewParsedFile == null)
						addition = " (removed)";
					else
						addition = " (updated)";
					LoggingService.Debug("ParseInformationUpdated " + e.FileName + addition);
					ParseInformationUpdated(null, e);
				});
		}
		
		/// <summary>
		/// Occurs when the parse step started by a timer finishes.
		/// This event is raised on the main thread.
		/// </summary>
		public static event EventHandler<ParserUpdateStepEventArgs> ParserUpdateStepFinished = delegate {};
		
		static void RaiseParserUpdateStepFinished(ParserUpdateStepEventArgs e)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					ParserUpdateStepFinished(null, e);
				});
		}
		
		#endregion
		
		#region LoadSolutionProjects
		
		/// <summary>
		/// Gets whether the LoadSolutionProjects thread is currently running.
		/// </summary>
		public static bool LoadSolutionProjectsThreadRunning {
			get { return LoadSolutionProjects.IsThreadRunning; }
		}
		
		/// <summary>
		/// Occurs when the 'load solution projects' thread has finished.
		/// This event is not raised when the 'load solution projects' is aborted because the solution was closed.
		/// This event is raised on the main thread.
		/// </summary>
		public static event EventHandler LoadSolutionProjectsThreadEnded {
			add { LoadSolutionProjects.ThreadEnded += value; }
			remove { LoadSolutionProjects.ThreadEnded -= value; }
		}
		
		internal static void OnSolutionLoaded()
		{
			//foreach (IProject project in ProjectService.OpenSolution.Projects) {
			//	RegisterProjectContentForAddedProject(project);
			//}
		}
		
		internal static void OnSolutionClosed()
		{
			LoadSolutionProjects.CancelAllJobs();
			lock (syncLock) {
				//projectContents.Clear();
			}
			ClearAllFileEntries();
		}
		
		static void ClearAllFileEntries()
		{
			FileEntry[] entries;
			lock (syncLock) {
				entries = fileEntryDict.Values.ToArray();
				fileEntryDict.Clear();
			}
			foreach (FileEntry entry in entries)
				entry.Clear();
		}
		#endregion
		
		public static ResolveResult Resolve(FileName fileName, TextLocation location, ITextSource fileContent = null,
		                                    CancellationToken cancellationToken = default(CancellationToken))
		{
			var entry = GetFileEntry(fileName, true);
			if (entry.parser == null)
				return null;
			var parseInfo = entry.Parse(fileContent);
			if (parseInfo == null)
				return null;
			var compilation = GetCompilationForFile(fileName);
			ResolveResult rr = entry.parser.Resolve(parseInfo, location, compilation, cancellationToken);
			LoggingService.Debug("Resolved " + location + " to " + rr);
			return rr;
		}
		
		public static Task<ResolveResult> ResolveAsync(FileName fileName, TextLocation location, ITextSource fileContent = null,
		                                               CancellationToken cancellationToken = default(CancellationToken))
		{
			var entry = GetFileEntry(fileName, true);
			if (entry.parser == null)
				return TaskFromResult<ResolveResult>(null);
			return entry.ParseAsync(fileContent).ContinueWith(
				delegate (Task<ParseInformation> parseInfoTask) {
					var parseInfo = parseInfoTask.Result;
					if (parseInfo == null)
						return null;
					var compilation = GetCompilationForFile(fileName);
					ResolveResult rr = entry.parser.Resolve(parseInfo, location, compilation, cancellationToken);
					LoggingService.Debug("Resolved " + location + " to " + rr);
					return rr;
				}, cancellationToken);
		}
		
		static Task<T> TaskFromResult<T>(T result)
		{
			TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
			tcs.SetResult(result);
			return tcs.Task;
		}
	}
}
