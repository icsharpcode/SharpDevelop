// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading.Tasks;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Stores the compilation units for files.
	/// </summary>
	public static class ParserService
	{
		static readonly object syncLock = new object();
		static IList<ParserDescriptor> parserDescriptors;
		static Dictionary<IProject, IProjectContent> projectContents = new Dictionary<IProject, IProjectContent>();
		static Dictionary<FileName, FileEntry> fileEntryDict = new Dictionary<FileName, FileEntry>();
		static DefaultProjectContent defaultProjectContent;
		
		#region Manage Project Contents
		/// <summary>
		/// Fetches the current project content.
		/// </summary>
		public static IProjectContent CurrentProjectContent {
			[DebuggerStepThrough]
			get {
				IProject currentProject = ProjectService.CurrentProject;
				lock (syncLock) {
					if (currentProject == null || !projectContents.ContainsKey(currentProject)) {
						return DefaultProjectContent;
					}
					return projectContents[currentProject];
				}
			}
		}
		
		public static IProjectContent GetProjectContent(IProject project)
		{
			lock (projectContents) {
				IProjectContent pc;
				if (projectContents.TryGetValue(project, out pc)) {
					return pc;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Gets the list of project contents of all open projects. Does not include assembly project contents.
		/// </summary>
		public static IEnumerable<IProjectContent> AllProjectContents {
			get {
				lock (syncLock) {
					return projectContents.Values.ToArray();
				}
			}
		}
		
		/// <summary>
		/// Gets the default project content used for files outside of projects.
		/// </summary>
		public static IProjectContent DefaultProjectContent {
			get {
				lock (syncLock) {
					if (defaultProjectContent == null) {
						CreateDefaultProjectContent();
					}
					return defaultProjectContent;
				}
			}
		}
		
		static void CreateDefaultProjectContent()
		{
			LoggingService.Info("Creating default project content");
			//LoggingService.Debug("Stacktrace is:\n" + Environment.StackTrace);
			defaultProjectContent = new DefaultProjectContent();
			defaultProjectContent.AddReferencedContent(AssemblyParserService.DefaultProjectContentRegistry.Mscorlib);
		}
		
		/// <summary>
		/// Gets all project contents that contain the specified file.
		/// </summary>
		static List<IProjectContent> GetProjectContents(string fileName)
		{
			List<IProjectContent> result = new List<IProjectContent>();
			List<IProjectContent> linkResults = new List<IProjectContent>();
			lock (projectContents) {
				foreach (KeyValuePair<IProject, IProjectContent> projectContent in projectContents) {
					FileProjectItem file = projectContent.Key.FindFile(fileName);
					if (file != null) {
						// Prefer normal files over linked files.
						// The order matters because GetParseInformation() will return the ICompilationUnit
						// for the first result.
						if (file.IsLink)
							linkResults.Add(projectContent.Value);
						else
							result.Add(projectContent.Value);
					}
				}
			}
			result.AddRange(linkResults);
			if (result.Count == 0)
				result.Add(DefaultProjectContent);
			return result;
		}
		
		internal static void RemoveProjectContentForRemovedProject(IProject project)
		{
			lock (projectContents) {
				projectContents.Remove(project);
			}
		}
		#endregion
		
		#region Initialization + ParserThread
		internal static void InitializeParserService()
		{
			if (parserDescriptors == null) {
				parserDescriptors = AddInTree.BuildItems<ParserDescriptor>("/Workspace/Parser", null, false);
				AssemblyParserService.Initialize();
				LoadSolutionProjects.Initialize();
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
			if (GetParser(fileName) == null)
				return;
			
			ITextBuffer snapshot;
			IEditable editable = viewContent as IEditable;
			if (editable != null)
				snapshot = editable.CreateSnapshot();
			else
				snapshot = GetParseableFileContent(viewContent.PrimaryFileName);
			
			lastParseRun = BeginParse(fileName, snapshot).ContinueWith(
				delegate(Task<ParseInformation> backgroundTask) {
					ParseInformation parseInfo = backgroundTask.Result;
					RaiseParserUpdateStepFinished(new ParserUpdateStepEventArgs(fileName, snapshot, parseInfo));
				});
		}
		#endregion
		
		#region GetParser / ExpressionFinder / Resolve / etc.
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
		public static IParser CreateParser(string fileName)
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
		/// Creates an IExpressionFinder instance for the specified file.
		/// This method is thread-safe.
		/// </summary>
		public static IExpressionFinder GetExpressionFinder(string fileName)
		{
			IParser parser = CreateParser(fileName);
			if (parser != null) {
				return parser.CreateExpressionFinder(fileName);
			}
			return null;
		}
		
		public static IResolver CreateResolver(string fileName)
		{
			IParser parser = CreateParser(fileName);
			if (parser != null) {
				return parser.CreateResolver();
			}
			return null;
		}
		
		/// <summary>
		/// Resolves given ExpressionResult.
		/// </summary>
		public static ResolveResult Resolve(ExpressionResult expressionResult,
		                                    int caretLineNumber, int caretColumn,
		                                    string fileName, string fileContent)
		{
			if (expressionResult.Region.IsEmpty) {
				expressionResult.Region = new DomRegion(caretLineNumber, caretColumn);
			}
			IResolver resolver = CreateResolver(fileName);
			if (resolver != null) {
				ParseInformation parseInfo = GetParseInformation(fileName);
				return resolver.Resolve(expressionResult, parseInfo, fileContent);
			}
			return null;
		}
		
		/// <summary>
		/// Resolves expression at given position.
		/// That is, finds ExpressionResult at that position and
		/// calls the overload Resolve(ExpressionResult,...).
		/// </summary>
		public static ResolveResult Resolve(int caretLine, int caretColumn, IDocument document, string fileName)
		{
			var expressionResult = FindFullExpression(caretLine, caretColumn, document, fileName);
			string expression = (expressionResult.Expression ?? "").Trim();
			if (expression.Length > 0) {
				return Resolve(expressionResult, caretLine, caretColumn, fileName, document.Text);
			} else
				return null;
		}

		public static ResolveResult Resolve(int offset, IDocument document, string fileName)
		{
			var position = document.OffsetToPosition(offset);
			return Resolve(position.Line, position.Column, document, fileName);
		}
		
		public static ExpressionResult FindFullExpression(int caretLine, int caretColumn, IDocument document, string fileName)
		{
			IExpressionFinder expressionFinder = GetExpressionFinder(fileName);
			if (expressionFinder == null)
				return ExpressionResult.Empty;
			if (caretColumn > document.GetLine(caretLine).Length)
				return ExpressionResult.Empty;
			return expressionFinder.FindFullExpression(document.Text, document.PositionToOffset(caretLine, caretColumn));
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
		public static ITextBuffer GetParseableFileContent(string fileName)
		{
			return Gui.WorkbenchSingleton.SafeThreadFunction(GetParseableFileContentInternal, fileName);
		}
		
		static ITextBuffer GetParseableFileContentInternal(string fileName)
		{
			//ITextBuffer res = project.GetParseableFileContent(fileName);
			//if (res != null)
			//	return res;
			
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
					return new StringTextBuffer(ICSharpCode.AvalonEdit.Utils.FileReader.ReadFileContent(s, DefaultFileEncoding));
				}
			}
			
			// load file
			return new StringTextBuffer(ICSharpCode.AvalonEdit.Utils.FileReader.ReadFileContent(fileName, DefaultFileEncoding));
		}
		#endregion
		
		#region Parse Information Management
		static readonly ICompilationUnit[] emptyCompilationUnitArray = new ICompilationUnit[0];
		
		sealed class FileEntry
		{
			readonly FileName fileName;
			internal readonly IParser parser;
			volatile ParseInformation parseInfo;
			ITextBufferVersion bufferVersion;
			ICompilationUnit[] oldUnits = emptyCompilationUnitArray;
			bool disposed;
			
			public FileEntry(FileName fileName)
			{
				this.fileName = fileName;
				this.parser = CreateParser(fileName);
			}
			
			/// <summary>
			/// for unit tests only
			/// </summary>
			public ParseInformation RegisterParseInformation(ICompilationUnit cu)
			{
				lock (this) {
					oldUnits = new ICompilationUnit[] { cu };
					return this.parseInfo = new ParseInformation(cu);
				}
			}
			
			public ParseInformation GetParseInformation(IProjectContent content)
			{
				ParseInformation p = GetExistingParseInformation(content);
				if (p != null)
					return p;
				else
					return ParseFile(content, null);
			}
			
			public ParseInformation GetExistingParseInformation(IProjectContent content)
			{
				if (content == null) {
					return this.parseInfo; // read volatile
				} else {
					ParseInformation p = this.parseInfo; // read volatile
					if (p != null && p.CompilationUnit.ProjectContent == content)
						return p;
					lock (this) {
						if (this.oldUnits != null) {
							ICompilationUnit cu = this.oldUnits.FirstOrDefault(c => c.ProjectContent == content);
							return cu != null ? new ParseInformation(cu) : null;
						} else {
							return null;
						}
					}
				}
			}
			
			public ParseInformation ParseFile(IProjectContent parentProjectContent, ITextBuffer fileContent)
			{
				if (parser == null)
					return null;
				
				if (fileContent == null) {
					// GetParseableFileContent must not be called inside any lock
					// (otherwise we'd risk deadlocks because GetParseableFileContent must invoke on the main thread)
					try {
						fileContent = GetParseableFileContent(fileName);
					} catch (System.Reflection.TargetInvocationException ex) {
						// It is possible that the file gets deleted/becomes inaccessible while a background parse
						// operation is enqueued, so we have to handle IO exceptions.
						if (ex.InnerException is IOException || ex.InnerException is UnauthorizedAccessException)
							return null;
						else
							throw;
					} catch (IOException) {
						return null;
					} catch (UnauthorizedAccessException) {
						return null;
					}
				}
				
				ITextBufferVersion fileContentVersion = fileContent.Version;
				List<IProjectContent> projectContents;
				lock (this) {
					if (this.disposed)
						return null;
					
					if (fileContentVersion != null && this.bufferVersion != null && this.bufferVersion.BelongsToSameDocumentAs(fileContentVersion)) {
						if (this.bufferVersion.CompareAge(fileContentVersion) >= 0) {
							// Special case: (necessary due to parentProjectContent optimization)
							// Detect when a file belongs to multiple projects but the ParserService hasn't realized
							// that, yet. In this case, do another parse run to detect all parent projects.
							if (!(parentProjectContent != null && this.oldUnits.Length == 1 && this.oldUnits[0].ProjectContent != parentProjectContent)) {
								return this.parseInfo;
							}
						}
					}
					
					if (parentProjectContent != null && (oldUnits.Length == 0 || (oldUnits.Length == 1 && oldUnits[0].ProjectContent == parentProjectContent))) {
						// Optimization: if parentProjectContent is specified and doesn't conflict with what we already know,
						// we will use it instead of doing an expensive GetProjectContents call.
						projectContents = new List<IProjectContent>();
						projectContents.Add(parentProjectContent);
					} else {
						projectContents = GetProjectContents(fileName);
					}
				}
				// We now leave the lock to do the actual parsing.
				// This is done to allow IParser implementations to invoke methods on the main thread without
				// risking deadlocks.
				
				// parse once for each project content that contains the file
				ICompilationUnit[] newUnits = new ICompilationUnit[projectContents.Count];
				ICompilationUnit resultUnit = null;
				for (int i = 0; i < newUnits.Length; i++) {
					IProjectContent pc = projectContents[i];
					try {
						newUnits[i] = parser.Parse(pc, fileName, fileContent);
					} catch (Exception ex) {
						throw new ApplicationException("Error parsing " + fileName, ex);
					}
					if (i == 0 || pc == parentProjectContent)
						resultUnit = newUnits[i];
				}
				lock (this) {
					if (this.disposed)
						return null;
					
					// ensure we never go backwards in time (we need to repeat this check after we've reacquired the lock)
					if (fileContentVersion != null && this.bufferVersion != null && this.bufferVersion.BelongsToSameDocumentAs(fileContentVersion)) {
						if (this.bufferVersion.CompareAge(fileContentVersion) >= 0) {
							if (parentProjectContent != null && parentProjectContent != parseInfo.CompilationUnit.ProjectContent) {
								ICompilationUnit oldUnit = oldUnits.FirstOrDefault(o => o.ProjectContent == parentProjectContent);
								if (oldUnit != null)
									return new ParseInformation(oldUnit);
							}
							return this.parseInfo;
						}
					}
					
					ParseInformation newParseInfo = new ParseInformation(resultUnit);
					
					for (int i = 0; i < newUnits.Length; i++) {
						IProjectContent pc = projectContents[i];
						// update the compilation unit
						ICompilationUnit oldUnit = oldUnits.FirstOrDefault(o => o.ProjectContent == pc);
						pc.UpdateCompilationUnit(oldUnit, newUnits[i], fileName);
						ParseInformation newUnitParseInfo = (newUnits[i] == resultUnit) ? newParseInfo : new ParseInformation(newUnits[i]);
						RaiseParseInformationUpdated(new ParseInformationEventArgs(fileName, pc, oldUnit, newUnitParseInfo, newUnits[i] == resultUnit));
					}
					
					// remove all old units that don't exist anymore
					foreach (ICompilationUnit oldUnit in oldUnits) {
						if (!newUnits.Any(n => n.ProjectContent == oldUnit.ProjectContent)) {
							oldUnit.ProjectContent.RemoveCompilationUnit(oldUnit);
							RaiseParseInformationUpdated(new ParseInformationEventArgs(fileName, oldUnit.ProjectContent, oldUnit, null, false));
						}
					}
					
					this.bufferVersion = fileContentVersion;
					this.oldUnits = newUnits;
					this.parseInfo = newParseInfo;
					return newParseInfo;
				}
			}
			
			public void Clear()
			{
				ParseInformation parseInfo;
				ICompilationUnit[] oldUnits;
				lock (this) {
					// by setting the disposed flag, we'll cause all running ParseFile() calls to return null and not
					// call into the parser anymore, so we can do the remainder of the clean-up work outside the lock
					this.disposed = true;
					parseInfo = this.parseInfo;
					oldUnits = this.oldUnits;
					this.oldUnits = null;
					this.bufferVersion = null;
					this.parseInfo = null;
				}
				foreach (ICompilationUnit oldUnit in oldUnits) {
					oldUnit.ProjectContent.RemoveCompilationUnit(oldUnit);
					bool isPrimary = parseInfo != null && parseInfo.CompilationUnit == oldUnit;
					RaiseParseInformationUpdated(new ParseInformationEventArgs(fileName, oldUnit.ProjectContent, oldUnit, null, isPrimary));
				}
			}
			
			public Task<ParseInformation> BeginParse(ITextBuffer fileContent)
			{
				return System.Threading.Tasks.Task.Factory.StartNew(
					delegate {
						try {
							return ParseFile(null, fileContent);
						} catch (Exception ex) {
							MessageService.ShowException(ex, "Error during async parse");
							return null;
						}
					}
				);
			}
		}
		
		static FileEntry GetFileEntry(string fileName, bool createOnDemand)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			FileName f = new FileName(fileName);
			
			FileEntry entry;
			lock (syncLock) {
				if (!fileEntryDict.TryGetValue(f, out entry)) {
					if (!createOnDemand)
						return null;
					entry = new FileEntry(f);
					fileEntryDict.Add(f, entry);
				}
			}
			return entry;
		}
		
		/// <summary>
		/// Removes all parse information stored for the specified file.
		/// This method is thread-safe.
		/// </summary>
		public static void ClearParseInformation(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			
			FileName f = new FileName(fileName);
			LoggingService.Info("ClearParseInformation: " + f);
			
			FileEntry entry;
			lock (syncLock) {
				if (fileEntryDict.TryGetValue(f, out entry)) {
					fileEntryDict.Remove(f);
				}
			}
			if (entry != null)
				entry.Clear();
		}
		
		/// <summary>
		/// Gets parse information for the specified file.
		/// Blocks if the file wasn't parsed yet, but may return an old parsed version.
		/// This method is thread-safe. This method involves waiting for the main thread, so using it while
		/// holding a lock can lead to deadlocks. You might want to use <see cref="GetExistingParseInformation"/> instead.
		/// </summary>
		/// <returns>Returns the ParseInformation for the specified file, or null if the file cannot be parsed.
		/// The returned ParseInformation might be stale (re-parse is not forced).</returns>
		public static ParseInformation GetParseInformation(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return null;
			return GetFileEntry(fileName, true).GetParseInformation(null);
		}
		
		/// <summary>
		/// Gets parse information for the specified file.
		/// This method is thread-safe.
		/// </summary>
		/// <returns>Returns the ParseInformation for the specified file, or null if the file has not been parsed already.</returns>
		public static ParseInformation GetExistingParseInformation(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return null;
			FileEntry entry = GetFileEntry(fileName, false);
			if (entry != null)
				return entry.GetExistingParseInformation(null);
			else
				return null;
		}
		
		/// <summary>
		/// Gets parse information for the specified file in the context of the
		/// specified project content.
		/// This method is thread-safe.
		/// </summary>
		/// <returns>Returns the ParseInformation for the specified file, or null if the file has not been parsed for that project content.</returns>
		public static ParseInformation GetExistingParseInformation(IProjectContent content, string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return null;
			FileEntry entry = GetFileEntry(fileName, false);
			if (entry != null)
				return entry.GetExistingParseInformation(content);
			else
				return null;
		}
		
		/// <summary>
		/// Gets parse information for the specified file.
		/// Blocks until a recent copy of the parse information is available.
		/// This method is thread-safe. This method involves waiting for the main thread, so using it while
		/// holding a lock can lead to deadlocks. You might want to use the overload taking ITextBuffer instead.
		/// </summary>
		/// <returns>Returns the ParseInformation for the specified file, or null if the file cannot be parsed.
		/// The returned ParseInformation will not be stale (re-parse is forced if required).</returns>
		public static ParseInformation ParseFile(string fileName)
		{
			return GetFileEntry(fileName, true).ParseFile(null, null);
		}
		
		/// <summary>
		/// Gets parse information for the specified file.
		/// The fileContent is taken as a hint - if a newer version than it is already available, that will be used instead.
		/// This method is thread-safe.
		/// </summary>
		/// <returns>Returns the ParseInformation for the specified file, or null if the file cannot be parsed.
		/// The returned ParseInformation will not be stale (re-parse is forced if required).</returns>
		public static ParseInformation ParseFile(string fileName, ITextBuffer fileContent)
		{
			if (fileContent == null)
				throw new ArgumentNullException("fileContent");
			return GetFileEntry(fileName, true).ParseFile(null, fileContent);
		}
		
		/// <summary>
		/// Gets parse information for the specified file.
		/// The fileContent is taken as a hint - if a newer version than it is already available, that will be used instead.
		/// This method is thread-safe.
		/// </summary>
		/// <returns>Returns the ParseInformation for the specified file, or null if the file cannot be parsed.
		/// The returned ParseInformation will not be stale (re-parse is forced if required).</returns>
		public static ParseInformation ParseFile(IProjectContent parentProjectContent, string fileName, ITextBuffer fileContent)
		{
			if (fileContent == null)
				throw new ArgumentNullException("fileContent");
			return GetFileEntry(fileName, true).ParseFile(parentProjectContent, fileContent);
		}
		
		/// <summary>
		/// Begins an asynchronous reparse.
		/// This method is thread-safe. The returned task might wait for the main thread to be ready, beware of deadlocks.
		/// You might want to use the overload taking ITextBuffer instead.
		/// </summary>
		/// <returns>
		/// Returns a task that will make the parse result available.
		/// </returns>
		/// <remarks>
		/// EnqueueForParsing has been renamed to BeginParse and now provides a future (Task&lt;ParseInformation&gt;)
		/// to allow waiting for the result. However, to avoid deadlocks, this should not be done by any
		/// thread the parser might be waiting for  (especially the main thread).
		/// 
		/// Unlike BeginParse().Wait(), ParseFile() is safe to call from the main thread.
		/// </remarks>
		public static Task<ParseInformation> BeginParse(string fileName)
		{
			return GetFileEntry(fileName, true).BeginParse(null);
		}
		
		/// <summary>
		/// Begins an asynchronous reparse.
		/// This method is thread-safe.
		/// </summary>
		/// <returns>
		/// Returns a task that will make the parse result available.
		/// </returns>
		/// <remarks>
		/// EnqueueForParsing has been renamed to BeginParse and now provides a future (Task&lt;ParseInformation&gt;)
		/// to allow waiting for the result. However, to avoid deadlocks, this should not be done by any
		/// thread the parser might be waiting for  (especially the main thread).
		/// 
		/// Unlike BeginParse().Wait(), ParseFile() is safe to call from the main thread.
		/// </remarks>
		public static Task<ParseInformation> BeginParse(string fileName, ITextBuffer fileContent)
		{
			if (fileContent == null)
				throw new ArgumentNullException("fileContent");
			// create snapshot (in case someone passes a mutable document to BeginParse)
			return GetFileEntry(fileName, true).BeginParse(fileContent.CreateSnapshot());
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
				return ParseFile(viewContent.PrimaryFileName, editable.CreateSnapshot());
			else
				return ParseFile(viewContent.PrimaryFileName);
		}
		
		/// <summary>
		/// Parses the current view content.
		/// This method can only be called from the main thread.
		/// </summary>
		/// <remarks>
		/// EnqueueForParsing has been renamed to BeginParse and now provides a future (Task&lt;ParseInformation&gt;)
		/// to allow waiting for the result. However, to avoid deadlocks, this should not be done by any
		/// thread the parser might be waiting for  (especially the main thread).
		/// 
		/// Unlike BeginParse().Wait(), ParseFile() is safe to call from the main thread.
		/// </remarks>
		public static Task<ParseInformation> BeginParseCurrentViewContent()
		{
			WorkbenchSingleton.AssertMainThread();
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (viewContent != null)
				return BeginParseViewContent(viewContent);
			else
				return NullTask();
		}
		
		/// <summary>
		/// Begins parsing the specified view content.
		/// This method can only be called from the main thread.
		/// </summary>
		/// <remarks>
		/// EnqueueForParsing has been renamed to BeginParse and now provides a future (Task&lt;ParseInformation&gt;)
		/// to allow waiting for the result. However, to avoid deadlocks, this should not be done by any
		/// thread the parser might be waiting for  (especially the main thread).
		/// 
		/// Unlike BeginParse().Wait(), ParseFile() is safe to call from the main thread.
		/// </remarks>
		public static Task<ParseInformation> BeginParseViewContent(IViewContent viewContent)
		{
			if (viewContent == null)
				throw new ArgumentNullException("viewContent");
			WorkbenchSingleton.AssertMainThread();
			if (string.IsNullOrEmpty(viewContent.PrimaryFileName))
				return NullTask();
			IEditable editable = viewContent as IEditable;
			if (editable != null)
				return BeginParse(viewContent.PrimaryFileName, editable.CreateSnapshot());
			else
				return BeginParse(viewContent.PrimaryFileName);
		}
		
		static Task<ParseInformation> NullTask()
		{
			return System.Threading.Tasks.Task.Factory.StartNew<ParseInformation>(
				delegate { return null; }
			);
		}
		
		
		/// <summary>
		/// Gets the parser instance that is responsible for the specified file.
		/// Will create a new IParser instance on demand.
		/// This method is thread-safe.
		/// </summary>
		public static IParser GetParser(string fileName)
		{
			return GetFileEntry(fileName, true).parser;
		}
		
		/// <summary>
		/// Registers a compilation unit in the parser service.
		/// Does not fire the OnParseInformationUpdated event, please use this for unit tests only!
		/// </summary>
		public static ParseInformation RegisterParseInformation(string fileName, ICompilationUnit cu)
		{
			FileEntry entry = GetFileEntry(fileName, true);
			return entry.RegisterParseInformation(cu);
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
					LoggingService.Debug("ParseInformationUpdated " + e.FileName + " new!=null:" + (e.NewCompilationUnit!=null));
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
		
		public static void Reparse(IProject project, bool initReferences, bool parseCode)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			LoadSolutionProjects.Reparse(project, initReferences, parseCode);
		}
		
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
			List<ParseProjectContent> createdContents = new List<ParseProjectContent>();
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				try {
					LoggingService.Debug("Creating project content for " + project.Name);
					ParseProjectContent newContent = project.CreateProjectContent();
					if (newContent != null) {
						lock (projectContents) {
							projectContents[project] = newContent;
						}
						createdContents.Add(newContent);
					}
				} catch (Exception e) {
					MessageService.ShowException(e, "Error while retrieving project contents from " + project);
				}
			}
			LoadSolutionProjects.OnSolutionLoaded(createdContents);
		}
		
		internal static void OnSolutionClosed()
		{
			LoadSolutionProjects.OnSolutionClosed();
			lock (projectContents) {
				foreach (IProjectContent content in projectContents.Values) {
					content.Dispose();
				}
				projectContents.Clear();
			}
			ClearAllFileEntries();
		}
		
		static void ClearAllFileEntries()
		{
			FileEntry[] entries;
			lock (fileEntryDict) {
				entries = fileEntryDict.Values.ToArray();
				fileEntryDict.Clear();
			}
			foreach (FileEntry entry in entries)
				entry.Clear();
		}
		
		/// <remarks>Can return null.</remarks>
		internal static IProjectContent CreateProjectContentForAddedProject(IProject project)
		{
			ParseProjectContent newContent = project.CreateProjectContent();
			if (newContent != null) {
				lock (projectContents) {
					projectContents[project] = newContent;
				}
				LoadSolutionProjects.InitNewProject(newContent);
			}
			return newContent;
		}
		#endregion
	}
}
