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
using ICSharpCode.Editor;
using ICSharpCode.NRefactory.CSharp.Resolver;
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
		static Dictionary<IProjectContent, IProject> projectContents = new Dictionary<IProjectContent, IProject>();
		static Dictionary<FileName, FileEntry> fileEntryDict = new Dictionary<FileName, FileEntry>();
		
		#region Manage Project Contents
		/// <summary>
		/// Gets the project content for the current project.
		/// </summary>
		/// <remarks>
		/// This property is thread-safe; but that the notion of 'current project'
		/// might not be meaningful on threads other than the main thread.
		/// </remarks>
		public static IProjectContent CurrentProjectContent {
			[DebuggerStepThrough]
			get {
				IProject currentProject = ProjectService.CurrentProject;
				if (currentProject != null)
					return currentProject.ProjectContent;
				else
					return DefaultProjectContent;
			}
		}
		
		/// <summary>
		/// Gets the type resolve context for the current project.
		/// </summary>
		/// <remarks>
		/// To improve performance and ensure the returned data is consistent, use the following code pattern:
		/// <code>
		/// using (var context = ParserService.CurrentTypeResolveContext.Synchronize()) {
		/// 	...
		/// }
		/// </code>
		/// 
		/// This property is thread-safe; but the notion of 'current project'
		/// might not be meaningful on threads other than the main thread.
		/// </remarks>
		public static ITypeResolveContext CurrentTypeResolveContext {
			get {
				IProject currentProject = ProjectService.CurrentProject;
				if (currentProject != null) {
					return currentProject.TypeResolveContext ?? GetDefaultTypeResolveContext();
				} else {
					return GetDefaultTypeResolveContext();
				}
			}
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
			lock (syncLock) {
				IProject project;
				if (projectContents.TryGetValue(projectContent, out project))
					return project;
				else
					return null;
			}
		}
		
		/// <summary>
		/// Gets the list of project contents of all open projects. Does not include assembly project contents.
		/// </summary>
		public static IEnumerable<IProjectContent> AllProjectContents {
			get {
				lock (syncLock) {
					return projectContents.Keys.ToArray();
				}
			}
		}
		
		/// <summary>
		/// Gets all project contents that contain the specified file.
		/// </summary>
		static List<IProjectContent> GetProjectContents(string fileName)
		{
			List<IProjectContent> result = new List<IProjectContent>();
			List<IProjectContent> linkResults = new List<IProjectContent>();
			
			KeyValuePair<IProjectContent, IProject>[] pairs;
			lock (syncLock) {
				pairs = projectContents.ToArray();
			}
			foreach (var pair in pairs) {
				FileProjectItem file = pair.Value.FindFile(fileName);
				if (file != null) {
					// Prefer normal files over linked files.
					// The order matters because GetParseInformation() will return the ICompilationUnit
					// for the first result.
					if (file.IsLink)
						linkResults.Add(pair.Key);
					else
						result.Add(pair.Key);
				}
			}
			result.AddRange(linkResults);
			if (result.Count == 0)
				result.Add(DefaultProjectContent);
			return result;
		}
		
		internal static void RegisterProjectContentForAddedProject(IProject project)
		{
			IProjectContent newContent = project.ProjectContent;
			if (newContent != null) {
				lock (syncLock) {
					projectContents[newContent] = project;
				}
			}
		}
		
		internal static void RemoveProjectContentForRemovedProject(IProject project)
		{
			lock (syncLock) {
				foreach (var pair in projectContents.ToArray()) {
					if (pair.Value == project)
						projectContents.Remove(pair.Key);
				}
			}
		}
		#endregion
		
		#region Default Project Content
		static readonly SimpleProjectContent defaultProjectContent = new SimpleProjectContent();
		static readonly Lazy<Task<IProjectContent>[]> defaultReferences = new Lazy<Task<IProjectContent>[]>(
			delegate {
				string[] assemblies = {
					"mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
					"System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
					"System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
					"System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
					"System.Xml.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
					"System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
					"Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
				};
				List<Task<IProjectContent>> tasks = new List<Task<IProjectContent>>();
				foreach (string assemblyName in assemblies) {
					DomAssemblyName name = new DomAssemblyName(assemblyName);
					string fileName = AssemblyParserService.FindReferenceAssembly(name.ShortName);
					if (fileName == null) {
						fileName = GacInterop.FindAssemblyInNetGac(name);
					}
					if (fileName != null) {
						tasks.Add(AssemblyParserService.GetAssemblyAsync(FileName.Create(fileName)));
					}
				}
				return tasks.ToArray();
			});
		
		/// <summary>
		/// Gets the default project content used for files outside of projects.
		/// </summary>
		public static IProjectContent DefaultProjectContent {
			get { return defaultProjectContent; }
		}
		
		/// <summary>
		/// Gets the type resolve context for the default project content.
		/// </summary>
		static ITypeResolveContext GetDefaultTypeResolveContext()
		{
			List<ITypeResolveContext> references = new List<ITypeResolveContext>();
			references.Add(defaultProjectContent);
			foreach (var task in defaultReferences.Value) {
				if (task.IsCompleted && !task.IsFaulted)
					references.Add(task.Result);
			}
			return new CompositeTypeResolveContext(references);
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
			if (GetParser(fileName) == null)
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
		static readonly IParsedFile[] emptyCompilationUnitArray = new IParsedFile[0];
		
		sealed class FileEntry
		{
			readonly FileName fileName;
			internal readonly IParser parser;
			volatile IParsedFile mainParsedFile;
			volatile ParseInformation cachedParseInformation;
			ITextSourceVersion bufferVersion;
			IParsedFile[] oldUnits = emptyCompilationUnitArray;
			bool disposed;
			
			public FileEntry(FileName fileName)
			{
				this.fileName = fileName;
				this.parser = CreateParser(fileName);
			}
			
			/// <summary>
			/// Intended for unit tests only
			/// </summary>
			public void RegisterParseInformation(IParsedFile cu)
			{
				lock (this) {
					this.oldUnits = new IParsedFile[] { cu };
					this.mainParsedFile = cu;
				}
			}
			
			public ParseInformation GetCachedParseInformation()
			{
				return cachedParseInformation; // read volatile
			}
			
			public IParsedFile GetExistingParsedFile(IProjectContent content)
			{
				if (content == null) {
					return this.mainParsedFile; // read volatile
				} else {
					IParsedFile p = this.mainParsedFile; // read volatile
					if (p != null && p.ProjectContent == content)
						return p;
					lock (this) {
						if (this.oldUnits != null) {
							IParsedFile cu = this.oldUnits.FirstOrDefault(c => c.ProjectContent == content);
							return cu;
						} else {
							return null;
						}
					}
				}
			}
			
			public ParseInformation Parse(ITextSource fileContent)
			{
				if (fileContent == null) {
					fileContent = WorkbenchSingleton.SafeThreadFunction(GetParseableFileContentForOpenFile, fileName);
				}
				
				ParseInformation parseInfo;
				IParsedFile parsedFile;
				DoParse(null, fileContent, true, out parseInfo, out parsedFile);
				return parseInfo;
			}
			
			public IParsedFile ParseFile(IProjectContent parentProjectContent, ITextSource fileContent)
			{
				if (fileContent == null) {
					fileContent = WorkbenchSingleton.SafeThreadFunction(GetParseableFileContentForOpenFile, fileName);
				}
				
				ParseInformation parseInfo;
				IParsedFile parsedFile;
				DoParse(parentProjectContent, fileContent, false, out parseInfo, out parsedFile);
				return parsedFile;
			}
			
			void DoParse(IProjectContent parentProjectContent, ITextSource fileContent,
			             bool fullParseInformationRequested, out ParseInformation resultParseInfo, out IParsedFile resultUnit)
			{
				resultParseInfo = null;
				resultUnit = null;
				
				if (parser == null)
					return;
				
				if (fileContent == null) {
					// GetParseableFileContent must not be called inside any lock
					// (otherwise we'd risk deadlocks because GetParseableFileContent must invoke on the main thread)
					fileContent = GetParseableFileContent(fileName);

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
				List<IProjectContent> projectContents;
				lock (this) {
					if (this.disposed)
						return;
					
					if (fileContentVersion != null && this.bufferVersion != null && this.bufferVersion.BelongsToSameDocumentAs(fileContentVersion)) {
						if (this.bufferVersion.CompareAge(fileContentVersion) >= 0) {
							// Special case: (necessary due to parentProjectContent optimization below)
							// Detect when a file belongs to multiple projects but the ParserService hasn't realized
							// that, yet. In this case, do another parse run to detect all parent projects.
							if (!(parentProjectContent != null && this.oldUnits.Length == 1 && this.oldUnits[0].ProjectContent != parentProjectContent)) {
								// If full parse info is requested, ensure we have full parse info.
								if (!(fullParseInformationRequested && this.cachedParseInformation == null)) {
									resultParseInfo = this.cachedParseInformation;
									resultUnit = this.mainParsedFile;
									if (parentProjectContent != null) {
										foreach (var oldUnit in this.oldUnits) {
											if (oldUnit.ProjectContent == parentProjectContent) {
												resultUnit = oldUnit;
												break;
											}
										}
									}
									return;
								}
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
				} // exit lock
				
				// We now leave the lock to do the actual parsing.
				// This is done to allow IParser implementations to invoke methods on the main thread without
				// risking deadlocks.
				
				// parse once for each project content that contains the file
				ParseInformation[] newParseInfo = new ParseInformation[projectContents.Count];
				IParsedFile[] newUnits = new IParsedFile[projectContents.Count];
				for (int i = 0; i < projectContents.Count; i++) {
					IProjectContent pc = projectContents[i];
					try {
						newParseInfo[i] = parser.Parse(pc, fileName, fileContent, fullParseInformationRequested);
					} catch (Exception ex) {
						throw new ApplicationException("Error parsing " + fileName, ex);
					}
					if (newParseInfo[i] == null)
						throw new NullReferenceException(parser.GetType().Name + ".Parse() returned null");
					if (fullParseInformationRequested && !newParseInfo[i].IsFullParseInformation)
						throw new InvalidOperationException(parser.GetType().Name + ".Parse() did not return full parse info as requested.");
					
					newUnits[i] = newParseInfo[i].ParsedFile;
					if (i == 0 || pc == parentProjectContent) {
						resultParseInfo = newParseInfo[i];
						resultUnit = newUnits[i];
					}
				}
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
								// Thus, we return the parse information that we found above,
								// but we won't register it.
								return;
							} else {
								resultParseInfo = this.cachedParseInformation;
								resultUnit = this.mainParsedFile;
								if (parentProjectContent != null) {
									foreach (var oldUnit in this.oldUnits) {
										if (oldUnit.ProjectContent == parentProjectContent) {
											resultUnit = oldUnit;
											break;
										}
									}
								}
								return;
							}
						}
					}
					
					for (int i = 0; i < newUnits.Length; i++) {
						IProjectContent pc = projectContents[i];
						// update the compilation unit
						IParsedFile oldUnit = oldUnits.FirstOrDefault(o => o.ProjectContent == pc);
						// ensure the new unit is frozen beforewe make it visible to the outside world
						newUnits[i].Freeze();
						pc.UpdateProjectContent(oldUnit, newUnits[i]);
						RaiseParseInformationUpdated(new ParseInformationEventArgs(oldUnit, newParseInfo[i], newParseInfo[i] == resultParseInfo));
					}
					
					// remove all old units that don't exist anymore
					foreach (IParsedFile oldUnit in oldUnits) {
						if (!newUnits.Any(n => n.ProjectContent == oldUnit.ProjectContent)) {
							oldUnit.ProjectContent.UpdateProjectContent(oldUnit, null);
							RaiseParseInformationUpdated(new ParseInformationEventArgs(oldUnit, null, false));
						}
					}
					
					this.bufferVersion = fileContentVersion;
					this.oldUnits = newUnits;
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
				IParsedFile[] oldUnits;
				lock (this) {
					// by setting the disposed flag, we'll cause all running ParseFile() calls to return null and not
					// call into the parser anymore, so we can do the remainder of the clean-up work outside the lock
					this.disposed = true;
					parseInfo = this.mainParsedFile;
					oldUnits = this.oldUnits;
					this.oldUnits = null;
					this.bufferVersion = null;
					this.mainParsedFile = null;
				}
				foreach (IParsedFile oldUnit in oldUnits) {
					oldUnit.ProjectContent.UpdateProjectContent(oldUnit, null);
					bool isPrimary = parseInfo == oldUnit;
					RaiseParseInformationUpdated(new ParseInformationEventArgs(oldUnit, null, isPrimary));
				}
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
			
			public Task<ParseInformation> ParseAsync(ITextSource fileContent)
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
							DoParse(null, fileContent, true, out parseInfo, out parsedFile);
							return parseInfo;
						} catch (Exception ex) {
							MessageService.ShowException(ex, "Error during async parse");
							return null;
						}
					}
				);
			}
			
			public Task<IParsedFile> ParseFileAsync(IProjectContent parentProjectContent, ITextSource fileContent)
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
							DoParse(parentProjectContent, fileContent, false, out parseInfo, out parsedFile);
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
		/// Gets parse information for the specified file.
		/// </summary>
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
				return entry.GetExistingParsedFile(null);
			else
				return null;
		}
		
		/// <summary>
		/// Gets parse information for the specified file in the context of the
		/// specified project content.
		/// </summary>
		/// <param name="parentProjectContent">
		/// Project content to use as a parent project for the parse run.
		/// Specifying the project content explicitly can be useful when a file is used in multiple projects.
		/// </param>
		/// <param name="fileName">Name of the file.</param>
		/// <returns>
		/// Returns the IParsedFile for the specified file,
		/// or null if the file has not been parsed for that project content.
		/// </returns>
		/// <remarks>This method is thread-safe.</remarks>
		public static IParsedFile GetExistingParsedFile(IProjectContent parentProjectContent, FileName fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return null;
			FileEntry entry = GetFileEntry(fileName, false);
			if (entry != null)
				return entry.GetExistingParsedFile(parentProjectContent);
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
			return GetFileEntry(fileName, true).ParseFile(null, fileContent);
		}
		
		/// <summary>
		/// Parses the specified file.
		/// This method does not request full parse information
		/// </summary>
		/// <param name="parentProjectContent">
		/// Project content to use as a parent project for the parse run.
		/// Specifying the project content explicitly can be useful when a file is used in multiple projects.
		/// </param>
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
		public static IParsedFile ParseFile(IProjectContent parentProjectContent, FileName fileName, ITextSource fileContent = null)
		{
			return GetFileEntry(fileName, true).ParseFile(parentProjectContent, fileContent);
		}
		
		/// <summary>
		/// Async version of ParseFile().
		/// </summary>
		public static Task<IParsedFile> ParseFileAsync(FileName fileName, ITextSource fileContent = null)
		{
			return GetFileEntry(fileName, true).ParseFileAsync(null, fileContent);
		}
		
		/// <summary>
		/// Async version of ParseFile().
		/// </summary>
		public static Task<IParsedFile> ParseFileAsync(IProjectContent parentProjectContent, FileName fileName, ITextSource fileContent = null)
		{
			return GetFileEntry(fileName, true).ParseFileAsync(parentProjectContent, fileContent);
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
		public static void RegisterParseInformation(string fileName, IParsedFile cu)
		{
			FileEntry entry = GetFileEntry(FileName.Create(fileName), true);
			entry.RegisterParseInformation(cu);
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
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				RegisterProjectContentForAddedProject(project);
			}
		}
		
		internal static void OnSolutionClosed()
		{
			LoadSolutionProjects.CancelAllJobs();
			lock (syncLock) {
				projectContents.Clear();
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
			IProject project = GetProject(parseInfo.ProjectContent);
			var context = project != null ? project.TypeResolveContext : GetDefaultTypeResolveContext();
			ResolveResult rr;
			using (var ctx = context.Synchronize()) {
				rr = entry.parser.Resolve(parseInfo, location, ctx, cancellationToken);
			}
			LoggingService.Debug("Resolved " + location + " to " + rr);
			return rr;
		}
		
		public static Task<ResolveResult> ResolveAsync(FileName fileName, TextLocation location, ITextSource fileContent = null,
		                                               CancellationToken cancellationToken = default(CancellationToken))
		{
			var entry = GetFileEntry(fileName, true);
			if (entry.parser == null)
				return NullTask<ResolveResult>();
			return entry.ParseAsync(fileContent).ContinueWith(
				delegate (Task<ParseInformation> parseInfoTask) {
					var parseInfo = parseInfoTask.Result;
					if (parseInfo == null)
						return null;
					IProject project = GetProject(parseInfo.ProjectContent);
					var context = project != null ? project.TypeResolveContext : GetDefaultTypeResolveContext();
					ResolveResult rr;
					using (var ctx = context.Synchronize()) {
						rr = entry.parser.Resolve(parseInfo, location, ctx, cancellationToken);
					}
					LoggingService.Debug("Resolved " + location + " to " + rr);
					return rr;
				}, cancellationToken);
		}
		
		static Task<T> NullTask<T>() where T : class
		{
			TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
			tcs.SetResult(null);
			return tcs.Task;
		}
	}
}
